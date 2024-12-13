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

    public bool ReplaceItem(string itemUid, Item newItemData)
    {
        var index = data.FindIndex(existingItem => existingItem.Uid == itemUid);
        var existingItem = data.FirstOrDefault(existingItem => existingItem.Uid == itemUid);

        if (index < 0)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(newItemData.Code)) existingItem.Code = newItemData.Code;
        if (!string.IsNullOrEmpty(newItemData.Description)) existingItem.Description = newItemData.Description;
        if (!string.IsNullOrEmpty(newItemData.Short_Description)) existingItem.Short_Description = newItemData.Short_Description;
        if (!string.IsNullOrEmpty(newItemData.Upc_Code)) existingItem.Upc_Code = newItemData.Upc_Code;
        if (!string.IsNullOrEmpty(newItemData.Model_Number)) existingItem.Model_Number = newItemData.Model_Number;
        if (!string.IsNullOrEmpty(newItemData.Commodity_Code)) existingItem.Commodity_Code = newItemData.Commodity_Code;
        if (newItemData.Item_Line != 0) existingItem.Item_Line = newItemData.Item_Line;
        if (newItemData.Item_Group != 0) existingItem.Item_Group = newItemData.Item_Group;
        if (newItemData.Item_Type != 0) existingItem.Item_Type = newItemData.Item_Type;
        if (newItemData.Unit_Purchase_Quantity != 0) existingItem.Unit_Purchase_Quantity = newItemData.Unit_Purchase_Quantity;
        if (newItemData.Unit_Order_Quantity != 0) existingItem.Unit_Order_Quantity = newItemData.Unit_Order_Quantity;
        if (newItemData.Pack_Order_Quantity != 0) existingItem.Pack_Order_Quantity = newItemData.Pack_Order_Quantity;
        if (newItemData.Supplier_Id != 0) existingItem.Supplier_Id = newItemData.Supplier_Id;
        if (!string.IsNullOrEmpty(newItemData.Supplier_Code)) existingItem.Supplier_Code = newItemData.Supplier_Code;
        if (!string.IsNullOrEmpty(newItemData.Supplier_Part_Number)) existingItem.Supplier_Part_Number = newItemData.Supplier_Part_Number;
        existingItem.Updated_At = GetTimestamp();
        
        return true;
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
