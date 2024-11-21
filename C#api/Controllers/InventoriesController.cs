using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoriesController : BaseApiController
{
    public InventoriesController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetInventories()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "get");
        if (auth != null) return auth;

        var inventories = DataProvider.fetch_inventory_pool().GetInventories();
        return Ok(inventories);
    }

    [HttpGet("{id}")]
    public IActionResult GetInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "get");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_inventory_pool().GetInventory(id);
        if (inventory == null) return NotFound();

        return Ok(inventory);
    }

    [HttpGet("{id}/locations")]
    public IActionResult GetInventoryLocations(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "get");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_inventory_pool().GetInventory(id);
        var locations = new List<Location>();
        foreach (int loc in inventory.Locations) locations.Add(DataProvider.fetch_location_pool().GetLocation(loc));
        foreach (Location loc in locations) if (loc == null) locations.Remove(loc);

        return Ok(locations);
    }

    [HttpPost]
    public IActionResult CreateInventory([FromBody] Inventory inventory)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "post");
        if (auth != null) return auth;

        if (inventory.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_inventory_pool().AddInventory(inventory);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_inventory_pool().Save();
        return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateInventory(int id, [FromBody] Inventory inventory)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "put");
        if (auth != null) return auth;

        if (inventory.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_inventory_pool().UpdateInventory(id, inventory);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_inventory_pool().RemoveInventory(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }
}