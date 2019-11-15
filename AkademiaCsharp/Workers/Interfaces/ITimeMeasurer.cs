namespace AkademiaCsharp.Workers.Interfaces
{
    public interface ITimeMeasurer
    {
        void Start();
        void Stop();
        long ElapsedMilliseconds { get; }
    }
}
