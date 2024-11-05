using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80, baseline: true)]
[MemoryDiagnoser(false)]
public class BenchmarkGetCustomerById : BaseBenchmark
{
    List<Guid> customerIds = new List<Guid>();
    private Guid _currentCustomerId { get; set; }
    private static readonly Random _random = new Random(200);
    
    public override async Task Setup()
    {
        await base.Setup();
        var customersRepository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");
        customerIds = (await customersRepository.GetAllCustomerIdsAsync()).ToList();
    }
    

    [IterationSetup]
    public void IterationSetup()
    {
        if (customerIds.Count <= 0)
        {
            return;
        }
        
        var randomIndex = _random.Next(0, customerIds.Count - 1);
        _currentCustomerId = customerIds[randomIndex];
    }
    
    [Benchmark]
    public async Task<CustomerDto?> GetCustomerById()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");

        return await repository.GetCustomerByIdAsync(_currentCustomerId);
    }
    
   
    [Benchmark]
    public async Task<CustomerDto?> GetCustomerByIdDapper()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("Dapper");

        return await repository.GetCustomerByIdAsync(_currentCustomerId);
    }
}