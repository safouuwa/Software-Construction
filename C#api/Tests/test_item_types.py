import httpx
import unittest
import json
import os
from datetime import datetime

class ApiItemTypesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v2/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New item type to create in the POST tests
        cls.new_item_type = {
            "Name": "New Item Type",
            "Description": "Description of the new item type",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }

        # Store the method names for ordering
        cls.test_methods = [method for method in dir(cls) if method.startswith('test_')]
        cls.current_test_index = 0

    def setUp(self):
        current_method = self._testMethodName
        expected_method = self.test_methods[self.current_test_index]
        self.assertEqual(current_method,
 expected_method, 
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
        response = self.client.get("item_types/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_item_type(self):
        response = self.client.get("item_types/-1")
        self.assertEqual(response.status_code, 204)

    # POST tests
    def test_4create_item_type(self):
        response = self.client.post("item_types", json=self.new_item_type)
        self.assertEqual(response.status_code, 201)
        created_item_type = self.GetJsonData("item_types")[-1]
        created_item_type.pop('Id')
        self.assertEqual(self.new_item_type, created_item_type)

    def test_5create_item_type_with_invalid_data(self):
        invalid_item_type = self.new_item_type.copy()
        invalid_item_type.pop("Name")  # Invalid because it has no Name
        response = self.client.post("item_types", json=invalid_item_type)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_type, self.GetJsonData("item_types"))

    # PUT tests
    def test_7update_existing_item_type(self):
        updated_item_type = self.new_item_type.copy()
        updated_item_type["Name"] = "Updated Item Type"
        updated_item_type["Description"] = "Updated description of the item type"
        updated_item_type["Updated_At"] = datetime.now().isoformat()

        last_id = self.GetJsonData("item_types")[-1]['Id']
        response = self.client.put(f"item_types/{last_id}", content=json.dumps(updated_item_type), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        item_types_data = self.GetJsonData("item_types")
        updated_item_type_exists = any(
            item_type['Id'] == last_id and item_type['Name'] == updated_item_type['Name']
            for item_type in item_types_data
        )
        self.assertTrue(updated_item_type_exists, "Updated item type with matching Id and Name not found in the data")

    def test_8update_non_existent_item_type(self):
        non_existent_item_type = self.new_item_type.copy()
        response = self.client.put("item_types/-1", content=json.dumps(non_existent_item_type), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_item_type, self.GetJsonData("item_types"))

    def test_9update_item_type_with_invalid_data(self):
        invalid_item_type = self.new_item_type.copy()
        invalid_item_type.pop("Name")  # Invalid because it has no Name
        last_id = self.GetJsonData("item_types")[-1]['Id']
        response = self.client.put(f"item_types/{last_id}", content=json.dumps(invalid_item_type), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_type, self.GetJsonData("item_types"))

    #patch tests
    def test_partial_non_existent_item_type(self):
        response = self.client.patch("item_types/-1", json={"Name": "Updated Item type"})
        self.assertEqual(response.status_code, 204)
        
    # DELETE tests
    def test_delete_item_type(self):
        last_id = self.GetJsonData("item_types")[-1]['Id']
        response = self.client.delete(f"item_types/{last_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_item_type, self.GetJsonData("item_types"))

    def test_delete_non_existent_item_type(self):
        response = self.client.delete("item_types/-1")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    #ID auto increment

    def test_11item_type_ID_auto_increment_working(self):
        idless_item_type = self.new_item_type.copy()
        old_id = self.GetJsonData("item_types")[-1]["Id"]
        response = self.client.post("item_types", json=idless_item_type)
        self.assertEqual(response.status_code, 201)
        created_item_type = self.GetJsonData("item_types")[-1]
        self.assertEqual(old_id + 1, created_item_type["Id"])
        self.assertEqual(idless_item_type["Name"], created_item_type["Name"])

        response = self.client.delete(f"item_types/{created_item_type['Id']}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_item_type, self.GetJsonData("item_types"))

if __name__ == '__main__':
    unittest.main()

