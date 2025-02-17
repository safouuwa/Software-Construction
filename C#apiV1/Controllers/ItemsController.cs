using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;
namespace Processors;

[ApiController]
[Route("api/v1/[controller]")]
public class ItemsController : BaseApiController
{
    public ItemsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetItems()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "get");
        if (auth != null) return auth;

        var items = DataProvider.fetch_item_pool().GetItems();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public IActionResult GetItem(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "get");
        if (auth != null) return auth;

        var item = DataProvider.fetch_item_pool().GetItem(id);
        if (item == null) return NotFound();

        return Ok(item);
    }

    [HttpGet("{id}/inventory")]
    public IActionResult GetItemInventories(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "get");
        if (auth != null) return auth;

        var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(id);
        return Ok(inventories);
    }

    [HttpGet("{id}/inventory/totals")]
    public IActionResult GetItemInventoryTotals(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "get");
        if (auth != null) return auth;

        var totals = DataProvider.fetch_inventory_pool().GetInventoryTotalsForItem(id);
        return Ok(totals);
    }

    [HttpPost]
    public IActionResult CreateItem([FromBody] Item item)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "post");
        if (auth != null) return auth;

        if (item.Uid == null) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_item_pool().AddItem(item);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_item_pool().Save();
        return CreatedAtAction(nameof(GetItem), new { id = item.Uid }, item);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItem(string id, [FromBody] Item item)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "put");
        if (auth != null) return auth;

        if (item.Uid == null) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_item_pool().UpdateItem(id, item);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_item_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItem(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_item_pool().RemoveItem(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_item_pool().Save();
        return Ok();
    }
}