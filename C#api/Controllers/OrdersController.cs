using Microsoft.AspNetCore.Mvc;
using Models;
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
        if (auth != null) return auth;

        var orders = DataProvider.fetch_order_pool().GetOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth != null) return auth;

        var order = DataProvider.fetch_order_pool().GetOrder(id);
        if (order == null) return NotFound();

        return Ok(order);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetOrderItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth != null) return auth;

        var items = DataProvider.fetch_order_pool().GetItemsInOrder(id);
        return Ok(items);
    }

    [HttpGet("{id}/status")]
    public IActionResult GetOrderStatus(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "get");
        if (auth != null) return auth;

        var order = DataProvider.fetch_order_pool().GetOrder(id);
        if (order == null) return NotFound();

        return Ok(order.Order_Status);
    }

    [HttpGet("search")]
    public IActionResult SearchOrders(
        [FromQuery] int? id = null, 
        [FromQuery] int? sourceId = null,
        [FromQuery] string orderStatus = null,
        [FromQuery] string orderDate = null,
        [FromQuery] string requestDate = null,
        [FromQuery] string reference = null,
        [FromQuery] string referenceExtra = null,
        [FromQuery] string notes = null,
        [FromQuery] string shippingNotes = null,
        [FromQuery] string pickingNotes = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] int? shipTo = null,
        [FromQuery] int? billTo = null,
        [FromQuery] int? shipmentId = null,
        [FromQuery] string created_At = null,
        [FromQuery] string updated_At = null)
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
                requestDate, 
                reference, 
                referenceExtra, 
                notes, 
                shippingNotes, 
                pickingNotes, 
                warehouseId, 
                shipTo, 
                billTo, 
                shipmentId, 
                created_At, 
                updated_At);

            if (orders == null || !orders.Any())
            {
                return NotFound("Error, er is geen Order(s) gevonden met deze gegevens.");
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

        if (order.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_order_pool().AddOrder(order);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_order_pool().Save();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, [FromBody] Order order)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "put");
        if (auth != null) return auth;

        if (order.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_order_pool().UpdateOrder(id, order);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

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
            return NotFound("No data for given ID");

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
        if (order == null) return NotFound("No data found with given ID");

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
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        _notificationSystem.Push($"Order with id: {order.Id} has been processed.");
        DataProvider.fetch_order_pool().Save();
        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "orders", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_order_pool().RemoveOrder(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_order_pool().Save();
        return Ok();
    }
}