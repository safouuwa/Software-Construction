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
            "Id": 0,
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
        self.assertEqual(response.status_code, 404)
    
    def test_search_locations_by_name(self):
        response = self.client.get("locations/search", params={"name": "New Location"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Name'] == "New Location" for location in locations))

    def test_search_locations_by_warehouse_id(self):
        response = self.client.get("locations/search", params={"warehouseId": 1})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Warehouse_Id'] == 1 for location in locations))

    def test_search_locations_by_code(self):
        response = self.client.get("locations/search", params={"code": "LOC001"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Code'] == "LOC001" for location in locations))

    def test_search_locations_by_created_at(self):
        response = self.client.get("locations/search", params={"created_At": "2024-11-14T16:10:14.227318"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Created_At'] == "2024-11-14T16:10:14.227318" for location in locations))

    def test_search_locations_by_updated_at(self):
        response = self.client.get("locations/search", params={"updated_At": "2024-11-14T16:10:14.227318"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Updated_At'] == "2024-11-14T16:10:14.227318" for location in locations))

    def test_search_locations_by_name_and_warehouse_id(self):
        response = self.client.get("locations/search", params={"name": "New Location", "warehouseId": 1})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Name'] == "New Location" and location['Warehouse_Id'] == 1 for location in locations))

    def test_search_locations_by_name_and_code(self):
        response = self.client.get("locations/search", params={"name": "New Location", "code": "LOC001"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Name'] == "New Location" and location['Code'] == "LOC001" for location in locations))

    def test_search_locations_by_warehouse_id_and_code(self):
        response = self.client.get("locations/search", params={"warehouseId": 1, "code": "LOC001"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertTrue(any(location['Warehouse_Id'] == 1 and location['Code'] == "LOC001" for location in locations))

    def test_search_locations_no_results(self):
        response = self.client.get("locations/search", params={"name": "NonExistent", "code": "CODE999"})
        self.assertEqual(response.status_code, 200)
        locations = response.json()
        self.assertEqual(len(locations), 0)
        invalid_location.pop("Id")  # Invalid because it has no Id
        response = self.client.post("locations", json=invalid_location)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_location, self.GetJsonData("locations"))

    def test_6create_duplicate_location(self):
        duplicate_location = self.new_location.copy()
        response = self.client.post("locations", json=duplicate_location)
        self.assertEqual(response.status_code, 404)


    # POST tests
    def test_4create_location(self):
        response = self.client.post("locations", json=self.new_location)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_location, self.GetJsonData("locations"))

    def test_5create_location_with_invalid_data(self):
        invalid_location = self.new_location.copy()
        invalid_location.pop("Id")  # Invalid because it has no Id
        response = self.client.post("locations", json=invalid_location)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_location, self.GetJsonData("locations"))

    def test_6create_duplicate_location(self):
        duplicate_location = self.new_location.copy()
        response = self.client.post("locations", json=duplicate_location)
        self.assertEqual(response.status_code, 404)
    
    

    # PUT tests
    def test_7update_existing_location(self):
        updated_location = {
            "Id": self.new_location['Id'],  # Keep the same ID
            "Warehouse_Id": self.new_location['Warehouse_Id'],  # Keep the same Warehouse ID
            "Code": "LOC002",  # Changed Code
            "Name": "Updated Location",  # Changed Name
            "Created_At": self.new_location['Created_At'],  # Keep the same creation time
            "Updated_At": datetime.now().isoformat()  # New update time
        }

        response = self.client.put(f"locations/{self.new_location['Id']}", content=json.dumps(updated_location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        locations_data = self.GetJsonData("locations")
        updated_location_exists = any(
            location['Id'] == updated_location['Id'] and location['Name'] == updated_location['Name']
            for location in locations_data
        )
        self.assertTrue(updated_location_exists, "Updated location with matching Id and Name not found in the data")

    def test_8update_non_existent_location(self):
        non_existent_location = self.new_location.copy()
        non_existent_location["Id"] = -1
        response = self.client.put("locations/-1", content=json.dumps(non_existent_location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_location, self.GetJsonData("locations"))

    def test_9update_location_with_invalid_data(self):
        invalid_location = self.new_location.copy()
        invalid_location.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"locations/{self.new_location['Id']}", content=json.dumps(invalid_location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_location, self.GetJsonData("locations"))

    def test_update_location_when_id_in_data_and_id_in_route_differ(self):
        location = self.new_location.copy()
        location["Id"] = -1 
        response = self.client.put("locations/1", content=json.dumps(location), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(location, self.GetJsonData("locations"))    

    # DELETE tests
    def test_delete_location(self):
        response = self.client.delete(f"locations/{self.new_location['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_location, self.GetJsonData("locations"))

    def test_delete_non_existent_location(self):
        response = self.client.delete("locations/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()
