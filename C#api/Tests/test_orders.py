import httpx
import unittest
import json
import os
from datetime import datetime

class ApiOrdersTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New order to create in the POST tests
        cls.new_order = {
            "Id": 0,
            "Source_Id": 1,  # Example source ID
            "Order_Date": "2024-11-14T16:10:14.227318",
            "Request_Date": "2024-11-14T16:10:14.227318",
            "Reference": "ORD123",
            "Reference_Extra": "Extra details here",
            "Order_Status": "Pending",
            "Notes": "Order notes",
            "Shipping_Notes": "Shipping instructions",
            "Picking_Notes": "Picking instructions",
            "Warehouse_Id": 1,  # Assuming warehouse ID 1
            "Ship_To": 2,  # Assuming ship-to address ID 2
            "Bill_To": 3,  # Assuming bill-to address ID 3
            "Shipment_Id": 4,  # Assuming shipment ID 4
            "Total_Amount": 1000.00,
            "Total_Discount": 50.00,
            "Total_Tax": 100.00,
            "Total_Surcharge": 20.00,
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
    def test_1get_all_orders(self):
        response = self.client.get("orders")
        self.assertEqual(response.status_code, 200)

    def test_2get_order_by_id(self):
        response = self.client.get("orders/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_3get_non_existent_order(self):
        response = self.client.get("orders/-1")
        self.assertEqual(response.status_code, 404)

    # POST tests
    def test_4create_order(self):
        response = self.client.post("orders", json=self.new_order)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_order, self.GetJsonData("orders"))

    def test_5create_order_with_invalid_data(self):
        invalid_order = self.new_order.copy()
        invalid_order["Id"] = 1 # Invalid because Id has been taken already
        response = self.client.post("orders", json=invalid_order)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_order, self.GetJsonData("orders"))

    def test_6create_duplicate_order(self):
        duplicate_order = self.new_order.copy()
        response = self.client.post("orders", json=duplicate_order)
        self.assertEqual(response.status_code, 400)

    # PUT tests
    def test_7update_existing_order(self):
        updated_order = {
            "Id": 0,
            "Source_Id": 1, 
            "Order_Date": "2024-11-14T16:10:14.227318",
            "Request_Date": "2024-11-14T16:10:14.227318",
            "Reference": "ORD123",
            "Reference_Extra": "Extra details here",
            "Order_Status": "Pending",
            "Notes": "Order notes",
            "Shipping_Notes": "Shipping instructions",
            "Picking_Notes": "Picking instructions",
            "Warehouse_Id": 1,  
            "Total_Amount": 1000.00,
            "Total_Discount": 50.00,
            "Total_Tax": 100.00,
            "Total_Surcharge": 20.00,
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }

        response = self.client.put(f"orders/{self.new_order['Id']}", content=json.dumps(updated_order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        orders_data = self.GetJsonData("orders")
        updated_order_exists = any(
            order['Id'] == updated_order['Id'] and order['Reference'] == updated_order['Reference']
            for order in orders_data
        )
        self.assertTrue(updated_order_exists, "Updated order with matching Id and Reference not found in the data")

    def test_8update_non_existent_order(self):
        non_existent_order = self.new_order.copy()
        non_existent_order["Id"] = -1
        response = self.client.put("orders/-1", content=json.dumps(non_existent_order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_order, self.GetJsonData("orders"))

    def test_9update_order_with_invalid_data(self):
        invalid_order = self.new_order.copy()
        invalid_order.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"orders/{self.new_order['Id']}", content=json.dumps(invalid_order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_order, self.GetJsonData("orders"))

    def test_update_order_when_id_in_data_and_id_in_route_differ(self):
        order = self.new_order.copy()
        order["Id"] = -1  # Different Id
        response = self.client.put("orders/1", content=json.dumps(order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(order, self.GetJsonData("orders"))

    #Status Retrieval
    def test_aget_order_status(self):
        response = self.client.get(f"orders/{self.new_order['Id']}/status")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.text, self.new_order['Order_Status'])

    # DELETE tests
    def test_delete_order(self):
        response = self.client.delete(f"orders/{self.new_order['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_order, self.GetJsonData("orders"))

    def test_delete_non_existent_order(self):
        response = self.client.delete("orders/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    #ID auto increment

    def test_11order_ID_auto_increment_working(self):
        idless_order = self.new_order.copy()
        idless_order.pop("Id")
        old_id = self.GetJsonData("orders")[-1].copy().pop("Id")
        response = self.client.post("orders", json=idless_order)
        self.assertEqual(response.status_code, 201)
        potential_order = self.GetJsonData("orders")[-1].copy()
        id = potential_order["Id"]
        potential_order.pop("Id")
        self.assertEqual(idless_order, potential_order)
        self.assertEqual(old_id+1, id) 

        response = self.client.delete(f"orders/{id}")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(idless_order, self.GetJsonData("orders"))

    
    def test_12order_ID_duplicate_creation_fails(self):
        new_order = self.new_order.copy()
        new_order.pop("Id")
        response = self.client.post("orders", json=new_order)
        self.assertEqual(response.status_code, 201)
        created_order = self.GetJsonData("orders")[-1]
        existing_id = created_order["Id"]

        duplicate_order = new_order.copy()
        duplicate_order["Id"] = existing_id
        orders_after = self.GetJsonData("orders")
        response = self.client.post("orders", json=duplicate_order)

        self.assertEqual(response.status_code, 400)

        self.assertEqual(len(orders_after), len(self.GetJsonData("orders")))

        response = self.client.delete(f"orders/{existing_id}")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_order, self.GetJsonData("orders"))

if __name__ == '__main__':
    unittest.main()
