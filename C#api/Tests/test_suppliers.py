import httpx
import unittest
import json
import os
from datetime import datetime

class ApiSuppliersTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # Define the Supplier model (fields changed to match the Supplier model)
        cls.new_supplier = {
            "Id": 0,
            "Code": "SUP001",
            "Name": "New Supplier",
            "Address": "123 Supplier St",
            "Address_Extra": "Suite 100",
            "City": "Supplier City",
            "Zip_Code": "12345",
            "Province": "Supplier Province",
            "Country": "Supplierland",
            "Contact_Name": "John Supplier",
            "Phonenumber": "123-456-7890",
            "Reference": "REF001",
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

    def test_1get_all_suppliers(self):
        response = self.client.get("suppliers")
        self.assertEqual(response.status_code, 200)

    def test_2get_supplier_by_id(self):
        response = self.client.get(f"suppliers/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_supplier(self):
        response = self.client.get("suppliers/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests

    def test_4create_supplier(self):
        response = self.client.post("suppliers", json=self.new_supplier)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_supplier, self.GetJsonData("suppliers"))

    def test_5create_supplier_with_invalid_data(self):
        invalid_supplier = self.new_supplier.copy()
        invalid_supplier["Id"] = 1 # Invalid because Id has been taken already
        response = self.client.post("suppliers", json=invalid_supplier)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_supplier, self.GetJsonData("suppliers"))

    def test_6create_duplicate_supplier(self):
        duplicate_supplier = self.new_supplier.copy()
        response = self.client.post("suppliers", json=duplicate_supplier)
        self.assertEqual(response.status_code, 400)

    # PUT tests

    def test_7update_existing_supplier(self):
        updated_supplier = {
            "Id": self.new_supplier['Id'],  # Keep the same ID
            "Code": "SUP002",  # Changed
            "Name": "Updated Supplier",  # Changed
            "Address": "456 Updated Supplier St",  # Changed
            "Address_Extra": "Suite 200",  # Changed
            "City": "Updated City",  # Changed
            "Zip_Code": "54321",  # Changed
            "Province": "Updated Province",  # Changed
            "Country": "Updatedland",  # Changed
            "Contact_Name": "Jane Supplier",  # Changed
            "Phonenumber": "987-654-3210",  # Changed
            "Reference": "REF002",  # Changed
            "Created_At": self.new_supplier['Created_At'],  # Keep the same creation time
            "Updated_At": datetime.now().isoformat()  # New update time
        }
        
        response = self.client.put(f"suppliers/{self.new_supplier['Id']}", content=json.dumps(updated_supplier), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)
        
        suppliers_data = self.GetJsonData("suppliers")
        updated_supplier_exists = any(
            supplier['Id'] == updated_supplier['Id'] and supplier['Name'] == updated_supplier['Name']
            for supplier in suppliers_data
        )
        self.assertTrue(updated_supplier_exists, "Updated supplier with matching Id and Name not found in the data")

    def test_8update_non_existent_supplier(self):
        non_existent_supplier = self.new_supplier.copy()
        non_existent_supplier["Id"] = -1
        response = self.client.put("suppliers/-1", content=json.dumps(non_existent_supplier), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_supplier, self.GetJsonData("suppliers"))

    def test_9update_supplier_with_invalid_data(self):
        invalid_supplier = self.new_supplier.copy()
        invalid_supplier.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"suppliers/{self.new_supplier['Id']}", content=json.dumps(invalid_supplier), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_supplier, self.GetJsonData("suppliers"))

    # DELETE tests

    def test_delete_supplier(self):
        response = self.client.delete(f"suppliers/{self.new_supplier['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_supplier, self.GetJsonData("suppliers"))

    def test_delete_non_existent_supplier(self):
        response = self.client.delete("suppliers/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    #ID auto increment

    def test_11supplier_ID_auto_increment_working(self):
        idless_supplier = self.new_supplier.copy()
        idless_supplier.pop("Id")
        old_id = self.GetJsonData("suppliers")[-1].copy().pop("Id")
        response = self.client.post("suppliers", json=idless_supplier)
        self.assertEqual(response.status_code, 201)
        potential_supplier = self.GetJsonData("suppliers")[-1].copy()
        id = potential_supplier["Id"]
        potential_supplier.pop("Id")
        self.assertEqual(idless_supplier, potential_supplier)
        self.assertEqual(old_id+1, id) 

        response = self.client.delete(f"suppliers/{id}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(idless_supplier, self.GetJsonData("suppliers"))

    
    def test_12supplier_ID_duplicate_creation_fails(self):
        new_supplier = self.new_supplier.copy()
        new_supplier.pop("Id")
        response = self.client.post("suppliers", json=new_supplier)
        self.assertEqual(response.status_code, 201)
        created_supplier = self.GetJsonData("suppliers")[-1]
        existing_id = created_supplier["Id"]

        duplicate_supplier = new_supplier.copy()
        duplicate_supplier["Id"] = existing_id
        suppliers_after = self.GetJsonData("suppliers")
        response = self.client.post("suppliers", json=duplicate_supplier)

        self.assertEqual(response.status_code, 400)

        self.assertEqual(len(suppliers_after), len(self.GetJsonData("suppliers")))

        response = self.client.delete(f"suppliers/{existing_id}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_supplier, self.GetJsonData("suppliers"))

if __name__ == '__main__':
    unittest.main()
