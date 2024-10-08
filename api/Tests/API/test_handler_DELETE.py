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

    def test_delete_client(self):
        client_id = 1
        response = httpx.delete(f"{BASE_URL}/clients/{client_id}")
        self.assertEqual(response.status_code, 204) 
        
        response = httpx.get(f"{BASE_URL}/clients/{client_id}")
        self.assertEqual(response.status_code, 200)
        client = json.loads(response.text)
        self.assertEqual(client, "null")

    def test_delete_inventory(self):
        inventory_id = 1
        response = httpx.delete(f"{BASE_URL}/inventories/{inventory_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/inventories/{inventory_id}")
        self.assertEqual(response.status_code, 200)
        inventory = json.loads(response.text)
        self.assertEqual(inventory, "null")

    def test_delete_item_group(self):
        item_group_id = 1
        response = httpx.delete(f"{BASE_URL}/item_groups/{item_group_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/item_groups/{item_group_id}")
        self.assertEqual(response.status_code, 200)
        item_group = json.loads(response.text)
        self.assertEqual(item_group, "null")

    def test_delete_item_line(self):
        item_line_id = 1
        response = httpx.delete(f"{BASE_URL}/item_lines/{item_line_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/item_lines/{item_line_id}")
        self.assertEqual(response.status_code, 200)
        item_line = json.loads(response.text)
        self.assertEqual(item_line, "null")

    def test_delete_item(self):
        item_id = "P000001"
        response = httpx.delete(f"{BASE_URL}/items/{item_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/items/{item_id}")
        self.assertEqual(response.status_code, 200)
        item = json.loads(response.text)
        self.assertEqual(item, "null")

    def test_delete_location(self):
        location_id = 1
        response = httpx.delete(f"{BASE_URL}/locations/{location_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/locations/{location_id}")
        self.assertEqual(response.status_code, 200)
        location = json.loads(response.text)
        self.assertEqual(location, "null")

    def test_delete_order(self):
        order_id = 1
        response = httpx.delete(f"{BASE_URL}/orders/{order_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/orders/{order_id}")
        self.assertEqual(response.status_code, 200)
        order = json.loads(response.text)
        self.assertEqual(order, "null")

    def test_delete_shipment(self):
        shipment_id = 1
        response = httpx.delete(f"{BASE_URL}/shipments/{shipment_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/shipments/{shipment_id}")
        self.assertEqual(response.status_code, 200)
        shipment = json.loads(response.text)
        self.assertEqual(shipment, "null")

    def test_delete_supplier(self):
        supplier_id = 1
        response = httpx.delete(f"{BASE_URL}/suppliers/{supplier_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/suppliers/{supplier_id}")
        self.assertEqual(response.status_code, 200)
        supplier = json.loads(response.text)
        self.assertEqual(supplier, "null")

    def test_delete_transfer(self):
        transfer_id = 1
        response = httpx.delete(f"{BASE_URL}/transfers/{transfer_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/transfers/{transfer_id}")
        self.assertEqual(response.status_code, 200)
        transfer = json.loads(response.text)
        self.assertEqual(transfer, "null")

    def test_delete_warehouse(self):
        warehouse_id = 1
        response = httpx.delete(f"{BASE_URL}/warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, 204)
        
        response = httpx.get(f"{BASE_URL}/warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, 200)
        warehouse = json.loads(response.text)
        self.assertEqual(warehouse, "null")


if __name__ == "__main__":
    unittest.main()
