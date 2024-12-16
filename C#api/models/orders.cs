using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Providers;
namespace Models;

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
    public string Order_Status { get; set; }
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
        data.Add(order);
        return true;
    }

    public List<Order> SearchOrders(int? id = null,int? sourceId = null, string orderStatus = null, string orderDate = null, string requestDate = null, string reference = null, string referenceExtra = null, string notes = null, string shippingNotes = null, string pickingNotes = null, int? warehouseId = null, int? shipTo = null, int? billTo = null, int? shipmentId = null, string created_At = null, string updated_At = null)
    {
        if (id == null && !sourceId.HasValue && string.IsNullOrEmpty(orderStatus) && string.IsNullOrEmpty(orderDate) && string.IsNullOrEmpty(requestDate) &&
            string.IsNullOrEmpty(reference) && string.IsNullOrEmpty(referenceExtra) && string.IsNullOrEmpty(notes) &&
            string.IsNullOrEmpty(shippingNotes) && string.IsNullOrEmpty(pickingNotes) && !warehouseId.HasValue &&
            !shipTo.HasValue && !billTo.HasValue && !shipmentId.HasValue && string.IsNullOrEmpty(created_At) && string.IsNullOrEmpty(updated_At))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();
        if (id.HasValue)
        {
            query = query.Where(order => order.Id == id.Value);
        }

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

        if (!string.IsNullOrEmpty(requestDate))
        {
            query = query.Where(order => order.Request_Date.Contains(requestDate, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(reference))
        {
            query = query.Where(order => order.Reference.Contains(reference, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(referenceExtra))
        {
            query = query.Where(order => order.Reference_Extra.Contains(referenceExtra, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(notes))
        {
            query = query.Where(order => order.Notes.Contains(notes, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(shippingNotes))
        {
            query = query.Where(order => order.Shipping_Notes.Contains(shippingNotes, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(pickingNotes))
        {
            query = query.Where(order => order.Picking_Notes.Contains(pickingNotes, StringComparison.OrdinalIgnoreCase));
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(order => order.Warehouse_Id == warehouseId.Value);
        }

        if (shipTo.HasValue)
        {
            query = query.Where(order => order.Ship_To == shipTo.Value);
        }

        if (billTo.HasValue)
        {
            query = query.Where(order => order.Bill_To == billTo.Value);
        }

        if (shipmentId.HasValue)
        {
            query = query.Where(order => order.Shipment_Id == shipmentId.Value);
        }

        if (!string.IsNullOrEmpty(created_At))
        {
            query = query.Where(order => order.Created_At.Contains(created_At, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(updated_At))
        {
            query = query.Where(order => order.Updated_At.Contains(updated_At, StringComparison.OrdinalIgnoreCase));
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
            data[index] = order;
            return true;
        }
        return false;
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
