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
        [DataTcp(1)]
        public Class1()
        {

        }

        protected override bool Initialize(DataTcp dataTcp)
        {
            return true;
        }

        [DataTcp(100)]
        public IGoOut A(int a) 
        {
            return Json(new { a });
        }

        [DataTcp(101)]
        public IGoOut B(string path)
        {
            byte[] s = System.IO.File.ReadAllBytes(path);
            return Write(s);
        }

        [DataTcp(102)]
        public IGoOut C(string path)
        {
            //System.IO.File.WriteAllBytes(path, Bytes);
            return Write("保存成功！"); //Ok("保存成功！", Bytes); //new byte[] { 0,1,2,3,4,5 }
        }

        [DataTcp(103)]
        public IGoOut D(string path)
        {
            return Write("保存成功！");
        }

    }
}
