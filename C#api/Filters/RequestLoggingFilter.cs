using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using InterfacesV2;
using Microsoft.AspNetCore.Mvc.Controllers;
using AttributesV2;

namespace FiltersV2;
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
            var sourceId = context.HttpContext.Request.Headers["Source_ID"].ToString();
            if (sourceId == "")
            {
                sourceId = "no-source-id";
            }
            
            string dataBefore = "no-data";
            string dataAfter = "no-data";

            if (context.Controller is not ILoggableAction) return;
            if (context.Controller is ILoggableAction loggableAction)
            {
                dataBefore = JsonSerializer.Serialize(loggableAction._dataBefore);
                dataAfter = JsonSerializer.Serialize(loggableAction._dataAfter);
            }

            var logEntry = $"{DateTime.UtcNow.AddHours(1):dd-MM-yyyy HH:mm:ss} | {request} | {httpMethod} | {sourceId} | {dataBefore} | {dataAfter}";
            

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