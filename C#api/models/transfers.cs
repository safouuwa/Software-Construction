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
    public int Id { get; set; } = -10;
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
        if (transfer.Id == -10)
        {
            transfer.Id = data.Count > 0 ? data.Max(t => t.Id) + 1 : 1;
        }
        else if (data.Any(existingTransfer => existingTransfer.Id == transfer.Id))
        {
            return false;
        }

        transfer.Transfer_Status = "Scheduled";
        if (transfer.Created_At == null) transfer.Created_At = GetTimestamp();
        if (transfer.Updated_At == null) transfer.Updated_At = GetTimestamp();
        data.Add(transfer);
        return true;
    }

    public bool UpdateTransfer(int transferId, Transfer transfer)
    {
        if (transfer.Id != transferId)
        {
            return false;
        }

        transfer.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingTransfer => existingTransfer.Id == transferId);
        
        if (index >= 0)
        {
            transfer.Created_At = data[index].Created_At;
            data[index] = transfer;
            return true;
        }
        return false;
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
