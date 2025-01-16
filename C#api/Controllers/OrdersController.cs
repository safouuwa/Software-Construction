using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;
using Providers;
using Interfaces;
using Attributes;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : BaseApiController, ILoggableAction
{
    public OrdersController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }
    public object _dataBefore { get; set; }
    public object _dataAfter { get; set; }



    [HttpGet]
    public IActionResult GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth is UnauthorizedResult) return auth;

        var orders = DataProvider.fetch_order_pool().GetOrders();

        if (auth is OkResult) 
        {
            var user = AuthProvider.GetUser(Request.Headers["API_KEY"]);
            orders = orders.Where(o => user.OwnWarehouses.Contains(o.Warehouse_Id)).ToList();
        }

        var response = PaginationHelper.Paginate(orders, page, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "getsingle");
        if (auth != null) return auth;

        var order = DataProvider.fetch_order_pool().GetOrder(id);
        if (order == null) return NoContent();

        return Ok(order);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetOrderItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_order_pool().GetItemsInOrder(id);
        return Ok(items);
    }

    [HttpGet("{id}/status")]
    public IActionResult GetOrderStatus(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "getsingle");
        if (auth != null) return auth;

        var order = DataProvider.fetch_order_pool().GetOrder(id);
        if (order == null) return NoContent();

        return Ok(order.Order_Status);
    }

    [HttpGet("search")]
    public IActionResult SearchOrders(
        [FromQuery] int? sourceId = null,
        [FromQuery] string orderStatus = null,
        [FromQuery] string orderDate = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth != null) return auth;

        try
        {
            var orders = DataProvider.fetch_order_pool().SearchOrders(
                sourceId, 
                orderStatus, 
                orderDate,  
                warehouseId);

            if (orders == null || !orders.Any())
            {
                return NoContent();
            }

            var response = PaginationHelper.Paginate(orders, page, pageSize);
            return Ok(orders);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/warehouse")]
    public IActionResult GetOrderWarehouse(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth is UnauthorizedResult) return auth;

        var order = DataProvider.fetch_order_pool().GetOrder(id);
        if (order == null)
        {
            return BadRequest("Invalid order ID");
        }

        var warehouse = DataProvider.fetch_warehouse_pool().GetWarehouse(order.Warehouse_Id);
        if (warehouse == null) return NoContent();

        return Ok(warehouse);
}

    [LogRequest]
    [HttpPost]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "post");
        if (auth != null) return auth;
        if (order.Id != null) return BadRequest("Order: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_order_pool().AddOrder(order);
        if (!success) return BadRequest("Order: Id already exists");

        _dataBefore = null;
        _dataAfter = order;


        DataProvider.fetch_order_pool().Save();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }
    [LogRequest]
    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, [FromBody] Order order)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "put");
        if (auth != null) return auth;

        if (order.Id != null) return BadRequest("Order: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_order_pool().UpdateOrder(id, order);
        if (!success) return NoContent();

        _dataBefore = DataProvider.fetch_order_pool().GetOrder(id);
        _dataAfter = order;

        DataProvider.fetch_order_pool().Save();
        return Ok();
    }
    [LogRequest]
    [HttpPut("{id}/items")]
    public IActionResult UpdateOrderItems(int id, [FromBody] List<Item> items)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "put");
        if (auth != null) return auth;

        if (items.Any(x => x.Uid == null)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_order_pool().GetOrder(id) == null)
            return NoContent();
        
        _dataBefore = DataProvider.fetch_order_pool().GetItemsInOrder(id);
        _dataAfter = items;

        DataProvider.fetch_order_pool().UpdateItemsInOrder(id, items);
        DataProvider.fetch_order_pool().Save();
        return Ok();
    }

    [HttpPut("{id}/commit")]
    public IActionResult CommitOrder(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "put");
        if (auth != null) return auth;

        var order = DataProvider.fetch_order_pool().GetOrder(id);
        if (order == null) return NoContent();

        foreach (var item in order.Items)
        {
            var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(item.Item_Id);
            foreach (var inventory in inventories)
            {
                if (inventory.Locations.Contains(order.Source_Id))
                {
                    inventory.Total_On_Hand -= item.Amount;
                }
            }
        }

        order.Order_Status = "Processed";
        var success = DataProvider.fetch_order_pool().UpdateOrder(id, order);
        if (!success) return NoContent();

        _notificationSystem.Push($"Order with id: {order.Id} has been processed.");
        DataProvider.fetch_order_pool().Save();
        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }
    [LogRequest]
    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateClient(int id, [FromBody] JsonElement partialOrder)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "patch");
        if (auth != null) return auth;

        if (partialOrder.ValueKind == JsonValueKind.Undefined)
            return BadRequest("No updates provided");

        var orderPool = DataProvider.fetch_order_pool();
        var existingOrder = orderPool.GetOrder(id);
        var originalFields = new Dictionary<string, object>();

        if (existingOrder == null) 
            return NoContent();

        if (partialOrder.TryGetProperty("Source_id", out var sourceId))
        {
            originalFields["Source_id"] = existingOrder.Source_Id;
            existingOrder.Source_Id = sourceId.GetInt32();
        }

        if (partialOrder.TryGetProperty("Order_date", out var orderDate))
        {
            originalFields["Order_date"] = existingOrder.Order_Date;
            existingOrder.Order_Date = orderDate.GetString();
        }

        if (partialOrder.TryGetProperty("Request_date", out var requestDate))
        {
            originalFields["Request_date"] = existingOrder.Request_Date;
            existingOrder.Request_Date = requestDate.GetString();
        }

        if(partialOrder.TryGetProperty("Reference", out var reference))
        {
            originalFields["Reference"] = existingOrder.Reference;
            existingOrder.Reference = reference.GetString();
        }

        if (partialOrder.TryGetProperty("Reference_extra", out var referenceExtra))
        {
            originalFields["Reference_extra"] = existingOrder.Reference_Extra;
            existingOrder.Reference_Extra = referenceExtra.GetString();
        }
        
        if (partialOrder.TryGetProperty("Order_status", out var orderStatus))
        {
            originalFields["Order_status"] = existingOrder.Order_Status;
            existingOrder.Order_Status = orderStatus.GetString();
        }

        if (partialOrder.TryGetProperty("Notes", out var notes))
        {
            originalFields["Notes"] = existingOrder.Notes;
            existingOrder.Notes = notes.GetString();
        }

        if (partialOrder.TryGetProperty("Shipping_notes", out var shippingNotes))
        {
            originalFields["Shipping_notes"] = existingOrder.Shipping_Notes;
            existingOrder.Shipping_Notes = shippingNotes.GetString();
        }

        if (partialOrder.TryGetProperty("Picking_notes", out var pickingNotes))
        {
            originalFields["Picking_notes"] = existingOrder.Picking_Notes;
            existingOrder.Picking_Notes = pickingNotes.GetString();
        }

        if (partialOrder.TryGetProperty("Warehouse_id", out var warehouseId))
        {
            originalFields["Warehouse_id"] = existingOrder.Warehouse_Id;
            existingOrder.Warehouse_Id = warehouseId.GetInt32();
        }

        if (partialOrder.TryGetProperty("Ship_to", out var shipTo))
        {
            originalFields["Ship_to"] = existingOrder.Ship_To;
            existingOrder.Ship_To = shipTo.GetInt32();
        }

        if (partialOrder.TryGetProperty("Bill_to", out var billTo))
        {
            originalFields["Bill_to"] = existingOrder.Bill_To;
            existingOrder.Bill_To = billTo.GetInt32();
        }

        if (partialOrder.TryGetProperty("Shipment_id", out var shipmentId))
        {
            originalFields["Shipment_id"] = existingOrder.Shipment_Id;
            existingOrder.Shipment_Id = shipmentId.GetInt32();
        }

        if (partialOrder.TryGetProperty("Total_Amount", out var totalAmount))
        {
            originalFields["Total_Amount"] = existingOrder.Total_Amount;
            existingOrder.Total_Amount = totalAmount.GetDecimal();
        }

        if (partialOrder.TryGetProperty("Total_Discount", out var totalDiscount))
        {
            originalFields["Total_Discount"] = existingOrder.Total_Discount;
            existingOrder.Total_Discount = totalDiscount.GetDecimal();
        }

        if (partialOrder.TryGetProperty("Total_Tax", out var totalTax))
        {
            originalFields["Total_Tax"] = existingOrder.Total_Tax;
            existingOrder.Total_Tax = totalTax.GetDecimal();
        }

        if (partialOrder.TryGetProperty("Total_Surcharge", out var totalSurcharge))
        {
            originalFields["Total_Surcharge"] = existingOrder.Total_Surcharge;
            existingOrder.Total_Surcharge = totalSurcharge.GetDecimal();
        }

        var success = orderPool.ReplaceOrder(id, existingOrder);
        if (!success) 
        return NoContent();

        _dataBefore = originalFields;
        _dataAfter = partialOrder;

        DataProvider.fetch_order_pool().Save();
        return Ok(existingOrder);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_order_pool().RemoveOrder(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_order_pool().Save();
        return Ok();
    }
}