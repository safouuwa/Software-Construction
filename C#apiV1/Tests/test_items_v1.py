import httpx
import unittest
import json
import os
from datetime import datetime

class ApiItemsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New item to create in the POST tests
        cls.new_item = {
            "Uid": "ITEM123",
            "Code": "CODE123",
            "Description": "This is a test item.",
            "Short_Description": "Test Item",
            "Upc_Code": "123456789012",
            "Model_Number": "MODEL123",
            "Commodity_Code": "COMMOD123",
            "Item_Line": 1,
            "Item_Group": 2,
            "Item_Type": 3,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 20,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP123",
            "Supplier_Part_Number": "SUP123-PART001",
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
    def test_1get_all_items(self):
        response = self.client.get("items")
        self.assertEqual(response.status_code, 200)

    def test_2get_item_by_uid(self):
        response = self.client.get("items/P000001")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Uid'], "P000001")

    def test_3get_non_existent_item(self):
        response = self.client.get("items/ITEM999")
        self.assertEqual(response.status_code, 404)

    # POST tests
    def test_4create_item(self):
        response = self.client.post("items", json=self.new_item)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_item, self.GetJsonData("items"))

    def test_5create_item_with_invalid_data(self):
        invalid_item = self.new_item.copy()
        invalid_item.pop("Uid")  # Invalid because it has no Uid
        response = self.client.post("items", json=invalid_item)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item, self.GetJsonData("items"))

    def test_6create_duplicate_item(self):
        duplicate_item = self.new_item.copy()
        response = self.client.post("items", json=duplicate_item)
        self.assertEqual(response.status_code, 404)

    # PUT tests
    def test_7update_existing_item(self):
        updated_item = {
            "Uid": self.new_item['Uid'],  # Keep the same Uid
            "Code": "CODE124",  # Changed code
            "Description": "Updated test item.",  # Changed description
            "Short_Description": "Updated Test Item",  # Changed short description
            "Upc_Code": "123456789013",  # Changed UPC code
            "Model_Number": "MODEL124",  # Changed model number
            "Commodity_Code": "COMMOD124",  # Changed commodity code
            "Item_Line": 2,  # Changed item line
            "Item_Group": 3,  # Changed item group
            "Item_Type": 4,  # Changed item type
            "Unit_Purchase_Quantity": 20,  # Changed unit purchase quantity
            "Unit_Order_Quantity": 10,  # Changed unit order quantity
            "Pack_Order_Quantity": 30,  # Changed pack order quantity
            "Supplier_Id": 2,  # Changed supplier id
            "Supplier_Code": "SUP124",  # Changed supplier code
            "Supplier_Part_Number": "SUP124-PART002",  # Changed supplier part number
            "Created_At": self.new_item['Created_At'],  # Keep the same creation time
            "Updated_At": "2024-11-14T16:10:14.227318"  # New update time
        }

        response = self.client.put(f"items/{self.new_item['Uid']}", content=json.dumps(updated_item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        items_data = self.GetJsonData("items")
        updated_item_exists = any(
            item['Uid'] == updated_item['Uid'] and item['Code'] == updated_item['Code']
            for item in items_data
        )
        self.assertTrue(updated_item_exists, "Updated item with matching Uid and Code not found in the data")

    def test_8update_non_existent_item(self):
        non_existent_item = self.new_item.copy()
        non_existent_item["Uid"] = "ITEM999"
        response = self.client.put("items/ITEM999", content=json.dumps(non_existent_item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_item, self.GetJsonData("items"))

    def test_9update_item_with_invalid_data(self):
        invalid_item = self.new_item.copy()
        invalid_item.pop("Uid")  # Invalid because it has no Uid
        response = self.client.put(f"items/{self.new_item['Uid']}", content=json.dumps(invalid_item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item, self.GetJsonData("items"))

    def test_update_item_when_uid_in_data_and_uid_in_route_differ(self):
        item = self.new_item.copy()
        item["Uid"] = "ITEM999"  # Different UID
        response = self.client.put("items/ITEM123", content=json.dumps(item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(item, self.GetJsonData("items"))

    # DELETE tests
    def test_delete_item(self):
        response = self.client.delete(f"items/{self.new_item['Uid']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_item, self.GetJsonData("items"))

    def test_delete_non_existent_item(self):
        response = self.client.delete("items/ITEM999")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()
