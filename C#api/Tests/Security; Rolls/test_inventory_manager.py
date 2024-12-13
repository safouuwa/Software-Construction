import httpx
import unittest
import os

class InventoryManagerApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "k1l2m3n4o5"})  # Inventory Manager API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")

    # 3 actions that they have the right to perform
        
    def test_GetSingleWarehouse(self):
        response = self.client.get("warehouses/1")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], 1)

    def test_PostItemLine(self):
        new_item_line = {
            "Id": 2000000,
            "Name": "New Item line",
            "Description": "Description of the new item line",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("item_lines", json=new_item_line)
        self.assertEqual(response.status_code, 201)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        
        response = self.client.delete(f"item_lines/{new_item_line['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "k1l2m3n4o5"

    def test_GetAllInventories(self):
        response = self.client.get("inventories")
        self.assertEqual(response.status_code, 200)

    # 3 actions that they do not have the right to perform

    def test_PostWarehouse(self):
        new_warehouse = {
            "Id": 0,
            "Code": "WAR001",
            "Name": "New Warehouse",
            "Address": "123 Storage St",
            "Zip": "12345",
            "City": "Storageville",
            "Province": "Storagestate",
            "Country": "Storageland",
            "Contact": {
                "Name": "John Doe",
                "Phone": "123-456-7890",
                "Email": "johndoe@example.com"
            },
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
        response = self.client.post("warehouses", json=new_warehouse)
        self.assertEqual(response.status_code, 401)

    def test_UpdateOrder(self):
        updated_order = {
            "Id": 1,
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
                {"Item_Id": "ITEM673", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
        response = self.client.put("orders/1", json=updated_order)
        self.assertEqual(response.status_code, 401)

    def test_DeleteClient(self):
        response = self.client.delete("clients/1")
        self.assertEqual(response.status_code, 401)

if __name__ == '__main__':
    unittest.main()