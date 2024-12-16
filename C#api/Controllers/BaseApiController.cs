using Microsoft.AspNetCore.Mvc;
using Providers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly NotificationSystem _notificationSystem;

    public BaseApiController(
        NotificationSystem notificationSystem)
    {
        _notificationSystem = notificationSystem;
    }

    protected IActionResult? CheckAuthorization(string apiKey, string resource, string operation)
    {
        var user = AuthProvider.GetUser(apiKey);
        if (user == null)
            return Unauthorized($"ApiKey: {apiKey} is not valid as it is not linked to an existing user");

        if (!AuthProvider.HasAccess(user, resource, operation))
            return Unauthorized($"{user.App} cannot access this functionality");

        if (user.OwnWarehouses.Count == 0) return null;

        if (operation == "get")
        {
            bool check = resource switch
            {
                "warehouses" => user.EndpointAccess.Warehouses.OwnWarehouse,
                "locations" => user.EndpointAccess.Locations.OwnWarehouse,
                "transfers" => user.EndpointAccess.Transfers.OwnWarehouse,
                "items" => user.EndpointAccess.Items.OwnWarehouse,
                "item_lines" => user.EndpointAccess.ItemLines.OwnWarehouse,
                "item_groups" => user.EndpointAccess.ItemGroups.OwnWarehouse,
                "item_types" => user.EndpointAccess.ItemTypes.OwnWarehouse,
                "suppliers" => user.EndpointAccess.Suppliers.OwnWarehouse,
                "orders" => user.EndpointAccess.Orders.OwnWarehouse,
                "clients" => user.EndpointAccess.Clients.OwnWarehouse,
                "shipments" => user.EndpointAccess.Shipments.OwnWarehouse,
                "inventories" => user.EndpointAccess.Inventories.OwnWarehouse,
                _ => false
            };
            if (check) return Ok();
        }


        return null;
    }
}