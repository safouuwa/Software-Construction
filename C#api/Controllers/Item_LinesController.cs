using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ModelsV2;
using ProvidersV2;
using HelpersV2;
using ProcessorsV2;

[ApiController]
[Route("api/v2/[controller]")]
public class Item_LinesController : BaseApiController
{
    public Item_LinesController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetItemLines(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "get");
        if (auth != null) return auth;

        var itemLines = DataProvider.fetch_itemline_pool().GetItemLines();

        var response = PaginationHelper.Paginate(itemLines, page, pageSize);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetItemLine(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "getsingle");
        if (auth != null) return auth;

        var itemLine = DataProvider.fetch_itemline_pool().GetItemLine(id);
        if (itemLine == null) return NoContent();

        return Ok(itemLine);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetItemLineItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_item_pool().GetItemsForItemLine(id);
        return Ok(items);
    }

    [HttpPost]
    public IActionResult CreateItemLine([FromBody] ItemLine itemLine)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "post");
        if (auth != null) return auth;
        if (itemLine.Id != null) return BadRequest("ItemLine: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_itemline_pool().AddItemline(itemLine);
        if (!success) return BadRequest("ItemLine: Id already exists");

        DataProvider.fetch_itemline_pool().Save();
        return CreatedAtAction(nameof(GetItemLine), new { id = itemLine.Id }, itemLine);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItemLine(int id, [FromBody] ItemLine itemLine)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "put");
        if (auth != null) return auth;

        if (itemLine.Id != null) return BadRequest("ItemLine: Id should not be given a value in the body; Id will be assigned automatically.");

        var success = DataProvider.fetch_itemline_pool().UpdateItemline(id, itemLine);
        if (!success) return NoContent();

        DataProvider.fetch_itemline_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateItemLine(int id, [FromBody] JsonElement partialItemLine)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "patch");
        if (auth != null) return auth;

        if (partialItemLine.ValueKind == JsonValueKind.Null)
            return BadRequest("No data in body");

        var itemLinePool = DataProvider.fetch_itemline_pool();
        var existingItemLine = itemLinePool.GetItemLine(id);

        if (existingItemLine == null) 
            return NoContent();

        if (partialItemLine.TryGetProperty("Name", out var name))
        {
            existingItemLine.Name = name.GetString();
        }

        if (partialItemLine.TryGetProperty("Description", out var description))
        {
            existingItemLine.Description = description.GetString();
        }

        var success = itemLinePool.ReplaceItemLine(id, existingItemLine);
        if (!success)
            return StatusCode(500, "Failed to update ItemLine");

        DataProvider.fetch_itemline_pool().Save();
        return Ok(existingItemLine);;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItemLine(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_itemline_pool().RemoveItemline(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_itemline_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteItemLine(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_itemline_pool().RemoveItemline(id, true);
        DataProvider.fetch_itemline_pool().Save();
        return Ok();
    }
}