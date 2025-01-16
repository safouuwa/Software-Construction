using System;
using Newtonsoft.Json;
using Xunit;
using Models;

public class ItemModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsItemObject()
    {
        // Arrange
        string json = @"{
            ""Uid"": ""ITEM001"",
            ""Code"": ""CODE001"",
            ""Description"": ""Test Item"",
            ""Short_Description"": ""Test"",
            ""Upc_Code"": ""123456789012"",
            ""Model_Number"": ""MODEL001"",
            ""Commodity_Code"": ""COMM001"",
            ""Item_Line"": 1,
            ""Item_Group"": 2,
            ""Item_Type"": 3,
            ""Unit_Purchase_Quantity"": 10,
            ""Unit_Order_Quantity"": 5,
            ""Pack_Order_Quantity"": 50,
            ""Supplier_Id"": 100,
            ""Supplier_Code"": ""SUP001"",
            ""Supplier_Part_Number"": ""PART001"",
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var item = JsonConvert.DeserializeObject<Item>(json);

        // Assert
        Assert.NotNull(item);
        Assert.Equal("ITEM001", item.Uid);
        Assert.Equal("CODE001", item.Code);
        Assert.Equal("Test Item", item.Description);
        Assert.Equal("Test", item.Short_Description);
        Assert.Equal("123456789012", item.Upc_Code);
        Assert.Equal("MODEL001", item.Model_Number);
        Assert.Equal("COMM001", item.Commodity_Code);
        Assert.Equal(1, item.Item_Line);
        Assert.Equal(2, item.Item_Group);
        Assert.Equal(3, item.Item_Type);
        Assert.Equal(10, item.Unit_Purchase_Quantity);
        Assert.Equal(5, item.Unit_Order_Quantity);
        Assert.Equal(50, item.Pack_Order_Quantity);
        Assert.Equal(100, item.Supplier_Id);
        Assert.Equal("SUP001", item.Supplier_Code);
        Assert.Equal("PART001", item.Supplier_Part_Number);
        Assert.Equal("2023-01-01T00:00:00Z", item.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", item.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsItemObject()
    {
        // Arrange
        string json = @"{
            ""Code"": ""CODE001"",
            ""Description"": ""Test Item"",
            ""Item_Line"": 1,
            ""Item_Group"": 2,
            ""Item_Type"": 3,
            ""Supplier_Id"": 100
        }";

        // Act
        var item = JsonConvert.DeserializeObject<Item>(json);

        // Assert
        Assert.NotNull(item);
        Assert.Null(item.Uid);
        Assert.Null(item.Short_Description);
        Assert.Null(item.Upc_Code);
        Assert.Null(item.Model_Number);
        Assert.Null(item.Commodity_Code);
        Assert.Equal(0, item.Unit_Purchase_Quantity);
        Assert.Equal(0, item.Unit_Order_Quantity);
        Assert.Equal(0, item.Pack_Order_Quantity);
        Assert.Null(item.Supplier_Code);
        Assert.Null(item.Supplier_Part_Number);
        Assert.Null(item.Created_At);
        Assert.Null(item.Updated_At);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Uid"": 12345,
            ""Item_Line"": ""Not a number"",
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Item>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Uid"": ""ITEM001"",
            ""Code"": ""CODE001""
        }";

        // Act
        var item = JsonConvert.DeserializeObject<Item>(json);

        // Assert
        Assert.NotNull(item);
        Assert.Equal("ITEM001", item.Uid);
        Assert.Equal("CODE001", item.Code);
        Assert.Null(item.Description);
        Assert.Equal(0, item.Item_Line);
        Assert.Equal(0, item.Item_Group);
        Assert.Equal(0, item.Item_Type);
        Assert.Equal(0, item.Supplier_Id);
    }
}

