import httpx
import unittest
import json
import os
from datetime import datetime

class ApiInventoriesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        cls.new_inventory = {
            "Id": 0,
            "Item_Id": "ITEM001",
            "Description": "Test Item",
            "Item_Reference": "REF001",
            "Locations": [1, 2, 3],
            "Total_On_Hand": 100,
            "Total_Expected": 150,
            "Total_Ordered": 50,
            "Total_Allocated": 30,
            "Total_Available": 70,
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        
        cls.test_methods = [method for method in dir(cls) if method.startswith('test_')]
        cls.current_test_index = 0

    def setUp(self):
        current_method = self._testMethodName
        expected_method = self.test_methods[self.current_test_index]
        self.assertEqual(current_method, expected_method, 
                         f"Tests are running out of order. Expected {expected_method}, but running {current_method}")
        self.__class__.current_test_index += 1

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json"), 'r') as file:
            data = json.load(file)
        return data
    
    # GET tests
    
    def test_1get_all_inventories(self):
        response = self.client.get("inventories")
        self.assertEqual(response.status_code, 200)

    def test_2get_inventory_by_id(self):
        response = self.client.get(f"inventories/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_inventory(self):
        response = self.client.get("inventories/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests
    
    def test_4create_inventory(self):
        response = self.client.post("inventories", json=self.new_inventory)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_inventory, self.GetJsonData("inventories"))

    def test_5create_inventory_with_invalid_data(self):
        invalid_inventory = self.new_inventory.copy()
        invalid_inventory.pop("Id")  # Invalid because it has no Id
        response = self.client.post("inventories", json=invalid_inventory)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_inventory, self.GetJsonData("inventories"))

    def test_6create_duplicate_inventory(self):
        duplicate_inventory = self.new_inventory.copy()
        response = self.client.post("inventories", json=duplicate_inventory)
        self.assertEqual(response.status_code, 404)

    # PUT tests
    
    def test_7update_existing_inventory(self):
        updated_inventory = {
            "Id": self.new_inventory['Id'],  # Keep the same ID
            "Item_Id": "ITEM002",  # Changed
            "Description": "Updated Item",  # Changed
            "Item_Reference": "REF002",  # Changed
            "Locations": [4, 5, 6],  # Changed
            "Total_On_Hand": 120,  # Changed
            "Total_Expected": 180,  # Changed
            "Total_Ordered": 60,  # Changed
            "Total_Allocated": 40,  # Changed
            "Total_Available": 80,  # Changed
            "Created_At": self.new_inventory['Created_At'],  # Keep the same creation time
            "Updated_At": "2024-11-14T16:10:14.227318"  # New update time
        }
        
        response = self.client.put(f"inventories/{self.new_inventory['Id']}", content=json.dumps(updated_inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)
        
        inventories_data = self.GetJsonData("inventories")
        updated_inventory_exists = any(
            inventory['Id'] == updated_inventory['Id'] and inventory['Item_Id'] == updated_inventory['Item_Id']
            for inventory in inventories_data
        )
        self.assertTrue(updated_inventory_exists, "Updated inventory with matching Id and Item_Id not found in the data")

    def test_8update_non_existent_inventory(self):
        non_existent_inventory = self.new_inventory.copy()
        non_existent_inventory["Id"] = -1
        response = self.client.put("inventories/-1", content=json.dumps(non_existent_inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_inventory, self.GetJsonData("inventories"))

    def test_9update_inventory_with_invalid_data(self):
        invalid_inventory = self.new_inventory.copy()
        invalid_inventory.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"inventories/{self.new_inventory['Id']}", content=json.dumps(invalid_inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_inventory, self.GetJsonData("inventories"))

    def test_update_inventory_when_id_in_data_and_id_in_route_differ(self):
        inventory = self.new_inventory.copy()
        inventory["Id"] = -1 
        response = self.client.put("inventories/1", content=json.dumps(inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(inventory, self.GetJsonData("inventories"))    

    # PATCH tests
    def test_partially_update_non_existent_inventory(self):
        non_existent_inventory = self.new_inventory.copy()
        non_existent_inventory["Id"] = -1
        response = self.client.patch(
            "inventories/-1",
            content=json.dumps(non_existent_inventory),
            headers={"Content-Type": "application/json"}
        )
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_inventory, self.GetJsonData("inventories"))

    # DELETE tests
    
    def test_delete_inventory(self):
        response = self.client.delete(f"inventories/{self.new_inventory['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_inventory, self.GetJsonData("inventories"))

    def test_delete_non_existent_inventory(self):
        response = self.client.delete("inventories/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()