using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Models;
using Providers;
using Newtonsoft.Json;

public class ShipmentItem
{
    public string Item_Id { get; set; }
    public int Amount { get; set; }
}
public class Shipment
{
    public int? Id { get; set; }
    public int Order_Id { get; set; }
    public int Source_Id { get; set; }
    public string Order_Date { get; set; }
    public string Request_Date { get; set; }
    public string Shipment_Date { get; set; }
    public string Shipment_Type { get; set; }
    public string? Shipment_Status { get; set; }
    public string Notes { get; set; }
    public string Carrier_Code { get; set; }
    public string Carrier_Description { get; set; }
    public string Service_Code { get; set; }
    public string Payment_Type { get; set; }
    public string Transfer_Mode { get; set; }
    public int Total_Package_Count { get; set; }
    public double Total_Package_Weight { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
    public List<ShipmentItem> Items { get; set; }
}

public class Shipments : Base
{
    private string dataPath;
    private List<Shipment> data;
    private Inventories inventory;

    public Shipments(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "shipments.json");
        Load(isDebug);
        inventory = new Inventories(rootPath);
    }

    public List<Shipment> GetShipments()
    {
        return data;
    }

    public Shipment GetShipment(int shipmentId)
    {
        return data.FirstOrDefault(shipment => shipment.Id == shipmentId);
    }

    public List<ShipmentItem> GetItemsInShipment(int shipmentId)
    {
        var shipment = GetShipment(shipmentId);
        return shipment.Items;
    }

    public bool AddShipment(Shipment shipment)
    {
        shipment.Id = data.Count > 0 ? data.Max(s => s.Id) + 1 : 1;
        if (shipment.Created_At == null) shipment.Created_At = GetTimestamp();
        if (shipment.Updated_At == null) shipment.Updated_At = GetTimestamp();
        shipment.Shipment_Status = "Planned";
        data.Add(shipment);
        return true;
    }

     public List<Shipment> SearchShipments(int? id = null,int? orderId = null,string orderDate = null, string requestDate = null, string shipmentDate = null, string shipmentStatus = null, string carrierCode = null)
    {
        if (id == null && !orderId.HasValue && string.IsNullOrEmpty(orderDate) && string.IsNullOrEmpty(requestDate) &&
            string.IsNullOrEmpty(shipmentDate) && string.IsNullOrEmpty(shipmentStatus) && string.IsNullOrEmpty(carrierCode))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();
        
        if (id.HasValue)
        {
            query = query.Where(shipment => shipment.Id == id.Value);
        }

        if (orderId.HasValue)
        {
            query = query.Where(shipment => shipment.Order_Id == orderId.Value);
        }

        if (!string.IsNullOrEmpty(orderDate))
        {
            query = query.Where(shipment => shipment.Order_Date.Contains(orderDate, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(requestDate))
        {
            query = query.Where(shipment => shipment.Request_Date.Contains(requestDate, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(shipmentDate))
        {
            query = query.Where(shipment => shipment.Shipment_Date.Contains(shipmentDate, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(shipmentStatus))
        {
            query = query.Where(shipment => shipment.Shipment_Status.Contains(shipmentStatus, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(carrierCode))
        {
            query = query.Where(shipment => shipment.Carrier_Code.Contains(carrierCode, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }


    public bool UpdateShipment(int shipmentId, Shipment shipment)
    {
        shipment.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingShipment => existingShipment.Id== shipmentId);
        
        if (index >= 0)
        {
            shipment.Id = data[index].Id;
            shipment.Created_At = data[index].Created_At;
            shipment.Shipment_Status = "Planned";
            data[index] = shipment;
            return true;
        }
        return false;
    }

    public bool ReplaceShipment(int shipmentId, Shipment newShipmentData)
    {
        var index = data.FindIndex(existingShipment => existingShipment.Id == shipmentId);
        var existingShipment = data.FirstOrDefault(existingShipment => existingShipment.Id == shipmentId);

        if (index < 0)
        {
            return false;
        }

        if (newShipmentData.Order_Id != 0) existingShipment.Order_Id = newShipmentData.Order_Id;
        if (newShipmentData.Source_Id != 0) existingShipment.Source_Id = newShipmentData.Source_Id;
        if (!string.IsNullOrEmpty(newShipmentData.Order_Date)) existingShipment.Order_Date = newShipmentData.Order_Date;
        if (!string.IsNullOrEmpty(newShipmentData.Request_Date)) existingShipment.Request_Date = newShipmentData.Request_Date;
        if (!string.IsNullOrEmpty(newShipmentData.Shipment_Date)) existingShipment.Shipment_Date = newShipmentData.Shipment_Date;
        if (!string.IsNullOrEmpty(newShipmentData.Shipment_Type)) existingShipment.Shipment_Type = newShipmentData.Shipment_Type;
        if (!string.IsNullOrEmpty(newShipmentData.Shipment_Status)) existingShipment.Shipment_Status = newShipmentData.Shipment_Status;
        if (!string.IsNullOrEmpty(newShipmentData.Notes)) existingShipment.Notes = newShipmentData.Notes;
        if (!string.IsNullOrEmpty(newShipmentData.Carrier_Code)) existingShipment.Carrier_Code = newShipmentData.Carrier_Code;
        if (!string.IsNullOrEmpty(newShipmentData.Carrier_Description)) existingShipment.Carrier_Description = newShipmentData.Carrier_Description;
        if (!string.IsNullOrEmpty(newShipmentData.Service_Code)) existingShipment.Service_Code = newShipmentData.Service_Code;
        if (!string.IsNullOrEmpty(newShipmentData.Payment_Type)) existingShipment.Payment_Type = newShipmentData.Payment_Type;
        if (!string.IsNullOrEmpty(newShipmentData.Transfer_Mode)) existingShipment.Transfer_Mode = newShipmentData.Transfer_Mode;
        if (newShipmentData.Total_Package_Count != 0) existingShipment.Total_Package_Count = newShipmentData.Total_Package_Count;
        if (newShipmentData.Total_Package_Weight != 0) existingShipment.Total_Package_Weight = newShipmentData.Total_Package_Weight;

        existingShipment.Updated_At = GetTimestamp();

        return true;
    }

    public void UpdateItemsInShipment(int shipmentId, List<Item> items)
    {
        var shipment = GetShipment(shipmentId);
        List<Item> currentItems = new List<Item>();
        foreach (var i in shipment.Items) currentItems.Add(DataProvider.fetch_item_pool().GetItem(i.Item_Id));
        foreach (var currentItem in currentItems)
        {
            if (!items.Any(newItem => newItem.Uid == currentItem.Uid))
            {
                var inventories = inventory.GetInventoriesForItem(currentItem.Uid);
                var maxInventory = inventories.OrderByDescending(i => Convert.ToInt64(i.Total_Ordered)).FirstOrDefault();

                if (maxInventory != null)
                {
                    maxInventory.Total_Ordered = maxInventory.Total_Ordered - shipment.Items.First(x => x.Item_Id == currentItem.Uid).Amount;
                    maxInventory.Total_Expected = maxInventory.Total_On_Hand + maxInventory.Total_Ordered;
                    inventory.UpdateInventory((int)maxInventory.Id, maxInventory);
                }
            }
        }

        foreach (var newItem in items)
        {
            var currentItem = currentItems.FirstOrDefault(ci => ci.Uid == newItem.Uid);
            var inventories = inventory.GetInventoriesForItem(newItem.Uid);
            var maxInventory = inventories.OrderByDescending(i => i.Total_Ordered).FirstOrDefault();

            if (maxInventory != null)
            {
                if (currentItem != null)
                {
                    maxInventory.Total_Ordered = (int)maxInventory.Total_Ordered + newItem.Pack_Order_Quantity - shipment.Items.First(x => x.Item_Id == currentItem.Uid).Amount;
                }
                else
                {
                    maxInventory.Total_Ordered = (int)maxInventory.Total_Ordered + newItem.Pack_Order_Quantity;
                }
                maxInventory.Total_Expected = (int)maxInventory.Total_On_Hand + (int)maxInventory.Total_Ordered;
                inventory.UpdateInventory((int)maxInventory.Id, maxInventory);
            }
        }
        List<ShipmentItem> list = new List<ShipmentItem>();
        foreach (var it in items)
        {
            list.Add(new ShipmentItem{ Item_Id = it.Uid, Amount = it.Unit_Order_Quantity });    
        }
        shipment.Items = list;
        UpdateShipment(shipmentId, shipment);
    }

    public bool RemoveShipment(int shipmentId, bool force = true)
    {
        var shipment = GetShipment(shipmentId);
        if (shipment == null) return false;
        if (force) return data.Remove(shipment);

        var orders = DataProvider.fetch_order_pool().GetOrders();
        if (orders.Any(order => order.Shipment_Id == shipmentId))
        {
            return false;
        }

        return data.Remove(shipment);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Shipment>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Shipment>>(json);
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
