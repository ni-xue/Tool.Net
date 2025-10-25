using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Sockets相关扩展支持
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class SocketsExtension
    {
        /// <summary>
        /// WebSocketMessageType 消息状态枚举
        /// </summary>
        public const string WebSocketMessageType = "WebSocketMessageType";

        private static void Set(Dictionary<string, object> datastate, string name, object value) => datastate[name] = value;

        private static object Get(Dictionary<string, object> datastate, string name) => datastate.TryGetValue(name, out object value) ? value : default;

        private static T Get<T>(Dictionary<string, object> datastate, string name) => datastate.TryGetValue(name, out object value) && value is T varAsT ? varAsT : default;

        #region WebClientAsync AND WebServerAsync

        /// <summary>
        /// 设置消息状态
        /// </summary>
        /// <typeparam name="T">连接对象</typeparam>
        /// <param name="sendBytes">消息缓冲区</param>
        /// <param name="type">枚举状态</param>
        public static void SetMessageType<T>(this SendBytes<T> sendBytes, WebSocketMessageType type) => SetMessageType(sendBytes.Client, sendBytes.datastate, type);

        /// <summary>
        /// 读取消息状态
        /// </summary>
        /// <typeparam name="T">连接对象</typeparam>
        /// <param name="sendBytes">消息缓冲区</param>
        /// <returns>枚举状态</returns>
        public static WebSocketMessageType GetMessageType<T>(this SendBytes<T> sendBytes) => GetMessageType(sendBytes.Client, sendBytes.datastate);

        /// <summary>
        /// 设置消息状态
        /// </summary>
        /// <typeparam name="T">连接对象</typeparam>
        /// <param name="receiveBytes">消息缓冲区</param>
        /// <param name="type">枚举状态</param>
        public static void SetMessageType<T>(this ReceiveBytes<T> receiveBytes, WebSocketMessageType type) => SetMessageType(receiveBytes.Client, receiveBytes.datastate, type);

        /// <summary>
        /// 读取消息状态
        /// </summary>
        /// <typeparam name="T">连接对象</typeparam>
        /// <param name="receiveBytes">消息缓冲区</param>
        /// <returns>枚举状态</returns>
        public static WebSocketMessageType GetMessageType<T>(this ReceiveBytes<T> receiveBytes) => GetMessageType(receiveBytes.Client, receiveBytes.datastate);

        private static WebSocketMessageType GetMessageType<T>(T client, Dictionary<string, object> datastate)
        {
            if (client is WebSocketContext or WebSocket)
            {
                return Get<WebSocketMessageType>(datastate, WebSocketMessageType);
            }
            else
            {
                throw new System.Exception("非WebSocket不能调用！");
            }
        }

        private static void SetMessageType<T>(T client, Dictionary<string, object> datastate, WebSocketMessageType type)
        {
            if (client is WebSocketContext or WebSocket)
            {
                if (type is System.Net.WebSockets.WebSocketMessageType.Close) throw new System.Exception("WebSocketMessageType.Close 是不能用于发送数据包的！");
                Set(datastate, WebSocketMessageType, type);
            }
            else 
            {
                throw new System.Exception("非WebSocket不能调用！"); 
            }
        }

        #endregion

        /// <summary>
        /// 从消息中读取所有数据并转换为字符串
        /// </summary>
        /// <typeparam name="T">连接对象</typeparam>
        /// <param name="receiveBytes">消息缓冲区</param>
        /// <param name="encoding">编码，默认UTF-8</param>
        /// <returns>字符串</returns>
        public static string GetString<T>(this ReceiveBytes<T> receiveBytes, Encoding encoding = null) 
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(receiveBytes.Span);
        }
    }
}
