import httpx
import unittest

class ApiGetTests(unittest.TestCase):
    @classmethod
    def setUpClass(self):
        self.base_url = "http://localhost:3000/api/v1/"
        self.client = httpx.Client(headers={"API_KEY": "a1b2c3d4e5"})

    # Clients
    def test_get_all_clients(self):
        response = self.client.get(f"{self.base_url}clients")
        self.assertEqual(response.status_code, 200)

    def test_get_client_by_id(self):
        client_id = 1
        response = self.client.get(f"{self.base_url}clients/{client_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], client_id)

    def test_get_client_orders(self):
        client_id = 1
        response = self.client.get(f"{self.base_url}clients/{client_id}/orders")
        self.assertEqual(response.status_code, 200)

    def test_get_non_existent_client(self):
        client_id = -1
        response = self.client.get(f"{self.base_url}clients/{client_id}")
        self.assertEqual(response.status_code, 404)

    # Shipments
    def test_get_all_shipments(self):
        response = self.client.get(f"{self.base_url}shipments")
        self.assertEqual(response.status_code, 200)

    def test_get_shipment_by_id(self):
        shipment_id = 1
        response = self.client.get(f"{self.base_url}shipments/{shipment_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], shipment_id)

    def test_get_shipment_orders(self):
        shipment_id = 1
        response = self.client.get(f"{self.base_url}shipments/{shipment_id}/orders")
        self.assertEqual(response.status_code, 200)

    def test_get_shipment_items(self):
        shipment_id = 1
        response = self.client.get(f"{self.base_url}shipments/{shipment_id}/orders")
        self.assertEqual(response.status_code, 200)

    def test_get_non_existent_shipment(self):
        shipment_id = -1
        response = self.client.get(f"{self.base_url}shipments/{shipment_id}")
        self.assertEqual(response.status_code, 404)

    # ItemGroups
    def test_get_all_item_groups(self):
        response = self.client.get(f"{self.base_url}item_groups")
        self.assertEqual(response.status_code, 200)

    def test_get_item_group_by_id(self):
        item_group_id = 1
        response = self.client.get(f"{self.base_url}item_groups/{item_group_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], item_group_id)

    def test_get_item_group_items(self):
        item_group_id = 1
        response = self.client.get(f"{self.base_url}item_groups/{item_group_id}/items")
        self.assertEqual(response.status_code, 200)

    def test_get_non_existent_item_group(self):
        item_group_id = -1
        response = self.client.get(f"{self.base_url}item_groups/{item_group_id}")
        self.assertEqual(response.status_code, 404)

    # Suppliers
    def test_get_all_suppliers(self):
        response = self.client.get(f"{self.base_url}suppliers")
        self.assertEqual(response.status_code, 200)

    def test_get_supplier_by_id(self):
        supplier_id = 1
        response = self.client.get(f"{self.base_url}suppliers/{supplier_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], supplier_id)

    def test_get_supplier_items(self):
        supplier_id = 1
        response = self.client.get(f"{self.base_url}suppliers/{supplier_id}/items")
        self.assertEqual(response.status_code, 200)

    def test_get_non_existent_supplier(self):
        supplier_id = -1
        response = self.client.get(f"{self.base_url}suppliers/{supplier_id}")
        self.assertEqual(response.status_code, 404)

    # Transfers
    def test_get_all_transfers(self):
        response = self.client.get(f"{self.base_url}transfers")
        self.assertEqual(response.status_code, 200)

    def test_get_transfer_by_id(self):
        transfer_id = 1
        response = self.client.get(f"{self.base_url}transfers/{transfer_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], transfer_id)

    def test_get_non_existent_transfer(self):
        transfer_id = -1
        response = self.client.get(f"{self.base_url}transfers/{transfer_id}")
        self.assertEqual(response.status_code, 404)

    def test_get_items_for_transfers(self):
        transfer_id = 1
        response = self.client.get(f"{self.base_url}transfers/{transfer_id}/items")
        self.assertEqual(response.status_code, 200)

    # Warehouses
    def test_get_all_warehouses(self):
        response = self.client.get(f"{self.base_url}warehouses")
        self.assertEqual(response.status_code, 200)

    def test_get_warehouse_by_id(self):
        warehouse_id = 1
        response = self.client.get(f"{self.base_url}warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], warehouse_id)

    def test_get_non_existent_warehouse(self):
        warehouse_id = -1
        response = self.client.get(f"{self.base_url}warehouses/{warehouse_id}")
        self.assertEqual(response.status_code, 404)

    # ItemTypes
    def test_get_all_item_types(self):
        response = self.client.get(f"{self.base_url}item_types")
        self.assertEqual(response.status_code, 200)

    def test_get_item_type_by_id(self):
        item_type_id = 1
        response = self.client.get(f"{self.base_url}item_types/{item_type_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], item_type_id)

    def test_get_item_type_items(self):
        item_type_id = 1
        response = self.client.get(f"{self.base_url}item_types/{item_type_id}/items")
        self.assertEqual(response.status_code, 200)

    def test_get_non_existent_item_type(self):
        item_type_id = -1
        response = self.client.get(f"{self.base_url}item_types/{item_type_id}")
        self.assertEqual(response.status_code, 404)

    # Items
    def test_get_all_items(self):
        response = self.client.get(f"{self.base_url}items")
        self.assertEqual(response.status_code, 200)

    def test_get_item_by_id(self):
        item_id = "P000047"
        response = self.client.get(f"{self.base_url}items/{item_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Uid'], item_id)

    def test_get_non_existent_item(self):
        item_id = "1"
        response = self.client.get(f"{self.base_url}items/{item_id}")
        self.assertEqual(response.status_code, 404)

    # Orders
    def test_get_all_orders(self):
        response = self.client.get(f"{self.base_url}orders")
        self.assertEqual(response.status_code, 200)

    def test_get_order_by_id(self):
        order_id = 1
        response = self.client.get(f"{self.base_url}orders/{order_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], order_id)

    def test_get_non_existent_order(self):
        order_id = -1
        response = self.client.get(f"{self.base_url}orders/{order_id}")
        self.assertEqual(response.status_code, 404)

    # Inventories
    def test_get_all_inventories(self):
        response = self.client.get(f"{self.base_url}inventories")
        self.assertEqual(response.status_code, 200)

    def test_get_inventory_by_id(self):
        inventory_id = 1
        response = self.client.get(f"{self.base_url}inventories/{inventory_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], inventory_id)

    def test_get_non_existent_inventory(self):
        inventory_id = -1
        response = self.client.get(f"{self.base_url}inventories/{inventory_id}")
        self.assertEqual(response.status_code, 404)

    # ItemLines
    def test_get_all_item_lines(self):
        response = self.client.get(f"{self.base_url}item_lines")
        self.assertEqual(response.status_code, 200)

    def test_get_item_line_by_id(self):
        item_line_id = 1
        response = self.client.get(f"{self.base_url}item_lines/{item_line_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], item_line_id)

    def test_get_non_existent_item_line(self):
        item_line_id = -1
        response = self.client.get(f"{self.base_url}item_lines/{item_line_id}")
        self.assertEqual(response.status_code, 404)

    # Locations
    def test_get_all_locations(self):
        response = self.client.get(f"{self.base_url}locations")
        self.assertEqual(response.status_code, 200)

    def test_get_location_by_id(self):
        location_id = 1
        response = self.client.get(f"{self.base_url}locations/{location_id}")
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json()['Id'], location_id)

    def test_get_non_existent_location(self):
        location_id = -1
        response = self.client.get(f"{self.base_url}locations/{location_id}")
        self.assertEqual(response.status_code, 404)

if __name__ == '__main__':
    unittest.main()
