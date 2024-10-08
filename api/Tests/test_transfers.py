import unittest
import json
import os

from api.models.transfers import Transfers


class TestTransfers(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.transfers = Transfers(root_path, is_debug=True)
        self.transfers.data = [
            {"id": 1, "items": ["item1", "item2"], "transfer_status": "Completed"},
            {"id": 2, "items": ["item3", "item4"], "transfer_status": "Pending"}
            ]

    def test_get_transfers(self):
        self.assertEqual(self.transfers.get_transfers(), self.transfers.data)

    def test_get_transfer(self):
        self.assertEqual(self.transfers.get_transfer(1), self.transfers.data[0])

    def test_get_transfer_wrongid(self):
        self.assertIsNone(self.transfers.get_transfer(-1))

    def test_get_items_in_transfer(self):
        self.assertEqual(self.transfers.get_items_in_transfer(1), ["item1", "item2"])

    def test_add_transfer(self):
        new_transfer = {"id": 3, "items": ["item5", "item6"]}
        self.transfers.add_transfer(new_transfer)
        self.assertIn(new_transfer, self.transfers.data)

    def test_update_transfer(self):
        updated_transfer = {"id": 1, "items": ["item1", "item2", "item3"], "transfer_status": "Completed"}
        self.transfers.update_transfer(1, updated_transfer)
        self.assertEqual(self.transfers.get_transfer(1), updated_transfer)

    def test_get_transfer_wrongid(self):
        self.assertIsNone(self.transfers.get_transfer(-1))

    def test_get_items_in_transfer(self):
        self.assertEqual(self.transfers.get_items_in_transfer(1), ["item1", "item2"])

    def test_add_transfer(self):
        new_transfer = {"id": 3, "items": ["item5", "item6"]}
        self.transfers.add_transfer(new_transfer)
        self.assertIn(new_transfer, self.transfers.data)

    def test_update_transfer(self):
        updated_transfer = {"id": 1, "items": ["item1", "item2", "item3"], "transfer_status": "Completed"}
        self.transfers.update_transfer(1, updated_transfer)
        self.assertEqual(self.transfers.get_transfer(1), updated_transfer)

    def test_get_transfer_wrongid(self):
        self.assertIsNone(self.transfers.get_transfer(-1))

    def test_get_items_in_transfer(self):
        self.assertEqual(self.transfers.get_items_in_transfer(1), ["item1", "item2"])

    def test_add_transfer(self):
        new_transfer = {"id": 3, "items": ["item5", "item6"]}
        self.transfers.add_transfer(new_transfer)
        self.assertIn(new_transfer, self.transfers.data)

    def test_update_transfer(self):
        updated_transfer = {"id": 1, "items": ["item1", "item2", "item3"], "transfer_status": "Completed"}
        self.transfers.update_transfer(1, updated_transfer)
        self.assertEqual(self.transfers.get_transfer(1), updated_transfer)


if __name__ == '__main__':
    unittest.main()
