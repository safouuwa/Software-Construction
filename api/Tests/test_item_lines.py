import unittest
import json
import os

from models.item_lines import ItemLines


class TestItem_lines(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(
            os.path.dirname(os.path.dirname(os.path.dirname(__file__))),
            'data/')
        root_path = root_path.replace('\\', '/')
        self.test_file = root_path + 'item_lines.json'
        self.item_line = ItemLines(root_path)

    def test_get_item_lines(self):
        self.assertEqual(self.item_line.get_item_lines(), self.item_line.data)

    def test_get_item_line(self):
        for x in self.item_line.data:
            if x["id"] == 1:
                item_line = x
        self.assertEqual(self.item_line.get_item_line(1),
                         item_line)

    def test_get_item_line_wrongid(self):
        self.assertEqual(self.item_line.get_item_line(-1), None)

    def test_add_item_line(self):
        item_line = self.item_line.data[0]
        item_line["id"] = 0
        self.item_line.add_item_line(item_line)
        self.assertIn(item_line, self.item_line.data)

    def test_update_item_line(self):
        item_line = self.item_line.data[0]
        item_line["name"] = "test"
        self.item_line.update_item_line(item_line["id"], item_line)
        self.assertEqual(self.item_line.data[0], item_line)

    def test_remove_item_line(self):
        item_line = self.item_line.data[0]
        self.item_line.remove_item_line(item_line["id"])
        self.assertNotIn(item_line, self.item_line.data)


if __name__ == '__main__':
    unittest.main()
