using System.Collections.Concurrent;
using System.Net.Sockets;
using System;
using Microsoft.AspNetCore.DataProtection;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    public interface INetworkConnect<ISocket> : IDisposable
    {
        string Server { get; }

        string LocalPoint { get; }

        bool IsClose { get; }

        bool Connected { get; }

        bool DisabledReceive { get; }

        void SetCompleted(Func<string, EnClient, DateTime, Task> Completed);

        void SetReceived(Func<ReceiveBytes<ISocket>, Task> Received);

        void ConnectAsync(string ip, int port);

        Task<bool> Reconnection();

        void SendAsync(params ArraySegment<byte>[] listData);

        void Send(params ArraySegment<byte>[] listData);

        void Close();
    }
}
