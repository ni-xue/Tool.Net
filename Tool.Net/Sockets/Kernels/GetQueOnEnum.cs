using System;
using System.Collections.Generic;
using System.Text;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 存储事件队列的类
    /// </summary>
    internal partial class GetQueOnEnum
    {
        /// <summary>
        /// 构造服务端或客户端事件
        /// </summary>
        /// <param name="Key">IP+端口</param>
        /// <param name="EnumAction">事件枚举</param>
        /// <param name="Completed">委托事件</param>
        public GetQueOnEnum(string Key, Enum EnumAction, Action<string, Enum, DateTime> Completed)
        {
            this.Key = Key;
            this.EnumAction = EnumAction;
            Time = DateTime.Now;
            EnCompleted = Completed;
        }

        /// <summary>
        /// 启动指定方法
        /// </summary>
        public void Completed()
        {
            try
            {
                //switch (EnumAction)
                //{
                //    case EnClient:
                //        (EnCompleted as Action<string, EnClient, DateTime>)?.Invoke(Key, (EnClient)EnumAction, Time);
                //        break;
                //    case EnServer:
                //        (EnCompleted as Action<string, EnServer, DateTime>)?.Invoke(Key, (EnServer)EnumAction, Time);
                //        break;
                //}

                EnCompleted?.Invoke(Key, EnumAction, Time);

                //if (EnumAction is EnClient client) // (typeof(EnClient) == EnumAction.GetType())
                //{
                //    (EnCompleted as Action<string, EnClient, DateTime>)?.Invoke(Key, client, Time);
                //}
                //else if (EnumAction is EnServer server) //(typeof(EnServer) == EnumAction.GetType())
                //{
                //    (EnCompleted as Action<string, EnServer, DateTime>)?.Invoke(Key, server, Time);
                //}
            }
            catch (Exception e)
            {
                Log.Error("TCP事件调用异常", e);
            }
        }

        /// <summary>
        /// IP+端口
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 消息发生时间
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// 回调函数
        /// </summary>
        public Action<string, Enum, DateTime> EnCompleted { get; set; }

        /// <summary>
        /// 客户端或服务器枚举
        /// </summary>
        public Enum EnumAction { get; set; }
    }
}
