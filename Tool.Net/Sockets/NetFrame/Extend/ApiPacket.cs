using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Tool.Sockets.Kernels;
using Tool.Utils;
using Tool.Utils.Data;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 接口请求数据包
    /// </summary>
    public class ApiPacket
    {
        ///// <summary>
        ///// 该字段默认 为true, 出现这个字段的本意是 作者认为， 通知都在线程池中操作 用 同步方案 好像很合理。
        ///// <para></para>
        ///// 但是实际情况是 好像 IO 线程 可以帮忙 所以默认是 启用异步通讯，可以根据自己的实际效果而定。
        ///// <para></para>
        ///// 这里设置成 true, 默认用全用异步发送，设置为false 将根据请求 类型 选择相对于的方式
        ///// </summary>
        //public static bool TcpAsync = true;

        /// <summary>
        /// 数据包初始化
        /// </summary>
        /// <param name="ClassID">类ID</param>
        /// <param name="ActionID">方法ID</param>
        public ApiPacket(byte ClassID, byte ActionID) : this(ClassID, ActionID, 60 * 1000)
        {
        }

        /// <summary>
        /// 数据包初始化
        /// </summary>
        /// <param name="ClassID">类ID</param>
        /// <param name="ActionID">方法ID</param>
        /// <param name="Millisecond">请求等待的毫秒</param>
        public ApiPacket(byte ClassID, byte ActionID, int Millisecond) : this(ClassID, ActionID, Millisecond, true)
        {
            
        }

        /// <summary>
        /// 数据包初始化
        /// </summary>
        /// <param name="ClassID">类ID</param>
        /// <param name="ActionID">方法ID</param>
        /// <param name="Millisecond">请求等待的毫秒</param>
        /// <param name="IsReply">是否需要有回复消息</param>
        public ApiPacket(byte ClassID, byte ActionID, int Millisecond, bool IsReply)
        {
            this.ClassID = ClassID;
            this.ActionID = ActionID;
            this.Millisecond = Millisecond;
            this.IsReply = IsReply;
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// 请求的类ID
        /// </summary>
        public byte ClassID { get; }

        /// <summary>
        /// 请求的方法ID
        /// </summary>
        public byte ActionID { get; }

        /// <summary>
        /// 默认等待超时时间为60秒
        /// </summary>
        public int Millisecond { get; }

        /// <summary>
        /// 是否需要有回复消息
        /// </summary>
        public bool IsReply { get; }

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public ArraySegment<byte> Bytes { get; set; } = null;

        /**
         * 发送的参数
         */
        internal readonly Dictionary<string, string> Data;

        internal Ipv4Port ipPort;
        internal bool isServer;

        /// <summary>
        /// 加入数据（如果有则修改）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值（支持传输转义）</param>
        public void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new FormatException("当前key的值，不能为空。");
            }

            char c = key[0];

            if (!('A' <= c && c <= 'Z' || 'a' <= c && c <= 'z'))
            {
                throw new FormatException("当前key的值不符合参数名的定义，请以首字母定义。");
            }

            var val = string.Concat(value);
            if (!Data.TryAdd(key, val))
            {
                Data[key] = val;
            }

            //if (key.Contains('=') || key.Contains('&'))
            //{
            //    throw new FormatException("当前key的值存在【=或&】符号，需要转义后再加入。");
            //}

            //var val = value.ToString();

            //if (val.Contains('=') || val.Contains('&'))
            //{
            //    throw new FormatException("当前value的值存在【=或&】符号，需要转义后再加入。");
            //}

            //if (value.GetType().IsType())
            //{
            //    var val = value ?? value.ToString();
            //    if (Data.ContainsKey(key))
            //    {
            //        Data[key] = val;
            //    }
            //    else
            //    {
            //        Data.Add(key, val);
            //    }
            //}
            //else
            //{
            //    throw new FormatException("当前value的值，不是系统变量值，无法发送。");
            //}
        }

        /// <summary>
        /// 加入数据,如果有则修改（以虚构对象参数传入，请确保已认真读注释。）（支持传输转义）
        /// </summary>
        /// <param name="dictionary">虚构对象</param>
        public void Set(object dictionary)
        {
            var _objs = dictionary.GetDictionary();

            foreach (var item in _objs)
            {
                string key = item.Key;

                object value = item.Value;

                Set(key, value);
            }
        }

        /// <summary>
        /// 获取键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">要返回的值</param>
        /// <returns>是否存在</returns>
        public bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        /// <summary>
        /// 从发送数据中移除所指定的键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功移除</returns>
        public bool Remove(string key)
        {
            return Data.Remove(key);
        }

        /**
         * 返回 Format 表单提交
         */
        internal string FormatData()
        {
            //var s = Data is null
            //    ? string.Empty
            //    : string.Join("&",
            //        Data.Select(d => string.Concat(d.Key, "=", d.Value)));
            return HttpHelpers.QueryString(Data);
        }
    }
}
