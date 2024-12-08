import httpx
import unittest
import json
import os
from datetime import datetime

class ApiTransfersTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New transfer to create in the POST tests
        cls.new_transfer = {
            "Id": 0,
            "Reference": "TRANS123",
            "Transfer_From": 1,  # Assuming warehouse ID 1
            "Transfer_To": 2,    # Assuming warehouse ID 2
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
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
    def test_1get_all_transfers(self):
        response = self.client.get("transfers")
        self.assertEqual(response.status_code, 200)

    def test_2get_transfer_by_id(self):
        response = self.client.get("transfers/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_get_transfer_items(self):
        response = self.client.get(f"transfers/1/items")
        self.assertEqual(response.status_code, 200)       

    def test_3get_non_existent_transfer(self):
        response = self.client.get("transfers/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests
    def test_4create_transfer(self):
        response = self.client.post("transfers", json=self.new_transfer)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_transfer, self.GetJsonData("transfers"))

    def test_5create_transfer_with_invalid_data(self):
        invalid_transfer = self.new_transfer.copy()
        invalid_transfer["Id"] = 1 # Invalid because Id has been taken already
        response = self.client.post("transfers", json=invalid_transfer)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_transfer, self.GetJsonData("transfers"))

    def test_6create_duplicate_transfer(self):
        duplicate_transfer = self.new_transfer.copy()
        response = self.client.post("transfers", json=duplicate_transfer)
        self.assertEqual(response.status_code, 400)

    # PUT tests
    def test_7update_existing_transfer(self):
        updated_transfer = {
            "Id": self.new_transfer['Id'],  # Keep the same Id
            "Reference": "TRANS124",  # Changed Reference
            "Transfer_From": 2,  # Changed Transfer_From
            "Transfer_To": 1,  # Changed Transfer_To
            "Transfer_Status": "Scheduled",
            "Created_At": self.new_transfer['Created_At'],  # Keep the same creation time
            "Updated_At": "2024-11-14T16:10:14.227318",  # New update time
            "Items": [
                {"Item_Id": "ITEM789", "Amount": 200},
                {"Item_Id": "ITEM101", "Amount": 150}
            ]
        }

        response = self.client.put(f"transfers/{self.new_transfer['Id']}", content=json.dumps(updated_transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        transfers_data = self.GetJsonData("transfers")
        updated_transfer_exists = any(
            transfer['Id'] == updated_transfer['Id'] and transfer['Reference'] == updated_transfer['Reference']
            for transfer in transfers_data
        )
        self.assertTrue(updated_transfer_exists, "Updated transfer with matching Id and Reference not found in the data")

    def test_8update_non_existent_transfer(self):
        non_existent_transfer = self.new_transfer.copy()
        non_existent_transfer["Id"] = -1
        response = self.client.put("transfers/-1", content=json.dumps(non_existent_transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_transfer, self.GetJsonData("transfers"))

    def test_9update_transfer_with_invalid_data(self):
        invalid_transfer = self.new_transfer.copy()
        invalid_transfer.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"transfers/{self.new_transfer['Id']}", content=json.dumps(invalid_transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_transfer, self.GetJsonData("transfers"))

    def test_update_transfer_when_id_in_data_and_id_in_route_differ(self):
        transfer = self.new_transfer.copy()
        transfer["Id"] = -1  # Different Id
        response = self.client.put("transfers/1", content=json.dumps(transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(transfer, self.GetJsonData("transfers"))

    #Status Retrieval
    def test_aget_transfer_status(self):
        response = self.client.get(f"transfers/{self.new_transfer['Id']}/status")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.text, self.new_transfer['Transfer_Status'])

    # DELETE tests
    def test_delete_transfer(self):
        response = self.client.delete(f"transfers/{self.new_transfer['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_transfer, self.GetJsonData("transfers"))

    def test_delete_non_existent_transfer(self):
        response = self.client.delete("transfers/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    #ID auto increment test

    def test_11transfer_ID_auto_increment_working(self):
        idless_transfer = self.new_transfer.copy()
        idless_transfer.pop("Id")
        old_id = self.GetJsonData("transfers")[-1].copy().pop("Id")
        response = self.client.post("transfers", json=idless_transfer)
        self.assertEqual(response.status_code, 201)
        potential_transfer = self.GetJsonData("transfers")[-1].copy()
        id = potential_transfer["Id"]
        potential_transfer.pop("Id")
        self.assertEqual(idless_transfer, potential_transfer)
        self.assertEqual(old_id+1, id) 

        response = self.client.delete(f"transfers/{id}")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(idless_transfer, self.GetJsonData("transfers"))

    def test_12transfer_ID_duplicate_creation_fails(self):
        new_transfer = self.new_transfer.copy()
        new_transfer.pop("Id")
        response = self.client.post("transfers", json=new_transfer)
        self.assertEqual(response.status_code, 201)
        created_transfer = self.GetJsonData("transfers")[-1]
        existing_id = created_transfer["Id"]

        duplicate_transfer = new_transfer.copy()
        duplicate_transfer["Id"] = existing_id
        transfers_after = self.GetJsonData("transfers")
        response = self.client.post("transfers", json=duplicate_transfer)

        self.assertEqual(response.status_code, 400)

        self.assertEqual(len(transfers_after), len(self.GetJsonData("transfers")))

        response = self.client.delete(f"transfers/{existing_id}")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_transfer, self.GetJsonData("transfers"))

if __name__ == '__main__':
    unittest.main()
