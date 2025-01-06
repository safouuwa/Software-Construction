import httpx
import unittest
import json
import os

class SalesApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "o1p2q3r4s5"})  # Sales API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "data").replace(os.sep, "/")

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json").replace("\\", "/"), 'r', encoding='utf-8') as file:
            data = json.load(file)
        return data

    # 3 actions that they have the right to perform
        
    def test_GetWarehouses(self):
        response = self.client.get("warehouses")
        self.assertEqual(response.status_code, 200)

    def test_PostClient(self):
        new_client = {
            "Name": "New Client",
            "Address": "123 Main St",
            "City": "Anytown",
            "Zip_code": "12345",
            "Province": "Province",
            "Country": "Country",
            "Contact_name": "John Doe",
            "Contact_phone": "123-456-7890",
            "Contact_email": "johndoe@example.com",
            "Created_at": "2024-11-14T16:10:14.227318",
            "Updated_at": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("clients", json=new_client)
        self.assertEqual(response.status_code, 201)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        
        response = self.client.delete(f"clients/{self.GetJsonData('clients')[-1]['Id']}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "o1p2q3r4s5"

    def test_UpdateItem(self):
        updated_item = {
            "Code": "CODE123",
            "Description": "This is a test item.",
            "Short_Description": "Test Item",
            "Upc_Code": "123456789012",
            "Model_Number": "MODEL123",
            "Commodity_Code": "COMMOD123",
            "Item_Line": 1,
            "Item_Group": 2,
            "Item_Type": 3,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 20,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP123",
            "Supplier_Part_Number": "SUP123-PART001",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }

        response = self.client.post("items", json=updated_item)
        self.assertEqual(response.status_code, 201)

        updated_item = {
            "Code": "CODE123",
            "Description": "This is a test item. 2", #change
            "Short_Description": "Test Item2",
            "Upc_Code": "123456789012",
            "Model_Number": "MODEL123",
            "Commodity_Code": "COMMOD123",
            "Item_Line": 1,
            "Item_Group": 2,
            "Item_Type": 3,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 20,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP123",
            "Supplier_Part_Number": "SUP123-PART001",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }

        response = self.client.put(f"items/{self.GetJsonData("items")[-1]["Uid"]}", json=updated_item)
        self.assertEqual(response.status_code, 200)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"

        response = self.client.delete(f"items/{self.GetJsonData("items")[-1]["Uid"]}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "o1p2q3r4s5"

    # 3 actions that they do not have the right to perform

    def test_PostWarehouse(self):
        new_warehouse = {
            "Code": "WAR001",
            "Name": "New Warehouse",
            "Address": "123 Storage St",
            "Zip": "12345",
            "City": "Storageville",
            "Province": "Storagestate",
            "Country": "Storageland",
            "Contact": {
                "Name": "John Doe",
                "Phone": "123-456-7890",
                "Email": "johndoe@example.com"
            },
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("warehouses", json=new_warehouse)
        self.assertEqual(response.status_code, 401)

    def test_GetTransfers(self):
        response = self.client.get("transfers")
        self.assertEqual(response.status_code, 401)

    def test_DeleteInventory(self):
        response = self.client.delete("inventories/1")
        self.assertEqual(response.status_code, 401)

if __name__ == '__main__':
    unittest.main()