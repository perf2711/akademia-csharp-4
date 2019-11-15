using AkademiaCsharp.Workers.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples.Caveats
{
    public class Example1_ThreadUnhandledExceptionKillsProcess : IExample
    {
        public Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token)
        {
            var throwingThread = new Thread(() =>
            {
                throw new Exception();
            });

            try
            {
                Console.WriteLine("Starting thread.");
                throwingThread.Start();

                Thread.Sleep(5000);

                Console.WriteLine("Joining thread.");
                throwingThread.Join();
            }
            catch (Exception e)
            {
                Console.WriteLine("Catched exception!");
                Console.WriteLine(e.Message);
            }

            return Task.FromResult(true);
        }
    }
}
