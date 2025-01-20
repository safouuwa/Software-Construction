import httpx
import pytest
import os
import time
import json
from datetime import datetime, timedelta, timezone

BASE_URL = "http://localhost:3000/api/v2"
base_path = os.path.abspath(__file__)
for _ in range(4):
    base_path = os.path.dirname(base_path)

log_file_path = os.path.join(base_path, "C#api", "RequestLogs", "RequestLogs.log")
LOG_FILE_PATH = log_file_path.replace(os.sep, '/')
data_root = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "data").replace(os.sep, "/")

def GetJsonData(model):
        with open(os.path.join(data_root, f"{model}.json"), 'r') as file:
            data = json.load(file)
        return data

@pytest.fixture(scope="module")
def client():
    headers = {"API_KEY": "a1b2c3d4e5"}
    return httpx.Client(base_url=BASE_URL, headers=headers)

def read_log_file():
    with open(LOG_FILE_PATH, 'r') as file:
        return file.readlines()

def clear_log_file():
    open(LOG_FILE_PATH, 'w').close()

def test_logging_middleware_happy_path(client):
    clear_log_file()

    new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
    response = client.post("transfers", json=new_transfer)
    assert response.status_code == 201
    created_transfer = GetJsonData("transfers")[-1]
    response = client.delete(f"transfers/{created_transfer.pop('Id')}")

    new_shipment = {
            "Order_Id": 123,
            "Source_Id": 1,
            "Order_Date": "2024-11-14T16:10:14.227318",
            "Request_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Type": "Standard",
            "Shipment_Status": "Pending",
            "Notes": "Urgent delivery",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 2,
            "Total_Package_Weight": 5.5,
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM001", "Amount": 10},
                {"Item_Id": "ITEM002", "Amount": 5}
            ]
        }
    response = client.post("shipments", json=new_shipment)
    assert response.status_code == 201
    created_shipment = GetJsonData("shipments")[-1]
    shipment_id = created_shipment['Id']	
    response = client.patch(f"shipments/{shipment_id}", json={"Shipment_Status": "Shipped"})
    assert response.status_code == 200
    response = client.delete(f"shipments/{shipment_id}/force")

    new_item = {
            "Code": "CODE123",
            "Description": "This is a test item.",
            "Short_Description": "Test Item",
            "Upc_Code": "123456789012",
            "Model_Number": "MODEL123",
            "Commodity_Code": "COMMOD123",
            "Item_Line": 1,
            "Item_Group": 2,
            "Item_Type": 3,
            "Unit_Purchase_Quantity": 10,
            "Unit_Order_Quantity": 5,
            "Pack_Order_Quantity": 20,
            "Supplier_Id": 1,
            "Supplier_Code": "SUP123",
            "Supplier_Part_Number": "SUP123-PART001",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318"
        }
    response = client.post("items", json=new_item)
    assert response.status_code == 201
    created_item = GetJsonData("items")[-1]
    response = client.put(f"items/{created_item['Uid']}", json=new_item)
    assert response.status_code == 200
    response = client.delete(f"items/{created_item['Uid']}/force")
    
    time.sleep(1)
    logs = read_log_file()
    assert len(logs) == 5
    for log in logs:
        assert "POST" in log or "PUT" in log or "PATCH" in log
        assert any(model in log for model in ["transfers", "shipments", "orders", "items"])

def test_logging_middleware_non_happy_path(client):
    clear_log_file()

    # Test not included model
    new_item_group = {
            "Name": "New Item Group",
            "Description": "Description of the new item group",
            "Created_At": datetime.now().isoformat(),
            "Updated_At": datetime.now().isoformat()
        }
    response = client.post("item_groups", json=new_item_group)
    assert response.status_code == 201
    created_item_group = GetJsonData("item_groups")[-1]
    response = client.delete(f"item_groups/{created_item_group.pop('Id')}/force")
    
    time.sleep(1)  
    logs = read_log_file()
    assert len(logs) == 0

def test_refresh_log_file_happy(client):
    new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
    response = client.post("transfers", json=new_transfer)
    assert response.status_code == 201
    created_transfer = GetJsonData("transfers")[-1]
    response = client.delete(f"transfers/{created_transfer.pop('Id')}")
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/refresh", headers=headers)
    assert response.status_code == 200
    assert response.text == "Logfile successfully refreshed!"
    
    logs = read_log_file()
    assert len(logs) == 0

def test_refresh_log_file(client):
    new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
    response = client.post("transfers", json=new_transfer)
    assert response.status_code == 201
    created_transfer = GetJsonData("transfers")[-1]
    response = client.delete(f"transfers/{created_transfer.pop('Id')}")

    # Attempt to refresh log file as non-Admin (Warehouse Manager)
    headers = {"API_KEY": "f6g7h8i9j0"}

    response = client.get("/RequestLog/refresh", headers=headers)
    assert response.status_code == 401
    assert "Warehouse Manager cannot access this functionality" in response.text
    

    logs = read_log_file()
    assert len(logs) > 0

def test_invalid_api_key(client):
    headers = {"API_KEY": "invalid_key"}
    response = client.get("/transfers", headers=headers)
    assert response.status_code == 401

    time.sleep(1)
    logs = read_log_file()
    assert all("invalid_key" not in log for log in logs)

def test_filter_requests_by_model(client):
    clear_log_file()
    new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
    response = client.post("transfers", json=new_transfer)
    assert response.status_code == 201
    created_transfer = GetJsonData("transfers")[-1]
    response = client.delete(f"transfers/{created_transfer.pop('Id')}")
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/filter?model=transfers", headers=headers)
    assert response.status_code == 200
    
    logs = response.json()
    assert len(logs) == 1
    assert any("transfers" in log for log in logs)

def test_filter_requests_by_method(client):
    clear_log_file()
    new_shipment = {
            "Order_Id": 123,
            "Source_Id": 1,
            "Order_Date": "2024-11-14T16:10:14.227318",
            "Request_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Date": "2024-11-14T16:10:14.227318",
            "Shipment_Type": "Standard",
            "Shipment_Status": "Pending",
            "Notes": "Urgent delivery",
            "Carrier_Code": "UPS",
            "Carrier_Description": "United Parcel Service",
            "Service_Code": "Ground",
            "Payment_Type": "Prepaid",
            "Transfer_Mode": "Air",
            "Total_Package_Count": 2,
            "Total_Package_Weight": 5.5,
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM001", "Amount": 10},
                {"Item_Id": "ITEM002", "Amount": 5}
            ]
        }
    response = client.post("shipments", json=new_shipment)
    assert response.status_code == 201
    created_shipment = GetJsonData("shipments")[-1]
    shipment_id = created_shipment['Id']	
    response = client.patch(f"shipments/{shipment_id}", json={"Shipment_Status": "Shipped"})
    assert response.status_code == 200
    response = client.delete(f"shipments/{shipment_id}/force")
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/filter?method=patch", headers=headers)
    assert response.status_code == 200
    
    logs = response.json()
    assert len(logs) == 1
    assert any("PATCH" in log for log in logs)

def test_filter_requests_by_date(client):
    clear_log_file()
    new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
    response = client.post("transfers", json=new_transfer)
    assert response.status_code == 201
    created_transfer = GetJsonData("transfers")[-1]
    response = client.delete(f"transfers/{created_transfer.pop('Id')}")
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    current_date = (datetime.now(timezone.utc) + timedelta(hours=1)).strftime("%d-%m-%Y")
    response = client.get(f"/RequestLog/filter?date={current_date}", headers=headers)
    assert response.status_code == 200
    
    logs = response.json()
    assert len(logs) == 1
    for log in logs:
        assert current_date in log

def test_filter_requests_no_results(client):
    new_transfer = {
            "Reference": "TRANS123",
            "Transfer_From": 1,
            "Transfer_To": 2,
            "Transfer_Status": "Scheduled",
            "Created_At": "2024-11-14T16:10:14.227318",
            "Updated_At": "2024-11-14T16:10:14.227318",
            "Items": [
                {"Item_Id": "ITEM123", "Amount": 100},
                {"Item_Id": "ITEM456", "Amount": 50}
            ]
        }
    response = client.post("transfers", json=new_transfer)
    assert response.status_code == 201
    created_transfer = GetJsonData("transfers")[-1]
    response = client.delete(f"transfers/{created_transfer.pop('Id')}")
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/filter?model=nonexistent", headers=headers)
    assert response.status_code == 204

def test_filter_requests_invalid_api_key(client):
    headers = {"API_KEY": "invalid_key"}
    response = client.get("/RequestLog/filter?model=transfers", headers=headers)
    assert response.status_code == 401
    clear_log_file()

if __name__ == "__main__":
    pytest.main([__file__])