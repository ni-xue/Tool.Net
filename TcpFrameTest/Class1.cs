using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.TcpFrame;

namespace TcpFrameTest
{
    public class Class1 : DataBase
    {
        public Class1() : base(1)
        {
        }

        [DataTcp(100, ObjType = DataTcpState.String)]
        public object A(int a) 
        {
            return a;// new { a };
        }

        [DataTcp(101, ObjType = DataTcpState.Byte)]
        public byte[] B(string path)
        {
            byte[] s = System.IO.File.ReadAllBytes(path);

            return s;//new byte[1024 * 100];
        }

        [DataTcp(102, ObjType = DataTcpState.String)]
        public string C(string path)
        {
            //System.IO.File.WriteAllBytes(path, Bytes);//23797

            return "保存成功！";//new byte[1024 * 100];
        }

    }
}
