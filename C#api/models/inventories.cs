using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using ProvidersV2;
namespace ModelsV2;

public class Inventory
{
    public int? Id { get; set; }
    public string Item_Id { get; set; }
    public string Description { get; set; }
    public string Item_Reference { get; set; }
    public List<int> Locations { get; set; }
    public int Total_On_Hand { get; set; }
    public int Total_Expected { get; set; }
    public int Total_Ordered { get; set; }
    public int Total_Allocated { get; set; }
    public int Total_Available { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class Inventories : Base
{
    private string _dataPath;
    private List<Inventory> _data;

    public Inventories(string rootPath, bool isDebug = false)
    {
        _dataPath = Path.Combine(rootPath, "inventories.json");
        Load(isDebug);
    }

    public List<Inventory> GetInventories()
    {
        return _data;
    }

    public Inventory GetInventory(int inventoryId)
    {
        return _data.Find(x => x.Id == inventoryId);
    }

    public List<Inventory> GetInventoriesForItem(string itemId)
    {
        return _data.FindAll(x => x.Item_Id == itemId);
    }

    public Dictionary<string, int> GetInventoryTotalsForItem(string itemId)
    {
        var result = new Dictionary<string, int>
        {
            { "total_expected", 0 },
            { "total_ordered", 0 },
            { "total_allocated", 0 },
            { "total_available", 0 }
        };

        foreach (var x in _data)
        {
            if (x.Item_Id == itemId)
            {
                result["total_expected"] += Convert.ToInt32(x.Total_Expected);
                result["total_ordered"] += Convert.ToInt32(x.Total_Ordered);
                result["total_allocated"] += Convert.ToInt32(x.Total_Allocated);
                result["total_available"] += Convert.ToInt32(x.Total_Available);
            }
        }

        return result;
    }

    public bool AddInventory(Inventory inventory)
    {
        if (DataProvider.fetch_item_pool().GetItems().Any(x => x.Uid == inventory.Item_Id) && !DataProvider.fetch_item_pool().GetItems().Any(x => x.Uid == inventory.Item_Id && x.Code == inventory.Item_Reference)) return false;
        inventory.Id = _data.Count > 0 ? _data.Max(i => i.Id) + 1 : 1;
        if (inventory.Created_At == null) inventory.Created_At = GetTimestamp();
        if (inventory.Updated_At == null) inventory.Updated_At = GetTimestamp();
        _data.Add(inventory);
        return true;
    }

    public bool UpdateInventory(int inventoryId, Inventory inventory)
    {
        if (DataProvider.fetch_item_pool().GetItems().Any(x => x.Uid == inventory.Item_Id) && !DataProvider.fetch_item_pool().GetItems().Any(x => x.Uid == inventory.Item_Id && x.Code == inventory.Item_Reference)) return false;
        inventory.Updated_At = GetTimestamp();
        var index = _data.FindIndex(x => x.Id == inventoryId);
        if (index >= 0)
        {
            inventory.Id = _data[index].Id;
            inventory.Created_At = _data[index].Created_At;
            _data[index] = inventory;
            return true;
        }

        return false;
    }
    
    public bool ReplaceInventory(int inventoryId, Inventory newInventoryData)
    {
        var index = _data.FindIndex(existingInventory => existingInventory.Id == inventoryId);
        var existingInventory = _data.FirstOrDefault(existingInventory => existingInventory.Id == inventoryId);

        if (index < 0)
        {

            return false;

        }

        if (!string.IsNullOrEmpty(newInventoryData.Item_Id) || !string.IsNullOrEmpty(newInventoryData.Item_Reference))
        {
            string itemIdToCheck = !string.IsNullOrEmpty(newInventoryData.Item_Id) ? newInventoryData.Item_Id : existingInventory.Item_Id;
            string itemReferenceToCheck = !string.IsNullOrEmpty(newInventoryData.Item_Reference) ? newInventoryData.Item_Reference : existingInventory.Item_Reference;
            if (DataProvider.fetch_item_pool().GetItems().Any(x => x.Uid == itemIdToCheck) && 
                !DataProvider.fetch_item_pool().GetItems().Any(x => x.Uid == itemIdToCheck && x.Code == itemReferenceToCheck))
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(newInventoryData.Item_Id)) existingInventory.Item_Id = newInventoryData.Item_Id;
        if (!string.IsNullOrEmpty(newInventoryData.Description)) existingInventory.Description = newInventoryData.Description;
        if (!string.IsNullOrEmpty(newInventoryData.Item_Reference)) existingInventory.Item_Reference = newInventoryData.Item_Reference;
        if (newInventoryData.Locations != null && newInventoryData.Locations.Count > 0) existingInventory.Locations = newInventoryData.Locations;
        if (newInventoryData.Total_On_Hand != 0) existingInventory.Total_On_Hand = newInventoryData.Total_On_Hand;
        if (newInventoryData.Total_Expected != 0) existingInventory.Total_Expected = newInventoryData.Total_Expected;
        if (newInventoryData.Total_Ordered != 0) existingInventory.Total_Ordered = newInventoryData.Total_Ordered;
        if (newInventoryData.Total_Allocated != 0) existingInventory.Total_Allocated = newInventoryData.Total_Allocated;
        if (newInventoryData.Total_Available != 0) existingInventory.Total_Available = newInventoryData.Total_Available;
        existingInventory.Updated_At = GetTimestamp();

        return true;
    }

    public bool RemoveInventory(int inventoryId, bool force = false)
    {
        var inventory = GetInventory(inventoryId);
        if (inventory == null) return false;
        if (force) return _data.Remove(inventory);
        if (DataProvider.fetch_item_pool().GetItem(inventory.Item_Id) != null) return false;

        return _data.Remove(inventory);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            _data = new List<Inventory>();
        }
        else
        {
            using (var reader = new StreamReader(_dataPath))
            {
                var json = reader.ReadToEnd();
                _data = JsonConvert.DeserializeObject<List<Inventory>>(json);
            }
        }
    }

    public void Save()
    {
        using (var writer = new StreamWriter(_dataPath))
        {
            var json = JsonConvert.SerializeObject(_data, Formatting.Indented);
            writer.Write(json);
        }
    }
}

