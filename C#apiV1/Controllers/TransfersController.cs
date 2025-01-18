using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;
namespace Processors;

[ApiController]
[Route("api/v1/[controller]")]
public class TransfersController : BaseApiController
{
    public TransfersController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    [HttpGet]
    public IActionResult GetTransfers()
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "get");
        if (auth != null) return auth;

        var transfers = DataProvider.fetch_transfer_pool().GetTransfers();
        return Ok(transfers);
    }

    [HttpGet("{id}")]
    public IActionResult GetTransfer(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "get");
        if (auth != null) return auth;

        var transfer = DataProvider.fetch_transfer_pool().GetTransfer(id);
        if (transfer == null) return NotFound();

        return Ok(transfer);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetTransferItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "get");
        if (auth != null) return auth;

        var items = DataProvider.fetch_transfer_pool().GetItemsInTransfer(id);
        return Ok(items);
    }

    [HttpPost]
    public IActionResult CreateTransfer([FromBody] Transfer transfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "post");
        if (auth != null) return auth;

        if (transfer.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_transfer_pool().AddTransfer(transfer);
        if (!success) return NotFound("ID already exists in data");

        DataProvider.fetch_transfer_pool().Save();
        return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTransfer(int id, [FromBody] Transfer transfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "put");
        if (auth != null) return auth;

        if (transfer.Id == -10) return BadRequest("ID not given in body");

        var success = DataProvider.fetch_transfer_pool().UpdateTransfer(id, transfer);
        if (!success) return NotFound("ID not found or ID in Body and Route are not matching");

        DataProvider.fetch_transfer_pool().Save();
        return Ok();
    }

    // [HttpPut("{id}/items")]
    // public IActionResult UpdateTransferItems(int id, [FromBody] List<Item> items)
    // {
    //     var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "put");
    //     if (auth != null) return auth;

    //     if (items.Any(x => x.Uid == null)) return BadRequest("ID not given in body");

    //     if (DataProvider.fetch_transfer_pool().GetTransfer(id) == null)
    //         return NotFound("No data for given ID");

    //     DataProvider.fetch_item_pool().UpdateItemsInTransfer(id, items);
    //     DataProvider.fetch_transfer_pool().Save();
    //     return Ok();
    // }

    [HttpPut("{id}/commit")]
    public IActionResult CommitTransfer(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "put");
        if (auth != null) return auth;

        var transfer = DataProvider.fetch_transfer_pool().GetTransfer(id);
        if (transfer == null) return NotFound("Transfer not found");

        if (transfer.Id == -10) return BadRequest("Invalid transfer ID");

        // Update inventories based on transfer items
        foreach (var item in transfer.Items)
        {
            var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(item.Item_Id);
            foreach (var inventory in inventories)
            {
                if (transfer.Transfer_From is null) break;
                if (inventory.Locations.Contains((int)transfer.Transfer_From))
                {
                    inventory.Total_On_Hand -= item.Amount;
                }
                else if (inventory.Locations.Contains((int)transfer.Transfer_To))
                {
                    inventory.Total_On_Hand += item.Amount;
                }
                DataProvider.fetch_inventory_pool().UpdateInventory(inventory.Id, inventory);
            }
        }

        transfer.Transfer_Status = "Processed";
        var success = DataProvider.fetch_transfer_pool().UpdateTransfer(id, transfer);
        if (!success) return NotFound("Failed to update transfer status");

        _notificationSystem.Push($"Processed batch transfer with id: {transfer.Id}");
        
        DataProvider.fetch_transfer_pool().Save();
        DataProvider.fetch_inventory_pool().Save();
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTransfer(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "delete");
        if (auth != null) return auth;

        var success = DataProvider.fetch_transfer_pool().RemoveTransfer(id);
        if (!success) return NotFound("ID not found or other data is dependent on this data");

        DataProvider.fetch_transfer_pool().Save();
        return Ok();
    }
}