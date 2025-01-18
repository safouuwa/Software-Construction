using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProvidersV2;
using FiltersV2;
using ProcessorsV2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => options.Filters.Add<RequestLoggingFilter>())
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


notisys.Push("Serving on PORT 3000");
app.Run();