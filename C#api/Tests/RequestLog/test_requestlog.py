import httpx
import pytest
import os
import time
from datetime import datetime, timedelta

BASE_URL = "http://localhost:3000/api/v1"
base_path = os.path.abspath(__file__)
for _ in range(4):
    base_path = os.path.dirname(base_path)

log_file_path = os.path.join(base_path, "C#api", "RequestLogs", "RequestLogs.txt")
LOG_FILE_PATH = log_file_path.replace(os.sep, '/')

@pytest.fixture(scope="module")
def client():
    return httpx.Client(base_url=BASE_URL)

def read_log_file():
    with open(LOG_FILE_PATH, 'r') as file:
        return file.readlines()

def clear_log_file():
    open(LOG_FILE_PATH, 'w').close()

def test_logging_middleware_happy_path(client):
    clear_log_file()
    
    # Test with Warehouse Manager
    headers = {"API_KEY": "f6g7h8i9j0"}
    
    # Test transfers endpoint
    response = client.get("/transfers", headers=headers)
    assert response.status_code == 200
    
    # Test shipments endpoint
    response = client.get("/shipments", headers=headers)
    assert response.status_code == 200
    
    # Test orders endpoint
    response = client.get("/orders", headers=headers)
    assert response.status_code == 200
    
    # Test items endpoint
    response = client.get("/items", headers=headers)
    assert response.status_code == 200
    
    time.sleep(1)
    logs = read_log_file()
    assert len(logs) == 4
    for log in logs:
        assert "Warehouse Manager" in log
        assert any(model in log for model in ["transfers", "shipments", "orders", "items"])
        assert "StatusCode: 200" in log or "StatusCode: 201" in log
        assert "Date and Time:" in log

def test_logging_middleware_non_happy_path(client):
    clear_log_file()
    
    # Operative (limited access)
    headers = {"API_KEY": "u1v2w3x4y5"}
    
    # Test unauthorized access to shipments
    response = client.delete("/shipments/1", headers=headers)
    assert response.status_code == 401
    
    # Test non-existent endpoint
    response = client.get("/non_existent", headers=headers)
    assert response.status_code == 404
    

    time.sleep(1)  
    logs = read_log_file()
    assert len(logs) == 1 
    assert "Operative" in logs[0]
    assert "shipments" in logs[0]
    assert "StatusCode: 401" in logs[0]

def test_refresh_log_file_happy(client):
    test_logging_middleware_happy_path(client)
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/refresh", headers=headers)
    assert response.status_code == 200
    assert response.text == "Logfile successfully refreshed!"
    
    logs = read_log_file()
    assert len(logs) == 0

def test_refresh_log_file(client):

    # Attempt to refresh log file as non-Admin (Warehouse Manager)
    headers = {"API_KEY": "f6g7h8i9j0"}

    response = client.get("/transfers", headers=headers)
    assert response.status_code == 200

    response = client.get("/RequestLog/refresh", headers=headers)
    assert response.status_code == 401
    assert "Warehouse Manager cannot access this functionality" in response.text
    

    logs = read_log_file()
    assert len(logs) > 0

    test_refresh_log_file_happy(client) # to refresh changes

def test_invalid_api_key(client):
    headers = {"API_KEY": "invalid_key"}
    response = client.get("/transfers", headers=headers)
    assert response.status_code == 401

    time.sleep(1)
    logs = read_log_file()
    assert all("invalid_key" not in log for log in logs)

def test_filter_requests_by_model(client):
        clear_log_file()
        test_logging_middleware_happy_path(client)
        
        headers = {"API_KEY": "a1b2c3d4e5"}
        response = client.get("/RequestLog/filter?model=transfers", headers=headers)
        assert response.status_code == 200
        
        logs = response.json()
        assert len(logs) == 1
        assert "transfers" in logs[0]

def test_filter_requests_by_user(client):
    clear_log_file()
    test_logging_middleware_happy_path(client)
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/filter?user=Warehouse%20Manager", headers=headers)
    assert response.status_code == 200
    
    logs = response.json()
    assert len(logs) == 4
    for log in logs:
        assert "Warehouse Manager" in log

def test_filter_requests_by_date(client):
    clear_log_file()
    test_logging_middleware_happy_path(client)
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    date_str = datetime.now().strftime("%Y-%m-%d")
    response = client.get(f"/RequestLog/filter?date={date_str}", headers=headers)
    assert response.status_code == 200
    
    logs = response.json()
    assert len(logs) == 4
    for log in logs:
        assert date_str in log

def test_filter_requests_no_results(client):
    clear_log_file()
    test_logging_middleware_happy_path(client)
    
    headers = {"API_KEY": "a1b2c3d4e5"}
    response = client.get("/RequestLog/filter?model=nonexistent", headers=headers)
    assert response.status_code == 404
    assert "No logs found with the specified criteria." in response.text

def test_filter_requests_invalid_api_key(client):
    headers = {"API_KEY": "invalid_key"}
    response = client.get("/RequestLog/filter?model=transfers", headers=headers)
    assert response.status_code == 401

time.sleep(1)
logs = read_log_file()
assert all("invalid_key" not in log for log in logs)

if __name__ == "__main__":
    pytest.main([__file__])