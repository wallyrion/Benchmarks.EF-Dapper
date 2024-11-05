using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
[MemoryDiagnoser(false)]
public class BenchmarkGetCustomersCount
{
    private IServiceProvider _serviceProvider = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Benchmark]
    public async Task<int> GetCustomersCount()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");

        return await repository.GetCustomersCountAsync();
    }
    
    [Benchmark]
    public async Task<int> GetCustomersCountDapper()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("Dapper");

        return await repository.GetCustomersCountAsync();
    }

    private void ConfigureServices(IServiceCollection services)
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