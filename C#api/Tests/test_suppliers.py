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
        
        # New supplier to create in the POST tests
        cls.new_supplier = {
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
    def test_1get_all_suppliers(self):
        response = self.client.get("suppliers")
        self.assertEqual(response.status_code, 200)

    def test_2get_supplier_by_id(self):
        response = self.client.get("suppliers/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_supplier(self):
        response = self.client.get("suppliers/-1")
        self.assertEqual(response.status_code, 404)
    
    def test_search_suppliers_name(self):
        response = self.client.get(f"suppliers/search?name=Lee, Parks and Johnson")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, True)
        for name in response.json():
            self.assertEqual(name['Name'], "Lee, Parks and Johnson")
        
    def test_search_suppliers_city(self):
        response = self.client.get(f"suppliers/search?city=Port Anitaburgh")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for city in response.json():
            self.assertEqual(city['City'], "Port Anitaburgh")
    
    def test_search_suppliers_country(self):
        response = self.client.get(f"suppliers/search?country=Czech Republic")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for country in response.json():
            self.assertEqual(country['Country'], "Czech Republic")
    
    def test_search_suppliers_code(self):
        response = self.client.get(f"suppliers/search?code=SUP0001")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for code in response.json():
            self.assertEqual(code['Code'], "SUP0001")
    
    def test_search_suppliers_reference(self):
        response = self.client.get(f"suppliers/search?reference=LPaJ-SUP0001")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for reference in response.json():
            self.assertEqual(reference['Reference'], "LPaJ-SUP0001")
        
    def test_search_suppliers_reference_and_name(self):
        response = self.client.get(f"suppliers/search?reference=LPaJ-SUP0001&name=Lee, Parks and Johnson")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for name in response.json():
            self.assertEqual(name['Reference'], "LPaJ-SUP0001")
            self.assertEqual(name['Name'], "Lee, Parks and Johnson")
    
    def test_search_suppliers_reference_and_city(self):
        response = self.client.get(f"suppliers/search?reference=LPaJ-SUP0001&city=Port Anitaburgh")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for city in response.json():
            self.assertEqual(city['Reference'], "LPaJ-SUP0001")
            self.assertEqual(city['City'], "Port Anitaburgh")
    
    def test_search_suppliers_reference_and_country(self):
        response = self.client.get(f"suppliers/search?reference=LPaJ-SUP0001&country=Czech Republic")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for country in response.json():
            self.assertEqual(country['Reference'], "LPaJ-SUP0001")
            self.assertEqual(country['Country'], "Czech Republic")

    # POST tests
    def test_4create_supplier(self):
        response = self.client.post("suppliers", json=self.new_supplier)
        self.assertEqual(response.status_code, 201)
        created_supplier = self.GetJsonData("suppliers")[-1]
        created_supplier.pop('Id')
        self.assertEqual(self.new_supplier, created_supplier)

    def test_5create_supplier_with_invalid_data(self):
        invalid_supplier = self.new_supplier.copy()
        invalid_supplier.pop("Name")  # Invalid because it has no Name
        response = self.client.post("suppliers", json=invalid_supplier)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_supplier, self.GetJsonData("suppliers"))

    # PUT tests
    def test_7update_existing_supplier(self):
        updated_supplier = {
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

        last_id = self.GetJsonData("suppliers")[-1]['Id']
        response = self.client.put(f"suppliers/{last_id}", content=json.dumps(updated_supplier), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        suppliers_data = self.GetJsonData("suppliers")
        updated_supplier_exists = any(
            supplier['Id'] == last_id and supplier['Name'] == updated_supplier['Name']
            for supplier in suppliers_data
        )
        self.assertTrue(updated_supplier_exists, "Updated supplier with matching Id and Name not found in the data")

    def test_8update_non_existent_supplier(self):
        non_existent_supplier = self.new_supplier.copy()
        response = self.client.put("suppliers/-1", content=json.dumps(non_existent_supplier), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_supplier, self.GetJsonData("suppliers"))

    def test_9update_supplier_with_invalid_data(self):
        invalid_supplier = self.new_supplier.copy()
        invalid_supplier.pop("Name")  # Invalid because it has no Name
        last_id = self.GetJsonData("suppliers")[-1]['Id']
        response = self.client.put(f"suppliers/{last_id}", content=json.dumps(invalid_supplier), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_supplier, self.GetJsonData("suppliers"))

    # DELETE tests
    def test_delete_supplier(self):
        last_id = self.GetJsonData("suppliers")[-1]['Id']
        response = self.client.delete(f"suppliers/{last_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_supplier, self.GetJsonData("suppliers"))

    def test_delete_non_existent_supplier(self):
        response = self.client.delete("suppliers/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    #ID auto increment

    def test_11supplier_ID_auto_increment_working(self):
        idless_supplier = self.new_supplier.copy()
        old_id = self.GetJsonData("suppliers")[-1]["Id"]
        response = self.client.post("suppliers", json=idless_supplier)
        self.assertEqual(response.status_code, 201)
        created_supplier = self.GetJsonData("suppliers")[-1]
        self.assertEqual(old_id + 1, created_supplier["Id"])
        self.assertEqual(idless_supplier["Name"], created_supplier["Name"])

        response = self.client.delete(f"suppliers/{created_supplier['Id']}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_supplier, self.GetJsonData("suppliers"))

if __name__ == '__main__':
    unittest.main()

