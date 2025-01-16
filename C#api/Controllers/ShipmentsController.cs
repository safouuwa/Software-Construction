using System.Text.Json;
using Attributes;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShipmentsController : BaseApiController, ILoggableAction
{
    public ShipmentsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }
    public object _dataBefore { get; set; }
    public object _dataAfter { get; set; }


    [HttpGet]
    public IActionResult GetShipments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "get");
        if (auth != null) return auth;

        var shipments = DataProvider.fetch_shipment_pool().GetShipments();
        var response = PaginationHelper.Paginate(shipments, pageSize, page);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetShipment(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "getsingle");
        if (auth != null) return auth;

        var shipment = DataProvider.fetch_shipment_pool().GetShipment(id);
        if (shipment == null) return NoContent();

        return Ok(shipment);
    }

    [HttpGet("{id}/orders")]
    public IActionResult GetShipmentOrders(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "getsingle");
        if (auth != null) return auth;

        var orders = DataProvider.fetch_order_pool().GetOrdersInShipment(id);
        return Ok(orders);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetShipmentItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_shipment_pool().GetItemsInShipment(id);
        return Ok(items);
    }

    [HttpGet("{id}/status")]
    public IActionResult GetShipmentStatus(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "getsingle");
        if (auth != null) return auth;

        var shipment = DataProvider.fetch_shipment_pool().GetShipment(id);
        if (shipment == null) return NoContent();

        return Ok(shipment.Shipment_Status);
    }

    [HttpGet("search")]
    public IActionResult SearchShipments(
        [FromQuery] int? orderId = null,
        [FromQuery] string orderDate = null,
        [FromQuery] string shipmentStatus = null,
        [FromQuery] string carrierCode = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "get");
        if (auth != null) return auth;

        try
        {
            var shipments = DataProvider.fetch_shipment_pool().SearchShipments(
                orderId,  
                orderDate, 
                shipmentStatus, 
                carrierCode);

            if (shipments == null || !shipments.Any())
            {
                return NoContent();
            }
            var response = PaginationHelper.Paginate(shipments, page, pageSize);
            return Ok(shipments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/order")]
    public IActionResult GetShipmentOrder(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "getsingle");
        if (auth != null) return auth;

        var shipment = DataProvider.fetch_shipment_pool().GetShipment(id);
        if (shipment == null) return BadRequest("Invalid Shipment ID");

        var order = DataProvider.fetch_order_pool().GetOrder(shipment.Order_Id);
        if (order == null) return NoContent();

        return Ok(order);
    }

    [LogRequest]
    [HttpPost]
    public IActionResult CreateShipment([FromBody] Shipment shipment)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "post");
        if (auth != null) return auth;
        if (shipment.Id != null) return BadRequest("Shipment: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_shipment_pool().AddShipment(shipment);
        if (!success) return BadRequest("Shipment: Id already exists");

        DataProvider.fetch_shipment_pool().Save();
        _dataBefore = null;
        _dataAfter = shipment;
        return CreatedAtAction(nameof(GetShipment), new { id = shipment.Id }, shipment);
    }
    [LogRequest]
    [HttpPut("{id}")]
    public IActionResult UpdateShipment(int id, [FromBody] Shipment shipment)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

                if (shipment.Id != null) return BadRequest("Shipment: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_shipment_pool().UpdateShipment(id, shipment);
        if (!success) return NoContent();
        _dataBefore = DataProvider.fetch_shipment_pool().GetShipment(id);
        _dataAfter = shipment;

        DataProvider.fetch_shipment_pool().Save();
        return Ok();
    }
    [LogRequest]
    [HttpPatch("{id}")]
    public IActionResult PartialUpdateShipment(int id, [FromBody] JsonElement partialShipment)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "patch");
        if (auth != null) return auth;

        if (partialShipment.ValueKind == JsonValueKind.Null) 
            return BadRequest("No data given in body");

        var shipmentPool = DataProvider.fetch_shipment_pool();
        var existingShipment = shipmentPool.GetShipment(id);
        var originalFields = new Dictionary<string, object>();

        if (existingShipment == null) 
            return NoContent();

        if (partialShipment.TryGetProperty("Order_Id", out var order_id))
        {
            originalFields["Order_Id"] = existingShipment.Order_Id;
            existingShipment.Order_Id = order_id.GetInt32();
        }

        if (partialShipment.TryGetProperty("Source_Id", out var source_id))
        {
            originalFields["Source_Id"] = existingShipment.Source_Id;
            existingShipment.Source_Id = source_id.GetInt32();
        }

        if (partialShipment.TryGetProperty("Order_Date", out var order_date))
        {
            originalFields["Order_Date"] = existingShipment.Order_Date;
            existingShipment.Order_Date = order_date.GetString();
        }

        if (partialShipment.TryGetProperty("Request_Date", out var request_date))
        {
            originalFields["Request_Date"] = existingShipment.Request_Date;
            existingShipment.Request_Date = request_date.GetString();
        }

        if (partialShipment.TryGetProperty("Shipment_Date", out var shipment_date))
        {
            originalFields["Shipment_Date"] = existingShipment.Shipment_Date;
            existingShipment.Shipment_Date = shipment_date.GetString();
        }

        if (partialShipment.TryGetProperty("Shipment_Type", out var shipment_type))
        {
            originalFields["Shipment_Type"] = existingShipment.Shipment_Type;
            existingShipment.Shipment_Type = shipment_type.GetString();
        }

        if (partialShipment.TryGetProperty("Shipment_Status", out var shipment_status))
        {
            originalFields["Shipment_Status"] = existingShipment.Shipment_Status;
            existingShipment.Shipment_Status = shipment_status.GetString();
        }

        if (partialShipment.TryGetProperty("Notes", out var notes))
        {
            originalFields["Notes"] = existingShipment.Notes;
            existingShipment.Notes = notes.GetString();
        }

        if (partialShipment.TryGetProperty("Carrier_Code", out var carrier_code))
        {
            originalFields["Carrier_Code"] = existingShipment.Carrier_Code;
            existingShipment.Carrier_Code = carrier_code.GetString();
        }

        if (partialShipment.TryGetProperty("Carrier_Description", out var carrier_description))
        {
            originalFields["Carrier_Description"] = existingShipment.Carrier_Description;
            existingShipment.Carrier_Description = carrier_description.GetString();
        }

        if (partialShipment.TryGetProperty("Service_Code", out var service_code))
        {
            originalFields["Service_Code"] = existingShipment.Service_Code;
            existingShipment.Service_Code = service_code.GetString();
        }

        if (partialShipment.TryGetProperty("Payment_Type", out var payment_type))
        {
            originalFields["Payment_Type"] = existingShipment.Payment_Type;
            existingShipment.Payment_Type = payment_type.GetString();
        }

        if (partialShipment.TryGetProperty("Transfer_Mode", out var transfer_mode))
        {
            originalFields["Transfer_Mode"] = existingShipment.Transfer_Mode;
            existingShipment.Transfer_Mode = transfer_mode.GetString();
        }

        if (partialShipment.TryGetProperty("Total_Package_Count", out var total_package_count))
        {
            originalFields["Total_Package_Count"] = existingShipment.Total_Package_Count;
            existingShipment.Total_Package_Count = total_package_count.GetInt32();
        }

        if (partialShipment.TryGetProperty("Total_Package_Weight", out var total_package_weight))
        {
            originalFields["Total_Package_Weight"] = existingShipment.Total_Package_Weight;
            existingShipment.Total_Package_Weight = total_package_weight.GetDouble();
        }

        var success = shipmentPool.ReplaceShipment(id, existingShipment);
        if (!success) 
            return NoContent();
        
        DataProvider.fetch_shipment_pool().Save();

        _dataBefore = originalFields;
        _dataAfter = partialShipment;
        return Ok(existingShipment);
    }
    [LogRequest]
    [HttpPut("{id}/orders")]
    public IActionResult UpdateShipmentOrders(int id, [FromBody] List<Order> orders)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

        if (orders.Any(x => x.Id == -10)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_shipment_pool().GetShipment(id) == null)
            return NoContent();
        
        _dataBefore = DataProvider.fetch_order_pool().GetOrdersInShipment(id);
        _dataAfter = orders;

        DataProvider.fetch_order_pool().UpdateOrdersInShipment(id, orders);
        DataProvider.fetch_order_pool().Save();
        return Ok();
    }
    [LogRequest]
    [HttpPut("{id}/items")]
    public IActionResult UpdateShipmentItems(int id, [FromBody] List<Item> items)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

        if (items.Any(x => x.Uid == null)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_shipment_pool().GetShipment(id) == null)
            return NoContent();

        _dataBefore = DataProvider.fetch_shipment_pool().GetItemsInShipment(id);
        _dataAfter = items;

        DataProvider.fetch_shipment_pool().UpdateItemsInShipment(id, items);
        DataProvider.fetch_shipment_pool().Save();
        return Ok();
    }

    [HttpPut("{id}/commit")]
    public IActionResult CommitShipment(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

        var shipment = DataProvider.fetch_shipment_pool().GetShipment(id);
        if (shipment == null) return NoContent();

        foreach (var item in shipment.Items)
        {
            var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(item.Item_Id);
            foreach (var inventory in inventories)
            {
                if (inventory.Locations.Contains(shipment.Source_Id))
                {
                    inventory.Total_On_Hand -= item.Amount;
                }
            }
        }

        shipment.Shipment_Status = "Shipped";
        var success = DataProvider.fetch_shipment_pool().UpdateShipment(id, shipment);
        if (!success) return NoContent();

        _notificationSystem.Push($"Shipment with id: {shipment.Id} has been processed.");
        DataProvider.fetch_shipment_pool().Save();
        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteShipment(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_shipment_pool().RemoveShipment(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_shipment_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteShipment(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_shipment_pool().RemoveShipment(id, true);
        DataProvider.fetch_shipment_pool().Save();
        return Ok();
    }
}