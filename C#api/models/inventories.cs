using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using Providers;
namespace Models;

public class Inventory
{
    public int Id { get; set; } = -10;
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
        if (inventory.Id == -10)
        {
            inventory.Id = _data.Count > 0 ? _data.Max(i => i.Id) + 1 : 1;
        }
        else if (_data.Exists(x => x.Id == inventory.Id))
        {
            return false;
        }

        if (inventory.Created_At == null) inventory.Created_At = GetTimestamp();
        if (inventory.Updated_At == null) inventory.Updated_At = GetTimestamp();
        _data.Add(inventory);
        return true;
    }

    public bool UpdateInventory(int inventoryId, Inventory inventory)
    {
        if (inventory.Id != inventoryId)
        {
            return false;
        }

        inventory.Updated_At = GetTimestamp();
        var index = _data.FindIndex(x => x.Id == inventoryId);
        if (index >= 0)
        {
            inventory.Created_At = _data[index].Created_At;
            _data[index] = inventory;
            return true;
        }

        return false;
    }

    public bool RemoveInventory(int inventoryId)
    {
        var inventory = GetInventory(inventoryId);
        if (inventory == null) return false;
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

