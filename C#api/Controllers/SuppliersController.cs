using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ModelsV2;
using ProvidersV2;
using HelpersV2;
using ProcessorsV2;

[ApiController]
[Route("api/v2/[controller]")]
public class SuppliersController : BaseApiController
{
    public SuppliersController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetSuppliers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortOrder = "asc")
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
        if (auth != null) return auth;

        var suppliers = DataProvider.fetch_supplier_pool().GetSuppliers();
        suppliers = sortOrder.ToLower() == "desc"
            ? suppliers.OrderByDescending(c => c.Id).ToList()
            : suppliers.OrderBy(c => c.Id).ToList();
        var response = PaginationHelper.Paginate(suppliers, page, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetSupplier(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "getsingle");
        if (auth != null) return auth;

        var supplier = DataProvider.fetch_supplier_pool().GetSupplier(id);
        if (supplier == null) return NoContent();

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
        [FromQuery] string name = null, 
        [FromQuery] string country = null,
        [FromQuery] string code = null, 
        [FromQuery] string phoneNumber = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
        if (auth != null) return auth;

        try
        {
            var suppliers = DataProvider.fetch_supplier_pool().SearchSuppliers(
                name,  
                country, 
                code,
                phoneNumber
                );

            if (suppliers == null || !suppliers.Any())
            {
                return NoContent();
            }

            var response = PaginationHelper.Paginate(suppliers, page, pageSize);
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
        if (supplier.Id != null) return BadRequest("Supplier: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_supplier_pool().AddSupplier(supplier);
        if (!success) return BadRequest("Supplier: Id already exists");

        DataProvider.fetch_supplier_pool().Save();
        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateSupplier(int id, [FromBody] Supplier supplier)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "put");
        if (auth != null) return auth;

        if (supplier.Id != null) return BadRequest("Supplier: Id should not be given a value in the body; Id will be assigned automatically.");

        var success = DataProvider.fetch_supplier_pool().UpdateSupplier(id, supplier);
        if (!success) return NoContent();

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
            return NoContent();

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
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_supplier_pool().Save();
        return Ok();
    }

    [HttpDelete("{id}/force")]
    public IActionResult ForceDeleteSupplier(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "forcedelete");
        if (auth != null) return auth;
        
        DataProvider.fetch_supplier_pool().RemoveSupplier(id, true);
        DataProvider.fetch_supplier_pool().Save();
        return Ok();
    }
}