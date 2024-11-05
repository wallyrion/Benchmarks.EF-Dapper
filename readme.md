
## Benchmark results
### Get customer by id. Include orders. Include orderItems

| Method          | Job      | Runtime  | UseSplitQuery | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------- |--------- |--------- |-------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
| GetCustomerById | .NET 8.0 | .NET 8.0 | False         | 1.688 ms | 0.0368 ms | 0.1038 ms |  1.00 |    0.09 |  53.82 KB |        1.00 |
| GetCustomerById | .NET 9.0 | .NET 9.0 | False         | 2.516 ms | 0.0474 ms | 0.1059 ms |  1.50 |    0.11 | 196.56 KB |        3.65 |
|                 |          |          |               |          |           |           |       |         |           |             |
| GetCustomerById | .NET 8.0 | .NET 8.0 | True          | 2.867 ms | 0.0598 ms | 0.1705 ms |  1.00 |    0.08 |  79.58 KB |        1.00 |
| GetCustomerById | .NET 9.0 | .NET 9.0 | True          | 3.114 ms | 0.0732 ms | 0.2136 ms |  1.09 |    0.10 | 161.05 KB |        2.02 |


### Get customer by id

| Method                | Job      | Runtime  | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------- |--------- |--------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| GetCustomerById       | .NET 8.0 | .NET 8.0 | 765.2 us | 14.68 us | 35.16 us |  1.00 |    0.06 |  13.09 KB |        1.00 |
| GetCustomerById       | .NET 9.0 | .NET 9.0 | 734.8 us | 14.44 us | 17.73 us |  0.96 |    0.05 |  14.59 KB |        1.12 |
|                       |          |          |          |          |          |       |         |           |             |
| GetCustomerByIdDapper | .NET 8.0 | .NET 8.0 | 620.8 us | 14.45 us | 41.68 us |  1.00 |    0.09 |   4.95 KB |        1.00 |
| GetCustomerByIdDapper | .NET 9.0 | .NET 9.0 | 592.3 us | 11.28 us | 18.22 us |  0.96 |    0.07 |   4.96 KB |        1.00 |

### Get all customers ids (there are 10000 in total)

| Method                | Job      | Runtime  | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|---------------------- |--------- |--------- |---------:|----------:|----------:|------:|--------:|-----------:|------------:|
| GetCustomersIds       | .NET 8.0 | .NET 8.0 | 2.853 ms | 0.0363 ms | 0.0340 ms |  1.00 |    0.02 | 2380.84 KB |        1.00 |
| GetCustomersIds       | .NET 9.0 | .NET 9.0 | 2.789 ms | 0.0530 ms | 0.0650 ms |  0.98 |    0.03 | 2381.85 KB |        1.00 |
|                       |          |          |          |           |           |       |         |            |             |
| GetCustomersIdsDapper | .NET 8.0 | .NET 8.0 | 2.565 ms | 0.0503 ms | 0.0471 ms |  1.00 |    0.03 |  981.21 KB |        1.00 |
| GetCustomersIdsDapper | .NET 9.0 | .NET 9.0 | 2.492 ms | 0.0367 ms | 0.0343 ms |  0.97 |    0.02 |  981.64 KB |        1.00 |

### Get customers count
| Method                  | Job      | Runtime  | Mean     | Error    | StdDev   | Allocated |
|------------------------ |--------- |--------- |---------:|---------:|---------:|----------:|
| GetCustomersCount       | .NET 8.0 | .NET 8.0 | 955.3 us | 11.84 us | 11.08 us |   4.97 KB |
| GetCustomersCountDapper | .NET 8.0 | .NET 8.0 | 924.6 us | 10.75 us | 10.06 us |   2.13 KB |
| GetCustomersCount       | .NET 9.0 | .NET 9.0 | 939.0 us |  6.70 us |  5.94 us |   5.56 KB |
| GetCustomersCountDapper | .NET 9.0 | .NET 9.0 | 919.2 us | 12.36 us | 11.56 us |   2.17 KB |




# Load Test Results Summary

Comparing the performance metrics for .NET 8 and .NET 9:

| Metric                      | .NET 8                                | .NET 9                                |
|----------------------------:|:-------------------------------------:|:-------------------------------------:|
| **Total Requests**          | 1,690,574                             | 1,287,101                             |
| **Requests per second**     | 1,741.02                              | 1,325.61                              |
| **Data Received**           | 13 GB                                 | 9.5 GB                                |
| **Data Sent**               | 220 MB                                | 164 MB                                |
| **HTTP Request Duration**   | avg=2.29s, p(90)=4.79s, p(95)=5.51s   | avg=3.32s, p(90)=6.73s, p(95)=8.06s   |
| **Request Blocking**        | avg=6.02µs, p(90)=5.01µs, p(95)=5.81µs| avg=6.65µs, p(90)=5.15µs, p(95)=6.02µs|
| **Request Receiving**       | avg=44.14µs, p(90)=61.38µs, p(95)=75.48µs| avg=42.62µs, p(90)=62.5µs, p(95)=76.01µs|
| **Request Sending**         | avg=14.77µs, p(90)=16.12µs, p(95)=32.88µs| avg=13.8µs, p(90)=15.95µs, p(95)=29.79µs|
| **Iteration Duration**      | avg=3.29s, p(90)=5.79s, p(95)=6.52s   | avg=4.32s, p(90)=7.73s, p(95)=9.06s   |
| **Virtual Users (VUs)**     | max=10,000                            | max=10,000                            |

To visualize these differences, here’s a comparative analysis:

![dotnet8_loadtests_result.png](dotnet8_loadtests_result.png)
![dotnet9_loadtests_result.png](dotnet9_loadtests_result.png)

This summary highlights key performance metrics, demonstrating how both .NET versions handle load testing. From the given data, we can see that .NET 8 had a higher throughput (Requests per second) and better average request duration compared to .NET 9.
