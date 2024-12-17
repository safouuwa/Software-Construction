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
    
    def test_search_orders_by_reference(self):
        response = self.client.get("orders/search?reference=ORD00001")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for reference in response.json():
            self.assertEqual(reference['Reference'], "ORD00001")
    
    def test_search_orders_by_status(self):
        response = self.client.get("orders/search?orderstatus=Delivered")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Order_Status'], "Delivered")
    
    def test_search_orders_by_source_id(self):
        response = self.client.get("orders/search?sourceid=33")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Source_Id'], 33)
            
    def test_search_orders_by_warehouse_id(self):
        response = self.client.get("orders/search?warehouseid=18")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Warehouse_Id'], 18)
    
    def test_search_orders_by_ship_to(self):
        response = self.client.get("orders/search?shipto=4562")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Ship_To'], 4562)
    
    def test_search_orders_by_bill_to(self):
        response = self.client.get("orders/search?billto=7863")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Bill_To'], 7863)
    
    def test_search_orders_by_shipment_id(self):
        response = self.client.get("orders/search?shipmentid=1")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Shipment_Id'], 1)
    
    def test_search_orders_by_status_and_reference(self):
        response = self.client.get("orders/search?status=Delivered&reference=ORD00001")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.status_code)
        for x in response.json():
            self.assertEqual(x['Order_Status'], "Delivered")
            self.assertEqual(x['Reference'], "ORD00001")
          
    def test_search_orders_by_status_and_source_id(self):
        response = self.client.get("orders/search?status=Delivered&sourceid=33")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Order_Status'], "Delivered")
            self.assertEqual(x['Source_Id'], 33)
            
    def test_search_orders_by_status_and_warehouse_id(self):
        response = self.client.get("orders/search?status=Delivered&warehouseid=18")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for x in response.json():
            self.assertEqual(x['Order_Status'], "Delivered")
            self.assertEqual(x['Warehouse_Id'], 18)
            
    # POST tests
    def test_4create_order(self):
        response = self.client.post("orders", json=self.new_order)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_order, self.GetJsonData("orders"))

    def test_5create_order_with_invalid_data(self):
        invalid_order = self.new_order.copy()
        invalid_order.pop("Id")  # Invalid because it has no Id
        response = self.client.post("orders", json=invalid_order)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_order, self.GetJsonData("orders"))

    def test_6create_duplicate_order(self):
        duplicate_order = self.new_order.copy()
        response = self.client.post("orders", json=duplicate_order)
        self.assertEqual(response.status_code, 404)

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

    #patch tests
    def test_partial_update_non_existent_order(self):
        response = self.client.patch("orders/-1", json={"Order_Status": "Shipped"})
        self.assertEqual(response.status_code, 404)

    # DELETE tests
    def test_delete_order(self):
        response = self.client.delete(f"orders/{self.new_order['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_order, self.GetJsonData("orders"))

    def test_delete_non_existent_order(self):
        response = self.client.delete("orders/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()