import unittest
import json
import os

from api.models.items import Items


class TestItems(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(
            os.path.dirname(os.path.dirname(os.path.dirname(__file__))),
            'data/')
        root_path = root_path.replace('\\', '/')
        self.items = Items(root_path)
        self.test_file = root_path + 'items.json'

    def test_get_items(self):
        self.assertEqual(self.items.get_items(), self.items.data)

    def test_get_item(self):
        item = self.items.data[0]
        self.assertEqual(self.items.get_item(item["uid"]), item)

    def test_get_item_wrongid(self):
        self.assertEqual(self.items.get_item(-1), None)

    def test_get_items_for_item_line(self):
        item = self.items.data[0]
        item_line_id = item["item_line"]
        self.assertIn(item, self.items.get_items_for_item_line(item_line_id))

    def test_get_items_for_item_group(self):
        item = self.items.data[0]
        item_group_id = item["item_group"]
        self.assertIn(item, self.items.get_items_for_item_group(item_group_id))

    def test_get_items_for_item_type(self):
        item = self.items.data[0]
        item_type_id = item["item_type"]
        self.assertIn(item, self.items.get_items_for_item_type(item_type_id))

    def test_get_items_for_supplier(self):
        item = self.items.data[0]
        supplier_id = item["supplier_id"]
        self.assertIn(item, self.items.get_items_for_supplier(supplier_id))

    def test_add_item(self):
        item = self.items.data[0].copy()
        item["uid"] = 999
        self.items.add_item(item)
        self.assertIn(item, self.items.data)
        self.assertEqual(self.items.get_item(999), item)

    def test_update_item(self):
        item = self.items.data[0].copy()
        item["name"] = "test"
        self.items.update_item(item["uid"], item)
        self.assertEqual(self.items.get_item(item["uid"])["name"], "test")

    def test_remove_item(self):
        item = self.items.data[0]
        self.items.remove_item(item["uid"])
        self.assertNotIn(item, self.items.data)


if __name__ == '__main__':
    unittest.main()
