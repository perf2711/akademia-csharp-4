using AkademiaCsharp.Workers.Interfaces;
using System.Diagnostics;

namespace AkademiaCsharp.Workers
{
    public class StopwatchTimeMeasurer : ITimeMeasurer
    {
        public Stopwatch Stopwatch { get; }

        public StopwatchTimeMeasurer()
        {
            Stopwatch = new Stopwatch();
        }

        public StopwatchTimeMeasurer(Stopwatch stopwatch)
        {
            Stopwatch = stopwatch;
        }

        public long ElapsedMilliseconds => Stopwatch.ElapsedMilliseconds;

        public void Start()
        {
            Stopwatch.Start();
        }

        public void Stop()
        {
            Stopwatch.Stop();
        }
    }
}
