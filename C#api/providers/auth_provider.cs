using System.Collections.Generic;
using System.Linq;
namespace ProvidersV2;

public class AuthProvider
{
    private static List<User> _users;

    private static readonly List<User> USERS = new List<User>
    {
        new User
        {
            ApiKey = "a1b2c3d4e5",
            App = "Admin",
            EndpointAccess = new EndpointAccess
            {
                Full = true
            }
        },
        new User
        {
            ApiKey = "f6g7h8i9j0",
            App = "Warehouse Manager",
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, Get = true, Post = true, GetSingle = true },
                Locations = new EndpointDetail { Full = true },
                Transfers = new EndpointDetail { Full = true },
                Items = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                ItemLines = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                ItemGroups = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                ItemTypes = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Suppliers = new EndpointDetail { Full = true },
                Orders = new EndpointDetail { Full = true },
                Clients = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true},
                Shipments = new EndpointDetail { Full = true },
                Inventories = new EndpointDetail { Full = true }
            }
        },
        new User
        {
            ApiKey = "k1l2m3n4o5",
            App = "Inventory Manager",
            OwnWarehouses = new List<int>{10, 11, 12},
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, GetSingle = true }, //single?
                Locations = new EndpointDetail { Full = true, OwnWarehouse = true }, //own warehouse?
                Transfers = new EndpointDetail { Full = true },
                Items = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemLines = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                ItemGroups = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                ItemTypes = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Suppliers = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Orders = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Clients = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Shipments = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = true }
            }
        },
        new User
        {
            ApiKey = "p6q7r8s9t0",
            App = "Floor Manager",
            OwnWarehouses = new List<int>{7, 8, 9},
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, GetSingle = true }, //single?
                Locations = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true, OwnWarehouse = true  }, //own warehouse?
                Transfers = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true  },
                Items = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemLines = new EndpointDetail { Full = false },
                ItemGroups = new EndpointDetail { Full = false },
                ItemTypes = new EndpointDetail { Full = false },
                Suppliers = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Orders = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Clients = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Shipments = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = false, Get = true, GetSingle = true }
            }
        },
        new User
        {
            ApiKey = "u1v2w3x4y5",
            App = "Operative",
            OwnWarehouses = new List<int>{4, 5, 6},
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false }, 
                Locations = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true}, //own warehouse?
                Transfers = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Items = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true }, //own warehouse?
                ItemLines = new EndpointDetail { Full = false },
                ItemGroups = new EndpointDetail { Full = false },
                ItemTypes = new EndpointDetail { Full = false },
                Suppliers = new EndpointDetail { Full = false, GetSingle = true }, //single?
                Orders = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true }, //own warehouse?
                Clients = new EndpointDetail { Full = false },
                Shipments = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true } //own warehouse?
            }
        },
        new User
        {
            ApiKey = "z6a7b8c9d0",
            App = "Supervisor",
            OwnWarehouses = new List<int>{1, 2, 3},
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false }, 
                Locations = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true}, //own warehouse?
                Transfers = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Items = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true }, //own warehouse?
                ItemLines = new EndpointDetail { Full = false },
                ItemGroups = new EndpointDetail { Full = false },
                ItemTypes = new EndpointDetail { Full = false },
                Suppliers = new EndpointDetail { Full = false, GetSingle = true }, //single?
                Orders = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true }, //own warehouse?
                Clients = new EndpointDetail { Full = false },
                Shipments = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = false, Get = true, GetSingle = true, OwnWarehouse = true } //own warehouse?
            }
        },
        new User
        {
            ApiKey = "e1f2g3h4i5",
            App = "Analyst",
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, Get = true, GetSingle = true }, 
                Locations = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Transfers = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Items = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemLines = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemGroups = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemTypes = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Suppliers = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Orders = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Clients = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Shipments = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = false, Get = true, GetSingle = true }
            }
        },
        new User
        {
            ApiKey = "j6k7l8m9n0",
            App = "Logistics",
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, Get = true, GetSingle = true }, 
                Locations = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Transfers = new EndpointDetail { Full = false },
                Items = new EndpointDetail { Full = false, Get = true, Post = true, GetSingle = true  },
                ItemLines = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemGroups = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemTypes = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Suppliers = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Orders = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Clients = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Shipments = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true }
            }
        },
        new User
        {
            ApiKey = "o1p2q3r4s5",
            App = "Sales",
            EndpointAccess = new EndpointAccess
            {
                Full = false,
                Warehouses = new EndpointDetail { Full = false, Get = true, GetSingle = true }, 
                Locations = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Transfers = new EndpointDetail { Full = false },
                Items = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                ItemLines = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemGroups = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                ItemTypes = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Suppliers = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Orders = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Clients = new EndpointDetail { Full = false, Get = true, Post = true, Put = true, GetSingle = true },
                Shipments = new EndpointDetail { Full = false, Get = true, GetSingle = true },
                Inventories = new EndpointDetail { Full = false, Get = true, GetSingle = true }
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

        bool checkfull = path switch
        {
            "warehouses" => access.Warehouses.Full,
            "locations" => access.Locations.Full,
            "transfers" => access.Transfers.Full,
            "items" => access.Items.Full,
            "item_lines" => access.ItemLines.Full,
            "item_groups" => access.ItemGroups.Full,
            "item_types" => access.ItemTypes.Full,
            "suppliers" => access.Suppliers.Full,
            "orders" => access.Orders.Full,
            "clients" => access.Clients.Full,
            "shipments" => access.Shipments.Full,
            "inventories" => access.Inventories.Full,
            _ => false
        };
        if (checkfull) return true;

        if (method == "get")
        {
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
                "inventories" => access.Inventories.Get,
                _ => false
            };
        }
        else if (method == "post")
        {
            return path switch
            {
                "warehouses" => access.Warehouses.Post,
                "locations" => access.Locations.Post,
                "transfers" => access.Transfers.Post,
                "items" => access.Items.Post,
                "item_lines" => access.ItemLines.Post,
                "item_groups" => access.ItemGroups.Post,
                "item_types" => access.ItemTypes.Post,
                "suppliers" => access.Suppliers.Post,
                "orders" => access.Orders.Post,
                "clients" => access.Clients.Post,
                "shipments" => access.Shipments.Post,
                "inventories" => access.Inventories.Post,
                _ => false
            };
        }
        else if (method == "put")
        {
            return path switch
            {
                "warehouses" => access.Warehouses.Put,
                "locations" => access.Locations.Put,
                "transfers" => access.Transfers.Put,
                "items" => access.Items.Put,
                "item_lines" => access.ItemLines.Put,
                "item_groups" => access.ItemGroups.Put,
                "item_types" => access.ItemTypes.Put,
                "suppliers" => access.Suppliers.Put,
                "orders" => access.Orders.Put,
                "clients" => access.Clients.Put,
                "shipments" => access.Shipments.Put,
                "inventories" => access.Inventories.Put,
                _ => false
            };
        }
        else if (method == "getsingle")
        {
            return path switch
            {
                "warehouses" => access.Warehouses.GetSingle,
                "locations" => access.Locations.GetSingle,
                "transfers" => access.Transfers.GetSingle,
                "items" => access.Items.GetSingle,
                "item_lines" => access.ItemLines.GetSingle,
                "item_groups" => access.ItemGroups.GetSingle,
                "item_types" => access.ItemTypes.GetSingle,
                "suppliers" => access.Suppliers.GetSingle,
                "orders" => access.Orders.GetSingle,
                "clients" => access.Clients.GetSingle,
                "shipments" => access.Shipments.GetSingle,
                "inventories" => access.Inventories.GetSingle,
                _ => false
            };
        }
        else if (method == "forcedelete")
        {
            return user.App == "Admin" ? true : false;
        }
        return path switch
        {
            "warehouses" => access.Warehouses.Delete,
            "locations" => access.Locations.Delete,
            "transfers" => access.Transfers.Delete,
            "items" => access.Items.Delete,
            "item_lines" => access.ItemLines.Delete,
            "item_groups" => access.ItemGroups.Delete,
            "item_types" => access.ItemTypes.Delete,
            "suppliers" => access.Suppliers.Delete,
            "orders" => access.Orders.Delete,
            "clients" => access.Clients.Delete,
            "shipments" => access.Shipments.Delete,
            "inventories" => access.Inventories.Delete,
            _ => false
        };
    }
}

public class User
{
    public string ApiKey { get; set; }
    public string App { get; set; }
    public List<int> OwnWarehouses { get; set;} = new List<int>();
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
    public EndpointDetail Inventories { get; set; }
}

public class EndpointDetail
{
    public bool Full { get; set; }
    public bool Get { get; set; }
    public bool OwnWarehouse { get; set; }
    public bool GetSingle { get; set; }
    public bool Post { get; set; }
    public bool Put { get; set; }
    public bool Delete { get; set; }
}
