using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class WarehousesController : BaseApiController
{
    public WarehousesController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetWarehouses()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "get");
        if (auth != null) return auth;

        var warehouses = DataProvider.fetch_warehouse_pool().GetWarehouses();
        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public IActionResult GetWarehouse(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "getsingle");
        if (auth != null) return auth;

        var warehouse = DataProvider.fetch_warehouse_pool().GetWarehouse(id);
        if (warehouse == null) return NotFound();

        return Ok(warehouse);
    }

    [HttpGet("{id}/locations")]
    public IActionResult GetWarehouseLocations(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "getsingle");
        if (auth != null) return auth;

        var locations = DataProvider.fetch_location_pool().GetLocationsInWarehouse(id);
        return Ok(locations);
    }

    [HttpGet("search")]
    public IActionResult SearchWarehouses(
        [FromQuery] int? id = null,
        [FromQuery] string code = null, 
        [FromQuery] string name = null, 
        [FromQuery] string address = null,
        [FromQuery] string zip = null,
        [FromQuery] string city = null,
        [FromQuery] string province = null,
        [FromQuery] string country = null,
        [FromQuery] string createdAt = null,
        [FromQuery] string updatedAt = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "get");
        if (auth != null) return auth;

        try
        {
            var warehouses = DataProvider.fetch_warehouse_pool().SearchWarehouses(
                id,
                code, 
                name, 
                address, 
                zip, 
                city, 
                province, 
                country, 
                createdAt, 
                updatedAt);
            
            if (warehouses == null || !warehouses.Any())
            {
                return NoContent();
            }

            return Ok(warehouses);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // [HttpGet("{id}/inventory")]
    // public IActionResult GetWarehouseInventory(int id)
    // {
    //     var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "getsingle");
    //     if (auth != null) return auth;

    //     var inventory = DataProvider.fetch_inventory_pool().GetInventoryForWarehouse(id);
    //     return Ok(inventory);
    // }

    [HttpPost]
    public IActionResult CreateWarehouse([FromBody] Warehouse warehouse)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "post");
        if (auth != null) return auth;

        if (warehouse.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_warehouse_pool().AddWarehouse(warehouse);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_warehouse_pool().Save();
        return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "put");
        if (auth != null) return auth;

        if (warehouse.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_warehouse_pool().UpdateWarehouse(id, warehouse);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_warehouse_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateWarehouse(int id, [FromBody] JsonElement partialwarehouse)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "patch");
        if (auth != null) return auth;

        if (partialwarehouse.ValueKind != JsonValueKind.Object) 
            return BadRequest("Body must be an object");

        var warehousePool = DataProvider.fetch_warehouse_pool();
        var existingwarehouse = warehousePool.GetWarehouse(id);

        if (existingwarehouse == null) 
        return NotFound("Warehouse not found");

        if (partialwarehouse.TryGetProperty("Code", out var code))
        {
            existingwarehouse.Code = code.GetString();
        }

        if (partialwarehouse.TryGetProperty("Name", out var name))
        {
            existingwarehouse.Name = name.GetString();
        }

        if (partialwarehouse.TryGetProperty("Address", out var address))
        {
            existingwarehouse.Address = address.GetString();
        }

        if (partialwarehouse.TryGetProperty("Zip", out var zip))
        {
            existingwarehouse.Zip = zip.GetString();
        }

        if (partialwarehouse.TryGetProperty("City", out var city))
        {
            existingwarehouse.City = city.GetString();
        }

        if (partialwarehouse.TryGetProperty("Province", out var province))
        {
            existingwarehouse.Province = province.GetString();
        }

        if (partialwarehouse.TryGetProperty("Country", out var country))
        {
            existingwarehouse.Country = country.GetString();
        }

        if (partialwarehouse.TryGetProperty("Contact", out var contact))
        {
            if (contact.ValueKind == JsonValueKind.Object)
            {
                if (contact.TryGetProperty("Name", out var contactName))
                {
                    existingwarehouse.Contact.Name = contactName.GetString();
                }

                if (contact.TryGetProperty("Phone", out var contactPhone))
                {
                    existingwarehouse.Contact.Phone = contactPhone.GetString();
                }

                if (contact.TryGetProperty("Email", out var contactEmail))
                {
                    existingwarehouse.Contact.Email = contactEmail.GetString();
                }
            }
        }



        var success = warehousePool.ReplaceWarehouse(id, existingwarehouse);
        if (!success) 
            return StatusCode(500, "Failed to update warehouse");

        DataProvider.fetch_warehouse_pool().Save();
        return Ok(existingwarehouse);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteWarehouse(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_warehouse_pool().RemoveWarehouse(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_warehouse_pool().Save();
        return Ok();
    }
}