using BenchmarkDotNet.Attributes;
using EfVersusDapper;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

public enum AccountStatus
{
    Unknown = 0,
    Active = 1,
    Frozen = 2,
    Closed = 3,
    Pending = 4,
    Suspended = 5,
    Preparation = 6
}


[MemoryDiagnoser(false)]
public class BenchmarkStatuses
{
    private List<AccountStatus> statesActive;
    private HashSet<AccountStatus> statesActiveSet;

    [Params(1, 5, 10, 20, 100)]
    public int Count { get; set; }

    private bool changed;
    
    [GlobalSetup]
    public void Setup()
    {
        //var enumValues = Enum.GetValues<AccountStatus>();

        statesActive = Enumerable.Range(0, Count).Select(x => AccountStatus.Active).ToList();
    }
    
    [Benchmark]
    public bool StatusesOld()
    {
        if (statesActive.Contains(AccountStatus.Closed))
        {
            changed = true;
        }

        if (statesActive.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }
        
        if (!statesActive.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }
        
        if (!statesActive.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }


        return true;
    }
    
    [Benchmark]
    public bool StatusesRefactored()
    {
        statesActiveSet = statesActive.ToHashSet();
        
        if (statesActiveSet.Contains(AccountStatus.Closed))
        {
            changed = true;
        }

        if (statesActiveSet.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }
        
        if (!statesActiveSet.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }
        
        if (!statesActiveSet.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }
        
        if (!statesActiveSet.Contains(AccountStatus.Suspended))
        {
            changed = true;
        }
        return true;

    }
}