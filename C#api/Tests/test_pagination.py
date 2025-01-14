import unittest
from datetime import datetime

class PaginationHelper:
    @staticmethod
    def paginate(source, page, page_size):
        total_items = len(source)
        paginated_items = source[(page - 1) * page_size: page * page_size]
        return {
            'TotalCount': total_items,
            'Page': page,
            'PageSize': page_size,
            'Items': paginated_items
        }

class TestPaginationHelper(unittest.TestCase):
    def test_paginate_normal_list(self):
        source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
        result = PaginationHelper.paginate(source, 2, 3)
        self.assertEqual(result['TotalCount'], 10)
        self.assertEqual(result['Page'], 2)
        self.assertEqual(result['PageSize'], 3)
        self.assertEqual(result['Items'], [4, 5, 6])

    def test_paginate_empty_list(self):
        source = []
        result = PaginationHelper.paginate(source, 1, 3)
        self.assertEqual(result['TotalCount'], 0)
        self.assertEqual(result['Page'], 1)
        self.assertEqual(result['PageSize'], 3)
        self.assertEqual(result['Items'], [])

    def test_paginate_single_item(self):
        source = [1]
        result = PaginationHelper.paginate(source, 1, 3)
        self.assertEqual(result['TotalCount'], 1)
        self.assertEqual(result['Page'], 1)
        self.assertEqual(result['PageSize'], 3)
        self.assertEqual(result['Items'], [1])

    def test_paginate_page_size_larger_than_list(self):
        source = [1, 2, 3]
        result = PaginationHelper.paginate(source, 1, 5)
        self.assertEqual(result['TotalCount'], 3)
        self.assertEqual(result['Page'], 1)
        self.assertEqual(result['PageSize'], 5)
        self.assertEqual(result['Items'], [1, 2, 3])

    def test_paginate_page_number_exceeds_total_pages(self):
        source = [1, 2, 3, 4, 5]
        result = PaginationHelper.paginate(source, 3, 2)
        self.assertEqual(result['TotalCount'], 5)
        self.assertEqual(result['Page'], 3)
        self.assertEqual(result['PageSize'], 2)
        self.assertEqual(result['Items'], [5])
        
if __name__ == '__main__':
    unittest.main()