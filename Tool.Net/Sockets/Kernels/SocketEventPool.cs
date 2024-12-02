using System.Net.Sockets;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Socket异步传输池
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class SocketEventPool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SocketAsyncEventArgs Pop() 
        {
            return new();
        }
    }
}
