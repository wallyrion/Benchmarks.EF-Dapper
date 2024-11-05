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
public class BenchmarkGetCustomersAllIds : BaseBenchmark
{
    [Benchmark]
    public async Task<IReadOnlyList<Guid>> GetCustomersIds()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("EF");

        return await repository.GetAllCustomerIdsAsync();
    }
    
    [Benchmark]
    public async Task<IReadOnlyList<Guid>> GetCustomersIdsDapper()
    {
        var repository = _serviceProvider.GetRequiredKeyedService<ICustomerRepository>("Dapper");

        return await repository.GetAllCustomerIdsAsync();
    }
}