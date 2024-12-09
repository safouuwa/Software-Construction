import json
import os
import unittest

import httpx


class ApiClientsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(
            os.path.dirname(
                os.path.dirname(
                    os.path.dirname(
                        os.path.abspath(__file__)
                    )
                )
            ),
            "data"
        ).replace(os.sep, "/")

        # Define the Client model
        cls.new_client = {
            "Id": 0,
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

        cls.test_methods = [method for method in dir(cls)
                            if method.startswith('test_')]
        cls.current_test_index = 0

    def setUp(self):
        current_method = self._testMethodName
        expected_method = self.test_methods[self.current_test_index]
        self.assertEqual(current_method, expected_method,
                         f"Tests are running out of order. "
                         f"Expected {expected_method},"
                         f" but running {current_method}")
        self.__class__.current_test_index += 1

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json"),
                  'r', encoding='utf-8') as file:
            data = json.load(file)
        return data

    # GET tests

    def test_1get_all_clients(self):
        response = self.client.get("clients")
        self.assertEqual(response.status_code, 200)

    def test_2get_client_by_id(self):
        response = self.client.get("clients/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_get_client_orders(self):
        response = self.client.get("clients/1/orders")
        self.assertEqual(response.status_code, 200)

    def test_3get_non_existent_client(self):
        response = self.client.get("clients/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests

    def test_4create_client(self):
        response = self.client.post("clients", json=self.new_client)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_client, self.GetJsonData("clients"))

    def test_5create_client_with_invalid_data(self):
        invalid_client = self.new_client.copy()
        invalid_client.pop("Id")  # Invalid because it has no Id
        response = self.client.post("clients", json=invalid_client)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_client, self.GetJsonData("clients"))

    def test_6create_duplicate_client(self):
        duplicate_client = self.new_client.copy()
        response = self.client.post("clients", json=duplicate_client)
        self.assertEqual(response.status_code, 404)

    # PUT tests

    def test_7update_existing_client(self):
        updated_client = {
            "Id": self.new_client['Id'],  # Keep the same ID
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

        response = self.client.put(f"clients/{self.new_client['Id']}",
                                   content=json.dumps(updated_client),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 200)

        clients_data = self.GetJsonData("clients")
        updated_client_exists = any(
            client['Id'] == updated_client['Id'] and
            client['Name'] == updated_client['Name']
            for client in clients_data
        )
        self.assertTrue(updated_client_exists,
                        "Updated client with matching Id and "
                        "Name not found in the data")

    def test_8update_non_existent_client(self):
        non_existent_client = self.new_client.copy()
        non_existent_client["Id"] = -1
        response = self.client.put("clients/-1",
                                   content=json.dumps(non_existent_client),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_client, self.GetJsonData("clients"))

    def test_9update_client_with_invalid_data(self):
        invalid_client = self.new_client.copy()
        invalid_client.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"clients/{self.new_client['Id']}",
                                   content=json.dumps(invalid_client),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_client, self.GetJsonData("clients"))

    def test_10partially_update_client(self):
        partial_update = {
            "Name": "Partially Updated Client",
            "Address": "789 Partially Updated St",
            "City": "Partially Updatedtown",
            "Zip_code": "54321",
            "Province": "Partially Updatedprovince",
            "Country": "Partially Updatedcountry",
            "Contact_name": "Jane Doe",
            "Contact_phone": "987-654-3210",
            "Contact_email": ""
        }

        response = self.client.put(f"clients/{self.new_client['Id']}",
                                   content=json.dumps(partial_update),
                                   headers={"Content-Type": "application/json"
                                            })
        self.assertEqual(response.status_code, 200)

        clients_data = self.GetJsonData("clients")
        partially_updated_client_exists = any(
            client['Id'] == self.new_client['Id'] and
            client['Name'] == partial_update['Name']
            for client in clients_data
        )
        self.assertTrue(partially_updated_client_exists,
                        "Partially updated client with matching Id and "
                        "Name not found in the data")

    # DELETE tests

    def test_delete_client(self):
        response = self.client.delete(f"clients/{self.new_client['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_client, self.GetJsonData("clients"))

    def test_delete_non_existent_client(self):
        response = self.client.delete("clients/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)


if __name__ == '__main__':
    unittest.main()
