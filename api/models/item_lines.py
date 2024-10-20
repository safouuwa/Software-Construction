import json

from models.base import Base
from providers import data_provider

ITEM_LINES = []


class ItemLines(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "item_lines.json"
        self.load(is_debug)

    def get_item_lines(self):
        return self.data

    def get_item_line(self, item_line_id):
        for x in self.data:
            if x["id"] == item_line_id:
                return x
        return None

    def add_item_line(self, item_line):
        for x in self.data:
            if x["id"] == item_line["id"]:
                return False
        item_line["created_at"] = self.get_timestamp()
        item_line["updated_at"] = self.get_timestamp()
        self.data.append(item_line)
        return True

    def update_item_line(self, item_line_id, item_line):
        if "id" in item_line:
            if item_line_id != item_line["id"]:
                return False
        item_line["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == item_line_id:
                self.data[i] = item_line
                return True

    def remove_item_line(self, item_line_id):
        item_line = self.get_item_line(item_line_id)
        if item_line is None: return False
        items = data_provider.fetch_item_pool().get_items_for_item_line(item_line_id)
        if len(items) != 0: return False
        for x in self.data:
            if x["id"] == item_line_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = ITEM_LINES
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
