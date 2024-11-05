using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

public class BaseBenchmark
{
    protected IServiceProvider _serviceProvider = null!;
    
    [GlobalSetup]
    public virtual Task Setup()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        return Task.CompletedTask;
    }

    
    protected void ConfigureServices(IServiceCollection services)
    {
        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Add other services to DI container

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddKeyedScoped<ICustomerRepository, EfCustomerRepository>("EF");
        services.AddKeyedScoped<ICustomerRepository, DapperCustomerRepository>("Dapper");

        // Add more services as needed
    }
}