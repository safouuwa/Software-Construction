using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : BaseApiController
{
    public OrdersController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth is UnauthorizedResult) return auth;

        var orders = DataProvider.fetch_order_pool().GetOrders();

        if (auth is OkResult) 
        {
            var user = AuthProvider.GetUser(Request.Headers["API_KEY"]);
            orders = orders.Where(o => user.OwnWarehouses.Contains(o.Warehouse_Id)).ToList();
        }
        return Ok(orders);
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
        [FromQuery] int? id = null, 
        [FromQuery] int? sourceId = null,
        [FromQuery] string orderStatus = null,
        [FromQuery] string orderDate = null,
        [FromQuery] int? warehouseId = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth != null) return auth;

        try
        {
            var orders = DataProvider.fetch_order_pool().SearchOrders(
                id,
                sourceId, 
                orderStatus, 
                orderDate,  
                warehouseId);

            if (orders == null || !orders.Any())
            {
                return NoContent();
            }

            return Ok(orders);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "post");
        if (auth != null) return auth;
        if (order.Id != null) return BadRequest("Order: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_order_pool().AddOrder(order);
        if (!success) return BadRequest("Order: Id already exists");

        DataProvider.fetch_order_pool().Save();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, [FromBody] Order order)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "put");
        if (auth != null) return auth;

        if (order.Id != null) return BadRequest("Order: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_order_pool().UpdateOrder(id, order);
        if (!success) return NoContent();

        DataProvider.fetch_order_pool().Save();
        return Ok();
    }

    [HttpPut("{id}/items")]
    public IActionResult UpdateOrderItems(int id, [FromBody] List<Item> items)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "put");
        if (auth != null) return auth;

        if (items.Any(x => x.Uid == null)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_order_pool().GetOrder(id) == null)
            return NoContent();

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

    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateClient(int id, [FromBody] JsonElement partialOrder)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "patch");
        if (auth != null) return auth;

        if (partialOrder.ValueKind == JsonValueKind.Undefined)
            return BadRequest("No updates provided");

        var orderPool = DataProvider.fetch_order_pool();
        var existingOrder = orderPool.GetOrder(id);

        if (existingOrder == null) 
            return NoContent();

        if (partialOrder.TryGetProperty("Source_id", out var sourceId))
        {
            existingOrder.Source_Id = sourceId.GetInt32();
        }

        if (partialOrder.TryGetProperty("Order_date", out var orderDate))
        {
            existingOrder.Order_Date = orderDate.GetString();
        }

        if (partialOrder.TryGetProperty("Request_date", out var requestDate))
        {
            existingOrder.Request_Date = requestDate.GetString();
        }

        if(partialOrder.TryGetProperty("Reference", out var reference))
        {
            existingOrder.Reference = reference.GetString();
        }

        if (partialOrder.TryGetProperty("Reference_extra", out var referenceExtra))
        {
            existingOrder.Reference_Extra = referenceExtra.GetString();
        }
        
        if (partialOrder.TryGetProperty("Order_status", out var orderStatus))
        {
            existingOrder.Order_Status = orderStatus.GetString();
        }

        if (partialOrder.TryGetProperty("Notes", out var notes))
        {
            existingOrder.Notes = notes.GetString();
        }

        if (partialOrder.TryGetProperty("Shipping_notes", out var shippingNotes))
        {
            existingOrder.Shipping_Notes = shippingNotes.GetString();
        }

        if (partialOrder.TryGetProperty("Picking_notes", out var pickingNotes))
        {
            existingOrder.Picking_Notes = pickingNotes.GetString();
        }

        if (partialOrder.TryGetProperty("Warehouse_id", out var warehouseId))
        {
            existingOrder.Warehouse_Id = warehouseId.GetInt32();
        }

        if (partialOrder.TryGetProperty("Ship_to", out var shipTo))
        {
            existingOrder.Ship_To = shipTo.GetInt32();
        }

        if (partialOrder.TryGetProperty("Bill_to", out var billTo))
        {
            existingOrder.Bill_To = billTo.GetInt32();
        }

        if (partialOrder.TryGetProperty("Shipment_id", out var shipmentId))
        {
            existingOrder.Shipment_Id = shipmentId.GetInt32();
        }

        if (partialOrder.TryGetProperty("Total_Amount", out var totalAmount))
        {
            existingOrder.Total_Amount = totalAmount.GetDecimal();
        }

        if (partialOrder.TryGetProperty("Total_Discount", out var totalDiscount))
        {
            existingOrder.Total_Discount = totalDiscount.GetDecimal();
        }

        if (partialOrder.TryGetProperty("Total_Tax", out var totalTax))
        {
            existingOrder.Total_Tax = totalTax.GetDecimal();
        }

        if (partialOrder.TryGetProperty("Total_Surcharge", out var totalSurcharge))
        {
            existingOrder.Total_Surcharge = totalSurcharge.GetDecimal();
        }

        var success = orderPool.ReplaceOrder(id, existingOrder);
        if (!success) 
        return StatusCode(500, "Failed to update order");

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