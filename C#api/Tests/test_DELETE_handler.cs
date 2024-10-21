using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Xunit;

public class ApiDeleteTests
{
    private readonly HttpClient _client;

    public ApiDeleteTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }

    private async Task<int> CreateClientAsync(Client client)
    {
        var content = new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("clients", content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.
        var createdClient = JsonConvert.DeserializeObject<Client>(await response.Content.ReadAsStringAsync());
        return createdClient.Id;
    }

    [Fact]
    public async Task Delete_Client()
    {
        var client = new Client { Name = "Test Client", Address = "123 Test St", City = "Test City", Zip_code = "12345" };
        int clientId = await CreateClientAsync(client);

        var response = await _client.DeleteAsync($"clients/{clientId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Client()
    {
        var response = await _client.DeleteAsync("clients/-1"); // Assume this ID does not exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Client_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync($"clients/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateShipmentAsync(Shipment shipment)
    {
        var content = new StringContent(JsonConvert.SerializeObject(shipment), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("shipments", content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.
        var createdShipment = JsonConvert.DeserializeObject<Shipment>(await response.Content.ReadAsStringAsync());
        return createdShipment.Id;
    }

    [Fact]
    public async Task Delete_Shipment_Should()
    {
        var shipment = new Shipment
        {
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "Express",
            Shipment_Status = "Shipped",
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 1 }
            }
        };
        int shipmentId = await CreateShipmentAsync(shipment);

        var response = await _client.DeleteAsync($"shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Shipment()
    {
        var response = await _client.DeleteAsync("shipments/-1"); // Assume this ID does not exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Shipment_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync($"shipments/1"); // Assume this shipment has dependent data
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateItemGroupAsync(ItemGroup itemGroup)
    {
        var content = new StringContent(JsonConvert.SerializeObject(itemGroup), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("itemgroups", content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.
        var createdItemGroup = JsonConvert.DeserializeObject<ItemGroup>(await response.Content.ReadAsStringAsync());
        return createdItemGroup.Id;
    }

    [Fact]
    public async Task Delete_ItemGroup_Should()
    {
        var itemGroup = new ItemGroup { Name = "Test Item Group", Description = "Description for test group" };
        int itemGroupId = await CreateItemGroupAsync(itemGroup);

        var response = await _client.DeleteAsync($"item_groups/{itemGroupId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_ItemGroup()
    {
        var response = await _client.DeleteAsync("item_groups/-1"); // Assume this ID does not exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ItemGroup_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync($"item_groups/1"); // Assume this item group has dependent data
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateSupplierAsync(Supplier supplier)
    {
        var content = new StringContent(JsonConvert.SerializeObject(supplier), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("suppliers", content);
        response.EnsureSuccessStatusCode();
        var createdSupplier = JsonConvert.DeserializeObject<Supplier>(await response.Content.ReadAsStringAsync());
        return createdSupplier.Id;
    }
    
    [Fact]
    public async Task Delete_Supplier()
    {
        var supplier = new Supplier { Name = "Test Supplier", Address = "123 Supplier St" };
        int supplierId = await CreateSupplierAsync(supplier);
        var response = await _client.DeleteAsync($"suppliers/{supplierId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Supplier()
    {
        var response = await _client.DeleteAsync("suppliers/-1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Supplier_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync($"suppliers/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    private async Task<int> CreateTransferAsync(Transfer transfer)
    {
        var content = new StringContent(JsonConvert.SerializeObject(transfer), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("transfers", content);
        response.EnsureSuccessStatusCode();
        var createdTransfer = JsonConvert.DeserializeObject<Transfer>(await response.Content.ReadAsStringAsync());
        return createdTransfer.Id;
    }

    [Fact]
    public async Task Delete_Transfer()
    {
        var transfer = new Transfer
        {
            Items = new List<TransferItem>
            {
                new TransferItem { Item_Id = "item1" },
                new TransferItem { Item_Id = "item2" }
            },
            Transfer_Status = "Pending"
        };
        int transferId = await CreateTransferAsync(transfer);
        var response = await _client.DeleteAsync($"transfers/{transferId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Transfer()
    {
        var response = await _client.DeleteAsync("transfers/-1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Transfer_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync($"transfers/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateWarehouseAsync(Warehouse warehouse)
    {
        var content = new StringContent(JsonConvert.SerializeObject(warehouse), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("warehouses", content);
        response.EnsureSuccessStatusCode();
        var createdWarehouse = JsonConvert.DeserializeObject<Warehouse>(await response.Content.ReadAsStringAsync());
        return createdWarehouse.Id;
    }

    [Fact]
    public async Task Delete_Warehouse()
    {
        var warehouse = new Warehouse { Name = "Test Warehouse", Address = "Test Address" };
        int warehouseId = await CreateWarehouseAsync(warehouse);
        var response = await _client.DeleteAsync($"warehouses/{warehouseId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Warehouse()
    {
        var response = await _client.DeleteAsync("warehouses/-1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Warehouse_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync($"warehousess/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateItemTypeAsync(ItemType itemType)
    {
        var content = new StringContent(JsonConvert.SerializeObject(itemType), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("item_types", content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.
        var createdItemType = JsonConvert.DeserializeObject<ItemType>(await response.Content.ReadAsStringAsync());
        return createdItemType.Id;
    }

    [Fact]
    public async Task Delete_ItemType_Should()
    {   
        var itemType = new ItemType { Name = "Test Item Type", Description = "Description for test type" };
        int itemTypeId = await CreateItemTypeAsync(itemType);

        var response = await _client.DeleteAsync("item_types/1");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_ItemType()
    {
        var response = await _client.DeleteAsync("item_types/-1"); // Assume this ID does not exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ItemType_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync("item_types/1"); // Assume this item type has dependent data
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<string> CreateItemAsync(Item item)
    {
        var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("items", content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.
        var createdItem = JsonConvert.DeserializeObject<Item>(await response.Content.ReadAsStringAsync());
        return createdItem.Uid;
    }

    [Fact]
    public async Task Delete_Item_Should()
    {
        var item = new Item
        {
            Code = "Test Item",
            Description = "Description for test item",
            Short_Description = "Short description",
            Upc_Code = "1234567890",
            Model_Number = "Model 123",
            Commodity_Code = "Commodity 123",
            Item_Line = 1,
            Item_Group = 1,
            Item_Type = 1,
            Unit_Purchase_Quantity = 1,
            Unit_Order_Quantity = 1,
            Pack_Order_Quantity = 1,
            Supplier_Id = 1,
            Supplier_Code = "Supplier 123",
            Supplier_Part_Number = "Part 123"
        };
        string itemUid = await CreateItemAsync(item);

        var response = await _client.DeleteAsync($"items/{itemUid}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Item()
    {
        var response = await _client.DeleteAsync("items/-1"); // Assume this ID does not exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Item_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync("items/1"); // Assume this item has dependent data
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    private async Task<int> CreateOrderAsync(Order order)
    {
        var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("orders", content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.
        var createdOrder = JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());
        return createdOrder.Id;
    }

    [Fact]
    public async Task Delete_Order_Should()
    {
        var order = new Order
        {
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Reference = "Test Order",
            Order_Status = "Scheduled",
            Items = new List<OrderItem>
            {
                new OrderItem { Item_Id = "item1", Amount = 1 }
            }
        };
        int orderId = await CreateOrderAsync(order);

        var response = await _client.DeleteAsync($"orders/{orderId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Non_Existent_Order()
    {
        var response = await _client.DeleteAsync("orders/-1"); // Assume this ID does not exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Order_When_Other_Data_Is_Dependent()
    {
        var response = await _client.DeleteAsync("orders/1"); // Assume this order has dependent data
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
