using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Interfaces;
using Microsoft.AspNetCore.Mvc.Controllers;
using Attributes;

namespace Filters;
public class RequestLoggingFilter : IActionFilter
{
    private readonly IWebHostEnvironment _env;


    public RequestLoggingFilter(IWebHostEnvironment env) => _env = env;

    public void OnActionExecuting(ActionExecutingContext context)
    {}

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var logRequestAttribute = (LogRequestAttribute)Attribute.GetCustomAttribute(
            ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo,
            typeof(LogRequestAttribute));

            if (logRequestAttribute == null)
            {
                return;
            }
            var httpMethod = context.HttpContext.Request.Method;
            var request = context.HttpContext.Request.Path;
            var sourceId = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            
            string dataBefore = "no-data";
            string dataAfter = "no-data";

            if (context.Controller is not ILoggableAction) return;
            if (context.Controller is ILoggableAction loggableAction)
            {
                dataBefore = JsonSerializer.Serialize(loggableAction.GetDataBefore());
                dataAfter = JsonSerializer.Serialize(loggableAction.GetDataAfter());
            }

            var logEntry = $"{DateTime.UtcNow:dd-MM-yyyy HH:mm:ss} | {request} | {httpMethod} | {sourceId} | {dataBefore} | {dataAfter}";
            

            WriteToLogFile(logEntry);
    }

    private void WriteToLogFile(string logEntry)
    {
        var logDirectory = Path.Combine(_env.ContentRootPath, "C#api/RequestLogs");
        var logPath = Path.Combine(logDirectory, "RequestLogs.log");

        if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            File.AppendAllText(logPath, logEntry + Environment.NewLine);
    }
}