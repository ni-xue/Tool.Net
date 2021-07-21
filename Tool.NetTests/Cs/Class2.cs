using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils.ActionDelegate;

namespace Tool.NetTests.Cs
{
    [TestClass()]
    public class Class2
    {
        readonly byte[] ListData;
        readonly Memory<byte> arraySegment;

        public Class2() 
        {
            ListData = new byte[] { 1,2,3,4,3,2,1 };

            arraySegment = ListData;
        }

        [TestMethod()]
        public void Cs0()
        {
            for (int i = 0; i < 10000000; i++)
            {
                byte[] headby = new byte[6];

                Array.Copy(ListData, 0, headby, 0, 6);
            }
        }

        [TestMethod()]
        public void Cs1()
        {
            for (int i = 0; i < 10000000; i++)
            {
                //byte[] headby = new byte[6];
                byte[] headby = ListData.AsSpan().Slice(0, 6).ToArray();//.CopyTo(headby);
            }
        }
    }
}
