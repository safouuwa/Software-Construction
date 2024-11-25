using Microsoft.AspNetCore.Mvc;
using Models;
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
    public IActionResult GetItemGroups()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "get");
        if (auth != null) return auth;

        var itemGroups = DataProvider.fetch_itemgroup_pool().GetItemGroups();
        return Ok(itemGroups);
    }

    [HttpGet("{id}")]
    public IActionResult GetItemGroup(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "get");
        if (auth != null) return auth;

        var itemGroup = DataProvider.fetch_itemgroup_pool().GetItemGroup(id);
        if (itemGroup == null) return NotFound();

        return Ok(itemGroup);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetItemGroupItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "get");
        if (auth != null) return auth;

        var items = DataProvider.fetch_item_pool().GetItemsForItemGroup(id);
        return Ok(items);
    }

    [HttpPost]
    public IActionResult CreateItemGroup([FromBody] ItemGroup itemGroup)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "post");
        if (auth != null) return auth;

        if (itemGroup.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_itemgroup_pool().AddItemGroup(itemGroup);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_itemgroup_pool().Save();
        return CreatedAtAction(nameof(GetItemGroup), new { id = itemGroup.Id }, itemGroup);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItemGroup(int id, [FromBody] ItemGroup itemGroup)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "put");
        if (auth != null) return auth;

        if (itemGroup.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_itemgroup_pool().UpdateItemGroup(id, itemGroup);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_itemgroup_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateItemGroup(int id, [FromBody] ItemGroup partialItemGroup)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_Group", "patch");
        if (auth != null) return auth;

        if (partialItemGroup == null) return BadRequest("No updates provided");

        var success = DataProvider.fetch_itemgroup_pool().UpdateItemGroup(id, partialItemGroup);
        if (!success) return NotFound("ID not found");

        DataProvider.fetch_itemgroup_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItemGroup(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_groups", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_itemgroup_pool().RemoveItemGroup(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_itemgroup_pool().Save();
        return Ok();
    }
}