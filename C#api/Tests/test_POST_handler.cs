using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;
using Xunit;

public class ApiPostTests
{
    private readonly HttpClient _client;

    public ApiPostTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }

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
}
