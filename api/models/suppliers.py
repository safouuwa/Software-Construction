import json

from models.base import Base
from providers import data_provider

SUPPLIERS = []


class Suppliers(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "suppliers.json"
        self.load(is_debug)

    def get_suppliers(self):
        return self.data

    def get_supplier(self, supplier_id):
        for x in self.data:
            if x["id"] == supplier_id:
                return x
        return None

    def add_supplier(self, supplier):
        for x in self.data:
            if x["id"] == supplier["id"]:
                return False
        supplier["created_at"] = self.get_timestamp()
        supplier["updated_at"] = self.get_timestamp()
        self.data.append(supplier)
        return True

    def update_supplier(self, supplier_id, supplier):
        if "id" in supplier:
            if supplier_id != supplier["id"]:
                return False
        supplier["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == supplier_id:
                self.data[i] = supplier
                return True

    def remove_supplier(self, supplier_id):
        supplier = self.get_supplier(supplier_id)
        if supplier is None: return False
        items = data_provider.fetch_item_pool().get_items()
        for y in items:
            if y["supplier_id"] == supplier_id:
                return False
        for x in self.data:
            if x["id"] == supplier_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = SUPPLIERS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
