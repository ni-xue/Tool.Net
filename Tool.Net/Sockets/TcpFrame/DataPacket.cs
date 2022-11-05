using System;
using System.Collections.Generic;
using System.Text;
using Tool.Sockets.SupportCode;

namespace Tool.Sockets.TcpFrame
{
    /**
    * 数据接收对象
    */
    //[Serializable]
    internal struct DataPacket //: IDisposable
    {
        /// <summary>
        /// 获取对应消息Key
        /// </summary>
        public string ActionKey => string.Concat(ClassID, '.', ActionID);

        /**
        * 通道ID
        */
        public byte ClassID { get; init; }

        /**
         * 事件ID
         */
        public byte ActionID { get; init; }

        /**
         * 唯一ID流
         */
        public Guid OnlyId { get; init; }//byte[]

        /**
         * 消息ID
         */
        public string OnlyID => OnlyId.ToString(); //{ get; }// set;

        /**
         * 消息是否是一部分
         */
        public Range Many { get; private set; } //= "0/0";//string

        public bool NotIsMany => Many.Equals(Range.All);

        /**
         * 当前包是发包还是回复
         */
        public bool IsSend { get; set; }

        /**
         * 当前包是否发生异常
         */
        public bool IsErr { get; set; }

        /**
         * 消息是发送给那一端
         */
        public bool IsServer { get; set; }

        /**
         * 是否异步，默认异步
         */
        public bool IsAsync { get; set; }

        /**
         * 是否转发数据，默认不转发
         */
        public bool IsIpIdea { get { return this.IpPort is not null; } }//{ get; set; }

        /**
        * 是否携带数据包 0 = 不携带， 1 = 携带字符串， 2 = 携带字节流， 3 = 携带字符串加字节流
        */
        public byte IsObj { get { return GetIsObj(); } }//{ get; set; }

        /**
         * 文本数据包
         */
        public string Text
        {
            get => TextBytes.Count is 0 ? null : Encoding.UTF8.GetString(TextBytes);
            set => TextBytes = value is null ? null : Encoding.UTF8.GetBytes(value);// ArraySegment<byte>.Empty 
        }

        /**
         * 文本流数据包
         */
        public ArraySegment<byte> TextBytes { get; set; }

        /**
         * 携带数据包
         */
        public ArraySegment<byte> Bytes { get; set; }

        /**
         * 当为转发时，转发给谁的IpPort
        */
        public string IpPort { get; set; }

        /**
         * 当前规定大小
        */
        private int BufferSize { get; set; }

        ///// <summary>
        ///// 内置数据源包
        ///// </summary>
        //internal object Data { get; set; }

        ///// <summary>
        ///// 返回当前对象的json字符串
        ///// </summary>
        ///// <returns></returns>
        //internal string Json()
        //{
        //    StringBuilder str = new StringBuilder("{\"OnlyID\":\"");
        //    str.Append(this.OnlyID);
        //    str.Append("\",\"IsSend\":");
        //    str.Append(this.IsSend ? "true" : "false");
        //    str.Append(",\"IsErr\":");
        //    str.Append(this.IsErr ? "true" : "false");
        //    str.Append(",\"IsServer\":");
        //    str.Append(this.IsServer ? "true" : "false");
        //    str.Append(",\"IsAsync\":");
        //    str.Append(this.IsAsync ? "true" : "false");
        //    str.Append(",\"Many\":\"");
        //    str.Append(this.Many);
        //    if (this.Obj == null)
        //    {
        //        str.Append("\",\"Obj\":null}");
        //    }
        //    else
        //    {
        //        str.Append("\",\"Obj\":\"");
        //        str.Append(this.Obj);
        //        str.Append("\"}");
        //    }
        //    string json = str.ToString();
        //    str.Clear();
        //    return json;
        //}

        private byte GetIsObj()
        {
            bool isbytes = Bytes.Count > 0; //!ArraySegment<byte>.Empty.Equals(Bytes);
            bool isobj = TextBytes.Count > 0; //!ArraySegment<byte>.Empty.Equals(TextBytes); //Obj != null;

            if (isbytes && isobj)
            {
                return 3;
            }
            else if (isbytes)
            {
                return 2;
            }
            else if (isobj)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /**
         * 判定是否需要分包
         */
        internal void SetMany(int BufferSize)
        {
            this.BufferSize = BufferSize;
            int listData = IsIpIdea ? 29 : 23;
            switch (IsObj)
            {
                case 3:
                    listData += TcpStateObject.HeadSize + this.TextBytes.Count + this.Bytes.Count;
                    break;
                case 2:
                    listData += this.Bytes.Count;
                    break;
                case 1:
                    listData += this.TextBytes.Count;
                    break;
            }

            if (listData > BufferSize)
            {
                //方案一
                //float count = listData / (float)BufferSize;
                //if (count > (int)count)
                //{
                //    count++;
                //}
                //方案二
                int count = listData / BufferSize;
                if ((listData % BufferSize) > 0)
                {
                    count++;
                }

                if (count > 255) throw new Exception("分包数超过 255 个包，请减少包体数据！");

                this.Many = new Range(0, ^count);

                if (IsObj == 3)
                {
                    ArraySegment<byte> newbytes = new byte[TcpStateObject.HeadSize + this.TextBytes.Count + Bytes.Count];
                    TcpStateObject.SetDataHead(newbytes, this.TextBytes.Count, 0);
                    this.TextBytes.CopyTo(newbytes[TcpStateObject.HeadSize..]);
                    this.Bytes.CopyTo(newbytes[(TcpStateObject.HeadSize + this.TextBytes.Count)..]);
                    this.Bytes = newbytes;

                    //ArraySegment<byte> objstr = SupportCode.TcpStateObject.GetDataSend(this.TextBytes, int.MaxValue);
                    //Array.Resize(ref objstr, objstr.Count + Bytes.Count);
                    //this.Bytes.CopyTo(objstr, objstr.Count - Bytes.Count);
                    //this.Bytes = objstr;

                    this.Text = null;
                }
                else if (IsObj == 1)
                {
                    ArraySegment<byte> newbytes = new byte[TcpStateObject.HeadSize + this.TextBytes.Count];
                    TcpStateObject.SetDataHead(newbytes, this.TextBytes.Count, 0);
                    this.TextBytes.CopyTo(newbytes[TcpStateObject.HeadSize..]);
                    this.Bytes = newbytes;

                    //this.Bytes = SupportCode.TcpStateObject.GetDataSend(this.TextBytes, int.MaxValue);
                    this.Text = null;
                }
            }
            else
            {
                EmptyMany();
            }
        }

        /**
         * 清空分包
         */
        internal void EmptyMany()
        {
            this.Many = Range.All;
        }

        /**
         * 将对象转为数据包
         */
        internal ArraySegment<byte> ByteData()
        {
            //**
            // * 固定体
            // * 固定头 123 代号 1 位
            // * 固定ID 32个 16 位
            // * 固定事件 2 位
            // * 固定大小 2 位
            // * 固定状态 6个 2位 IsSend，IsErr，IsServer，IsAsync，IsIpIdea，IsObj
            // * 
            // * 可变体
            // * 动态值 转发IP 4 位 + 端口 2位 合计6位
            // * 动态数据 大小未知
            // * 
            // * 最少位数：23~29
            // */

            int index = 0, origsize = IsIpIdea ? 29 : 23, size = origsize;
            ArraySegment<byte> bytes;//byte[]

            //byte[]
            switch (IsObj)
            {
                case 3:
                    size += TcpStateObject.HeadSize;
                    size += this.TextBytes.Count;
                    size += this.Bytes.Count;
                    bytes = new byte[size];

                    TcpStateObject.SetDataHead(bytes, this.TextBytes.Count, origsize);
                    this.TextBytes.CopyTo(bytes[(origsize + TcpStateObject.HeadSize)..]);
                    this.Bytes.CopyTo(bytes[(size - this.Bytes.Count)..]);

                    //byte[] objstr = SupportCode.TcpStateObject.GetDataSend(this.TextBytes, int.MaxValue);
                    //size += objstr.Length;
                    //size += this.Bytes.Count;
                    //bytes = new byte[size];
                    //objstr.CopyTo(bytes.Array, origsize);
                    //this.Bytes.CopyTo(bytes.Array, size - this.Bytes.Count);
                    break;
                case 2:
                    size += this.Bytes.Count;
                    bytes = new byte[size];
                    this.Bytes.CopyTo(bytes[origsize..]);
                    break;
                case 1:
                    size += this.TextBytes.Count;
                    bytes = new byte[size];
                    this.TextBytes.CopyTo(bytes[origsize..]);
                    break;
                default:
                    bytes = new byte[size];
                    break;
            }

            bytes[index++] = 123;
            //this.OnlyId.ToByteArray().CopyTo(bytes, index); //Array.Copy(OnlyId, 0, bytes, index, 16);
            this.OnlyId.TryWriteBytes(bytes[index..]);
            index += 16;
            bytes[index++] = ClassID;
            bytes[index++] = ActionID;
            bytes[index++] = (byte)this.Many.Start.Value;
            bytes[index++] = (byte)this.Many.End.Value;
            bytes[index++] = (byte)((this.IsSend ? 100 : 0) + (this.IsErr ? 10 : 0) + (this.IsServer ? 1 : 0)); // string.Concat(this.IsSend ? 1 : 0, this.IsErr ? 1 : 0, this.IsServer ? 1 : 0).ToVar<byte>();
            bytes[index++] = (byte)((this.IsAsync ? 100 : 0) + (this.IsIpIdea ? 10 : 0) + this.IsObj); // string.Concat(this.IsAsync ? 1 : 0, this.IsIpIdea ? 1 : 0, this.IsObj).ToVar<byte>();
            if (this.IsIpIdea)
            {
                //string[] Ips = this.IpPort.Split('.');

                //for (int i = 0; i < 4; i++)
                //{
                //    if (i == 3)
                //    {
                //        //UInt16 s = 65535;
                //        string[] iporport = Ips[i].Split(':');
                //        bytes[index++] = iporport[0].ToVar<byte>();
                //        byte[] ports = BitConverter.GetBytes(iporport[1].ToVar<ushort>());
                //        ports.CopyTo(bytes, index); //Array.Copy(ports, 0, bytes, index, 2);
                //        index += 2;
                //    }
                //    else
                //    {
                //        bytes[index++] = Ips[i].ToVar<byte>();
                //    }
                //}
                //System.Net.EndPoint s = new System.Net.DnsEndPoint(IpPort.Split(':')[0], IpPort.Split(':')[1].ToInt());


                System.Net.IPEndPoint s1 = System.Net.IPEndPoint.Parse(IpPort);

                //s1.Address.GetAddressBytes().CopyTo(bytes, index);

                s1.Address.TryWriteBytes(bytes[index..], out _);

                index += 4;
                //byte[] ports = BitConverter.GetBytes((ushort)s1.Port);
                //ports.CopyTo(bytes, index);

                BitConverter.TryWriteBytes(bytes[index..], (ushort)s1.Port);
                //index += 2;
            }

            ////byte[] 
            //switch (IsObj)
            //{
            //    case 3:
            //        byte[] objstr = SupportCode.TcpStateObject.GetDataSend(Encoding.UTF8.GetBytes(this.Obj), int.MaxValue);

            //        //Array.Resize(ref bytes, bytes.Length + objstr.Length + this.Bytes.Length);

            //        //Array.Copy(objstr, 0, bytes, index, objstr.Length);  //bytes.AddRange(objstr);
            //        objstr.CopyTo(bytes, index);
            //        index += objstr.Length;
            //        this.Bytes.CopyTo(bytes, index);//Array.Copy(this.Bytes, 0, bytes, index, this.Bytes.Length); //bytes.AddRange(this.Bytes);
            //        break;
            //    case 2:
            //        //Array.Resize(ref bytes, bytes.Length + this.Bytes.Length);
            //        this.Bytes.CopyTo(bytes, index); //Array.Copy(this.Bytes, 0, bytes, index, this.Bytes.Length); //bytes.AddRange(this.Bytes);
            //        break;
            //    case 1:
            //        objstr = Encoding.UTF8.GetBytes(this.Obj);
            //        //Array.Resize(ref bytes, bytes.Length + objstr.Length);
            //        objstr.CopyTo(bytes, index);//Array.Copy(objstr, 0, bytes, index, objstr.Length); //bytes.AddRange(Encoding.UTF8.GetBytes(this.Obj));
            //        break;
            //}

            //for (int r = 0; r < 1000000; r++)
            //{
            //    var ssss1 = DataByte(bytes);
            //}
            //List<byte> bytes = new(IsIpIdea ? 29 : 23);
            //bytes.Add(123);
            //bytes.AddRange(OnlyId);
            //bytes.Add(ClassID);
            //bytes.Add(ActionID);
            //bytes.Add(this.Many.Start.Value.ToVar<byte>());
            //bytes.Add(this.Many.End.Value.ToVar<byte>());
            //bytes.Add(string.Concat(this.IsSend ? 1 : 0, this.IsErr ? 1 : 0, this.IsServer ? 1 : 0).ToVar<byte>());
            //bytes.Add(string.Concat(this.IsAsync ? 1 : 0, this.IsIpIdea ? 1 : 0, this.IsObj).ToVar<byte>());

            //if (this.IsIpIdea)
            //{
            //    string[] Ips = this.IpPort.Split('.');

            //    for (int i = 0; i < 4; i++)
            //    {
            //        if (i == 3)
            //        {
            //            //UInt16 s = 65535;
            //            string[] iporport = Ips[i].Split(':');
            //            bytes.Add(iporport[0].ToVar<byte>());
            //            byte[] ports = BitConverter.GetBytes(iporport[1].ToVar<ushort>());
            //            bytes.AddRange(ports);
            //        }
            //        else
            //        {
            //            bytes.Add(Ips[i].ToVar<byte>());
            //        }
            //    }
            //}

            //switch (IsObj)
            //{
            //    case 3:
            //        byte[] objstr = SupportCode.TcpStateObject.GetDataSend(Encoding.UTF8.GetBytes(this.Obj), int.MaxValue);
            //        bytes.AddRange(objstr);
            //        bytes.AddRange(this.Bytes);
            //        break;
            //    case 2:
            //        bytes.AddRange(this.Bytes);
            //        break;
            //    case 1:
            //        bytes.AddRange(Encoding.UTF8.GetBytes(this.Obj));
            //        break;
            //}

            //var ssss = DataByte(bytes.ToArray());

            //string _str = string.Empty; //string.Concat("[#", this.OnlyID, '&', this.IsSend ? '1' : '0', this.IsErr ? '1' : '0', this.IsServer ? '1' : '0', this.IsAsync ? '1' : '0', this.IsIpIdea ? '1' : '0', '&', this.Many, '@', "#]");

            //for (int i = 0; i < 1000000; i++)
            //{
            //    //_str = string.Concat("[#", this.OnlyID, '&', this.IsSend ? '1' : '0', this.IsErr ? '1' : '0', this.IsServer ? '1' : '0', this.IsAsync ? '1' : '0', this.IsIpIdea ? '1' : '0', '&', this.Many, '@', "#]");

            //    //byte[] by2 = Encoding.UTF8.GetBytes(_str);

            //    _str = StringData();

            //    var ssss = DataString(_str);
            //}

            //for (int i = 0; i < 1000000; i++)
            //{
            //    var ssss = DataString(_str);
            //}

            ////for (int i = 0; i < 1000000; i++)
            ////{
            ////    _str = string.Concat("[#", this.OnlyID, '&', this.IsSend ? '1' : '0', this.IsErr ? '1' : '0', this.IsServer ? '1' : '0', this.IsAsync ? '1' : '0', this.IsIpIdea ? '1' : '0', '&', this.Many, '@', "#]");

            ////    byte[] by = CoreCode.GetBytes(_str);
            ////}

            ////string data = CoreCode.GetStrings(by);

            //int length = this.Obj == null ? 100 : this.Obj.Length + 100;
            //byte[] by1 = Encoding.UTF8.GetBytes(Obj);
            //byte[] Data = SupportCode.TcpStateObject.GetDataSend(by1, 10000);

            //char random = this.OnlyID[4];
            //StringBuilder str = new StringBuilder("[#", length);//{}
            //str.Append(random).Append('#').Append(this.OnlyID).Append('&');
            //str.Append(this.IsSend ? 1 : 0).Append(this.IsErr ? 1 : 0).Append(this.IsServer ? 1 : 0).Append(this.IsAsync ? 1 : 0).Append(this.IsIpIdea ? 1 : 0);
            //str.Append('&').Append(this.Many).Append('@');

            ////const string s = "";

            ////for (int i = 0; i < 10000; i++)
            ////{
            ////CoreCode.GetBytes(str.ToString());
            ////str.ToString();

            ////StringBuilder str1 = new StringBuilder("[#", length);//{}
            ////str1.Append(random).Append('#').Append(this.OnlyID).Append('&');
            ////str1.Append(this.IsSend ? 1 : 0).Append(this.IsErr ? 1 : 0).Append(this.IsServer ? 1 : 0).Append(this.IsAsync ? 1 : 0).Append(this.IsIpIdea ? 1 : 0);
            ////str1.Append('&').Append(this.Many).Append('@');

            ////string _str = string.Concat("[#", random, '#', this.OnlyID, '&', this.IsSend ? '1' : '0', this.IsErr ? '1' : '0', this.IsServer ? '1' : '0', this.IsAsync ? '1' : '0', this.IsIpIdea ? '1' : '0', '&', this.Many, '@');

            ////byte[] by = CoreCode.GetBytes(_str);

            ////string _str1 = CoreCode.GetStrings(by);
            ////}

            //if (this.Obj == null)
            //{
            //    str.Append("null");
            //}
            //else
            //{
            //    if (this.IsIpIdea)
            //    {
            //        str.Append(string.Concat('[', IpPort, ']', this.Obj));
            //    }
            //    else
            //    {
            //        str.Append(this.Obj);
            //    }
            //}
            //str.Append('@').Append(random).Append("#]");

            ////str.Insert(1, str.Length);

            //string json = str.ToString();
            //str.Clear();

            if (bytes.Count > this.BufferSize)
            {
                throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{bytes.Count},本该最大大小{this.BufferSize}。）");
            }
            return bytes;
        }

        /**
         * 将字节流转为原对象
         */
        internal static DataPacket DataByte(Span<byte> bytes)//byte[]
        {
            // { OnlyId = new byte[16] }
            //unsafe
            //{
            //    //int length = bytes.Length;

            //    fixed (byte* ps = bytes)
            //    {
            //        int i = 1;
            //        Array.Copy(bytes, 1, packet.OnlyId, 0, 16);
            //        packet.ClassID = ps[17];
            //        packet.ActionID = ps[18];
            //        packet.Many = new Range(ps[19], ps[20]);

            //        byte is_1 = ps[21], is_2 = ps[22];
            //         i += 22;
            //        if (is_1 > 99)
            //        {
            //            is_1 -= 100;
            //            packet.IsSend = true;
            //        }
            //        if (is_1 > 9)
            //        {
            //            is_1 -= 10;
            //            packet.IsErr = true;
            //        }
            //        if (is_1 > 0)
            //        {
            //            packet.IsServer = true;
            //        }

            //        if (is_2 > 99)
            //        {
            //            is_2 -= 100;
            //            packet.IsAsync = true;
            //        }
            //        if (is_2 > 9)
            //        {
            //            is_2 -= 10;
            //            packet.IpPort = string.Concat(ps[23], '.', ps[24], '.', ps[25], '.', ps[26], ':', BitConverter.ToUInt16(bytes, 27));
            //            i += 6;
            //        }

            //        switch (is_2)
            //        {
            //            case 3:
            //                byte[] head = new byte[6];
            //                Array.Copy(bytes, i, head, 0, 6);
            //                int length = SupportCode.TcpStateObject.GetDataHead(head);
            //                i += 6;

            //                packet.Obj = Encoding.UTF8.GetString(bytes, i, length);
            //                i += length;

            //                packet.Bytes = new byte[bytes.Length - i];
            //                Array.Copy(bytes, i, packet.Bytes, 0, packet.Bytes.Length);
            //                break;
            //            case 2:
            //                packet.Bytes = new byte[bytes.Length - i];
            //                Array.Copy(bytes, i, packet.Bytes, 0, packet.Bytes.Length);
            //                break;
            //            case 1:
            //                packet.Obj = Encoding.UTF8.GetString(bytes, i, bytes.Length - i);
            //                break;
            //        }

            //    }

            //string is_1 = ps[21].ToString(), is_2 = ps[22].ToString();
            //int i = 23;
            //packet.IsSend = is_1[0] == '1';
            //packet.IsErr = is_1[1] == '1';
            //packet.IsServer = is_1[2] == '1';

            //packet.IsAsync = is_2[0] == '1';

            //if (is_2[1] == '1')
            //{
            //    packet.IpPort = string.Concat(ps[23], '.', ps[24], '.', ps[25], '.', ps[26], ':', BitConverter.ToUInt16(bytes, 27));
            //    i += 6;
            //}

            //switch (is_2[2])
            //{
            //    case '3':
            //        byte[] head = new byte[6];
            //        Array.Copy(bytes, i, head, 0, 6);
            //        int length = SupportCode.TcpStateObject.GetDataHead(head);
            //        i += 6;

            //        packet.Obj = Encoding.UTF8.GetString(bytes, i, length);
            //        i += length;

            //        packet.Bytes = new byte[bytes.Length - i];
            //        Array.Copy(bytes, i, packet.Bytes, 0, packet.Bytes.Length);
            //        break;
            //    case '2':
            //        packet.Bytes = new byte[bytes.Length - i];
            //        Array.Copy(bytes, i, packet.Bytes, 0, packet.Bytes.Length);
            //        break;
            //    case '1':
            //        packet.Obj = Encoding.UTF8.GetString(bytes, i, bytes.Length - i);
            //        break;
            //}

            //}
            int i = 1;
            //Array.Copy(bytes, 1, packet.OnlyId, 0, 16);
            DataPacket packet = new()
            {
                OnlyId = new Guid(bytes.Slice(i, 16)), //bytes.AsMemory(i, 16).ToArray(); //ReadOnlySpan
                ClassID = bytes[17],
                ActionID = bytes[18],
                Many = new Range(bytes[19], ^bytes[20])
            };
            byte is_1 = bytes[21], is_2 = bytes[22];
            i += 22;
            if (is_1 > 99)
            {
                is_1 -= 100;
                packet.IsSend = true;
            }
            if (is_1 > 9)
            {
                is_1 -= 10;
                packet.IsErr = true;
            }
            if (is_1 > 0)
            {
                packet.IsServer = true;
            }

            if (is_2 > 99)
            {
                is_2 -= 100;
                packet.IsAsync = true;
            }
            if (is_2 > 9)
            {
                is_2 -= 10;

                packet.IpPort = $"{bytes[23]}.{bytes[24]}.{bytes[25]}.{bytes[26]}:{BitConverter.ToUInt16(bytes.Slice(27, 2))}";

                //packet.IpPort = new System.Net.IPEndPoint(new System.Net.IPAddress(bytes.Slice(i, 4)), BitConverter.ToUInt16(bytes.Slice(i + 4, 2))).ToString();//bytes, 27
                //packet.IpPort = string.Concat(bytes[23], '.', bytes[24], '.', bytes[25], '.', bytes[26], ':', BitConverter.ToUInt16(bytes, 27));
                i += TcpStateObject.HeadSize;
            }

            switch (is_2)
            {
                case 3:
                    var head = bytes.Slice(i, TcpStateObject.HeadSize); //bytes.AsMemory(i, 6).ToArray(); //new byte[6];
                    //Array.Copy(bytes, i, head, 0, 6);
                    int length = TcpStateObject.GetDataHead(head);
                    i += TcpStateObject.HeadSize;

                    //packet.Text = Encoding.UTF8.GetString(bytes.Slice(i, length));//(bytes.ToArray(), i, length);
                    packet.TextBytes = bytes.Slice(i, length).ToArray();
                    i += length;

                    packet.Bytes = bytes[i..].ToArray(); //new byte[bytes.Length - i];
                    //Array.Copy(bytes.ToArray(), i, packet.Bytes, 0, packet.Bytes.Length);
                    break;
                case 2:
                    packet.Bytes = bytes[i..].ToArray(); //new byte[bytes.Length - i];
                    //Array.Copy(bytes.ToArray(), i, packet.Bytes, 0, packet.Bytes.Length);
                    break;
                case 1:
                    //packet.Text = Encoding.UTF8.GetString(bytes[i..]); //(bytes.ToArray(), i, bytes.Length - i);
                    packet.TextBytes = bytes[i..].ToArray();
                    break;
            }
            return packet;
        }

        #region 原来的旧处理算法-已废弃

        //**
        // * 将对象转为数据包
        // */
        //internal string StringData()
        //{
        //    int length = this.Obj == null ? 100 : this.Obj.Length + 100;

        //    char random = this.OnlyID[4];
        //    StringBuilder str = new StringBuilder("[#", length);//{}
        //    str.Append(random).Append('#').Append(this.OnlyID).Append('&');
        //    str.Append(this.IsSend ? 1 : 0).Append(this.IsErr ? 1 : 0).Append(this.IsServer ? 1 : 0).Append(this.IsAsync ? 1 : 0).Append(this.IsIpIdea ? 1 : 0);
        //    str.Append('&').Append("0/0"/*this.Many*/).Append('@');

        //    //const string s = "";

        //    //for (int i = 0; i < 10000; i++)
        //    //{
        //    //CoreCode.GetBytes(str.ToString());
        //    //str.ToString();

        //    //StringBuilder str1 = new StringBuilder("[#", length);//{}
        //    //str1.Append(random).Append('#').Append(this.OnlyID).Append('&');
        //    //str1.Append(this.IsSend ? 1 : 0).Append(this.IsErr ? 1 : 0).Append(this.IsServer ? 1 : 0).Append(this.IsAsync ? 1 : 0).Append(this.IsIpIdea ? 1 : 0);
        //    //str1.Append('&').Append(this.Many).Append('@');

        //    //string _str = string.Concat("[#", random, '#', this.OnlyID, '&', this.IsSend ? '1' : '0', this.IsErr ? '1' : '0', this.IsServer ? '1' : '0', this.IsAsync ? '1' : '0', this.IsIpIdea ? '1' : '0', '&', this.Many, '@');

        //    //byte[] by = CoreCode.GetBytes(_str);

        //    //string _str1 = CoreCode.GetStrings(by);
        //    //}

        //    if (this.Obj == null)
        //    {
        //        str.Append("null");
        //    }
        //    else
        //    {
        //        if (this.IsIpIdea)
        //        {
        //            str.Append(string.Concat('[', IpPort, ']', this.Obj));
        //        }
        //        else
        //        {
        //            str.Append(this.Obj);
        //        }
        //    }
        //    str.Append('@').Append(random).Append("#]");

        //    //str.Insert(1, str.Length);

        //    string json = str.ToString();
        //    str.Clear();
        //    return json;
        //}

        //**
        // * 将包字符串转成原数据包
        // * str 数据包
        // */
        //internal static DataPacket DataString(string str)
        //{
        //    DataPacket packet = new DataPacket();
        //    int i = 4;
        //    unsafe
        //    {
        //        int length = str.Length;

        //        fixed (char* ps = str)
        //        {
        //            while (ps[i] != '&') i++;

        //            packet.OnlyID = str.Substring(4, (++i - 5));

        //            packet.IsSend = ps[i++] == '1' ? true : false;
        //            packet.IsErr = ps[i++] == '1' ? true : false;
        //            packet.IsServer = ps[i++] == '1' ? true : false;
        //            packet.IsAsync = ps[i++] == '1' ? true : false;
        //            //packet.IsIpIdea = ps[i++] == '1' ? true : false;

        //            int index = ++i;
        //            while (ps[i] != '@') i++;
        //            packet.Many = new Range();//str.Substring(index, i - index); ;

        //            string data = str.Substring(++i, length - (i + 4));

        //            if (!(data.Length == 4 && data == "null"))
        //            {
        //                if (packet.IsIpIdea)
        //                {
        //                    string _ip = null;
        //                    if (packet.IsIpIdea)
        //                    {
        //                        _ip = DataTcp.GetIsIpIdea(out int startIndex, data);
        //                        if (_ip != null)
        //                        {
        //                            packet.Obj = data.Substring(startIndex + 1);
        //                        }
        //                    }
        //                    packet.IpPort = _ip;
        //                }
        //                else
        //                {
        //                    packet.Obj = data;
        //                }
        //            }
        //        }
        //    }

        //    //int i;
        //    //for (i = 4; i < 100; i++)
        //    //{
        //    //    if (str[i] == '&')
        //    //    {
        //    //        packet.OnlyID = str.Substring(4, i - 4);
        //    //        break;
        //    //    }
        //    //    //packet.OnlyID += str[i];
        //    //}

        //    //packet.IsSend = str[i++] == '1' ? true : false;
        //    //packet.IsErr = str[i++] == '1' ? true : false;
        //    //packet.IsServer = str[i++] == '1' ? true : false;
        //    //packet.IsAsync = str[i++] == '1' ? true : false;

        //    //packet.Many = "";
        //    //for (i++; i < 100; i++)
        //    //{
        //    //    if (str[i] == '@')
        //    //    {
        //    //        break;
        //    //    }
        //    //    packet.Many += str[i];
        //    //}

        //    //result.Remove(0, ++i);

        //    //result.Remove(result.Length - 4, 4);

        //    //packet.Obj = str.Substring(++i, str.Length - (i + 4)); //result.ToString();

        //    return packet;
        //}

        #endregion

        /**
         * 返回指定的部分数据包
         */
        private void GetCount(ArraySegment<byte> bytes, int index, int count, int BufferSize)//ref byte[] internal
        {
            BufferSize -= IsIpIdea ? 29 : 23;

            if (index == count - 1)
            {
                this.Bytes = bytes.Slice(index * BufferSize);//, bytes.Length - index * BufferSize
            }
            else
            {
                this.Bytes = bytes.Slice(index * BufferSize, BufferSize);
            }

            //this.Many = string.Concat(index, '/', count);
            if (index > 0)
                this.Many = new Range(index, ^count);

            //string Part;
            //if (index == count - 1)
            //{
            //    Part = Encoding.UTF8.GetString(bytes, index * BufferSize, bytes.Length - index * BufferSize);
            //}
            //else
            //{
            //    Part = Encoding.UTF8.GetString(bytes, index * BufferSize, BufferSize);
            //}

            //this.Obj = Part;

            //StringBuilder stringBuilder = new StringBuilder(Part);

            //stringBuilder.Length - this.Obj.Length

            //if (stringBuilder[0] == '\"')
            //{
            //    stringBuilder.Remove(0, 1);
            //}
            //if (stringBuilder[stringBuilder.Length - 1] == '\\')
            //{
            //    stringBuilder.Append('\"');
            //}

            //string ds = bianma(Part);
            //byte[] bytes1 = Encoding.UTF8.GetBytes(Part);
            //string Part1 = BitConverter.ToString(bytes, index * BufferSize, BufferSize);
            //byte[] bytes2 = Encoding.UTF8.GetBytes(Part1);

            //DataPacket packet = new DataPacket()
            //{
            //    IsAsync = this.IsAsync,
            //    IsErr = this.IsErr,
            //    IsSend = this.IsSend,
            //    IsServer = this.IsServer,
            //    OnlyID = this.OnlyID,
            //    Many = $"{index}/{count}",
            //    Obj = Part // stringBuilder.ToString()
            //};
        }

        internal ArraySegment<byte>[] ByteDatas()
        {
            int count = this.Many.End.Value;// Substring(2).ToInt();
            ArraySegment<byte>[] buffers = new ArraySegment<byte>[count];
            ArraySegment<byte> bytes = this.Bytes;//Encoding.UTF8.GetBytes(dataPacket.Obj);// as byte[];string strobj = dataPacket.Obj;
            for (int i = 0; i < count; i++)
            {
                this.GetCount(bytes, i, count, this.BufferSize);
                //string Json = dataPacket.StringData(); //data.Json();
                ArraySegment<byte> listData = this.ByteData();//Encoding.UTF8.GetBytes(Json);
                if (listData.Count > this.BufferSize)
                {
                    throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Count},本该最大大小{this.BufferSize}。）");
                }
                buffers[i] = listData;
            }

            return buffers;
        }

        public void Dispose()
        {
            if (this.ClassID > 0)
            {
                // 请将清理代码放入下面
                //this.ClassID = 0;
                //this.ActionID = 0;
                //this.OnlyId = Guid.Empty;
                this.Many = Range.All;
                this.IpPort = null;
                this.TextBytes = null;
                this.Bytes = null;
                this.BufferSize = 0;
                //GC.SuppressFinalize(this);
            }
        }

        ///// <summary>
        ///// 将字符串转成二进制
        ///// </summary>
        ///// <param name="s"></param>
        ///// <returns></returns>
        //public static string bianma(string s)
        //{
        //    //BitConverter.ToString(s);
        //       byte[] data = Encoding.Unicode.GetBytes(s);
        //    StringBuilder result = new StringBuilder(data.Length * 8);

        //    foreach (byte b in data)
        //    {
        //        result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        //    }
        //    return result.ToString();
        //}

        ///// <summary>
        ///// 将字符串转成二进制
        ///// </summary>
        ///// <param name="s"></param>
        ///// <returns></returns>
        //public static string StringHex(string str)
        //{
        //    StringBuilder result = new StringBuilder(str.Length * 3);
        //    byte[] bytes = Encoding.UTF8.GetBytes(str);

        //    byte[] bytes1 = Convert.FromBase64CharArray(str.ToCharArray(), 0, str.Length);


        //    unsafe
        //    {
        //        //int length = str.Length; 

        //        //int d = sizeof(byte);
        //        //fixed (char* ps = str)
        //        //{
        //        //    //char d = ps[0];
        //        //    for (int i = 0; i < length; i++)
        //        //    {
        //        //        int hex8 = (int)ps[i];

        //        //        //string hex16 = Convert.ToString(hex8, 16);

        //        //        result.Append(hex8);
        //        //    }
        //        //}
        //        fixed (byte* by = bytes)
        //        {
        //            int length = bytes.Length;
        //            for (int i = 0; i < length; i++)
        //            {
        //                byte hex8 = by[i];

        //                //string hex16 = Convert.ToString(hex8, 16);

        //                result.Append("5b");
        //            }
        //        }
        //    }

        //    //BitConverter.ToString(s);
        //    byte[] data = Encoding.UTF8.GetBytes(str);


        //    foreach (byte b in data)
        //    {
        //        result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        //    }
        //    return result.ToString();
        //}
    }
}
