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
        self.assertEqual(response.status_code, 204)

    def test_get_item_locations(self):
        response = self.client.get("items/P000001/locations")
        self.assertEqual(response.status_code, 200)
    
    def test_search_items_by_code(self):
        response = self.client.get("items/search?code=sjQ23408K")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for code in response.json():
            self.assertEqual(code['Code'], "sjQ23408K")
    
    def test_search_items_by_upc_code(self):
        response = self.client.get("items/search?upccode=6523540947122")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Upc_Code'], "6523540947122")
    
    def test_search_items_by_commodity_code(self):
        response = self.client.get("items/search?commodityCode=oTo304")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Commodity_Code'], "oTo304")
    
    def test_search_items_by_supplier_code(self):
        response = self.client.get("items/search?suppliercode=SUP423")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Supplier_Code'], "SUP423")
    
    def test_search_items_with_invalid_parameter(self):
        response = self.client.get("items/search?invalid_param=invalid_value")
        self.assertEqual(response.status_code, 400)
        self.assertIn("At least one search parameter must be provided.", response.text)
    
    def test_search_items_with_valid_and_invalid_parameter(self):
        response = self.client.get("items/search?code=sjQ23408K&invalid_param=invalid_value")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Code'], "sjQ23408K")
    
    def test_search_items_by_code_and_supplier_code(self):
        response = self.client.get("items/search?code=sjQ23408K&supplierCode=SUP423")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Code'], "sjQ23408K")
            self.assertEqual(item['Supplier_Code'], "SUP423")
    
    def test_search_items_by_commodity_code_and_upc_code(self):
        response = self.client.get("items/search?commodityCode=p-69292-Xkv&upccode=8196931578335")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item ['Commodity_Code'], "p-69292-Xkv")
            self.assertEqual(item ['Upc_Code'], "8196931578335")
    
    def test_search_items_by_code_and_commodity_code(self):
        response = self.client.get("items/search?code=gVK34692I&commodityCode=p-69292-Xkv")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for item in response.json():
            self.assertEqual(item['Code'], "gVK34692I")
            self.assertEqual(item['Commodity_Code'], "p-69292-Xkv")

    def test_get_item_supplier(self):
        id = "P000006"
        response = self.client.get(f"items/{id}/supplier")
        self.assertEqual(response.status_code, 200)
        supplier = response.json()
        self.assertEqual(supplier['Id'], 69)
        self.assertEqual(supplier['Code'], "SUP0069")
        self.assertEqual(supplier['Name'], "Hunt, Chang and Parker")
        self.assertEqual(supplier['Address'], "37747 Cassandra Isle")

    def test_get_item_details_itemline(self):
        id = "P000014"
        detail_type = "itemline"
        response = self.client.get(f"items/{id}/{detail_type}")
        self.assertEqual(response.status_code, 200)
        detail = response.json()
        self.assertIsNotNone(detail)
        self.assertEqual(detail['Id'], 67)

    def test_get_item_details_itemgroup(self):
        id = "P000014"
        detail_type = "itemgroup"
        response = self.client.get(f"items/{id}/{detail_type}")
        self.assertEqual(response.status_code, 200)
        detail = response.json()
        self.assertIsNotNone(detail)
        self.assertEqual(detail['Id'], 31)

    def test_get_item_details_itemtype(self):
        id = "P000014"
        detail_type = "itemtype"
        response = self.client.get(f"items/{id}/{detail_type}")
        self.assertEqual(response.status_code, 200)
        detail = response.json()
        self.assertIsNotNone(detail)
        self.assertEqual(detail['Id'], 36)
    
    def test_get_item_supplier_invalid_id(self):
        response = self.client.get("items/ITEM999/supplier")
        self.assertEqual(response.status_code, 204)
    
    def test_get_item_itemline_invalid_id(self):
        response = self.client.get("items/ITEM999/itemline")
        self.assertEqual(response.status_code, 204)
    
    def test_get_item_itemgroup_invalid_id(self):
        response = self.client.get("items/ITEM999/itemgroup")
        self.assertEqual(response.status_code, 204)
    
    def test_get_item_itemtype_invalid_id(self):
        response = self.client.get("items/ITEM999/itemtype")
        self.assertEqual(response.status_code, 204)
        
    def test_sort_order_in_items(self):
        response = self.client.get("items?sortOrder=desc")
        self.assertEqual(response.status_code, 200)
        items = response.json()["Items"]
        ids = [item["Uid"] for item in items]
        self.assertEqual(ids, sorted(ids, reverse=True), "Items are not sorted in descending order by Uid")
        
    # POST tests
    def test_4create_item(self):
        response = self.client.post("items", json=self.new_item)
        self.assertEqual(response.status_code, 201)
        item = self.GetJsonData("items")[-1]
        item.pop("Uid")
        self.assertEqual(self.new_item, item)

    def test_5create_item_with_invalid_data(self):
        invalid_item = self.new_item.copy()
        invalid_item.pop("Code")  # Invalid because it has no Code
        response = self.client.post("items", json=invalid_item)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item, self.GetJsonData("items"))

    # PUT tests
    def test_7update_existing_item(self):
        updated_item = {
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

        response = self.client.put(f"items/{self.GetJsonData("items")[-1]['Uid']}", content=json.dumps(updated_item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        existing_id = self.GetJsonData("items")[-1]['Uid']
        items_data = self.GetJsonData("items")
        updated_item_exists = any(
            item['Uid'] == existing_id and item['Code'] == updated_item['Code']
            for item in items_data
        )
        self.assertTrue(updated_item_exists, "Updated item with matching Uid and Code not found in the data")

    def test_8update_non_existent_item(self):
        non_existent_item = self.new_item.copy()
        response = self.client.put("items/ITEM999", content=json.dumps(non_existent_item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_item, self.GetJsonData("items"))

    def test_9update_item_with_invalid_data(self):
        invalid_item = self.new_item.copy()
        invalid_item.pop("Code")  # Invalid because it has no Code
        response = self.client.put(f"items/{self.GetJsonData("items")[-1]['Uid']}", content=json.dumps(invalid_item), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_item, self.GetJsonData("items")) 

    # PATCH tests
    def test_partially_update_non_existent_item(self):
        non_existent_item = self.new_item.copy()
        non_existent_item["Uid"] = "ITEM999"
        response = self.client.patch(
            "items/ITEM999",
            content=json.dumps(non_existent_item),
            headers={"Content-Type": "application/json"}
        )
        self.assertEqual(response.status_code, 204)

    # DELETE tests
    def test_delete_item(self):
        response = self.client.delete(f"items/{self.GetJsonData("items")[-1]['Uid']}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_item, self.GetJsonData("items"))

    def test_delete_non_existent_item(self):
        response = self.client.delete("items/ITEM999")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    # Transfer history tests

    def test_get_item_transfer_history_non_existent_id(self):
        response = self.client.get("items/ITEM999/transfers")
        self.assertEqual(response.status_code, 204)

    def test_get_item_transfer_history(self):
        response = self.client.post("items", json=self.new_item)
        self.assertEqual(response.status_code, 201)
        item = self.GetJsonData("items")[-1]
        uid = item.pop("Uid")

        new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": uid, "Amount": 100}
            ]
        }
        response = self.client.post("transfers", json=new_transfer)
        firstid = self.GetJsonData("transfers")[-1].pop('Id')
        self.assertEqual(response.status_code, 201)

        new_transfer['Updated_At'] = "2020-11-14T16:10:14.227318"
        response = self.client.post("transfers", json=new_transfer)
        secondid = self.GetJsonData("transfers")[-1].pop('Id')
        self.assertEqual(response.status_code, 201)

        response = self.client.get(f"items/{uid}/transfers")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(len(response.json()), 2)
        self.assertTrue(response.json()[0]['Updated_At'] > response.json()[1]['Updated_At'])

        self.client.delete(f"transfers/{firstid}")
        self.client.delete(f"transfers/{secondid}")
        self.client.delete(f"items/{uid}/force")



if __name__ == '__main__':
    unittest.main()