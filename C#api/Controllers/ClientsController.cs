using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClientsController : BaseApiController
{
    public ClientsController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetClients()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "get");
        if (auth != null) return auth;

        var clients = DataProvider.fetch_client_pool().GetClients();
        return Ok(clients);
    }

    [HttpGet("search")]
    public IActionResult SearchClients(
        [FromQuery] string name = null,
        [FromQuery] string address = null, 
        [FromQuery] string city = null,
        [FromQuery] string zipCode = null,
        [FromQuery] string province = null,
        [FromQuery] string country = null,
        [FromQuery] string contactName = null,
        [FromQuery] string contactPhone = null,
        [FromQuery] string contactEmail = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "get");
        if (auth != null) return auth;

        try
        {
            var clients = DataProvider.fetch_client_pool().SearchClients(name, address, city, zipCode, province, country, contactName, contactPhone, contactEmail);
            return Ok(clients);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetClient(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "get");
        if (auth != null) return auth;

        var client = DataProvider.fetch_client_pool().GetClient(id);
        if (client == null) return NotFound();

        return Ok(client);
    }

    [HttpGet("{id}/orders")]
    public IActionResult GetClientOrders(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "get");
        if (auth != null) return auth;

        var orders = DataProvider.fetch_order_pool().GetOrdersForClient(id);
        return Ok(orders);
    }

    [HttpPost]
    public IActionResult CreateClient([FromBody] Client client)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "post");
        if (auth != null) return auth;

        if (client.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_client_pool().AddClient(client);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_client_pool().Save();
        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateClient(int id, [FromBody] Client client)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "put");
        if (auth != null) return auth;

        if (client.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_client_pool().UpdateClient(id, client);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_client_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteClient(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_client_pool().RemoveClient(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_client_pool().Save();
        return Ok();
    }
}