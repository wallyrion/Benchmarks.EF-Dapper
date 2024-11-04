using EfVersusDapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Add to Program.cs
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerRepository>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var useEf = configuration.GetValue<bool>("UseEf");

    if (useEf)
    {
        return new EfCustomerRepository(serviceProvider.GetRequiredService<ApplicationDbContext>());
    }

    return new DapperCustomerRepository(configuration);
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
