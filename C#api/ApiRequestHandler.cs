using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Models;

using Newtonsoft.Json;

public class ApiRequestHandler
{
    public void HandleGet(HttpListenerRequest request, HttpListenerResponse response)
    {
        string apiKey = request.Headers["API_KEY"];
        var user = AuthProvider.GetUser(apiKey);

        if (user == null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Close();
            return;
        }

        try
        {
            string[] path = request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length > 2 && path[0] == "api" && path[1] == "v1")
            {
                HandleGetVersion1(path[2..], user, response);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
            }
        }
        catch (Exception)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Close();
        }
    }

    private void HandleGetVersion1(string[] path, User user, HttpListenerResponse response)
    {
        if (!AuthProvider.HasAccess(user, path[0], "get"))
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.Close();
            return;
        }

        switch (path[0])
        {
            case "warehouses":
                HandleWarehouses(path, response);
                break;
            case "locations":
                HandleLocations(path, response);
                break;
            case "transfers":
                HandleTransfers(path, response);
                break;
            case "items":
                HandleItems(path, response);
                break;
            case "item_lines":
                HandleItemLines(path, response);
                break;
            case "item_groups":
                HandleItemGroups(path, response);
                break;
            case "item_types":
                HandleItemTypes(path, response);
                break;
            case "inventories":
                HandleInventories(path, response);
                break;
            case "suppliers":
                HandleSuppliers(path, response);
                break;
            case "orders":
                HandleOrders(path, response);
                break;
            case "clients":
                HandleClients(path, response);
                break;
            case "shipments":
                HandleShipments(path, response);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleWarehouses(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var warehouses = DataProvider.fetch_warehouse_pool().GetWarehouses();
                SendResponse(response, warehouses);
                break;
            case 2:
                int warehouseId = int.Parse(path[1]);
                var warehouse = DataProvider.fetch_warehouse_pool().GetWarehouse(warehouseId);
                SendResponse(response, warehouse);
                break;
            case 3 when path[2] == "locations":
                warehouseId = int.Parse(path[1]);
                var locations = DataProvider.fetch_location_pool().GetLocationsInWarehouse(warehouseId);
                SendResponse(response, locations);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleLocations(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var locations = DataProvider.fetch_location_pool().GetLocations();
                SendResponse(response, locations);
                break;
            case 2:
                int locationId = int.Parse(path[1]);
                var location = DataProvider.fetch_location_pool().GetLocation(locationId);
                SendResponse(response, location);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleTransfers(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var transfers = DataProvider.fetch_transfer_pool().GetTransfers();
                SendResponse(response, transfers);
                break;
            case 2:
                int transferId = int.Parse(path[1]);
                var transfer = DataProvider.fetch_transfer_pool().GetTransfer(transferId);
                SendResponse(response, transfer);
                break;
            case 3 when path[2] == "items":
                transferId = int.Parse(path[1]);
                var items = DataProvider.fetch_transfer_pool().GetItemsInTransfer(transferId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleItems(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var items = DataProvider.fetch_item_pool().GetItems();
                SendResponse(response, items);
                break;
            case 2:
                var itemId = path[1];
                var item = DataProvider.fetch_item_pool().GetItem(itemId);
                SendResponse(response, item);
                break;
            case 3 when path[2] == "inventory":
                itemId = path[1];
                var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(itemId);
                SendResponse(response, inventories);
                break;
            case 4 when path[2] == "inventory" && path[3] == "totals":
                itemId = path[1];
                var totals = DataProvider.fetch_inventory_pool().GetInventoryTotalsForItem(itemId);
                SendResponse(response, totals);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleItemLines(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var itemLines = DataProvider.fetch_itemline_pool().GetItemLines();
                SendResponse(response, itemLines);
                break;
            case 2:
                int itemLineId = int.Parse(path[1]);
                var itemLine = DataProvider.fetch_itemline_pool().GetItemLine(itemLineId);
                SendResponse(response, itemLine);
                break;
            case 3 when path[2] == "items":
                itemLineId = int.Parse(path[1]);
                var items = DataProvider.fetch_item_pool().GetItemsForItemLine(itemLineId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleItemGroups(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var itemGroups = DataProvider.fetch_itemgroup_pool().GetItemGroups();
                SendResponse(response, itemGroups);
                break;
            case 2:
                int itemGroupId = int.Parse(path[1]);
                var itemGroup = DataProvider.fetch_itemgroup_pool().GetItemGroup(itemGroupId);
                SendResponse(response, itemGroup);
                break;
            case 3 when path[2] == "items":
                itemGroupId = int.Parse(path[1]);
                var items = DataProvider.fetch_item_pool().GetItemsForItemGroup(itemGroupId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleItemTypes(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var itemTypes = DataProvider.fetch_itemtype_pool().GetItemTypes();
                SendResponse(response, itemTypes);
                break;
            case 2:
                int itemTypeId = int.Parse(path[1]);
                var itemType = DataProvider.fetch_itemtype_pool().GetItemType(itemTypeId);
                SendResponse(response, itemType);
                break;
            case 3 when path[2] == "items":
                itemTypeId = int.Parse(path[1]);
                var items = DataProvider.fetch_item_pool().GetItemsForItemType(itemTypeId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleInventories(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var inventories = DataProvider.fetch_inventory_pool().GetInventories();
                SendResponse(response, inventories);
                break;
            case 2:
                int inventoryId = int.Parse(path[1]);
                var inventory = DataProvider.fetch_inventory_pool().GetInventory(inventoryId);
                SendResponse(response, inventory);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleSuppliers(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var suppliers = DataProvider.fetch_supplier_pool().GetSuppliers();
                SendResponse(response, suppliers);
                break;
            case 2:
                int supplierId = int.Parse(path[1]);
                var supplier = DataProvider.fetch_supplier_pool().GetSupplier(supplierId);
                SendResponse(response, supplier);
                break;
            case 3 when path[2] == "items":
                supplierId = int.Parse(path[1]);
                var items = DataProvider.fetch_item_pool().GetItemsForSupplier(supplierId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleOrders(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var orders = DataProvider.fetch_order_pool().GetOrders();
                SendResponse(response, orders);
                break;
            case 2:
                int orderId = int.Parse(path[1]);
                var order = DataProvider.fetch_order_pool().GetOrder(orderId);
                SendResponse(response, order);
                break;
            case 3 when path[2] == "items":
                orderId = int.Parse(path[1]);
                var items = DataProvider.fetch_order_pool().GetItemsInOrder(orderId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleClients(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var clients = DataProvider.fetch_client_pool().GetClients();
                SendResponse(response, clients);
                break;
            case 2:
                int clientId = int.Parse(path[1]);
                var client = DataProvider.fetch_client_pool().GetClient(clientId);
                SendResponse(response, client);
                break;
            case 3 when path[2] == "orders":
                clientId = int.Parse(path[1]);
                var orders = DataProvider.fetch_order_pool().GetOrdersForClient(clientId);
                SendResponse(response, orders);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandleShipments(string[] path, HttpListenerResponse response)
    {
        switch (path.Length)
        {
            case 1:
                var shipments = DataProvider.fetch_shipment_pool().GetShipments();
                SendResponse(response, shipments);
                break;
            case 2:
                int shipmentId = int.Parse(path[1]);
                var shipment = DataProvider.fetch_shipment_pool().GetShipment(shipmentId);
                SendResponse(response, shipment);
                break;
            case 3 when path[2] == "orders":
                shipmentId = int.Parse(path[1]);
                var orders = DataProvider.fetch_order_pool().GetOrdersInShipment(shipmentId);
                SendResponse(response, orders);
                break;
            case 3 when path[2] == "items":
                shipmentId = int.Parse(path[1]);
                var items = DataProvider.fetch_shipment_pool().GetItemsInShipment(shipmentId);
                SendResponse(response, items);
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    public void HandleDelete(HttpListenerRequest request, HttpListenerResponse response)
    {
        string apiKey = request.Headers["API_KEY"];
        var user = AuthProvider.GetUser(apiKey);

        if (user == null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Close();
            return;
        }

        try
        {
            string[] path = request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length > 2 && path[0] == "api" && path[1] == "v1")
            {
                HandleDeleteVersion1(path[2..], user, response);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
            }
        }
        catch (Exception)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Close();
        }
    }

    public void HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        string apiKey = request.Headers["API_KEY"];
        var user = AuthProvider.GetUser(apiKey);

        if (user == null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Close();
            return;
        }

        try
        {
            string[] path = request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length > 2 && path[0] == "api" && path[1] == "v1")
            {
                HandlePostVersion1(path[2..], user, request, response);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
            }
        }
        catch (Exception)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Close();
        }
    }

    public void HandlePut(HttpListenerRequest request, HttpListenerResponse response)
    {
        string apiKey = request.Headers["API_KEY"];
        var user = AuthProvider.GetUser(apiKey);

        if (user == null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Close();
            return;
        }

        try
        {
            string[] path = request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (path.Length > 2 && path[0] == "api" && path[1] == "v1")
            {
                HandlePutVersion1(path[2..], user, request, response);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
            }
        }
        catch (Exception)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Close();
        }
    }


    private void HandlePostVersion1(string[] path, User user, HttpListenerRequest request, HttpListenerResponse response)
    {
        if (!AuthProvider.HasAccess(user, path[0], "post"))
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.Close();
            return;
        }

        int contentLength = (int)request.ContentLength64;
        byte[] buffer = new byte[contentLength];
        request.InputStream.Read(buffer, 0, contentLength);
        string postData = Encoding.UTF8.GetString(buffer);

        bool check;
        switch (path[0])
        {
            case "warehouses":
                var newWarehouse = JsonConvert.DeserializeObject<Warehouse>(postData);
                if (newWarehouse.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_warehouse_pool().AddWarehouse(newWarehouse);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_warehouse_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "locations":
                var newLocation = JsonConvert.DeserializeObject<Location>(postData);
                if (newLocation.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_location_pool().AddLocation(newLocation);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_location_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "transfers":
                var newTransfer = JsonConvert.DeserializeObject<Transfer>(postData);
                if (newTransfer.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_transfer_pool().AddTransfer(newTransfer);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_transfer_pool().Save();
                var notificationSystem = new NotificationSystem();
                notificationSystem.Push($"Scheduled batch transfer {newTransfer.Id}");
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "items":
                var newItem = JsonConvert.DeserializeObject<Item>(postData);
                if (newItem.Uid == null)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_item_pool().AddItem(newItem);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_item_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "item_lines":
                var newItemLine = JsonConvert.DeserializeObject<ItemLine>(postData);
                if (newItemLine.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_itemline_pool().AddItemline(newItemLine);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_itemline_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "item_types":
                var newItemType = JsonConvert.DeserializeObject<ItemType>(postData);
                if (newItemType.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_itemtype_pool().AddItemtype(newItemType);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_itemtype_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "item_groups":
                var newItemGroup = JsonConvert.DeserializeObject<ItemGroup>(postData);
                if (newItemGroup.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_itemgroup_pool().AddItemGroup(newItemGroup);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_itemgroup_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "inventories":
                var newInventory = JsonConvert.DeserializeObject<Inventory>(postData);
                if (newInventory.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_inventory_pool().AddInventory(newInventory);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_inventory_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "suppliers":
                var newSupplier = JsonConvert.DeserializeObject<Supplier>(postData);
                if (newSupplier.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_supplier_pool().AddSupplier(newSupplier);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_supplier_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "orders":
                var newOrder = JsonConvert.DeserializeObject<Order>(postData);
                if (newOrder.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_order_pool().AddOrder(newOrder);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_order_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "clients":
                var newClient = JsonConvert.DeserializeObject<Client>(postData);
                if (newClient.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_client_pool().AddClient(newClient);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_client_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            case "shipments":
                var newShipment = JsonConvert.DeserializeObject<Shipment>(postData);
                if (newShipment.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_shipment_pool().AddShipment(newShipment);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID in Body already exists in data";
                    response.Close();
                    return;
                }
                DataProvider.fetch_shipment_pool().Save();
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Close();
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                break;
        }
    }

    private void HandlePutVersion1(string[] path, User user, HttpListenerRequest request, HttpListenerResponse response)
    {
        if (!AuthProvider.HasAccess(user, path[0], "put"))
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.Close();
            return;
        }

        int contentLength = (int)request.ContentLength64;
        byte[] buffer = new byte[contentLength];
        request.InputStream.Read(buffer, 0, contentLength);
        string putData = Encoding.UTF8.GetString(buffer);

        bool check;

        switch (path[0])
        {
            case "warehouses":
                int warehouseId = int.Parse(path[1]);
                var updatedWarehouse = JsonConvert.DeserializeObject<Warehouse>(putData);
                if (updatedWarehouse.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_warehouse_pool().UpdateWarehouse(warehouseId, updatedWarehouse);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_warehouse_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "locations":
                int locationId = int.Parse(path[1]);
                var updatedLocation = JsonConvert.DeserializeObject<Location>(putData);
                if (updatedLocation.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_location_pool().UpdateLocation(locationId, updatedLocation);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_location_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "transfers":
                if (path.Length == 2) // Update transfer
            {
                int transferId = int.Parse(path[1]);
                var updatedTransfer = JsonConvert.DeserializeObject<Transfer>(putData);
                if (updatedTransfer.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_transfer_pool().UpdateTransfer(transferId, updatedTransfer);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_transfer_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
            }
            else if (path.Length == 3 && path[2] == "commit") // Commit transfer
            {
                int transferId = int.Parse(path[1]);
                var transfer = DataProvider.fetch_transfer_pool().GetTransfer(transferId);
                if (transfer.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                // Update inventories based on transfer items
                foreach (var item in transfer.Items)
                {
                    var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(item.Item_Id);
                    foreach (var inventory in inventories)
                    {
                        if (transfer.Transfer_From is null) break;
                        if (inventory.Locations.Contains((int)transfer.Transfer_From))
                        {
                            inventory.Total_On_Hand -= item.Amount;
                        }
                        else if (inventory.Locations.Contains((int)transfer.Transfer_To))
                        {
                            inventory.Total_On_Hand += item.Amount;
                        }
                        DataProvider.fetch_inventory_pool().UpdateInventory(inventory.Id, inventory);
                    }
                }
                transfer.Transfer_Status = "Processed";
                check = DataProvider.fetch_transfer_pool().UpdateTransfer(transferId, transfer);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                // Notify processing completion
                var notificationSystem = new NotificationSystem();
                notificationSystem.Push($"Processed batch transfer with id: {transfer.Id}");
                DataProvider.fetch_transfer_pool().Save();
                DataProvider.fetch_inventory_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
            }
                break;

            case "items":
                string itemId = path[1];
                var updatedItem = JsonConvert.DeserializeObject<Item>(putData);
                if (updatedItem.Uid == null)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_item_pool().UpdateItem(itemId, updatedItem);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_item_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "item_lines":
                int itemLineId = int.Parse(path[1]);
                var updatedItemLine = JsonConvert.DeserializeObject<ItemLine>(putData);
                if (updatedItemLine.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_itemline_pool().UpdateItemline(itemLineId, updatedItemLine);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_itemline_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "item_types":
                int itemTypeId = int.Parse(path[1]);
                var updatedItemType = JsonConvert.DeserializeObject<ItemType>(putData);
                if (updatedItemType.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_itemtype_pool().UpdateItemtype(itemTypeId, updatedItemType);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_itemtype_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "item_groups":
                int itemGroupId = int.Parse(path[1]);
                var updatedItemGroup = JsonConvert.DeserializeObject<ItemGroup>(putData);
                if (updatedItemGroup.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_itemgroup_pool().UpdateItemGroup(itemGroupId, updatedItemGroup);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_itemgroup_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "inventories":
                int inventoryId = int.Parse(path[1]);
                var updatedInventory = JsonConvert.DeserializeObject<Inventory>(putData);
                if (updatedInventory.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_inventory_pool().UpdateInventory(inventoryId, updatedInventory);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_inventory_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "suppliers":
                int supplierId = int.Parse(path[1]);
                var updatedSupplier = JsonConvert.DeserializeObject<Supplier>(putData);
                if (updatedSupplier.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_supplier_pool().UpdateSupplier(supplierId, updatedSupplier);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_supplier_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "orders":
                if (path.Length == 2) // Update order
                {
                    int orderId = int.Parse(path[1]);
                    var updatedOrder = JsonConvert.DeserializeObject<Order>(putData);
                    if (updatedOrder.Id == -10)
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusDescription = "ID not given in body";
                        response.Close();
                        return;
                    }
                    check = DataProvider.fetch_order_pool().UpdateOrder(orderId, updatedOrder);
                    if (!check)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                        response.Close();
                        return;
                    }
                    DataProvider.fetch_order_pool().Save();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                else if (path.Length == 3 && path[2] == "items") // Update items in order
                {
                    int orderId = int.Parse(path[1]);
                    var updatedItems = JsonConvert.DeserializeObject<List<Item>>(putData);
                    if (updatedItems.Any(x => x.Uid == null))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusDescription = "ID not given (everywhere) in body";
                        response.Close();
                        return;
                    }
                    DataProvider.fetch_order_pool().UpdateItemsInOrder(orderId, updatedItems);
                    DataProvider.fetch_order_pool().Save();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
                break;

            case "clients":
                int clientId = int.Parse(path[1]);
                var updatedClient = JsonConvert.DeserializeObject<Client>(putData);
                if (updatedClient.Id == -10)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "ID not given in body";
                    response.Close();
                    return;
                }
                check = DataProvider.fetch_client_pool().UpdateClient(clientId, updatedClient);
                if (!check)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                    response.Close();
                    return;
                }
                DataProvider.fetch_client_pool().Save();
                response.StatusCode = (int)HttpStatusCode.OK;
                break;

            case "shipments":
                if (path.Length == 2) // Update shipment
                {
                    int shipmentId = int.Parse(path[1]);
                    var updatedShipment = JsonConvert.DeserializeObject<Shipment>(putData);
                    if (updatedShipment.Id == -10)
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusDescription = "ID not given in body";
                        response.Close();
                        return;
                    }
                    check = DataProvider.fetch_shipment_pool().UpdateShipment(shipmentId, updatedShipment);
                    if (!check)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                        response.Close();
                        return;
                    }
                    DataProvider.fetch_shipment_pool().Save();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                else if (path.Length == 3 && path[2] == "orders") // Update orders in shipment
                {
                    int shipmentId = int.Parse(path[1]);
                    var updatedOrders = JsonConvert.DeserializeObject<List<Order>>(putData);
                    if (updatedOrders.Any(x => x.Id == -10))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusDescription = "ID not given (everywhere) in body";
                        response.Close();
                        return;
                    }
                    if (DataProvider.fetch_shipment_pool().GetShipment(shipmentId) == null)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "No data for given ID";
                        response.Close();
                        return;
                    }
                    DataProvider.fetch_order_pool().UpdateOrdersInShipment(shipmentId, updatedOrders);
                    DataProvider.fetch_order_pool().Save();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                else if (path.Length == 3 && path[2] == "items") // Update items in shipment
                {
                    int shipmentId = int.Parse(path[1]);
                    var updatedItems = JsonConvert.DeserializeObject<List<Item>>(putData);
                    if (updatedItems.Any(x => x.Uid == null))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusDescription = "ID not given (everywhere) in body";
                        response.Close();
                        return;
                    }
                    if (DataProvider.fetch_shipment_pool().GetShipment(shipmentId) == null)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "No data for given ID";
                        response.Close();
                        return;
                    }
                    DataProvider.fetch_shipment_pool().UpdateItemsInShipment(shipmentId, updatedItems);
                    DataProvider.fetch_shipment_pool().Save();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                else if (path.Length == 3 && path[2] == "commit") // Commit shipment
                {
                    int shipmentId = int.Parse(path[1]);
                    var shipment = DataProvider.fetch_shipment_pool().GetShipment(shipmentId);
                    if (shipment == null)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "No data found with given ID";
                        response.Close();
                        return;
                    }
                    // Update inventories based on shipment items
                    foreach (var item in shipment.Items)
                    {
                        var inventories = DataProvider.fetch_inventory_pool().GetInventoriesForItem(item.Item_Id);
                        foreach (var inventory in inventories)
                        {
                            if (inventory.Locations.Contains(shipment.Source_Id))
                            {
                                inventory.Total_On_Hand -= item.Amount;
                            }
                        }
                    }
                    shipment.Shipment_Status = "Shipped";
                    check = DataProvider.fetch_shipment_pool().UpdateShipment(shipmentId, shipment);
                    if (!check)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusDescription = "ID not found or ID in Body and in Route are not matching";
                        response.Close();
                        return;
                    }
                    var notificationSystem = new NotificationSystem();
                    notificationSystem.Push($"Shipment with id: {shipment.Id} has been processed.");
                    DataProvider.fetch_shipment_pool().Save();
                    DataProvider.fetch_inventory_pool().Save();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }

        response.Close();
    }


    private void HandleDeleteVersion1(string[] path, User user, HttpListenerResponse response)
    {
        if (!AuthProvider.HasAccess(user, path[0], "delete"))
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.Close();
            return;
        }

        bool check;

        switch (path[0])
        {
            case "warehouses":
                int warehouseId = int.Parse(path[1]);
                check = DataProvider.fetch_warehouse_pool().RemoveWarehouse(warehouseId);
                if (check) DataProvider.fetch_warehouse_pool().Save();
                break;
            case "locations":
                int locationId = int.Parse(path[1]);
                check = DataProvider.fetch_location_pool().RemoveLocation(locationId);
                if (check) DataProvider.fetch_location_pool().Save();
                break;
            case "transfers":
                int transferId = int.Parse(path[1]);
                check = DataProvider.fetch_transfer_pool().RemoveTransfer(transferId);
                if (check) DataProvider.fetch_transfer_pool().Save();
                break;
            case "items":
                string itemId = path[1];
                check = DataProvider.fetch_item_pool().RemoveItem(itemId);
                if (check) DataProvider.fetch_item_pool().Save();
                break;
            case "item_lines":
                int itemLineId = int.Parse(path[1]);
                check = DataProvider.fetch_itemline_pool().RemoveItemline(itemLineId);
                if (check) DataProvider.fetch_itemline_pool().Save();
                break;
            case "item_groups":
                int itemGroupId = int.Parse(path[1]);
                check = DataProvider.fetch_itemgroup_pool().RemoveItemGroup(itemGroupId);
                if (check) DataProvider.fetch_itemgroup_pool().Save();
                break;
            case "item_types":
                int itemTypeId = int.Parse(path[1]);
                check = DataProvider.fetch_itemtype_pool().RemoveItemtype(itemTypeId);
                if (check) DataProvider.fetch_itemtype_pool().Save();
                break;
            case "inventories":
                int inventoryId = int.Parse(path[1]);
                check = DataProvider.fetch_inventory_pool().RemoveInventory(inventoryId);
                if (check) DataProvider.fetch_inventory_pool().Save();
                break;
            case "suppliers":
                int supplierId = int.Parse(path[1]);
                check = DataProvider.fetch_supplier_pool().RemoveSupplier(supplierId);
                if (check) DataProvider.fetch_supplier_pool().Save();
                break;
            case "orders":
                int orderId = int.Parse(path[1]);
                check = DataProvider.fetch_order_pool().RemoveOrder(orderId);
                if (check) DataProvider.fetch_order_pool().Save();
                break;
            case "clients":
                int clientId = int.Parse(path[1]);
                check = DataProvider.fetch_client_pool().RemoveClient(clientId);
                if (check) DataProvider.fetch_client_pool().Save();
                break;
            case "shipments":
                int shipmentId = int.Parse(path[1]);
                check = DataProvider.fetch_shipment_pool().RemoveShipment(shipmentId);
                if (check) DataProvider.fetch_shipment_pool().Save();
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
                return;
        }

        if (!check)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.StatusDescription = "ID not found or other data is dependent on this data";
            response.Close();
            return;
        }

        response.StatusCode = (int)HttpStatusCode.OK;
        response.Close();
    }


    private void SendResponse(HttpListenerResponse response, object data)
    {
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "application/json";
        using (var writer = new StreamWriter(response.OutputStream))
        {
            string jsonResponse = System.Text.Json.JsonSerializer.Serialize(data);
            writer.Write(jsonResponse);
        }
        response.Close();
    }
}
