import unittest
import json
import os

from api.models.suppliers import Suppliers


class TestSuppliers(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.supplier = Suppliers(root_path)
        self.test_file = root_path + 'suppliers.json'

    def test_get_suppliers(self):
        self.assertEqual(self.supplier.get_suppliers(), self.supplier.data)

    def test_get_supplier(self):
        self.assertEqual(self.supplier.get_supplier(1), self.supplier.data[0])

    def test_get_supplier_wrongid(self):
        self.assertEqual(self.supplier.get_supplier(-1), None)

    def test_add_supplier(self):
        supplier = self.supplier.data[0]
        supplier["id"] = 0
        self.supplier.add_supplier(supplier)
        self.assertIn(supplier, self.supplier.data)
        supplier = self.supplier.get_supplier(0)
        self.assertEqual(supplier, self.supplier.data[0])

    def test_update_supplier(self):
        supplier = self.supplier.data[0]
        supplier["name"] = "test"
        self.supplier.update_supplier(supplier["id"], supplier)
        self.assertEqual(self.supplier.data[0], supplier)

    def test_remove_supplier(self):
        supplier = self.supplier.data[0]
        self.supplier.data.remove(supplier)
        self.assertNotIn(supplier, self.supplier.data)


if __name__ == '__main__':
    unittest.main()
