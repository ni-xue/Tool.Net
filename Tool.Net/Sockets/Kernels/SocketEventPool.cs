using System.Net.Sockets;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Socket异步传输池
    /// </summary>
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
