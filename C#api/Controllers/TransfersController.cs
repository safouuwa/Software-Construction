using System.Text.Json;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Providers;
using Interfaces;
using Filters;
using Attributes;

[ApiController]
[Route("api/v1/[controller]")]
public class TransfersController : BaseApiController, ILoggableAction
{
    public TransfersController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }
    public object _dataBefore { get; set; }
    public object _dataAfter { get; set; }


    [HttpGet]
    public IActionResult GetTransfers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "get");
        if (auth != null) return auth;

        var transfers = DataProvider.fetch_transfer_pool().GetTransfers();
        var response = PaginationHelper.Paginate(transfers, page, pageSize);
        return Ok(response);
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
        [FromQuery] int? transferFrom = null,
        [FromQuery] int? transferTo = null,
        [FromQuery] string transferStatus = null,
        [FromQuery] string createdAt = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "get");
        if (auth != null) return auth;

        try
        {
            var transfers = DataProvider.fetch_transfer_pool().SearchTransfers(
                transferFrom, 
                transferTo, 
                transferStatus, 
                createdAt);

            if (transfers == null || !transfers.Any())
            {
                return NoContent();
            }

            var response = PaginationHelper.Paginate(transfers, page, pageSize);
            return Ok(transfers);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/locations")]
        public IActionResult GetTransferLocations(int id)
        {
            var transfer = DataProvider.fetch_transfer_pool().GetTransfer(id);
            if (transfer == null) return NoContent();
            
            var locationFrom = DataProvider.fetch_location_pool().GetLocation(transfer.Transfer_From ?? 0);
            var locationTo = DataProvider.fetch_location_pool().GetLocation(transfer.Transfer_To ?? 0);

            if (locationFrom == null && locationTo == null) return NoContent();

            return Ok(new { LocationFrom = locationFrom, LocationTo = locationTo });

        }

    [LogRequest]
    [HttpPost]
    public IActionResult CreateTransfer([FromBody] Transfer transfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "post");
        if (auth != null) return auth;
        if (transfer.Id != null) return BadRequest("Transfer: Id should not be given a value in the body; Id will be assigned automatically.");
        var success = DataProvider.fetch_transfer_pool().AddTransfer(transfer);
        if (!success) return BadRequest("Transfer: Id already exists");

        _dataBefore = null;
        _dataAfter = transfer;

        DataProvider.fetch_transfer_pool().Save();
        return CreatedAtAction(nameof(GetTransfer), new { id = transfer.Id }, transfer);
    }
    [LogRequest]
    [HttpPut("{id}")]
    public IActionResult UpdateTransfer(int id, [FromBody] Transfer transfer)
    {
        var auth = CheckAuthorization(Request.Headers["API_KEY"], "transfers", "put");
        if (auth != null) return auth;

        if (transfer.Id != null) return BadRequest("Transfer: Id should not be given a value in the body; Id will be assigned automatically.");


        var success = DataProvider.fetch_transfer_pool().UpdateTransfer(id, transfer);
        if (!success) return NoContent();

        _dataBefore = DataProvider.fetch_transfer_pool().GetTransfer(id);
        _dataAfter = transfer;

        DataProvider.fetch_transfer_pool().Save();
        return Ok();
    }

    [LogRequest]
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

        var originalFields = new Dictionary<string, object>();
        var newTransferData = new Transfer { Id = id };

        if (partialTransfer.TryGetProperty("Reference", out var reference))
        {
            originalFields["Reference"] = existingTransfer.Reference;
            newTransferData.Reference = reference.GetString();
        }

        if (partialTransfer.TryGetProperty("Transfer_From", out var transferFrom))
        {
            originalFields["Transfer_From"] = existingTransfer.Transfer_From;
            newTransferData.Transfer_From = transferFrom.GetInt32();
        }

        if (partialTransfer.TryGetProperty("Transfer_To", out var transferTo))
        {
            originalFields["Transfer_To"] = existingTransfer.Transfer_To;
            newTransferData.Transfer_To = transferTo.GetInt32();
        }

        if (partialTransfer.TryGetProperty("Transfer_Status", out var transferStatus))
        {
            originalFields["Transfer_Status"] = existingTransfer.Transfer_Status;
            newTransferData.Transfer_Status = transferStatus.GetString();
        }


        var success = transferPool.ReplaceTransfer(id, newTransferData);
        if (!success) 
            return NoContent();

        DataProvider.fetch_transfer_pool().Save();
        _dataBefore = originalFields;
        _dataAfter = partialTransfer;

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
        if (!success) return BadRequest("ID not found or other data is dependent on this data");

        DataProvider.fetch_transfer_pool().Save();
        return Ok();
    }
}