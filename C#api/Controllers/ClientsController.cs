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

    [HttpGet("{id}")]
    public IActionResult GetClient(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "getsingle");
        if (auth != null) return auth;

        var client = DataProvider.fetch_client_pool().GetClient(id);
        if (client == null) return NotFound();

        return Ok(client);
    }

    [HttpGet("{id}/orders")]
    public IActionResult GetClientOrders(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "getsingle");
        if (auth != null) return auth;

        var orders = DataProvider.fetch_order_pool().GetOrdersForClient(id);
        return Ok(orders);
    }

    [HttpPost]
    public IActionResult CreateClient([FromBody] Client client)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "post");
        if (auth != null) return auth;

        var success = DataProvider.fetch_client_pool().AddClient(client);
        if (!success) return BadRequest("Client: Id already exists");

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

