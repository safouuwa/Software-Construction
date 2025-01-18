using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProvidersV2;
using FiltersV2;
using ProcessorsV2;
using Providers;
using Processors;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => options.Filters.Add<RequestLoggingFilter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;  // Disable camelCase
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // V1 definition
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CargoHub V1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "CargoHub V2", Version = "v2" });
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.RelativePath.Contains($"{docName}");
    });
    
});

// Register your services
builder.Services.AddSingleton<ProvidersV2.AuthProvider>();
builder.Services.AddSingleton<ProvidersV2.DataProvider>();
builder.Services.AddSingleton<ProcessorsV2.NotificationSystem>();
builder.Services.AddSingleton<Providers.AuthProvider>();
builder.Services.AddSingleton<Providers.DataProvider>();
builder.Services.AddSingleton<Processors.NotificationSystem>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CargoHub V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "CargoHub V2");
    });
}

// Initialize your providers
Providers.AuthProvider.Init();
Providers.DataProvider.Init();
ProvidersV2.AuthProvider.Init();
ProvidersV2.DataProvider.Init();
Processors.NotificationSystem notisys = new Processors.NotificationSystem();
Processors.NotificationSystem.Start();
ProcessorsV2.NotificationSystem notisys2 = new ProcessorsV2.NotificationSystem();
ProcessorsV2.NotificationSystem.Start();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://localhost:3000/");


notisys.Push("Serving on PORT 3000");
app.Run();