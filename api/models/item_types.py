import json

from models.base import Base
from providers import data_provider

ITEM_TYPES = []


class ItemTypes(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "item_types.json"
        self.load(is_debug)

    def get_item_types(self):
        return self.data

    def get_item_type(self, item_type_id):
        for x in self.data:
            if x["id"] == item_type_id:
                return x
        return None

    def add_item_type(self, item_type):
        for x in self.data:
            if x["id"] == item_type["id"]:
                return False
        item_type["created_at"] = self.get_timestamp()
        item_type["updated_at"] = self.get_timestamp()
        self.data.append(item_type)
        return True

    def update_item_type(self, item_type_id, item_type):
        if "id" in item_type:
            if item_type_id != item_type["id"]:
                return False
        item_type["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == item_type_id:
                self.data[i] = item_type
                return True

    def remove_item_type(self, item_type_id):
        item_type = self.get_item_type(item_type_id)
        if item_type is None: return False
        items = data_provider.fetch_item_pool().get_items_for_item_type(item_type_id)
        if len(items) != 0: return False
        for x in self.data:
            if x["id"] == item_type_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = ITEM_TYPES
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
