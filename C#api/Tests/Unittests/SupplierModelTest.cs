using System;
using Newtonsoft.Json;
using Xunit;
using ModelsV2;

public class SupplierModelTests
{
    [Fact]
    public void Deserialize_ValidJson_ReturnsSupplierObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Code"": ""SUP001"",
            ""Name"": ""Test Supplier"",
            ""Address"": ""123 Test St"",
            ""Address_Extra"": ""Suite 100"",
            ""City"": ""Test City"",
            ""Zip_Code"": ""12345"",
            ""Province"": ""Test Province"",
            ""Country"": ""Test Country"",
            ""Contact_Name"": ""John Doe"",
            ""Phonenumber"": ""123-456-7890"",
            ""Reference"": ""REF001"",
            ""Created_At"": ""2023-01-01T00:00:00Z"",
            ""Updated_At"": ""2023-01-02T00:00:00Z""
        }";

        // Act
        var supplier = JsonConvert.DeserializeObject<Supplier>(json);

        // Assert
        Assert.NotNull(supplier);
        Assert.Equal(1, supplier.Id);
        Assert.Equal("SUP001", supplier.Code);
        Assert.Equal("Test Supplier", supplier.Name);
        Assert.Equal("123 Test St", supplier.Address);
        Assert.Equal("Suite 100", supplier.Address_Extra);
        Assert.Equal("Test City", supplier.City);
        Assert.Equal("12345", supplier.Zip_Code);
        Assert.Equal("Test Province", supplier.Province);
        Assert.Equal("Test Country", supplier.Country);
        Assert.Equal("John Doe", supplier.Contact_Name);
        Assert.Equal("123-456-7890", supplier.Phonenumber);
        Assert.Equal("REF001", supplier.Reference);
        Assert.Equal("2023-01-01T00:00:00Z", supplier.Created_At);
        Assert.Equal("2023-01-02T00:00:00Z", supplier.Updated_At);
    }

    [Fact]
    public void Deserialize_MissingOptionalFields_ReturnsSupplierObject()
    {
        // Arrange
        string json = @"{
            ""Code"": ""SUP001"",
            ""Name"": ""Test Supplier"",
            ""Address"": ""123 Test St"",
            ""City"": ""Test City"",
            ""Zip_Code"": ""12345"",
            ""Country"": ""Test Country"",
            ""Contact_Name"": ""John Doe"",
            ""Phonenumber"": ""123-456-7890""
        }";

        // Act
        var supplier = JsonConvert.DeserializeObject<Supplier>(json);

        // Assert
        Assert.NotNull(supplier);
        Assert.Null(supplier.Id);
        Assert.Null(supplier.Address_Extra);
        Assert.Null(supplier.Province);
        Assert.Null(supplier.Reference);
        Assert.Null(supplier.Created_At);
        Assert.Null(supplier.Updated_At);
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
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Supplier>(json));
    }

    [Fact]
    public void Deserialize_MissingRequiredFields_ReturnsIncompleteObject()
    {
        // Arrange
        string json = @"{
            ""Id"": 1,
            ""Code"": ""SUP001""
        }";

        // Act
        var supplier = JsonConvert.DeserializeObject<Supplier>(json);

        // Assert
        Assert.NotNull(supplier);
        Assert.Equal(1, supplier.Id);
        Assert.Equal("SUP001", supplier.Code);
        Assert.Null(supplier.Name);
        Assert.Null(supplier.Address);
        Assert.Null(supplier.City);
        Assert.Null(supplier.Zip_Code);
        Assert.Null(supplier.Country);
        Assert.Null(supplier.Contact_Name);
        Assert.Null(supplier.Phonenumber);
    }
}

