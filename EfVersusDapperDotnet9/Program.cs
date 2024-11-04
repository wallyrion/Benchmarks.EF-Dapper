using EfVersusDapper;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();

        metrics.AddMeter("System.Runtime");
        // Metrics provides by ASP.NET Core in .NET 8
        metrics.AddMeter("Microsoft.AspNetCore.Hosting");
        metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

        metrics.AddPrometheusExporter();
        metrics.AddOtlpExporter();
    });

// Add to Program.cs
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddKeyedScoped<ICustomerRepository, EfCustomerRepository>("EF");
builder.Services.AddKeyedScoped<ICustomerRepository, DapperCustomerRepository>("Dapper");

builder.Services.AddScoped<ICustomerRepository>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var useEf = configuration.GetValue<bool>("UseEf");

    return serviceProvider.GetRequiredKeyedService<ICustomerRepository>(useEf ? "EF" : "Dapper");
});
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await DbInitializer.Initialize(scope.ServiceProvider, app.Logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");

        throw;
    }
}

app.MapPrometheusScrapingEndpoint();
app.UseSwagger();
app.UseSwaggerUI();
app.MapHealthChecks("/healthz");

app.MapGet("/customers/{id:guid}", async (ICustomerRepository repository, Guid id) =>
{
    var customer = await repository.GetCustomerWithOrdersAsync(id);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
});

app.MapGet("/customers/all", async (ICustomerRepository repository) => 
    await repository.GetAllCustomerIdsAsync());

app.MapGet("/customers", async (ICustomerRepository repository) =>
{
    var customers = await repository.GetAllCustomersAsync();
    return Results.Ok(customers);
});

app.MapPost("/customers", async (ICustomerRepository repository, CustomerDto customerDto) =>
{
    await repository.AddCustomerAsync(customerDto);

    return Results.Created($"/customers/{customerDto.Id}", customerDto);
});
app.Run();
