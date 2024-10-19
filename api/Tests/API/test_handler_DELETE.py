import httpx
import json
import unittest
from models.clients import Clients
from models.inventories import Inventories
from models.item_groups import ItemGroups
from models.item_lines import ItemLines
from models.item_types import ItemTypes
from models.items import Items
from models.locations import Locations
from models.orders import Orders
from models.shipments import Shipments
from models.suppliers import Suppliers
from models.transfers import Transfers
from models.warehouses import Warehouses

BASE_URL = "http://localhost:3000/api/v1"

class TestApiHandlerDELETE(unittest.TestCase):

    def setUp(self):
        self.headers = {
        "API_KEY": "a1b2c3d4e5"
        }

    def test_delete_client(self):
        client_id = 1
        response = httpx.delete(f"{BASE_URL}/clients/{client_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200) 
        
        response = httpx.get(f"{BASE_URL}/clients/{client_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        client = json.loads(response.text)
        self.assertEqual(client, None)

    def test_delete_inventory(self):
        inventory_id = 1
        response = httpx.delete(f"{BASE_URL}/inventories/{inventory_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/inventories/{inventory_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        inventory = json.loads(response.text)
        self.assertEqual(inventory, None)

    def test_delete_item_group(self):
        item_group_id = 1
        response = httpx.delete(f"{BASE_URL}/item_groups/{item_group_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/item_groups/{item_group_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        item_group = json.loads(response.text)
        self.assertEqual(item_group, None)

    def test_delete_item_line(self):
        item_line_id = 1
        response = httpx.delete(f"{BASE_URL}/item_lines/{item_line_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/item_lines/{item_line_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        item_line = json.loads(response.text)
        self.assertEqual(item_line, None)

    def test_delete_item(self):
        item_id = "P000001"
        response = httpx.delete(f"{BASE_URL}/items/{item_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/items/{item_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        item = json.loads(response.text)
        self.assertEqual(item, None)

    def test_delete_location(self):
        location_id = 1
        response = httpx.delete(f"{BASE_URL}/locations/{location_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/locations/{location_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        location = json.loads(response.text)
        self.assertEqual(location, None)

    def test_delete_order(self):
        order_id = 1
        response = httpx.delete(f"{BASE_URL}/orders/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/orders/{order_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        order = json.loads(response.text)
        self.assertEqual(order, None)

    def test_delete_shipment(self):
        shipment_id = 1
        response = httpx.delete(f"{BASE_URL}/shipments/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/shipments/{shipment_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        shipment = json.loads(response.text)
        self.assertEqual(shipment, None)

    def test_delete_supplier(self):
        supplier_id = 1
        response = httpx.delete(f"{BASE_URL}/suppliers/{supplier_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/suppliers/{supplier_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        supplier = json.loads(response.text)
        self.assertEqual(supplier, None)

    def test_delete_transfer(self):
        transfer_id = 1
        response = httpx.delete(f"{BASE_URL}/transfers/{transfer_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/transfers/{transfer_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        transfer = json.loads(response.text)
        self.assertEqual(transfer, None)

    def test_delete_warehouse(self):
        warehouse_id = 1
        response = httpx.delete(f"{BASE_URL}/warehouses/{warehouse_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        
        response = httpx.get(f"{BASE_URL}/warehouses/{warehouse_id}", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        warehouse = json.loads(response.text)
        self.assertEqual(warehouse, None)


if __name__ == "__main__":
    unittest.main()
