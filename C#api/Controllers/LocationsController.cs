using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class LocationsController : BaseApiController
{
    public LocationsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetLocations()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth != null) return auth;

        var locations = DataProvider.fetch_location_pool().GetLocations();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public IActionResult GetLocation(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "getsingle");
        if (auth != null) return auth;

        var location = DataProvider.fetch_location_pool().GetLocation(id);
        if (location == null) return NotFound();

        return Ok(location);
    }

    [HttpGet("{id}/inventory")]
    public IActionResult GetLocationInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "getsingle");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_location_pool().GetLocationsInWarehouse(id);
        return Ok(inventory);
    }

    [HttpPost]
    public IActionResult CreateLocation([FromBody] Location location)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "post");
        if (auth != null) return auth;

        if (location.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_location_pool().AddLocation(location);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_location_pool().Save();
        return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateLocation(int id, [FromBody] Location location)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "put");
        if (auth != null) return auth;

        if (location.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_location_pool().UpdateLocation(id, location);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_location_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateLocation(int id, [FromBody] JsonElement partiallocation)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "patch");
        if (auth != null) return auth;

        if (partiallocation.ValueKind == JsonValueKind.Undefined)
        {
            return BadRequest("No updates provided");
        }

        var locationPool = DataProvider.fetch_location_pool();
        var existingLocation = locationPool.GetLocation(id);

        if (existingLocation == null)
        {
            return NotFound("Location not found");
        }

        if (partiallocation.TryGetProperty("Code", out var code))
        {
            existingLocation.Code = code.GetString();
        }

        if (partiallocation.TryGetProperty("Name", out var name))
        {
            existingLocation.Name = name.GetString();
        }

        if (partiallocation.TryGetProperty("Warehouse_Id", out var warehouseId))
        {
            existingLocation.Warehouse_Id = warehouseId.GetInt32();
        }

        var success = locationPool.ReplaceLocation(id, existingLocation);
        if (!success) 
            return StatusCode(500, "Failed to update location");

        DataProvider.fetch_location_pool().Save();
        return Ok(existingLocation);
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteLocation(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_location_pool().RemoveLocation(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_location_pool().Save();
        return Ok();
    }
}