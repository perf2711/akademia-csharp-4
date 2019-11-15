using AkademiaCsharp.Examples;
using AkademiaCsharp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Workers
{
    public class ExampleTimeMeasurer
    {
        public async Task<ExampleTimeResult> Measure(string name, IExample example, int count, CancellationToken token)
        {
            var times = new List<long>();
            var results = new List<bool>();

            var measurer = new StopwatchTimeMeasurer();
            for (var i = 0; i < count; i++)
            {
                Console.WriteLine($"[{i + 1}/{count}] Running example...");
                measurer.Stopwatch.Reset();
                results.Add(await example.InvokeAsync(measurer, token));
                times.Add(measurer.ElapsedMilliseconds);
            }

            return new ExampleTimeResult(name, results, times);
        }
    }
}
