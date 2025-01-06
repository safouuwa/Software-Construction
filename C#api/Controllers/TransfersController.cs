using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;

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
        if (transfer == null) return NoContent();

        return Ok(transfer);
    }

    [HttpGet("{id}/items")]
    public IActionResult GetTransferItems(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "getsingle");
        if (auth != null) return auth;

        var items = DataProvider.fetch_transfer_pool().GetItemsInTransfer(id);
        return Ok(items);
    }

    [HttpGet("{id}/status")]
    public IActionResult GetTransferStatus(int id)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "getsingle");
        if (auth != null) return auth;

        var transfer = DataProvider.fetch_transfer_pool().GetTransfer(id);
        if (transfer == null) return NoContent();

        return Ok(transfer.Transfer_Status);
    }

    [HttpGet("search")]
    public IActionResult SearchTransfers(
        [FromQuery] int? id = null,
        [FromQuery] string reference = null,
        [FromQuery] int? transferFrom = null,
        [FromQuery] int? transferTo = null,
        [FromQuery] string transferStatus = null,
        [FromQuery] string createdAt = null)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "get");
        if (auth != null) return auth;

        try
        {
            var transfers = DataProvider.fetch_transfer_pool().SearchTransfers(
                id,
                reference, 
                transferFrom, 
                transferTo, 
                transferStatus, 
                createdAt);

            if (transfers == null || !transfers.Any())
            {
                return NoContent();
            }

            return Ok(transfers);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateTransfer([FromBody] Transfer transfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "post");
        if (auth != null) return auth;
        if (transfer.Id != null) return BadRequest("Transfer: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_transfer_pool().AddTransfer(transfer);
        if (!success) return BadRequest("Transfer: Id already exists");

        DataProvider.fetch_transfer_pool().Save();
        return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTransfer(int id, [FromBody] Transfer transfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "put");
        if (auth != null) return auth;

        if (transfer.Id != null) return BadRequest("Transfer: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_transfer_pool().UpdateTransfer(id, transfer);
        if (!success) return NoContent();

        DataProvider.fetch_transfer_pool().Save();
        return Ok();
    }

    [HttpPatch("{id}")]
    public IActionResult PartialUpdateTransfer(int id, [FromBody] JsonElement partialTransfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "patch");
        if (auth != null) return auth;

        if(partialTransfer.ValueKind == JsonValueKind.Undefined)
            return BadRequest("No Updates Provided");

        var transferPool = DataProvider.fetch_transfer_pool();
        var existingTransfer = transferPool.GetTransfer(id);

        if (existingTransfer == null) 
        return NoContent();

        if (partialTransfer.TryGetProperty("Reference", out var reference))
        {
            existingTransfer.Reference = reference.GetString();
        }

        if (partialTransfer.TryGetProperty("Transfer_From", out var transferFrom))
        {
            existingTransfer.Transfer_From = transferFrom.GetInt32();
        }

        if (partialTransfer.TryGetProperty("Transfer_To", out var transferTo))
        {
            existingTransfer.Transfer_To = transferTo.GetInt32();
        }

        if (partialTransfer.TryGetProperty("Transfer_Status", out var transferStatus))
        {
            existingTransfer.Transfer_Status = transferStatus.GetString();
        }

        var success = transferPool.ReplaceTransfer(id, existingTransfer);
        if (!success) 
            return StatusCode(500,"Failed to update transfer");


        DataProvider.fetch_transfer_pool().Save();
        return Ok(existingTransfer);
    }


    // [HttpPut("{id}/items")]
    // public IActionResult UpdateTransferItems(int id, [FromBody] List<Item> items)
    // {
    //     var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "put");
    //     if (auth != null) return auth;

    //     if (items.Any(x => x.Uid == null)) return BadRequest("ID not given in body");

    //     if (DataProvider.fetch_transfer_pool().GetTransfer(id) == null)
    //         return NoContent();

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
        if (transfer == null) return NoContent();

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
                DataProvider.fetch_inventory_pool().UpdateInventory((int)inventory.Id, inventory);
            }
        }

        transfer.Transfer_Status = "Processed";
        var success = DataProvider.fetch_transfer_pool().UpdateTransfer(id, transfer);
        if (!success) return NoContent();

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
        if (!success) return NoContent();

        DataProvider.fetch_transfer_pool().Save();
        return Ok();
    }
}