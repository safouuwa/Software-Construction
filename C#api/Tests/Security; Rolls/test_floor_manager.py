import httpx
import unittest
import os

class FloorManagerApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "p6q7r8s9t0"})  # Floor Manager API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")

    # 3 actions that they have the right to perform
        
    def test_GetSingleWarehouse(self):
        response = self.client.get("warehouses/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_PostLocation(self):
        new_location = {
            "Id": 0,
            "Warehouse_Id": 1,
            "Code": "LOC002",
            "Name": "New Floor Location",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("locations", json=new_location)
        self.assertEqual(response.status_code, 201)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        response = self.client.delete(f"locations/{new_location['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "p6q7r8s9t0"

    def test_GetAllTransfers(self):
        response = self.client.get("transfers")
        self.assertEqual(response.status_code, 200)

    # 3 actions that they do not have the right to perform

    def test_PostItemLine(self):
        new_item_line = {
            "Id": 0,
            "Name": "New Item line",
            "Description": "Description of the new item line",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("item_lines", json=new_item_line)
        self.assertEqual(response.status_code, 401)

    def test_UpdateItemGroup(self):
        updated_item_group = {
            "Id": 1,
            "Name": "New Item group",
            "Description": "Description of the new item line",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.put("item_groups/1", json=updated_item_group)
        self.assertEqual(response.status_code, 401)

    def test_DeleteClient(self):
        response = self.client.delete("clients/1")
        self.assertEqual(response.status_code, 401)

if __name__ == '__main__':
    unittest.main()