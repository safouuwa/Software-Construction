using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

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
        if (auth is UnauthorizedResult) return auth;

        var items = DataProvider.fetch_item_pool().GetItems();
        var newitems = new List<Item>();

        if (auth is OkResult)
        {
            var user = AuthProvider.GetUser(Request.Headers["API_KEY"]);
            var inventories = DataProvider.fetch_inventory_pool().GetInventories();
            var locations = DataProvider.fetch_location_pool().GetLocations();
            var locationids = locations.Where(x => user.OwnWarehouses.Contains(x.Warehouse_Id)).Select(x => x.Id).ToList();
            var ids = inventories.Where(x => x.Locations.Any(y => locationids.Contains(y))).Select(x => x.Item_Id);
            foreach (var id in ids) newitems.Add(DataProvider.fetch_item_pool().GetItem(id));
        }
        return Ok(newitems.Count == 0 ? items : newitems);
    }

    [HttpGet("{id}")]
    public IActionResult GetItem(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "getsingle");
        if (auth != null) return auth;

        var item = DataProvider.fetch_item_pool().GetItem(id);
        if (item == null) return NotFound();

        return Ok(item);
    }

    [HttpGet("{id}/inventory")]
    public IActionResult GetItemInventories(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "getsingle");
        if (auth != null) return auth;

        var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(id);
        return Ok(inventories);
    }

    [HttpGet("{id}/locations")]
    public IActionResult GetItemLocations(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "getsingle");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_inventory_pool().GetInventoriesForItem(id)[0];
        var locations = new List<Location>();
        foreach (int loc in inventory.Locations) locations.Add(DataProvider.fetch_location_pool().GetLocation(loc));
        foreach (Location loc in locations) if (loc == null) locations.Remove(loc);
        return Ok(locations);
    }

    [HttpGet("{id}/inventory/totals")]
    public IActionResult GetItemInventoryTotals(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "getsingle");
        if (auth != null) return auth;

        var totals = DataProvider.fetch_inventory_pool().GetInventoryTotalsForItem(id);
        return Ok(totals);
    }

    [HttpGet("search")]
    public IActionResult SearchItems(
        [FromQuery] string uid = null,
        [FromQuery] string description = null, 
        [FromQuery] string code = null, 
        [FromQuery] string upcCode = null, 
        [FromQuery] string modelNumber = null, 
        [FromQuery] string commodityCode = null, 
        [FromQuery] string supplierCode = null, 
        [FromQuery] string supplierPartNumber = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "get");
        if (auth != null) return auth;

        try
        {
            var items = DataProvider.fetch_item_pool().SearchItems(
                uid,
                description, 
                code, 
                upcCode, 
                modelNumber, 
                commodityCode, 
                supplierCode, 
                supplierPartNumber);

            if (items == null || !items.Any())
            {
                return NotFound("Error, er is geen Item(s) gevonden met deze gegevens.");
            }
            
            return Ok(items);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateItem([FromBody] Item item)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "post");
        if (auth != null) return auth;

        if (item.Uid != null) return BadRequest("Item: Uid should not be given a value in the body; Uid will be assigned automatically.");


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

        if (item.Uid != null) return BadRequest("Item: Uid should not be given a value in the body; Uid will be assigned automatically.");

        var success = DataProvider.fetch_item_pool().UpdateItem(id, item);
        if (!success) return NotFound("ID not found");

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

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteClient(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_item_pool().RemoveItem(id, true);

        DataProvider.fetch_item_pool().Save();
        return Ok();
    }
}