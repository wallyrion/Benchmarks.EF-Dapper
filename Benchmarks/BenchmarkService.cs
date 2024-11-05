﻿using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80, baseline: true)]
[MemoryDiagnoser(false)]
public class BenchmarkService
{
    private IServiceProvider _serviceProvider = null!;
    private List<Guid> _customerIds = [];
    private Guid CurrentCustomerId { get; set; }
    
    [Params(true, false)]
    public bool UseSplitQuery { get; set; } = false;  
    private static readonly Random Random = new(200);


    [GlobalSetup]
    public async Task Setup()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        var customersRepository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");
        _customerIds = (await customersRepository.GetAllCustomerIdsAsync()).ToList();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        if (_customerIds.Count <= 0)
        {
            return;
        }
        
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        configuration["UseSplitQuery"] = UseSplitQuery.ToString();;
        
        var randomIndex = Random.Next(0, _customerIds.Count - 1);
        CurrentCustomerId = _customerIds[randomIndex];
    }

    [Benchmark]
    public async Task<CustomerDto?> GetCustomerById()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");

        return await repository.GetCustomerWithOrdersAsync(CurrentCustomerId);
    }

   
    /*[Benchmark]
    public async Task<CustomerDto?> GetCustomerByIdDapper()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("Dapper");

        return await repository.GetCustomerWithOrdersAsync(_currentCustomerId);
    }*/

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