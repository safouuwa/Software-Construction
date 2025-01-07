using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.JsonPatch;

using Providers;
using System.Text.Json;

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
        [FromQuery] int? id = null,
        [FromQuery] string name = null,
        [FromQuery] string address = null, 
        [FromQuery] string city = null,
        [FromQuery] string zipCode = null,
        [FromQuery] string country = null,
        [FromQuery] string contactName = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "get");
        if (auth != null) return auth;

        try
        {
            var clients = DataProvider.fetch_client_pool().SearchClients(id,name, address, city, zipCode,country, contactName);
            
            if (clients == null || !clients.Any())
            {
                return NoContent();
            }
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

    [HttpPatch("{id}")]
    public IActionResult PartiallyUpdateClient(int id, [FromBody] JsonElement partialClient)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "clients", "patch");
        if (auth != null) return auth;

        if (partialClient.ValueKind == JsonValueKind.Undefined)
            return BadRequest("No updates provided");

        var clientPool = DataProvider.fetch_client_pool();
        var existingClient = clientPool.GetClient(id);

        if (existingClient == null)
            return NotFound("Client not found");

        if (partialClient.TryGetProperty("Name", out var name))
        {
            existingClient.Name = name.GetString();
        }

        if (partialClient.TryGetProperty("Address", out var address))
        {
            existingClient.Address = address.GetString();
        }

        if (partialClient.TryGetProperty("City", out var city))
        {
            existingClient.City = city.GetString();
        }

        if (partialClient.TryGetProperty("Zip_code", out var zipCode))
        {
            existingClient.Zip_code = zipCode.GetString();
        }

        if (partialClient.TryGetProperty("Province", out var province))
        {
            existingClient.Province = province.GetString();
        }

        if (partialClient.TryGetProperty("Country", out var country))
        {
            existingClient.Country = country.GetString();
        }

        if (partialClient.TryGetProperty("Contact_name", out var contactName))
        {
            existingClient.Contact_name = contactName.GetString();
        }

        if (partialClient.TryGetProperty("Contact_phone", out var contactPhone))
        {
            existingClient.Contact_phone = contactPhone.GetString();
        }

        if (partialClient.TryGetProperty("Contact_email", out var contactEmail))
        {
            existingClient.Contact_email = contactEmail.GetString();
        }


        var success = clientPool.ReplaceClient(id, existingClient);
        if (!success)
            return StatusCode(500, "Failed to update client");

        DataProvider.fetch_client_pool().Save();
        return Ok(existingClient);
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