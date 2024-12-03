using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class Item_LinesController : BaseApiController
{
    public Item_LinesController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetItemLines()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "get");
        if (auth != null) return auth;

        var itemLines = DataProvider.fetch_itemline_pool().GetItemLines();
        return Ok(itemLines);
    }

    [HttpGet("{id}")]
    public IActionResult GetItemLine(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "getsingle");
        if (auth != null) return auth;

        var itemLine = DataProvider.fetch_itemline_pool().GetItemLine(id);
        if (itemLine == null) return NotFound();

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

        if (itemLine.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_itemline_pool().AddItemline(itemLine);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_itemline_pool().Save();
        return CreatedAtAction(nameof(GetItemLine), new { id = itemLine.Id }, itemLine);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItemLine(int id, [FromBody] ItemLine itemLine)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "put");
        if (auth != null) return auth;

        if (itemLine.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_itemline_pool().UpdateItemline(id, itemLine);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_itemline_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItemLine(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "item_lines", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_itemline_pool().RemoveItemline(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_itemline_pool().Save();
        return Ok();
    }
}