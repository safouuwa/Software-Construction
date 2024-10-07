import unittest
import json
from api.models.item_lines import ItemLines
import os

class TestItem_lines(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.test_file = root_path + 'item_lines.json'
        self.item_line = ItemLines(root_path, is_debug=True)

    def test_get_item_lines(self):
        self.assertEqual(self.item_line.get_item_lines(), self.item_line.data)

    def test_get_item_line(self):
        self.assertEqual(self.item_line.get_item_line(1), self.item_line.data[0])

    def test_get_item_line_wrongid(self):
        self.assertEqual(self.item_line.get_item_lines(-1), None)

    def test_add_item_line(self):
        item_line = self.item_line.data[0]
        item_line["id"] = 0
        self.item_line.add_item_line(item_line)
        self.assertIn(item_line, self.item_line.data)
        item_line = self.item_line.get_item_line(0)
        self.assertEqual(item_line, self.item_line.data)

    def test_update_item_line(self):
        item_line = self.item_line.data[0]
        item_line["name"] = "test"
        self.item_line.update_item_line(item_line["id"], item_line)
        self.assertEqual(self.item_line.data[0], item_line)


    def test_remove_item_line(self):
        item_line = self.item_line.data[0]
        self.item_line.remove_item_line(item_line["id"])
        self.assertIn(item_line, self.item_line.data)



if __name__ == '__main__':
    unittest.main()