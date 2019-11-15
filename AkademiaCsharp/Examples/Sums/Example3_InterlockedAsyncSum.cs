using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples
{
    public class Example3_InterlockedAsyncSum : IExample
    {
        private readonly int _taskCount;
        private readonly int _operationCount;

        public Example3_InterlockedAsyncSum(int taskCount, int operationCount)
        {
            _taskCount = taskCount;
            _operationCount = operationCount;
        }

        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var result = 0;
            var expectedResult = _taskCount * _operationCount;
            var tasks = new List<Task>(_taskCount);

            timeMeasurer.Start();
            for (var i = 0; i < _taskCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < _operationCount; j++)
                    {
                        Interlocked.Add(ref result, 1);
                        // Interlocked.Increment(ref result);
                    }
                }));
            }
            await Task.WhenAll(tasks);
            timeMeasurer.Stop();

            Console.WriteLine($"Expected: {expectedResult}");
            Console.WriteLine($"Actual: {result}");

            return result == expectedResult;
        }
    }
}
