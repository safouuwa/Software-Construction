using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace ModelsV2;
using ProvidersV2;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

public class Supplier
{
    public int? Id { get; set; }
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
        supplier.Id = data.Count > 0 ? data.Max(s => s.Id) + 1 : 1;
        if (supplier.Created_At == null) supplier.Created_At = GetTimestamp();
        if (supplier.Updated_At == null) supplier.Updated_At = GetTimestamp();
        data.Add(supplier);
        return true;
    }

    public List<Supplier> SearchSuppliers(string name = null, string country = null, string code = null, string phoneNumber = null)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(country) &&
            string.IsNullOrEmpty(code) && string.IsNullOrEmpty(phoneNumber))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(supplier => supplier.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }


        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(supplier => supplier.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(supplier => supplier.Code.Contains(code, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            query = query.Where(supplier => supplier.Phonenumber.Contains(phoneNumber, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public bool UpdateSupplier(int supplierId, Supplier supplier)
    {
        supplier.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingSupplier => existingSupplier.Id == supplierId);
        
        if (index >= 0)
        {
            supplier.Id = data[index].Id;
            supplier.Created_At = data[index].Created_At;
            data[index] = supplier;
            return true;
        }
        return false;
    }

    public bool ReplaceSupplier(int supplierId, Supplier newSupplierData)
    {

    var index = data.FindIndex(existingSupplier => existingSupplier.Id == supplierId);
    var existingSupplier = data.FirstOrDefault(existingSupplier => existingSupplier.Id == supplierId);

    if (index < 0)
    {
        return false;
    }

    if (!string.IsNullOrEmpty(newSupplierData.Code)) existingSupplier.Code = newSupplierData.Code;
    if (!string.IsNullOrEmpty(newSupplierData.Name)) existingSupplier.Name = newSupplierData.Name;
    if (!string.IsNullOrEmpty(newSupplierData.Address)) existingSupplier.Address = newSupplierData.Address;
    if (!string.IsNullOrEmpty(newSupplierData.Address_Extra)) existingSupplier.Address_Extra = newSupplierData.Address_Extra;
    if (!string.IsNullOrEmpty(newSupplierData.City)) existingSupplier.City = newSupplierData.City;
    if (!string.IsNullOrEmpty(newSupplierData.Zip_Code)) existingSupplier.Zip_Code = newSupplierData.Zip_Code;
    if (!string.IsNullOrEmpty(newSupplierData.Province)) existingSupplier.Province = newSupplierData.Province;
    if (!string.IsNullOrEmpty(newSupplierData.Country)) existingSupplier.Country = newSupplierData.Country;
    if (!string.IsNullOrEmpty(newSupplierData.Contact_Name)) existingSupplier.Contact_Name = newSupplierData.Contact_Name;
    if (!string.IsNullOrEmpty(newSupplierData.Phonenumber)) existingSupplier.Phonenumber = newSupplierData.Phonenumber;
    if (!string.IsNullOrEmpty(newSupplierData.Reference)) existingSupplier.Reference = newSupplierData.Reference;
    existingSupplier.Updated_At = GetTimestamp();

    return true;
    }
    public bool RemoveSupplier(int supplierId, bool force = false)
    {
        var supplier = GetSupplier(supplierId);
        if (supplier == null) return false;
        if (force) return data.Remove(supplier);

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
