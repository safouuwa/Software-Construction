using System;
using Newtonsoft.Json;
using Xunit;
using ModelsV2;

public class ItemGroupModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsItemGroupObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Name"": ""Test Item Group"",
            ""Description"": ""This is a test item group"",
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var itemGroup = JsonConvert.DeserializeObject<ItemGroup>(json);

        // Assert
        Assert.NotNull(itemGroup);
        Assert.Equal(1, itemGroup.Id);
        Assert.Equal("Test Item Group", itemGroup.Name);
        Assert.Equal("This is a test item group", itemGroup.Description);
        Assert.Equal("2023-01-01T00:00:00Z", itemGroup.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", itemGroup.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsItemGroupObject()
    {
        // Arrange
        string json = @"{
            ""Name"": ""Test Item Group"",
            ""Description"": ""This is a test item group""
        }";

        // Act
        var itemGroup = JsonConvert.DeserializeObject<ItemGroup>(json);

        // Assert
        Assert.NotNull(itemGroup);
        Assert.Null(itemGroup.Id);
        Assert.Null(itemGroup.Created_At);
        Assert.Null(itemGroup.Updated_At);
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
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<ItemGroup>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1
        }";

        // Act
        var itemGroup = JsonConvert.DeserializeObject<ItemGroup>(json);

        // Assert
        Assert.NotNull(itemGroup);
        Assert.Equal(1, itemGroup.Id);
        Assert.Null(itemGroup.Name);
        Assert.Null(itemGroup.Description);
    }
}

