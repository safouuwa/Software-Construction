using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;
using System.Text.Json;

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
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "getsingle");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_inventory_pool().GetInventory(id);
        if (inventory == null) return NotFound();

        return Ok(inventory);
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

    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateInventory(int id, [FromBody] JsonElement partialInventory)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "patch");
        if (auth != null) return auth;

        if (partialInventory.ValueKind == JsonValueKind.Undefined)
            return BadRequest("No updates provided");

        var InventoryPool = DataProvider.fetch_inventory_pool();
        var existingInventory = InventoryPool.GetInventory(id);

        if (existingInventory == null)
            return NotFound("Client not found");

        if (partialInventory.TryGetProperty("ItemId", out var itemIdElement))
            existingInventory.Item_Id = itemIdElement.GetString();

        if (partialInventory.TryGetProperty("Description", out var descriptionElement))
            existingInventory.Description = descriptionElement.GetString();

        if (partialInventory.TryGetProperty("ItemReference", out var itemReferenceElement))
            existingInventory.Item_Reference = itemReferenceElement.GetString();

        if (partialInventory.TryGetProperty("Locations", out var locationsElement))
        {
            var locationsString = locationsElement.GetString();
            if (locationsString != null)
            {
                existingInventory.Locations = locationsString.Split(',')
                    .Select(int.Parse)
                    .ToList();
            }
            else
            {
                existingInventory.Locations = new List<int>();
            }
        }

        if (partialInventory.TryGetProperty("TotalOnHand", out var totalOnHandElement))
            existingInventory.Total_On_Hand = totalOnHandElement.GetInt32();

        if (partialInventory.TryGetProperty("TotalExpected", out var totalExpectedElement))
            existingInventory.Total_Expected = totalExpectedElement.GetInt32();
        
        if (partialInventory.TryGetProperty("TotalOrdered", out var totalOrderedElement))
            existingInventory.Total_Ordered = totalOrderedElement.GetInt32();

        if (partialInventory.TryGetProperty("TotalAllocated", out var totalAllocatedElement))
            existingInventory.Total_Allocated = totalAllocatedElement.GetInt32();

        if (partialInventory.TryGetProperty("TotalAvailable", out var totalAvailableElement))
            existingInventory.Total_Available = totalAvailableElement.GetInt32();

        var success = InventoryPool.UpdateInventory(id, existingInventory);
        if (!success)
            return StatusCode(500, "Failed to update client");

        DataProvider.fetch_inventory_pool().Save();
        return Ok(existingInventory);
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