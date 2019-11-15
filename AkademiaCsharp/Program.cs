using AkademiaCsharp.Examples;
using AkademiaCsharp.Examples.Caveats;
using AkademiaCsharp.Examples.Hashing;
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
                { $"Parallel async hashing - single stream for all tasks ({streamCount})",  new Example10_ParallelSingleStreamAsyncHashing(asyncHasher, streamLength, streamSeed, streamAsyncCount) },
                { $"Parallel async hashing - single stream per task ({streamCount})",       new Example11_ParallelMultiStreamAsyncHashing(asyncHasher, streamLength, streamSeed, streamAsyncCount) },
            };

            var sumExamples = new Dictionary<string, IExample>
            {
                { $"Async sum ({sumTaskCount})",                                            new Example1_AsyncSum(sumTaskCount, sumOperationCount) },
                { $"Lock-synchronized async sum ({sumTaskCount})",                          new Example2_LockSynchronizedAsyncSum(sumTaskCount, sumOperationCount) },
                { $"Interlocked async sum ({sumTaskCount})",                                new Example3_InterlockedAsyncSum(sumTaskCount, sumOperationCount) },
                { $"Sync sum",                                                              new Example4_SyncSum(sumTaskCount * sumOperationCount) },
            };

            var caveatsExamples = new Dictionary<string, IExample>
            {
                //{ "Thread unhandled exception kills process",                               new Example1_ThreadUnhandledExceptionKillsProcess() },
                { "Using clause in async method",                                           new Example2_UsingClauseInAsyncMethods(asyncHasher, streamLength, streamSeed) }
            };

            await RunExamples("hashing", hashExamples, tokenSource.Token);
            //await RunExamples("sum", sumExamples, tokenSource.Token);
            //await RunExamples("caveats", caveatsExamples, tokenSource.Token);

            Console.ReadLine();
        }

        private static async Task RunExamples(string name, IDictionary<string, IExample> examples, CancellationToken token)
        {
            var measurer = new ExampleTimeMeasurer();
            var results = new List<ExampleTimeResult>(examples.Count);
            foreach (var example in examples)
            {
                ExampleTimeResult result;
                Print(example.Key);
                try
                {
                    result = await measurer.Measure(example.Key, example.Value, 100, token);
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
