import httpx
import unittest
import json
import os
from datetime import datetime

class ApiOrdersTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v2/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New order to create in the POST tests
        cls.new_order = {
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
        self.assertEqual(response.status_code, 204)
    
    def test_search_orders_by_source_id(self):
        response = self.client.get("orders/search?sourceid=33")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Source_Id'], 33)
    
    def test_search_orders_by_order_status(self):
        response = self.client.get("orders/search?orderstatus=Delivered")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Order_Status'], "Delivered")
    
    def test_search_orders_by_order_date(self):
        response = self.client.get("orders/search?orderdate=2020-05-02T23:13:56Z")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Order_Date'], "2020-05-02T23:13:56Z")

    def test_search_orders_by_warehouse_id(self):
        response = self.client.get("orders/search?warehouseid=18")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Warehouse_Id'], 18)
    
    def test_search_orders_with_invalid_parameter(self):
        response = self.client.get("orders/search?invalid_param=invalid_value")
        self.assertEqual(response.status_code, 400)
        self.assertIn("At least one search parameter must be provided.", response.text)
    
    def test_search_orders_with_valid_and_invalid_parameter(self):
        response = self.client.get("orders/search?warehouseid=18&invalid_param=invalid_value")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        for order in response.json():
            self.assertEqual(order['Warehouse_Id'], 18)

 
    def test_search_orders_by_order_status_and_order_date(self):
        response = self.client.get("orders/search?status=Delivered&orderDate=2020-05-02T23:13:56Z")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.status_code)
        for x in response.json():
            self.assertEqual(x['Order_Status'], "Delivered")
            self.assertEqual(x['Order_Date'], "2020-05-02T23:13:56Z")
          
    def test_search_orders_by_order_status_and_source_id(self):
        response = self.client.get("orders/search?orderStatus=Delivered&sourceid=33")
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
      
    def test_get_order_warehouse(self):
        order_id = 1
        response = self.client.get(f"orders/{order_id}/warehouse")
        self.assertEqual(response.status_code, 200)
        warehouse = response.json()
        self.assertIn("Id", warehouse)
        self.assertIn("Name", warehouse)
        self.assertIn("Address", warehouse)
    
    def test_get_order_warehouse_invalid_id(self):
        order_id = -1
        response = self.client.get(f"orders/{order_id}/warehouse")
        self.assertEqual(response.status_code, 204)
  
    def test_sort_orders_by_order_id(self):
        response = self.client.get("orders?sortOrder=desc&page=1&pageSize=10")
        self.assertEqual(response.status_code, 200)
        response_data = response.json()
        items = response_data.get("Items", [])  
        self.assertTrue(len(items) > 0, f"No orders found: {response_data}")
        ids = [order["Id"] for order in items]
        self.assertEqual(ids, sorted(ids, reverse=True))
        
    # POST tests
    def test_4create_order(self):
        response = self.client.post("orders", json=self.new_order)
        self.assertEqual(response.status_code, 201)
        created_order = self.GetJsonData("orders")[-1]
        created_order.pop('Id')
        status = created_order.pop('Order_Status')
        self.new_order.pop('Order_Status')
        self.assertEqual(self.new_order, created_order)
        self.assertEqual(status, "Open")

    def test_5create_order_with_invalid_data(self):
        invalid_order = self.new_order.copy()
        invalid_order.pop("Reference")  # Invalid because it has no Reference
        response = self.client.post("orders", json=invalid_order)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_order, self.GetJsonData("orders"))

    # PUT tests
    def test_7update_existing_order(self):
        updated_order = self.new_order.copy()
        updated_order["Reference"] = "ORD124"
        updated_order["Updated_At"] = datetime.now().isoformat()

        last_id = self.GetJsonData("orders")[-1]['Id']
        response = self.client.put(f"orders/{last_id}", content=json.dumps(updated_order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        orders_data = self.GetJsonData("orders")
        updated_order_exists = any(
            order['Id'] == last_id and order['Reference'] == updated_order['Reference'] and order['Order_Status'] == "Open"
            for order in orders_data
        )
        self.assertTrue(updated_order_exists, "Updated order with matching Id and Reference not found in the data")

    def test_8update_non_existent_order(self):
        non_existent_order = self.new_order.copy()
        response = self.client.put("orders/-1", content=json.dumps(non_existent_order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 204)
        self.assertNotIn(non_existent_order, self.GetJsonData("orders"))

    def test_9update_order_with_invalid_data(self):
        invalid_order = self.new_order.copy()
        invalid_order.pop("Reference")  # Invalid because it has no Reference
        last_id = self.GetJsonData("orders")[-1]['Id']
        response = self.client.put(f"orders/{last_id}", content=json.dumps(invalid_order), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_order, self.GetJsonData("orders"))

    #Status Retrieval
    def test_aget_order_status(self):
        last_id = self.GetJsonData("orders")[-1]['Id']
        response = self.client.get(f"orders/{last_id}/status")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.text, "Open")

    #patch tests
    def test_partial_update_non_existent_order(self):
        response = self.client.patch("orders/-1", json={"Order_Status": "Shipped"})
        self.assertEqual(response.status_code, 204)

    # DELETE tests
    def test_delete_order(self):
        last_id = self.GetJsonData("orders")[-1]['Id']
        response = self.client.delete(f"orders/{last_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_order, self.GetJsonData("orders"))

    def test_delete_non_existent_order(self):
        response = self.client.delete("orders/-1/")
        self.assertEqual(response.status_code, httpx.codes.BAD_REQUEST)

    #ID auto increment

    def test_11order_ID_auto_increment_working(self):
        idless_order = self.new_order.copy()
        old_id = self.GetJsonData("orders")[-1]["Id"]
        response = self.client.post("orders", json=idless_order)
        self.assertEqual(response.status_code, 201)
        created_order = self.GetJsonData("orders")[-1]
        self.assertEqual(old_id + 1, created_order["Id"])
        self.assertEqual(idless_order["Reference"], created_order["Reference"])

        response = self.client.delete(f"orders/{created_order['Id']}")
        self.assertEqual(response.status_code, 200)
        self.assertNotIn(created_order, self.GetJsonData("orders"))

if __name__ == '__main__':
    unittest.main()