import unittest
import json
import os

from api.models.clients import Clients


class TestClients(unittest.TestCase):
    def setUp(self):
        self.client = Clients()
        root_path = os.path.join(
            os.path.dirname(os.path.dirname(os.path.dirname(__file__))),
            'data/')
        root_path = root_path.replace('\\', '/')
        self.test_file = root_path + 'clients.json'

    def test_get_clients(self):
        self.assertEqual(self.client.get_clients(), self.client.data)

    def test_get_client(self):
        self.assertEqual(self.client.get_client(1), self.client.data[0])

    def test_get_client_wrongid(self):
        self.assertEqual(self.client.get_clients(-1), None)

    def test_add_client(self):
        client = self.client.data[0]
        client["id"] = 0
        self.client.add_client(client)
        self.assertIn(client, self.client.data)
        client = self.client.get_client(0)
        self.assertEqual(client, self.client.data)

    def test_update_client(self):
        client = self.client.data[0]
        client["name"] = "test"
        self.client.update_client(client["id"], client)
        self.assertEqual(self.client.data[0], client)

    def test_remove_client(self):
        client = self.client.data[0]
        self.client.remove_client(client["id"])
        self.assertIn(client, self.client.data)


if __name__ == '__main__':
    unittest.main()
