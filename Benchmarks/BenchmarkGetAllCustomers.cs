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
public class BenchmarkGetAllCustomers : BaseBenchmark
{
    [Benchmark]
    public async Task<List<CustomerDto>> GetALlCustomers()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");

        return (await repository.GetAllCustomersAsync()).ToList();
    }
    
    [Benchmark]
    public async Task<List<CustomerDto>> GetALlCustomersDapper()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("Dapper");

        return (await repository.GetAllCustomersAsync()).ToList();
    }
}