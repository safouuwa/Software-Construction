import unittest
import json
import os

from models.clients import Clients


class TestClients(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.client = Clients(root_path)

    def test_get_clients(self):
        self.assertEqual(self.client.get_clients(), self.client.data)

    def test_get_client(self):
        for x in self.client.data:
            if x["id"] == 1:
                client = x
        self.assertEqual(self.client.get_client(1), client)

    def test_get_client_wrongid(self):
        self.assertEqual(self.client.get_client(-1), None)

    def test_add_client(self):
        client = self.client.data[0]
        client["id"] = 0
        self.client.add_client(client)
        self.assertIn(client, self.client.data)
        client = self.client.get_client(0)
        self.assertEqual(client, self.client.data[0])

    def test_update_client(self):
        client = self.client.data[0]
        client["name"] = "test"
        self.client.update_client(client["id"], client)
        self.assertEqual(self.client.data[0], client)

    def test_remove_client(self):
        client = self.client.data[0]
        self.client.remove_client(client["id"])
        self.assertNotIn(client, self.client.data)


if __name__ == '__main__':
    unittest.main()
