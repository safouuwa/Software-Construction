import httpx
import unittest
import json
import os
from datetime import datetime

class ApiItemlinesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        cls.new_item_line = {
            "Id": 19998,
            "Name": "New Item line",
            "Description": "Description of the new item line",
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

    def test_1get_all_item_lines(self):
        response = self.client.get("item_lines")
        self.assertEqual(response.status_code, 200)

    def test_2get_item_line_by_id(self):
        response = self.client.get(f"item_lines/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)
    
    def test_get_item_line_items(self):
        response = self.client.get(f"item_lines/1/items")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()[0]["Item_Line"], 1)

    def test_3get_non_existent_item_line(self):
        response = self.client.get("item_lines/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests

    def test_4create_item_line(self):
        response = self.client.post("item_lines", json=self.new_item_line)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_item_line, self.GetJsonData("item_lines"))

    def test_5create_item_line_with_invalid_data(self):
        invalid_item_line = self.new_item_line.copy()
        invalid_item_line.pop("Id")  # Invalid because it has no Id
        response = self.client.post("item_lines", json=invalid_item_line)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_line, self.GetJsonData("item_lines"))

    def test_6create_duplicate_item_line(self):
        duplicate_item_line = self.new_item_line.copy()
        response = self.client.post("item_lines", json=duplicate_item_line)
        self.assertEqual(response.status_code, 404)

    # PUT tests

    def test_7update_existing_item_line(self):
        updated_item_line = {
            "Id": self.new_item_line['Id'],  # Keep the same ID
            "Name": "Updated Item line",  # Changed
            "Description": "Updated description",  # Changed
            "Created_At": self.new_item_line['Created_At'],  # Keep the same creation time
            "Updated_At": datetime.now().isoformat()  # New update time
        }
        
        response = self.client.put(f"item_lines/{self.new_item_line['Id']}", content=json.dumps(updated_item_line), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)
        
        item_lines_data = self.GetJsonData("item_lines")
        updated_item_line_exists = any(
            line['Id'] == updated_item_line['Id'] and line['Name'] == updated_item_line['Name']
            for line in item_lines_data
        )
        self.assertTrue(updated_item_line_exists, "Updated item line with matching Id and Name not found in the data")

    def test_8update_non_existent_item_line(self):
        non_existent_item_line = self.new_item_line.copy()
        non_existent_item_line["Id"] = -1
        response = self.client.put("item_lines/-1", content=json.dumps(non_existent_item_line), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_item_line, self.GetJsonData("item_lines"))

    def test_9update_item_line_with_invalid_data(self):
        invalid_item_line = self.new_item_line.copy()
        invalid_item_line.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"item_lines/{self.new_item_line['Id']}", content=json.dumps(invalid_item_line), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_line, self.GetJsonData("item_lines"))

    # DELETE tests

    def test_delete_item_line(self):
        response = self.client.delete(f"item_lines/{self.new_item_line['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_item_line, self.GetJsonData("item_lines"))

    def test_delete_non_existent_item_line(self):
        response = self.client.delete("item_lines/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()