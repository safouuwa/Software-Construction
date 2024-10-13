import json

from models.base import Base

TRANSFERS = []


class Transfers(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "transfers.json"
        self.load(is_debug)

    def get_transfers(self):
        return self.data

    def get_transfer(self, transfer_id):
        for x in self.data:
            if x["id"] == transfer_id:
                return x
        return None

    def get_items_in_transfer(self, transfer_id):
        for x in self.data:
            if x["id"] == transfer_id:
                return x["items"]
        return None

    def add_transfer(self, transfer):
        for x in self.data:
            if x["id"] == transfer["id"]:
                return False
        transfer["transfer_status"] = "Scheduled"
        transfer["created_at"] = self.get_timestamp()
        transfer["updated_at"] = self.get_timestamp()
        self.data.append(transfer)
        return True

    def update_transfer(self, transfer_id, transfer):
        if "id" in transfer:
            if transfer_id != transfer["id"]:
                return False
        transfer["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == transfer_id:
                self.data[i] = transfer
                return True

    def remove_transfer(self, transfer_id):
        transfer = self.get_transfer(transfer_id)
        if transfer is None: return False
        for x in self.data:
            if x["id"] == transfer_id:
                self.data.remove(x)
                return True

    def load(self, is_debug):
        if is_debug:
            self.data = TRANSFERS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
