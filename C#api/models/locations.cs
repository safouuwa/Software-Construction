using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Providers;
namespace Models;

public class Location
{
    public int? Id { get; set; }
    public int Warehouse_Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class Locations : Base
{
    private string dataPath;
    private List<Location> data;

    public Locations(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "locations.json");
        Load(isDebug);
    }

    public List<Location> GetLocations()
    {
        return data;
    }

    public Location GetLocation(int locationId)
    {
        return data.FirstOrDefault(location => location.Id == locationId);
    }

    public List<Location> GetLocationsInWarehouse(int warehouseId)
    {
        return data.Where(location => location.Warehouse_Id == warehouseId).ToList();
    }

    public List<Location> SearchLocations(int? id = null,string name = null, int? warehouseId = null, string code = null)
    {
        if (id == null && string.IsNullOrEmpty(name) && !warehouseId.HasValue && string.IsNullOrEmpty(code))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();

        if (id.HasValue)
        {
            query = query.Where(location => location.Id == id.Value);
        }

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(location => location.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(location => location.Warehouse_Id == warehouseId.Value);
        }

        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(location => location.Code.Contains(code, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }
    public bool AddLocation(Location location)
    {
        location.Id = data.Count > 0 ? data.Max(l => l.Id) + 1 : 1;
        if (location.Created_At == null) location.Created_At = GetTimestamp();
        if (location.Updated_At == null) location.Updated_At = GetTimestamp();
        data.Add(location);
        return true;
    }

    public bool UpdateLocation(int locationId, Location location)
    {
        location.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingLocation => existingLocation.Id == locationId);

        if (index >= 0)
        {
            location.Id = data[index].Id;
            location.Created_At = data[index].Created_At;
            data[index] = location;
            return true;
        }
        return false;
    }

    public bool ReplaceLocation(int locationId, Location newLocationData)
    {
        var index = data.FindIndex(existingLocation => existingLocation.Id == locationId);
        var existingLocation = data.FirstOrDefault(existingLocation => existingLocation.Id == locationId);

        if (index < 0)
        {
            return false;
        }
        
        if (!string.IsNullOrEmpty(newLocationData.Code)) existingLocation.Code = newLocationData.Code;
        if (!string.IsNullOrEmpty(newLocationData.Name)) existingLocation.Name = newLocationData.Name;
        if (newLocationData.Warehouse_Id != 0) existingLocation.Warehouse_Id = newLocationData.Warehouse_Id;
        existingLocation.Updated_At = GetTimestamp();

        return true;
    }

    public bool RemoveLocation(int locationId, bool force = false)
    {
        var location = GetLocation(locationId);
        if (location == null) return false;
        if (force) return data.Remove(location);

        var inventories = DataProvider.fetch_inventory_pool().GetInventories();

        if (inventories.Any(inventory => inventory.Locations.Any(l => l == locationId)))
        {
            return false;
        }

        return data.Remove(location);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Location>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Location>>(json);
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
