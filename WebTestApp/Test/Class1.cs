using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tool;
using Tool.Sockets.NetFrame;

namespace WebTestApp.Test
{
    public class Class1 : IApiResult
    {
        public Dictionary<string, ApiValue> Keys { get; } = new Dictionary<string, ApiValue>();
        public Stream Stream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void sd() 
        {
            int aa = (ApiValue)1235;

            Keys.Clear();

            Keys.Add("12340", (byte)1);

            Keys.Add("12341", "123456");

            Keys.Add("12342", DateTime.Now);

            Keys.Add("12343", int.MaxValue);

            Keys.Add("12344", long.MaxValue);

            Keys.Add("12345", decimal.MaxValue);

            Keys.Add("12346", double.MaxValue);

            Keys.Add("12347", new List<ApiValue> { (byte)1, (byte)2, (byte)3, (byte)4, (byte)5 });

            Keys.Add("12348", new Dictionary<string, ApiValue> { { "a", 5000 }, { "b", "呵呵" }, { "c", DateTime.Now } });

            Keys.Add("12349", DateTime.Now);

            DateTime time = Keys["12349"];
        }
    }
}
