using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[MemoryDiagnoser(false)]
public class BenchmarkService : BaseBenchmark
{
    private List<Guid> _customerIds = [];
    private Guid CurrentCustomerId { get; set; }
    
    [Params(true, false)]
    public bool UseSplitQuery { get; set; } = false;  
    private static readonly Random Random = new(200);

    public override async Task Setup()
    {
        await base.Setup();

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
}