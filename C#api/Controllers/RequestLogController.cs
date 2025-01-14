using Microsoft.AspNetCore.Mvc;
using Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        System.IO.File.WriteAllText("C#api/RequestLogs/RequestLogs.log", string.Empty);
        return Ok("Logfile successfully refreshed!");
    }

    [HttpGet("filter")]
    public IActionResult FilterRequests(
        [FromQuery] string model = null,
        [FromQuery] string method = null,
        [FromQuery] string date = null)
    {
        var auth = CheckAdmin(Request.Headers["API_KEY"]);
        if (auth != null) return auth;

        var logDirectory = "C#api/RequestLogs";
        var logFilePath = Path.Combine(logDirectory, "RequestLogs.log");

        if (!System.IO.File.Exists(logFilePath))
        {
            return NoContent();
        }

        var logLines = System.IO.File.ReadAllLines(logFilePath);

        var filteredLogs = new List<string>();

        foreach (var line in logLines)
        {
            var parts = line.Split('|');

            var logModel = parts[1].Split('/')[3].Trim();
            var logMethod = parts[2].Trim();
            var logDate = parts[0].Split(' ')[0].Trim();

            if (!string.IsNullOrEmpty(model) && !logModel.Equals(model, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!string.IsNullOrEmpty(method) && !logMethod.Equals(method, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!string.IsNullOrEmpty(date) && !logDate.Equals(date, StringComparison.OrdinalIgnoreCase))
                continue;
            
            filteredLogs.Add(line);
        }

        if (filteredLogs == null || !filteredLogs.Any())
        {
            return NoContent();
        }

        return Ok(filteredLogs);
    }
}