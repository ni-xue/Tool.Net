using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.UdpHelper;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// UDP/TCP 协议模式 保证数据 握手模式，验证双方是否满足条件
    /// </summary>
    internal static class Handshake
    {
        /// <summary>
        /// 描述UDP可靠传输验证协议
        /// </summary>
        private static readonly ArraySegment<byte> Bytes = new byte[] { 0, 2, 4, 8, 16, 32, 64, 128, 0 }; //000204081020408000

        /// <summary>
        /// 描述UDP可靠传输完成协议
        /// </summary>
        private static readonly ArraySegment<byte> Ready = new byte[] { 0, 128, 64, 32, 16, 8, 4, 2, 0 }; //008040201008040200

        #region UDP模块

        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        public static async Task UdpAuthenticAtion(Socket socket, UdpEndPoint endPoint)
        {
            try
            {
                await socket.SendToAsync(Bytes, SocketFlags.None, endPoint).IsNewTask();
                ArraySegment<byte> buffer = new byte[9];
                using CancellationTokenSource tokenSource = new(10000);
                int coun = await ReceiveAsync(socket, endPoint, buffer, tokenSource.Token);
                if (!Utility.SequenceCompare(Ready, buffer[..coun]))
                {
                    throw new Exception("于服务器协议不一致！");
                }
            }
            catch (Exception)
            {
                socket.Dispose();//回收资源
                throw;
            }

        }

        /// <summary>
        /// 签名（认证对方信息，并签名返回）
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UdpAutograph(Socket socket, UdpEndPoint endPoint, Memory<byte> buffer)
        {
            try
            {
                //System.Net.NetworkInformation.IPGlobalProperties
                if (Utility.SequenceCompare(Bytes, buffer[..Bytes.Count].Span))
                {
                    await socket.SendToAsync(Ready, SocketFlags.None, endPoint).IsNewTask();
                    return true;
                }
                else
                {
                    Debug.Fail("于客户端协议不一致！");
                    //throw new Exception("于客户端协议不一致！");
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task<int> ReceiveAsync(Socket socket, UdpEndPoint endPoint, Memory<byte> buffer, CancellationToken token)
        {
            SocketReceiveFromResult result;
            try
            {
#if NET5_0
                result = await Task.Run(() => socket.ReceiveFromAsync(buffer.AsArraySegment(), SocketFlags.None, endPoint), token);
#else
                result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, endPoint, token);
#endif
            }
            catch (OperationCanceledException)
            {
                throw new Exception("连接超时，或服务器不应答，请检查于服务器连接是否一致！");
            }
            catch (Exception)
            {
                throw new Exception("请检查，服务配置后重试！");
            }

            if (result.RemoteEndPoint is UdpEndPoint point && point == endPoint)
            {
                return result.ReceivedBytes;
            }
            else
            {
                throw new Exception("验证方，非连接方！");
            }
        }

        #endregion

        #region TCP模块

        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        public static async Task TcpAuthenticAtion(Socket socket)
        {
            try
            {
                await socket.SendAsync(Bytes, SocketFlags.None).IsNewTask();
                ArraySegment<byte> buffer = new byte[9];
                using CancellationTokenSource tokenSource = new(10000);
                int coun = await ReceiveAsync(socket, buffer, tokenSource.Token);
                if (!Utility.SequenceCompare(Ready, buffer[..coun]))
                {
                    throw new Exception("于服务器协议不一致！");
                }
            }
            catch (Exception)
            {
                socket.Dispose();//回收资源
                throw;
            }

        }

        /// <summary>
        /// 签名（认证对方信息，并签名返回）
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> TcpAutograph(Socket socket)
        {
            try
            {
                Memory<byte> buffer = new byte[9];
                using CancellationTokenSource tokenSource = new(10000);
                int coun = await ReceiveAsync(socket, buffer, tokenSource.Token);

                //System.Net.NetworkInformation.IPGlobalProperties
                if (Utility.SequenceCompare(Bytes, buffer[..Bytes.Count].Span))
                {
                    await socket.SendAsync(Ready, SocketFlags.None).IsNewTask();
                    return true;
                }
                else
                {
                    Debug.Fail("于客户端协议不一致！");
                    socket.Dispose();
                    //throw new Exception("于客户端协议不一致！");
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async ValueTask<int> ReceiveAsync(Socket socket, Memory<byte> buffer, CancellationToken token)
        {
            try
            {
                return await socket.ReceiveAsync(buffer, SocketFlags.None, token);
            }
            catch (OperationCanceledException)
            {
                throw new Exception("连接超时，或服务器不应答，请检查于服务器连接是否一致！");
            }
            catch (Exception)
            {
                throw new Exception("请检查，服务配置后重试！");
            }
        }

        #endregion
    }
}
