using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using ProvidersV2;
namespace ModelsV2;

public class ItemGroup
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class ItemGroups : Base
{
    private readonly string _dataPath;
    private List<ItemGroup> _data;

    public ItemGroups(string rootPath, bool isDebug = false)
    {
        _dataPath = Path.Combine(rootPath, "item_groups.json");
        Load(isDebug);
    }

    public List<ItemGroup> GetItemGroups()
    {
        return _data;
    }

    public ItemGroup GetItemGroup(int itemGroupId)
    {
        return _data.Find(x => x.Id == itemGroupId);
    }

    public bool AddItemGroup(ItemGroup itemGroup)
    {

        itemGroup.Id = _data.Count > 0 ? _data.Max(ig => ig.Id) + 1 : 1;
        if (itemGroup.Created_At == null) itemGroup.Created_At = GetTimestamp();
        if (itemGroup.Updated_At == null) itemGroup.Updated_At = GetTimestamp();
        _data.Add(itemGroup);
        return true;
    }

    public bool UpdateItemGroup(int itemGroupId, ItemGroup itemGroup)
    {
        itemGroup.Updated_At = GetTimestamp();
        var index = _data.FindIndex(x => x.Id == itemGroupId);
        if (index >= 0)
        {
            itemGroup.Id = _data[index].Id;
            itemGroup.Created_At = _data[index].Created_At;
            _data[index] = itemGroup;
            return true;
        }

        return false;
    }

    public bool ReplaceItemGroup(int itemGroupId, ItemGroup newItemGroup)
    {
        var index = _data.FindIndex(existingItemGroup => existingItemGroup.Id == itemGroupId);
        var existingItemGroup = _data.FirstOrDefault(existingItemGroup => existingItemGroup.Id == itemGroupId);

        if (index < 0)
        {

            return false;

        }

        if (!string.IsNullOrEmpty(newItemGroup.Name)) existingItemGroup.Name = newItemGroup.Name;
        if (!string.IsNullOrEmpty(newItemGroup.Description)) existingItemGroup.Description = newItemGroup.Description;
        existingItemGroup.Updated_At = GetTimestamp();

        return true;
    }

    public bool RemoveItemGroup(int itemGroupId, bool force = false)
    {
        var itemGroup = GetItemGroup(itemGroupId);
        if (itemGroup == null) return false;
        if (force) return _data.Remove(itemGroup);
        var items = DataProvider.fetch_item_pool().GetItemsForItemGroup(itemGroupId);
        if (items.Count() != 0) return false;

        return _data.Remove(itemGroup);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            _data = new List<ItemGroup>();
        }
        else
        {
            using (var reader = new StreamReader(_dataPath))
            {
                var json = reader.ReadToEnd();
                _data = JsonConvert.DeserializeObject<List<ItemGroup>>(json);
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
