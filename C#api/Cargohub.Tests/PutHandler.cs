using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Models;

public class ApiPutTests
{
    private readonly HttpClient _client;

    public ApiPutTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }

    // Inventory Tests
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
}
