import httpx
import unittest
import json
import os
from datetime import datetime

class ApiLocationsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New location to create in the POST tests
        cls.new_location = {
            "Warehouse_Id": 1,  # Assuming the location is associated with a warehouse with ID 1
            "Code": "LOC001",
            "Name": "New Location",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
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
    def test_1get_all_locations(self):
        response = self.client.get("locations")
        self.assertEqual(response.status_code, 200)

    def test_2get_location_by_id(self):
        response = self.client.get("locations/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_location(self):
        response = self.client.get("locations/-1")
        self.assertEqual(response.status_code, 204)

    def test_search_locations_by_name(self):
        response = self.client.get("locations/search?name=Row: A, Rack: 1, Shelf: 0")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for name in response.json():
            self.assertEqual(name['Name'], "Row: A, Rack: 1, Shelf: 0")
    
    def test_search_locations_by_code(self):
        response = self.client.get("locations/search?code=A.1.0")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for code in response.json():
            self.assertEqual(code['Code'], "A.1.0")
    
    def test_search_locations_by_warehouse_id(self):
        response = self.client.get("locations/search?warehouseid=1")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for location in response.json():
            self.assertTrue(location['Warehouse_Id'] == 1)
    
    def test_search_locations_with_invalid_parameter(self):
        response = self.client.get("locations/search?invalid_param=invalid_value")
        self.assertEqual(response.status_code, 400)
        self.assertIn("At least one search parameter must be provided.", response.text)
    
    def test_search_locations_with_valid_and_invalid_parameter(self):
        response = self.client.get("locations/search?name=Row: A, Rack: 1, Shelf: 0&invalid_param=invalid_value")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for location in response.json():
            self.assertEqual(location['Name'], "Row: A, Rack: 1, Shelf: 0")
    
    def test_search_locations_by_name_and_code(self):
        response = self.client.get("locations/search?name=Row: A, Rack: 1, Shelf: 0&code=A.1.0")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Name'], "Row: A, Rack: 1, Shelf: 0")
            self.assertEqual(x['Code'], "A.1.0")
            
    def test_search_locations_by_name_and_warehouse_id(self):
        response = self.client.get("locations/search?name=Row: A, Rack: 1, Shelf: 0&warehouseid=1")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Name'], "Row: A, Rack: 1, Shelf: 0")
            self.assertEqual(x['Warehouse_Id'], 1)
            
    def test_search_locations_by_code_and_warehouse_id(self):
        response = self.client.get("locations/search?code=A.1.0&warehouseid=1")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Code'], "A.1.0")
            self.assertEqual(x['Warehouse_Id'], 1)
    
    def test_search_locations_by_name_and_code_and_warehouse_id(self):
        response = self.client.get("locations/search?name=Row: A, Rack: 1, Shelf: 0&code=A.1.0&warehouseid=1")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Name'], "Row: A, Rack: 1, Shelf: 0")
            self.assertEqual(x['Code'], "A.1.0")
            self.assertEqual(x['Warehouse_Id'], 1)

    def test_get_location_warehouse(self):
        id = 1
        response = self.client.get(f"locations/{id}/warehouse")
        self.assertEqual(response.status_code, 200)
        warehouse = response.json()
        self.assertIn("Id", warehouse)
        self.assertIn("Name", warehouse)
        self.assertIn("Address", warehouse)
    
    def test_get_location_warehouse_invalid_id(self):
        response = self.client.get("locations/-1/warehouse")
        self.assertEqual(response.status_code, 204)
       
    # POST tests
    def test_4create_location(self):
        response = self.client.post("locations", json=self.new_location)
        self.assertEqual(response.status_code, 201)
        created_location = self.GetJsonData("locations")[-1]
        created_location.pop('Id')
        self.assertEqual(self.new_location, created_location)

    def test_5create_location_with_invalid_data(self):
        invalid_location = self.new_location.copy()
        invalid_location.pop("Name")  # Invalid because it has no Name
        response = self.client.post("locations", json=invalid_location)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_location, self.GetJsonData("locations"))

    # PUT tests
    def test_7update_existing_location(self):
        updated_location = {
            "Warehouse_Id": self.new_location['Warehouse_Id'],  # Keep the same Warehouse ID
            "Code": "LOC002",  # Changed Code
            "Name": "Updated Location",  # Changed Name
            "Created_At": self.new_location['Created_At'],  # Keep the same creation time
            "Updated_At": datetime.now().isoformat()  # New update time
        }

        last_id = self.GetJsonData("locations")[-1]['Id']
        response = self.client.put(f"locations/{last_id}", content=json.dumps(updated_location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        locations_data = self.GetJsonData("locations")
        updated_location_exists = any(
            location['Id'] == last_id and location['Name'] == updated_location['Name']
            for location in locations_data
        )
        self.assertTrue(updated_location_exists, "Updated location with matching Id and Name not found in the data")

    def test_8update_non_existent_location(self):
        non_existent_location = self.new_location.copy()
        response = self.client.put("locations/-1", content=json.dumps(non_existent_location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_location, self.GetJsonData("locations"))

    def test_9update_location_with_invalid_data(self):
        invalid_location = self.new_location.copy()
        invalid_location.pop("Name")  # Invalid because it has no Name
        last_id = self.GetJsonData("locations")[-1]['Id']
        response = self.client.put(f"locations/{last_id}", content=json.dumps(invalid_location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_location, self.GetJsonData("locations"))  

    # PATCH tests
    def test_partial_update_non_existent_location(self):
        response = self.client.patch("locations/-1", json={"Name": "Updated Location"})
        self.assertEqual(response.status_code, 204)

    # DELETE tests
    def test_delete_location(self):
        last_id = self.GetJsonData("locations")[-1]['Id']
        response = self.client.delete(f"locations/{last_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_location, self.GetJsonData("locations"))

    def test_delete_non_existent_location(self):
        response = self.client.delete("locations/-1")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    #ID auto increment

    def test_11location_ID_auto_increment_working(self):
        idless_location = self.new_location.copy()
        old_id = self.GetJsonData("locations")[-1]["Id"]
        response = self.client.post("locations", json=idless_location)
        self.assertEqual(response.status_code, 201)
        created_location = self.GetJsonData("locations")[-1]
        self.assertEqual(old_id + 1, created_location["Id"])
        self.assertEqual(idless_location["Name"], created_location["Name"])

        response = self.client.delete(f"locations/{created_location['Id']}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_location, self.GetJsonData("locations"))

if __name__ == '__main__':
    unittest.main()