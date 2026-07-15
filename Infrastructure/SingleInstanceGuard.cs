using System;
using System.Threading;

namespace AndonVoiceBroadcaster.Infrastructure
{
    public sealed class SingleInstanceGuard : IDisposable
    {
        private readonly Mutex _mutex;

        public SingleInstanceGuard(string name)
        {
            bool createdNew;
            _mutex = new Mutex(true, "Global\\" + name, out createdNew);
            IsFirstInstance = createdNew;
        }

        public bool IsFirstInstance { get; private set; }

        public void Dispose()
        {
            if (_mutex == null)
            {
                return;
            }

            if (IsFirstInstance)
            {
                _mutex.ReleaseMutex();
            }
            _mutex.Dispose();
        }
    }
}
