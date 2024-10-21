using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

public class TestClientApi
{
    private readonly HttpClient _client;
    private const string BASE_URL = "http://localhost:3000";

    public TestClientApi()
    {
        _client = new HttpClient();
    }

    [Fact]
    public async Task Test_GetClients_HappyPath()
    {
        // Arrange
        var response = await _client.GetAsync($"{BASE_URL}/clients");

        // Act & Assert
        Assert.Equal(200, (int)response.StatusCode);
        var clients = JsonConvert.DeserializeObject<List<object>>(await response.Content.ReadAsStringAsync());
        Assert.IsType<List<object>>(clients); // Ensure it's a list
    }

    [Fact]
    public async Task Test_GetClient_ByValidId()
    {
        // Arrange
        int clientId = 1;
        var response = await _client.GetAsync($"{BASE_URL}/clients/{clientId}");

        // Act & Assert
        Assert.Equal(200, (int)response.StatusCode);
        var client = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(clientId, (int)client["id"]);
    }
}
