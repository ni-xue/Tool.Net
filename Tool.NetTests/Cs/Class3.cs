using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.NetTests.Cs
{
    [TestClass()]
    public class Class3
    {
        private readonly string ipv4 = "120.25.1.0:65535";
        public Class3()
        {
            //ipv4 = "192.1.1.135:46517";
        }

        [TestMethod("IPEndPoint")]
        public void Cs0()
        {
            for (int i = 0; i < 1000000; i++)
            {
                ReadOnlySpan<char> chars = ipv4.AsSpan();
                if (chars.Contains('.') && IPEndPoint.TryParse(chars, out var result) && result.AddressFamily is AddressFamily.InterNetwork)
                {
                    Memory<byte> bytes = new byte[6];
                    Span<byte> span = bytes.Span;
                    result.Address.TryWriteBytes(span, out var bytesWritten);
                    BitConverter.TryWriteBytes(span[bytesWritten..], (ushort)result.Port);
                    //Ipv4Port ipv4Port = new(in bytes);
                }
            }
        }

        //[TestMethod("UserKey")]
        //public void Cs1()
        //{
        //    for (int i = 0; i < 1000000; i++)
        //    {
        //        //string b64 = Convert.ToBase64String(ipv4.Span);
        //        //Console.WriteLine(ipv4.ToString().GetHashCode());
        //        UserKey userKey = "120.25.1.0:65535";
        //    }
        //}

        [TestMethod("Ipv4Port")]
        public void Cs2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                //Console.WriteLine(ipv4.GetHashCode());
                Ipv4Port ipv4Port = ipv4;
                //StateObject.IsIpPort("120.25.1.0:65535", out var ipnum);
            }
        }
    }
}
