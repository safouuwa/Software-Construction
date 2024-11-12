import httpx
import unittest
import datetime
import json

class ApiPostTests(unittest.TestCase):

    @classmethod
    def setUpClass(self):
        self.client = httpx.Client(base_url="http://127.0.0.1:3000/api/v1/")
        self.client.headers["API_KEY"] = "a1b2c3d4e5"

    def test_create_client(self):
        new_client = {
            "Id": 0,
            "Name": "New Client",
            "Address": "123 Main St",
            "City": "Anytown",
            "Zip_code": "12345",
            "Province": "Province",
            "Country": "Country",
            "Contact_name": "John Doe",
            "Contact_phone": "123-456-7890",
            "Contact_email": "johndoe@example.com"
        }

        response = self.client.post("clients", json=new_client)
        self.assertEqual(response.status_code, 201)

    def test_create_client_with_invalid_data(self):
        invalid_client = {
            "Name": "",  # Invalid because it has no ID
            "Address": "123 Main St",
            "City": "Anytown",
            "Zip_code": "12345",
            "Province": "Province",
            "Country": "Country",
            "Contact_name": "John Doe",
            "Contact_phone": "123-456-7890",
            "Contact_email": "johndoe@example.com"
        }

        response = self.client.post("clients", json=invalid_client)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_client(self):
        duplicate_client = {
            "Id": 1,  # Assume this ID already exists
            "Name": "Duplicate Client",
            "Address": "123 Main St",
            "City": "Anytown",
            "Zip_code": "12345",
            "Province": "Province",
            "Country": "Country",
            "Contact_name": "Jane Doe",
            "Contact_phone": "987-654-3210",
            "Contact_email": "janedoe@example.com"
        }

        response = self.client.post("clients", json=duplicate_client)
        self.assertEqual(response.status_code, 404)

    def test_create_shipment(self):
        new_shipment = {
            "Id": 0,
            "Order_Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "Standard",
            "Shipment_Status": "Pending",
            "Notes": "Handle with care",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 3,
            "Total_Package_Weight": 10.5,
            "Items": [
                {"Item_Id": "item1", "Amount": 2},
                {"Item_Id": "item2", "Amount": 1}
            ]
        }

        response = self.client.post("shipments", json=new_shipment)
        self.assertEqual(response.status_code, 201)

    def test_create_shipment_with_invalid_data(self):
        invalid_shipment = {
            "Order_Id": 1,  # Invalid because there is no Id
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "",
            "Shipment_Status": "Pending",
            "Notes": "Handle with care",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 3,
            "Total_Package_Weight": 10.5,
            "Items": [{"Item_Id": "item1", "Amount": 2}]
        }

        response = self.client.post("shipments", json=invalid_shipment)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_shipment(self):
        duplicate_shipment = {
            "Id": 1,  # Assume this ID already exists
            "Order_Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "Standard",
            "Shipment_Status": "Pending",
            "Notes": "Handle with care",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 3,
            "Total_Package_Weight": 10.5,
            "Items": [{"Item_Id": "item1", "Amount": 2}]
        }

        response = self.client.post("shipments", json=duplicate_shipment)
        self.assertEqual(response.status_code, 404)

    # Item Groups
    def test_create_item_group(self):
        new_item_group = {
            "Id": 300,
            "Name": "New Item Group",
            "Description": "Description of new group"
        }
        
        response = self.client.post("item_groups", json=new_item_group)
        self.assertEqual(response.status_code, 201)

    def test_create_item_group_with_invalid_data(self):
        invalid_item_group = {
            "Name": "",  # Invalid because there is no Id
            "Description": "Description of invalid group"
        }

        response = self.client.post("item_groups", json=invalid_item_group)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_item_group(self):
        duplicate_item_group = {
            "Id": 1,  # Assume this ID already exists
            "Name": "Duplicate Item Group",
            "Description": "Description of duplicate group"
        }

        response = self.client.post("item_groups", json=duplicate_item_group)
        self.assertEqual(response.status_code, 404)

    # Suppliers
    def test_create_supplier(self):
        new_supplier = {
            "Id": 0,
            "Code": "SUP0001",
            "Name": "test",
            "Address": "5989 Sullivan Drives",
            "Address_Extra": "Apt. 996",
            "City": "Port Anitaburgh",
            "Zip_Code": "91688",
            "Province": "Illinois",
            "Country": "Czech Republic",
            "Contact_Name": "Toni Barnett",
            "Phonenumber": "363.541.7282x36825",
            "Reference": "LPaJ-SUP0001"
        }

        response = self.client.post("suppliers", json=new_supplier)
        self.assertEqual(response.status_code, 201)

    def test_create_supplier_with_invalid_data(self):
        invalid_supplier = {
            "Code": "SUP0001",
            "Name": "",
            "Address": "5989 Sullivan Drives",
            "Address_Extra": "Apt. 996",
            "City": "Port Anitaburgh",
            "Zip_Code": "91688",
            "Province": "Illinois",
            "Country": "Czech Republic",
            "Contact_Name": "Toni Barnett",
            "Phonenumber": "363.541.7282x36825",
            "Reference": "LPaJ-SUP0001"
        }

        response = self.client.post("suppliers", json=invalid_supplier)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_supplier(self):
        duplicate_supplier = {
            "Id": 1,  # Assume this ID already exists
            "Code": "SUP0001",
            "Name": "Existing Supplier",
            "Address": "5989 Sullivan Drives",
            "Address_Extra": "Apt. 996",
            "City": "Port Anitaburgh",
            "Zip_Code": "91688",
            "Province": "Illinois",
            "Country": "Czech Republic",
            "Contact_Name": "Toni Barnett",
            "Phonenumber": "363.541.7282x36825",
            "Reference": "LPaJ-SUP0001"
        }

        response = self.client.post("suppliers", json=duplicate_supplier)
        self.assertEqual(response.status_code, 404)

    # Transfers
    def test_create_transfer(self):
        new_transfer = {
            "Id": 0,
            "Reference": "TEST00001",
            "Transfer_From": None,
            "Transfer_To": 9229,
            "Transfer_Status": "Completed",
            "Created_At": "2000-03-11T13:11:14Z",
            "Updated_At": "2000-03-12T16:11:14Z",
            "Items": [
                {"Item_Id": "P007435", "Amount": 23}
            ]
        }

        response = self.client.post("transfers", json=new_transfer)
        self.assertEqual(response.status_code, 201)

    def test_create_transfer_with_invalid_data(self):
        invalid_transfer = {
            "Reference": "",  # Invalid there is no Id
            "Transfer_From": None,
            "Transfer_To": 9229,
            "Transfer_Status": "Completed",
            "Created_At": "2000-03-11T13:11:14Z",
            "Updated_At": "2000-03-12T16:11:14Z",
            "Items": [
                {"Item_Id": "P007435", "Amount": 23}
            ]
        }

        response = self.client.post("transfers", json=invalid_transfer)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_transfer(self):
        duplicate_transfer = {
            "Id": 1,
            "Reference": "TEST00001",
            "Transfer_From": None,
            "Transfer_To": 9229,
            "Transfer_Status": "Completed",
            "Created_At": "2000-03-11T13:11:14Z",
            "Updated_At": "2000-03-12T16:11:14Z",
            "Items": [
                {"Item_Id": "P007435", "Amount": 23}
            ]
        }

        response = self.client.post("transfers", json=duplicate_transfer)
        self.assertEqual(response.status_code, 404)

    # Warehouses
    def test_create_warehouse(self):
        new_warehouse = {
            "Id": 0,
            "Code": "YQZZNL56",
            "Name": "TEST cargo hub",
            "Address": "Karlijndreef 281",
            "Zip": "4002 AS",
            "City": "Heemskerk",
            "Province": "Friesland",
            "Country": "NL",
            "Contact": {
                "Name": "Fem Keijzer",
                "Phone": "(078) 0013363",
                "Email": "blamore@example.net"
            }
        }

        response = self.client.post("warehouses", json=new_warehouse)
        self.assertEqual(response.status_code, 201)

    def test_create_warehouse_with_invalid_data(self):
        invalid_warehouse = {
            "Code": "",
            "Name": "",
            "Address": "Karlijndreef 281",
            "Zip": "4002 AS",
            "City": "Heemskerk",
            "Province": "Friesland",
            "Country": "NL",
            "Contact": {
                "Name": "Fem Keijzer",
                "Phone": "(078) 0013363",
                "Email": "blamore@example.net"
            }
        }

        response = self.client.post("warehouses", json=invalid_warehouse)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_warehouse(self):
        duplicate_warehouse = {
            "Id": 1,  # Assume this ID already exists
            "Code": "WH001",
            "Name": "Main Warehouse",
            "Address": "Karlijndreef 281",
            "Zip": "4002 AS",
            "City": "Heemskerk",
            "Province": "Friesland",
            "Country": "NL",
            "Contact": {
                "Name": "Fem Keijzer",
                "Phone": "(078) 0013363",
                "Email": "blamore@example.net"
            }
        }

        response = self.client.post("warehouses", json=duplicate_warehouse)
        self.assertEqual(response.status_code, 404)

    # Item Types
    def test_create_item_type(self):
        new_item_type = {
            "Id": 1267835,
            "Name": "New Item Type",
            "Description": "Description of new item type",
        }

        response = self.client.post("item_types", json=new_item_type)
        self.assertEqual(response.status_code, 201)

    def test_create_item_type_with_invalid_data(self):
        invalid_item_type = {
            "Name": "",  # Invalid because there is no Id
            "Description": "Description of invalid item type",
        }

        response = self.client.post("item_types", json=invalid_item_type)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_item_type(self):
        duplicate_item_type = {
            "Id": 1,  # Assume this ID already exists
            "Name": "Duplicate Item Type",
            "Description": "Description of duplicate item type",
        }

        response = self.client.post("item_types", json=duplicate_item_type)
        self.assertEqual(response.status_code, 404)

    # Items
    def test_create_item(self):
        new_item = {
            "Uid": "testitemW",
            "Code": "ITEM001",
            "Description": "New Item Description",
            "Short_Description": "Short description of new item",
            "Upc_Code": "012345678901",
            "Model_Number": "ModelX",
            "Commodity_Code": "Commodity123",
            "Item_Line": 1,
            "Item_Group": 1,
            "Item_Type": 1,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 15,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP001",
            "Supplier_Part_Number": "PART001"
        }

        response = self.client.post("items", json=new_item)
        self.assertEqual(response.status_code, 201)

    def test_create_item_with_invalid_data(self):
        invalid_item = {
            "Code": "ITEM001",
            "Description": "New Item Description",
            "Short_Description": "Short description of new item",
            "Upc_Code": "012345678901",
            "Model_Number": "ModelX",
            "Commodity_Code": "Commodity123",
            "Item_Line": 1,
            "Item_Group": 1,
            "Item_Type": 1,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 15,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP001",
            "Supplier_Part_Number": "PART001"
        }

        response = self.client.post("items", json=invalid_item)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_item(self):
        duplicate_item = {
            "Uid": "P000001",  # Assume this ID already exists
            "Code": "ITEM001",
            "Description": "New Item Description",
            "Short_Description": "Short description of new item",
            "Upc_Code": "012345678901",
            "Model_Number": "ModelX",
            "Commodity_Code": "Commodity123",
            "Item_Line": 1,
            "Item_Group": 1,
            "Item_Type": 1,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 15,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP001",
            "Supplier_Part_Number": "PART001"
        }

        response = self.client.post("items", json=duplicate_item)
        self.assertEqual(response.status_code, 404)

    # Orders
    def test_create_order(self):
        new_order = {
            "Id" : 34567,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Reference": "Ref001",
            "Reference_Extra": "ExtraRef",
            "Order_Status": "Pending",
            "Notes": "New order notes",
            "Shipping_Notes": "Ship quickly",
            "Picking_Notes": "Pick carefully",
            "Warehouse_Id": 1,
            "Ship_To": 1,
            "Bill_To": 2,
            "Shipment_Id": None,
            "Total_Amount": 150.75,
            "Total_Discount": 10.00,
            "Total_Tax": 5.00,
            "Total_Surcharge": 0.00,
            "Items": [
                {"Item_Id": "item1", "Amount": 2},
                {"Item_Id": "item2", "Amount": 1}
            ]
        }

        response = self.client.post("orders", json=new_order)
        self.assertEqual(response.status_code, 201)

    def test_create_order_with_invalid_data(self):
        invalid_order = {
            "Source_Id": 1,
            "Order_Date": "",
            "Request_Date": "2024-10-19",
            "Reference": "",
            "Reference_Extra": "",
            "Order_Status": "",
            "Notes": "order notes",
            "Shipping_Notes": "Ship slower",
            "Picking_Notes": "Pick fast",
            "Warehouse_Id": -1,
            "Ship_To": None,
            "Bill_To": None,
            "Shipment_Id": None,
            "Total_Amount": -150.75,
            "Total_Discount": -10.00,
            "Total_Tax": -5.00,
            "Total_Surcharge": -3.00,
            "Created_At": "",  # Invalid date
            "Updated_At": ""   # Invalid date
        }

        response = self.client.post("orders", json=invalid_order)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_order(self):
        duplicate_order = {
            "Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Reference": "Ref001",
            "Reference_Extra": "ExtraRef",
            "Order_Status": "Pending",
            "Notes": "New order notes",
            "Shipping_Notes": "Ship quickly",
            "Picking_Notes": "Pick carefully",
            "Warehouse_Id": 1,
            "Ship_To": 1,
            "Bill_To": 2,
            "Shipment_Id": None,
            "Total_Amount": 150.75,
            "Total_Discount": 10.00,
            "Total_Tax": 5.00,
            "Total_Surcharge": 0.00,
            "Items": [
                {"Item_Id": "item1", "Amount": 2},
                {"Item_Id": "item2", "Amount": 1}
            ]
        }

        response = self.client.post("orders", json=duplicate_order)
        self.assertEqual(response.status_code, 404)

    # Inventory
    def test_create_inventory(self):
        new_inventory = {
            "Id": 34567, 
            "Warehouse_Id": 1,
            "Item_Id": "item-001",
            "Quantity": 100
        }

        response = self.client.post("inventories", json=new_inventory)
        self.assertEqual(response.status_code, 201)

    def test_create_inventory_with_invalid_data(self):
        invalid_inventory = {
            "Warehouse_Id": 1,
            "Item_Id": "item-001",
            "Quantity": -10  # Invalid negative quantity
        }

        response = self.client.post("inventories", json=invalid_inventory)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_inventory(self):
        duplicate_inventory = {
            "Id": 1,
            "Warehouse_Id": 1,
            "Item_Id": "item-001",  # Assume this already exists
            "Quantity": 100
        }

        response = self.client.post("inventories", json=duplicate_inventory)
        self.assertEqual(response.status_code, 404)

    # Item Lines
    def test_create_item_line(self):
        new_item_line = {"id": 1234567897, "name": "Test ItemLine", "description": "Description of the test item line"}

        response = self.client.post("item_lines", json=new_item_line)
        self.assertEqual(response.status_code, 201)

    def test_create_item_line_with_invalid_data(self):
        invalid_item_line = {"name": "Test ItemLine", "description": "Description of the test item line"}

        response = self.client.post("item_lines", json=invalid_item_line)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_item_line(self):
        duplicate_item_line = {"id": 1, "name": "Test ItemLine", "description": "Description of the test item line"}

        response = self.client.post("item_lines", json=duplicate_item_line)
        self.assertEqual(response.status_code, 404)

    # Locations
    def test_create_location(self):
        new_location = {"id": 198765, "warehouse_id": 1, "code": "A.1.0", "name": "Row: A, Rack: 1, Shelf: 0", "created_at": "1992-05-15 03:21:32", "updated_at": "1992-05-15 03:21:32"}

        response = self.client.post("locations", json=new_location)
        self.assertEqual(response.status_code, 201)

    def test_create_location_with_invalid_data(self):
        invalid_location = {"warehouse_id": 1, "code": "A.1.0", "name": "Row: A, Rack: 1, Shelf: 0", "created_at": "1992-05-15 03:21:32", "updated_at": "1992-05-15 03:21:32"}

        response = self.client.post("locations", json=invalid_location)
        self.assertEqual(response.status_code, 400)

    def test_attempt_to_create_duplicate_location(self):
        duplicate_location = {"id": 1, "warehouse_id": 1, "code": "A.1.0", "name": "Row: A, Rack: 1, Shelf: 0", "created_at": "1992-05-15 03:21:32", "updated_at": "1992-05-15 03:21:32"}

        response = self.client.post("locations", json=duplicate_location)
        self.assertEqual(response.status_code, 404)

if __name__ == "__main__":
    unittest.main()
