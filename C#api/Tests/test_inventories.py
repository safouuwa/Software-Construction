import httpx
import unittest
import json
import os
from datetime import datetime

class ApiInventoriesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v2/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        cls.new_inventory = {
            "Item_Id": "ITEM001",
            "Description": "Test Item",
            "Item_Reference": "REF001",
            "Locations": [1, 2, 3],
            "Total_On_Hand": 100,
            "Total_Expected": 150,
            "Total_Ordered": 50,
            "Total_Allocated": 30,
            "Total_Available": 70,
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
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
    
    def test_1get_all_inventories(self):
        response = self.client.get("inventories")
        self.assertEqual(response.status_code, 200)

    def test_2get_inventory_by_id(self):
        response = self.client.get(f"inventories/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_inventory(self):
        response = self.client.get("inventories/-1")
        self.assertEqual(response.status_code, 204)

    def test_sort_order_in_inventories(self):
        response = self.client.get("inventories?page=1&pageSize=5&sortOrder=desc")
        self.assertEqual(response.status_code, 200)
        items = response.json()["Items"]
        ids = [item["Id"] for item in items]
        self.assertEqual(ids, sorted(ids, reverse=True), "Items are not sorted in descending order by Id")


    # POST tests
    
    def test_4create_inventory(self):
        response = self.client.post("inventories", json=self.new_inventory)
        self.assertEqual(response.status_code, 201)
        potential_inventory = self.GetJsonData("inventories")[-1].copy()
        potential_inventory.pop("Id")
        self.assertEqual(self.new_inventory, potential_inventory)

    def test_5create_inventory_with_invalid_data(self):
        invalid_inventory = self.new_inventory.copy()
        invalid_inventory["Id"] = 1 # Invalid because Id has been taken already
        response = self.client.post("inventories", json=invalid_inventory)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_inventory, self.GetJsonData("inventories"))


    # PUT tests
    
    def test_7update_existing_inventory(self):
        updated_inventory = {
            "Item_Id": "ITEM002",  # Changed
            "Description": "Updated Item",  # Changed
            "Item_Reference": "REF002",  # Changed
            "Locations": [4, 5, 6],  # Changed
            "Total_On_Hand": 120,  # Changed
            "Total_Expected": 180,  # Changed
            "Total_Ordered": 60,  # Changed
            "Total_Allocated": 40,  # Changed
            "Total_Available": 80,  # Changed
            "Created_At": self.new_inventory['Created_At'],  # Keep the same creation time
            "Updated_At": "2024-11-14T16:10:14.227318"  # New update time
        }
        created_inventory = self.GetJsonData("inventories")[-1]
        existing_id = created_inventory["Id"]
        
        response = self.client.put(f"inventories/{existing_id}", content=json.dumps(updated_inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)
        
        inventories_data = self.GetJsonData("inventories")
        updated_inventory_exists = any(
            inventory['Id'] == existing_id and inventory['Item_Id'] == updated_inventory['Item_Id']
            for inventory in inventories_data
        )
        self.assertTrue(updated_inventory_exists, "Updated inventory with matching Id and Item_Id not found in the data")

    def test_8update_non_existent_inventory(self):
        non_existent_inventory = self.new_inventory.copy()
        response = self.client.put("inventories/-1", content=json.dumps(non_existent_inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_inventory, self.GetJsonData("inventories"))

    def test_9update_inventory_with_invalid_data(self):
        invalid_inventory = self.new_inventory.copy()
        invalid_inventory.pop("Description")  # Invalid because it has no Description
        created_inventory = self.GetJsonData("inventories")[-1]
        existing_id = created_inventory["Id"]
        response = self.client.put(f"inventories/{existing_id}", content=json.dumps(invalid_inventory), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_inventory, self.GetJsonData("inventories"))  

    # PATCH tests
    def test_partially_update_non_existent_inventory(self):
        non_existent_inventory = self.new_inventory.copy()
        non_existent_inventory["Id"] = -1
        response = self.client.patch(
            "inventories/-1",
            content=json.dumps(non_existent_inventory),
            headers={"Content-Type": "application/json"}
        )
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_inventory, self.GetJsonData("inventories"))

    # DELETE tests
    
    def test_delete_inventory(self):
        created_inventory = self.GetJsonData("inventories")[-1]
        existing_id = created_inventory["Id"]
        response = self.client.delete(f"inventories/{existing_id}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_inventory, self.GetJsonData("inventories"))

    def test_delete_non_existent_inventory(self):
        response = self.client.delete("inventories/-1")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    #ID auto increment

    def test_11inventory_ID_auto_increment_working(self):
        idless_inventory = self.new_inventory.copy()
        old_id = self.GetJsonData("inventories")[-1].copy().pop("Id")
        response = self.client.post("inventories", json=idless_inventory)
        self.assertEqual(response.status_code, 201)
        potential_inventory = self.GetJsonData("inventories")[-1].copy()
        id = potential_inventory["Id"]
        potential_inventory.pop("Id")
        self.assertEqual(idless_inventory, potential_inventory)
        self.assertEqual(old_id+1, id) 

        response = self.client.delete(f"inventories/{id}/force")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(idless_inventory, self.GetJsonData("inventories"))

    # Item_Id & reference test

    def test_12inventory_Item_Id_and_reference_unique(self): #Non existent Item
        response = self.client.post("inventories", json=self.new_inventory)
        self.assertEqual(response.status_code, 201)
        
        created_inventory = self.GetJsonData("inventories")[-1]
        existing_id = created_inventory["Id"]
        response = self.client.delete(f"inventories/{existing_id}/force")

    def test_13inventory_Item_Id_and_reference_unique(self): #Existent Item
        inventory  = self.new_inventory.copy()
        new_item = {
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

        response = self.client.post("items", json=new_item)
        self.assertEqual(response.status_code, 201)
        item = self.GetJsonData("items")[-1]
        uid = item.pop("Uid")
        
        inventory["Item_Id"] = uid
        inventory["Item_Reference"] = item["Code"] + "1"

        response = self.client.post("inventories", json=inventory)
        self.assertEqual(response.status_code, 400)

        response = self.client.delete(f"items/{uid}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)

    def test_14update_inventory_with_mismatched_item_id_and_reference(self):
        new_item = {
            "Code": "CODE456",
            "Description": "This is another test item.",
            "Short_Description": "Test Item 2",
            "Upc_Code": "987654321098",
            "Model_Number": "MODEL456",
            "Commodity_Code": "COMMOD456",
            "Item_Line": 2,
            "Item_Group": 3,
            "Item_Type": 4,
            "Unit_Purchase_Quantity": 15,
            "Unit_Order_Quantity": 7,
            "Pack_Order_Quantity": 25,
            "Supplier_Id": 2,
            "Supplier_Code": "SUP456",
            "Supplier_Part_Number": "SUP456-PART002",
            "Created_At": "2024-11-15T10:00:00.000000",
            "Updated_At": "2024-11-15T10:00:00.000000"
        }
        response = self.client.post("items", json=new_item)
        self.assertEqual(response.status_code, 201)
        created_item = self.GetJsonData("items")[-1]
        item_uid = created_item["Uid"]

        inventory = self.new_inventory.copy()
        inventory["Item_Id"] = item_uid
        inventory["Item_Reference"] = created_item["Code"]
        response = self.client.post("inventories", json=inventory)
        self.assertEqual(response.status_code, 201)
        created_inventory = self.GetJsonData("inventories")[-1]
        inventory_id = created_inventory["Id"]

        updated_inventory = inventory.copy()
        updated_inventory["Item_Reference"] = "MISMATCH_REF"
        response = self.client.put(f"inventories/{inventory_id}", json=updated_inventory)
        self.assertEqual(response.status_code, 400)
        self.assertIn("Item ID and Reference do not refer to the same Item entity", response.text)

        self.client.delete(f"inventories/{inventory_id}/force")
        self.client.delete(f"items/{item_uid}/force")

    def test_15patch_inventory_with_mismatched_item_id_and_reference(self):
        new_item = {
            "Code": "CODE789",
            "Description": "This is a third test item.",
            "Short_Description": "Test Item 3",
            "Upc_Code": "135792468024",
            "Model_Number": "MODEL789",
            "Commodity_Code": "COMMOD789",
            "Item_Line": 3,
            "Item_Group": 4,
            "Item_Type": 5,
            "Unit_Purchase_Quantity": 20,
            "Unit_Order_Quantity": 10,
            "Pack_Order_Quantity": 30,
            "Supplier_Id": 3,
            "Supplier_Code": "SUP789",
            "Supplier_Part_Number": "SUP789-PART003",
            "Created_At": "2024-11-16T12:00:00.000000",
            "Updated_At": "2024-11-16T12:00:00.000000"
        }
        response = self.client.post("items", json=new_item)
        self.assertEqual(response.status_code, 201)
        created_item = self.GetJsonData("items")[-1]
        item_uid = created_item["Uid"]

        inventory = self.new_inventory.copy()
        inventory["Item_Id"] = item_uid
        inventory["Item_Reference"] = created_item["Code"]
        response = self.client.post("inventories", json=inventory)
        self.assertEqual(response.status_code, 201)
        created_inventory = self.GetJsonData("inventories")[-1]
        inventory_id = created_inventory["Id"]

        patch_data = {
            "Item_Id": item_uid,	
            "Item_Reference": "MISMATCH_REF"
        }
        response = self.client.patch(f"inventories/{inventory_id}", json=patch_data)
        self.assertEqual(response.status_code, 400)
        self.assertIn("Item ID and Reference do not refer to the same Item entity", response.text)

        self.client.delete(f"inventories/{inventory_id}/force")
        self.client.delete(f"items/{item_uid}/force")


if __name__ == '__main__':
    unittest.main()