import unittest
import json
import os

from models.shipments import Shipments

class TestShipments(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.shipment = Shipments(root_path)

    def test_get_shipments(self):
        self.assertEqual(self.shipment.get_shipments(), self.shipment.data)

    def test_get_shipment(self):
        for x in self.shipment.data:
            if x["id"] == 1:
                shipment = x
        self.assertEqual(self.shipment.get_shipment(1), shipment)

    def test_get_shipment_wrongid(self):
        self.assertEqual(self.shipment.get_shipment(-1), None)

    def test_add_shipment(self):
        shipment = self.shipment.data[0]
        shipment["id"] = 0
        self.shipment.add_shipment(shipment)
        self.assertIn(shipment, self.shipment.data)

    def test_update_shipment(self):
        shipment = self.shipment.data[0]
        shipment["name"] = "test"
        self.shipment.update_shipment(shipment["id"], shipment)
        self.assertEqual(self.shipment.data[0], shipment)

    def test_remove_shipment(self):
        shipment = self.shipment.data[0]
        self.shipment.remove_shipment(shipment["id"])
        self.assertNotIn(shipment, self.shipment.data)


if __name__ == '__main__':
    unittest.main()
