using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Providers;
namespace Models;

public class Client
{
    public int Id { get; set; } = -10;
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Zip_code { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public string Contact_name { get; set; }
    public string Contact_phone { get; set; }
    public string Contact_email { get; set; }
    public string? Created_at { get; set; }
    public string? Updated_at { get; set; }
}

public class Clients : Base
{
    private string dataPath;
    private List<Client> data;

    public Clients(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "clients.json");
        Load(isDebug);
    }

    public List<Client> GetClients()
    {
        return data;
    }

    public Client GetClient(int clientId)
    {
        return data.Find(x => Convert.ToInt64(x.Id) == clientId);
    }

    public bool AddClient(Client client)
    {
        if (data.Any(existingClient => Convert.ToInt64(existingClient.Id) == client.Id))
        {
            return false;
        }

        if (client.Created_at == null) client.Created_at = GetTimestamp();
        if (client.Updated_at == null) client.Updated_at = GetTimestamp();
        data.Add(client);
        return true;
    }

    public bool UpdateClient(int clientId, Client client)
    {
        if (Convert.ToInt64(client.Id) != clientId)
        {
            return false;
        }

        client.Updated_at = GetTimestamp();
        var index = data.FindIndex(existingClient => existingClient.Id == clientId);

        if (index >= 0)
        {
            client.Created_at = data[index].Created_at;
            data[index] = client;
            return true;
        }
        return false;
    }

    public bool RemoveClient(int clientId)
    {
        var client = GetClient(clientId);
        if (client == null) return false;

        var orders = DataProvider.fetch_order_pool().GetOrders();

        if (orders.Any(order => order.Ship_To == clientId || order.Bill_To == clientId))
        {
            return false;
        }

        return data.Remove(client);
    }

    public List<Client> SearchClients(string name = null, string city = null, string country = null)
    {
        var query = data.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(client => client.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(client => client.City.Contains(city, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(client => client.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Client>();
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Client>>(json);
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
