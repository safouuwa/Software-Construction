import httpx
import unittest
import json
import os

class WarehouseManagerApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "f6g7h8i9j0"}) # Warehouse manager API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "data").replace(os.sep, "/")

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json").replace("\\", "/"), 'r', encoding='utf-8') as file:
            data = json.load(file)
        return data
    # 3 actions that they have the right to perform
        
    def test_GetAllTransfers(self):
        response = self.client.get("transfers")
        self.assertEqual(response.status_code, 200)

    def test_PostLocation(self):
        new_location = {
            "Warehouse_Id": 1,
            "Code": "LOC001",
            "Name": "New Location",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("locations", json=new_location)
        self.assertEqual(response.status_code, 201)
        
        response = self.client.delete(f"locations/{self.GetJsonData('locations')[-1]['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)

    def test_GetOrderbyId(self):
        response = self.client.get("orders/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    # 3 actions that they do not have the right to perform

    def test_DeleteWarehouse(self):
        response = self.client.delete(f"warehouses/1")
        self.assertEqual(response.status_code, 401)

    def test_DeleteClient(self):
        response = self.client.delete("clients/1")
        self.assertEqual(response.status_code, 401)

    def test_DeleteItemType(self):
        response = self.client.delete("item_types/1")
        self.assertEqual(response.status_code, 401)



if __name__ == '__main__':
    unittest.main() 