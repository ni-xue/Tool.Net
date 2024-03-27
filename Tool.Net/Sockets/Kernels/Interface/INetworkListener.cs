using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    public interface INetworkListener : IDisposable
    {
        string Server { get; }

        bool IsClose { get; }

        bool IsThreadPool { get; }

        bool DisabledReceive { get; }

        void SetCompleted(Func<string, EnServer, DateTime, Task> Completed);

        void StartAsync(string ip, int port);

        void Close();
    }

    public interface INetworkListener<ISocket> : INetworkListener //ISocket -> Socket, WebSocket, Quic
    {
        IReadOnlyDictionary<string, ISocket> ListClient { get; }

        void SendAsync(ISocket client, params ArraySegment<byte>[] listData);

        void Send(ISocket client, params ArraySegment<byte>[] listData);

        void SetReceived(Func<ReceiveBytes<ISocket>, Task> Received);

        bool TrySocket(string key, out ISocket client);
    }
}
