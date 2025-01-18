using System;
using Newtonsoft.Json;
using Xunit;
using ModelsV2;

public class LocationModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsLocationObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Warehouse_Id"": 100,
            ""Code"": ""LOC001"",
            ""Name"": ""Test Location"",
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var location = JsonConvert.DeserializeObject<Location>(json);

        // Assert
        Assert.NotNull(location);
        Assert.Equal(1, location.Id);
        Assert.Equal(100, location.Warehouse_Id);
        Assert.Equal("LOC001", location.Code);
        Assert.Equal("Test Location", location.Name);
        Assert.Equal("2023-01-01T00:00:00Z", location.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", location.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsLocationObject()
    {
        // Arrange
        string json = @"{
            ""Warehouse_Id"": 100,
            ""Code"": ""LOC001"",
            ""Name"": ""Test Location""
        }";

        // Act
        var location = JsonConvert.DeserializeObject<Location>(json);

        // Assert
        Assert.NotNull(location);
        Assert.Null(location.Id);
        Assert.Null(location.Created_At);
        Assert.Null(location.Updated_At);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Id"": ""Not a number"",
            ""Warehouse_Id"": ""Also not a number"",
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Location>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Warehouse_Id"": 100
        }";

        // Act
        var location = JsonConvert.DeserializeObject<Location>(json);

        // Assert
        Assert.NotNull(location);
        Assert.Equal(1, location.Id);
        Assert.Equal(100, location.Warehouse_Id);
        Assert.Null(location.Code);
        Assert.Null(location.Name);
    }
}

