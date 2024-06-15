using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 提供的唯一数据包接口类，必须实现
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class DataBase : IDisposable
    {
        private IDataPacket packet;

        /**
         * 获取方法调用参数
         * parameters 参数信息
         * 参数值
         */
        private object[] GetForm(Parameter[] parameters)
        {
            object[] paras = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter parameter = parameters[i];
                if (parameter.IsType)
                {
                    if (this.Form?.TryGetValue(parameter.Name, out string value) ?? false)
                    {
                        object _obj = value.ToVar(parameter.ParameterType, false);
                        if (_obj == null)
                        {
                            paras[i] = parameter.ValueOrObj;
                        }
                        else
                        {
                            paras[i] = _obj;
                        }
                    }
                    else
                    {
                        paras[i] = parameter.ValueOrObj;
                    }
                }
                else
                {
                    paras[i] = parameter.ValueOrObj;
                }
            }

            return paras;
        }

        internal async Task<IDataPacket> RequestAsync(IDataPacket _packet, Ipv4Port ip, DataNet dataTcp)
        {
            IPEndPoint = ip;
            packet = _packet;

            this.Form = HttpHelpers.FormatData(packet.Text)?.AsReadOnly();

            object[] paras = this.GetForm(dataTcp.Parameters);

            SendDataPacket dataPacket = new(packet.ClassID, packet.ActionID, packet.OnlyId)
            {
                IsSend = false,
                IsServer = !packet.IsServer,
                IsReply = packet.IsReply,
                IpPort = packet.IpPort,
            };

            try
            {
                if (this.Initialize(dataTcp))
                {
                    IGoOut goobj;
                    if (dataTcp.IsTask)
                    {
                        goobj = await dataTcp.Action.ExecuteAsync(this, paras);
                    }
                    else
                    {
                        goobj = dataTcp.Action.Execute(this, paras);
                    }
                    if (IsReply)
                    {
                        dataPacket.SetBuffer(goobj.Bytes);
                        dataPacket.Text = goobj.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                dataPacket.SetErr("接口调用异常");
                try
                {
                    this.NetException(ex);
                }
                catch (Exception)
                {
                }
            }
            return dataPacket;
        }

        /// <summary>
        /// 数据交互的方
        /// </summary>
        public Ipv4Port IPEndPoint { get; private set; }

        /// <summary>
        /// 发送的参数
        /// </summary>
        public IReadOnlyDictionary<string, string> Form { get; private set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string OnlyID => OnlyId.ToString();

        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid OnlyId => packet.OnlyId;

        /// <summary>
        /// 是否需要有回复消息
        /// </summary>
        public bool IsReply => packet.IsReply;

        /// <summary>
        /// 消息接收的字节流数据
        /// </summary>
        public ArraySegment<byte> Array => packet.Bytes.AsArraySegment();

        /// <summary>
        /// 消息接收的字节流数据
        /// </summary>
        public Span<byte> Bytes => MemoryBytes.Span;

        /// <summary>
        /// 消息接收的字节流数据
        /// </summary>
        public Memory<byte> MemoryBytes => packet.Bytes;

        /// <summary>
        /// 当消息真实有效时被执行，默认返回执行。（该方法是用于给使用者重写的）
        /// </summary>
        /// <param name="dataTcp">调用方法信息</param>
        protected virtual bool Initialize(DataNet dataTcp)
        {
            return true;
        }

        #region 同步返回模块

        /// <summary>
        /// 默认完成结果
        /// </summary>
        /// <returns></returns>
        public IGoOut Ok() => GoOut.Empty;

        /// <summary>
        /// 完成结果,并输出类容
        /// </summary>
        /// <param name="text">文本类容</param>
        /// <param name="bytes">字节流类容</param>
        /// <returns></returns>
        public IGoOut Ok(string text, ArraySegment<byte> bytes)
        {
            if (text == null) throw new ArgumentException("参数为空！", nameof(text));
            if (bytes == null) throw new ArgumentException("参数为空！", nameof(bytes));
            return new GoOut(bytes, text);
        }

        /// <summary>
        /// 完成结果,并输出文本类容
        /// </summary>
        /// <param name="text">文本类容</param>
        /// <returns></returns>
        public IGoOut Write(string text)
        {
            if (text == null) throw new ArgumentException("参数为空！", nameof(text));
            return new GoOut(text);
        }

        /// <summary>
        /// 完成结果,并输出字节流类容
        /// </summary>
        /// <param name="bytes">字节流类容</param>
        /// <returns></returns>
        public IGoOut Write(ArraySegment<byte> bytes)
        {
            if (bytes == null) throw new ArgumentException("参数为空！", nameof(bytes));
            return new GoOut(bytes);
        }

        /// <summary>
        /// 完成结果,返回Json格式数据
        /// </summary>
        /// <param name="json">Json格式数据</param>
        /// <returns></returns>
        public IGoOut Json(object json)
        {
            if (json == null) throw new ArgumentException("参数为空！", nameof(json));
            string jsonStr;
            if (json is string str) jsonStr = str; else jsonStr = json.ToJson();
            return new GoOut(jsonStr);
        }

        #endregion

        #region 异步返回模块

        /// <summary>
        /// 默认完成结果
        /// </summary>
        /// <returns></returns>
        public Task<IGoOut> OkAsync() => Task.FromResult(Ok());

        /// <summary>
        /// 完成结果,并输出类容
        /// </summary>
        /// <param name="text">文本类容</param>
        /// <param name="bytes">字节流类容</param>
        /// <returns></returns>
        public Task<IGoOut> OkAsync(string text, ArraySegment<byte> bytes) => Task.FromResult(Ok(text, bytes));

        /// <summary>
        /// 完成结果,并输出文本类容
        /// </summary>
        /// <param name="text">文本类容</param>
        /// <returns></returns>
        public Task<IGoOut> WriteAsync(string text) => Task.FromResult(Write(text));

        /// <summary>
        /// 完成结果,并输出字节流类容
        /// </summary>
        /// <param name="bytes">字节流类容</param>
        /// <returns></returns>
        public Task<IGoOut> WriteAsync(ArraySegment<byte> bytes) => Task.FromResult(Write(bytes));

        /// <summary>
        /// 完成结果,返回Json格式数据
        /// </summary>
        /// <param name="json">Json格式数据</param>
        /// <returns></returns>
        public Task<IGoOut> JsonAsync(object json) => Task.FromResult(Json(json));

        #endregion

        /// <summary>
        /// 当前API消息发生异常时触发
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        protected virtual void NetException(Exception ex)
        {
            //throw ex;
        }

        #region IDisposable Support
        //private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 用于开发者重写的回收（可回收使用的非托管资源）
        /// </summary>
        protected virtual void Dispose()
        {

        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DataBase()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        /// 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose();

            //Form.Clear();
            Form = null;
            IPEndPoint = null;
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
