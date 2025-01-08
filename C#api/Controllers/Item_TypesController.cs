using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;
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
        if (itemType == null) return NoContent();

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
        if (itemType.Id != null) return BadRequest("ItemType: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_itemtype_pool().AddItemtype(itemType);
        if (!success) return BadRequest("ItemType: Id already exists");

        DataProvider.fetch_itemtype_pool().Save();
        return CreatedAtAction(nameof(GetItemType), new { id = itemType.Id }, itemType);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItemType(int id, [FromBody] ItemType itemType)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "put");
        if (auth != null) return auth;

        if (itemType.Id != null) return BadRequest("ItemType: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_itemtype_pool().UpdateItemtype(id, itemType);
        if (!success) return NoContent();

        DataProvider.fetch_itemtype_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateItemType(int id, [FromBody] JsonElement partialItemType)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "patch");
        if (auth != null) return auth;

        if (partialItemType.ValueKind == JsonValueKind.Null)
            return BadRequest("No data in body");

        var itemTypePool = DataProvider.fetch_itemtype_pool();
        var existingItemType = itemTypePool.GetItemType(id);

        if (existingItemType == null) 
            return NoContent();

        if (partialItemType.TryGetProperty("Name", out var name))
        {
            existingItemType.Name = name.GetString();
        }

        if (partialItemType.TryGetProperty("Description", out var description))
        {
            existingItemType.Description = description.GetString();
        }

        var success = itemTypePool.ReplaceItemType(id, existingItemType);
        if (!success)
            return StatusCode(500, "Failed to update ItemType");

        DataProvider.fetch_itemtype_pool().Save();
        return Ok(existingItemType);;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItemType(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_itemtype_pool().RemoveItemtype(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_itemtype_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteItemType(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_types", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_itemtype_pool().RemoveItemtype(id, true);
        DataProvider.fetch_itemtype_pool().Save();
        return Ok();
    }
}