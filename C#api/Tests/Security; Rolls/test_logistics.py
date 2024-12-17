import httpx
import unittest
import json
import os

class LogisticsApiTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.base_url = "http://127.0.0.1:3000/api/v1/"
        cls.client = httpx.Client(base_url=cls.base_url, headers={"API_KEY": "j6k7l8m9n0"})  # Logistics API key
        cls.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "data").replace(os.sep, "/")

    @classmethod
    def GetJsonData(cls, model):
        with open(os.path.join(cls.data_root, f"{model}.json").replace("\\", "/"), 'r', encoding='utf-8') as file:
            data = json.load(file)
        return data
    # 3 actions that they have the right to perform
        
    def test_GetWarehouses(self):
        response = self.client.get("warehouses")
        self.assertEqual(response.status_code, 200)

    def test_PostItem(self):
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

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        
        response = self.client.delete(f"items/{self.GetJsonData('items')[-1]['Uid']}/force")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "j6k7l8m9n0"

    def test_UpdateOrder(self):
        updated_order = {
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
                {"Item_Id": "ITEM192", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }

        response = self.client.post("orders", json=updated_order)
        self.assertEqual(response.status_code, 201)

        updated_order = {
            "Source_Id": 1, 
            "Order_Date": "2024-11-14T16:10:14.227318",
            "Request_Date": "2024-11-14T16:10:14.227318",
            "Reference": "ORD123",
            "Reference_Extra": "Extra details here 2", #change
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
                {"Item_Id": "ITEM192", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
        response = self.client.put(f"orders/{self.GetJsonData('orders')[-1]['Id']}", json=updated_order)
        self.assertEqual(response.status_code, 200)

        self.client.headers["API_KEY"] = "a1b2c3d4e5"
        response = self.client.delete(f"orders/{self.GetJsonData('orders')[-1]['Id']}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.client.headers["API_KEY"] = "j6k7l8m9n0"
        

    # 3 actions that they do not have the right to perform

    def test_PostWarehouse(self):
        new_warehouse = {
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

    def test_GetTransfers(self):
        response = self.client.get("transfers")
        self.assertEqual(response.status_code, 401)

    def test_DeleteItemLine(self):
        response = self.client.delete("item_lines/1")
        self.assertEqual(response.status_code, 401)

if __name__ == '__main__':
    unittest.main()