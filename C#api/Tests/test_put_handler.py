import httpx
import unittest
import json

class ApiPutTests(unittest.TestCase):
    @classmethod
    def setUpClass(self):
        self.client = httpx.Client(base_url="http://127.0.0.1:3000/api/v1/")
        self.client.headers["API_KEY"] = "a1b2c3d4e5"

    # Clients
    def test_update_existing_client(self):
        updated_client = json.dumps({
            "Id": 1,
            "Name": "Updated Client",
            "Address": "456 Updated St",
            "City": "Updated City",
            "Zip_code": "54321",
            "Province": "Updated Province",
            "Country": "Updated Country",
            "Contact_name": "Updated Name",
            "Contact_phone": "321-654-0987",
            "Contact_email": "updated@example.com"
        })
        response = self.client.put("clients/1", content=updated_client, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_client(self):
        updated_client = json.dumps({
            "Id": -1,
            "Name": "Non-existent Client",
            "Address": "789 Non-existent St",
            "City": "Nowhere",
            "Zip_code": "00000",
            "Province": "Non-existent Province",
            "Country": "Non-existent Country",
            "Contact_name": "Ghost",
            "Contact_phone": "000-000-0000",
            "Contact_email": "ghost@example.com"
        })
        response = self.client.put("clients/-1", content=updated_client, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)

    def test_update_client_with_invalid_data(self):
        invalid_client = json.dumps({
            # Invalid because there is no id
            "Name": "",
            "Address": "456 Updated St",
            "City": "Updated City",
            "Zip_code": "54321",
            "Province": "Updated Province",
            "Country": "Updated Country",
            "Contact_name": "Updated Name",
            "Contact_phone": "321-654-0987",
            "Contact_email": "updated@example.com"
        })
        response = self.client.put("clients/0", content=invalid_client, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)

    def test_update_client_when_id_in_data_and_id_in_route_differ(self):
        conflicting_client = json.dumps({
            "Id": 2,
            "Name": "Conflicting Client",
            "Address": "456 Conflicting St",
            "City": "Conflict City",
            "Zip_code": "54321",
            "Province": "Conflict Province",
            "Country": "Conflict Country",
            "Contact_name": "Conflict Name",
            "Contact_phone": "321-654-0987",
            "Contact_email": "conflict@example.com"
        })
        response = self.client.put("clients/1", content=conflicting_client, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)

    # Item Groups
    def test_update_existing_item_group(self):
        updated_item_group = json.dumps({
            "Id": 1,
            "Name": "Updated Item Group",
            "Description": "Updated description"
        })
        response = self.client.put("item_groups/1", content=updated_item_group, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_item_group(self):
        updated_item_group = json.dumps({
            "Id": 999,
            "Name": "Non-existent Item Group",
            "Description": "This item group does not exist"
        })
        response = self.client.put("item_groups/999", content=updated_item_group, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)

    def test_update_item_group_with_invalid_data(self):
        invalid_item_group = json.dumps({
             # Invalid because there is no Id
            "Name": "",
            "Description": "Some description"
        })
        response = self.client.put("item_groups/0", content=invalid_item_group, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)

    def test_update_item_group_when_id_in_data_and_id_in_route_differ(self):
        conflicting_item_group = json.dumps({
            "Id": 2,
            "Name": "Conflicting Item Group",
            "Description": "This item group has a conflicting ID"
        })
        response = self.client.put("item_groups/1", content=conflicting_item_group, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)

    # Shipments
    def test_update_existing_shipment(self):
        updated_shipment = json.dumps({
            "Id": 1,
            "Order_Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "Express",
            "Shipment_Status": "Shipped",
            "Notes": "Updated notes",
            "Carrier_Code": "FedEx",
            "Carrier_Description": "Federal Express",
            "Service_Code": "Express",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 2,
            "Total_Package_Weight": 5.0,
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        })
        response = self.client.put("shipments/1", content=updated_shipment, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_shipment(self):
        updated_shipment = json.dumps({
            "Id": -1,
            "Order_Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "Standard",
            "Shipment_Status": "Pending",
            "Notes": "No longer needed",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 3,
            "Total_Package_Weight": 10.0,
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        })
        response = self.client.put("shipments/-1", content=updated_shipment, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)

    def test_update_shipments_with_invalid_data(self):
        invalid_shipment = json.dumps({
              # Invalid because there is no Id
            "Order_Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "",  # Invalid because Shipment_Type is empty
            "Shipment_Status": "Pending",
            "Notes": "Invalid update",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 3,
            "Total_Package_Weight": 10.0,
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        })
        response = self.client.put("shipments/0", content=invalid_shipment, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)

    def test_update_shipment_when_id_in_data_and_id_in_route_differ(self):
        conflicting_shipment = json.dumps({
            "Id": 2,
            "Order_Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Shipment_Date": "2024-10-20",
            "Shipment_Type": "Express",
            "Shipment_Status": "Shipped",
            "Notes": "Conflicting update",
            "Carrier_Code": "FedEx",
            "Carrier_Description": "Federal Express",
            "Service_Code": "Express",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 1,
            "Total_Package_Weight": 2.5,
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        })
        response = self.client.put("shipments/1", content=conflicting_shipment, headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)

    def test_commit_shipment(self):
        response = self.client.put("shipments/1/commit", content=None)
        self.assertEqual(response.status_code, 200)

    def test_commit_non_existent_shipment(self):
        response = self.client.put("shipments/-1/commit", content=None)
        self.assertEqual(response.status_code, 404)

    # Transfers    

    def test_update_existing_transfer(self):
        updated_transfer = {
            "Id": 1,
            "Reference": "Updated Task",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Completed",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20",
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        }
        response = self.client.put(f"transfers/{updated_transfer['Id']}", json=updated_transfer)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_transfer(self):
        updated_transfer = {
            "Id": -1,
            "Reference": "Non-existent Task",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Pending",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20",
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        }
        response = self.client.put(f"transfers/{updated_transfer['Id']}", json=updated_transfer)
        self.assertEqual(response.status_code, 404)

    def test_update_transfer_with_invalid_data(self):
        invalid_transfer = {
            "Reference": "",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Pending",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20",
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        }
        response = self.client.put(f"transfers/0", json=invalid_transfer)
        self.assertEqual(response.status_code, 400)

    def test_update_transfer_when_id_in_data_and_id_in_route_differ(self):
        conflicting_transfer = {
            "Id": 2,
            "Reference": "Conflicting Task",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Pending",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20",
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        }
        response = self.client.put("transfers/1", json=conflicting_transfer)
        self.assertEqual(response.status_code, 404)

    def test_commit_transfer(self):
        commit_transfer = {
            "Id": 1,
            "Reference": "Commit Task",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Pending",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20",
            "Items": [{"Item_Id": "item1", "Amount": 1}]
        }
        response = self.client.put("transfers/1/commit", json=commit_transfer)
        self.assertEqual(response.status_code, 200)

    def test_commit_non_existent_transfer(self):
        response = self.client.put("transfers/-1/commit", json=None)
        self.assertEqual(response.status_code, 404)


    # Region: Supplier
    def test_update_existing_supplier(self):
        updated_supplier = {
            "Id": 1,
            "Name": "Updated Supplier",
            "Address": "456 Updated St",
            "City": "Updated City",
            "Zip_Code": "54321",
            "Province": "Updated Province",
            "Country": "Updated Country",
            "Contact_Name": "Updated Name",
            "Phonenumber": "321-654-0987",
            "Contact_email": "updated@example.com"
        }
        response = self.client.put(f"suppliers/{updated_supplier['Id']}", json=updated_supplier)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_supplier(self):
        updated_supplier = {
            "Id": -1,
            "Name": "Non-existent Supplier",
            "Address": "789 Non-existent St",
            "City": "Nowhere",
            "Zip_Code": "00000",
            "Province": "Non-existent Province",
            "Country": "Non-existent Country",
            "Contact_Name": "Ghost",
            "Phonenumber": "000-000-0000",
            "Contact_email": "ghost@example.com"
        }
        response = self.client.put(f"suppliers/{updated_supplier['Id']}", json=updated_supplier)
        self.assertEqual(response.status_code, 404)

    def test_update_supplier_with_invalid_data(self):
        invalid_supplier = {
            "Name": "",
            "Address": "456 Updated St",
            "City": "Updated City",
            "Zip_Code": "54321",
            "Province": "Updated Province",
            "Country": "Updated Country",
            "Contact_Name": "Updated Name",
            "Phonenumber": "321-654-0987",
            "Contact_email": "updated@example.com"
        }
        response = self.client.put(f"suppliers/0", json=invalid_supplier)
        self.assertEqual(response.status_code, 400)

    def test_update_supplier_when_id_in_data_and_id_in_route_differ(self):
        conflicting_supplier = {
            "Id": 2,
            "Name": "Conflicting Supplier",
            "Address": "456 Conflicting St",
            "City": "Conflict City",
            "Zip_Code": "54321",
            "Province": "Conflict Province",
            "Country": "Conflict Country",
            "Contact_Name": "Conflict Name",
            "Phonenumber": "321-654-0987",
            "Contact_email": "conflict@example.com"
        }
        response = self.client.put("suppliers/1", json=conflicting_supplier)
        self.assertEqual(response.status_code, 404)

    # Region: Warehouse
    def test_update_existing_warehouse(self):
        updated_warehouse = {
            "Id": 1,
            "Name": "Updated Warehouse",
            "Address": "456 Updated St",
        }
        response = self.client.put(f"warehouses/{updated_warehouse['Id']}", json=updated_warehouse)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_warehouse(self):
        updated_warehouse = {
            "Id": -1,
            "Name": "Non-existent Warehouse",
            "Address": "789 Non-existent St",
        }
        response = self.client.put(f"warehouses/{updated_warehouse['Id']}", json=updated_warehouse)
        self.assertEqual(response.status_code, 404)

    def test_update_warehouse_with_invalid_data(self):
        invalid_warehouse = {
            "Name": "",
            "Address": "456 Updated St",
        }
        response = self.client.put(f"warehouses/0", json=invalid_warehouse)
        self.assertEqual(response.status_code, 400)

    def test_update_warehouse_when_id_in_data_and_id_in_route_differ(self):
        conflicting_warehouse = {
            "Id": 2,
            "Name": "Conflicting Warehouse",
            "Address": "456 Conflicting St",
        }
        response = self.client.put("warehouses/1", json=conflicting_warehouse)
        self.assertEqual(response.status_code, 404)

    # Region: ItemType
    def test_update_existing_item_type(self):
        updated_item_type = {
            "Id": 1,
            "Name": "Updated Item Type",
            "Description": "Updated description",
        }
        response = self.client.put(f"item_types/{updated_item_type['Id']}", json=updated_item_type)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_item_type(self):
        updated_item_type = {
            "Id": 999,
            "Name": "Non-existent Item Type",
            "Description": "This item type does not exist",
        }
        response = self.client.put(f"item_types/{updated_item_type['Id']}", json=updated_item_type)
        self.assertEqual(response.status_code, 404)

    def test_update_item_type_with_invalid_data(self):
        invalid_item_type = {
            "Name": "",
            "Description": "Some description",
        }
        response = self.client.put(f"item_types/0", json=invalid_item_type)
        self.assertEqual(response.status_code, 400)

    # Region: Items
    def test_update_existing_item(self):
        updated_item = {
            "Uid": "P000001",
            "Code": "Updated Code",
            "Description": "Updated description",
            "Item_Line": 1,
            "Supplier_Id": 1
        }
        response = self.client.put(f"items/{updated_item['Uid']}", json=updated_item)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_item(self):
        updated_item = {
            "Uid": "item999",
            "Code": "Non-existent Code",
            "Description": "This item does not exist",
            "Item_Line": 1,
            "Supplier_Id": 1
        }
        response = self.client.put(f"items/{updated_item['Uid']}", json=updated_item)
        self.assertEqual(response.status_code, 404)

    def test_update_item_with_invalid_data(self):
        invalid_item = {
            "Code": "",
            "Description": "Some description",
            "Item_Line": 1,
            "Supplier_Id": 1
        }
        response = self.client.put(f"items/test", json=invalid_item)
        self.assertEqual(response.status_code, 400) 

    # Region: Orders
    def test_update_existing_order(self):
        updated_order = {
            "Id": 1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Reference": "Updated Ref",
            "Total_Amount": 100.00
        }
        response = self.client.put(f"orders/{updated_order['Id']}", json=updated_order)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_order(self):
        updated_order = {
            "Id": -1,
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Reference": "Non-existent Ref",
            "Total_Amount": 100.00
        }
        response = self.client.put(f"orders/{updated_order['Id']}", json=updated_order)
        self.assertEqual(response.status_code, 404)

    def test_update_order_with_invalid_data(self):
        invalid_order = {
            "Source_Id": 1,
            "Order_Date": "2024-10-18",
            "Request_Date": "2024-10-19",
            "Reference": "Invalid Ref",
            "Total_Amount": 100.00
        }
        response = self.client.put(f"orders/0", json=invalid_order)
        self.assertEqual(response.status_code, 400)

    # Region: Inventory
    def test_update_existing_inventory(self):
        updated_inventory = {
            "Id": 1,
            "Item_Id": "item1",
            "Description": "Updated Inventory",
            "Locations": [1, 2],
            "Total_On_Hand": 100,
            "Total_Expected": 200,
            "Total_Ordered": 150,
            "Total_Allocated": 50,
            "Total_Available": 50,
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20"
        }
        response = self.client.put(f"inventories/{updated_inventory['Id']}", json=updated_inventory)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_inventory(self):
        updated_inventory = {
            "Id": -1,
            "Item_Id": "item1",
            "Description": "Non-existent Inventory",
            "Locations": [1, 2],
            "Total_On_Hand": 100,
            "Total_Expected": 200,
            "Total_Ordered": 150,
            "Total_Allocated": 50,
            "Total_Available": 50,
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20"
        }
        response = self.client.put(f"inventories/{updated_inventory['Id']}", json=updated_inventory)
        self.assertEqual(response.status_code, 404)

    # Region: Item Lines
    def test_update_existing_item_line(self):
        updated_item_line = {
            "Id": 1,
            "Name": "Updated Item Line",
            "Description": "Updated Description",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20"
        }
        response = self.client.put(f"item_lines/{updated_item_line['Id']}", json=updated_item_line)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_item_line(self):
        updated_item_line = {
            "Id": -1,
            "Name": "Non-existent Item Line",
            "Description": "Non-existent Description",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20"
        }
        response = self.client.put(f"item_lines/{updated_item_line['Id']}", json=updated_item_line)
        self.assertEqual(response.status_code, 404)

    # Region: Locations
    def test_update_existing_location(self):
        updated_location = {
            "Id": 1,
            "Warehouse_Id": 1,
            "Code": "LOC01",
            "Name": "Updated Location",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20"
        }
        response = self.client.put(f"locations/{updated_location['Id']}", json=updated_location)
        self.assertEqual(response.status_code, 200)

    def test_update_non_existent_location(self):
        updated_location = {
            "Id": -1,
            "Warehouse_Id": 1,
            "Code": "LOC01",
            "Name": "Non-existent Location",
            "Created_At": "2024-01-01",
            "Updated_At": "2024-10-20"
        }
        response = self.client.put(f"locations/{updated_location['Id']}", json=updated_location)
        self.assertEqual(response.status_code, 404)       

if __name__ == "__main__":
    unittest.main()
