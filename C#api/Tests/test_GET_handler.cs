using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;
using Xunit;

public class ApiGetTests
{
    private readonly HttpClient _client;

    public ApiGetTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }

    [Fact]
    public async Task Get_All_Clients()
    {
        var response = await _client.GetAsync("clients");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualClients = JsonConvert.DeserializeObject<List<Client>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_client_pool().GetClients(), actualClients);
    }

    [Fact]
    public async Task Get_Client_By_Id()
    {
        var clientId = 1; // Assume this client exists
        var response = await _client.GetAsync($"clients/{clientId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualClient = JsonConvert.DeserializeObject<Client>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_client_pool().GetClient(clientId), actualClient);

    }

    [Fact]
    public async Task Get_Client_Orders()
    {
        var clientId = 1; // Assume this client exists
        var response = await _client.GetAsync($"clients/{clientId}/orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualClients = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_order_pool().GetOrdersForClient(clientId), actualClients);
    }

    [Fact]
    public async Task Get_Non_Existent_Client()
    {
        var clientId = -1; // Assume this client does not exist
        var response = await _client.GetAsync($"clients/{clientId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_Shipments()
    {
        var response = await _client.GetAsync("shipments");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualShipments = JsonConvert.DeserializeObject<List<Shipment>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_shipment_pool().GetShipments(), actualShipments);
    }


    [Fact]
    public async Task Get_Shipment_By_Id()
    {
        var shipmentId = 1; // Assume this shipment exists
        var response = await _client.GetAsync($"shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualShipment = JsonConvert.DeserializeObject<Shipment>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_shipment_pool().GetShipment(shipmentId), actualShipment);


    }

    [Fact]
    public async Task Get_Shipment_Orders()
    {
        var shipmentId = 1; // Assume this shipment exists
        var response = await _client.GetAsync($"shipments/{shipmentId}/orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualShipments = JsonConvert.DeserializeObject<List<Shipment>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_shipment_pool().GetShipments(), actualShipments);
    }

    [Fact]
    public async Task Get_Shipment_items()
    {
        var shipmentId = 1; // Assume this shipment exists
        var response = await _client.GetAsync($"shipments/{shipmentId}/orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItems = JsonConvert.DeserializeObject<List<ShipmentItem>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_shipment_pool().GetItemsInShipment(shipmentId), actualItems);
    }


    [Fact]
    public async Task Get_Non_Existent_Shipment()
    {
        var shipmentId = -1; // Assume this shipment does not exist
        var response = await _client.GetAsync($"shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_ItemGroups()
    {
        var response = await _client.GetAsync("item_groups");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItemGroups = JsonConvert.DeserializeObject<List<ItemGroup>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_itemgroup_pool().GetItemGroups(), actualItemGroups);
    }


    [Fact]
    public async Task Get_ItemGroup_By_Id()
    {
        var item_groupId = 1; // Assume this item_group exists
        var response = await _client.GetAsync($"item_groups/{item_groupId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItemGroup = JsonConvert.DeserializeObject<ItemGroup>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_itemgroup_pool().GetItemGroup(item_groupId), actualItemGroup);


    }


    [Fact]
    public async Task Get_ItemGroup_Items()
    {
        var item_groupId = 1; // Assume this item_group exists
        var response = await _client.GetAsync($"item_groups/{item_groupId}/orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItems = JsonConvert.DeserializeObject<List<Item>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_item_pool().GetItemsForItemGroup(item_groupId), actualItems);
    }


    [Fact]
    public async Task Get_Non_Existent_ItemGroup()
    {
        var item_groupId = -1; // Assume this item_group does not exist
        var response = await _client.GetAsync($"item_groups/{item_groupId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_Suppliers()
    {
        var response = await _client.GetAsync("suppliers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualSuppliers = JsonConvert.DeserializeObject<List<Supplier>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_supplier_pool().GetSuppliers(), actualSuppliers);
    }

    [Fact]
    public async Task Get_Supplier_By_Id()
    {
        int supplierId = 1; // Example supplier ID
        var response = await _client.GetAsync($"suppliers/{supplierId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualSupplier = JsonConvert.DeserializeObject<Supplier>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_supplier_pool().GetSupplier(supplierId), actualSupplier);
    }
    
    [Fact]
    public async Task Get_Non_Existent_Supplier()
    {
        int supplierId = -1; // Example non-existent supplier ID
        var response = await _client.GetAsync($"suppliers/{supplierId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_Transfers()
    {
        var response = await _client.GetAsync("transfers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualTransfers = JsonConvert.DeserializeObject<List<Transfer>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_transfer_pool().GetTransfers(), actualTransfers);
    }

    [Fact]
    public async Task Get_Transfer_By_Id()
    {
        int transferId = 1; // Example transfer ID
        var response = await _client.GetAsync($"transfers/{transferId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualTransfer = JsonConvert.DeserializeObject<Transfer>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_transfer_pool().GetTransfer(transferId), actualTransfer);
    }

    [Fact]
    public async Task Get_Non_Existent_Transfer()
    {
        int transferId = -1; // Example non-existent transfer ID
        var response = await _client.GetAsync($"transfers/{transferId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_Warehouses()
    {
        var response = await _client.GetAsync("warehouses");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualWarehouses = JsonConvert.DeserializeObject<List<Warehouse>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_warehouse_pool().GetWarehouses(), actualWarehouses);
    }

    [Fact]
    public async Task Get_Warehouse_By_Id()
    {
        int warehouseId = 1; // Example warehouse ID
        var response = await _client.GetAsync($"warehouses/{warehouseId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualWarehouse = JsonConvert.DeserializeObject<Warehouse>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_warehouse_pool().GetWarehouse(warehouseId), actualWarehouse);
    }

    [Fact]
    public async Task Get_Non_Existent_Warehouse()
    {
        int warehouseId = -1; // Example non-existent warehouse ID
        var response = await _client.GetAsync($"warehouses/{warehouseId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_ItemTypes()
    {
        var response = await _client.GetAsync("item_types");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItemTypes = JsonConvert.DeserializeObject<List<ItemType>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_itemtype_pool().GetItemTypes(), actualItemTypes);
    
    }

    [Fact]
    public async Task Get_ItemType_By_Id()
    {
        var item_typeId = 1; // Assume this item_type exists
        var response = await _client.GetAsync($"item_types/{item_typeId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItemType = JsonConvert.DeserializeObject<ItemType>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_itemtype_pool().GetItemType(item_typeId), actualItemType);
    }
    
    [Fact]
    public async Task Get_ItemType_By_Invalid_Id()
    {
        var item_typeId = -1; // Assume this item_type does not exist
        var response = await _client.GetAsync($"item_types/{item_typeId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Non_Existent_ItemType()
    {
        var item_typeId = -1; // Assume this item_type does not exist
        var response = await _client.GetAsync($"item_types/{item_typeId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_Items()
    {
        var response = await _client.GetAsync("items");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItems = JsonConvert.DeserializeObject<List<Item>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_item_pool().GetItems(), actualItems);
    }

    [Fact]
    public async Task Get_Item_By_Id()
    {
        var itemId = 1; // Assume this item exists
        var response = await _client.GetAsync($"items/{itemId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualItem = JsonConvert.DeserializeObject<Item>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_item_pool().GetItem(itemId.ToString()), actualItem);
    }

    [Fact]
    public async Task Get_Non_Existent_Item()
    {
        var itemId = -1; // Assume this item does not exist
        var response = await _client.GetAsync($"items/{itemId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_All_Orders()
    {
        var response = await _client.GetAsync("orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualOrders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_order_pool().GetOrders(), actualOrders);
    }

    [Fact]
    public async Task Get_Order_By_Id()
    {
        var orderId = 1; // Assume this order exists
        var response = await _client.GetAsync($"orders/{orderId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualOrder = JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_order_pool().GetOrder(orderId), actualOrder);
    }

    [Fact]
    public async Task Get_Non_Existent_Order()
    {
        var orderId = -1; // Assume this order does not exist
        var response = await _client.GetAsync($"orders/{orderId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
