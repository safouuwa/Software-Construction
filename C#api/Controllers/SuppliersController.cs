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
                reference);

            if (suppliers == null || !suppliers.Any())
            {
                return NotFound("Error, er is geen Supplier(s) gevonden met deze gegevens.");
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
        if (!success) return NotFound("ID not found");

        DataProvider.fetch_supplier_pool().Save();
        return Ok();
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