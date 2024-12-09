using Microsoft.AspNetCore.Mvc;
using Providers;
using System.IO;

[ApiController]
[Route("api/v1/[controller]")]
public class RequestLogController : BaseApiController
{
    protected readonly NotificationSystem _notificationSystem;

    public RequestLogController(
        NotificationSystem notificationSystem)
        : base(notificationSystem)
    {
    }

    protected IActionResult CheckAdmin(string apiKey)
    {
        var user = AuthProvider.GetUser(apiKey);
        if (user == null)
            return Unauthorized($"ApiKey: {apiKey} is not valid as it is not linked to an existing user");

        if (user.App != "Admin")
            return Unauthorized($"{user.App} cannot access this functionality");

        return null;
    }

    [HttpGet("refresh")]
    public IActionResult RefreshLogFile()
    {
        var auth = CheckAdmin(Request.Headers["API_KEY"]);
        if (auth != null) return auth;
        System.IO.File.WriteAllText("C#api/RequestLogs/RequestLogs.txt", string.Empty);
        return Ok("Logfile successfully refreshed!");
    }
}