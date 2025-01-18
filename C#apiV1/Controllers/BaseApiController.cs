using Microsoft.AspNetCore.Mvc;
using Providers;
namespace Processors;

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
            return Unauthorized();

        if (!AuthProvider.HasAccess(user, resource, operation))
            return Forbid();

        return null;
    }
}