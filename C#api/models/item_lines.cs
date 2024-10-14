using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;


public class ItemLine
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Created_At { get; set; }
    public string Updated_At { get; set; }
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
        if (_data.Exists(x => Convert.ToInt64(x.Id) == itemline.Id))
        {
            return false;
        }

        itemline.Created_At = GetTimestamp();
        itemline.Updated_At = GetTimestamp();
        _data.Add(itemline);
        return true;
    }

    public bool UpdateItemline(int itemlineId, ItemLine itemline)
    {
        if (itemline.Id != itemlineId)
        {
            return false;
        }

        itemline.Updated_At = GetTimestamp();
        var index = _data.FindIndex(x => x.Id == itemlineId);
        if (index >= 0)
        {
            _data[index] = itemline;
            return true;
        }

        return false;
    }

    public bool RemoveItemline(int itemlineId)
    {
        var itemline = GetItemLine(itemlineId);
        if (itemline == null) return false;
        var items = DataProvider.fetch_item_pool().GetItemsForItemLine(itemlineId);
        if (items.Count <= 0) return false;

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
