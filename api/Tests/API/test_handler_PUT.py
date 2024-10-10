import httpx
import json
import unittest

BASE_URL = "http://localhost:3000/api/v1"

class TestApiHandlerPUT(unittest.TestCase):

    def setUp(self):
        self.headers = {
        "API_KEY": "a1b2c3d4e5"
        }

    def create_test_client(self):
        new_client = {
            "id": 0,
            "name": "Raymond Inc",
            "address": "1296 Daniel Road Apt. 349",
            "city": "Pierceview",
            "zip_code": "28301",
            "province": "Colorado",
            "country": "United States",
            "contact_name": "Bryan Clark",
            "contact_phone": "242.732.3483x2573",
            "contact_email": "robertcharles@example.net"
        }
        response = httpx.post(f"{BASE_URL}/clients", json=new_client, headers=self.headers)
        return new_client['id']

    def create_test_inventory(self):
        new_inventory = {
            "item_id": "PD000000",
            "description": "Face-to-face clear-thinking complexity",
            "locations": [3211, 24700, 14123, 19538, 31071, 24701, 11606, 11817],
            "total_on_hand": 262,
            "total_expected": 0,
            "total_ordered": 80,
            "total_allocated": 41,
            "total_available": 141
        }
        response = httpx.post(f"{BASE_URL}/inventories", json=new_inventory, headers=self.headers)
        return new_inventory['item_id']

    def create_test_item(self):
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
            "supplier_part_number": "E-86805-uTM"
        }
        response = httpx.post(f"{BASE_URL}/items", json=new_item, headers=self.headers)
        return new_item['uid']

    def create_test_location(self):
        new_location = {
            "warehouse_id": 1,
            "code": "test",
            "name": "Row: A, Rack: 1, Shelf: 0"
        }
        response = httpx.post(f"{BASE_URL}/locations", json=new_location, headers=self.headers)
        return new_location['warehouse_id']

    def create_test_order(self):
        new_order = {
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
            "ship_to": None,
            "bill_to": None,
            "shipment_id": 3,
            "total_amount": 1156.14,
            "total_discount": 420.45,
            "total_tax": 677.42,
            "total_surcharge": 86.03
        }
        response = httpx.post(f"{BASE_URL}/orders", json=new_order, headers=self.headers)
        return new_order['source_id']

    def create_test_shipment(self):
        new_shipment = {
            "id": 0,
            "order_id": self.order_id,
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
            "total_package_weight": 463.0
        }
        response = httpx.post(f"{BASE_URL}/shipments", json=new_shipment, headers=self.headers)
        return new_shipment['id']

    def create_test_supplier(self):
        new_supplier = {
            "id": 0,
            "code": "SUP0001",
            "name": "test",
            "address": "5989 Sullivan Drives",
            "address_extra": "Apt. 996",
            "city": "Port Anitaburgh",
            "zip_code": "91688",
            "province": "Illinois",
            "country": "Czech Republic",
            "contact_name": "Toni Barnett",
            "phonenumber": "363.541.7282x36825",
            "reference": "LPaJ-SUP0001"
        }
        response = httpx.post(f"{BASE_URL}/suppliers", json=new_supplier, headers=self.headers)
        return new_supplier['id']

    def create_test_transfer(self):
        new_transfer = {
            "id": 0,
            "reference": "TEST00001",
            "transfer_from": None,
            "transfer_to": 9229,
            "transfer_status": "Completed"
        }
        response = httpx.post(f"{BASE_URL}/transfers", json=new_transfer, headers=self.headers)
        return new_transfer['id']

    def create_test_warehouse(self):
        new_warehouse = {
            "id": 0,
            "code": "YQZZNL56",
            "name": "TEST cargo hub",
            "address": "Karlijndreef 281",
            "zip": "4002 AS",
            "city": "Heemskerk",
            "province": "Friesland",
            "country": "NL",
            "contact": {
                "name": "Fem Keijzer",
                "phone": "(078) 0013363",
                "email": "blamore@example.net"
            }
        }
        response = httpx.post(f"{BASE_URL}/warehouses", json=new_warehouse, headers=self.headers)
        return new_warehouse['id']

    def test_update_client(self):
        self.client_id = self.create_test_client()
        updated_client = {
            "name": "Raymond Inc Updated",
            "address": "New Address 123",
            "city": "New City",
            "zip_code": "12345",
            "province": "New Province",
            "country": "Updated Country",
            "contact_name": "Bryan Clark Updated",
            "contact_phone": "123-456-7890",
            "contact_email": "updated@example.com"
        }
        response = httpx.put(f"{BASE_URL}/clients/{self.client_id}", json=updated_client, headers=self.headers)
        self.assertEqual(response.status_code, 200)

        response = httpx.get(f"{BASE_URL}/clients/{self.client_id}", headers=self.headers)
        updated_data = json.loads(response.text)
        self.assertEqual(updated_data['name'], updated_client['name'])
        self.assertEqual(updated_data['address'], updated_client['address'])

    # def test_update_inventory(self):
    #     self.inventory_id = self.create_test_inventory()
    #     updated_inventory = {
    #         "item_id": "PD000000",
    #         "description": "Updated description",
    #         "locations": [3211, 24700],
    #         "total_on_hand": 300,
    #         "total_expected": 10,
    #         "total_ordered": 90,
    #         "total_allocated": 50,
    #         "total_available": 200
    #     }
    #     response = httpx.put(f"{BASE_URL}/inventories/{self.inventory_id}", json=updated_inventory, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/inventories/{self.inventory_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['description'], updated_inventory['description'])

    # def test_update_item(self):
    #     self.item_id = self.create_test_item()
    #     updated_item = {
    #         "uid": "test_updated",
    #         "code": "sjQ23408K",
    #         "description": "Updated description",
    #         "short_description": "must updated",
    #         "upc_code": "6523540947123",
    #         "model_number": "63-OFFTq0T_updated",
    #         "commodity_code": "oTo304_updated",
    #         "item_line": 12,
    #         "item_group": 74,
    #         "item_type": 15,
    #         "unit_purchase_quantity": 50,
    #         "unit_order_quantity": 15,
    #         "pack_order_quantity": 12,
    #         "supplier_id": 35,
    #         "supplier_code": "SUP424",
    #         "supplier_part_number": "E-86806-uTM"
    #     }
    #     response = httpx.put(f"{BASE_URL}/items/{self.item_id}", json=updated_item, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/items/{self.item_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['description'], updated_item['description'])

    # def test_update_location(self):
    #     self.location_id = self.create_test_location()
    #     updated_location = {
    #         "warehouse_id": 2,
    #         "code": "test_updated",
    #         "name": "Row: A, Rack: 2, Shelf: 1"
    #     }
    #     response = httpx.put(f"{BASE_URL}/locations/{self.location_id}", json=updated_location, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/locations/{self.location_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['name'], updated_location['name'])

    # def test_update_order(self):
    #     self.order_id = self.create_test_order()
    #     updated_order = {
    #         "source_id": 53,
    #         "order_date": "1983-10-26T19:06:08Z",
    #         "request_date": "1983-10-30T19:06:08Z",
    #         "reference": "ORD00004",
    #         "reference_extra": "test updated",
    #         "order_status": "Pending",
    #         "notes": "Updated notes.",
    #         "shipping_notes": "Updated shipping notes.",
    #         "picking_notes": "Updated picking notes.",
    #         "warehouse_id": 12,
    #         "total_amount": 1200.00,
    #         "total_discount": 450.00,
    #         "total_tax": 700.00,
    #         "total_surcharge": 90.00
    #     }
    #     response = httpx.put(f"{BASE_URL}/orders/{self.order_id}", json=updated_order, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/orders/{self.order_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['reference'], updated_order['reference'])

    # def test_update_shipment(self):
    #     self.shipment_id = self.create_test_shipment()
    #     updated_shipment = {
    #         "order_id": self.order_id,
    #         "source_id": 53,
    #         "shipment_date": "1973-02-02",
    #         "shipment_type": "I",
    #         "shipment_status": "Shipped",
    #         "notes": "Updated shipment notes.",
    #         "carrier_code": "DHL",
    #         "carrier_description": "DHL Express Updated",
    #         "service_code": "SameDay",
    #         "payment_type": "Automatic",
    #         "transfer_mode": "Air",
    #         "total_package_count": 30,
    #         "total_package_weight": 470.0
    #     }
    #     response = httpx.put(f"{BASE_URL}/shipments/{self.shipment_id}", json=updated_shipment, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/shipments/{self.shipment_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['shipment_status'], updated_shipment['shipment_status'])

    # def test_update_supplier(self):
    #     self.supplier_id = self.create_test_supplier()
    #     updated_supplier = {
    #         "code": "SUP0001_UPDATED",
    #         "name": "test updated",
    #         "address": "New Address 456",
    #         "address_extra": "Apt. 999",
    #         "city": "New Port Anitaburgh",
    #         "zip_code": "91689",
    #         "province": "Illinois",
    #         "country": "Czech Republic",
    #         "contact_name": "Toni Barnett Updated",
    #         "phonenumber": "363.541.7283x36826"
    #     }
    #     response = httpx.put(f"{BASE_URL}/suppliers/{self.supplier_id}", json=updated_supplier, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/suppliers/{self.supplier_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['name'], updated_supplier['name'])

    # def test_update_transfer(self):
    #     self.transfer_id = self.create_test_transfer()
    #     updated_transfer = {
    #         "reference": "TEST00001_UPDATED",
    #         "transfer_from": None,
    #         "transfer_to": 9230,
    #         "transfer_status": "In Progress"
    #     }
    #     response = httpx.put(f"{BASE_URL}/transfers/{self.transfer_id}", json=updated_transfer, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/transfers/{self.transfer_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['reference'], updated_transfer['reference'])

    # def test_update_warehouse(self):
    #     self.warehouse_id = self.create_test_warehouse()
    #     updated_warehouse = {
    #         "code": "YQZZNL57",
    #         "name": "TEST cargo hub Updated",
    #         "address": "New Address 789",
    #         "zip": "4003 AS",
    #         "city": "New Heemskerk",
    #         "province": "Friesland",
    #         "country": "NL",
    #         "contact": {
    #             "name": "Fem Keijzer Updated",
    #             "phone": "(078) 0013364",
    #             "email": "updated@example.net"
    #         }
    #     }
    #     response = httpx.put(f"{BASE_URL}/warehouses/{self.warehouse_id}", json=updated_warehouse, headers=self.headers)
    #     self.assertEqual(response.status_code, 200)

    #     response = httpx.get(f"{BASE_URL}/warehouses/{self.warehouse_id}", headers=self.headers)
    #     updated_data = json.loads(response.text)
    #     self.assertEqual(updated_data['name'], updated_warehouse['name'])

if __name__ == "__main__":
    unittest.main()
