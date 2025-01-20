using System;
using Newtonsoft.Json;
using Xunit;
using ModelsV2;

public class ItemTypeModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsItemTypeObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Name"": ""Test Item Type"",
            ""Description"": ""This is a test item type"",
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var itemType = JsonConvert.DeserializeObject<ItemType>(json);

        // Assert
        Assert.NotNull(itemType);
        Assert.Equal(1, itemType.Id);
        Assert.Equal("Test Item Type", itemType.Name);
        Assert.Equal("This is a test item type", itemType.Description);
        Assert.Equal("2023-01-01T00:00:00Z", itemType.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", itemType.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsItemTypeObject()
    {
        // Arrange
        string json = @"{
            ""Name"": ""Test Item Type"",
            ""Description"": ""This is a test item type""
        }";

        // Act
        var itemType = JsonConvert.DeserializeObject<ItemType>(json);

        // Assert
        Assert.NotNull(itemType);
        Assert.Null(itemType.Id);
        Assert.Null(itemType.Created_At);
        Assert.Null(itemType.Updated_At);
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
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<ItemType>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1
        }";

        // Act
        var itemType = JsonConvert.DeserializeObject<ItemType>(json);

        // Assert
        Assert.NotNull(itemType);
        Assert.Equal(1, itemType.Id);
        Assert.Null(itemType.Name);
        Assert.Null(itemType.Description);
    }
}

