using System;
using Models;
using System.IO;

class DataProvider
{
    private static readonly bool DEBUG = false;
    private static readonly string ROOT_PATH = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "data").Replace('\\', '/');

    private static Warehouses _warehouses;
    private static Locations _locations;
    private static Transfers _transfers;
    private static Items _items;
    private static ItemLines _itemLines;
    private static ItemGroups _itemGroups;
    private static ItemTypes _itemTypes;
    private static Inventories _inventories;
    private static Suppliers _suppliers;
    private static Orders _orders;
    private static Clients _clients;
    private static Shipments _shipments;


    public static void Init()
    {
        _warehouses = new Warehouses(ROOT_PATH, DEBUG);
        _locations = new Locations(ROOT_PATH, DEBUG);
        _transfers = new Transfers(ROOT_PATH, DEBUG);
        _items = new Items(ROOT_PATH, DEBUG);
        _itemLines = new ItemLines(ROOT_PATH, DEBUG);
        _itemGroups = new ItemGroups(ROOT_PATH, DEBUG);
        _itemTypes = new ItemTypes(ROOT_PATH, DEBUG);
        _inventories = new Inventories(ROOT_PATH, DEBUG);
        _suppliers = new Suppliers(ROOT_PATH, DEBUG);
        _orders = new Orders(ROOT_PATH, DEBUG);
        _clients = new Clients(ROOT_PATH, DEBUG);
        _shipments = new Shipments(ROOT_PATH, DEBUG);
    }

    public static Warehouses fetch_warehouse_pool() => _warehouses;
    public static Locations fetch_location_pool() => _locations;
    public static Transfers fetch_transfer_pool() => _transfers;
    public static Items fetch_item_pool() => _items;
    public static ItemLines fetch_itemline_pool() => _itemLines;
    public static ItemGroups fetch_itemgroup_pool() => _itemGroups;
    public static ItemTypes fetch_itemtype_pool() => _itemTypes;
    public static Inventories fetch_inventory_pool() => _inventories;
    public static Suppliers fetch_supplier_pool() => _suppliers;
    public static Orders fetch_order_pool() => _orders;
    public static Clients fetch_client_pool() => _clients;
    public static Shipments fetch_shipment_pool() => _shipments;
}
