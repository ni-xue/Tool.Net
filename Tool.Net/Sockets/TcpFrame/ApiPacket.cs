using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tool.Utils;
using Tool.Utils.Data;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 接口请求数据包
    /// </summary>
    public class ApiPacket
    {
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
        public ApiPacket(byte ClassID, byte ActionID, int Millisecond)
        {
            this.ClassID = ClassID;
            this.ActionID = ActionID;
            this.Millisecond = Millisecond;
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
        public int Millisecond { get; } = 60 * 1000;

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public byte[] Bytes { get; set; } = null;

        /**
         * 发送的参数
         */
        readonly internal Dictionary<string, string> Data;// { get; set; }

        /// <summary>
        /// 加入数据（如果有则修改）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
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

            if (key.Contains("=") || key.Contains("&"))
            {
                throw new FormatException("当前key的值存在【=或&】符号，需要转义后再加入。");
            }

            var val = value.ToString();

            if (val.Contains("=") || val.Contains("&"))
            {
                throw new FormatException("当前value的值存在【=或&】符号，需要转义后再加入。");
            }

            if (value.GetType().IsType())
            {
                if (Data.ContainsKey(key))
                {
                    Data[key] = val;
                }
                else
                {
                    Data.Add(key, val);
                }
            }
            else
            {
                throw new FormatException("当前value的值，不是系统变量值，无法发送。");
            }
        }

        /// <summary>
        /// 加入数据,如果有则修改（以虚构对象参数传入，请确保已认真读注释。）
        /// </summary>
        /// <param name="dictionary">虚构对象</param>
        public void Set(object dictionary)
        {
            var _objs = dictionary.ToDictionary();

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
            var s = Data == null
                ? string.Empty
                : string.Join("&",
                    Data.Select(d => string.Concat(d.Key, "=", d.Value)));

            //if (s.Contains("\""))
            //{
            //    s.Replace("\"", "\\\"");//解析
            //}
            return s;
        }
    }

    /**
     * 内部包
     */
    internal class ThreadObj : IDisposable
    {
        public ThreadObj(string onlyID)
        {
            AutoReset = new ManualResetEvent(false);
            Response = new TcpResponse(onlyID);
        }

        /**
         * 一个锁，保证其在线程中的安全
         */
        internal readonly object _lock = new();

        public int Count { get; set; }
        public byte[][] OjbCount { get; set; }

        public ManualResetEvent AutoReset { get; set; }

        public int Length { get; set; }

        public TcpResponse Response { get; set; }

        //public Action<TcpResponse> Action { get; set; }

        //~ThreadObj()
        //{
        //    Dispose();
        //}

        /**
         * 回收资源
         */
        public void Dispose()
        {
            if (AutoReset != null)
            {
                AutoReset.Dispose();
                AutoReset = null;
                Response = null;
                //Action = null;
            }
            Length = 0;
            OjbCount = null;
            Count = 0;
            GC.SuppressFinalize(this);
        }
    }
}
