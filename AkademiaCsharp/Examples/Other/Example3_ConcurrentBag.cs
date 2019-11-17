using AkademiaCsharp.Workers.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Other
{
    public class Example3_ConcurrentBag : IExample
    {
        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var concurrentBag = new ConcurrentBag<int>();
            var tasks = new List<Task>();

            timeMeasurer.Start();
            for (var i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Factory.StartNew(() => concurrentBag.Add(i)));
            }
            await Task.WhenAll(tasks);
            timeMeasurer.Stop();

            return concurrentBag.Count == 1000;
        }
    }
}
