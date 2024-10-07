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

class TestApiHandlerPOST(unittest.TestCase):

    def setUp(self):
        self.data_provider = self.DataProvider()

    def test_create_client(self):
        new_client = {"id": 0, "name": "Raymond Inc", "address": "1296 Daniel Road Apt. 349", "city": "Pierceview", "zip_code": "28301", "province": "Colorado", "country": "United States", "contact_name": "Bryan Clark", "contact_phone": "242.732.3483x2573", "contact_email": "robertcharles@example.net", "created_at": "2010-04-28 02:22:53", "updated_at": "2022-02-09 20:22:35"}
        response = httpx.post(f"{BASE_URL}/clients", json=new_client)
        self.assertEqual(response.status_code, 201)
        
        response = httpx.get(f"{BASE_URL}/clients/{new_client['id']}")
        self.assertEqual(response.status_code, 200)
        created_client = json.loads(response.text)
        self.assertEqual(created_client['id'], new_client['id'])

    def test_create_inventory(self):
        new_inventory = {"id": 0, "item_id": "PD000000", "description": "Face-to-face clear-thinking complexity", "item_reference": "sjQ23408K", "locations": [3211, 24700, 14123, 19538, 31071, 24701, 11606, 11817], "total_on_hand": 262, "total_expected": 0, "total_ordered": 80, "total_allocated": 41, "total_available": 141, "created_at": "2015-02-19 16:08:24", "updated_at": "2015-09-26 06:37:56"}
        response = httpx.post(f"{BASE_URL}/inventories", json=new_inventory)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/inventories/{new_inventory['id']}")
        self.assertEqual(response.status_code, 200)
        created_inventory = json.loads(response.text)
        self.assertEqual(created_inventory['id'], new_inventory['id'])

    def test_create_item(self):
        new_item = {
        "uid": "test",
        "code": "sjQ23408K",
        "description": "Face-to-face clear-thinking complexity",
        "short_description": "must",
        "upc_code": "6523540947122",
        "model_number": "63-OFFTq0T",
        "commodity_code": "oTo304",
        "item_line": 11,
        "item_group": 73,
        "item_type": 14,
        "unit_purchase_quantity": 47,
        "unit_order_quantity": 13,
        "pack_order_quantity": 11,
        "supplier_id": 34,
        "supplier_code": "SUP423",
        "supplier_part_number": "E-86805-uTM",
        "created_at": "2015-02-19 16:08:24",
        "updated_at": "2015-09-26 06:37:56"
        }
        response = httpx.post(f"{BASE_URL}/items", json=new_item)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/items/{new_item['id']}")
        self.assertEqual(response.status_code, 200)
        created_item = json.loads(response.text)
        self.assertEqual(created_item['id'], new_item['id'])

    def test_create_location(self):
        new_location = {"id": 0, "warehouse_id": 1, "code": "test", "name": "Row: A, Rack: 1, Shelf: 0", "created_at": "1992-05-15 03:21:32", "updated_at": "1992-05-15 03:21:32"}
        response = httpx.post(f"{BASE_URL}/locations", json=new_location)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/locations/{new_location['id']}")
        self.assertEqual(response.status_code, 200)
        created_location = json.loads(response.text)
        self.assertEqual(created_location['id'], new_location['id'])

    def test_create_order(self):
        new_order = {
        "id": 0,
        "source_id": 52,
        "order_date": "1983-09-26T19:06:08Z",
        "request_date": "1983-09-30T19:06:08Z",
        "reference": "ORD00003",
        "reference_extra": "test",
        "order_status": "Delivered",
        "notes": "Zeil hoeveel onze map sex ding.",
        "shipping_notes": "Ontvangen schoon voorzichtig instrument ster vijver kunnen raam.",
        "picking_notes": "Grof geven politie suiker bodem zuid.",
        "warehouse_id": 11,
        "ship_to": null,
        "bill_to": null,
        "shipment_id": 3,
        "total_amount": 1156.14,
        "total_discount": 420.45,
        "total_tax": 677.42,
        "total_surcharge": 86.03,
        "created_at": "1983-09-26T19:06:08Z",
        "updated_at": "1983-09-28T15:06:08Z",
        "items": [
            {
                "item_id": "P010669",
                "amount": 16
            }
        ]
    }
        response = httpx.post(f"{BASE_URL}/orders", json=new_order)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/orders/{new_order['id']}")
        self.assertEqual(response.status_code, 200)
        created_order = json.loads(response.text)
        self.assertEqual(created_order['id'], new_order['id'])

    def test_create_shipment(self):
        new_shipment = {
        "id": 0,
        "order_id": 3,
        "source_id": 52,
        "order_date": "1973-01-28",
        "request_date": "1973-01-30",
        "shipment_date": "1973-02-01",
        "shipment_type": "I",
        "shipment_status": "Pending",
        "notes": "test",
        "carrier_code": "DHL",
        "carrier_description": "DHL Express",
        "service_code": "NextDay",
        "payment_type": "Automatic",
        "transfer_mode": "Ground",
        "total_package_count": 29,
        "total_package_weight": 463.0,
        "created_at": "1973-01-28T20:09:11Z",
        "updated_at": "1973-01-29T22:09:11Z",
        "items": [
            {
                "item_id": "P010669",
                "amount": 16
            }
        ]
    }
        response = httpx.post(f"{BASE_URL}/shipments", json=new_shipment)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/shipments/{new_shipment['id']}")
        self.assertEqual(response.status_code, 200)
        created_shipment = json.loads(response.text)
        self.assertEqual(created_shipment['id'], new_shipment['id'])

    def test_create_supplier(self):
        new_supplier = {"id": 0, "code": "SUP0001", "name": "test", "address": "5989 Sullivan Drives", "address_extra": "Apt. 996", "city": "Port Anitaburgh", "zip_code": "91688", "province": "Illinois", "country": "Czech Republic", "contact_name": "Toni Barnett", "phonenumber": "363.541.7282x36825", "reference": "LPaJ-SUP0001", "created_at": "1971-10-20 18:06:17", "updated_at": "1985-06-08 00:13:46"}
        response = httpx.post(f"{BASE_URL}/suppliers", json=new_supplier)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/suppliers/{new_supplier['id']}")
        self.assertEqual(response.status_code, 200)
        created_supplier = json.loads(response.text)
        self.assertEqual(created_supplier['id'], new_supplier['id'])

    def test_create_transfer(self):
        new_transfer = {
        "id": 0,
        "reference": "TEST00001",
        "transfer_from": null,
        "transfer_to": 9229,
        "transfer_status": "Completed",
        "created_at": "2000-03-11T13:11:14Z",
        "updated_at": "2000-03-12T16:11:14Z",
        "items": [
            {
                "item_id": "P007435",
                "amount": 23
            }
        ]
    }
        response = httpx.post(f"{BASE_URL}/transfers", json=new_transfer)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/transfers/{new_transfer['id']}")
        self.assertEqual(response.status_code, 200)
        created_transfer = json.loads(response.text)
        self.assertEqual(created_transfer['id'], new_transfer['id'])

    def test_create_warehouse(self):
        new_warehouse = {"id": 0, "code": "YQZZNL56", "name": "TEST cargo hub", "address": "Karlijndreef 281", "zip": "4002 AS", "city": "Heemskerk", "province": "Friesland", "country": "NL", "contact": {"name": "Fem Keijzer", "phone": "(078) 0013363", "email": "blamore@example.net"}, "created_at": "1983-04-13 04:59:55", "updated_at": "2007-02-08 20:11:00"}
        response = httpx.post(f"{BASE_URL}/warehouses", json=new_warehouse)
        self.assertEqual(response.status_code, 201)

        response = httpx.get(f"{BASE_URL}/warehouses/{new_warehouse['id']}")
        self.assertEqual(response.status_code, 200)
        created_warehouse = json.loads(response.text)
        self.assertEqual(created_warehouse['id'], new_warehouse['id'])

if __name__ == "__main__":
    unittest.main()
