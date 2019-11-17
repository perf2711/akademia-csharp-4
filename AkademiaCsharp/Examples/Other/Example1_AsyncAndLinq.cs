using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Other
{
    public class Example1_AsyncAndLinq : IExample
    {
        private async Task<int> LongRunningAddition(int a, int b)
        {
            await Task.Delay(b);
            return a + b;
        }

        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var numbers = Enumerable.Repeat(1, 100);

            timeMeasurer.Start();
            var results = await Task.WhenAll(numbers.Select(n => LongRunningAddition(n, 100)));
            timeMeasurer.Stop();

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            return results.All(r => r == 1 + 100);
        }
    }
}
