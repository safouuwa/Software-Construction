using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Providers;
namespace Models;


public class Item
{
    public string Uid { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Short_Description { get; set; }
    public string Upc_Code { get; set; }
    public string Model_Number { get; set; }
    public string Commodity_Code { get; set; }
    public int Item_Line { get; set; }
    public int Item_Group { get; set; }
    public int Item_Type { get; set; }
    public int Unit_Purchase_Quantity { get; set; }
    public int Unit_Order_Quantity { get; set; }
    public int Pack_Order_Quantity { get; set; }
    public int Supplier_Id { get; set; }
    public string Supplier_Code { get; set; }
    public string Supplier_Part_Number { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class Items : Base
{
    private string dataPath;
    private List<Item> data;

    public Items(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "items.json");
        Load(isDebug);
    }

    public List<Item> GetItems()
    {
        return data;
    }

    public Item GetItem(string itemId)
    {
        return data.Find(item => item.Uid == itemId);
    }

    public List<Item> GetItemsForItemLine(int itemLineId)
    {
        return data.Where(item => item.Item_Line == itemLineId).ToList();
    }

    public List<Item> GetItemsForItemGroup(int itemGroupId)
    {
        return data.Where(item => item.Item_Group == itemGroupId).ToList();
    }

    public List<Item> GetItemsForItemType(int itemTypeId)
    {
        return data.Where(item => item.Item_Type == itemTypeId).ToList();
    }

    public List<Item> GetItemsForSupplier(int supplierId)
    {
        return data.Where(item => item.Supplier_Id== supplierId).ToList();
    }

    public bool AddItem(Item item)
    {
        if (data.Any(existingItem => existingItem.Uid== item.Uid))
        {
            return false;
        }

        if (item.Created_At == null) item.Created_At = GetTimestamp();
        if (item.Updated_At == null) item.Updated_At = GetTimestamp();
        data.Add(item);
        return true;
    }

    public List<Item> SearchItems(string description = null, string code = null, string upcCode = null, string modelNumber = null, string commodityCode = null, string supplierCode = null, string supplierPartNumber = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = data.AsQueryable();

        if (!string.IsNullOrEmpty(description))
        {
            query = query.Where(item => item.Description.Contains(description, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(item => item.Code.Contains(code, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(upcCode))
        {
            query = query.Where(item => item.Upc_Code.Contains(upcCode, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(modelNumber))
        {
            query = query.Where(item => item.Model_Number.Contains(modelNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(commodityCode))
        {
            query = query.Where(item => item.Commodity_Code.Contains(commodityCode, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(supplierCode))
        {
            query = query.Where(item => item.Supplier_Code.Contains(supplierCode, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(supplierPartNumber))
        {
            query = query.Where(item => item.Supplier_Part_Number.Contains(supplierPartNumber, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public bool UpdateItem(string itemId, Item item)
    {
        if ((item.Uid != itemId))
        {
            return false;
        }

        item.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingItem => existingItem.Uid == itemId);

        if (index >= 0)
        {
            item.Created_At = data[index].Created_At;
            data[index] = item;
            return true;
        }
        return false;
    }

    public bool RemoveItem(string itemId)
    {
        var item = GetItem(itemId);
        if (item == null) return false;

        var orders = DataProvider.fetch_order_pool().GetOrders(); 
        var shipments = DataProvider.fetch_shipment_pool().GetShipments();
        var transfers = DataProvider.fetch_transfer_pool().GetTransfers();

        if (orders.Any(x => x.Items.Any(y => y.Item_Id == itemId)) || shipments.Any(x => x.Items.Any(y => y.Item_Id == itemId)) || transfers.Any(x => x.Items.Any(y => y.Item_Id == itemId)))
        {
            return false;
        }


        return data.Remove(item);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Item>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Item>>(json);
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
