using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Providers;
namespace Models;

public class Location
{
    public int Id { get; set; } = -10;
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

    public bool AddLocation(Location location)
    {
        if (data.Any(existingLocation => existingLocation.Id == location.Id))
        {
            return false;
        }

        if (location.Created_At == null) location.Created_At = GetTimestamp();
        if (location.Updated_At == null) location.Updated_At = GetTimestamp();
        data.Add(location);
        return true;
    }

    public bool UpdateLocation(int locationId, Location location)
    {
        if (location.Id != locationId)
        {
            return false;
        }

        location.Updated_At = GetTimestamp();
        var index = data.FindIndex(existingLocation => existingLocation.Id == locationId);

        if (index >= 0)
        {
            location.Created_At = data[index].Created_At;
            data[index] = location;
            return true;
        }
        return false;
    }

    public bool RemoveLocation(int locationId)
    {
        var location = GetLocation(locationId);
        if (location == null) return false;

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
