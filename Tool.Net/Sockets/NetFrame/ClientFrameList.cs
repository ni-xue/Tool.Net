using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 用于连接多服务器，分发消息的客户端帮助类，可以保证线程安全，均衡分发数据包。
    /// </summary>
    public class ClientFrameList: IDisposable
    {
        /// <summary>
        /// 绑定多服务器队列统一消息
        /// </summary>
        public event Func<string, EnClient, DateTime, Task> Completed;

        /// <summary>
        /// 原子锁
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// lock 安全锁
        /// </summary>
        private int LockCount;

        /// <summary>
        /// 当前分发消息的服务器服务器队列
        /// </summary>
        private List<ClientFrame> ClientFrames { get; set; }

        /// <summary>
        /// 当前拥有的队列数
        /// </summary>
        public int ClientCount => ClientFrames.Count;

        /// <summary>
        /// 返回加入的ClientFrame对象
        /// </summary>
        /// <param name="i">下标</param>
        /// <returns>位于指定索引处的元素。</returns>
        public ClientFrame this[int i]
        {
            get { return ClientFrames[i]; }
        }

        /// <summary>
        /// 初始化一次性加入队列服务器
        /// </summary>
        /// <param name="clientFrames">队列服务器</param>
        public ClientFrameList(IList<ClientFrame> clientFrames) : this(clientFrames.ToArray())
        {
        }

        /// <summary>
        /// 初始化一次性加入队列服务器
        /// </summary>
        /// <param name="clientFrames">队列服务器</param>
        public ClientFrameList(IEnumerable<ClientFrame> clientFrames) : this(clientFrames.ToArray())
        {
        }

        /// <summary>
        /// 初始化一次性加入队列服务器
        /// </summary>
        /// <param name="clientFrames">队列服务器</param>
        public ClientFrameList(params ClientFrame[] clientFrames) : this()
        {
            if (clientFrames == null)
            {
                throw new Exception("服务器队列不能为空");
            }
            else if (clientFrames.Length < 2)
            {
                throw new Exception("服务器队列必须大于2个以上");
            }
            ClientFrames = new List<ClientFrame>(clientFrames);
            ClientFrames.ForEach(x => { x.SetCompleted(SetCompleted); });
        }

        /// <summary>
        /// 初始化 可为空
        /// </summary>
        /// <param name="capacity">默认大小</param>
        public ClientFrameList(int capacity) : this()
        {
            ClientFrames = new List<ClientFrame>(capacity);
            ClientFrames.ForEach(x => { x.SetCompleted(SetCompleted); });
        }

        private ClientFrameList() 
        {
            LockCount = -1;
        }

        /// <summary>
        /// 主动添加客户端服务
        /// </summary>
        /// <param name="clientFrame">客户端</param>
        public void AddClientFrame(ClientFrame clientFrame) 
        {
            clientFrame.SetCompleted(SetCompleted);
            ClientFrames.Add(clientFrame);
        }

        private Task SetCompleted(string arg1, EnClient arg2, DateTime arg3)
        {
            Completed?.Invoke(arg1, arg2, arg3).Wait();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 同步发送消息（多服务器协调发送）
        /// </summary>
        /// <param name="api">接口调用信息</param>
        /// <param name="i">返回成功发送包的下标</param>
        /// <returns>返回数据包</returns>
        public NetResponse Send(ApiPacket api, out int i)
        {
            i = Interlocked.Increment(ref LockCount);
            if (ClientCount - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientCount);
            }
            if (i >= ClientCount)
            {
                i = 0;
            }
            return ClientFrames[i].Send(api);
        }

        /// <summary>
        /// 异步发送消息（多服务器协调发送）
        /// </summary>
        /// <param name="api">接口调用信息</param>
        /// <returns>返回数据包，以及下标</returns>
        public async Task<(NetResponse, int i)> SendAsync(ApiPacket api)
        {
            int i = Interlocked.Increment(ref LockCount);
            if (ClientCount - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientCount);
            }
            if (i >= ClientCount)
            {
                i = 0;
            }
            return (await ClientFrames[i].SendAsync(api), i);
        }

        /// <summary>
        /// 同步发送消息（多服务器协调发送+转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        /// <param name="i">返回成功发送包的下标</param>
        /// <returns>返回数据包</returns>
        public NetResponse SendIpIdea(string IpPort, ApiPacket api, out int i)
        {
            i = Interlocked.Increment(ref LockCount);
            if (ClientCount - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientCount);
            }
            if (i >= ClientCount)
            {
                i = 0;
            }
            return ClientFrames[i].SendIpIdea(IpPort, api);
        }

        /// <summary>
        /// 异步发送消息（多服务器协调发送+转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        /// <returns>返回数据包，以及下标</returns>
        public async Task<(NetResponse, int i)> SendIpIdeaAsync(string IpPort, ApiPacket api)
        {
            int i = Interlocked.Increment(ref LockCount);
            if (ClientCount - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientCount);
            }
            if (i >= ClientCount)
            {
                i = 0;
            }
            return (await ClientFrames[i].SendIpIdeaAsync(IpPort, api), i);
        }

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        /// <param name="i">要重连的下标</param>
        /// <returns></returns>
        public async Task<bool> Reconnection(int i)
        {
            if (i > -1 && i < ClientCount)
            {
                return await ClientFrames[i].Reconnection();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 同步发送消息（多服务器协调发送）
        /// </summary>
        /// <param name="i">向那个服务器端口发包</param>
        /// <param name="api">接口调用信息</param>
        /// <returns>返回数据包</returns>
        public NetResponse Send(int i, ApiPacket api)
        {
            if (i >= ClientCount)
            {
                i = 0;
            }
            return ClientFrames[i].Send(api);
        }

        /// <summary>
        /// 异步发送消息（多服务器协调发送）
        /// </summary>
        /// <param name="i">向那个服务器端口发包</param>
        /// <param name="api">接口调用信息</param>
        public async Task<NetResponse> SendAsync(int i, ApiPacket api)
        {
            if (i >= ClientCount)
            {
                i = 0;
            }
            return await ClientFrames[i].SendAsync(api);
        }

        /// <summary>
        /// 回收连接对象池，释放相关的全部连接
        /// </summary>
        public void Dispose()
        {
            lock (_lock) 
            {
                foreach (var item in ClientFrames)
                {
                    item.Close();
                }

                ClientFrames.Clear();
            }
        }
    }
}
