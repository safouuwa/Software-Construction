using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public class SuppliersController : BaseApiController
{
    public SuppliersController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetSuppliers()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
        if (auth != null) return auth;

        var suppliers = DataProvider.fetch_supplier_pool().GetSuppliers();
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public IActionResult GetSupplier(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "getsingle");
        if (auth != null) return auth;

        var supplier = DataProvider.fetch_supplier_pool().GetSupplier(id);
        if (supplier == null) return NotFound();

        return Ok(supplier);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetSupplierItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_item_pool().GetItemsForSupplier(id);
        return Ok(items);
    }

    [HttpGet("search")]
    public IActionResult SearchSuppliers(
        [FromQuery] int? id = null,
        [FromQuery] string name = null, 
        [FromQuery] string city = null, 
        [FromQuery] string country = null,
        [FromQuery] string code = null, 
        [FromQuery] string contactName = null,
        [FromQuery] string phoneNumber = null,
        [FromQuery] string reference = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
        if (auth != null) return auth;

        try
        {
            var suppliers = DataProvider.fetch_supplier_pool().SearchSuppliers(
                id,
                name, 
                city, 
                country, 
                code,
                contactName,
                phoneNumber,
                reference
                );

            if (suppliers == null || !suppliers.Any())
            {
                return NoContent();
            }

            return Ok(suppliers);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateSupplier([FromBody] Supplier supplier)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "post");
        if (auth != null) return auth;

        if (supplier.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_supplier_pool().AddSupplier(supplier);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_supplier_pool().Save();
        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateSupplier(int id, [FromBody] Supplier supplier)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "put");
        if (auth != null) return auth;

        if (supplier.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_supplier_pool().UpdateSupplier(id, supplier);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_supplier_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateSupplier(int id, [FromBody] JsonElement partialSupplier)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "patch");
        if (auth != null) return auth;

        if (partialSupplier.ValueKind == JsonValueKind.Undefined) 
            return BadRequest("No updates provided");
        var supplierPool = DataProvider.fetch_supplier_pool();
        var existingSupplier = supplierPool.GetSupplier(id);
        
        if (existingSupplier == null) 
            return NotFound("supplier not found");

        if (partialSupplier.TryGetProperty("Code", out var code))
        {
                existingSupplier.Code = code.GetString();
        }

        if (partialSupplier.TryGetProperty("Name", out var name))
        {
            existingSupplier.Name = name.GetString();
        }

        if (partialSupplier.TryGetProperty("Address", out var address))
        {
            existingSupplier.Address = address.GetString();
        }

        if (partialSupplier.TryGetProperty("Address_extra", out var addressExtra))
        {
            existingSupplier.Address_Extra = addressExtra.GetString();
        }

        if (partialSupplier.TryGetProperty("City", out var city))
        {
            existingSupplier.City = city.GetString();
        }

        if (partialSupplier.TryGetProperty("Zip_code", out var zipCode))
        {
            existingSupplier.Zip_Code = zipCode.GetString();
        }

        if (partialSupplier.TryGetProperty("Province", out var province))
        {
            existingSupplier.Province = province.GetString();
        }

        if (partialSupplier.TryGetProperty("Country", out var country))
        {
            existingSupplier.Country = country.GetString();
        }

        if (partialSupplier.TryGetProperty("Contact_name", out var contactName))
        {
            existingSupplier.Contact_Name = contactName.GetString();
        }

        if (partialSupplier.TryGetProperty("Phonenumber", out var phonenumber))
        {
            existingSupplier.Phonenumber = phonenumber.GetString();
        }

        if (partialSupplier.TryGetProperty("Reference", out var reference))
        {
            existingSupplier.Reference = reference.GetString();
        }

        var success = supplierPool.ReplaceSupplier(id, existingSupplier);
        if (!success) 
        return StatusCode(500,"Failed to update supplier");

        DataProvider.fetch_supplier_pool().Save();
        return Ok(existingSupplier);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSupplier(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_supplier_pool().RemoveSupplier(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_supplier_pool().Save();
        return Ok();
    }
}