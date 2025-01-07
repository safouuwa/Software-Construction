using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;  // Disable camelCase
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Register your services
builder.Services.AddSingleton<AuthProvider>();
builder.Services.AddSingleton<DataProvider>();
builder.Services.AddSingleton<NotificationSystem>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Initialize your providers
AuthProvider.Init();
DataProvider.Init();
NotificationSystem notisys = new NotificationSystem();
NotificationSystem.Start();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://localhost:3000/");

app.Use(async (context, next) =>
{
    await next.Invoke();
    var path = context.Request.Path.Value;
    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string logFilePath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "C#api", "RequestLogs", "RequestLogs.txt"));    
    var pathParts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

    if (pathParts.Length > 2 && new[] { "transfers", "shipments", "orders", "items" }.Contains(pathParts[2]))
    {
        var user = AuthProvider.GetUser(context.Request.Headers["API_KEY"]);
        if (user == null) return;
        await File.AppendAllTextAsync(logFilePath, $"{context.Request.Method} {context.Request.Path} made by {user.App}; Model: {pathParts[2]}; StatusCode: {context.Response.StatusCode}; Date and Time: {DateTime.UtcNow.AddHours(1).ToString("dd-MM-yyyy HH:mm:ss")}\n");
    }
});

notisys.Push("Serving on PORT 3000");
app.Run();