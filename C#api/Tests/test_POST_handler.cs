using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;
using Xunit;
using System.Runtime.CompilerServices;

public class ApiPostTests
{
    private readonly HttpClient _client;

    public ApiPostTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }
    #region Clients
    [Fact]
    public async Task Create_Client()
    {
        var newClient = new Client
        {
            Id = 0,
            Name = "New Client",
            Address = "123 Main St",
            City = "Anytown",
            Zip_code = "12345",
            Province = "Province",
            Country = "Country",
            Contact_name = "John Doe",
            Contact_phone = "123-456-7890",
            Contact_email = "johndoe@example.com"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(newClient), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("clients", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Client_With_Invalid_Data()
    {
        var invalidClient = new Client
        {
            Name = "", // Invalid because it has no ID
            Address = "123 Main St",
            City = "Anytown",
            Zip_code = "12345",
            Province = "Province",
            Country = "Country",
            Contact_name = "John Doe",
            Contact_phone = "123-456-7890",
            Contact_email = "johndoe@example.com"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(invalidClient), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("clients", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Client()
    {
        var duplicateClient = new Client
        {
            Id = 1, // Assume this ID already exists
            Name = "Duplicate Client",
            Address = "123 Main St",
            City = "Anytown",
            Zip_code = "12345",
            Province = "Province",
            Country = "Country",
            Contact_name = "Jane Doe",
            Contact_phone = "987-654-3210",
            Contact_email = "janedoe@example.com"
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateClient), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("clients", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Clients
    #region Shipments

    [Fact]
    public async Task Create_Shipment()
    {
        var newShipment = new Shipment
        {
            Id = 0,
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "Standard",
            Shipment_Status = "Pending",
            Notes = "Handle with care",
            Carrier_Code = "UPS",
            Carrier_Description = "United Parcel Service",
            Service_Code = "Ground",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 3,
            Total_Package_Weight = 10.5,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 2 },
                new ShipmentItem { Item_Id = "item2", Amount = 1 }
            }
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(newShipment), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("shipments", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Shipment_With_Invalid_Data()
    {
        var invalidShipment = new Shipment
        {
            Order_Id = 1, // Invalid because there is no Id
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "", 
            Shipment_Status = "Pending",
            Notes = "Handle with care",
            Carrier_Code = "UPS",
            Carrier_Description = "United Parcel Service",
            Service_Code = "Ground",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 3,
            Total_Package_Weight = 10.5,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 2 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidShipment), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("shipments", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Shipment()
    {
        var duplicateShipment = new Shipment
        {
            Id = 1, // Assume this ID already exists
            Order_Id = 1,
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Shipment_Date = "2024-10-20",
            Shipment_Type = "Standard",
            Shipment_Status = "Pending",
            Notes = "Handle with care",
            Carrier_Code = "UPS",
            Carrier_Description = "United Parcel Service",
            Service_Code = "Ground",
            Payment_Type = "Prepaid",
            Transfer_Mode = "Air",
            Total_Package_Count = 3,
            Total_Package_Weight = 10.5,
            Items = new List<ShipmentItem>
            {
                new ShipmentItem { Item_Id = "item1", Amount = 2 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateShipment), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("shipments", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Shipments
    #region ItemGroup

    [Fact]
    public async Task Create_ItemGroup()
    {
        var newItemGroup = new ItemGroup
        {
            Id = 300,
            Name = "New Item Group",
            Description = "Description of new group"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(newItemGroup), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("item_groups", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_ItemGroup_With_Invalid_Data()
    {
        var invalidItemGroup = new ItemGroup
        {
            Name = "", // Invalid because there is no Id
            Description = "Description of invalid group"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(invalidItemGroup), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("item_groups", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_ItemGroup()
    {
        var duplicateItemGroup = new ItemGroup
        {
            Id = 1, // Assume this ID already exists
            Name = "Duplicate Item Group",
            Description = "Description of duplicate group"
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateItemGroup), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("item_groups", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion ItemGroup
    #region Suppliers

    [Fact]
    public async Task Create_Supplier()
    {
        var newSupplier = new Supplier
        {
            Id = 0,
            Code = "SUP0001",
            Name = "test",
            Address = "5989 Sullivan Drives",
            Address_Extra = "Apt. 996",
            City = "Port Anitaburgh",
            Zip_Code = "91688",
            Province = "Illinois",
            Country = "Czech Republic",
            Contact_Name = "Toni Barnett",
            Phonenumber = "363.541.7282x36825",
            Reference = "LPaJ-SUP0001"
        };

        var content = new StringContent(JsonConvert.SerializeObject(newSupplier), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("suppliers", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSupplier = JsonConvert.DeserializeObject<Supplier>(await response.Content.ReadAsStringAsync());
        Assert.Equal(newSupplier.Id, createdSupplier.Id);
    }

    [Fact]
    public async Task Create_Supplier_With_Invalid_Data()
    {
        var invalidSupplier = new Supplier
        {
            Id = 0,
            Code = "SUP0001",
            Name = "",
            Address = "5989 Sullivan Drives",
            Address_Extra = "Apt. 996",
            City = "Port Anitaburgh",
            Zip_Code = "91688",
            Province = "Illinois",
            Country = "Czech Republic",
            Contact_Name = "Toni Barnett",
            Phonenumber = "363.541.7282x36825",
            Reference = "LPaJ-SUP0001"
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidSupplier), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("suppliers", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Supplier()
    {
        var duplicateSupplier = new Supplier
        {
            Id = 0, // Assume this ID already exists
            Code = "SUP0001",
            Name = "Existing Supplier",
            Address = "5989 Sullivan Drives",
            Address_Extra = "Apt. 996",
            City = "Port Anitaburgh",
            Zip_Code = "91688",
            Province = "Illinois",
            Country = "Czech Republic",
            Contact_Name = "Toni Barnett",
            Phonenumber = "363.541.7282x36825",
            Reference = "LPaJ-SUP0001"
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateSupplier), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("suppliers", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Suppliers
    #region Transfers

    [Fact]
    public async Task Create_Transfer()
    {
        var newTransfer = new Transfer
        {
            Id = 0,
            Reference = "TEST00001",
            Transfer_From = null,
            Transfer_To = 9229,
            Transfer_Status = "Completed",
            Created_At = "2000-03-11T13:11:14Z",
            Updated_At = DateTime.Parse("2000-03-12T16:11:14Z").ToString("o"),
            Items = new List<TransferItem>
            {
                new TransferItem { Item_Id = "P007435", Amount = 23 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(newTransfer), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("transfers", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdTransfer = JsonConvert.DeserializeObject<Transfer>(await response.Content.ReadAsStringAsync());
        Assert.Equal(newTransfer.Id, createdTransfer.Id);
    }

    [Fact]
    public async Task Create_Transfer_With_Invalid_Data()
    {
        var invalidTransfer = new Transfer
        {
            Id = 0,
            Reference = "", // Invalid because Reference is empty
            Transfer_From = null,
            Transfer_To = 9229,
            Transfer_Status = "Completed",
            Created_At = "2000-03-11T13:11:14Z",
            Updated_At = DateTime.Parse("2000-03-12T16:11:14Z").ToString("o"),
            Items = new List<TransferItem>
            {
                new TransferItem { Item_Id = "P007435", Amount = 23 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidTransfer), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("transfers", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Transfer()
    {
        var duplicateTransfer = new Transfer
        {
            Id = 0,
            Reference = "TEST00001",
            Transfer_From = null,
            Transfer_To = 9229,
            Transfer_Status = "Completed",
            Created_At = "2000-03-11T13:11:14Z",
            Updated_At = DateTime.Parse("2000-03-12T16:11:14Z").ToString("o"),
            Items = new List<TransferItem>
            {
                new TransferItem { Item_Id = "P007435", Amount = 23 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateTransfer), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("transfer", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Transfers
    #region Warehouses

    [Fact]
    public async Task Create_Warehouse()
    {
        var newWarehouse = new Warehouse
        {
            Id = 0,
            Code = "YQZZNL56",
            Name = "TEST cargo hub",
            Address = "Karlijndreef 281",
            Zip = "4002 AS",
            City = "Heemskerk",
            Province = "Friesland",
            Country = "NL",
            Contact = new ContactInfo
            {
                Name = "Fem Keijzer",
                Phone = "(078) 0013363",
                Email = "blamore@example.net"
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(newWarehouse), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("warehouses", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdWarehouse = JsonConvert.DeserializeObject<Warehouse>(await response.Content.ReadAsStringAsync());
        Assert.Equal(newWarehouse.Id, createdWarehouse.Id);
    }
    [Fact]
    public async Task Create_Warehouse_With_Invalid_Data()
    {
        var invalidWarehouse = new Warehouse
        {
            Id = 0,
            Code = "", // Invalid: Code is empty
            Name = "", // Invalid: Name is empty
            Address = "Karlijndreef 281",
            Zip = "4002 AS",
            City = "Heemskerk",
            Province = "Friesland",
            Country = "NL",
            Contact = new ContactInfo
            {
                Name = "Fem Keijzer",
                Phone = "(078) 0013363",
                Email = "blamore@example.net"
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidWarehouse), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("warehouses", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task Attempt_To_Create_Duplicate_Warehouse()
    {
        var duplicateWarehouse = new Warehouse
        {
            Id = 1, // Assume this ID already exists
            Code = "WH001",
            Name = "Main Warehouse",
            Address = "Karlijndreef 281",
            Zip = "4002 AS",
            City = "Heemskerk",
            Province = "Friesland",
            Country = "NL",
            Contact = new ContactInfo
            {
                Name = "Fem Keijzer",
                Phone = "(078) 0013363",
                Email = "blamore@example.net"
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateWarehouse), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("warehouses", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Warehouses
    #region ItemType

    [Fact]
    public async Task Create_ItemType()
    {
        var newItemType = new ItemType
        {
            Id = 12, 
            Name = "New Item Type",
            Description = "Description of new item type",
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(newItemType), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("item_types", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_ItemType_With_Invalid_Data()
    {
        var invalidItemType = new ItemType
        {
            Name = "", // Invalid because there is no Id
            Description = "Description of invalid item type",
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(invalidItemType), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("item_types", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_ItemType()
    {
        var duplicateItemType = new ItemType
        {
            Id = 1, // Assume this ID already exists
            Name = "Duplicate Item Type",
            Description = "Description of duplicate item type",
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateItemType), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("item_types", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion ItemType
    #region Item

    [Fact]
    public async Task Create_Item()
    {
        var newItem = new Item
        {
            Uid = "item-001",
            Code = "ITEM001",
            Description = "New Item Description",
            Short_Description = "Short description of new item",
            Upc_Code = "012345678901",
            Model_Number = "ModelX",
            Commodity_Code = "Commodity123",
            Item_Line = 1,
            Item_Group = 1,
            Item_Type = 1,
            Unit_Purchase_Quantity = 10,
            Unit_Order_Quantity = 5,
            Pack_Order_Quantity = 15,
            Supplier_Id = 1,
            Supplier_Code = "SUP001",
            Supplier_Part_Number = "PART001",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(newItem), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("items", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Item_With_Invalid_Data()
    {
        var invalidItem = new Item
        {
            Code = "ITEM001",
            Description = "New Item Description",
            Short_Description = "Short description of new item",
            Upc_Code = "012345678901",
            Model_Number = "ModelX",
            Commodity_Code = "Commodity123",
            Item_Line = 1,
            Item_Group = 1,
            Item_Type = 1,
            Unit_Purchase_Quantity = 10,
            Unit_Order_Quantity = 5,
            Pack_Order_Quantity = 15,
            Supplier_Id = 1,
            Supplier_Code = "SUP001",
            Supplier_Part_Number = "PART001",
            Created_At = DateTime.UtcNow.ToString("o.o"),
            Updated_At = DateTime.UtcNow.ToString("o.o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidItem), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("items", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Item()
    {
        var duplicateItem = new Item
        {
            Uid = "item-001", // Assume this ID already exists
            Code = "ITEM001",
            Description = "New Item Description",
            Short_Description = "Short description of new item",
            Upc_Code = "012345678901",
            Model_Number = "ModelX",
            Commodity_Code = "Commodity123",
            Item_Line = 1,
            Item_Group = 1,
            Item_Type = 1,
            Unit_Purchase_Quantity = 10,
            Unit_Order_Quantity = 5,
            Pack_Order_Quantity = 15,
            Supplier_Id = 1,
            Supplier_Code = "SUP001",
            Supplier_Part_Number = "PART001",
            Created_At = DateTime.UtcNow.ToString("o.o"),
            Updated_At = DateTime.UtcNow.ToString("o.o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateItem), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("items", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Item
    #region Orders

    [Fact]
    public async Task Create_Order()
    {
        var newOrder = new Order
        {
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Reference = "Ref001",
            Reference_Extra = "ExtraRef",
            Order_Status = "Pending",
            Notes = "New order notes",
            Shipping_Notes = "Ship quickly",
            Picking_Notes = "Pick carefully",
            Warehouse_Id = 1,
            Ship_To = 1,
            Bill_To = 2,
            Shipment_Id = null,
            Total_Amount = 150.75m,
            Total_Discount = 10.00m,
            Total_Tax = 5.00m,
            Total_Surcharge = 0.00m,
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o"),
            Items = new List<OrderItem>
            {
                new OrderItem { Item_Id = "item1", Amount = 2 },
                new OrderItem { Item_Id = "item2", Amount = 1 }
            }
        };  

        var content = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("orders", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Order_With_Invalid_Data()
    {
        var invalidOrder = new Order
        {
            Source_Id = 1,
            Order_Date = "",
            Request_Date = "2024-10-19",
            Reference = "",
            Reference_Extra = "",
            Order_Status = "",
            Notes = "order notes",
            Shipping_Notes = "Ship slower",
            Picking_Notes = "Pick fast",
            Warehouse_Id = -1,
            Ship_To = null,
            Bill_To = null,
            Shipment_Id = null,
            Total_Amount = -150.75m,
            Total_Discount = -10.00m,
            Total_Tax = -5.00m,
            Total_Surcharge = -3.00m,
            Created_At = "", // Invalid date
            Updated_At = "" // Invalid date
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidOrder), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("orders", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Order()
    {
        var duplicateOrder = new Order
        {
            Source_Id = 1,
            Order_Date = "2024-10-18",
            Request_Date = "2024-10-19",
            Reference = "Ref001",
            Reference_Extra = "ExtraRef",
            Order_Status = "Pending",
            Notes = "New order notes",
            Shipping_Notes = "Ship quickly",
            Picking_Notes = "Pick carefully",
            Warehouse_Id = 1,
            Ship_To = 1,
            Bill_To = 2,
            Shipment_Id = null,
            Total_Amount = 150.75m,
            Total_Discount = 10.00m,
            Total_Tax = 5.00m,
            Total_Surcharge = 0.00m,
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o"),
            Items = new List<OrderItem>
            {
                new OrderItem { Item_Id = "item1", Amount = 2 },
                new OrderItem { Item_Id = "item2", Amount = 1 }
            }
        };  

        var content = new StringContent(JsonConvert.SerializeObject(duplicateOrder), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("orders", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Orders
    #region Inventories

    [Fact]
    public async Task Create_Inventory() //happy
    {
        var newInventory = new Inventory
        {
            Id = 0,
            Item_Id = "P000001",
            Description = "Sample Inventory Item",
            Item_Reference = "Ref001",
            Locations = new List<int> { 1, 2 }, 
            Total_On_Hand = 100,
            Total_Expected = 150,
            Total_Ordered = 50,
            Total_Allocated = 20,
            Total_Available = 80,
            Created_At = DateTime.UtcNow.ToString("o"), 
            Updated_At = DateTime.UtcNow.ToString("o")
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(newInventory), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("inventories", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Inventory_With_Invalid_Data() // Non happy
    {
        var invalidInventory = new Inventory
        {
            Item_Id = "", // will fail because id is required
            Description = "Invalid Inventory Item",
            Item_Reference = "Ref002",
            Locations = new List<int> { 1, 2 },
            Total_On_Hand = 100,
            Total_Expected = 150,
            Total_Ordered = 50,
            Total_Allocated = 20,
            Total_Available = 80,
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };
        var content = new StringContent(JsonConvert.SerializeObject(invalidInventory), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("inventories", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Inventory() // Non happy
    {
        var duplicateInventory = new Inventory
        {
            Id = 1, // Assume this ID already exists
            Item_Id = "P000001",
            Description = "Duplicate Inventory Item",
            Item_Reference = "Ref003",
            Locations = new List<int> { 1, 2 },
            Total_On_Hand = 100,
            Total_Expected = 150,
            Total_Ordered = 50,
            Total_Allocated = 20,
            Total_Available = 80,
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };
        var content = new StringContent(JsonConvert.SerializeObject(duplicateInventory), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("inventories", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    #endregion Inventories
    #region ItemLines

    [Fact]
    public async Task Create_ItemLine() //happy
    {
        var newItemLine = new ItemLine
        {
            Id = 0,
            Name = "New Item Line",
            Description = "Description of the new item line",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(newItemLine), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("item_lines", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_ItemLine_With_Invalid_Data() //Non happy
    {
        var invalidItemLine = new ItemLine
        {
            Name = "",
            Description = "Invalid Item Line",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidItemLine), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("item_lines", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_ItemLine() // Non happy
    {
        var duplicateItemLine = new ItemLine
        {
            Id = 1,
            Name = "Duplicate Item Line",
            Description = "Duplicate item line description",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateItemLine), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("item_lines", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion ItemLines
    #region Locations

    // Locations
    [Fact]
    public async Task Create_Location() //happy
    {
        var newLocation = new Location
        {
            Id = 0,
            Warehouse_Id = 1,
            Code = "LOC001",
            Name = "New Location",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(newLocation), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("locations", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_Location_With_Invalid_Data() //Non happy
    {
        var invalidLocation = new Location
        {
            Warehouse_Id = 0,
            Code = "",
            Name = "Invalid Location",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidLocation), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("locations", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempt_To_Create_Duplicate_Location()// Non happy
    {
        var duplicateLocation = new Location
        {
            Id = 1,
            Warehouse_Id = 1,
            Code = "LOC001",
            Name = "Duplicate Location",
            Created_At = DateTime.UtcNow.ToString("o"),
            Updated_At = DateTime.UtcNow.ToString("o")
        };

        var content = new StringContent(JsonConvert.SerializeObject(duplicateLocation), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("locations", content);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    #endregion Locations


}
