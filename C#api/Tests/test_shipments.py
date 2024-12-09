import httpx
import unittest
import json
import os
from datetime import datetime

class ApiShipmentsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "a1b2c3d4e5"})
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")
        
        # New shipment to create in the POST tests
        cls.new_shipment = {
            "Id": 0,
            "Order_Id": 123,
            "Source_Id": 1,  # Assuming source location with ID 1
            "Order_Date": "2024-11-14T16:10:14.227318",
            "Request_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Type": "Standard",
            "Shipment_Status": "Pending",
            "Notes": "Urgent delivery",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 2,
            "Total_Package_Weight": 5.5,
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM001", "Amount": 10},
                {"Item_Id": "ITEM002", "Amount": 5}
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
    def test_1get_all_shipments(self):
        response = self.client.get("shipments")
        self.assertEqual(response.status_code, 200)

    def test_2get_shipment_by_id(self):
        response = self.client.get("shipments/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)
    
    def test_get_shipment_orders(self):
        shipment_id = 1
        response = self.client.get(f"shipments/1/orders")
        self.assertEqual(response.status_code, 200)   

    def test_get_shipment_items(self):
        shipment_id = 1
        response = self.client.get(f"shipments/1/items")
        self.assertEqual(response.status_code, 200)   

    def test_3get_non_existent_shipment(self):
        response = self.client.get("shipments/-1")
        self.assertEqual(response.status_code, 404)
    
    def test_search_shipments_by_order_id(self):
        response = self.client.get("shipments?order_id=1")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Order_Id'], 1)
        
    def test_search_shipments_by_shipment_status(self):
        response = self.client.get("shipments?shipment_status=Pending")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Shipment_Status'], "Pending")
        
    def test_search_shipments_by_shipment_date(self):
        response = self.client.get("shipments?shipment_date=2000-03-13")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Shipment_Date'], "2000-03-13")
    
    def test_search_shipments_by_request_date(self):
        response = self.client.get("shipments?request_date=2000-03-11")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Request_Date'], "2000-03-11")
    
    def test_search_shipments_by_carrier_code(self):
        response = self.client.get("shipments?carrier_code=DPD")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Carrier_Code'], "DPD")
    
    def test_search_shipments_by_source_id(self):
        response = self.client.get("shipments?source_id=33")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Source_Id'], 33)
    
    def test_search_shipments_by_order_id_and_shipment_status(self):
        response = self.client.get("shipments?order_id=1&shipment_status=Pending")
        self.assertEqual(response.status_code, 200)
        self.assertTrue(len(response.json()) > 0, response.json())
        self.assertEqual(response.json()[0]['Order_Id'], 1, "Pending")

    # POST tests
    def test_4create_shipment(self):
        response = self.client.post("shipments", json=self.new_shipment)
        self.assertEqual(response.status_code, 201)
        self.assertIn(self.new_shipment, self.GetJsonData("shipments"))

    def test_5create_shipment_with_invalid_data(self):
        invalid_shipment = self.new_shipment.copy()
        invalid_shipment.pop("Id")  # Invalid because it has no Id
        response = self.client.post("shipments", json=invalid_shipment)
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_shipment, self.GetJsonData("shipments"))

    def test_6create_duplicate_shipment(self):
        duplicate_shipment = self.new_shipment.copy()
        response = self.client.post("shipments", json=duplicate_shipment)
        self.assertEqual(response.status_code, 404)

    # PUT tests
    def test_7update_existing_shipment(self):
        updated_shipment = {
            "Id": self.new_shipment['Id'],  # Keep the same ID
            "Order_Id": self.new_shipment['Order_Id'],
            "Source_Id": self.new_shipment['Source_Id'],
            "Order_Date": self.new_shipment['Order_Date'],
            "Request_Date": self.new_shipment['Request_Date'],
            "Shipment_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Type": "Express",  # Changed shipment type
            "Shipment_Status": "Pending",  
            "Notes": "Updated delivery instructions",
            "Carrier_Code": "DHL",  # Changed carrier
            "Carrier_Description": "DHL Express",
            "Service_Code": "Express",
            "Payment_Type": "COD",
            "Transfer_Mode": "Road",
            "Total_Package_Count": 3,  # Changed package count
            "Total_Package_Weight": 7.5,  # Changed weight
            "Created_At": self.new_shipment['Created_At'],
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM001", "Amount": 15},  # Changed amount for ITEM001
                {"Item_Id": "ITEM003", "Amount": 2}  # Added new item
            ]
        }

        response = self.client.put(f"shipments/{self.new_shipment['Id']}", content=json.dumps(updated_shipment), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 200)

        shipments_data = self.GetJsonData("shipments")
        updated_shipment_exists = any(
            shipment['Id'] == updated_shipment['Id'] and shipment['Shipment_Status'] == updated_shipment['Shipment_Status']
            for shipment in shipments_data
        )
        self.assertTrue(updated_shipment_exists, "Updated shipment with matching Id and Status not found in the data")

    def test_8update_non_existent_shipment(self):
        non_existent_shipment = self.new_shipment.copy()
        non_existent_shipment["Id"] = -1
        response = self.client.put("shipments/-1", content=json.dumps(non_existent_shipment), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(non_existent_shipment, self.GetJsonData("shipments"))

    def test_9update_shipment_with_invalid_data(self):
        invalid_shipment = self.new_shipment.copy()
        invalid_shipment.pop("Id")  # Invalid because it has no Id
        response = self.client.put(f"shipments/{self.new_shipment['Id']}", content=json.dumps(invalid_shipment), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 400)
        self.assertNotIn(invalid_shipment, self.GetJsonData("shipments"))

    def test_update_shipment_when_id_in_data_and_id_in_route_differ(self):
        shipment = self.new_shipment.copy()
        shipment["Id"] = -1 
        response = self.client.put("shipments/1", content=json.dumps(shipment), headers={"Content-Type": "application/json"})
        self.assertEqual(response.status_code, 404)
        self.assertNotIn(shipment, self.GetJsonData("shipments"))

    #Status Retrieval
    def test_aget_shipment_status(self):
        response = self.client.get(f"shipments/{self.new_shipment['Id']}/status")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.text, self.new_shipment['Shipment_Status'])

    # DELETE tests
    def test_delete_shipment(self):
        response = self.client.delete(f"shipments/{self.new_shipment['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(self.new_shipment, self.GetJsonData("shipments"))

    def test_delete_non_existent_shipment(self):
        response = self.client.delete("shipments/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

if __name__ == '__main__':
    unittest.main()
