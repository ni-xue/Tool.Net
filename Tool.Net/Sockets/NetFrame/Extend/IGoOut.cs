using System;

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
        public ArraySegment<byte> Bytes { get; }

        /// <summary>
        /// 返回的文本类容
        /// </summary>
        public string Text { get; }
    }

    /// <summary>
    /// 默认实现
    /// </summary>
    public readonly struct GoOut : IGoOut
    {
        /// <summary>
        /// 空对象
        /// </summary>
        public static GoOut Empty { get; } = new();

        ///// <summary>
        ///// 初始化输出结果
        ///// </summary>
        //public GoOut()
        //{

        //}

        /// <summary>
        /// 初始化输出结果
        /// </summary>
        /// <param name="text">字符串类容</param>
        public GoOut(string text)
        {
            this.Bytes = ArraySegment<byte>.Empty;
            this.Text = text;
        }

        /// <summary>
        /// 初始化输出结果
        /// </summary>
        /// <param name="bytes">字节流类容</param>
        public GoOut(ArraySegment<byte> bytes)
        {
            this.Bytes = bytes;
            this.Text = null;
        }

        /// <summary>
        /// 初始化输出结果
        /// </summary>
        /// <param name="bytes">字节流类容</param>
        /// <param name="text">字符串类容</param>
        public GoOut(ArraySegment<byte> bytes, string text)
        {
            this.Bytes = bytes;
            this.Text = text;
        }

        /// <summary>
        /// 返回的数据流
        /// </summary>
        public ArraySegment<byte> Bytes { get; }

        /// <summary>
        /// 返回的文本类容
        /// </summary>
        public string Text { get; }
    }
}
