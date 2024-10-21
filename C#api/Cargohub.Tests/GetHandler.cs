using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;
using Xunit;

public class GetHandler
{
    private readonly HttpClient _client;
    public GetHandler()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:3000/api/v1/") };
        _client.DefaultRequestHeaders.Add("API_KEY", "a1b2c3d4e5");
    }

    // INVENTORIES
    [Fact]
    public async Task Get_All_Inventories() // happy
    {
        var response = await _client.GetAsync("Inventories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualInventories = JsonConvert.DeserializeObject<List<Inventory>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_inventory_pool().GetInventories(), actualInventories);
    }

    [Fact]
    public async Task Get_One_Inventory() // happy
    {
        var InvID = 1;
        var response = await _client.GetAsync($"Inventories/{InvID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualInventory = JsonConvert.DeserializeObject<Inventory>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_inventory_pool().GetInventory(InvID), actualInventory);
    }

    [Fact]
    public async Task Get_Inventories_Item() // happy
    {
        var InvID = "P000001";
        var response = await _client.GetAsync($"items/{InvID}/inventory");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualInventories = JsonConvert.DeserializeObject<List<Inventory>>(await response.Content.ReadAsStringAsync());
        var expectedInventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(InvID);
        Assert.Equal(expectedInventories, actualInventories);
    }

    [Fact]
    public async Task Get_Inventory_Totals_Item() // happy
    {
        var itemId = "P000001"; // Using the provided item ID
        var response = await _client.GetAsync($"items/{itemId}/inventory/totals");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var actualTotals = JsonConvert.DeserializeObject<Dictionary<string, int>>(await response.Content.ReadAsStringAsync());
        var expectedTotals = DataProvider.fetch_inventory_pool().GetInventoryTotalsForItem(itemId);

        Assert.Equal(expectedTotals["total_expected"], actualTotals["total_expected"]);
        Assert.Equal(expectedTotals["total_ordered"], actualTotals["total_ordered"]);
        Assert.Equal(expectedTotals["total_allocated"], actualTotals["total_allocated"]);
        Assert.Equal(expectedTotals["total_available"], actualTotals["total_available"]);
    }

    // Item_Lines
    [Fact]
    public async Task Get_All_ItemLines() // happy
    {
        var response = await _client.GetAsync("item_lines");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualLines = JsonConvert.DeserializeObject<List<ItemLine>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_itemline_pool().GetItemLines(), actualLines);
    }

    [Fact]
    public async Task Get_One_ItemLine() // happy
    {
        var ILID = 1;
        var response = await _client.GetAsync($"item_lines/{ILID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualLines = JsonConvert.DeserializeObject<ItemLine>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_itemline_pool().GetItemLine(ILID), actualLines);
    }
    //Locations
    [Fact]
    public async Task Get_All_Locations() // happy
    {
        var response = await _client.GetAsync("locations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualLocations = JsonConvert.DeserializeObject<List<Location>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_location_pool().GetLocations(), actualLocations);
    }

    [Fact]
    public async Task Get_One_Location() // happy
    {
        var LocID = 1;
        var response = await _client.GetAsync($"locations/{LocID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualLocations = JsonConvert.DeserializeObject<Location>(await response.Content.ReadAsStringAsync());
        Assert.Equal(DataProvider.fetch_location_pool().GetLocation(LocID), actualLocations);
    }

    [Fact]
    public async Task Get_Location_With_Warehouse() // happy
    {
        var warehouseId = 8; //use 8 because it only has 168 entries, faster test run
        var response = await _client.GetAsync($"warehouses/{warehouseId}/locations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var actualLocations = JsonConvert.DeserializeObject<List<Location>>(await response.Content.ReadAsStringAsync());
        var expectedLocations = DataProvider.fetch_location_pool().GetLocationsInWarehouse(warehouseId);

        Assert.Equal(expectedLocations, actualLocations);
    }




}