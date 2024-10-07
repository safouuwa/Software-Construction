import unittest
from unittest.mock import patch
import json
from api.models.locations import LOCATIONS, Locations
import os


class Testlocations(unittest.TestCase):
    def setUp(self):
        root_path = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(__file__))), 'data/')
        root_path = root_path.replace('\\', '/')
        self.test_file = root_path + 'locations.json'
        self.location = Locations(root_path, is_debug=True)

    @patch('api.models.locations.get_timestamp', return_value="2023-01-01 00:00:00")
    def test_add_location(self, mock_timestamp):
        location = {"id": 2, "name": "location 2"}
        expected_location = {"id": 2, "name": "location 2", "created_at": "2023-01-01 00:00:00", "updated_at": "2023-01-01 00:00:00"}
        self.location.add_location(location)
        self.assertIn(expected_location, self.location.data)


    def test_get_location(self):
        location_id = 1
        self.location.data = [{"id": 1, "name": "location 1"}]
        self.assertEqual(self.location.get_location(location_id), self.location.data[0])

    def test_get_locations(self):
        self.location.data = [{"id": 1, "name": "location 1"}, {"id": 1, "name": "location 2"}]
        self.assertEqual(self.location.get_locations(), self.location.data)

    def test_get_location_wrongid(self):
        location_id = -1
        self.location.data = [{"id": 1, "name": "location 1"}]
        self.assertEqual(self.location.get_location(location_id), None)

    def test_get_locations_in_warehouse(self):
        warehouse_id = 1
        self.location.data = [{"id": 1, "warehouse_id": 1, "name": "WAREHOUSE1"}, {"id": 2, "warehouse_id": 1, "name": "WAREHOUSE1"}]
        expected = [{"id": 1, "warehouse_id": 1, "name": "WAREHOUSE1"}]
        self.assertEqual(self.location.get_locations_in_warehouse(warehouse_id), expected)

    def test_update_location(self):
        location_id = 1
        self.location.data = [{"id": 1, "name": "location 1"}]
        updated_location = {"id": 1, "name": "Updated location 1"}
        self.location.update_location(location_id, updated_location)
        self.assertEqual(self.location.data[0], updated_location)

    def test_remove_location(self):
        location_id = 1
        self.location.data = [{"id": 1, "name": "location 1"}]
        self.location.remove_location(location_id)
        self.assertNotIn({"id": 1, "name": "location 1"}, self.location.data)

    def test_load(self):
        self.location.load(is_debug=True)
        self.assertEqual(self.location.data, self.LOCATIONS)

    def test_save(self):
        self.location.data = [{"id": 1, "name": "location 1"}]
        self.location.save()
        with open(self.test_file, "r") as f:
            data = json.load(f)
        self.assertEqual(data, self.location.data)

if __name__ == '__main__':
    unittest.main()
