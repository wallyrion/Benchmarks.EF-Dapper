// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Benchmarks;

/*var s = new BenchmarkService();
s.Setup();
s.IterationSetup();
s.GetCustomerById();
var r = await s.GetCustomerById();*/

/*
BenchmarkRunner.Run<BenchmarkGetCustomerById>();
BenchmarkRunner.Run<BenchmarkGetCustomersAllIds>();
BenchmarkRunner.Run<BenchmarkGetCustomersCount>();
*/


BenchmarkRunner.Run<BenchmarkService>();
