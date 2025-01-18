using System;
using Newtonsoft.Json;
using Xunit;
using ModelsV2;

public class ItemLineModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsItemLineObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Name"": ""Test Item Line"",
            ""Description"": ""This is a test item line"",
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var itemLine = JsonConvert.DeserializeObject<ItemLine>(json);

        // Assert
        Assert.NotNull(itemLine);
        Assert.Equal(1, itemLine.Id);
        Assert.Equal("Test Item Line", itemLine.Name);
        Assert.Equal("This is a test item line", itemLine.Description);
        Assert.Equal("2023-01-01T00:00:00Z", itemLine.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", itemLine.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsItemLineObject()
    {
        // Arrange
        string json = @"{
            ""Name"": ""Test Item Line"",
            ""Description"": ""This is a test item line""
        }";

        // Act
        var itemLine = JsonConvert.DeserializeObject<ItemLine>(json);

        // Assert
        Assert.NotNull(itemLine);
        Assert.Null(itemLine.Id);
        Assert.Null(itemLine.Created_At);
        Assert.Null(itemLine.Updated_At);
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
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<ItemLine>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1
        }";

        // Act
        var itemLine = JsonConvert.DeserializeObject<ItemLine>(json);

        // Assert
        Assert.NotNull(itemLine);
        Assert.Equal(1, itemLine.Id);
        Assert.Null(itemLine.Name);
        Assert.Null(itemLine.Description);
    }
}

