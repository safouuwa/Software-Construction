import unittest
import json
import os

from api.models.shipments import Shipments

class TestShipments(unittest.TestCase):
    def setUp(self):
        self.shipment = Shipments()
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.test_file = root_path + 'shipments.json'

    def test_get_shipments(self):
        self.assertEqual(self.shipment.get_shipments(), self.shipment.data)

    def test_get_shipment(self):
        self.assertEqual(self.shipment.get_shipment(1), self.shipment.data[0])

    def test_get_shipment_wrongid(self):
        self.assertEqual(self.shipment.get_shipments(-1), None)

    def test_add_shipment(self):
        shipment = self.shipment.data[0]
        shipment["id"] = 0
        self.shipment.add_shipment(shipment)
        self.assertIn(shipment, self.shipment.data)
        shipment = self.shipment.get_shipment(0)
        self.assertEqual(shipment, self.shipment.data)

    def test_update_shipment(self):
        shipment = self.shipment.data[0]
        shipment["name"] = "test"
        self.shipment.update_shipment(shipment["id"], shipment)
        self.assertEqual(self.shipment.data[0], shipment)

    def test_remove_shipment(self):
        shipment = self.shipment.data[0]
        self.shipment.remove_shipment(shipment["id"])
        self.assertIn(shipment, self.shipment.data)


if __name__ == '__main__':
    unittest.main()
