import httpx
import unittest
import json
import os

class OperativeApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "u1v2w3x4y5"})  # Operative API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "data").replace(os.sep, "/")

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json"), 'r', encoding='utf-8') as file:
            data = json.load(file)
        return data
    # 3 actions that they have the right to perform
        
    def test_GetLocations(self):
        response = self.client.get("locations")
        self.assertEqual(response.status_code, 200)

    def test_PostTransfer(self):
        new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,   
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM125", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
        response = self.client.post("transfers", json=new_transfer)
        self.assertEqual(response.status_code, 201)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        
        response = self.client.delete(f"transfers/{self.GetJsonData('transfers')[-1]['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "u1v2w3x4y5"

    def test_GetSingleSupplier(self):
        response = self.client.get("suppliers/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    # 3 actions that they do not have the right to perform

    def test_GetWarehouses(self):
        response = self.client.get("warehouses")
        self.assertEqual(response.status_code, 401)

    def test_PostItemLine(self):
        new_item_line = {
            "Name": "New Item line",
            "Description": "Description of the new item line",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("item_lines", json=new_item_line)
        self.assertEqual(response.status_code, 401)

    def test_GetClients(self):
        response = self.client.get("clients")
        self.assertEqual(response.status_code, 401)

    # Own warehouse, Warehouse values hardcoded, because they are stored in a C# class
    def test_GetItems(self):
        response = self.client.get("items")
        self.assertEqual(response.status_code, 200)
        self.assertNotEqual(response.json(), self.GetJsonData("items"))
        inventory_dict = {item["Item_Id"]: item for item in self.GetJsonData("inventories")}
        inventorylist = [i for i in response.json()["Items"] if i["Uid"] in inventory_dict and inventory_dict[i["Uid"]] == i["Uid"]]
        for i in inventorylist:
            check = any(
                y["Warehouse_Id"] == 4 or y["Warehouse_Id"] == 5 or y["Warehouse_Id"] == 6
                for y in i["Locations"]
            )
            self.assertTrue(check)

if __name__ == '__main__':
    unittest.main()