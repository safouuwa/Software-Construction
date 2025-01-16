using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class Item_GroupsController : BaseApiController
{
    public Item_GroupsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetItemGroups(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "get");
        if (auth != null) return auth;

        var itemGroups = DataProvider.fetch_itemgroup_pool().GetItemGroups();

        var response = PaginationHelper.Paginate(itemGroups, page, pageSize);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetItemGroup(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "getsingle");
        if (auth != null) return auth;

        var itemGroup = DataProvider.fetch_itemgroup_pool().GetItemGroup(id);
        if (itemGroup == null) return NoContent();

        return Ok(itemGroup);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetItemGroupItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_item_pool().GetItemsForItemGroup(id);
        return Ok(items);
    }

    [HttpPost]
    public IActionResult CreateItemGroup([FromBody] ItemGroup itemGroup)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "post");
        if (auth != null) return auth;
        if (itemGroup.Id != null) return BadRequest("ItemGroup: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_itemgroup_pool().AddItemGroup(itemGroup);
        if (!success) return BadRequest("ItemGroup: Id already exists");

        DataProvider.fetch_itemgroup_pool().Save();
        return CreatedAtAction(nameof(GetItemGroup), new { id = itemGroup.Id }, itemGroup);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItemGroup(int id, [FromBody] ItemGroup itemGroup)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "put");
        if (auth != null) return auth;

        if (itemGroup.Id != null) return BadRequest("ItemGroup: Id should not be given a value in the body; Id will be assigned automatically.");

        var success = DataProvider.fetch_itemgroup_pool().UpdateItemGroup(id, itemGroup);
        if (!success) return NoContent();

        DataProvider.fetch_itemgroup_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateItemGroup(int id, [FromBody] JsonElement partialItemGroup)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_group", "patch");
        if (auth != null) return auth;

        if (partialItemGroup.ValueKind == JsonValueKind.Undefined)
            return BadRequest("No updates provided");

        var itemGroupPool = DataProvider.fetch_itemgroup_pool();
        var existingItemGroup = itemGroupPool.GetItemGroup(id);

        if (existingItemGroup == null) 
        return NoContent();

        if (partialItemGroup.TryGetProperty("Name", out var name))
        {
            existingItemGroup.Name = name.GetString();
        }

        if (partialItemGroup.TryGetProperty("Description", out var description))
        {
            existingItemGroup.Description = description.GetString();
        }

        var success = itemGroupPool.ReplaceItemGroup(id, existingItemGroup);
        if (!success)
            return StatusCode(500, "Failed to update client");

        DataProvider.fetch_itemgroup_pool().Save();
        return Ok(existingItemGroup);;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItemGroup(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_itemgroup_pool().RemoveItemGroup(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_itemgroup_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteItemGroup(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_itemgroup_pool().RemoveItemGroup(id, true);
        DataProvider.fetch_itemgroup_pool().Save();
        return Ok();
    }
}