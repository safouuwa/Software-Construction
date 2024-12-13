using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class Item_TypesController : BaseApiController
{
    public Item_TypesController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetItemTypes()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "get");
        if (auth != null) return auth;

        var itemTypes = DataProvider.fetch_itemtype_pool().GetItemTypes();
        return Ok(itemTypes);
    }

    [HttpGet("{id}")]
    public IActionResult GetItemType(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "getsingle");
        if (auth != null) return auth;

        var itemType = DataProvider.fetch_itemtype_pool().GetItemType(id);
        if (itemType == null) return NotFound();

        return Ok(itemType);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetItemTypeItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_item_pool().GetItemsForItemType(id);
        return Ok(items);
    }

    [HttpPost]
    public IActionResult CreateItemType([FromBody] ItemType itemType)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "post");
        if (auth != null) return auth;

        if (itemType.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_itemtype_pool().AddItemtype(itemType);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_itemtype_pool().Save();
        return CreatedAtAction(nameof(GetItemType), new { id = itemType.Id }, itemType);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItemType(int id, [FromBody] ItemType itemType)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "put");
        if (auth != null) return auth;

        if (itemType.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_itemtype_pool().UpdateItemtype(id, itemType);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_itemtype_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateItemType(int id, [FromBody] ItemType partialItemType)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "patch");
        if (auth != null) return auth;

        if (partialItemType == null) return BadRequest("No updates provided");

        var success = DataProvider.fetch_itemtype_pool().ReplaceItemTypes(id, partialItemType);
        if (!success) return NotFound("ID not found");

        DataProvider.fetch_itemtype_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItemType(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_itemtype_pool().RemoveItemtype(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_itemtype_pool().Save();
        return Ok();
    }
}