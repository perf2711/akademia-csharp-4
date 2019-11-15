using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples
{
    public class Example4_SyncSum : IExample
    {
        private readonly int _operationCount;

        public Example4_SyncSum(int operationCount)
        {
            _operationCount = operationCount;
        }

        public Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var result = 0;
            var expectedResult = _operationCount;

            timeMeasurer.Start();
            for (var i = 0; i < _operationCount; i++)
            {
                result += 1;
            }
            timeMeasurer.Stop();

            Console.WriteLine($"Expected: {expectedResult}");
            Console.WriteLine($"Actual: {result}");

            return Task.FromResult(result == expectedResult);
        }
    }
}
