using System;
using Newtonsoft.Json;
using Xunit;
using Models;

public class WarehouseModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsWarehouseObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Code"": ""WH001"",
            ""Name"": ""Test Warehouse"",
            ""Address"": ""123 Test St"",
            ""Zip"": ""12345"",
            ""City"": ""Test City"",
            ""Province"": ""Test Province"",
            ""Country"": ""Test Country"",
            ""Contact"": {
                ""Name"": ""John Doe"",
                ""Phone"": ""123-456-7890"",
                ""Email"": ""john@example.com""
            },
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var warehouse = JsonConvert.DeserializeObject<Warehouse>(json);

        // Assert
        Assert.NotNull(warehouse);
        Assert.Equal(1, warehouse.Id);
        Assert.Equal("WH001", warehouse.Code);
        Assert.Equal("Test Warehouse", warehouse.Name);
        Assert.Equal("123 Test St", warehouse.Address);
        Assert.Equal("12345", warehouse.Zip);
        Assert.Equal("Test City", warehouse.City);
        Assert.Equal("Test Province", warehouse.Province);
        Assert.Equal("Test Country", warehouse.Country);
        Assert.NotNull(warehouse.Contact);
        Assert.Equal("John Doe", warehouse.Contact.Name);
        Assert.Equal("123-456-7890", warehouse.Contact.Phone);
        Assert.Equal("john@example.com", warehouse.Contact.Email);
        Assert.Equal("2023-01-01T00:00:00Z", warehouse.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", warehouse.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsWarehouseObject()
    {
        // Arrange
        string json = @"{
            ""Code"": ""WH001"",
            ""Name"": ""Test Warehouse"",
            ""Address"": ""123 Test St"",
            ""Zip"": ""12345"",
            ""City"": ""Test City"",
            ""Country"": ""Test Country""
        }";

        // Act
        var warehouse = JsonConvert.DeserializeObject<Warehouse>(json);

        // Assert
        Assert.NotNull(warehouse);
        Assert.Null(warehouse.Id);
        Assert.Null(warehouse.Province);
        Assert.Null(warehouse.Contact);
        Assert.Null(warehouse.Created_At);
        Assert.Null(warehouse.Updated_At);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Id"": ""Not a number"",
            ""Code"": 12345,
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Warehouse>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Code"": ""WH001""
        }";

        // Act
        var warehouse = JsonConvert.DeserializeObject<Warehouse>(json);

        // Assert
        Assert.NotNull(warehouse);
        Assert.Equal(1, warehouse.Id);
        Assert.Equal("WH001", warehouse.Code);
        Assert.Null(warehouse.Name);
        Assert.Null(warehouse.Address);
        Assert.Null(warehouse.Zip);
        Assert.Null(warehouse.City);
        Assert.Null(warehouse.Country);
    }
}

