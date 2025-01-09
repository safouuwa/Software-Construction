using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;
using Models;

public class OrderModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsOrderObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Source_Id"": 100,
            ""Order_Date"": ""2023-01-01"",
            ""Request_Date"": ""2023-01-10"",
            ""Reference"": ""ORD001"",
            ""Reference_Extra"": ""Extra info"",
            ""Order_Status"": ""Pending"",
            ""Notes"": ""Test order"",
            ""Shipping_Notes"": ""Handle with care"",
            ""Picking_Notes"": ""Pick from aisle 5"",
            ""Warehouse_Id"": 1,
            ""Ship_To"": 1000,
            ""Bill_To"": 1001,
            ""Shipment_Id"": 500,
            ""Total_Amount"": 1000.50,
            ""Total_Discount"": 50.25,
            ""Total_Tax"": 75.50,
            ""Total_Surcharge"": 10.00,
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z"",
            ""Items"": [
                {
                    ""Item_Id"": ""ITEM001"",
                    ""Amount"": 5
                },
                {
                    ""Item_Id"": ""ITEM002"",
                    ""Amount"": 3
                }
            ]
        }";

        // Act
        var order = JsonConvert.DeserializeObject<Order>(json);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(1, order.Id);
        Assert.Equal(100, order.Source_Id);
        Assert.Equal("2023-01-01", order.Order_Date);
        Assert.Equal("2023-01-10", order.Request_Date);
        Assert.Equal("ORD001", order.Reference);
        Assert.Equal("Extra info", order.Reference_Extra);
        Assert.Equal("Pending", order.Order_Status);
        Assert.Equal("Test order", order.Notes);
        Assert.Equal("Handle with care", order.Shipping_Notes);
        Assert.Equal("Pick from aisle 5", order.Picking_Notes);
        Assert.Equal(1, order.Warehouse_Id);
        Assert.Equal(1000, order.Ship_To);
        Assert.Equal(1001, order.Bill_To);
        Assert.Equal(500, order.Shipment_Id);
        Assert.Equal(1000.50m, order.Total_Amount);
        Assert.Equal(50.25m, order.Total_Discount);
        Assert.Equal(75.50m, order.Total_Tax);
        Assert.Equal(10.00m, order.Total_Surcharge);
        Assert.Equal("2023-01-01T00:00:00Z", order.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", order.Updated_At);
        Assert.NotNull(order.Items);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal("ITEM001", order.Items[0].Item_Id);
        Assert.Equal(5, order.Items[0].Amount);
        Assert.Equal("ITEM002", order.Items[1].Item_Id);
        Assert.Equal(3, order.Items[1].Amount);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsOrderObject()
    {
        // Arrange
        string json = @"{
            ""Source_Id"": 100,
            ""Order_Date"": ""2023-01-01"",
            ""Request_Date"": ""2023-01-10"",
            ""Reference"": ""ORD001"",
            ""Warehouse_Id"": 1,
            ""Total_Amount"": 1000.50,
            ""Items"": []
        }";

        // Act
        var order = JsonConvert.DeserializeObject<Order>(json);

        // Assert
        Assert.NotNull(order);
        Assert.Null(order.Id);
        Assert.Null(order.Order_Status);
        Assert.Null(order.Ship_To);
        Assert.Null(order.Bill_To);
        Assert.Null(order.Shipment_Id);
        Assert.Null(order.Created_At);
        Assert.Null(order.Updated_At);
        Assert.NotNull(order.Items);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Id"": ""Not a number"",
            ""Source_Id"": ""Also not a number"",
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Order>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Source_Id"": 100
        }";

        // Act
        var order = JsonConvert.DeserializeObject<Order>(json);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(1, order.Id);
        Assert.Equal(100, order.Source_Id);
        Assert.Null(order.Order_Date);
        Assert.Null(order.Request_Date);
        Assert.Null(order.Reference);
        Assert.Equal(0, order.Warehouse_Id);
        Assert.Equal(0, order.Total_Amount);
        Assert.Null(order.Items);
    }
}

