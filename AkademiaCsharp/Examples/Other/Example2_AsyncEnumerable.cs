using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Other
{
    public class Example2_AsyncEnumerable : IExample
    {
        private async IAsyncEnumerable<int> LongRunningAddition(IEnumerable<int> numbers, int b)
        {
            foreach (var a in numbers)
            {
                await Task.Delay(b);
                yield return a + b;
            }
        }

        public async Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var numbers = Enumerable.Repeat(1, 100);

            timeMeasurer.Start();
            await foreach (var number in LongRunningAddition(numbers, 100).WithCancellation(token))
            {
                Console.WriteLine(number);
            }
            timeMeasurer.Stop();

            return true;
        }
    }
}
