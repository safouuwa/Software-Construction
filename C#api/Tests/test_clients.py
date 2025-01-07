import httpx
import unittest
import json
import os
from datetime import datetime

class ApiClientsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # Define the Client model
        cls.new_client = {
            "Name": "New Client",
            "Address": "123 Main St",
            "City": "Anytown",
            "Zip_code": "12345",
            "Province": "Province",
            "Country": "Country",
            "Contact_name": "John Doe",
            "Contact_phone": "123-456-7890",
            "Contact_email": "johndoe@example.com",
            "Created_at": "2024-11-14T16:10:14.227318",
            "Updated_at": "2024-11-14T16:10:14.227318"
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
        with open(os.path.join(cls.data_root, f"{model}.json"), 'r', encoding='utf-8') as file:
            data = json.load(file)
        return data

    
    # GET tests

    def test_1get_all_clients(self):
        response = self.client.get("clients")
        self.assertEqual(response.status_code, 200)

    def test_2get_client_by_id(self):
        response = self.client.get(f"clients/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)
    
    def test_get_client_orders(self):
        client_id = 1
        response = self.client.get(f"clients/1/orders")
        self.assertEqual(response.status_code, 200)      

    def test_3get_non_existent_client(self):
        response = self.client.get("clients/-1")
        self.assertEqual(response.status_code, 204)
    
    def test_search_by_name(self):
        response = self.client.get(f"clients/search?name=Raymond Inc")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json()[0])
        for name in response.json():
            self.assertEqual(name['Name'], "Raymond Inc")
    
    def test_search_by_city(self):
        response = self.client.get(f"clients/search?city=Pierceview")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json()[0])
        for city in response.json():
            if self.assertEqual(city['City'], "Pierceview"):
                return city
            else:
                return False
                
    def test_search_by_country(self):
        response = self.client.get(f"clients/search?country=United States")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for country in response.json():
            self.assertEqual(country['Country'], "United States")
    
    def test_search_by_name_and_city(self):
        response = self.client.get(f"clients/search?name=Raymond Inc&city=Pierceview")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Name'], "Raymond Inc")
            self.assertEqual(x['City'], "Pierceview")
    
    def test_search_by_name_and_country(self):
        response = self.client.get(f"clients/search?name=Raymond Inc&country=United States")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Name'], "Raymond Inc")
            self.assertEqual(x['Country'], "United States")
    
    def test_search_by_city_and_country(self):
        response = self.client.get(f"clients/search?city=Pierceview&country=United States")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['City'], "Pierceview")
            self.assertEqual(x['Country'], "United States")
    
    def test_search_by_name_and_city_and_country(self):
        response = self.client.get(f"clients/search?name=Raymond Inc&city=Pierceview&country=United States")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Name'], "Raymond Inc")
            self.assertEqual(x['City'], "Pierceview")
            self.assertEqual(x['Country'], "United States")
    
    # POST tests

    def test_4create_client(self):
        response = self.client.post("clients", json=self.new_client)
        self.assertEqual(response.status_code, 201)
        potential_client = self.GetJsonData("clients")[-1].copy()
        potential_client.pop("Id")
        self.assertEqual(self.new_client, potential_client)

    def test_5create_client_with_invalid_data(self):
        invalid_client = self.new_client.copy()
        invalid_client.pop("Name")  # Invalid because it has no Name
        response = self.client.post("clients", json=invalid_client)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_client, self.GetJsonData("clients"))


    # PUT tests

    def test_7update_existing_client(self):
        updated_client = {
            "Name": "Updated Client",  # Changed
            "Address": "456 Updated St",  # Changed
            "City": "Updatedtown",  # Changed
            "Zip_code": "54321",  # Changed
            "Province": "Updatedprovince",  # Changed
            "Country": "Updatedcountry",  # Changed
            "Contact_name": "Jane Doe",  # Changed
            "Contact_phone": "987-654-3210",  # Changed
            "Contact_email": "janedoe@example.com",  # Changed
        }
        created_client = self.GetJsonData("clients")[-1]
        existing_id = created_client["Id"]

        response = self.client.put(f"clients/{existing_id}", content=json.dumps(updated_client), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        clients_data = self.GetJsonData("clients")
        updated_client_exists = any(
            client['Id'] == existing_id and client['Name'] == updated_client['Name']
            for client in clients_data
        )
        self.assertTrue(updated_client_exists, "Updated client with matching Id and Name not found in the data")

    def test_8update_non_existent_client(self):
        non_existent_client = self.new_client.copy()
        response = self.client.put("clients/-1", content=json.dumps(non_existent_client), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_client, self.GetJsonData("clients"))

    def test_9update_client_with_invalid_data(self):
        invalid_client = self.new_client.copy()
        invalid_client.pop("Name")  # Invalid because it has no Name
        created_client = self.GetJsonData("clients")[-1]
        existing_id = created_client["Id"]
        response = self.client.put(f"clients/{existing_id}", content=json.dumps(invalid_client), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_client, self.GetJsonData("clients"))

# patch tests
    def test_11partially_update_non_existent_client(self):
        non_existent_client = self.new_client.copy()
        non_existent_client["Id"] = -1
        response = self.client.patch(
            "clients/-1",
            content=json.dumps(non_existent_client),
            headers={"Content-Type": "application/json"}
        )
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_client, self.GetJsonData("clients"))

    # DELETE tests

    def test_delete_client(self):
        created_client = self.GetJsonData("clients")[-1]
        existing_id = created_client["Id"]
        response = self.client.delete(f"clients/{existing_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_client, self.GetJsonData("clients"))

    def test_delete_non_existent_client(self):
        response = self.client.delete("clients/-1")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    # ID Auto Increment tests

    def test_11ID_auto_increment_working(self):
        idless_client = self.new_client.copy()
        old_id = self.GetJsonData("clients")[-1].copy().pop("Id")
        response = self.client.post("clients", json=idless_client)
        self.assertEqual(response.status_code, 201)
        potential_client = self.GetJsonData("clients")[-1].copy()
        id = potential_client["Id"]
        potential_client.pop("Id")
        self.assertEqual(idless_client, potential_client)
        self.assertEqual(old_id+1, id) 

        response = self.client.delete(f"clients/{id}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(idless_client, self.GetJsonData("clients"))




if __name__ == '__main__':
    unittest.main() 
