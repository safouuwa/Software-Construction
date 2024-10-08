import unittest
import json
import os

from api.models.inventories import INVENTORIES, Inventories


class TestInventories(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.test_file = root_path + 'inventories.json'
        self.inventory = Inventories(root_path, is_debug=True)

    def test_get_inventory(self):
        inventory_id = 1
        self.inventory.data = [{"id": 1, "name": "Inventory 1"}]
        self.assertEqual(self.inventory.get_inventory(inventory_id), self.inventory.data[0])

    def test_get_inventories(self):
        self.inventory.data = [{"id": 1, "name": "Inventory 1"}, {"id": 1, "name": "Inventory 2"}]
        self.assertEqual(self.inventory.get_inventories(), self.inventory.data)

    def test_get_inventories_for_item(self):
        item_id = "P000004"
        self.inventory.data = [
            {"id": 1, "item_id": "P000004", "name": "Inventory 1"},
            {"id": 2, "item_id": "P000005", "name": "Inventory 2"},
            {"id": 3, "item_id": "P000004", "name": "Inventory 3"}
        ]
        expected_result = [
            {"id": 1, "item_id": "P000004", "name": "Inventory 1"},
            {"id": 3, "item_id": "P000004", "name": "Inventory 3"}
        ]
        self.assertEqual(self.inventory.get_inventories_for_item(item_id), expected_result)

    def test_get_inventory_totals_for_item(self):
        item_id = "P000001"
        self.inventory.data = [
            {
                "id": 1, "item_id": "P000001", "total_expected": 10,
                "total_ordered": 5, "total_allocated": 3, "total_available": 2
            },
            {
                "id": 2, "item_id": "P000002", "total_expected": 15,
                "total_ordered": 10, "total_allocated": 7, "total_available": 3
            },
            {
                "id": 3, "item_id": "P000001", "total_expected": 20,
                "total_ordered": 8, "total_allocated": 4, "total_available": 8
            }
        ]
        expected_result = {
            "total_expected": 30,
            "total_ordered": 13,
            "total_allocated": 7,
            "total_available": 10
        }
        self.assertEqual(self.inventory.get_inventory_totals_for_item(item_id), expected_result)

    def test_get_inventory_wrongid(self):
        inventory_id = -1
        self.inventory.data = [{"id": 1, "name": "Inventory 1"}]
        self.assertEqual(self.inventory.get_inventory(inventory_id), None)

    def test_add_inventory(self):
        inventory = {"id": 2, "name": "Inventory 2"}
        self.inventory.add_inventory(inventory)
        self.assertIn(inventory, self.inventory.data)

    def test_update_inventory(self):
        inventory_id = 1
        self.inventory.data = [{"id": 1, "name": "Inventory 1"}]
        updated_inventory = {"id": 1, "name": "Updated Inventory 1"}
        self.inventory.update_inventory(inventory_id, updated_inventory)
        self.assertEqual(self.inventory.data[0], updated_inventory)

    def test_remove_inventory(self):
        inventory_id = 1
        self.inventory.data = [{"id": 1, "name": "Inventory 1"}]
        self.inventory.remove_inventory(inventory_id)
        self.assertNotIn({"id": 1, "name": "Inventory 1"}, self.inventory.data)

    def test_load(self):
        self.inventory.load(is_debug=True)
        self.assertEqual(self.inventory.data, INVENTORIES)

    def test_save(self):
        self.inventory.data = [{"id": 1, "name": "Inventory 1"}]
        self.inventory.save()
        with open(self.test_file, "r") as f:
            data = json.load(f)
        self.assertEqual(data, self.inventory.data)


if __name__ == '__main__':
    unittest.main()
