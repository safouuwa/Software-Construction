using Microsoft.AspNetCore.Mvc;
using ModelsV2;
using ProvidersV2;
using System.Text.Json;
using HelpersV2;
using ProcessorsV2;


[ApiController]
[Route("api/v2/[controller]")]
public class InventoriesController : BaseApiController
{
    public InventoriesController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetInventories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortOrder = "asc"
        )
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "get");
        if (auth is UnauthorizedResult) return auth;

        var inventories = DataProvider.fetch_inventory_pool().GetInventories();
            inventories = sortOrder.ToLower() == "desc"
                        ? inventories.OrderByDescending(i => i.Id).ToList()
                        : inventories.OrderBy(i => i.Id).ToList();
    if (auth is OkResult)
        {
            var user = AuthProvider.GetUser(Request.Headers["API_KEY"]);
            var locations = DataProvider.fetch_location_pool().GetLocations();
            var locationids = locations.Where(x => user.OwnWarehouses.Contains(x.Warehouse_Id)).Select(x => x.Id).ToList();
            inventories = inventories.Where(x => x.Locations.Any(y => locationids.Contains(y))).ToList();
        }

        var response = PaginationHelper.Paginate(inventories, page, pageSize);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "getsingle");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_inventory_pool().GetInventory(id);
        if (inventory == null) return NoContent();

        return Ok(inventory);
    }

    [HttpPost]
    public IActionResult CreateInventory([FromBody] Inventory inventory)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "post");
        if (auth != null) return auth;
        if (inventory.Id != null) return BadRequest("Inventory: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_inventory_pool().AddInventory(inventory);
        if (!success) return BadRequest("Item ID and Reference do not refer to the same Item entity");

        DataProvider.fetch_inventory_pool().Save();
        return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateInventory(int id, [FromBody] Inventory inventory)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "put");
        if (auth != null) return auth;

        if (inventory.Id != null) return BadRequest("Inventory: Id should not be given a value in the body; Id will be assigned automatically.");
        if (DataProvider.fetch_inventory_pool().GetInventory(id) == null) return NoContent();
        var success = DataProvider.fetch_inventory_pool().UpdateInventory(id, inventory);
        if (!success) return BadRequest("Item ID and Reference do not refer to the same Item entity");

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
            return NoContent();

        if (partialInventory.TryGetProperty("Item_Id", out var inventoryId))
        {
            existingInventory.Item_Id = inventoryId.GetString();
        }

        if (partialInventory.TryGetProperty("Description", out var description))
        {   
            existingInventory.Description = description.GetString();
        }

        if (partialInventory.TryGetProperty("Item_Reference", out var itemReference))
        {
            existingInventory.Item_Reference = itemReference.GetString();
        }

        if (partialInventory.TryGetProperty("Locations", out var locations))
        {
            var locationsString = locations.ToString();
            if (!string.IsNullOrEmpty(locationsString))
            {
                existingInventory.Locations = locationsString.Trim('[',']').Split(',')
                    .Select(int.Parse)
                    .ToList();
            }
            else
            {
                existingInventory.Locations = new List<int>();
            }
        }

        if (partialInventory.TryGetProperty("TotalOnHand", out var totalOnHand))
        {
            existingInventory.Total_On_Hand = totalOnHand.GetInt32();
        }

        if (partialInventory.TryGetProperty("TotalExpected", out var totalExpected))
        {
            existingInventory.Total_Expected = totalExpected.GetInt32();
        }

        if (partialInventory.TryGetProperty("TotalOrdered", out var totalOrdered))
        {
            existingInventory.Total_Ordered = totalOrdered.GetInt32();
        }

        if (partialInventory.TryGetProperty("TotalAllocated", out var totalAllocated))
        {
            existingInventory.Total_Allocated = totalAllocated.GetInt32();
        }

        if (partialInventory.TryGetProperty("TotalAvailable", out var totalAvailable))
        {
            existingInventory.Total_Available = totalAvailable.GetInt32();
        }

        var success = InventoryPool.ReplaceInventory(id, existingInventory);
        if (!success)
            return BadRequest("Item ID and Reference do not refer to the same Item entity");

        DataProvider.fetch_inventory_pool().Save();
        return Ok(existingInventory);
    }
    

    [HttpDelete("{id}")]
    public IActionResult DeleteInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_inventory_pool().RemoveInventory(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "inventories", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_inventory_pool().RemoveInventory(id, true);
        DataProvider.fetch_inventory_pool().Save();
        return Ok();
    }
}