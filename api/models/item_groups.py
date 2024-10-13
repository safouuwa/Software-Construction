import json

from models.base import Base
from providers import data_provider

ITEM_GROUPS = []


class ItemGroups(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "item_groups.json"
        self.load(is_debug)

    def get_item_groups(self):
        return self.data

    def get_item_group(self, item_group_id):
        for x in self.data:
            if x["id"] == item_group_id:
                return x
        return None

    def add_item_group(self, item_group):
        for x in self.data:
            if x["id"] == item_group["id"]:
                return False
        item_group["created_at"] = self.get_timestamp()
        item_group["updated_at"] = self.get_timestamp()
        self.data.append(item_group)
        return True

    def update_item_group(self, item_group_id, item_group):
        if "id" in item_group:
            if item_group_id != item_group["id"]:
                return False
        item_group["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == item_group_id:
                self.data[i] = item_group
                return True

    def remove_item_group(self, item_group_id):
        item_group = self.get_item_group(item_group_id)
        if item_group is None: return False
        items = data_provider.fetch_item_pool().get_items_for_item_group(item_group_id)
        if len(items) != 0: return False
        for x in self.data:
            if x["id"] == item_group_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = ITEM_GROUPS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
