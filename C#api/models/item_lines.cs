using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using Providers;
namespace Models;


public class ItemLine
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class ItemLines : Base
{
    private readonly string _dataPath;
    private List<ItemLine> _data;

    public ItemLines(string rootPath, bool isDebug = false)
    {
        _dataPath = Path.Combine(rootPath, "item_lines.json");
        Load(isDebug);
    }

    public List<ItemLine> GetItemLines()
    {
        return _data;
    }

    public ItemLine GetItemLine(int itemlineId)
    {
        return _data.Find(x => x.Id == itemlineId);
    }

    public bool AddItemline(ItemLine itemline)
    {
        itemline.Id = _data.Count > 0 ? _data.Max(il => il.Id) + 1 : 1;
        if (itemline.Created_At == null) itemline.Created_At = GetTimestamp();
        if (itemline.Updated_At == null) itemline.Updated_At = GetTimestamp();
        _data.Add(itemline);
        return true;
    }

    public bool UpdateItemline(int itemlineId, ItemLine itemline)
    {
        itemline.Updated_At = GetTimestamp();
        var index = _data.FindIndex(x => x.Id == itemlineId);
        if (index >= 0)
        {
            itemline.Id = _data[index].Id;
            itemline.Created_At = _data[index].Created_At;
            _data[index] = itemline;
            return true;
        }

        return false;
    }

    public bool ReplaceItemLine(int itemLineId , ItemLine newItemLine)
    {
        var index = _data.FindIndex(existingItemLine => existingItemLine.Id == itemLineId);
        var existingItemLine = _data.FirstOrDefault(existingItemLine => existingItemLine.Id == itemLineId);

        if (index < 0)
        {

            return false;

        }

        if (!string.IsNullOrEmpty(newItemLine.Name)) existingItemLine.Name = newItemLine.Name;
        if (!string.IsNullOrEmpty(newItemLine.Description)) existingItemLine.Description = newItemLine.Description;
        existingItemLine.Updated_At = GetTimestamp();

        return true;
    }

    public bool RemoveItemline(int itemlineId, bool force = false)
    {
        var itemline = GetItemLine(itemlineId);
        if (itemline == null) return false;
        if (force) return _data.Remove(itemline);
        var items = DataProvider.fetch_item_pool().GetItemsForItemLine(itemlineId);
        if (items.Count() != 0) return false;

        return _data.Remove(itemline);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            _data = new List<ItemLine>();
        }
        else
        {
            using (var reader = new StreamReader(_dataPath))
            {
                var json = reader.ReadToEnd();
                _data = JsonConvert.DeserializeObject<List<ItemLine>>(json);
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
