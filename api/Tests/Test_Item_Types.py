import unittest
import json
import os
from api.models.item_types import ItemTypes


class TestItemTypes(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.item_types = ItemTypes(root_path)
        self.test_file = root_path + 'item_types.json'

    def test_get_item_types(self):
        self.assertEqual(self.item_types.get_item_types(), self.item_types.data)

    def test_get_item_type(self):
        item_type = self.item_types.data[0]
        self.assertEqual(self.item_types.get_item_type(item_type["id"]), item_type)

    def test_get_item_type_wrongid(self):
        self.assertEqual(self.item_types.get_item_type(-1), None)

    def test_add_item_type(self):
        item_type = self.item_types.data[0].copy()
        item_type["id"] = 999
        self.item_types.add_item_type(item_type)
        self.assertIn(item_type, self.item_types.data)
        self.assertEqual(self.item_types.get_item_type(999), item_type)

    def test_update_item_type(self):
        item_type = self.item_types.data[0].copy()
        item_type["name"] = "test"
        self.item_types.update_item_type(item_type["id"], item_type)
        self.assertEqual(self.item_types.get_item_type(item_type["id"])["name"], "test")

    def test_remove_item_type(self):
        item_type = self.item_types.data[0]
        self.item_types.remove_item_type(item_type["id"])
        self.assertNotIn(item_type, self.item_types.data)


if __name__ == '__main__':
    unittest.main()
