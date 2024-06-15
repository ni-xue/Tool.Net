using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tool;
using Tool.Utils;
using Tool.Web.Api;
using Tool.Web;
using Tool.Utils.Data;

namespace WebTestApp.Api
{
    public class GetCore : ApiAshx
    {
        //public HttpClient httpClient { get; set; }

        private readonly Tool.SqlCore.DbHelper dbHelper;

        public GetCore(Tool.SqlCore.DbHelper dbHelper)//HttpClient httpClient
        {
            this.dbHelper = dbHelper;

            //this.httpClient = httpClient;
            //var configuration = AppSettings.Configuration;

            //var v = AppSettings.GetSection("Logging:LogLevel:Default");

            //var v1 = configuration["Sqls:WebSql"];
        }

        protected override bool Initialize(Ashx ashx)
        {
            //var s = ED();

            //var a1 = s.A1;

            //var a2 = s.A2;

            //Session.Set("key10", "123456");

            //ps sd = new ps() { key = 1050 };
            //Session.Set("key11", sd);

            //Context
            //Json(new { m = "无需结果" });
            return true;
        }

        protected override void OnResult(Ashx ashx)
        {

        }

        //public (int A1, string A2) ED() 
        //{
        //    return (1, "1");
        //}

        public object System(object dataSet)
        {
            return (dataSet as System.Data.DataSet).ToJSON();
        }

        [Ashx(State = AshxState.All), CrossDomain]
        public void Get(
            [ApiVal(Val.Query)] int Id
            , [ApiVal(Val.Form)] string Key
            , [ApiVal(Val.Service)] Tool.SqlCore.DbHelper dbHelper
            , [ApiVal(Val.File)] IFormFile file
            , [ApiVal(Val.Files)] IFormFileCollection files
            //,[ApiVal(Val.Session)] int key10
            //,[ApiVal(Val.Session)] ps key11
            , [ApiVal(Val.RouteKey)] int id1
            , [ApiVal(Val.RouteKey)] string controller
            , [ApiVal(Val.RouteKey)] string action

            , [ApiVal(Val.Service)] Microsoft.Extensions.Logging.ILoggerFactory factory

            , [ApiVal(Val.Cookie)] int SessionId
            , [ApiVal(Val.Header)] string Cookie1

            )
        {

            //Session.TryGetValue("key", out string v);
            //Session.TryGetValue("key1", out ps v1);

            Json(new { i = 5, mag = "", cos = 15000 });
        }

        [CrossDomain]
        public async Task Async([ApiVal(Val.Service)] IHttpContextAccessor context)
        {
            //"asd".ToInt();

            await JsonAsync(new { i = 10 });


            //await Task.Run(() => 
            //{
            //    "asd".ToInt();
            //    //"asd".ToInt();
            //    Json(new { i = 10 });
            //});
        }
        public async ValueTask Value()
        {
            await Task.Delay(1000);
            await JsonAsync(new { mag = "保存成功！" });
        }

        public Task Async1(int id = 5)
        {
            //"sda".ToInt();

            //ps s = "{\"msg\":\"暂无数据。\",\"time\":\"2021-05-04 15:41:00.407\",\"isTask\":false}".Json<ps>(RouteData.GetNewJsonOptions());

            return JsonAsync(new { msg = "暂无数据。", time = DateTime.Now, IsTask = false }); // Task.Run(() => { Json(new { msg = "暂无数据。", IsTask = false }); });
        }

        public void Ok(int id = 5)
        {
            //"sda".ToInt();

            //ps s = "{\"msg\":\"暂无数据。\",\"time\":\"2021-05-04 15:41:00.407\",\"isTask\":false}".Json<ps>(RouteData.GetNewJsonOptions());

            Json(new { msg = "暂无数据。", time = DateTime.Now, IsTask = false }); // Task.Run(() => { Json(new { msg = "暂无数据。", IsTask = false }); });
        }

        protected override void AshxException(AshxException ex)
        {
            ex.ExceptionHandled = true;

            Log.Debug("测试", ex);

            Json(new { msg = "系统错误。" });
        }
    }

    public class Ps
    {
        public string msg { set; get; }

        public DateTime time { set; get; }

        public bool isTask { set; get; }
    }
}
