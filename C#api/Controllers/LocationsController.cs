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
        if (location.Id != null) return BadRequest("Location: Id should not be given a value in the body; Id will be assigned automatically.");

        var success = DataProvider.fetch_location_pool().AddLocation(location);
        if (!success) return BadRequest("Location: Id already exists");

        DataProvider.fetch_location_pool().Save();
        return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateLocation(int id, [FromBody] Location location)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "put");
        if (auth != null) return auth;

        if (location.Id != null) return BadRequest("Location: Id should not be given a value in the body; Id will be assigned automatically.");

        var success = DataProvider.fetch_location_pool().UpdateLocation(id, location);
        if (!success) return NotFound("ID not found");

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

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteLocation(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_location_pool().RemoveLocation(id, true);
        DataProvider.fetch_location_pool().Save();
        return Ok();
    }
}