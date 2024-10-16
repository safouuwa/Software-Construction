import unittest
import json
import os

from models.item_groups import ItemGroups


class TestItem_groups(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.item_group = ItemGroups(root_path)

    def test_get_item_groups(self):
        self.assertEqual(self.item_group.get_item_groups(),
                         self.item_group.data)

    def test_get_item_group(self):
        for x in self.item_group.data:
            if x["id"] == 1:
                item_group = x
        self.assertEqual(self.item_group.get_item_group(1), item_group)

    def test_get_item_group_wrongid(self):
        self.assertEqual(self.item_group.get_item_group(-1), None)

    def test_add_item_group(self):
        item_group = self.item_group.data[0]
        item_group["id"] = 0
        self.item_group.add_item_group(item_group)
        self.assertIn(item_group, self.item_group.data)

    def test_update_item_group(self):
        item_group = self.item_group.data[0]
        item_group["name"] = "test"
        self.item_group.update_item_group(item_group["id"], item_group)
        self.assertEqual(self.item_group.data[0], item_group)

    def test_remove_item_group(self):
        item_group = self.item_group.data[0]
        self.item_group.remove_item_group(item_group["id"])
        self.assertNotIn(item_group, self.item_group.data)


if __name__ == '__main__':
    unittest.main()
