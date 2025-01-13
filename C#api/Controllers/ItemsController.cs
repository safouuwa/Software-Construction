using System.Text.Json;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;
using Attributes;

[ApiController]
[Route("api/v1/[controller]")]
public class ItemsController : BaseApiController, ILoggableAction
{
    public ItemsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    private object _dataBefore;
    private object _dataAfter;
    public object GetDataBefore() => _dataBefore;
    public object GetDataAfter() => _dataAfter;

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
        if (item == null) return NoContent();

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
                return NoContent();
            }
            
            return Ok(items);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [LogRequest]
    [HttpPost]
    public IActionResult CreateItem([FromBody] Item item)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "post");
        if (auth != null) return auth;

        if (item.Uid != null) return BadRequest("Item: Uid should not be given a value in the body; Uid will be assigned automatically.");


        var success = DataProvider.fetch_item_pool().AddItem(item);
        if (!success) return BadRequest("ID already exists in data");

        DataProvider.fetch_item_pool().Save();
        return CreatedAtAction(nameof(GetItem), new { id = item.Uid }, item);
    }
    [LogRequest]
    [HttpPut("{id}")]
    public IActionResult UpdateItem(string id, [FromBody] Item item)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "put");
        if (auth != null) return auth;

        if (item.Uid != null) return BadRequest("Item: Uid should not be given a value in the body; Uid will be assigned automatically.");

        var success = DataProvider.fetch_item_pool().UpdateItem(id, item);
        if (!success) return NoContent();

        _dataBefore = null;
        _dataAfter = item;

        DataProvider.fetch_item_pool().Save();
        return Ok();
    }
    [LogRequest]
    [HttpPatch("{id}")]
    public IActionResult PartialUpdateItems(string id, [FromBody] JsonElement partialItem)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "patch");
        if (auth != null) return auth;

        if (partialItem.ValueKind != JsonValueKind.Object)
            return BadRequest("Body must be an object");

        var itemPool = DataProvider.fetch_item_pool();
        var excistingItem = itemPool.GetItem(id);
        var originalFields = new Dictionary<string, object>();

        if (excistingItem == null) 
            return NoContent();

        if (partialItem.TryGetProperty("Code", out var code))
        {  
            originalFields["Code"] = excistingItem.Code;
            excistingItem.Code = code.GetString();
        }

        if (partialItem.TryGetProperty("Description", out var description))
        {
            originalFields["Description"] = excistingItem.Description;
            excistingItem.Description = description.GetString();
        }

        if (partialItem.TryGetProperty("Short_Description", out var shortDescription))
        {
            originalFields["Short_Description"] = excistingItem.Short_Description;
            excistingItem.Short_Description = shortDescription.GetString();
        }

        if (partialItem.TryGetProperty("Upc_Code", out var upcCode))
        {
            originalFields["Upc_Code"] = excistingItem.Upc_Code;
            excistingItem.Upc_Code = upcCode.GetString();
        }

        if (partialItem.TryGetProperty("Model_Number", out var modelNumber))
        {
            originalFields["Model_Number"] = excistingItem.Model_Number;
            excistingItem.Model_Number = modelNumber.GetString();
        }

        if (partialItem.TryGetProperty("Commodity_Code", out var commodityCode))
        {
            originalFields["Commodity_Code"] = excistingItem.Commodity_Code;
            excistingItem.Commodity_Code = commodityCode.GetString();
        }

        if (partialItem.TryGetProperty("Item_Line", out var itemLine))
        {
            originalFields["Item_Line"] = excistingItem.Item_Line;
            excistingItem.Item_Line = itemLine.GetInt32();
        }

        if (partialItem.TryGetProperty("Item_Group", out var itemGroup))
        {
            originalFields["Item_Group"] = excistingItem.Item_Group;
            excistingItem.Item_Group = itemGroup.GetInt32();
        }

        if (partialItem.TryGetProperty("Item_Type", out var itemType))
        {
            originalFields["Item_Type"] = excistingItem.Item_Type;
            excistingItem.Item_Type = itemType.GetInt32();
        }

        if (partialItem.TryGetProperty("Unit_Purchase_Quantity", out var unitPurchaseQuantity))
        {
            originalFields["Unit_Purchase_Quantity"] = excistingItem.Unit_Purchase_Quantity;
            excistingItem.Unit_Purchase_Quantity = unitPurchaseQuantity.GetInt32();
        }

        if (partialItem.TryGetProperty("Unit_Order_Quantity", out var unitOrderQuantity))
        {
            originalFields["Unit_Order_Quantity"] = excistingItem.Unit_Order_Quantity;
            excistingItem.Unit_Order_Quantity = unitOrderQuantity.GetInt32();
        }

        if (partialItem.TryGetProperty("Pack_Order_Quantity", out var packOrderQuantity))
        {
            originalFields["Pack_Order_Quantity"] = excistingItem.Pack_Order_Quantity;
            excistingItem.Pack_Order_Quantity = packOrderQuantity.GetInt32();
        }

        if (partialItem.TryGetProperty("Supplier_Id", out var supplierId))
        {
            originalFields["Supplier_Id"] = excistingItem.Supplier_Id;
            excistingItem.Supplier_Id = supplierId.GetInt32();
        }

        if (partialItem.TryGetProperty("Supplier_Code", out var supplierCode))
        {
            originalFields["Supplier_Code"] = excistingItem.Supplier_Code;
            excistingItem.Supplier_Code = supplierCode.GetString();
        }

        if (partialItem.TryGetProperty("Supplier_Part_Number", out var supplierPartNumber))
        {
            originalFields["Supplier_Part_Number"] = excistingItem.Supplier_Part_Number;
            excistingItem.Supplier_Part_Number = supplierPartNumber.GetString();
        }

        
        var success = itemPool.ReplaceItem(id, excistingItem);
        if (!success) return NoContent();

        _dataBefore = originalFields;
        _dataAfter = partialItem;

        DataProvider.fetch_item_pool().Save();
        return Ok(excistingItem);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItem(string id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "items", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_item_pool().RemoveItem(id);
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

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