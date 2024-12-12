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
        created_transfer = self.GetJsonData("transfers")[-1]
        created_transfer.pop('Id')
        self.assertEqual(self.new_transfer, created_transfer)

    def test_5create_transfer_with_invalid_data(self):
        invalid_transfer = self.new_transfer.copy()
        invalid_transfer.pop("Reference")  # Invalid because it has no Reference
        response = self.client.post("transfers", json=invalid_transfer)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_transfer, self.GetJsonData("transfers"))

    # PUT tests
    def test_7update_existing_transfer(self):
        updated_transfer = self.new_transfer.copy()
        updated_transfer["Reference"] = "TRANS124"
        updated_transfer["Updated_At"] = datetime.now().isoformat()

        last_id = self.GetJsonData("transfers")[-1]['Id']
        response = self.client.put(f"transfers/{last_id}", content=json.dumps(updated_transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        transfers_data = self.GetJsonData("transfers")
        updated_transfer_exists = any(
            transfer['Id'] == last_id and transfer['Reference'] == updated_transfer['Reference']
            for transfer in transfers_data
        )
        self.assertTrue(updated_transfer_exists, "Updated transfer with matching Id and Reference not found in the data")

    def test_8update_non_existent_transfer(self):
        non_existent_transfer = self.new_transfer.copy()
        response = self.client.put("transfers/-1", content=json.dumps(non_existent_transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_transfer, self.GetJsonData("transfers"))

    def test_9update_transfer_with_invalid_data(self):
        invalid_transfer = self.new_transfer.copy()
        invalid_transfer.pop("Reference")  # Invalid because it has no Reference
        last_id = self.GetJsonData("transfers")[-1]['Id']
        response = self.client.put(f"transfers/{last_id}", content=json.dumps(invalid_transfer), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_transfer, self.GetJsonData("transfers"))

    #Status Retrieval
    def test_aget_transfer_status(self):
        last_id = self.GetJsonData("transfers")[-1]['Id']
        response = self.client.get(f"transfers/{last_id}/status")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.text, self.new_transfer['Transfer_Status'])

    # DELETE tests
    def test_delete_transfer(self):
        last_id = self.GetJsonData("transfers")[-1]['Id']
        response = self.client.delete(f"transfers/{last_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_transfer, self.GetJsonData("transfers"))

    def test_delete_non_existent_transfer(self):
        response = self.client.delete("transfers/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    #ID auto increment

    def test_11transfer_ID_auto_increment_working(self):
        idless_transfer = self.new_transfer.copy()
        old_id = self.GetJsonData("transfers")[-1]["Id"]
        response = self.client.post("transfers", json=idless_transfer)
        self.assertEqual(response.status_code, 201)
        created_transfer = self.GetJsonData("transfers")[-1]
        self.assertEqual(old_id + 1, created_transfer["Id"])
        self.assertEqual(idless_transfer["Reference"], created_transfer["Reference"])

        response = self.client.delete(f"transfers/{created_transfer['Id']}")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_transfer, self.GetJsonData("transfers"))

if __name__ == '__main__':
    unittest.main()

