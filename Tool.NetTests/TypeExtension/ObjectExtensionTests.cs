using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using Tool.Web.Api;

namespace Tool.Tests
{
    [TestClass()]
    public class ObjectExtensionTests
    {
        [TestMethod()]
        public void ToJsonActionTest()//[ApiVal(Val.Query)] int id, [ApiVal(Val.Form)] int id1
        {
            string text = string.Concat("{\"code\":", 200, ",\"msg\":\"", "哈哈哈", "\"}");

            int sd1 = "你是傻逼！".ToTryVar(10);

            decimal sd2 = "你是傻逼！".ToTryVar(10.00m);

            double sd3 = "你是傻逼！".ToTryVar(10.00);

            DateTime sd4 = "你是傻逼！".ToTryVar(DateTime.Now);

            int sd = "1000000000".ToTryVar(10);

            (1000).ToTryVar<long>(10);

            (1000).ToTryVar<double>(10);

            (1000).ToTryVar<decimal>(10);

            (1000).ToTryVar<string>("10");

            Exception ex = new AggregateException();

            ex.ToTryVar<ApplicationException>(null);

            Console.WriteLine(sd);
            Console.WriteLine(sd1);

            //Test1 test1 = new Test1 { Data = new byte[] { 10, 20 }, Data1 = new int[] { 30, 40 }, I = 50 };

            //string json = test1.ToJsonWeb(s => {/* s.Converters.Add(new WebJsonFormat());*/ });


            //Test1 test2 = json.Json<Test1>(new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));

            //string md50 = json.MD5Upper(false);
            //string md51 = Utils.Encryption.TextEncrypt.MD5EncryptPassword(json, Utils.Encryption.MD5ResultMode.Weak);

            //Console.WriteLine(md50);
            //Console.WriteLine(md51);
        }

        public class WebJsonFormat : JsonConverter<byte[]>
        {
            //public override bool CanConvert(Type typeToConvert)
            //{
            //    return true;
            //}

            public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                //HandleNull
                //reader
                string bytes = reader.GetString();
                bytes = bytes[1..^1];
                string[] vs = bytes.Split(',');
                byte[] _bytes = new byte[vs.Length];
                for (int i = 0; i < vs.Length; i++)
                {
                    _bytes[i] = vs[i].ToVar<byte>();
                }
                return _bytes;
            }

            public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
            {
                StringBuilder stringBuilder = new StringBuilder("[");
                stringBuilder.AppendJoin(',', value).Append(']');
                writer.WriteStringValue(stringBuilder.ToString());

                //throw new NotImplementedException();
            }
        }

        public class Test1 
        {
            [JsonConverter(typeof(WebJsonFormat))]
            public byte[] Data { get; set; }

            public int[] Data1 { get; set; }

            //[JsonConverter(typeof(WebJsonFormat))]
            public int I { get; set; }

            //[JsonConverter(typeof(System.ComponentModel.DateTimeOffsetConverter))]
            public DateTime Date { get; set; } = DateTime.Now;
        }
    }
}