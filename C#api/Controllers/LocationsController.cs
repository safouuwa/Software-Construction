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
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth != null) return auth;

        var location = DataProvider.fetch_location_pool().GetLocation(id);
        if (location == null) return NotFound();

        return Ok(location);
    }

    [HttpGet("{id}/inventory")]
    public IActionResult GetLocationInventory(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth != null) return auth;

        var inventory = DataProvider.fetch_location_pool().GetLocationsInWarehouse(id);
        return Ok(inventory);
    }

    [HttpGet("search")]
    public IActionResult SearchLocations(
        [FromQuery] string name = null, 
        [FromQuery] string created_At = null, 
        [FromQuery] string updated_At = null, 
        [FromQuery] int? warehouseId = null, 
        [FromQuery] string code = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth != null) return auth;

        try
        {
            var locations = DataProvider.fetch_location_pool().SearchLocations(
                name, 
                created_At, 
                updated_At, 
                warehouseId, 
                code);
            return Ok(locations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
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