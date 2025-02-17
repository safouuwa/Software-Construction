import httpx
import unittest
import json
import os
from datetime import datetime

class ApiItemLinesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v2/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New item line to create in the POST tests
        cls.new_item_line = {
            "Name": "New Item Line",
            "Description": "Description of the new item line",
            "Created_At": datetime.now().isoformat(),
            "Updated_At": datetime.now().isoformat()
        }

        # Store the method names for ordering
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
        response = self.client.get("item_lines/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_item_line(self):
        response = self.client.get("item_lines/-1")
        self.assertEqual(response.status_code, 204)

    # POST tests
    def test_4create_item_line(self):
        response = self.client.post("item_lines", json=self.new_item_line)
        self.assertEqual(response.status_code, 201)
        created_item_line = self.GetJsonData("item_lines")[-1]
        created_item_line.pop('Id')
        self.assertEqual(self.new_item_line, created_item_line)

    def test_5create_item_line_with_invalid_data(self):
        invalid_item_line = self.new_item_line.copy()
        invalid_item_line.pop("Name")  # Invalid because it has no Name
        response = self.client.post("item_lines", json=invalid_item_line)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_line, self.GetJsonData("item_lines"))

    # PUT tests
    def test_7update_existing_item_line(self):
        updated_item_line = self.new_item_line.copy()
        updated_item_line["Name"] = "Updated Item Line"
        updated_item_line["Description"] = "Updated description"
        updated_item_line["Updated_At"] = datetime.now().isoformat()

        last_id = self.GetJsonData("item_lines")[-1]['Id']
        response = self.client.put(f"item_lines/{last_id}", content=json.dumps(updated_item_line), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        item_lines_data = self.GetJsonData("item_lines")
        updated_item_line_exists = any(
            item_line['Id'] == last_id and item_line['Name'] == updated_item_line['Name']
            for item_line in item_lines_data
        )
        self.assertTrue(updated_item_line_exists, "Updated item line with matching Id and Name not found in the data")

    def test_8update_non_existent_item_line(self):
        non_existent_item_line = self.new_item_line.copy()
        response = self.client.put("item_lines/-1", content=json.dumps(non_existent_item_line), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_item_line, self.GetJsonData("item_lines"))

    def test_9update_item_line_with_invalid_data(self):
        invalid_item_line = self.new_item_line.copy()
        invalid_item_line.pop("Name")  # Invalid because it has no Name
        last_id = self.GetJsonData("item_lines")[-1]['Id']
        response = self.client.put(f"item_lines/{last_id}", content=json.dumps(invalid_item_line), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item_line, self.GetJsonData("item_lines"))

    # patch tests
    def test_partial_non_existent_item_line(self):
        response = self.client.patch("item_lines/-1", json={"Name": "Updated Item line"})
        self.assertEqual(response.status_code, 204)

    # DELETE tests
    def test_delete_item_line(self):
        last_id = self.GetJsonData("item_lines")[-1]['Id']
        response = self.client.delete(f"item_lines/{last_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_item_line, self.GetJsonData("item_lines"))

    def test_delete_non_existent_item_line(self):
        response = self.client.delete("item_lines/-1")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    #ID auto increment

    def test_11item_line_ID_auto_increment_working(self):
        idless_item_line = self.new_item_line.copy()
        old_id = self.GetJsonData("item_lines")[-1]["Id"]
        response = self.client.post("item_lines", json=idless_item_line)
        self.assertEqual(response.status_code, 201)
        created_item_line = self.GetJsonData("item_lines")[-1]
        self.assertEqual(old_id + 1, created_item_line["Id"])
        self.assertEqual(idless_item_line["Name"], created_item_line["Name"])

        response = self.client.delete(f"item_lines/{created_item_line['Id']}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_item_line, self.GetJsonData("item_lines"))

if __name__ == '__main__':
    unittest.main()

