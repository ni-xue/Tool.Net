using Microsoft.AspNetCore.DataProtection;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.SupportCode;

namespace Tool.Sockets.TcpHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class TcpClient : IDisposable
    {
        private readonly int _dataLength;

        private readonly byte[] _bytesSend;
        private Memory<byte> MemorySend => _bytesSend;

        private readonly byte[] _bytesReceive;
        private Memory<byte> MemoryReceive => _bytesReceive;

        private readonly Socket _socket;

        private readonly SocketAsyncEventArgs _eventSend;

        private readonly SocketAsyncEventArgs _eventReceive;

        //private readonly Thread _threadSend;

        //private readonly Thread _threadReceive;

        private readonly CancellationTokenSource cancelTokenSource;

        private readonly ManualResetEvent doSend;

        //private readonly ManualResetEvent doReceive;

        private readonly ManualResetEvent _mre;

        private readonly ConcurrentQueue<SendBytes> _queSend;

        //private readonly Semaphore semaphore;

        /// <summary>
        /// 构造连接流协议
        /// </summary>
        /// <param name="socket">连接对象</param>
        /// <param name="DataLength"></param>
        public TcpClient(Socket socket, int DataLength)
        {
            if (socket is null) throw new("未提供链接对象。");
            _socket = socket;
            _dataLength = DataLength;

            _bytesSend = new byte[_dataLength];
            _bytesReceive = new byte[_dataLength];

            _eventSend = new SocketAsyncEventArgs(true) { AcceptSocket = socket };
            _eventSend.Completed += _OnCompleted;
            _eventReceive = new SocketAsyncEventArgs(true) { AcceptSocket = socket };
            _eventReceive.Completed += _OnCompleted;

            //_threadSend = new Thread(SendAsync)
            //{
            //    Name = "日志线程",
            //    IsBackground = true,//false
            //    Priority = ThreadPriority.Highest
            //};

            //_threadReceive = new Thread(ReceiveAsync)
            //{
            //    Name = "日志线程",
            //    IsBackground = true,//false
            //    Priority = ThreadPriority.HighestCountdownEvent
            //};

            cancelTokenSource = new CancellationTokenSource();

            doSend = new(false);
            //doReceive = new(false);
            _mre = new(false);
            _queSend = new();

            //semaphore = new(1, 10000);

            //_threadSend.Start();
            //_threadReceive.Start();

            Task.Run(Initialize);

            KeepAlive keep = new(1, () =>
            {
                Console.Clear();
                Console.WriteLine("情况：{0}，{1}，{2}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount);
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(i);
                    Console.WriteLine("计数：{0}", a);
                }
            });
        }

        private void Initialize()
        {
            ReceiveAsync();

            while (true)
            {
                //SendAsync();
                if (true)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="memory">数据包</param>
        public void SendAsync(SendBytes memory)
        {
            try
            {
                _queSend.Enqueue(memory);

                //semaphore.WaitOne();
                //if (!_mre.SafeWaitHandle.IsClosed)
                //{
                //    _queSend.Enqueue(memory);
                //    _mre.Set();//启动
                //}

                SendAsync();
            }
            finally
            {
                //semaphore.Release();
            }
        }

        private void _OnCompleted(object sender, SocketAsyncEventArgs e) 
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Send:
                    _eventSend_Completed(e);
                    doSend.Set();
                    break;
                case SocketAsyncOperation.Receive:
                    _eventReceive_Completed(e);
                    break;

                //case SocketAsyncOperation.SendTo:
                //    break;
                //case SocketAsyncOperation.ReceiveFrom:
                //    break;

                default:
                    throw new("未实现的，异步通讯事件！");
            }
        }


        ulong a = 0;
        private void _eventSend_Completed(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError != SocketError.Success) throw new();       //异步处理失败，不做处理

                //Interlocked.Increment(ref a);

                SendAsync();
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void _eventReceive_Completed(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError != SocketError.Success && e.BytesTransferred < 1) throw new();       //异步处理失败，不做处理

                Console.WriteLine("头：{0}，长度：{1}", MemoryReceive.Span[0], e.BytesTransferred);

                ReceiveAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("断线");
                //throw;
            }
        }

        private void SendAsync()
        {
            if (!cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    _mre.WaitOne();
                    int Offset = 0;
                    A:
                    var objmy = MemorySend[Offset..];
                    while (!_queSend.IsEmpty && _queSend.TryDequeue(out SendBytes getbytes))
                    {
                        using (getbytes)
                        {
                            Interlocked.Increment(ref a);
                            if (Offset + getbytes.Length > _dataLength)
                            {
                                Send(MemorySend[..Offset]);
                                getbytes.Memory.CopyTo(MemorySend);
                                Offset = getbytes.Length;
                            }
                            else
                            {
                                getbytes.Memory.CopyTo(objmy);
                                Offset += getbytes.Length;
                            }

                            if (_queSend.IsEmpty)
                            {
                                Send(MemorySend[..Offset]);
                                Offset = 0;
                            }
                            else
                            {
                                goto A;
                            }
                        }

                        Thread.Sleep(1);
                    }
                }
                catch //(Exception ex)
                {

                }
                _mre.Reset();
            }

            void Send(Memory<byte> memory) 
            {
                if (_socket.Connected)
                {
                    //_eventSend.UserToken = getbytes;
                    _eventSend.SetBuffer(memory);
                    if (!_socket.SendAsync(_eventSend))
                    {
                        _eventSend_Completed(_eventSend);
                    }
                    else
                    {
                        doSend.WaitOne();
                    }
                }
            }
        }

        private void ReceiveAsync()
        {
            if (!cancelTokenSource.IsCancellationRequested)
            {
                _eventReceive.SetBuffer(MemoryReceive);
                if (!_socket.ReceiveAsync(_eventReceive))
                {
                    _eventReceive_Completed(_eventReceive);
                }
                //else
                //{
                //    doReceive.WaitOne();
                //}
                //_eventReceive.SetBuffer(0, _eventReceive.Count);
                //Thread.Sleep(200);
            }
        }

        /// <summary>
        /// 回收相关全部对象信息
        /// </summary>
        public void Dispose()
        {
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
            doSend.Close();
            doSend.Dispose();
            //doReceive.Close();
            //doReceive.Dispose();
            _eventSend.Dispose();
            _eventReceive.Dispose();
            _socket.Close();
            _socket.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
