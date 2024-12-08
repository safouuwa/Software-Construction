using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Models;
using Providers;
using Newtonsoft.Json; // Ensure you have Newtonsoft.Json package installed

public class ContactInfo
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}
public class Warehouse
{
    public int Id { get; set; } = -10;
    public string Code { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public ContactInfo Contact { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }

}


public class Warehouses : Base
{
    private string dataPath;
    private List<Warehouse> data;

    public Warehouses(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "warehouses.json");
        dataPath = dataPath.Replace("\\", "/");
        Load(isDebug);
    }

    public List<Warehouse> GetWarehouses()
    {
        return data;
    }

    public Warehouse GetWarehouse(int warehouseId)
    {
        return data.FirstOrDefault(warehouse => warehouse.Id == warehouseId);
    }

    public bool AddWarehouse(Warehouse warehouse)
    {
        if (warehouse.Id == -10)
        {
            warehouse.Id = data.Count > 0 ? data.Max(w => w.Id) + 1 : 1;
        }
        else if (data.Any(existingWarehouse => existingWarehouse.Id == warehouse.Id))
        {
            return false;
        }

        if (warehouse.Created_At == null) warehouse.Created_At = GetTimestamp();
        if (warehouse.Updated_At == null) warehouse.Updated_At = GetTimestamp();
        data.Add(warehouse);
        return true;
    }

    public bool UpdateWarehouse(int warehouseId, Warehouse warehouse)
    {
        if (warehouse.Id != warehouseId)
        {
            return false;
        }

        warehouse.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingWarehouse => existingWarehouse.Id == warehouseId);
        
        if (index >= 0)
        {
            warehouse.Created_At = data[index].Created_At;
            data[index] = warehouse;
            return true;
        }
        return false;
    }

    public bool RemoveWarehouse(int warehouseId, bool force = false)
    {
        var warehouse = GetWarehouse(warehouseId);
        if (warehouse == null) return false;

        if (force) return data.Remove(warehouse);
        var orders = DataProvider.fetch_order_pool().GetOrders(); 
        if (orders.Any(order => order.Warehouse_Id == warehouseId))
        {
            return false;
        }

        return data.Remove(warehouse);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Warehouse>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Warehouse>>(json);
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
