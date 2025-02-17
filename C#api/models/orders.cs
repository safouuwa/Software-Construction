using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProvidersV2;
namespace ModelsV2;

public class OrderItem
    {
        public string Item_Id { get; set; }
        public int Amount { get; set; }
    }

public class Order
{
    public int? Id { get; set; }
    public int Source_Id { get; set; }
    public string Order_Date { get; set; }
    public string Request_Date { get; set; }
    public string Reference { get; set; }
    public string Reference_Extra { get; set; }
    public string? Order_Status { get; set; }
    public string Notes { get; set; }
    public string Shipping_Notes { get; set; }
    public string Picking_Notes { get; set; }
    public int Warehouse_Id { get; set; }
    public int? Ship_To { get; set; }
    public int? Bill_To { get; set; }
    public int? Shipment_Id { get; set; }
    public decimal Total_Amount { get; set; }
    public decimal Total_Discount { get; set; }
    public decimal Total_Tax { get; set; }
    public decimal Total_Surcharge { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class Orders : Base
{
    private string dataPath;
    private List<Order> data;
    private Inventories inventory;

    public Orders(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "orders.json");
        Load(isDebug);
        inventory = new Inventories(rootPath);
    }

    public List<Order> GetOrders()
    {
        return data;
    }

    public Order GetOrder(int orderId)
    {
        return data.FirstOrDefault(order => order.Id == orderId);
    }

    public List<OrderItem> GetItemsInOrder(int orderId)
    {
        var order = GetOrder(orderId);
        return order.Items;
    }

    public List<Order> GetOrdersInShipment(int shipmentId)
    {
        return data.Where(order => order.Shipment_Id == shipmentId).ToList();
    }

    public List<Order> GetOrdersForClient(int clientId)
    {
        return data.Where(order => order.Ship_To == clientId ||
                                   order.Bill_To == clientId).ToList();
    }

    public bool AddOrder(Order order)
    {
        order.Id = data.Count > 0 ? data.Max(o => o.Id) + 1 : 1;
        if (order.Created_At == null) order.Created_At = GetTimestamp();
        if (order.Updated_At == null) order.Updated_At = GetTimestamp();
        order.Order_Status = "Open";
        data.Add(order);
        return true;
    }

    public List<Order> SearchOrders(int? sourceId = null, string orderStatus = null, string orderDate = null, int? warehouseId = null, string createdAt = null)
    {
        if (!sourceId.HasValue && string.IsNullOrEmpty(orderStatus) && string.IsNullOrEmpty(orderDate) &&
            !warehouseId.HasValue)
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();
        
        if (sourceId.HasValue)
        {
            query = query.Where(order => order.Source_Id == sourceId.Value);
        }

        if (!string.IsNullOrEmpty(orderStatus))
        {
            query = query.Where(order => order.Order_Status.Contains(orderStatus, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(orderDate))
        {
            query = query.Where(order => order.Order_Date.Contains(orderDate, StringComparison.OrdinalIgnoreCase));
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(order => order.Warehouse_Id == warehouseId.Value);
        }

        return query.ToList();
    }


    public bool UpdateOrder(int orderId, Order order)
    {
        order.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingOrder => Convert.ToInt64(existingOrder.Id) == orderId);
        if (index >= 0)
        {
            order.Id = data[index].Id;
            order.Created_At = data[index].Created_At;
            order.Order_Status = "Open";
            data[index] = order;
            return true;
        }
        return false;
    }

    public bool ReplaceOrder(int orderId, Order newOrderData)
    {
        var index = data.FindIndex(existingOrder => existingOrder.Id == orderId);
        var existingOrder = data.FirstOrDefault(existingOrder => existingOrder.Id == orderId);

        if (index < 0)
        {
            return false;
        }

        if (newOrderData.Source_Id != 0) existingOrder.Source_Id = newOrderData.Source_Id;
        if (!string.IsNullOrEmpty(newOrderData.Order_Date)) existingOrder.Order_Date = newOrderData.Order_Date;
        if (!string.IsNullOrEmpty(newOrderData.Request_Date)) existingOrder.Request_Date = newOrderData.Request_Date;
        if (!string.IsNullOrEmpty(newOrderData.Reference)) existingOrder.Reference = newOrderData.Reference;
        if (!string.IsNullOrEmpty(newOrderData.Reference_Extra)) existingOrder.Reference_Extra = newOrderData.Reference_Extra;
        if (!string.IsNullOrEmpty(newOrderData.Order_Status)) existingOrder.Order_Status = newOrderData.Order_Status;
        if (!string.IsNullOrEmpty(newOrderData.Notes)) existingOrder.Notes = newOrderData.Notes;
        if (!string.IsNullOrEmpty(newOrderData.Shipping_Notes)) existingOrder.Shipping_Notes = newOrderData.Shipping_Notes;
        if (!string.IsNullOrEmpty(newOrderData.Picking_Notes)) existingOrder.Picking_Notes = newOrderData.Picking_Notes;
        if (newOrderData.Warehouse_Id != 0) existingOrder.Warehouse_Id = newOrderData.Warehouse_Id;
        if (newOrderData.Ship_To != 0) existingOrder.Ship_To = newOrderData.Ship_To;
        if (newOrderData.Bill_To != 0) existingOrder.Bill_To = newOrderData.Bill_To;
        if (newOrderData.Shipment_Id != 0) existingOrder.Shipment_Id = newOrderData.Shipment_Id;
        if (newOrderData.Total_Amount != 0) existingOrder.Total_Amount = newOrderData.Total_Amount;
        if (newOrderData.Total_Discount != 0) existingOrder.Total_Discount = newOrderData.Total_Discount;
        if (newOrderData.Total_Tax != 0) existingOrder.Total_Tax = newOrderData.Total_Tax;
        if (newOrderData.Total_Surcharge != 0) existingOrder.Total_Surcharge = newOrderData.Total_Surcharge;
        existingOrder.Updated_At = GetTimestamp();

        return true;
    }

    public void UpdateItemsInOrder(int orderId, List<Item> items)
    {
        var order = GetOrder(orderId);
        List<Item> currentItems = new List<Item>();
        foreach (var i in order.Items) currentItems.Add(DataProvider.fetch_item_pool().GetItem(i.Item_Id));

        foreach (var currentItem in currentItems)
        {
            var itemToUpdate = items.FirstOrDefault(item => item.Uid == currentItem.Uid);
            if (itemToUpdate == null)
            {
                var inventories = inventory.GetInventoriesForItem(currentItem.Uid);
                var minInventory = inventories.OrderBy(i => i.Total_Allocated).FirstOrDefault();

                if (minInventory != null)
                {
                    minInventory.Total_Allocated = minInventory.Total_Allocated - order.Items.First(x => x.Item_Id == currentItem.Uid).Amount;
                    minInventory.Total_Expected = minInventory.Total_On_Hand + minInventory.Total_Ordered;
                    inventory.UpdateInventory((int)minInventory.Id, minInventory);
                }
            }
        }

        foreach (var newItem in items)
        {
            var currentItem = currentItems.FirstOrDefault(ci => ci.Uid == newItem.Uid);
            var inventories = inventory.GetInventoriesForItem(newItem.Uid);
            var minInventory = inventories.OrderBy(i => i.Total_Allocated).FirstOrDefault();

            if (minInventory != null)
            {
                if (currentItem != null)
                {
                    minInventory.Total_Allocated = minInventory.Total_Allocated + newItem.Unit_Order_Quantity - order.Items.First(x => x.Item_Id == currentItem.Uid).Amount;
                }
                else
                {
                    minInventory.Total_Allocated = minInventory.Total_Allocated + newItem.Unit_Order_Quantity;
                }
                minInventory.Total_Expected = minInventory.Total_On_Hand + minInventory.Total_Ordered;
                inventory.UpdateInventory((int)minInventory.Id, minInventory);
            }
        }
        List<OrderItem> list = new List<OrderItem>();
        foreach (var it in items)
        {
            list.Add(new OrderItem{ Item_Id = it.Uid, Amount = it.Unit_Order_Quantity });    
        }
        order.Items = list;
        UpdateOrder(orderId, order);
    }

    public void UpdateOrdersInShipment(int shipmentId, List<Order> orders)
    {
        var packedOrders = GetOrdersInShipment(shipmentId);
        
        foreach (var packedOrder in packedOrders)
        {
            if (!orders.Contains(packedOrder))
            {
                var order = GetOrder((int)packedOrder.Id);
                if (order != null)
                {
                    order.Shipment_Id = -1;
                    order.Order_Status = "Scheduled";
                    UpdateOrder((int)packedOrder.Id, order);
                }
            }
        }

        foreach (var order in orders)
        {
            var orderToUpdate = GetOrder((int)order.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.Shipment_Id = shipmentId;
                orderToUpdate.Order_Status = "Packed";
                UpdateOrder((int)order.Id, orderToUpdate);
            }
        }
    }

    public bool RemoveOrder(int orderId)
    {
        var order = GetOrder(orderId);
        if (order == null) return false;

        return data.Remove(order);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Order>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Order>>(json);
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
