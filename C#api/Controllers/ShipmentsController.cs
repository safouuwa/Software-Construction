using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShipmentsController : BaseApiController
{
    public ShipmentsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetShipments()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "get");
        if (auth != null) return auth;

        var shipments = DataProvider.fetch_shipment_pool().GetShipments();
        return Ok(shipments);
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
        [FromQuery] int? id = null,
        [FromQuery] int? orderId = null,
        [FromQuery] string orderDate = null,
        [FromQuery] string shipmentStatus = null,
        [FromQuery] string carrierCode = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "get");
        if (auth != null) return auth;

        try
        {
            var shipments = DataProvider.fetch_shipment_pool().SearchShipments(
                id,
                orderId,  
                orderDate, 
                shipmentStatus, 
                carrierCode);

            if (shipments == null || !shipments.Any())
            {
                return NoContent();
            }

            return Ok(shipments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateShipment([FromBody] Shipment shipment)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "post");
        if (auth != null) return auth;
        if (shipment.Id != null) return BadRequest("Shipment: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_shipment_pool().AddShipment(shipment);
        if (!success) return BadRequest("Shipment: Id already exists");

        DataProvider.fetch_shipment_pool().Save();
        return CreatedAtAction(nameof(GetShipment), new { id = shipment.Id }, shipment);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateShipment(int id, [FromBody] Shipment shipment)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

                if (shipment.Id != null) return BadRequest("Shipment: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_shipment_pool().UpdateShipment(id, shipment);
        if (!success) return NoContent();

        DataProvider.fetch_shipment_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateShipment(int id, [FromBody] JsonElement partialShipment)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "patch");
        if (auth != null) return auth;

        if (partialShipment.ValueKind == JsonValueKind.Null) 
            return BadRequest("No data given in body");

        var shipmentPool = DataProvider.fetch_shipment_pool();
        var existingShipment = shipmentPool.GetShipment(id);

        if (existingShipment == null) 
            return NoContent();

        if (partialShipment.TryGetProperty("Order_Id", out var order_id))
        {
            existingShipment.Order_Id = order_id.GetInt32();
        }

        if (partialShipment.TryGetProperty("Source_Id", out var source_id))
        {
            existingShipment.Source_Id = source_id.GetInt32();
        }

        if (partialShipment.TryGetProperty("Order_Date", out var order_date))
        {
            existingShipment.Order_Date = order_date.GetString();
            }

        if (partialShipment.TryGetProperty("Request_Date", out var request_date))
        {
            existingShipment.Request_Date = request_date.GetString();
        }

        if (partialShipment.TryGetProperty("Shipment_Date", out var shipment_date))
        {
            existingShipment.Shipment_Date = shipment_date.GetString();
        }

        if (partialShipment.TryGetProperty("Shipment_Type", out var shipment_type))
        {
            existingShipment.Shipment_Type = shipment_type.GetString();
        }

        if (partialShipment.TryGetProperty("Shipment_Status", out var shipment_status))
        {
            existingShipment.Shipment_Status = shipment_status.GetString();
        }

        if (partialShipment.TryGetProperty("Notes", out var notes))
        {
            existingShipment.Notes = notes.GetString();
        }

        if (partialShipment.TryGetProperty("Carrier_Code", out var carrier_code))
        {
            existingShipment.Carrier_Code = carrier_code.GetString();
        }

        if (partialShipment.TryGetProperty("Carrier_Description", out var carrier_description))
        {
            existingShipment.Carrier_Description = carrier_description.GetString();
        }

        if (partialShipment.TryGetProperty("Service_Code", out var service_code))
        {
            existingShipment.Service_Code = service_code.GetString();
        }

        if (partialShipment.TryGetProperty("Payment_Type", out var payment_type))
        {
            existingShipment.Payment_Type = payment_type.GetString();
        }

        if (partialShipment.TryGetProperty("Transfer_Mode", out var transfer_mode))
        {
            existingShipment.Transfer_Mode = transfer_mode.GetString();
        }

        if (partialShipment.TryGetProperty("Total_Package_Count", out var total_package_count))
        {
            existingShipment.Total_Package_Count = total_package_count.GetInt32();
        }

        if (partialShipment.TryGetProperty("Total_Package_Weight", out var total_package_weight))
        {
            existingShipment.Total_Package_Weight = total_package_weight.GetDouble();
        }

        var success = shipmentPool.ReplaceShipment(id, existingShipment);
        if (!success) 
            return StatusCode(500,"ID not found or ID in Body and Route are not matching");
        
        DataProvider.fetch_shipment_pool().Save();
        return Ok(existingShipment);
    }

    [HttpPut("{id}/orders")]
    public IActionResult UpdateShipmentOrders(int id, [FromBody] List<Order> orders)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

        if (orders.Any(x => x.Id == -10)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_shipment_pool().GetShipment(id) == null)
            return NoContent();

        DataProvider.fetch_order_pool().UpdateOrdersInShipment(id, orders);
        DataProvider.fetch_order_pool().Save();
        return Ok();
    }

    [HttpPut("{id}/items")]
    public IActionResult UpdateShipmentItems(int id, [FromBody] List<Item> items)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

        if (items.Any(x => x.Uid == null)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_shipment_pool().GetShipment(id) == null)
            return NoContent();

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