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

// Initialize your providers
AuthProvider.Init();
DataProvider.Init();
// NotificationSystem.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://localhost:3000/");

app.Run();
Console.WriteLine("Hello");