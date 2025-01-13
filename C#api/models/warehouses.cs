using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Models;
using Providers;
using Newtonsoft.Json; // Ensure you have Newtonsoft.Json package installed
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

public class ContactInfo
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}
public class Warehouse
{
    public int? Id { get; set; }
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
        warehouse.Id = data.Count > 0 ? data.Max(w => w.Id) + 1 : 1;
        if (warehouse.Created_At == null) warehouse.Created_At = GetTimestamp();
        if (warehouse.Updated_At == null) warehouse.Updated_At = GetTimestamp();
        data.Add(warehouse);
        return true;
    }

    public List<Warehouse> SearchWarehouses(string code = null, string name = null, string city = null ,string country = null)
    {
        if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();

        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(warehouse => warehouse.Code.Contains(code, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(warehouse => warehouse.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(warehouse => warehouse.City.Contains(city, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(warehouse => warehouse.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
        }
        return query.ToList();
    }

    public bool UpdateWarehouse(int warehouseId, Warehouse warehouse)
    {
        warehouse.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingWarehouse => existingWarehouse.Id == warehouseId);
        
        if (index >= 0)
        {
            warehouse.Id = data[index].Id;
            warehouse.Created_At = data[index].Created_At;
            data[index] = warehouse;
            return true;
        }
        return false;
    }

    public bool ReplaceWarehouse(int warehouseId, Warehouse newWarehouseData)
    {
        var index = data.FindIndex(existingWarehouse => existingWarehouse.Id == warehouseId);
        var existingWarehouse = data.FirstOrDefault(existingWarehouse => existingWarehouse.Id == warehouseId);

        if (index < 0) 
        {
            return false;
        }

        if (!string.IsNullOrEmpty(newWarehouseData.Code)) existingWarehouse.Code = newWarehouseData.Code;
        if (!string.IsNullOrEmpty(newWarehouseData.Name)) existingWarehouse.Name = newWarehouseData.Name;
        if (!string.IsNullOrEmpty(newWarehouseData.Address)) existingWarehouse.Address = newWarehouseData.Address;
        if (!string.IsNullOrEmpty(newWarehouseData.Zip)) existingWarehouse.Zip = newWarehouseData.Zip;
        if (!string.IsNullOrEmpty(newWarehouseData.City)) existingWarehouse.City = newWarehouseData.City;
        if (!string.IsNullOrEmpty(newWarehouseData.Province)) existingWarehouse.Province = newWarehouseData.Province;
        if (!string.IsNullOrEmpty(newWarehouseData.Country)) existingWarehouse.Country = newWarehouseData.Country;
        if (newWarehouseData.Contact != null)
        {
            if (!string.IsNullOrEmpty(newWarehouseData.Contact.Name)) existingWarehouse.Contact.Name = newWarehouseData.Contact.Name;
            if (!string.IsNullOrEmpty(newWarehouseData.Contact.Phone)) existingWarehouse.Contact.Phone = newWarehouseData.Contact.Phone;
            if (!string.IsNullOrEmpty(newWarehouseData.Contact.Email)) existingWarehouse.Contact.Email = newWarehouseData.Contact.Email;
        }
        existingWarehouse.Updated_At = GetTimestamp();

        return true;

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
