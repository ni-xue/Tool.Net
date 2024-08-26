using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Tool.Sockets.NetFrame;

namespace Tool.Sockets.Kernels
{
    internal readonly struct PoolData
    {
        public PoolData(in UserKey key, Socket socket, IDataPacket packet)
        {
            Key = key;
            Client = socket;
            Packet = packet;
        }

        public readonly UserKey Key;

        public readonly Socket Client;

        public readonly IDataPacket Packet;

        public async ValueTask<IDataPacket> RequestAsync() 
        {
            IDataPacket dataPacket;
            if (DataNet.DicDataTcps.TryGetValue(Packet.ActionKey, out DataNet dataTcp))
            {
                DataBase handler = dataTcp.NewClass.Invoke();
                using (handler)
                {
                    dataPacket = await handler.RequestAsync(Packet, Key, dataTcp);
                }
            }
            else
            {
                dataPacket = Packet.CopyTo(false, false);
                dataPacket.ResetValue(false, !Packet.IsServer);
                dataPacket.SetErr("接口不存在");
            }
            return dataPacket;
        }

        //public async Task OnPool(Func<DataPacket, Socket, Task> func)
        //{
        //    var dataPacket = await RequestAsync();
        //    await func(dataPacket, Client);
        //}

        //public async Task OnPool(Func<DataPacket, Task> func)
        //{
        //    var dataPacket = await RequestAsync();
        //    await func(dataPacket);
        //}

        public void Dispose() 
        {
            Packet.Dispose();
        }
    }
}
