import unittest
import json
from api.models.orders import Orders
import os

class TestOrders(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.orders = Orders(root_path)
        self.test_file = root_path + 'orders.json'

    def test_get_orders(self):
        self.assertEqual(self.orders.get_orders(), self.orders.data)

    def test_get_order(self):
        order = self.orders.data[0]
        self.assertEqual(self.orders.get_order(order["id"]), order)

    def test_get_order_wrongid(self):
        self.assertEqual(self.orders.get_order(-1), None)

    def test_get_items_in_order(self):
        order = self.orders.data[0]
        self.assertEqual(self.orders.get_items_in_order(order["id"]), order["items"])

    def test_get_orders_in_shipment(self):
        order = self.orders.data[0]
        shipment_id = order["shipment_id"]
        self.assertIn(order["id"], self.orders.get_orders_in_shipment(shipment_id))

    def test_get_orders_for_client(self):
        order = self.orders.data[0]
        client_id = order["ship_to"]
        self.assertIn(order, self.orders.get_orders_for_client(client_id))

    def test_add_order(self):
        order = self.orders.data[0].copy()
        order["id"] = 999
        self.orders.add_order(order)
        self.assertIn(order, self.orders.data)
        self.assertEqual(self.orders.get_order(999), order)

    def test_update_order(self):
        order = self.orders.data[0].copy()
        order["status"] = "Updated"
        self.orders.update_order(order["id"], order)
        self.assertEqual(self.orders.get_order(order["id"])["status"], "Updated")

    def test_update_items_in_order(self):
        order = self.orders.data[0]
        new_items = [{"item_id": 999, "amount": 10}]
        self.orders.update_items_in_order(order["id"], new_items)
        self.assertEqual(self.orders.get_items_in_order(order["id"]), new_items)

    def test_update_orders_in_shipment(self):
        shipment_id = self.orders.data[0]["shipment_id"]
        new_orders = [self.orders.data[0]["id"]]
        self.orders.update_orders_in_shipment(shipment_id, new_orders)
        self.assertIn(self.orders.data[0]["id"], self.orders.get_orders_in_shipment(shipment_id))

    def test_remove_order(self):
        order = self.orders.data[0]
        self.orders.remove_order(order["id"])
        self.assertNotIn(order, self.orders.data)

if __name__ == '__main__':
    unittest.main()
