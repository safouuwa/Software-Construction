using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Models;
using Providers;
using Newtonsoft.Json;

public class TransferItem
{
    public string Item_Id { get; set; }
    public int Amount { get; set; }
}
public class Transfer
{
    public int? Id { get; set; }
    public string Reference { get; set; }
    public int? Transfer_From { get; set; } 
    public int? Transfer_To { get; set; }
    public string Transfer_Status { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
    public List<TransferItem> Items { get; set; }
}

public class Transfers : Base
{
    private string dataPath;
    private List<Transfer> data;

    public Transfers(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "transfers.json");
        Load(isDebug);
    }

    public List<Transfer> GetTransfers()
    {
        return data;
    }

    public Transfer GetTransfer(int transferId)
    {
        return data.FirstOrDefault(transfer => transfer.Id == transferId);
    }

    public List<TransferItem> GetItemsInTransfer(int transferId)
    {
        var transfer = GetTransfer(transferId);
        return transfer.Items;
    }

    public bool AddTransfer(Transfer transfer)
    {
        transfer.Id = data.Count > 0 ? data.Max(t => t.Id) + 1 : 1;
        transfer.Transfer_Status = "Scheduled";
        if (transfer.Created_At == null) transfer.Created_At = GetTimestamp();
        if (transfer.Updated_At == null) transfer.Updated_At = GetTimestamp();
        data.Add(transfer);
        return true;
    }

    public List<Transfer> SearchTransfers(int? transferFrom = null, int? transferTo = null, string transferStatus = null, string createdAt = null)
    {
        if (!transferFrom.HasValue && !transferTo.HasValue && string.IsNullOrEmpty(transferStatus) && string.IsNullOrEmpty(createdAt))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();

        if (transferFrom.HasValue)
        {
            query = query.Where(transfer => transfer.Transfer_From == transferFrom.Value);
        }

        if (transferTo.HasValue)
        {
            query = query.Where(transfer => transfer.Transfer_To == transferTo.Value);
        }

        if (!string.IsNullOrEmpty(transferStatus))
        {
            query = query.Where(transfer => transfer.Transfer_Status.Contains(transferStatus, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(createdAt))
        {
            query = query.Where(transfer => transfer.Created_At.Contains(createdAt, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public bool UpdateTransfer(int transferId, Transfer transfer)
    {
        transfer.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingTransfer => existingTransfer.Id == transferId);
        
        if (index >= 0)
        {
            transfer.Id = data[index].Id;
            transfer.Created_At = data[index].Created_At;
            data[index] = transfer;
            return true;
        }
        return false;
    }

    public bool ReplaceTransfer(int transferId, Transfer newTransferData)
    {
        var index = data.FindIndex(existingTransfer => existingTransfer.Id == transferId);
        var existingTransfer = data.FirstOrDefault(existingTransfer => existingTransfer.Id == transferId);

        if (index < 0)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(newTransferData.Reference)) existingTransfer.Reference = newTransferData.Reference;   
        if (newTransferData.Transfer_From != null) existingTransfer.Transfer_From = newTransferData.Transfer_From;
        if (newTransferData.Transfer_To != null) existingTransfer.Transfer_To = newTransferData.Transfer_To;
        if (!string.IsNullOrEmpty(newTransferData.Transfer_Status)) existingTransfer.Transfer_Status = newTransferData.Transfer_Status;
        existingTransfer.Updated_At = GetTimestamp();
        
        return true;
    }

    public bool RemoveTransfer(int transferId)
    {
        var transfer = GetTransfer(transferId);
        if (transfer == null) return false;

        return data.Remove(transfer);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Transfer>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = File.ReadAllText(dataPath);
                data = JsonConvert.DeserializeObject<List<Transfer>>(json);
            }
        }
    }

    public void Save()
    {
        using (var writer = new StreamWriter(dataPath))
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            writer.Write(json);
        }
    }
}
