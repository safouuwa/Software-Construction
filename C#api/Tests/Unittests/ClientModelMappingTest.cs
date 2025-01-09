using System;
using Newtonsoft.Json;
using Xunit;
using Models;

public class ClientModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsClientObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Name"": ""Test Client"",
            ""Address"": ""456 Client St"",
            ""City"": ""Client City"",
            ""Zip_code"": ""54321"",
            ""Province"": ""Client Province"",
            ""Country"": ""Client Country"",
            ""Contact_name"": ""Jane Smith"",
            ""Contact_phone"": ""987-654-3210"",
            ""Contact_email"": ""jane@example.com"",
            ""Created_at"": ""2023-01-01T00:00:00Z"",
            ""Updated_at"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var client = JsonConvert.DeserializeObject<Client>(json);

        // Assert
        Assert.NotNull(client);
        Assert.Equal(1, client.Id);
        Assert.Equal("Test Client", client.Name);
        Assert.Equal("456 Client St", client.Address);
        Assert.Equal("Client City", client.City);
        Assert.Equal("54321", client.Zip_code);
        Assert.Equal("Client Province", client.Province);
        Assert.Equal("Client Country", client.Country);
        Assert.Equal("Jane Smith", client.Contact_name);
        Assert.Equal("987-654-3210", client.Contact_phone);
        Assert.Equal("jane@example.com", client.Contact_email);
        Assert.Equal("2023-01-01T00:00:00Z", client.Created_at);
        Assert.Equal("2023-01-02T00:00:00Z", client.Updated_at);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsClientObject()
    {
        // Arrange
        string json = @"{
            ""Name"": ""Test Client"",
            ""Address"": ""456 Client St"",
            ""City"": ""Client City"",
            ""Zip_code"": ""54321"",
            ""Country"": ""Client Country"",
            ""Contact_name"": ""Jane Smith"",
            ""Contact_phone"": ""987-654-3210""
        }";

        // Act
        var client = JsonConvert.DeserializeObject<Client>(json);

        // Assert
        Assert.NotNull(client);
        Assert.Null(client.Id);
        Assert.Null(client.Province);
        Assert.Null(client.Contact_email);
        Assert.Null(client.Created_at);
        Assert.Null(client.Updated_at);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Id"": ""Not a number"",
            ""Name"": 12345,
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Client>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Name"": ""Test Client""
        }";

        // Act
        var client = JsonConvert.DeserializeObject<Client>(json);

        // Assert
        Assert.NotNull(client);
        Assert.Equal(1, client.Id);
        Assert.Equal("Test Client", client.Name);
        Assert.Null(client.Address);
        Assert.Null(client.City);
        Assert.Null(client.Zip_code);
        Assert.Null(client.Country);
        Assert.Null(client.Contact_name);
        Assert.Null(client.Contact_phone);
    }
}

