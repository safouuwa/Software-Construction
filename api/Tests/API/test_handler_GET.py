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

class TestApiHandlerGET(unittest.TestCase):

    def setUp(self):
        self.data_provider = self.DataProvider()

    def test_get_clients(self):
        response = httpx.get(f"{BASE_URL}/clients")
        self.assertEqual(response.status_code, 200)
        clients = json.loads(response.text)
        self.assertEqual(Clients().data, clients)

    def test_get_client_by_id(self):
        client_id = 1
        response = httpx.get(f"{BASE_URL}/clients/{client_id}")
        self.assertEqual(response.status_code, 200)
        client = json.loads(response.text)
        self.assertEqual(client['id'], client_id)
        self.assertIn(client, Clients().data)

    def test_get_client_by_wrong_id(self):
        client_id = 0
        response = httpx.get(f"{BASE_URL}/clients/{client_id}")
        self.assertEqual(response.status_code, 200)
        client = json.loads(response.text)
        self.assertEqual(client, "null")  # Assuming API returns "null" for not found

    def test_get_inventories(self):
        response = httpx.get(f"{BASE_URL}/inventories")
        self.assertEqual(response.status_code, 200)
        inventories = json.loads(response.text)
        self.assertEqual(Inventories().data, inventories)

    def test_get_inventory_by_id(self):
        inventory_id = 1
        response = httpx.get(f"{BASE_URL}/inventories/{inventory_id}")
        self.assertEqual(response.status_code, 200)
        inventory = json.loads(response.text)
        self.assertEqual(inventory['id'], inventory_id)
        self.assertIn(inventory, Inventories().data)

    def test_get_inventory_by_wrong_id(self):
        inventory_id = 0
        response = httpx.get(f"{BASE_URL}/inventories/{inventory_id}")
        self.assertEqual(response.status_code, 200)
        inventory = json.loads(response.text)
        self.assertEqual(inventory, "null")

    def test_get_item_groups(self):
        response = httpx.get(f"{BASE_URL}/item_groups")
        self.assertEqual(response.status_code, 200)
        item_groups = json.loads(response.text)
        self.assertEqual(ItemGroups().data, item_groups)

    def test_get_item_group_by_id(self):
        item_group_id = 1
        response = httpx.get(f"{BASE_URL}/item_groups/{item_group_id}")
        self.assertEqual(response.status_code, 200)
        item_group = json.loads(response.text)
        self.assertEqual(item_group['id'], item_group_id)
        self.assertIn(item_group, ItemGroups().data)

    def test_get_item_group_by_wrong_id(self):
        item_group_id = 0
        response = httpx.get(f"{BASE_URL}/item_groups/{item_group_id}")
        self.assertEqual(response.status_code, 200)
        item_group = json.loads(response.text)
        self.assertEqual(item_group, "null")

    def test_get_item_types(self):
        response = httpx.get(f"{BASE_URL}/item_types")
        self.assertEqual(response.status_code, 200)
        item_types = json.loads(response.text)
        self.assertEqual(ItemTypes().data, item_types)

    def test_get_item_type_by_id(self):
        item_type_id = 1
        response = httpx.get(f"{BASE_URL}/item_types/{item_type_id}")
        self.assertEqual(response.status_code, 200)
        item_type = json.loads(response.text)
        self.assertEqual(item_type['id'], item_type_id)
        self.assertIn(item_type, ItemTypes().data)

    def test_get_item_type_by_wrong_id(self):
        item_type_id = 0
        response = httpx.get(f"{BASE_URL}/item_types/{item_type_id}")
        self.assertEqual(response.status_code, 200)
        item_type = json.loads(response.text)
        self.assertEqual(item_type, "null")

    def test_get_item_lines(self):
        response = httpx.get(f"{BASE_URL}/item_lines")
        self.assertEqual(response.status_code, 200)
        item_lines = json.loads(response.text)
        self.assertEqual(ItemLines().data, item_lines)

    def test_get_item_line_by_id(self):
        item_line_id = 1
        response = httpx.get(f"{BASE_URL}/item_lines/{item_line_id}")
        self.assertEqual(response.status_code, 200)
        item_line = json.loads(response.text)
        self.assertEqual(item_line['id'], item_line_id)
        self.assertIn(item_line, ItemLines().data)

    def test_get_item_line_by_wrong_id(self):
        item_line_id = 0
        response = httpx.get(f"{BASE_URL}/item_lines/{item_line_id}")
        self.assertEqual(response.status_code, 200)
        item_line = json.loads(response.text)
        self.assertEqual(item_line, "null")

    def test_get_items(self):
        response = httpx.get(f"{BASE_URL}/items")
        self.assertEqual(response.status_code, 200)
        items = json.loads(response.text)
        self.assertEqual(Items().data, items)

    def test_get_item_by_id(self):
        item_id = 1
        response = httpx.get(f"{BASE_URL}/items/{item_id}")
        self.assertEqual(response.status_code, 200)
        item = json.loads(response.text)
        self.assertEqual(item['id'], item_id)
        self.assertIn(item, Items().data)

    def test_get_item_by_wrong_id(self):
        item_id = 0
        response = httpx.get(f"{BASE_URL}/items/{item_id}")
        self.assertEqual(response.status_code, 200)
        item = json.loads(response.text)
        self.assertEqual(item, "null")

    def test_get_locations(self):
        response = httpx.get(f"{BASE_URL}/locations")
        self.assertEqual(response.status_code, 200)
        locations = json.loads(response.text)
        self.assertEqual(Locations().data, locations)

    def test_get_location_by_id(self):
        location_id = 1
        response = httpx.get(f"{BASE_URL}/locations/{location_id}")
        self.assertEqual(response.status_code, 200)
        location = json.loads(response.text)
        self.assertEqual(location['id'], location_id)
        self.assertIn(location, Locations().data)

    def test_get_location_by_wrong_id(self):
        location_id = 0
        response = httpx.get(f"{BASE_URL}/locations/{location_id}")
        self.assertEqual(response.status_code, 200)
        location = json.loads(response.text)
        self.assertEqual(location, "null")

    def test_get_orders(self):
        response = httpx.get(f"{BASE_URL}/orders")
        self.assertEqual(response.status_code, 200)
        orders = json.loads(response.text)
        self.assertEqual(Orders().data, orders)

    def test_get_order_by_id(self):
        order_id = 1
        response = httpx.get(f"{BASE_URL}/orders/{order_id}")
        self.assertEqual(response.status_code, 200)
        order = json.loads(response.text)
        self.assertEqual(order['id'], order_id)
        self.assertIn(order, Orders().data)

    def test_get_order_by_wrong_id(self):
        order_id = 0
        response = httpx.get(f"{BASE_URL}/orders/{order_id}")
        self.assertEqual(response.status_code, 200)
        order = json.loads(response.text)
        self.assertEqual(order, "null")

    def test_get_shipments(self):
        response = httpx.get(f"{BASE_URL}/shipments")
        self.assertEqual(response.status_code, 200)
        shipments = json.loads(response.text)
        self.assertEqual(Shipments().data, shipments)

    def test_get_shipment_by_id(self):
        shipment_id = 1
        response = httpx.get(f"{BASE_URL}/shipments/{shipment_id}")
        self.assertEqual(response.status_code, 200)
        shipment = json.loads(response.text)
        self.assertEqual(shipment['id'], shipment_id)
        self.assertIn(shipment, Shipments().data)

    def test_get_shipment_by_wrong_id(self):
        shipment_id = 0
        response = httpx.get(f"{BASE_URL}/shipments/{shipment_id}")
        self.assertEqual(response.status_code, 200)
        shipment = json.loads(response.text)
        self.assertEqual(shipment, "null")

    def test_get_suppliers(self):
        response = httpx.get(f"{BASE_URL}/suppliers")
        self.assertEqual(response.status_code, 200)
        suppliers = json.loads(response.text)
        self.assertEqual(Suppliers().data, suppliers)

    def test_get_supplier_by_id(self):
        supplier_id = 1
        response = httpx.get(f"{BASE_URL}/suppliers/{supplier_id}")
        self.assertEqual(response.status_code, 200)
        supplier = json.loads(response.text)
        self.assertEqual(supplier['id'], supplier_id)
        self.assertIn(supplier, Suppliers().data)

    def test_get_supplier_by_wrong_id(self):
        supplier_id = 0
        response = httpx.get(f"{BASE_URL}/suppliers/{supplier_id}")
        self.assertEqual(response.status_code, 200)
        supplier = json.loads(response.text)
        self.assertEqual(supplier, "null")

    def test_get_transfers(self):
        response = httpx.get(f"{BASE_URL}/transfers")
        self.assertEqual(response.status_code, 200)
        transfers = json.loads(response.text)
        self.assertEqual(Transfers().data, transfers)

    def test_get_transfer_by_id(self):
        transfer_id = 1
        response = httpx.get(f"{BASE_URL}/transfers/{transfer_id}")
        self.assertEqual(response.status_code, 200)
        transfer = json.loads(response.text)
        self.assertEqual(transfer['id'], transfer_id)
        self.assertIn(transfer, Transfers().data)

    def test_get_transfer_by_wrong_id(self):
        transfer_id = 0
        response = httpx.get(f"{BASE_URL}/transfers/{transfer_id}")
        self.assertEqual(response.status_code, 200)
        transfer = json.loads(response.text)
        self.assertEqual(transfer, "null")

    def test_get_warehouses(self):
        response = httpx.get(f"{BASE_URL}/warehouses")
        self.assertEqual(response.status_code, 200)
        warehouses = json.loads(response.text)
        self.assertEqual(Warehouses().data, warehouses)

    def test_get_warehouse_by_id(self):
        warehouse_id = 1
        response = httpx.get(f"{BASE_URL}/warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, 200)
        warehouse = json.loads(response.text)
        self.assertEqual(warehouse['id'], warehouse_id)
        self.assertIn(warehouse, Warehouses().data)

    def test_get_warehouse_by_wrong_id(self):
        warehouse_id = 0
        response = httpx.get(f"{BASE_URL}/warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, 200)
        warehouse = json.loads(response.text)
        self.assertEqual("null", warehouse)

if __name__ == "__main__":
    unittest.main()
