using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Tool.Sockets.SupportCode;

namespace Tool.Sockets.WebTcp
{
    /// <summary>
    /// 获取当客户端的详细信息
    /// </summary>
    public class WebContext
    {
        /// <summary>
        /// 连接时的原始对象信息
        /// </summary>
        internal HttpListenerContext httpListenerContext { get; }

        /// <summary>
        /// 握手后的重要数据
        /// </summary>
        internal HttpListenerWebSocketContext httpListenerWebSocketContext { get; }

        /// <summary>
        /// 当前客户端IP信息
        /// </summary>
        public string IpPort { get; }

        /// <summary>
        /// 当前连接客户端信息
        /// </summary>
        public WebSocket Socket { get { return httpListenerWebSocketContext.WebSocket; } }

        /// <summary>
        /// 返回 WebSocket 连接的当前状态。
        /// </summary>
        /// <remarks>WebSocket 连接的当前状态。</remarks>
        public WebSocketState State { get { return Socket.State; } }

        /// <summary>
        /// 创建用户连接信息存储对象
        /// </summary>
        /// <param name="httpListenerContext">用户连接凭证</param>
        public WebContext(HttpListenerContext httpListenerContext)
        {
            if (httpListenerContext == null)
            {
                throw new NullReferenceException("httpListenerContext 对象不能为空！");
            }

            this.httpListenerContext = httpListenerContext;

            IpPort = WebStateObject.GetIpPort(httpListenerContext);

            this.httpListenerWebSocketContext = httpListenerContext.AcceptWebSocketAsync(null).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 关闭当前用户连接以及数据
        /// </summary>
        public void Close()
        {
            Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "已经断开连接！", System.Threading.CancellationToken.None).ContinueWith((i, ip) =>
            {
                if (i.IsCompleted)
                {
                    Debug.WriteLine("客户端：{0}，已经断开！", ip);
                }
            }, IpPort);
            //Socket.Abort();
        }
    }
}
