import json

from models.base import Base
from providers import data_provider

ITEMS = []


class Items(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "items.json"
        self.load(is_debug)

    def get_items(self):
        return self.data

    def get_item(self, item_id):
        for x in self.data:
            if x["uid"] == item_id:
                return x
        return None

    def get_items_for_item_line(self, item_line_id):
        result = []
        for x in self.data:
            if x["item_line"] == item_line_id:
                result.append(x)
        return result

    def get_items_for_item_group(self, item_group_id):
        result = []
        for x in self.data:
            if x["item_group"] == item_group_id:
                result.append(x)
        return result

    def get_items_for_item_type(self, item_type_id):
        result = []
        for x in self.data:
            if x["item_type"] == item_type_id:
                result.append(x)
        return result

    def get_items_for_supplier(self, supplier_id):
        result = []
        for x in self.data:
            if x["supplier_id"] == supplier_id:
                result.append(x)
        return result

    def add_item(self, item):
        for x in self.data:
            if x["uid"] == item["uid"]:
                return False
        item["created_at"] = self.get_timestamp()
        item["updated_at"] = self.get_timestamp()
        self.data.append(item)
        return True

    def update_item(self, item_id, item):
        if "id" in item:
            if item_id != item["id"]:
                return False
        item["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["uid"] == item_id:
                self.data[i] = item
                return True

    def remove_item(self, item_id):
        item = self.get_item(item_id)
        if item is None: return False
        orders = data_provider.fetch_order_pool().get_orders()
        for y in orders:
            for items in y["items"]:
                for z in items:
                    if z == x:
                        return False
        shipments = data_provider.fetch_shipment_pool().get_shipments()
        for y in shipments:
            for items in y["items"]:
                for z in items:
                    if z == x:
                        return False
        transfers = data_provider.fetch_transfer_pool().get_transfers()
        for y in transfers:
            for items in y["items"]:
                for z in items:
                    if z == x:
                        return False
        for x in self.data:
            if x["uid"] == item_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = ITEMS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
