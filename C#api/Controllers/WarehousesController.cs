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
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "get");
        if (auth != null) return auth;

        var warehouse = DataProvider.fetch_warehouse_pool().GetWarehouse(id);
        if (warehouse == null) return NotFound();

        return Ok(warehouse);
    }

    [HttpGet("{id}/locations")]
    public IActionResult GetWarehouseLocations(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "get");
        if (auth != null) return auth;

        var locations = DataProvider.fetch_location_pool().GetLocationsInWarehouse(id);
        return Ok(locations);
    }

    // [HttpGet("{id}/inventory")]
    // public IActionResult GetWarehouseInventory(int id)
    // {
    //     var auth = CheckAuthorization(Request.Headers["API_KEY"], "warehouses", "get");
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