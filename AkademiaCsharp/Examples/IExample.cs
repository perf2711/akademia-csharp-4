using AkademiaCsharp.Workers.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace AkademiaCsharp.Examples
{
    public interface IExample
    {
        Task<bool> InvokeAsync(ITimeMeasurer timeMeasurer, CancellationToken token);
    }
}
