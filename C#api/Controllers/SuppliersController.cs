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

        if (partialSupplier.TryGetProperty("code", out var codeElement))
        {
                existingSupplier.Code = codeElement.GetString();
        }

        if (partialSupplier.TryGetProperty("name", out var nameElement))
        {
            existingSupplier.Name = nameElement.GetString();
        }

        if (partialSupplier.TryGetProperty("address", out var addressElement))
        {
            existingSupplier.Address = addressElement.GetString();
        }

        if (partialSupplier.TryGetProperty("address_extra", out var addressExtraElement))
        {
            existingSupplier.Address_Extra = addressExtraElement.GetString();
        }

        if (partialSupplier.TryGetProperty("city", out var cityElement))
        {
            existingSupplier.City = cityElement.GetString();
        }

        if (partialSupplier.TryGetProperty("zip_code", out var zipCodeElement))
        {
            existingSupplier.Zip_Code = zipCodeElement.GetString();
        }

        if (partialSupplier.TryGetProperty("province", out var provinceElement))
        {
            existingSupplier.Province = provinceElement.GetString();
        }

        if (partialSupplier.TryGetProperty("country", out var countryElement))
        {
            existingSupplier.Country = countryElement.GetString();
        }

        if (partialSupplier.TryGetProperty("contact_name", out var contactNameElement))
        {
            existingSupplier.Contact_Name = contactNameElement.GetString();
        }

        if (partialSupplier.TryGetProperty("phonenumber", out var phonenumberElement))
        {
            existingSupplier.Phonenumber = phonenumberElement.GetString();
        }

        if (partialSupplier.TryGetProperty("reference", out var referenceElement))
        {
            existingSupplier.Reference = referenceElement.GetString();
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