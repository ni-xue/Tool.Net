using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Tool;
using Tool.Utils;
using Tool.Web.Api;
using WebTestApp.Test;

namespace WebTestApp.ApiView
{
    public class Class : MinApi
    {
        [CrossDomain(Origin = "*", Headers = "content-Type")]
        [Ashx(State = AshxState.Get)]
        public IApiOut As() { return ApiOut.Json(new { a = 5 }); }

        public IApiOut Api([ApiVal(Val.Body)] Memory<byte> stream)
        {
            return ApiOut.Json(new { a = 5 });
        }

        public IApiOut Api0([ApiVal(Val.Body)] Stream stream)
        {
            return ApiOut.Json(new { a = 5 });
        }

        public IApiOut Api1([ApiVal(Val.Body)] PipeReader writer)
        {
            return ApiOut.Json(new { a = 5 });
        }

        public IApiOut Api2([ApiVal(Val.BodyJson)] JsonVar keys)
        {
            return ApiOut.Json(keys.Data);
        }

        public IApiOut Api3([ApiVal(Val.BodyJson)] List<SystemTest> test)
        {
            return ApiOut.Json(new { a = 5 });
        }

        public IApiOut Api4([ApiVal(Val.BodyString)] string json)
        {
            //List<SystemTest> list = [];
            //for (int i = 0; i < 1000; i++)
            //{
            //    list.Add(new() { id = i, key_cn = StringExtension.GetGuid(), key_en = StringExtension.GetGuid(), value = StringExtension.GetGuid() });
            //}
            return ApiOut.Json(json);
        }
    }
}
