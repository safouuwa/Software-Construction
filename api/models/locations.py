import json

from models.base import Base
from providers import data_provider

LOCATIONS = []


class Locations(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "locations.json"
        self.load(is_debug)

    def get_locations(self):
        return self.data

    def get_location(self, location_id):
        for x in self.data:
            if x["id"] == location_id:
                return x
        return None

    def get_locations_in_warehouse(self, warehouse_id):
        result = []
        for x in self.data:
            if x["warehouse_id"] == warehouse_id:
                result.append(x)
        return result

    def add_location(self, location):
        for x in self.data:
            if x["id"] == location["id"]:
                return False
        location["created_at"] = self.get_timestamp()
        location["updated_at"] = self.get_timestamp()
        self.data.append(location)
        return True

    def update_location(self, location_id, location):
        if "id" in location:
            if location_id != location["id"]:
                return False
        location["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == location_id:
                self.data[i] = location
                return True

    def remove_location(self, location_id):
        location = self.get_location(location_id)
        if location is None: return False
        inventories = data_provider.fetch_inventory_pool().get_inventories()
        for y in inventories:
            for z in y["locations"]:
                for a in z:
                    if a == location:
                        return False
        for x in self.data:
            if x["id"] == location_id:
                self.data.remove(x)

    def load(self, is_debug):
        if is_debug:
            self.data = LOCATIONS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
