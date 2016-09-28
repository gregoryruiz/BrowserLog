using System;
using System.Threading;

namespace BrowserLog.TinyServer
{
    public interface IEventChannel : IDisposable
    {
        void Send(ServerSentEvent message, CancellationToken token);
    }
}
