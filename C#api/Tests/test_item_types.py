import httpx
import unittest
import json
import os
from datetime import datetime

class ApiItemtypesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        cls.new_item_type = {
            "Id": 19998,
            "Name": "New Item type",
            "Description": "Description of the new item type",
            "Created_At": datetime.now().isoformat(),
            "Updated_At": datetime.now().isoformat()
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

    def test_1get_all_item_types(self):
        response = self.client.get("item_types")
        self.assertEqual(response.status_code, 200)

    def test_2get_item_type_by_id(self):
        response = self.client.get(f"item_types/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_get_item_types_items(self):
        response = self.client.get(f"item_types/1/items")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()[0]["Item_Type"], 1)

    def test_3get_non_existent_item_type(self):
        response = self.client.get("item_types/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests

    def test_4create_item_type(self):
        response = self.client.post("item_types", json=self.new_item_type)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_item_type, self.GetJsonData("item_types"))

    def test_5create_item_type_with_invalid_data(self):
        invalid_item_type = self.new_item_type.copy()
        invalid_item_type.pop("Id")  # Invalid because it has no Id
        response = self.client.post("item_types", json=invalid_item_type)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_type, self.GetJsonData("item_types"))

    def test_6create_duplicate_item_type(self):
        duplicate_item_type = self.new_item_type.copy()
        response = self.client.post("item_types", json=duplicate_item_type)
        self.assertEqual(response.status_code, 404)

    # PUT tests

    def test_7update_existing_item_type(self):
        updated_item_type = {
            "Id": self.new_item_type['Id'],  # Keep the same ID
            "Name": "Updated Item type",  # Changed
            "Description": "Updated description",  # Changed
            "Created_At": self.new_item_type['Created_At'],  # Keep the same creation time
            "Updated_At": datetime.now().isoformat()  # New update time
        }
        
        response = self.client.put(f"item_types/{self.new_item_type['Id']}", content=json.dumps(updated_item_type), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)
        
        item_types_data = self.GetJsonData("item_types")
        updated_item_type_exists = any(
            type['Id'] == updated_item_type['Id'] and type['Name'] == updated_item_type['Name']
            for type in item_types_data
        )
        self.assertTrue(updated_item_type_exists, "Updated item type with matching Id and Name not found in the data")

    def test_8update_non_existent_item_type(self):
        non_existent_item_type = self.new_item_type.copy()
        non_existent_item_type["Id"] = -1
        response = self.client.put("item_types/-1", content=json.dumps(non_existent_item_type), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_item_type, self.GetJsonData("item_types"))

    def test_9update_item_type_with_invalid_data(self):
        invalid_item_type = self.new_item_type.copy()
        invalid_item_type.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"item_types/{self.new_item_type['Id']}", content=json.dumps(invalid_item_type), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_type, self.GetJsonData("item_types"))

    # DELETE tests

    def test_delete_item_type(self):
        response = self.client.delete(f"item_types/{self.new_item_type['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_item_type, self.GetJsonData("item_types"))

    def test_delete_non_existent_item_type(self):
        response = self.client.delete("item_types/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()