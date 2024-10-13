import json

from models.base import Base
from providers import data_provider

WAREHOUSES = []


class Warehouses(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "warehouses.json"
        self.load(is_debug)

    def get_warehouses(self):
        return self.data

    def get_warehouse(self, warehouse_id):
        for x in self.data:
            if x["id"] == warehouse_id:
                return x
        return None

    def add_warehouse(self, warehouse):
        for x in self.data:
            if x["id"] == warehouse["id"]:
                return False
        warehouse["created_at"] = self.get_timestamp()
        warehouse["updated_at"] = self.get_timestamp()
        self.data.append(warehouse)
        return True

    def update_warehouse(self, warehouse_id, warehouse):
        if "id" in warehouse:
            if warehouse_id != warehouse["id"]:
                return False
        warehouse["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == warehouse_id:
                self.data[i] = warehouse
                return True

    def remove_warehouse(self, warehouse_id):
        warehouse = self.get_warehouse(warehouse_id)
        if warehouse is None: return False
        orders = data_provider.fetch_order_pool().get_orders()
        for y in orders:
            if y["warehouse_id"] == warehouse_id:
                return False
        for x in self.data:
            if x["id"] == warehouse_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = WAREHOUSES
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
