using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Sums
{
    public class Example2_LockSynchronizedThreadSum : IExample
    {
        private readonly int _threadCount;
        private readonly int _operationCount;

        public Example2_LockSynchronizedThreadSum(int threadCount, int operationCount)
        {
            _threadCount = threadCount;
            _operationCount = operationCount;
        }

        public Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var result = 0;
            var expectedResult = _threadCount * _operationCount;
            var tasks = new List<Task>(_threadCount);

            var threads = new List<Thread>(_threadCount);
            for (var i = 0; i < _threadCount; i++)
            {
                var thread = new Thread(() =>
                {
                    for (var j = 0; j < _operationCount; j++)
                    {
                        lock (tasks)
                        {
                            result += 1;
                        }
                    }
                });

                threads.Add(thread);
            }

            timeMeasurer.Start();
            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            timeMeasurer.Stop();

            Console.WriteLine($"Expected: {expectedResult}");
            Console.WriteLine($"Actual: {result}");

            return Task.FromResult(result == expectedResult);
        }
    }
}
