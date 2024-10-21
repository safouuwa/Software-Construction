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


    // Inventories
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

    // ItemLines
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



}