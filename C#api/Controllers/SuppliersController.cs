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

    [HttpGet("search")]
    public IActionResult SearchSuppliers([FromQuery] string name, [FromQuery] string city, [FromQuery] string country)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
        if (auth != null) return auth;

        var suppliers = DataProvider.fetch_supplier_pool().SearchSuppliers(name, city, country);
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public IActionResult GetSupplier(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
        if (auth != null) return auth;

        var supplier = DataProvider.fetch_supplier_pool().GetSupplier(id);
        if (supplier == null) return NotFound();

        return Ok(supplier);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetSupplierItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "suppliers", "get");
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