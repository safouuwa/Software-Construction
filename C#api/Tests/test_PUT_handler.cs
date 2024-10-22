using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;	
using Xunit;

public class ApiPutTests
{
    private readonly HttpClient _client;

    public ApiPutTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }
    #region Clients

    [Fact]
    public async Task Update_Existing_Client()
    {
        var updatedClient = new Client
        {
            Id = 1, // Assume this ID exists
            Name = "Updated Client",
            Address = "456 Updated St",
            City = "Updated City",
            Zip_code = "54321",
            Province = "Updated Province",
            Country = "Updated Country",
            Contact_name = "Updated Name",
            Contact_phone = "321-654-0987",
            Contact_email = "updated@example.com"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(updatedClient), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"clients/{updatedClient.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Client()
    {
        var updatedClient = new Client
        {
            Id = -1, // Assume this ID does not exist
            Name = "Non-existent Client",
            Address = "789 Non-existent St",
            City = "Nowhere",
            Zip_code = "00000",
            Province = "Non-existent Province",
            Country = "Non-existent Country",
            Contact_name = "Ghost",
            Contact_phone = "000-000-0000",
            Contact_email = "ghost@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedClient), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"clients/{updatedClient.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Client_With_Invalid_Data()
    {
        var invalidClient = new Client
        {
            Name = "", // Invalid because there is no id
            Address = "456 Updated St",
            City = "Updated City",
            Zip_code = "54321",
            Province = "Updated Province",
            Country = "Updated Country",
            Contact_name = "Updated Name",
            Contact_phone = "321-654-0987",
            Contact_email = "updated@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidClient), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"clients/{invalidClient.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Client_When_Id_In_Data_And_Id_In_Route_Differ()
    {
        var conflictingClient = new Client
        {
            Id = 2, // Different ID from the route
            Name = "Conflicting Client",
            Address = "456 Conflicting St",
            City = "Conflict City",
            Zip_code = "54321",
            Province = "Conflict Province",
            Country = "Conflict Country",
            Contact_name = "Conflict Name",
            Contact_phone = "321-654-0987",
            Contact_email = "conflict@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(conflictingClient), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"clients/1", content); // Route ID is 1, data ID is 2
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion Clients
    #region ItemGroup

    [Fact]
    public async Task Update_Existing_ItemGroup()
    {
        var updatedItemGroup = new ItemGroup
        {
            Id = 1, // Assume this ID exists
            Name = "Updated Item Group",
            Description = "Updated description"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItemGroup), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_groups/{updatedItemGroup.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_ItemGroup()
    {
        var updatedItemGroup = new ItemGroup
        {
            Id = 999, // Assume this ID does not exist
            Name = "Non-existent Item Group",
            Description = "This item group does not exist"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItemGroup), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_groups/{updatedItemGroup.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ItemGroup_With_Invalid_Data()

    {
        var invalidItemGroup = new ItemGroup
        {
            Name = "", // Invalid because there is no Id
            Description = "Some description"
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidItemGroup), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_groups/{invalidItemGroup.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ItemGroup_When_Id_In_Data_And_Id_In_Route_Differ()
    {
        var conflictingItemGroup = new ItemGroup
        {
            Id = 2, // Different ID from the route
            Name = "Conflicting Item Group",
            Description = "This item group has a conflicting ID"
        };

        var content = new StringContent(JsonConvert.SerializeObject(conflictingItemGroup), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_groups/1", content); // Route ID is 1, data ID is 2
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion ItemGroup
    #region Shipment

    [Fact]
    public async Task Update_Existing_Shipment()
    {
        var updatedShipment = new Shipment
        {
            Id = 1, // Assume this ID exists
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "Express",
            Shipment_Status = "Shipped",
            Notes = "Updated notes",
            Carrier_Code = "FedEx",
            Carrier_Description = "Federal Express",
            Service_Code = "Express",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 2,
            Total_Package_Weight = 5.0,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 1 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedShipment), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"shipments/{updatedShipment.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Shipment()
    {
        var updatedShipment = new Shipment
        {
            Id = -1, // Assume this ID does not exist
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "Standard",
            Shipment_Status = "Pending",
            Notes = "No longer needed",
            Carrier_Code = "UPS",
            Carrier_Description = "United Parcel Service",
            Service_Code = "Ground",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 3,
            Total_Package_Weight = 10.0,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 1 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedShipment), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"shipments/{updatedShipment.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Shipments_With_Invalid_Data()

    {
        var invalidShipment = new Shipment
        {
            // Invalid because there is no Id
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "", // Invalid because Shipment_Type is empty
            Shipment_Status = "Pending",
            Notes = "Invalid update",
            Carrier_Code = "UPS",
            Carrier_Description = "United Parcel Service",
            Service_Code = "Ground",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 3,
            Total_Package_Weight = 10.0,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 1 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidShipment), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"shipments/{invalidShipment.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Shipment_When_Id_In_Data_And_Id_In_Route_Differ()
    {
        var conflictingShipment = new Shipment
        {
            Id = 2, // Different ID from the route
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "Express",
            Shipment_Status = "Shipped",
            Notes = "Conflicting update",
            Carrier_Code = "FedEx",
            Carrier_Description = "Federal Express",
            Service_Code = "Express",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 1,
            Total_Package_Weight = 2.5,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 1 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(conflictingShipment), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"shipments/1", content); // Route ID is 1, data ID is 2
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Orders_In_Shipment()
    {
        var updatedOrders = new List<Order>
        {
            new Order { Id = 1, Source_Id = 1, Order_Date = "2024-10-18", Request_Date = "2024-10-19", Reference = "Ref1", Total_Amount = 100.00m },
            new Order { Id = 2, Source_Id = 1, Order_Date = "2024-10-18", Request_Date = "2024-10-19", Reference = "Ref2", Total_Amount = 200.00m }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedOrders), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/1/orders", content); // Assume shipment ID is 1
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Orders_In_Non_Existent_Shipment()
    {
        var updatedOrders = new List<Order>
        {
            new Order { Id = 999, Source_Id = 1, Order_Date = "2024-10-18", Request_Date = "2024-10-19" } // Assume this ID does not exist
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedOrders), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/999/orders", content); // Non-existent shipment ID
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Orders_With_Invalid_Data()
    {
        var invalidOrders = new List<Order>
        {
            new Order { Source_Id = 1, Order_Date = "2024-10-18", Request_Date = "2024-10-19" } // No ID
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidOrders), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/1/orders", content); // Assume shipment ID is 1
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Items_In_Shipment()
    {
        var updatedItems = new List<Item>
        {
            new Item { Uid = "item1", Code = "Code1", Description = "Description1", Item_Line = 1, Supplier_Id = 1 },
            new Item { Uid = "item2", Code = "Code2", Description = "Description2", Item_Line = 2, Supplier_Id = 1 }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItems), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/1/items", content); // Assume shipment ID is 1
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Items_In_Non_Existent_Shipment()
    {
        var updatedItems = new List<Item>
        {
            new Item { Uid = "item999", Code = "Code999", Description = "Non-existent item", Item_Line = 999, Supplier_Id = 999 } // Assume this UID does not exist
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItems), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/999/items", content); // Non-existent shipment ID
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Items_With_Invalid_Data()
    {
        var invalidItems = new List<Item>
        {
            new Item { Code = "Code1", Description = "Description1", Item_Line = 1, Supplier_Id = 1 } // Invalid Uid
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidItems), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/1/items", content); // Assume shipment ID is 1
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Commit_Shipment()
    {
        var response = await _client.PutAsync("shipments/1/commit", null); // Assume shipment ID is 1
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Commit_Non_Existent_Shipment()
    {
        var response = await _client.PutAsync("shipments/-1/commit", null); // Non-existent shipment ID
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Commit_Shipment_With_Invalid_ID()
    {
        var invalidShipment = new Shipment
        {
            Id = -10 // Invalid ID
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidShipment), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("shipments/1/commit", content); // Assume shipment ID is 1
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion Shipment
    #region Transfer

    [Fact]
    public async Task Update_Existing_Transfer()
    {
        var updatedTransfer = new Transfer
        {
            Id = 1, // Assume this ID exists
            Reference = "Updated Task",
            Transfer_From = 1,
            Transfer_To = 2,
            Transfer_Status = "Completed",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20",
            Items = new List<TransferItem> { new TransferItem { Item_Id = "item1", Amount = 1 } }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedTransfer), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"transfers/{updatedTransfer.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Transfer()
    {
        var updatedTransfer = new Transfer
        {
            Id = -1, // Assume this ID does not exist
            Reference = "Non-existent Task",
            Transfer_From = 1,
            Transfer_To = 2,
            Transfer_Status = "Pending",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20",
            Items = new List<TransferItem> { new TransferItem { Item_Id = "item1", Amount = 1 } }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedTransfer), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"transfers/{updatedTransfer.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Transfer_With_Invalid_Data()
    {
        var invalidTransfer = new Transfer
        {
            Reference = "", // Invalid because there is no Id
            Transfer_From = 1,
            Transfer_To = 2,
            Transfer_Status = "Pending",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20",
            Items = new List<TransferItem> { new TransferItem { Item_Id = "item1", Amount = 1 } }
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidTransfer), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"transfers/{invalidTransfer.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Transfer_When_Id_In_Data_And_Id_In_Route_Differ()
    {
        var conflictingTransfer = new Transfer
        {
            Id = 2, // Different ID from the route
            Reference = "Conflicting Task",
            Transfer_From = 1,
            Transfer_To = 2,
            Transfer_Status = "Pending",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20",
            Items = new List<TransferItem> { new TransferItem { Item_Id = "item1", Amount = 1 } }
        };

        var content = new StringContent(JsonConvert.SerializeObject(conflictingTransfer), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"transfers/1", content); // Route ID is 1, data ID is 2
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async Task Commit_Transfer()
    {
        var commitTransfer = new Transfer
        {
            Id = 1,
            Reference = "Commit Task",
            Transfer_From = 1,
            Transfer_To = 2,
            Transfer_Status = "Pending",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20",
            Items = new List<TransferItem> { new TransferItem { Item_Id = "item1", Amount = 1 } }
        };

        var content = new StringContent(JsonConvert.SerializeObject(commitTransfer), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"transfers/1/commit", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Commit_Non_Existent_Transfer()
    {
        var response = await _client.PutAsync("transfers/-1/commit", null); // Non-existent transfer ID
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Commit_Transfer_With_Invalid_ID()
    {
        var invalidTransfer = new Transfer
        {
            Id = -10 // Invalid ID
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidTransfer), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("transfers/1/commit", content); // Assume transfer ID is 1
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    #endregion Transfer
    #region Supplier

    [Fact]
    public async Task Update_Existing_Supplier()
    {
        var updatedSupplier = new Supplier
        {
            Id = 1, // Assume this ID exists
            Name = "Updated Supplier",
            Address = "456 Updated St",
            City = "Updated City",
            Zip_Code = "54321",
            Province = "Updated Province",
            Country = "Updated Country",
            Contact_Name = "Updated Name",
            Phonenumber = "321-654-0987",
            Contact_email = "updated@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedSupplier), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"suppliers/{updatedSupplier.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Supplier()
    {
        var updatedSupplier = new Supplier
        {
            Id = -1, // Assume this ID does not exist
            Name = "Non-existent Supplier",
            Address = "789 Non-existent St",
            City = "Nowhere",
            Zip_Code = "00000",
            Province = "Non-existent Province",
            Country = "Non-existent Country",
            Contact_Name = "Ghost",
            Phonenumber = "000-000-0000",
            Contact_email = "ghost@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedSupplier), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"suppliers/{updatedSupplier.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Supplier_With_Invalid_Data()
    {
        var invalidSupplier = new Supplier
        {
            Name = "", // Invalid because there is no Id
            Address = "456 Updated St",
            City = "Updated City",
            Zip_Code = "54321",
            Province = "Updated Province",
            Country = "Updated Country",
            Contact_Name = "Updated Name",
            Phonenumber = "321-654-0987",
            Contact_email = "updated@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidSupplier), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"suppliers/{invalidSupplier.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Supplier_When_Id_In_Data_And_Id_In_Route_Differ()
    {
        var conflictingSupplier = new Supplier
        {
            Id = 2, // Different ID from the route
            Name = "Conflicting Supplier",
            Address = "456 Conflicting St",
            City = "Conflict City",
            Zip_Code = "54321",
            Province = "Conflict Province",
            Country = "Conflict Country",
            Contact_Name= "Conflict Name",
            Phonenumber = "321-654-0987",
            Contact_email = "conflict@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(conflictingSupplier), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"suppliers/1", content); // Route ID is 1, data ID is 2
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion Supplier
    #region Warehouse

    [Fact]
    public async Task Update_Existing_Warehouse()
    {
        var updatedWarehouse = new Warehouse
        {
            Id = 1, // Assume this ID exists
            Name = "Updated Warehouse",
            Address = "456 Updated St",
            //Capacity = 1000
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedWarehouse), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"warehouses/{updatedWarehouse.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Warehouse()
    {
        var updatedWarehouse = new Warehouse
        {
            Id = -1, // Assume this ID does not exist
            Name = "Non-existent Warehouse",
            Address = "789 Non-existent St",
            //Capacity = 0
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedWarehouse), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"warehouses/{updatedWarehouse.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Warehouse_With_Invalid_Data()
    {
        var invalidWarehouse = new Warehouse
        {
            Name = "", // Invalid because there is no Id
            Address = "456 Updated St",
            //Capacity = 1000
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidWarehouse), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"warehouses/{invalidWarehouse.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Warehouse_When_Id_In_Data_And_Id_In_Route_Differ()
    {
        var conflictingWarehouse = new Warehouse
        {
            Id = 2, // Different ID from the route
            Name = "Conflicting Warehouse",
            Address = "456 Conflicting St",
            //Capacity = 1000
        };

        var content = new StringContent(JsonConvert.SerializeObject(conflictingWarehouse), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"warehouses/1", content); // Route ID is 1, data ID is 2
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion Warehouse
    #region ItemType

    [Fact]
    public async Task Update_Existing_ItemType()
    {
        var updatedItemType = new ItemType
        {
            Id = 1, // Assume this ID exists
            Name = "Updated Item Type",
            Description = "Updated description",
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItemType), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_types/{updatedItemType.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_ItemType()
    {
        var updatedItemType = new ItemType
        {
            Id = 999, // Assume this ID does not exist
            Name = "Non-existent Item Type",
            Description = "This item type does not exist",
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItemType), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_types/{updatedItemType.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ItemType_With_Invalid_Data()
    {
        var invalidItemType = new ItemType
        {
            Name = "", // Invalid because there is no Id
            Description = "Some description",
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidItemType), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_types/{invalidItemType.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion ItemType
    #region Items

    [Fact]
    public async Task Update_Existing_Item()
    {
        var updatedItem = new Item
        {
            Uid = "item1", // Assume this UID exists
            Code = "Updated Code",
            Description = "Updated description",
            Item_Line = 1,
            Supplier_Id = 1
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItem), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"items/{updatedItem.Uid}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Item()
    {
        var updatedItem = new Item
        {
            Uid = "item999", // Assume this UID does not exist
            Code = "Non-existent Code",
            Description = "This item does not exist",
            Item_Line = 999,
            Supplier_Id = 999
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItem), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"items/{updatedItem.Uid}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Item_With_Invalid_Data()
    {
        var invalidItem = new Item
        {
            Code = "Invalid Code", // Invalid because there is no UID
            Description = "Some description",
            Item_Line = 1,
            Supplier_Id = 1
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidItem), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"items/{invalidItem.Uid}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion Items
    #region Orders

    [Fact]
    public async Task Update_Existing_Order()
    {
        var updatedOrder = new Order
        {
            Id = 1, // Assume this ID exists
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Reference = "Updated Ref",
            Total_Amount = 100.00m
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedOrder), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"orders/{updatedOrder.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Order()
    {
        var updatedOrder = new Order
        {
            Id = -1, // Assume this ID does not exist
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Reference = "Non-existent Ref",
            Total_Amount = 100.00m
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedOrder), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"orders/{updatedOrder.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Order_With_Invalid_Data()
    {
        var invalidOrder = new Order
        {
            Source_Id = 1, // Invalid because there is no Id
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Reference = "Invalid Ref",
            Total_Amount = 100.00m
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidOrder), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"orders/{invalidOrder.Id}", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    #endregion Orders
    #region Inventory

    [Fact]
    public async Task Update_Existing_Inventory() // happy 
    {
        var updatedInventory = new Inventory
        {
            Id = 1, 
            Item_Id = "item1",
            Description = "Updated Inventory",
            Locations = new List<int> { 1, 2 },
            Total_On_Hand = 100,
            Total_Expected = 200,
            Total_Ordered = 150,
            Total_Allocated = 50,
            Total_Available = 50,
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedInventory), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"inventories/{updatedInventory.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Inventory() // Non happy
    {
        var updatedInventory = new Inventory
        {
            Id = -1, 
            Item_Id = "item1",
            Description = "Non-existent Inventory",
            Locations = new List<int> { 1, 2 },
            Total_On_Hand = 100,
            Total_Expected = 200,
            Total_Ordered = 150,
            Total_Allocated = 50,
            Total_Available = 50,
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedInventory), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"inventories/{updatedInventory.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion Inventory
    #region ItemLine

    // ItemLine Tests
    [Fact]
    public async Task Update_Existing_ItemLine() // happy
    {
        var updatedItemLine = new ItemLine
        {
            Id = 1, // Assume this ID exists
            Name = "Updated Item Line",
            Description = "Updated Description",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItemLine), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_lines/{updatedItemLine.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_ItemLine() // non happy
    {
        var updatedItemLine = new ItemLine
        {
            Id = -1, 
            Name = "Non-existent Item Line",
            Description = "Non-existent Description",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedItemLine), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"item_lines/{updatedItemLine.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion ItemLine
    #region Locations

    // Location Tests
    [Fact]
    public async Task Update_Existing_Location() // happy
    {
        var updatedLocation = new Location
        {
            Id = 1, // Assume this ID exists
            Warehouse_Id = 1,
            Code = "LOC01",
            Name = "Updated Location",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedLocation), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"locations/{updatedLocation.Id}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Non_Existent_Location() // non happy
    {
        var updatedLocation = new Location
        {
            Id = -1, // Assume this ID does not exist
            Warehouse_Id = 1,
            Code = "LOC01",
            Name = "Non-existent Location",
            Created_At = "2024-01-01",
            Updated_At = "2024-10-20"
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedLocation), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"locations/{updatedLocation.Id}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    #endregion Locations

}
