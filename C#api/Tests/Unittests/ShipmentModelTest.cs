using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;
using Models;

public class ShipmentModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsShipmentObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Order_Id"": 100,
            ""Source_Id"": 200,
            ""Order_Date"": ""2023-01-01"",
            ""Request_Date"": ""2023-01-10"",
            ""Shipment_Date"": ""2023-01-15"",
            ""Shipment_Type"": ""Express"",
            ""Shipment_Status"": ""In Transit"",
            ""Notes"": ""Handle with care"",
            ""Carrier_Code"": ""CAR001"",
            ""Carrier_Description"": ""Test Carrier"",
            ""Service_Code"": ""SVC001"",
            ""Payment_Type"": ""Prepaid"",
            ""Transfer_Mode"": ""Air"",
            ""Total_Package_Count"": 5,
            ""Total_Package_Weight"": 25.5,
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z"",
            ""Items"": [
                {
                    ""Item_Id"": ""ITEM001"",
                    ""Amount"": 3
                },
                {
                    ""Item_Id"": ""ITEM002"",
                    ""Amount"": 2
                }
            ]
        }";

        // Act
        var shipment = JsonConvert.DeserializeObject<Shipment>(json);

        // Assert
        Assert.NotNull(shipment);
        Assert.Equal(1, shipment.Id);
        Assert.Equal(100, shipment.Order_Id);
        Assert.Equal(200, shipment.Source_Id);
        Assert.Equal("2023-01-01", shipment.Order_Date);
        Assert.Equal("2023-01-10", shipment.Request_Date);
        Assert.Equal("2023-01-15", shipment.Shipment_Date);
        Assert.Equal("Express", shipment.Shipment_Type);
        Assert.Equal("In Transit", shipment.Shipment_Status);
        Assert.Equal("Handle with care", shipment.Notes);
        Assert.Equal("CAR001", shipment.Carrier_Code);
        Assert.Equal("Test Carrier", shipment.Carrier_Description);
        Assert.Equal("SVC001", shipment.Service_Code);
        Assert.Equal("Prepaid", shipment.Payment_Type);
        Assert.Equal("Air", shipment.Transfer_Mode);
        Assert.Equal(5, shipment.Total_Package_Count);
        Assert.Equal(25.5, shipment.Total_Package_Weight);
        Assert.Equal("2023-01-01T00:00:00Z", shipment.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", shipment.Updated_At);
        Assert.NotNull(shipment.Items);
        Assert.Equal(2, shipment.Items.Count);
        Assert.Equal("ITEM001", shipment.Items[0].Item_Id);
        Assert.Equal(3, shipment.Items[0].Amount);
        Assert.Equal("ITEM002", shipment.Items[1].Item_Id);
        Assert.Equal(2, shipment.Items[1].Amount);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsShipmentObject()
    {
        // Arrange
        string json = @"{
            ""Order_Id"": 100,
            ""Source_Id"": 200,
            ""Order_Date"": ""2023-01-01"",
            ""Request_Date"": ""2023-01-10"",
            ""Shipment_Date"": ""2023-01-15"",
            ""Shipment_Type"": ""Express"",
            ""Total_Package_Count"": 5,
            ""Total_Package_Weight"": 25.5,
            ""Items"": []
        }";

        // Act
        var shipment = JsonConvert.DeserializeObject<Shipment>(json);

        // Assert
        Assert.NotNull(shipment);
        Assert.Null(shipment.Id);
        Assert.Null(shipment.Shipment_Status);
        Assert.Null(shipment.Notes);
        Assert.Null(shipment.Carrier_Code);
        Assert.Null(shipment.Carrier_Description);
        Assert.Null(shipment.Service_Code);
        Assert.Null(shipment.Payment_Type);
        Assert.Null(shipment.Transfer_Mode);
        Assert.Null(shipment.Created_At);
        Assert.Null(shipment.Updated_At);
        Assert.NotNull(shipment.Items);
        Assert.Empty(shipment.Items);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Id"": ""Not a number"",
            ""Order_Id"": ""Also not a number"",
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Shipment>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Order_Id"": 100
        }";

        // Act
        var shipment = JsonConvert.DeserializeObject<Shipment>(json);

        // Assert
        Assert.NotNull(shipment);
        Assert.Equal(1, shipment.Id);
        Assert.Equal(100, shipment.Order_Id);
        Assert.Equal(0, shipment.Source_Id);
        Assert.Null(shipment.Order_Date);
        Assert.Null(shipment.Request_Date);
        Assert.Null(shipment.Shipment_Date);
        Assert.Null(shipment.Shipment_Type);
        Assert.Equal(0, shipment.Total_Package_Count);
        Assert.Equal(0, shipment.Total_Package_Weight);
        Assert.Null(shipment.Items);
    }
}

