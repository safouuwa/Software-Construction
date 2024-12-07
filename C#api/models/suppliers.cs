using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Models;
using Providers;
using Newtonsoft.Json;

public class Supplier
{
    public int Id { get; set; } = -10;
    public string Code { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Address_Extra { get; set; }
    public string City { get; set; }
    public string Zip_Code { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public string Contact_Name { get; set; }
    public string Phonenumber { get; set; }
    public string Reference { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class Suppliers : Base
{
    private string dataPath;
    private List<Supplier> data;

    public Suppliers(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "suppliers.json");
        Load(isDebug);
    }

    public List<Supplier> GetSuppliers()
    {
        return data;
    }

    public Supplier GetSupplier(int supplierId)
    {
        return data.FirstOrDefault(supplier => supplier.Id == supplierId);
    }

    public bool AddSupplier(Supplier supplier)
    {
        if (supplier.Id == -10)
        {
            supplier.Id = data.Count > 0 ? data.Max(s => s.Id) + 1 : 1;
        }
        else if (data.Any(existingSupplier => existingSupplier.Id == supplier.Id))
        {
            return false;
        }

        if (supplier.Created_At == null) supplier.Created_At = GetTimestamp();
        if (supplier.Updated_At == null) supplier.Updated_At = GetTimestamp();
        data.Add(supplier);
        return true;
    }

    public bool UpdateSupplier(int supplierId, Supplier supplier)
    {
        if (supplier.Id != supplierId)
        {
            return false;
        }

        supplier.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingSupplier => existingSupplier.Id == supplierId);
        
        if (index >= 0)
        {
            supplier.Created_At = data[index].Created_At;
            data[index] = supplier;
            return true;
        }
        return false;
    }

    public bool RemoveSupplier(int supplierId)
    {
        var supplier = GetSupplier(supplierId);
        if (supplier == null) return false;

        var items = DataProvider.fetch_item_pool().GetItems(); 
        if (items.Any(item => item.Supplier_Id == supplierId))
        {
            return false;
        }

        return data.Remove(supplier);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Supplier>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Supplier>>(json);
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
