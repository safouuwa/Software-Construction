using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using ProvidersV2;
namespace ModelsV2;

public class ItemType
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}


public class ItemTypes : Base
{
    private readonly string _dataPath;
    private List<ItemType> _data;

    public ItemTypes(string rootPath, bool isDebug = false)
    {
        _dataPath = Path.Combine(rootPath, "item_types.json");
        Load(isDebug);
    }

    public List<ItemType> GetItemTypes()
    {
        return _data;
    }

    public ItemType GetItemType(int itemtypeId)
    {
        return _data.Find(x => x.Id == itemtypeId);
    }

    public bool AddItemtype(ItemType itemtype)
    {
        itemtype.Id = _data.Count > 0 ? _data.Max(it => it.Id) + 1 : 1;
        if (itemtype.Created_At == null) itemtype.Created_At = GetTimestamp();
        if (itemtype.Updated_At == null) itemtype.Updated_At = GetTimestamp();
        _data.Add(itemtype);
        return true;
    }

    public bool UpdateItemtype(int itemtypeId, ItemType itemtype)
    {
        itemtype.Updated_At = GetTimestamp();
        var index = _data.FindIndex(x => x.Id == itemtypeId);
        if (index >= 0)
        {
            itemtype.Id = _data[index].Id;
            itemtype.Created_At = _data[index].Created_At;
            _data[index] = itemtype;
            return true;
        }

        return false;
    }
        public bool ReplaceItemType(int itemTypeId , ItemType newItemType)
    {
        var index = _data.FindIndex(existingItemType => existingItemType.Id == itemTypeId);
        var existingItemType = _data.FirstOrDefault(existingItemType => existingItemType.Id == itemTypeId);

        if (index < 0)
        {

            return false;

        }

        if (!string.IsNullOrEmpty(newItemType.Name)) existingItemType.Name = newItemType.Name;
        if (!string.IsNullOrEmpty(newItemType.Description)) existingItemType.Description = newItemType.Description;
        existingItemType.Updated_At = GetTimestamp();

        return true;
    }

    public bool RemoveItemtype(int itemtypeId, bool force = false)
    {
        var itemtype = GetItemType(itemtypeId);
        if (itemtype == null) return false;
        if (force) return _data.Remove(itemtype);
        var items = DataProvider.fetch_item_pool().GetItemsForItemType(itemtypeId);
        if (items.Count() != 0) return false;

        return _data.Remove(itemtype);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            _data = new List<ItemType>();
        }
        else
        {
            using (var reader = new StreamReader(_dataPath))
            {
                var json = reader.ReadToEnd();
                _data = JsonConvert.DeserializeObject<List<ItemType>>(json);
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
