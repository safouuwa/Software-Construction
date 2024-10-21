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


    private async Task<int> CreateInventoryAsync(Inventory inventory)
    {
        var content = new StringContent(JsonConvert.SerializeObject(inventory), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("inventories", content);
        response.EnsureSuccessStatusCode();
        var createdInventory = JsonConvert.DeserializeObject<Inventory>(await response.Content.ReadAsStringAsync());
        return createdInventory.Id;
    }

    private async Task<int> CreateItemLineAsync(ItemLine itemLine)
    {
        var content = new StringContent(JsonConvert.SerializeObject(itemLine), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("item_lines", content);
        response.EnsureSuccessStatusCode();
        var createdItemLine = JsonConvert.DeserializeObject<ItemLine>(await response.Content.ReadAsStringAsync());
        return createdItemLine.Id;
    }

    private async Task<int> CreateLocationAsync(Location location)
    {
        var content = new StringContent(JsonConvert.SerializeObject(location), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("locations", content);
        response.EnsureSuccessStatusCode();
        var createdLocation = JsonConvert.DeserializeObject<Location>(await response.Content.ReadAsStringAsync());
        return createdLocation.Id;
    }


    [Fact]
    public async Task Delete_Inventory() // happy
    {
        var inventory = new Inventory 
        {
            Item_Id = "P000TEST", 
            Description = "Test Inventory",
            Locations = new List<int> { 1, 2 },
            Total_On_Hand = 50,
            Total_Expected = 20,
            Total_Ordered = 10,
            Total_Allocated = 5, 
            Total_Available = 45
        };

        // create inv
        int inventoryId = await CreateInventoryAsync(inventory);

        // test deletion
        var response = await _client.DeleteAsync($"inventories/{inventoryId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // verify that it no longer exists
        var getResponse = await _client.GetAsync($"inventories/{inventoryId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Wrong_Inventory() // non happy
    {
        var nonExistentInventoryId = 99999; 
        var response = await _client.DeleteAsync($"inventories/{nonExistentInventoryId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); 
    }

    //ItemLines
    [Fact]
    public async Task Delete_ItemLine() // happy
    {
        var itemLine = new ItemLine 
        { 
            Name = "Test ItemLine",
            Description = "Description of the test item line"
        };

        int itemLineId = await CreateItemLineAsync(itemLine); 

        // Test deletion
        var response = await _client.DeleteAsync($"item_lines/{itemLineId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // verify
        var getResponse = await _client.GetAsync($"item_lines/{itemLineId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemLine() // non happy
    {
        var nonExistentItemLineId = 99999; 
        var response = await _client.DeleteAsync($"item_lines/{nonExistentItemLineId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); // Expecting 404 Not Found
    }

    //LOCATIONS
    [Fact]
    public async Task Delete_Location() // happy
    {
        var location = new Location 
        {
            Warehouse_Id = 1,
            Code = "LOC001",
            Name = "Test Location"
        };

        int locationId = await CreateLocationAsync(location);

        // Test deletion
        var response = await _client.DeleteAsync($"locations/{locationId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // verify 
        var getResponse = await _client.GetAsync($"locations/{locationId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistent_Location() // non happy
    {
        var nonExistentLocationId = 99999; 
        var response = await _client.DeleteAsync($"locations/{nonExistentLocationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); 
    }










}
