using AkademiaCsharp.Examples;
using AkademiaCsharp.Examples.Caveats;
using AkademiaCsharp.Examples.Hashing;
using AkademiaCsharp.Examples.Other;
using AkademiaCsharp.Examples.Sums;
using AkademiaCsharp.Models;
using AkademiaCsharp.Workers;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, __) => tokenSource.Cancel();

            const string hashAlgorithm = "SHA256";
            var syncHasher = new SyncHasher(hashAlgorithm);
            var apmHasher = new ApmHasher(hashAlgorithm);
            var asyncHasher = new AsyncHasher(hashAlgorithm);

            const int iterations = 100;

            const int streamLength = 1024 * 1024 * 10;
            const int streamSeed = 10924124;
            const int streamCount = 10;
            const int streamAsyncCount = 10;

            const int sumTaskCount = 100;
            const int sumOperationCount = 100000;

            var hashExamples = new Dictionary<string, IExample>
            {
                { "Sync hashing (1)",                                                       new Example1_SyncHashing(syncHasher, streamLength, streamSeed, 1) },
                { $"Sync hashing ({streamCount})",                                          new Example1_SyncHashing(syncHasher, streamLength, streamSeed, streamCount) },
                //{ $"Thread hashing - single stream for all threads ({streamCount})",        new Example2_SingleStreamThreadHashing(syncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Thread hashing - single stream per thread ({streamCount})",             new Example3_MultiStreamThreadHashing(syncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Lock-synchronized thread hashing ({streamCount})",                      new Example4_LockSynchronizedThreadHashing(syncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Monitor-synchronized thread hashing ({streamCount})",                   new Example5_MonitorSynchronizedThreadHashing(syncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Semaphore-synchronized thread hashing ({streamCount})",                 new Example6_SemaphoreSynchronizedThreadHashing(syncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"APM hashing hashing ({streamCount})",                                   new Example7_ApmHashing(apmHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Parallel Linq hashing ({streamCount})",                                 new Example8_ParallelLinqHashing(syncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Async hashing ({streamCount})",                                         new Example9_AsyncHashing(asyncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Parallel async hashing - stream for all tasks ({streamCount})",         new Example10_ParallelSingleStreamAsyncHashing(asyncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Parallel async hashing - stream per task ({streamCount})",              new Example11_ParallelMultiStreamAsyncHashing(asyncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Parallel asyncified APM hashing - stream per task ({streamCount})",     new Example12_AsyncifiedApmHashing(apmHasher, streamLength, streamSeed, streamAsyncCount) },
            };

            var sumExamples = new Dictionary<string, IExample>
            {
                { $"Thread sum ({sumTaskCount}",                                            new Example1_ThreadSum(sumTaskCount, sumOperationCount) },
                { $"Lock-synchronized thread sum ({sumTaskCount})",                         new Example2_LockSynchronizedThreadSum(sumTaskCount, sumOperationCount) },
                { $"Interlocked thread sum ({sumTaskCount})",                               new Example3_InterlockedThreadSum(sumTaskCount, sumOperationCount) },
                { $"Async sum ({sumTaskCount})",                                            new Example4_AsyncSum(sumTaskCount, sumOperationCount) },
                { $"Sync sum",                                                              new Example5_SyncSum(sumTaskCount * sumOperationCount) },
            };

            var caveatsExamples = new Dictionary<string, IExample>
            {
                //{ "Thread unhandled exception kills process",                               new Example1_ThreadUnhandledExceptionKillsProcess() },
                //{ "Using clause in async method",                                           new Example2_UsingClauseInAsyncMethods(asyncHasher, streamLength, streamSeed) },
                { "Thread ID changing in async methods",                                    new Example3_CurrentThreadInAsyncMethods(10, 5) },
            };

            var otherExamples = new Dictionary<string, IExample>
            {
                //{ "Async and Linq",                                                         new Example1_AsyncAndLinq() },
                //{ "IAsyncEnumerable",                                                       new Example2_AsyncEnumerable() },
                { "ConcurrentBag",                                                          new Example3_ConcurrentBag() }
            };

            //await RunExamples("hashing", hashExamples, iterations, tokenSource.Token);
            //await RunExamples("sum", sumExamples, iterations, tokenSource.Token);
            await RunExamples("caveats", caveatsExamples, 1, tokenSource.Token);
            await RunExamples("other", otherExamples, 1, tokenSource.Token);

            Console.ReadLine();
        }

        private static async Task RunExamples(string name, IDictionary<string, IExample> examples, int iterations, CancellationToken token)
        {
            var measurer = new ExampleTimeMeasurer();
            var results = new List<ExampleTimeResult>(examples.Count);
            foreach (var example in examples)
            {
                ExampleTimeResult result;
                Print(example.Key);
                try
                {
                    result = await measurer.Measure(example.Key, example.Value, iterations, token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine();
                    continue;
                }

                results.Add(result);
                Print(example.Key);
                Print(result);

                Console.WriteLine();
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            Console.WriteLine($"Mean results for {name}");
            var counter = 1;
            foreach (var result in results.OrderBy(r => !r.Succeeded).ThenBy(r => r.Mean))
            {
                Console.WriteLine($"{counter++}. {result.ExampleName} [{(result.Succeeded ? "ok" : "failed")}]: ~{result.Mean}ms");
            }
            Console.WriteLine();

            using (var fs = File.OpenWrite($"{name}_{DateTimeOffset.Now.ToString("yyyyMMddTHHmmss")}_.csv"))
            using (var sw = new StreamWriter(fs))
            using (var csvWriter = new CsvWriter(sw))
            {
                csvWriter.WriteHeader<ExampleTimeResult>();
                csvWriter.NextRecord();
                csvWriter.WriteRecords(results);
            }
        }

        private static void Print(string name)
        {
            Console.WriteLine("========================");
            Console.WriteLine(name);
            Console.WriteLine("========================");
        }
        private static void Print(ExampleTimeResult result)
        {
            Console.WriteLine($"Count: {result.Count}");
            Console.WriteLine("============");
            Console.WriteLine($"Maximum: {result.Maximum}ms");
            Console.WriteLine($"Minimum: {result.Minimum}ms");
            Console.WriteLine($"Mean: {result.Mean}ms");
            Console.WriteLine($"StdDev: {result.StandardDeviation}ms");
        }
    }
}
