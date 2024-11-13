import httpx
import unittest
import random
import string
import json
import os

def generate_uid():
    return ''.join(random.choices(string.ascii_letters + string.digits, k=10))

class ApiDeleteTests(unittest.TestCase):
    def setUp(self):
        self.base_url = "http://127.0.0.1:3000/api/v1/"
        self.client = httpx.Client(headers={"API_KEY": "a1b2c3d4e5"})
        self.data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))), "data").replace(os.sep, "/")

    def GetJsonData(self, model):
        with open(os.path.join(self.data_root, f"{model}.json"), 'r') as file:
            data = json.load(file)
        return data

    def test_delete_client(self):
        client_data = '{"id": 1234567890, "name": "Test Client", "address": "123 Test St", "city": "Test City", "zip_code": "12345"}'
        response = self.client.post(f"{self.base_url}clients", data=client_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(client_data, self.GetJsonData("clients"))
        client_id = 1234567890  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}clients/{client_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(client_data, self.GetJsonData("clients"))

    def test_delete_non_existent_client(self):
        response = self.client.delete(f"{self.base_url}clients/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_shipment(self):
        shipment_data = '''{
            "id": 1234567891,
            "order_id": 1,
            "source_id": 1,
            "order_date": "2024-10-18",
            "request_date": "2024-10-19",
            "shipment_date": "2024-10-20",
            "shipment_type": "Express",
            "shipment_status": "Shipped",
            "items": [{"item_id": "item1", "amount": 1}]
        }'''
        response = self.client.post(f"{self.base_url}shipments", data=shipment_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(shipment_data, self.GetJsonData("shipments"))
        shipment_id = 1234567891  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}shipments/{shipment_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(shipment_data, self.GetJsonData("shipments"))

    def test_delete_non_existent_shipment(self):
        response = self.client.delete(f"{self.base_url}shipments/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_item_group(self):
        item_group_data = '{"id": 1234567892, "name": "Test Item Group", "description": "Description for test group"}'
        response = self.client.post(f"{self.base_url}item_groups", data=item_group_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(item_group_data, self.GetJsonData("item_groups"))
        item_group_id = 1234567892  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}item_groups/{item_group_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(item_group_data, self.GetJsonData("item_groups"))

    def test_delete_non_existent_item_group(self):
        response = self.client.delete(f"{self.base_url}item_groups/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_supplier(self):
        supplier_data = '{"id": 1234567893, "name": "Test Supplier", "address": "123 Supplier St"}'
        response = self.client.post(f"{self.base_url}suppliers", data=supplier_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(supplier_data, self.GetJsonData("suppliers"))
        supplier_id = 1234567893  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}suppliers/{supplier_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(supplier_data, self.GetJsonData("suppliers"))

    def test_delete_non_existent_supplier(self):
        response = self.client.delete(f"{self.base_url}suppliers/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_transfer(self):
        transfer_data = '{"id": 1234567894, "items": [{"item_id": "item1"}, {"item_id": "item2"}], "transfer_status": "Pending"}'
        response = self.client.post(f"{self.base_url}transfers", data=transfer_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(transfer_data, self.GetJsonData("transfers"))
        transfer_id = 1234567894  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}transfers/{transfer_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(transfer_data, self.GetJsonData("transfers"))

    def test_delete_non_existent_transfer(self):
        response = self.client.delete(f"{self.base_url}transfers/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_warehouse(self):
        warehouse_data = '{"id": 1234567895, "name": "Test Warehouse", "address": "Test Address"}'
        response = self.client.post(f"{self.base_url}warehouses", data=warehouse_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(warehouse_data, self.GetJsonData("warehouses"))
        warehouse_id = 1234567895  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(warehouse_data, self.GetJsonData("warehouses"))

    def test_delete_non_existent_warehouse(self):
        response = self.client.delete(f"{self.base_url}warehouses/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_item_type(self):
        item_type_data = '{"id": 1234567896, "name": "Test Item Type", "description": "Description for test type"}'
        response = self.client.post(f"{self.base_url}item_types", data=item_type_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(item_type_data, self.GetJsonData("item_types"))
        item_type_id = 1234567896  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}item_types/{item_type_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(item_type_data, self.GetJsonData("item_types"))

    def test_delete_non_existent_item_type(self):
        response = self.client.delete(f"{self.base_url}item_types/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_item_line(self):
        item_line_data = '{"id": 1234567897, "name": "Test ItemLine", "description": "Description of the test item line"}'
        response = self.client.post(f"{self.base_url}item_lines", data=item_line_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(item_line_data, self.GetJsonData("item_lines"))
        item_line_id = 1234567897  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}item_lines/{item_line_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(item_line_data, self.GetJsonData("item_lines"))

    def test_delete_non_existent_item_line(self):
        response = self.client.delete(f"{self.base_url}item_lines/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_item(self):
        item_data = '''{
            "uid": "hgdfhjnfJFBDH",
            "code": "Test Item",
            "description": "Description for test item",
            "short_description": "Short description",
            "upc_code": "1234567890",
            "model_number": "Model 123",
            "commodity_code": "Commodity 123",
            "item_line": 1,
            "item_group": 1,
            "item_type": 1,
            "unit_purchase_quantity": 1,
            "unit_order_quantity": 1,
            "pack_order_quantity": 1,
            "supplier_id": 1,
            "supplier_code": "Supplier 123",
            "supplier_part_number": "Part 123"
        }'''
        response = self.client.post(f"{self.base_url}items", data=item_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(item_data, self.GetJsonData("items"))
        item_uid = "hgdfhjnfJFBDH"  # Replace with the actual UID or another identifier if needed

        response = self.client.delete(f"{self.base_url}items/{item_uid}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(item_data, self.GetJsonData("items"))

    def test_delete_non_existent_item(self):
        response = self.client.delete(f"{self.base_url}items/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_order(self):
        order_data = '''{
            "id": 1234567898,
            "source_id": 1,
            "order_date": "2024-10-18",
            "request_date": "2024-10-19",
            "reference": "Test Order",
            "order_status": "Scheduled",
            "items": [{"item_id": "item1", "amount": 1}]
        }'''
        response = self.client.post(f"{self.base_url}orders", data=order_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(order_data, self.GetJsonData("orders"))
        order_id = 1234567898  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}orders/{order_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(order_data, self.GetJsonData("orders"))

    def test_delete_non_existent_order(self):
        response = self.client.delete(f"{self.base_url}orders/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_inventory(self):
        inventory_data = '''{
            "id": 1234567899,
            "item_id": "P000TEST",
            "description": "Test Inventory",
            "locations": [1, 2],
            "total_on_hand": 100
        }'''
        response = self.client.post(f"{self.base_url}inventories", data=inventory_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(inventory_data, self.GetJsonData("locations"))
        inventory_id = 1234567899  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}inventories/{inventory_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(inventory_data, self.GetJsonData("inventories"))

    def test_delete_non_existent_inventory(self):
        response = self.client.delete(f"{self.base_url}inventories/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def test_delete_location(self):
        location_data = '{"id": 156738, "warehouse_id": 1, "code": "A.1.0", "name": "Row: A, Rack: 1, Shelf: 0", "created_at": "1992-05-15 03:21:32", "updated_at": "1992-05-15 03:21:32"}'
        response = self.client.post(f"{self.base_url}locations", data=location_data)
        self.assertEqual(response.status_code, httpx.codes.CREATED)
        self.assertIn(location_data, self.GetJsonData("locations"))
        location_id = 156738  # Directly using the ID from the data

        response = self.client.delete(f"{self.base_url}locations/{location_id}")
        self.assertEqual(response.status_code, httpx.codes.OK)
        self.assertNotIn(location_data, self.GetJsonData("locations"))

    def test_delete_non_existent_location(self):
        response = self.client.delete(f"{self.base_url}locations/-1")
        self.assertEqual(response.status_code, httpx.codes.NOT_FOUND)

    def tearDown(self):
        self.client.close()

if __name__ == "__main__":
    unittest.main()
