using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;
using Models;

public class TransferModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsTransferObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Reference"": ""TRF001"",
            ""Transfer_From"": 100,
            ""Transfer_To"": 200,
            ""Transfer_Status"": ""In Progress"",
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
        var transfer = JsonConvert.DeserializeObject<Transfer>(json);

        // Assert
        Assert.NotNull(transfer);
        Assert.Equal(1, transfer.Id);
        Assert.Equal("TRF001", transfer.Reference);
        Assert.Equal(100, transfer.Transfer_From);
        Assert.Equal(200, transfer.Transfer_To);
        Assert.Equal("In Progress", transfer.Transfer_Status);
        Assert.Equal("2023-01-01T00:00:00Z", transfer.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", transfer.Updated_At);
        Assert.NotNull(transfer.Items);
        Assert.Equal(2, transfer.Items.Count);
        Assert.Equal("ITEM001", transfer.Items[0].Item_Id);
        Assert.Equal(5, transfer.Items[0].Amount);
        Assert.Equal("ITEM002", transfer.Items[1].Item_Id);
        Assert.Equal(3, transfer.Items[1].Amount);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsTransferObject()
    {
        // Arrange
        string json = @"{
            ""Reference"": ""TRF001"",
            ""Transfer_From"": 100,
            ""Transfer_To"": 200,
            ""Transfer_Status"": ""In Progress"",
            ""Items"": []
        }";

        // Act
        var transfer = JsonConvert.DeserializeObject<Transfer>(json);

        // Assert
        Assert.NotNull(transfer);
        Assert.Null(transfer.Id);
        Assert.Null(transfer.Created_At);
        Assert.Null(transfer.Updated_At);
        Assert.NotNull(transfer.Items);
        Assert.Empty(transfer.Items);
    }

    [Fact]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = @"{
            ""Id"": ""Not a number"",
            ""Transfer_From"": ""Also not a number"",
            ""Invalid_Field"": ""This field doesn't exist""
        }";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Transfer>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Reference"": ""TRF001""
        }";

        // Act
        var transfer = JsonConvert.DeserializeObject<Transfer>(json);

        // Assert
        Assert.NotNull(transfer);
        Assert.Equal(1, transfer.Id);
        Assert.Equal("TRF001", transfer.Reference);
        Assert.Null(transfer.Transfer_From);
        Assert.Null(transfer.Transfer_To);
        Assert.Null(transfer.Transfer_Status);
        Assert.Null(transfer.Items);
    }
}

