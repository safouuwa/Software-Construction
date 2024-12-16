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
        if (shipment == null) return NotFound();

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
        if (shipment == null) return NotFound();

        return Ok(shipment.Shipment_Status);
    }

    [HttpGet("search")]
    public IActionResult SearchShipments(
        [FromQuery] int? id = null,
        [FromQuery] int? orderId = null,
        [FromQuery] int? sourceId = null,
        [FromQuery] string orderDate = null,
        [FromQuery] string requestDate = null,
        [FromQuery] string shipmentDate = null,
        [FromQuery] string shipmentType = null,
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
                sourceId, 
                orderDate, 
                requestDate, 
                shipmentDate, 
                shipmentType, 
                shipmentStatus, 
                carrierCode);

            if (shipments == null || !shipments.Any())
            {
                return NotFound("Error, er is geen Shipment(s) gevonden met deze gegevens.");
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
        if (!success) return NotFound("ID not found");

        DataProvider.fetch_shipment_pool().Save();
        return Ok();
    }

    [HttpPut("{id}/orders")]
    public IActionResult UpdateShipmentOrders(int id, [FromBody] List<Order> orders)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "shipments", "put");
        if (auth != null) return auth;

        if (orders.Any(x => x.Id == -10)) return BadRequest("ID not given in body");

        if (DataProvider.fetch_shipment_pool().GetShipment(id) == null)
            return NotFound("No data for given ID");

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
            return NotFound("No data for given ID");

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
        if (shipment == null) return NotFound("No data found with given ID");

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
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

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
        if (!success) return NotFound("ID not found or other data is dependent on this data");

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