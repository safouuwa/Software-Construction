import httpx
import unittest
import json
import os
from datetime import datetime

class ApiWarehousesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v2/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        cls.new_warehouse = {
            "Code": "WAR001",
            "Name": "New Warehouse",
            "Address": "123 Storage St",
            "Zip": "12345",
            "City": "Storageville",
            "Province": "Storagestate",
            "Country": "Storageland",
            "Contact": {
                "Name": "John Doe",
                "Phone": "123-456-7890",
                "Email": "johndoe@example.com"
            },
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
    
    def test_1get_all_warehouses(self):
        response = self.client.get("warehouses")
        self.assertEqual(response.status_code, 200)

    def test_2get_warehouse_by_id(self):
        response = self.client.get(f"warehouses/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_warehouse(self):
        response = self.client.get("warehouses/-1")
        self.assertEqual(response.status_code, 204)
    
    def test_search_warehouses_by_name(self):
        response = self.client.get(f"warehouses/search?name=Heemskerk cargo hub")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Name'], "Heemskerk cargo hub")
    
    def test_search_warehouses_by_code(self):
        response = self.client.get(f"warehouses/search?code=YQZZNL56")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Code'], "YQZZNL56")

    def test_search_warehouses_by_name(self):
        response = self.client.get(f"warehouses/search?name=Heemskerk cargo hub")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Name'], "Heemskerk cargo hub")
    
    def test_search_warehouses_by_city(self):
        response = self.client.get(f"warehouses/search?city=Heemskerk")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['City'], "Heemskerk")

    def test_search_warehouses_by_country(self):
        response = self.client.get(f"warehouses/search?country=NL")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for response in response.json():
            self.assertEqual(response['Country'], "NL")
            
    def test_search_warehouses_with_invalid_parameter(self):
        response = self.client.get("warehouses/search?invalid_param=invalid_value")
        self.assertEqual(response.status_code, 400)
        self.assertIn("At least one search parameter must be provided.", response.text)
    
    def test_search_warehouses_with_valid_and_invalid_parameter(self):
        response = self.client.get("warehouses/search?code=YQZZNL56&invalid_param=invalid_value")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for warehouse in response.json():
            self.assertEqual(warehouse['Code'], "YQZZNL56")
    
    def test_search_warehouses_by_code_and_country(self):
        response = self.client.get(f"warehouses/search?code=YQZZNL56&country=NL")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for response in response.json():
            self.assertEqual(response['Code'], "YQZZNL56")
            self.assertEqual(response['Country'], "NL")
    
    def test_search_warehouses_by_code_and_city(self):
        response = self.client.get(f"warehouses/search?code=YQZZNL56&city=Heemskerk")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for warehouse in response.json():
            self.assertEqual(warehouse['Code'], "YQZZNL56")
            self.assertEqual(warehouse['City'], "Heemskerk")
            
    def test_sort_warehouses_by_warehouse_id(self):
        response = self.client.get("warehouses?sortOrder=desc&page=1&pageSize=10")
        self.assertEqual(response.status_code, 200)
        response_data = response.json()
        items = response_data.get("Items", [])
        self.assertTrue(len(items) > 0, f"No warehouses found: {response_data}")
        ids = [warehouse["Id"] for warehouse in items]
        self.assertEqual(ids, sorted(ids, reverse=True))

    # POST tests
    
    def test_4create_warehouse(self):
        response = self.client.post("warehouses", json=self.new_warehouse)
        self.assertEqual(response.status_code, 201)
        created_warehouse = self.GetJsonData("warehouses")[-1]
        created_warehouse.pop('Id')
        self.assertEqual(self.new_warehouse, created_warehouse)

    def test_5create_warehouse_with_invalid_data(self):
        invalid_warehouse = self.new_warehouse.copy()
        invalid_warehouse.pop("Name")  # Invalid because it has no Name
        response = self.client.post("warehouses", json=invalid_warehouse)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_warehouse, self.GetJsonData("warehouses"))

    # PUT tests
    
    def test_7update_existing_warehouse(self):
        updated_warehouse = self.new_warehouse.copy()
        updated_warehouse["Name"] = "Updated Warehouse"
        updated_warehouse["Address"] = "456 Updated St"
        updated_warehouse["Updated_At"] = datetime.now().isoformat()
        
        last_id = self.GetJsonData("warehouses")[-1]['Id']
        response = self.client.put(f"warehouses/{last_id}", content=json.dumps(updated_warehouse), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)
        
        warehouses_data = self.GetJsonData("warehouses")
        updated_warehouse_exists = any(
            warehouse['Id'] == last_id and warehouse['Name'] == updated_warehouse['Name']
            for warehouse in warehouses_data
        )
        self.assertTrue(updated_warehouse_exists, "Updated warehouse with matching Id and Name not found in the data")

    def test_8update_non_existent_warehouse(self):
        non_existent_warehouse = self.new_warehouse.copy()
        response = self.client.put("warehouses/-1", content=json.dumps(non_existent_warehouse), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_warehouse, self.GetJsonData("warehouses"))

    def test_9update_warehouse_with_invalid_data(self):
        invalid_warehouse = self.new_warehouse.copy()
        invalid_warehouse.pop("Name")  # Invalid because it has no Name
        last_id = self.GetJsonData("warehouses")[-1]['Id']
        response = self.client.put(f"warehouses/{last_id}", content=json.dumps(invalid_warehouse), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_warehouse, self.GetJsonData("warehouses"))  

    # PATCH tests
    def test_11partially_update_non_existent_warehouse(self):
        response = self.client.patch("warehouses/-1", json={"Name": "Updated Warehouse"})
        self.assertEqual(response.status_code, 204)

    # DELETE tests
    
    def test_delete_warehouse(self):
        last_id = self.GetJsonData("warehouses")[-1]['Id']
        response = self.client.delete(f"warehouses/{last_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_warehouse, self.GetJsonData("warehouses"))

    def test_delete_non_existent_warehouse(self):
        response = self.client.delete("warehouses/-1/")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)
    
    #ID auto increment

    def test_11warehouse_ID_auto_increment_working(self):
        idless_warehouse = self.new_warehouse.copy()
        old_id = self.GetJsonData("warehouses")[-1]["Id"]
        response = self.client.post("warehouses", json=idless_warehouse)
        self.assertEqual(response.status_code, 201)
        created_warehouse = self.GetJsonData("warehouses")[-1]
        self.assertEqual(old_id + 1, created_warehouse["Id"])
        self.assertEqual(idless_warehouse["Name"], created_warehouse["Name"])

        response = self.client.delete(f"warehouses/{created_warehouse['Id']}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_warehouse, self.GetJsonData("warehouses"))

if __name__ == '__main__':
    unittest.main()

