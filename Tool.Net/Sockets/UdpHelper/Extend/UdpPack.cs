using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.UdpHelper.Extend
{
    internal class UdpPack : IUdpCore
    {
        /// <summary>
        /// UDP 文报传输最大缓冲区
        /// </summary>
        public const ushort MaxBuffer = ushort.MaxValue - 20 - 8 - 12;

        private bool _dispose;
        private bool _isClose;

        private readonly bool isserver;
        private readonly bool isp2p;
        private readonly INetworkCore networkCore;
        private readonly Action<UserKey, byte> complete;
        private readonly ActionBlock<BytesCore> block;
        private readonly UdpStateObject udpState;

        internal UdpPack(INetworkCore networkCore, UdpEndPoint endPoint, Socket socket, int dataLength, bool onlyData, bool isserver, bool isp2p, Action<UserKey, byte> complete, ReceiveEvent<IUdpCore> received)
        {
            if (endPoint is null) throw new ArgumentException("endPoint 对象是空的！", nameof(endPoint));
            if (socket is null) throw new ArgumentException("socket 对象是空的！", nameof(socket));
            EndPoint = endPoint;
            Socket = socket;
            this.isserver = isserver;
            this.isp2p = isp2p;
            this.networkCore = networkCore ?? throw new ArgumentNullException(nameof(complete));
            this.complete = complete ?? throw new ArgumentNullException(nameof(complete));

            udpState = new UdpStateObject(this, dataLength, onlyData, received);
            block = new(ReceiveBlockAsync);
        }

        public UdpEndPoint EndPoint { get; }

        public Socket Socket { get; }

        public int DataLength => udpState.DataLength;

        public bool OnlyData => udpState.OnlyData;

        UdpStateObject IUdpCore.UdpState => udpState;

        public async Task CloseAsync()
        {
            if (!_isClose)
            {
                _isClose = true;
                if (networkCore is INetworkConnect network)
                {
                    network.Close();
                }
                else if (networkCore is UdpServerAsync serverAsync)
                {
                    await serverAsync.ClientCloes(new KeyValuePair<UserKey, IUdpCore>(EndPoint.Ipv4, this));
                }
            }
        }

        public void Dispose()
        {
            if (!_dispose)
            {
                _dispose = true;
                block.Complete();//标记任务已结束了
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        Memory<byte> IUdpCore.GetSendMemory(in SendBytes<IUdpCore> sendBytes, ref bool ispart, ref int i)
        {
            if (sendBytes.OnlyData != OnlyData)
            {
                throw new ArgumentException("与当前套接字（OnlyData）协议不一致！", nameof(sendBytes));
            }
            return sendBytes.GetMemory(0);
        }

        bool IUdpCore.IsOnLine(int receiveTimeout) => !_isClose && udpState.IsOnLine(receiveTimeout);

        Task IUdpCore.ReceiveAsync(Memory<byte> memory) //原模式完全还原原始效果，优化了多个连接者缓冲区增大
        {
            if (isserver || isp2p && udpState.IsKeepAlive(in memory))
            {
                complete(udpState.IpPort, 0);
            }
            else
            {
                BytesCore owner = new(memory.Length);
                owner.SetMemory(in memory);
                if (!block.Post(owner))
                {
                    throw new Exception("缓冲池已关闭！");
                }
            }
            return Task.CompletedTask;
        }

        async Task IUdpCore.SendAsync(Memory<byte> memory)
        {
            if (!UdpStateObject.IsConnected(Socket)) throw new Exception(isserver ? "服务端已关闭！" : "与服务端的连接已中断！");
#if NET5_0
            await Socket.SendToAsync(memory.AsArraySegment(), SocketFlags.None, EndPoint);
#else
            await Socket.SendToAsync(memory, SocketFlags.None, EndPoint);
#endif
            udpState.UpDateSignal();
        }

        //int a = 0;
        private async Task ReceiveBlockAsync(BytesCore owner)
        {
            //await Console.Out.WriteLineAsync($"计数：{++a}, {string.Join(',', owner.Array)}");
            await Task.Delay(networkCore.Millisecond);
            complete(udpState.IpPort, 1);
            await udpState.OnReceive(networkCore.IsThreadPool, owner);
        }
    }
}
