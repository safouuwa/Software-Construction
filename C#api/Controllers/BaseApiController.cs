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

    protected IActionResult CheckAuthorization(string apiKey, string resource, string operation)
    {
        var user = AuthProvider.GetUser(apiKey);
        if (user == null)
            return Unauthorized($"ApiKey: {apiKey} is not valid as it is not linked to an existing user");

        if (!AuthProvider.HasAccess(user, resource, operation))
            return Unauthorized($"{user.App} cannot access this functionality");

        return null;
    }
}