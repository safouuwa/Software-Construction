using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Providers;
namespace Models;

public class Client
{
    public int? Id { get; set; }
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
        bool test = data.Select(x => x.Id).ToList().Distinct().Count() == 9820;
    }

    public List<Client> GetClients()
    {
        return data;
    }

    public Client GetClient(int clientId)
    {
        return data.Find(x => Convert.ToInt64(x.Id) == clientId);
    }

    public List<Client> SearchClients(string name = null, string address = null, string country = null, string contactName = null)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(address) && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(contactName))
        {
            throw new ArgumentException("At least one search parameter must be provided.");
        }

        var query = data.AsQueryable();
        
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(client => client.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(address))
        {
            query = query.Where(client => client.Address.Contains(address, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(country))
        {
            query = query.Where(client => client.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(contactName))
        {
            query = query.Where(client => client.Contact_name.Contains(contactName, StringComparison.OrdinalIgnoreCase));
        }
        
        if (query.Count() == 0)
        {
            throw new ArgumentException("No clients found.");
        }

        return query.ToList();
    }

    public int GetNextAvailableId()
    {
        return data.Count > 0 ? data.Max(c => (int)c.Id) + 1 : 1;
    }

    public bool AddClient(Client client)
    {
        client.Id = GetNextAvailableId();
        if (client.Created_at == null) client.Created_at = GetTimestamp();
        if (client.Updated_at == null) client.Updated_at = GetTimestamp();
        data.Add(client);
        return true;
    }

    public bool UpdateClient(int clientId, Client client)
    {
        client.Updated_at = GetTimestamp();
        var index = data.FindIndex(existingClient => existingClient.Id == clientId);

        if (index >= 0)
        {
            client.Id = data[index].Id;
            client.Created_at = data[index].Created_at;
            data[index] = client;
            return true;
        }
        return false;
    }

    public bool ReplaceClient(int clientId, Client newClientData)
    { 
        

        var index = data.FindIndex(existingClient => existingClient.Id == clientId);
        var existingClient = data.FirstOrDefault(existingClient => existingClient.Id == clientId);

        if (index < 0)
        {

            return false;

        }

        if (!string.IsNullOrEmpty(newClientData.Name)) existingClient.Name = newClientData.Name;
        if (!string.IsNullOrEmpty(newClientData.Address)) existingClient.Address = newClientData.Address;
        if (!string.IsNullOrEmpty(newClientData.City)) existingClient.City = newClientData.City;
        if (!string.IsNullOrEmpty(newClientData.Zip_code)) existingClient.Zip_code = newClientData.Zip_code;
        if (!string.IsNullOrEmpty(newClientData.Province)) existingClient.Province = newClientData.Province;
        if (!string.IsNullOrEmpty(newClientData.Country)) existingClient.Country = newClientData.Country;
        if (!string.IsNullOrEmpty(newClientData.Contact_name)) existingClient.Contact_name = newClientData.Contact_name;
        if (!string.IsNullOrEmpty(newClientData.Contact_phone)) existingClient.Contact_phone = newClientData.Contact_phone;
        if (!string.IsNullOrEmpty(newClientData.Contact_email)) existingClient.Contact_email = newClientData.Contact_email;
        existingClient.Updated_at = GetTimestamp();
    
        return true;

    }

    public bool RemoveClient(int clientId, bool force = false)
    {
        var client = GetClient(clientId);
        if (client == null) return false;

        var orders = DataProvider.fetch_order_pool().GetOrders();

        if (force) return data.Remove(client);
        
        if (orders.Any(order => order.Ship_To == clientId || order.Bill_To == clientId))
        {
            return false;
        }

        return data.Remove(client);
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

