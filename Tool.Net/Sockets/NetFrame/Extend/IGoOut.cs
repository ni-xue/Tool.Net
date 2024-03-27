using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 实现<see cref="DataBase"/>协议，接口输出规范
    /// <para>两种数据格式可一起使用。</para>
    /// </summary>
    public interface IGoOut
    {
        /// <summary>
        /// 返回的数据流
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// 返回的文本类容
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// 默认实现
    /// </summary>
    public class GoOut : IGoOut
    {
        /// <summary>
        /// 初始化输出结果
        /// </summary>
        public GoOut()
        {

        }

        /// <summary>
        /// 初始化输出结果
        /// </summary>
        /// <param name="text">字符串类容</param>
        public GoOut(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// 初始化输出结果
        /// </summary>
        /// <param name="bytes">字节流类容</param>
        public GoOut(byte[] bytes)
        {
            this.Bytes = bytes;
        }

        /// <summary>
        /// 初始化输出结果
        /// </summary>
        /// <param name="bytes">字节流类容</param>
        /// <param name="text">字符串类容</param>
        public GoOut(byte[] bytes, string text)
        {
            this.Bytes = bytes;
            this.Text = text;
        }

        /// <summary>
        /// 返回的数据流
        /// </summary>
        public byte[] Bytes { get; set; } = null;

        /// <summary>
        /// 返回的文本类容
        /// </summary>
        public string Text { get; set; } = null;
    }
}
