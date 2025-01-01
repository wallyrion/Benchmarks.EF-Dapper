// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Benchmarks;

/*var b = new BenchmarkStatuses();
b.Count = 10;
b.Setup();
b.StatusesRefactored();*/

/*
var s = new BenchmarkService();
await s.Setup();
s.IterationSetup();
//s.GetCustomerById();
var r = await s.GetCustomerByIdRaw();
*/

/*
BenchmarkRunner.Run<BenchmarkGetCustomerById>();
BenchmarkRunner.Run<BenchmarkGetCustomersAllIds>();
BenchmarkRunner.Run<BenchmarkGetCustomersCount>();
*/

//BenchmarkRunner.Run<BenchmarkGetCustomerByidRawSql>();
//BenchmarkRunner.Run<BenchmarkGetCustomerByidRawSql>();
BenchmarkRunner.Run<BenchmarkStatuses>();
