import unittest
import json
from api.models.warehouses import Warehouses
import os

class TestWarehouses(unittest.TestCase):
	def setUp(self):
		self.warehouse = Warehouses()
		root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
		root_path = root_path.replace('\\', '/') 
		self.test_file = root_path + 'warehouses.json'  

	def test_get_warehouse(self):
		warehouse_id = 1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.assertEqual(self.warehouse.get_warehouse(warehouse_id), self.warehouse.data[0])

	def test_get_warehouse_wrongid(self):
		warehouse_id = -1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.assertEqual(self.warehouse.get_warehouse(warehouse_id), None)

	def test_add_warehouse(self):
		warehouse = {"id": 2, "name": "Warehouse 2"}
		self.warehouse.add_warehouse(warehouse)
		self.assertIn(warehouse, self.warehouse.data)

	def test_update_warehouse(self):
		warehouse_id = 1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		updated_warehouse = {"id": 1, "name": "Updated Warehouse 1"}
		self.warehouse.update_warehouse(warehouse_id, updated_warehouse)
		self.assertEqual(self.warehouse.data[0], updated_warehouse)

	def test_remove_warehouse(self):
		warehouse_id = 1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.warehouse.remove_warehouse(warehouse_id)
		self.assertNotIn({"id": 1, "name": "Warehouse 1"}, self.warehouse.data)

	def test_load(self):
		self.warehouse.load(is_debug=True)
		self.assertEqual(self.warehouse.data, WAREHOUSES)

	def test_save(self):
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.warehouse.save()
		with open(self.test_file, "r") as f:
			data = json.load(f)
		self.assertEqual(data, self.warehouse.data)

	def test_get_warehouse(self):
		warehouse_id = 1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.assertEqual(self.warehouse.get_warehouse(warehouse_id), self.warehouse.data[0])

	def test_get_warehouse_wrongid(self):
		warehouse_id = -1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.assertEqual(self.warehouse.get_warehouse(warehouse_id), None)

	def test_add_warehouse(self):
		warehouse = {"id": 2, "name": "Warehouse 2"}
		self.warehouse.add_warehouse(warehouse)
		self.assertIn(warehouse, self.warehouse.data)

	def test_update_warehouse(self):
		warehouse_id = 1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		updated_warehouse = {"id": 1, "name": "Updated Warehouse 1"}
		self.warehouse.update_warehouse(warehouse_id, updated_warehouse)
		self.assertEqual(self.warehouse.data[0], updated_warehouse)

	def test_remove_warehouse(self):
		warehouse_id = 1
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.warehouse.remove_warehouse(warehouse_id)
		self.assertNotIn({"id": 1, "name": "Warehouse 1"}, self.warehouse.data)

	def test_load(self):
		self.warehouse.load(is_debug=True)
		self.assertEqual(self.warehouse.data, WAREHOUSES)

	def test_save(self):
		self.warehouse.data = [{"id": 1, "name": "Warehouse 1"}]
		self.warehouse.save()
		with open(self.test_file, "r") as f:
			data = json.load(f)
		self.assertEqual(data, self.warehouse.data)

if __name__ == '__main__':
	unittest.main()