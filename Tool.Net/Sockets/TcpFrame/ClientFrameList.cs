﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tool.Sockets.SupportCode;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 用于连接多服务器，分发消息的客户端帮助类，可以保证线程安全，均衡分发数据包。
    /// </summary>
    public class ClientFrameList
    {
        /// <summary>
        /// 绑定多服务器队列统一消息
        /// </summary>
        public event Action<string, EnClient, DateTime> Completed;

        /// <summary>
        /// lock 安全锁
        /// </summary>
        private int LockCount;

        /// <summary>
        /// 当前分发消息的服务器服务器队列
        /// </summary>
        private List<ClientFrame> ClientFrames { get; set; }

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
        public ClientFrameList(List<ClientFrame> clientFrames)
        {
            LockCount = -1;
            if (clientFrames == null)
            {
                throw new Exception("服务器队列不能为空");
            }
            else if (clientFrames.Count < 2)
            {
                throw new Exception("服务器队列必须大于2个以上");
            }
            ClientFrames = clientFrames;

            ClientFrames.ForEach(x => { x.SetCompleted(SetCompleted); });
        }

        /// <summary>
        /// 初始化一次性加入队列服务器
        /// </summary>
        /// <param name="clientFrames">队列服务器</param>
        public ClientFrameList(IList<ClientFrame> clientFrames)
        {
            LockCount = -1;
            if (clientFrames == null)
            {
                throw new Exception("服务器队列不能为空");
            }
            else if (clientFrames.Count < 2)
            {
                throw new Exception("服务器队列必须大于2个以上");
            }
            ClientFrames = new List<ClientFrame>(clientFrames);

            ClientFrames.ForEach(x => { x.SetCompleted(SetCompleted); });
        }

        /// <summary>
        /// 初始化一次性加入队列服务器
        /// </summary>
        /// <param name="clientFrames">队列服务器</param>
        public ClientFrameList(IEnumerable<ClientFrame> clientFrames)
        {
            LockCount = -1;
            if (clientFrames == null)
            {
                throw new Exception("服务器队列不能为空");
            }
            else if (clientFrames.Count() < 2)
            {
                throw new Exception("服务器队列必须大于2个以上");
            }
            ClientFrames = new List<ClientFrame>(clientFrames);

            ClientFrames.ForEach(x => { x.SetCompleted(SetCompleted); });
        }

        /// <summary>
        /// 初始化一次性加入队列服务器
        /// </summary>
        /// <param name="clientFrames">队列服务器</param>
        public ClientFrameList(params ClientFrame[] clientFrames)
        {
            LockCount = -1;
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

        private void SetCompleted(string arg1, EnClient arg2, DateTime arg3)
        {
            Completed?.Invoke(arg1, arg2, arg3);
        }

        /// <summary>
        /// 同步发送消息（多服务器协调发送）
        /// </summary>
        /// <param name="api">接口调用信息</param>
        /// <param name="i">返回成功发送包的下标</param>
        /// <returns>返回数据包</returns>
        public TcpResponse Send(ApiPacket api, out int i)
        {
            i = Interlocked.Increment(ref LockCount);
            if (ClientFrames.Count - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientFrames.Count);
            }
            if (i >= ClientFrames.Count)
            {
                i = 0;
            }
            return ClientFrames[i].Send(api);
        }

        /// <summary>
        /// 异步发送消息（多服务器协调发送）
        /// </summary>
        /// <param name="api">接口调用信息</param>
        /// <param name="action">异步回调返回消息</param>
        /// <param name="i">返回成功发送包的下标</param>
        public void SendAsync(ApiPacket api, Action<TcpResponse> action, out int i)
        {
            i = Interlocked.Increment(ref LockCount);
            if (ClientFrames.Count - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientFrames.Count);
            }
            if (i >= ClientFrames.Count)
            {
                i = 0;
            }
            ClientFrames[i].SendAsync(api, action);
        }

        /// <summary>
        /// 同步发送消息（多服务器协调发送+转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        /// <param name="i">返回成功发送包的下标</param>
        /// <returns>返回数据包</returns>
        public TcpResponse SendIpIdea(string IpPort, ApiPacket api, out int i)
        {
            i = Interlocked.Increment(ref LockCount);
            if (ClientFrames.Count - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientFrames.Count);
            }
            if (i >= ClientFrames.Count)
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
        /// <param name="action">异步回调返回消息</param>
        /// <param name="i">返回成功发送包的下标</param>
        public void SendIpIdeaAsync(string IpPort, ApiPacket api, Action<TcpResponse> action, out int i)
        {
            i = Interlocked.Increment(ref LockCount);
            if (ClientFrames.Count - 1 == i)
            {
                Interlocked.Add(ref LockCount, -ClientFrames.Count);
            }
            if (i >= ClientFrames.Count)
            {
                i = 0;
            }
            ClientFrames[i].SendIpIdeaAsync(IpPort, api, action);
        }

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        /// <param name="i">要重连的下标</param>
        /// <returns></returns>
        public bool Reconnection(int i)
        {
            if (i > -1 && i < ClientFrames.Count)
            {
                return ClientFrames[i].Reconnection();
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
        public TcpResponse Send(int i, ApiPacket api)
        {
            if (i >= ClientFrames.Count)
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
        /// <param name="action">异步回调返回消息</param>
        public void SendAsync(int i, ApiPacket api, Action<TcpResponse> action)
        {
            if (i >= ClientFrames.Count)
            {
                i = 0;
            }
            ClientFrames[i].SendAsync(api, action);
        }

    }
}