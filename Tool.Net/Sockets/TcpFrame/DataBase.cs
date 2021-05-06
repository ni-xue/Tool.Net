using System;
using System.Collections.Generic;
using System.Text;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 提供的唯一数据包接口类，必须实现
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class DataBase : IDisposable
    {
        /// <summary>
        /// 必须定义ID，并保证唯一
        /// </summary>
        /// <param name="ClassID"></param>
        public DataBase(byte ClassID)
        {
            this.ClassID = ClassID;
            //Form = new Dictionary<string, string>().AsReadOnly();
        }

        /**
         * 初始化数据绑定
         */
        private void Initialize(string OnlyID, string IPEndPoint, string Data, byte[] Bytes)
        {
            this.IPEndPoint = IPEndPoint;
            this.OnlyID = OnlyID;
            this.Bytes = Bytes;
            if (Data != null)
            {
                string objstr = Data;
                if (objstr.Length > 0)
                {
                    var keys = new Dictionary<string, string>();

                    string[] forms = objstr.Split("&");
                    foreach (var form in forms)
                    {
                        string[] kv = form.Split("=");
                        keys.Add(kv[0], kv[1]);
                    }
                    this.Form = keys.AsReadOnly();
                }
            }
        }

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
                    if (this.Form.TryGetValue(parameter.Name, out string value))
                    {
                        object _obj = value.ToVar(parameter.ParameterType, false);
                        if (_obj == null)
                        {
                            paras[i] = parameter.DefaultValue.GetType() == typeof(DBNull) ? parameter.ParameterObj : parameter.DefaultValue;
                        }
                        else
                        {
                            paras[i] = _obj;
                        }
                    }
                    else
                    {
                        paras[i] = parameter.DefaultValue.GetType() == typeof(DBNull) ? parameter.ParameterObj : parameter.DefaultValue;
                    }
                }
                else
                {
                    paras[i] = parameter.DefaultValue.GetType() == typeof(DBNull) ? parameter.ParameterObj : parameter.DefaultValue;
                }
            }

            return paras;
        }

        internal DataPacket Request(DataPacket packet, string ip, DataTcp dataTcp)
        {
            this.Initialize(packet.OnlyID, ip, packet.Obj, packet.Bytes);

            object[] paras = this.GetForm(dataTcp.Parameters);

            DataPacket dataPacket = new()
            {
                ClassID = packet.ClassID,
                ActionID = packet.ActionID,
                OnlyId = packet.OnlyId,

                //OnlyID = packet.OnlyID,
                //ObjType = packet.ObjType,
                IsSend = false,
                //IsErr = false,
                IsServer = !packet.IsServer,
                IsAsync = packet.IsAsync,
                //IsIpIdea = packet.IsIpIdea,
                IpPort = packet.IpPort,
            };

            try
            {
                if (this.Initialize(dataTcp))
                {
                    object obj = dataTcp.Action.Invoke(this, paras);
                    if (obj != null)
                    {
                        switch (dataTcp.ObjType)
                        {
                            case DataTcpState.Byte:
                                if (obj is byte[])
                                {
                                    dataPacket.Bytes = obj.ToVar<byte[]>();
                                }
                                break;
                            case DataTcpState.Json:
                                if (obj is string)
                                {
                                    dataPacket.Obj = obj.ToString();
                                }
                                else
                                {
                                    dataPacket.Obj = obj.ToJson();
                                }
                                break;
                            case DataTcpState.String:
                                dataPacket.Obj = obj.ToString();
                                break;
                        }

                        //Type type = obj.GetType();
                        //string strobj;
                        //if (type.IsType())
                        //{
                        //    strobj = obj.ToString();
                        //}
                        //else
                        //{
                        //    strobj = obj.ToJson();
                        //}

                        //byte[] strbytes = Encoding.UTF8.GetBytes(strobj);

                        //string sd = Convert.ToString(strbytes[11], 16);

                        //string as1 = DataPacket.StringHex(strobj);
                        //hex.Length

                        //byte by = Convert.ToByte("73", 16);
                        //if (strobj.Contains("}{"))
                        //{
                        //    strobj = strobj.Replace("}{", "} {");
                        //}
                        //if (strobj.Contains("\""))
                        //{
                        //    strobj = strobj.Replace("\"", "\\\"");//解析
                        //}

                        //int listData = Encoding.UTF8.GetByteCount(strobj);//给定空包为200

                        //if (listData > BufferSize)
                        //{
                        //    double count = (double)listData / BufferSize;
                        //    if (count > (int)count)
                        //    {
                        //        count++;
                        //    }

                        //    dataPacket.Many = string.Concat("0/", (byte)count); //$"0/{(int)(count)}";
                        //}

                        //dataPacket.SetMany(listData, BufferSize);
                        //dataPacket.Obj = strobj;
                    }
                    //dataPacket.Obj = strobj;
                    dataPacket.IsErr = false;
                }
            }
            catch (Exception ex)
            {
                dataPacket.IsErr = true;
                dataPacket.Obj = "接口调用异常";
                this.TcpException(ex);
            }
            //string as1 = dataPacket.StringData();
            //DataPacket packet1 = DataPacket.DataString(as1);
            return dataPacket;
        }

        /// <summary>
        /// 为当前类设置一个唯一ID，用于通讯
        /// </summary>
        public byte ClassID { get; }

        /// <summary>
        /// 发送的参数
        /// </summary>
        public IReadOnlyDictionary<string, string> Form { get; private set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public string OnlyID { get; private set; }

        /// <summary>
        /// 消息接收的字节流数据
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// 数据交互的方
        /// </summary>
        public string IPEndPoint { get; private set; }

        /// <summary>
        /// 当消息真实有效时被执行，默认返回执行。（该方法是用于给使用者重写的）
        /// </summary>
        /// <param name="dataTcp">调用方法信息</param>
        protected virtual bool Initialize(DataTcp dataTcp)
        {
            return true;
        }

        /// <summary>
        /// 当前API消息发生异常时触发
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        protected virtual void TcpException(Exception ex)
        {
            throw ex;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 用于开发者重写的回收
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
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
            Dispose(true);

            //Form.Clear();
            Form = null;
            OnlyID = null;
            Bytes = null;
            IPEndPoint = null;
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
