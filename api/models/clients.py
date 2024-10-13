import json

from models.base import Base
from providers import data_provider

CLIENTS = []


class Clients(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "clients.json"
        self.load(is_debug)

    def get_clients(self):
        return self.data

    def get_client(self, client_id):
        for x in self.data:
            if x["id"] == client_id:
                return x
        return None

    def add_client(self, client):
        for x in self.data:
            if x["id"] == client["id"]:
                return False
        client["created_at"] = self.get_timestamp()
        client["updated_at"] = self.get_timestamp()
        self.data.append(client)
        return True

    def update_client(self, client_id, client):
        if "id" in client:
            if client_id != client["id"]:
                return False
        client["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == client_id:
                self.data[i] = client
                break

    def remove_client(self, client_id):
        client = self.get_client(client_id)
        if client is None: return False
        orders = data_provider.fetch_order_pool().get_orders()
        for x in orders:
            if x["ship_to"] == client_id or x["bill_to"] == client_id:
                return False
        for x in self.data:
            if x["id"] == client_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = CLIENTS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
