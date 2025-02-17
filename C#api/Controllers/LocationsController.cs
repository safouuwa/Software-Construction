using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ModelsV2;
using ProvidersV2;
using HelpersV2;
using ProcessorsV2;

[ApiController]
[Route("api/v2/[controller]")]
public class LocationsController : BaseApiController
{
    public LocationsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetLocations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth is UnauthorizedResult) return auth;

        var locations = DataProvider.fetch_location_pool().GetLocations();

        if (auth is OkResult)
        {
            var user = AuthProvider.GetUser(Request.Headers["API_KEY"]);
            locations = locations.Where(x => user.OwnWarehouses.Contains(x.Warehouse_Id)).ToList();
        }

        var response = PaginationHelper.Paginate(locations, page, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetLocation(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "getsingle");
        if (auth != null) return auth;

        var location = DataProvider.fetch_location_pool().GetLocation(id);
        if (location == null) return NoContent();

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

    [HttpGet("search")]
    public IActionResult SearchLocations(
        [FromQuery] string name = null, 
        [FromQuery] int? warehouseId = null, 
        [FromQuery] string code = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth != null) return auth;

        try
        {
            var locations = DataProvider.fetch_location_pool().SearchLocations(
                name, 
                warehouseId, 
                code);
            
            if (locations == null || !locations.Any())
            {
                return NoContent();
            }

            var response = PaginationHelper.Paginate(locations, page, pageSize);
            return Ok(locations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/warehouse")]
    public IActionResult GetLocationWarehouse(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "locations", "get");
        if (auth is UnauthorizedResult) return auth;

        var location = DataProvider.fetch_location_pool().GetLocation(id);
        if (location == null)
        {
            return NoContent();
        }

        var warehouse = DataProvider.fetch_warehouse_pool().GetWarehouse(location.Warehouse_Id);
        if (warehouse == null) return NoContent();

        return Ok(warehouse);
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
        if (!success) return NoContent();

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
            return NoContent();
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
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

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