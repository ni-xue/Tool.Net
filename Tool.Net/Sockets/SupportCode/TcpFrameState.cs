using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// 对TcpFrame响应的状态
    /// </summary>
    public enum TcpFrameState
    {
        /// <summary>
        /// 表示，无任何动作！
        /// </summary>
        Default = 0,
        /// <summary>
        /// 表示，因出现新的相同的消息ID，前一个将被强制关闭，以保证唯一性！
        /// </summary>
        OnlyID = 100,
        /// <summary>
        /// 表示，被触发了！
        /// </summary>
        Success = 200,
        /// <summary>
        /// 数据发送失败，请查看详细错误。
        /// </summary>
        SendFail = 300,
        /// <summary>
        /// 表示，超时了！
        /// </summary>
        Timeout = 400,
        /// <summary>
        /// 表示，发生异常！
        /// </summary>
        Exception = 500
    }
}
