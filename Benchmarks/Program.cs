// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Benchmarks;

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
BenchmarkRunner.Run<BenchmarkGetCustomerByidRawSql>();
