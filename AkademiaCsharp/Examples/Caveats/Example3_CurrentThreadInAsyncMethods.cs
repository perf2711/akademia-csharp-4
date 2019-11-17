using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Caveats
{
    public class Example3_CurrentThreadInAsyncMethods : IExample
    {
        private readonly int _taskCount;
        private readonly int _waitCounts;

        public Example3_CurrentThreadInAsyncMethods(int taskCount, int waitCounts)
        {
            _taskCount = taskCount;
            _waitCounts = waitCounts;
        }

        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var results = await Task.WhenAll(Enumerable.Range(0, _taskCount).Select(n => Test(n, _waitCounts)));

            foreach (var result in results)
            {
                Console.WriteLine($"{result.id}: {string.Join(", ", result.threadIds)}");
                Console.WriteLine();
            }

            return true;
        }

        private async Task<(int id, int[] threadIds)> Test(int taskId, int waits)
        {
            var threadIds = new int[waits];
            for (var i=0; i<waits; i++)
            {
                threadIds[i] = Thread.CurrentThread.ManagedThreadId;
                await Task.Delay(1);
            }

            return (taskId, threadIds);
        }
    }
}
