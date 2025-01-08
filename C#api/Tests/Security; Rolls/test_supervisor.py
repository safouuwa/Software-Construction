import httpx
import unittest
import json
import os
import json

class SupervisorApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "z6a7b8c9d0"})  # Supervisor API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "data").replace(os.sep, "/")

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json"), 'r', encoding='utf-8') as file:
            data = json.load(file)
        return data
    # 3 actions that they have the right to perform
        
    def test_GetSingleOrder(self):
        response = self.client.get("orders/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_PostTransfer(self):
        new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,   
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
        response = self.client.post("transfers", json=new_transfer)
        self.assertEqual(response.status_code, 201)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        
        response = self.client.delete(f"transfers/{self.GetJsonData('transfers')[-1]['Id']}") 
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "z6a7b8c9d0"

    def test_GetShipments(self):
        response = self.client.get("shipments")
        self.assertEqual(response.status_code, 200)

    # 3 actions that they do not have the right to perform

    def test_PostSupplier(self):
        new_supplier = {
            "Code": "SUP001",
            "Name": "New Supplier",
            "Address": "123 Supplier St",
            "Address_Extra": "Suite 100",
            "City": "Supplier City",
            "Zip_Code": "12345",
            "Province": "Supplier Province",
            "Country": "Supplierland",
            "Contact_Name": "John Supplier",
            "Phonenumber": "123-456-7890",
            "Reference": "REF001",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("suppliers", json=new_supplier)
        self.assertEqual(response.status_code, 401)

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
        response = self.client.put(f"items/{self.GetJsonData("items")[-1]['Uid']}", json=updated_item)
        self.assertEqual(response.status_code, 401)

    def test_DeleteOrder(self):
        response = self.client.delete("orders/1")
        self.assertEqual(response.status_code, 401)

    # Own warehouse, Warehouse values hardcoded, because they are stored in a C# class
    def test_GetOrders(self):
        response = self.client.get("orders")
        self.assertEqual(response.status_code, 200)
        self.assertNotEqual(response.json(), self.GetJsonData("orders"))
        check = all(
            order["Warehouse_Id"] == 1 or order["Warehouse_Id"] == 2 or order["Warehouse_Id"] == 3
            for order in response.json()
        )
        self.assertTrue(check)

    def test_GetInventories(self):
        response = self.client.get("inventories")
        self.assertEqual(response.status_code, 200)
        self.assertNotEqual(response.json(), self.GetJsonData("inventories"))
        location_dict = {x["Id"]: x for x in self.GetJsonData("locations")}
        warehouse_ids = {1, 2, 3}
        for i in response.json():
            locationlist = [location_dict[y] for y in i["Locations"] if y in location_dict]
            check = any(y["Warehouse_Id"] in warehouse_ids for y in locationlist)
            self.assertTrue(check)
            locationlist = []


if __name__ == '__main__':
    unittest.main()