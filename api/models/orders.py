import json

from models.base import Base
from models.inventories import Inventories
from providers import data_provider

ORDERS = []


class Orders(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "orders.json"
        self.load(is_debug)
        self.inventory = Inventories(root_path)

    def get_orders(self):
        return self.data

    def get_order(self, order_id):
        for x in self.data:
            if x["id"] == order_id:
                return x
        return None

    def get_items_in_order(self, order_id):
        for x in self.data:
            if x["id"] == order_id:
                return x["items"]
        return None

    def get_orders_in_shipment(self, shipment_id):
        result = []
        for x in self.data:
            if x["shipment_id"] == shipment_id:
                result.append(x)
        return result

    def get_orders_for_client(self, client_id):
        result = []
        for x in self.data:
            if x["ship_to"] == client_id or x["bill_to"] == client_id:
                result.append(x)
        return result

    def add_order(self, order):
        for x in self.data:
            if x["id"] == order["id"]:
                return False
        order["created_at"] = self.get_timestamp()
        order["updated_at"] = self.get_timestamp()
        self.data.append(order)
        return True

    def update_order(self, order_id, order):
        if "id" in order:
            if order_id != order["id"]:
                return False
        order["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == order_id:
                self.data[i] = order
                return True

    def update_items_in_order(self, order_id, items):
        order = self.get_order(order_id)
        current = order["items"]
        for x in current:
            found = False
            for y in items:
                if x["item_id"] == y["item_id"]:
                    found = True
                    break
            if not found:
                inventories = self.inventory.get_inventories_for_item(x["item_id"])
                min_ordered = 1_000_000_000_000_000_000
                min_inventory = min(inventories, key=lambda z: z["total_allocated"])
                for z in inventories:
                    if z["total_allocated"] > min_ordered:
                        min_ordered = z["total_allocated"]
                        min_inventory = z
                min_inventory["total_allocated"] -= x["amount"]
                min_inventory["total_expected"] = z["total_on_hand"] + z["total_ordered"]
                self.inventory.update_inventory(min_inventory["id"], min_inventory)
        for x in current:
            for y in items:
                if x["item_id"] == y["item_id"]:
                    inventories = self.inventory.get_inventories_for_item(x["item_id"])
                    min_ordered = 1_000_000_000_000_000_000
                    min_inventory = min(inventories, key=lambda z: z["total_allocated"])
                    for z in inventories:
                        if z["total_allocated"] < min_ordered:
                            min_ordered = z["total_allocated"]
                            min_inventory = z
                min_inventory["total_allocated"] += y["amount"] - x["amount"]
                min_inventory["total_expected"] = z["total_on_hand"] + z["total_ordered"]
                self.inventory.update_inventory(min_inventory["id"], min_inventory)
        order["items"] = items
        self.update_order(order_id, order)

    def update_orders_in_shipment(self, shipment_id, orders):
        packed_orders = self.get_orders_in_shipment(shipment_id)
        for x in packed_orders:
            if x not in orders:
                order = self.get_order(x)
                order["shipment_id"] = -1
                order["order_status"] = "Scheduled"
                self.update_order(x, order)
        for x in orders:
            order = self.get_order(x)
            order["shipment_id"] = shipment_id
            order["order_status"] = "Packed"
            self.update_order(x, order)

    def remove_order(self, order_id):
        order = self.get_order(order_id)
        if order is None: return False
        for x in self.data:
            if x["id"] == order_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = ORDERS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
