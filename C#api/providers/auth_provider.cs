using System.Collections.Generic;
using System.Linq;
namespace Providers;

public class AuthProvider
{
    private static List<User> _users;

    private static readonly List<User> USERS = new List<User>
    {
        new User
        {
            ApiKey = "a1b2c3d4e5",
            App = "CargoHUB Dashboard 1",
            EndpointAccess = new EndpointAccess
            {
                Full = true
            }
        },
        new User
        {
            ApiKey = "f6g7h8i9j0",
            App = "CargoHUB Dashboard 2",
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, Get = true },
                Locations = new EndpointDetail { Full = false, Get = true },
                Transfers = new EndpointDetail { Full = false, Get = true },
                Items = new EndpointDetail { Full = false, Get = true },
                ItemLines = new EndpointDetail { Full = false, Get = true },
                ItemGroups = new EndpointDetail { Full = false, Get = true },
                ItemTypes = new EndpointDetail { Full = false, Get = true },
                Suppliers = new EndpointDetail { Full = false, Get = true },
                Orders = new EndpointDetail { Full = false, Get = true },
                Clients = new EndpointDetail { Full = false, Get = true },
                Shipments = new EndpointDetail { Full = false, Get = true }
            }
        }
    };

    public static void Init()
    {
        _users = USERS;
    }

    public static User GetUser(string apiKey)
    {
        return _users.FirstOrDefault(user => user.ApiKey == apiKey);
    }

    public static bool HasAccess(User user, string path, string method)
    {
        var access = user.EndpointAccess;
        if (access.Full)
        {
            return true;
        }

        return path switch
        {
            "warehouses" => access.Warehouses.Get,
            "locations" => access.Locations.Get,
            "transfers" => access.Transfers.Get,
            "items" => access.Items.Get,
            "item_lines" => access.ItemLines.Get,
            "item_groups" => access.ItemGroups.Get,
            "item_types" => access.ItemTypes.Get,
            "suppliers" => access.Suppliers.Get,
            "orders" => access.Orders.Get,
            "clients" => access.Clients.Get,
            "shipments" => access.Shipments.Get,
            _ => false
        };
    }
}

public class User
{
    public string ApiKey { get; set; }
    public string App { get; set; }
    public EndpointAccess EndpointAccess { get; set; }
}

public class EndpointAccess
{
    public bool Full { get; set; }
    public EndpointDetail Warehouses { get; set; }
    public EndpointDetail Locations { get; set; }
    public EndpointDetail Transfers { get; set; }
    public EndpointDetail Items { get; set; }
    public EndpointDetail ItemLines { get; set; }
    public EndpointDetail ItemGroups { get; set; }
    public EndpointDetail ItemTypes { get; set; }
    public EndpointDetail Suppliers { get; set; }
    public EndpointDetail Orders { get; set; }
    public EndpointDetail Clients { get; set; }
    public EndpointDetail Shipments { get; set; }
}

public class EndpointDetail
{
    public bool Full { get; set; }
    public bool Get { get; set; }
    public bool Post { get; set; }
    public bool Put { get; set; }
    public bool Delete { get; set; }
}
