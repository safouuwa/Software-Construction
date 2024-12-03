import httpx
import unittest
import json
import os
from datetime import datetime


class ApiWarehousesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url,
                                  headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join
        (os.path.dirname
         (os.path.dirname
          (os.path.dirname
           (os.path.abspath(__file__)))), "data").replace(os.sep, "/")

        cls.new_warehouse = {
            "Id": 0,
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
        cls.test_methods = [method for method in dir(cls)
                            if method.startswith('test_')]
        cls.current_test_index = 0

    def setUp(self):
        current_method = self._testMethodName
        expected_method = self.test_methods[self.current_test_index]
        self.assertEqual(current_method, expected_method,
                         f"Tests are running out of order."
                         f"Expected {expected_method}, "
                         f"but running {current_method}")
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
        response = self.client.get("warehouses/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_warehouse(self):
        response = self.client.get("warehouses/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests

    def test_4create_warehouse(self):
        response = self.client.post("warehouses", json=self.new_warehouse)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_warehouse, self.GetJsonData("warehouses"))

    def test_5create_warehouse_with_invalid_data(self):
        invalid_warehouse = self.new_warehouse.copy()
        invalid_warehouse.pop("Id")  # Invalid because it has no Id
        response = self.client.post("warehouses", json=invalid_warehouse)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_warehouse, self.GetJsonData("warehouses"))

    def test_6create_duplicate_warehouse(self):
        duplicate_warehouse = self.new_warehouse.copy()
        response = self.client.post("warehouses", json=duplicate_warehouse)
        self.assertEqual(response.status_code, 404)

    # PUT tests

    def test_7update_existing_warehouse(self):
        updated_warehouse = {
            "Id": self.new_warehouse['Id'],  # Keep the same ID
            "Code": "WAR002",  # Changed
            "Name": "Updated Warehouse",  # Changed
            "Address": "456 Updated St",  # Changed
            "Zip": "54321",  # Changed
            "City": "Updatedville",  # Changed
            "Province": "Updatestate",  # Changed
            "Country": "Updateland",  # Changed
            "Contact": {
                "Name": "Jane Doe",  # Changed
                "Phone": "987-654-3210",  # Changed
                "Email": "janedoe@example.com"  # Changed
            },
            "Created_At": self.new_warehouse['Created_At'],
            "Updated_At": datetime.now().isoformat()  # New update time
        }

        response = self.client.put(f"warehouses/{self.new_warehouse['Id']}",
                                   content=json.dumps(updated_warehouse),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 200)

        warehouses_data = self.GetJsonData("warehouses")
        updated_warehouse_exists = any(
            warehouse['Id'] == updated_warehouse['Id'] and
            warehouse['Name'] == updated_warehouse['Name']
            for warehouse in warehouses_data
        )
        self.assertTrue(updated_warehouse_exists,
                        "Updated warehouse with matching Id and "
                        "Name not found in the data")

    def test_8update_non_existent_warehouse(self):
        non_existent_warehouse = self.new_warehouse.copy()
        non_existent_warehouse["Id"] = -1
        response = self.client.put("warehouses/-1",
                                   content=json.dumps(non_existent_warehouse),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_warehouse,
                         self.GetJsonData("warehouses"))

    def test_9update_warehouse_with_invalid_data(self):
        invalid_warehouse = self.new_warehouse.copy()
        invalid_warehouse.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"warehouses/{self.new_warehouse['Id']}",
                                   content=json.dumps(invalid_warehouse),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_warehouse, self.GetJsonData("warehouses"))

    def test_update_warehouse_when_id_in_data_and_id_in_route_differ(self):
        warehouse = self.new_warehouse.copy()
        warehouse["Id"] = -1
        response = self.client.put("warehouses/1",
                                   content=json.dumps(warehouse),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(warehouse, self.GetJsonData("warehouses"))

    # DELETE tests

    def test_delete_warehouse(self):
        response = self.client.delete(f"warehouses/{self.new_warehouse['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_warehouse, self.GetJsonData("warehouses"))

    def test_delete_non_existent_warehouse(self):
        response = self.client.delete("warehouses/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)


if __name__ == '__main__':
    unittest.main()
