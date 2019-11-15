using System;
using System.Collections.Generic;
using System.Linq;

namespace AkademiaCsharp.Models
{
    public class ExampleTimeResult
    {
        public ExampleTimeResult(string name, IEnumerable<bool> results, IEnumerable<long> times)
        {
            ExampleName = name;
            Results = results;
            Times = times.ToArray();
            Maximum = times.Max();
            Minimum = times.Min();
            Mean = times.Average();
            StandardDeviation = Math.Sqrt(times.Sum(t => Math.Pow(t - Mean, 2)) / Times.Length);
        }

        public string ExampleName { get; }
        public IEnumerable<bool> Results { get; }
        public long[] Times { get; }
        public long Maximum { get; }
        public long Minimum { get; }
        public double Mean { get; }
        public double StandardDeviation { get; }
        public int Count => Times.Length;
        public bool Succeeded => Results.All(r => r);
    }
}
