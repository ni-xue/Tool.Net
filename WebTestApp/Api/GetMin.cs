using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool;
using Tool.SqlCore;
using Tool.Utils;
using Tool.Utils.Data;
using Tool.Web;
using Tool.Web.Api;
using Tool.Web.Routing;

namespace WebTestApp.Api
{
    public class GetMin : MinApi
    {
        int count = -10000;

        private DbHelper dbHelper; 

        public GetMin()
        {
            //ProviderFactory.AddFactory<MySql.Data.MySqlClient.MySqlClientFactory>(DbProviderType.MySql);
            //dbHelper = new DbHelper("data source=47.94.109.199;database=liquortribe;user id=liquortribe;password=NjCHBrzhrWpJZr8a;pooling=true;charset=utf8;", DbProviderType.MySql, new MySqlProvider());
            count = 0;
        }

        protected override IApiOut Initialize(AshxRouteData ashxRoute)
        {
            if (dbHelper == null)
            {
                dbHelper = ashxRoute.HttpContext.GetService<DbHelper>();
                ILoggerFactory loggerFactory = ashxRoute.HttpContext.GetService<ILoggerFactory>();//ILogger loggerFactory1 = context.GetService<ILogger<Share>>();
                dbHelper.SetLogger(loggerFactory.CreateLogger("Sql"));
            }
            count++;
            return null;
        }

        protected override IApiOut AshxException(AshxException ex)
        {
            Log.Debug("GetMin", ex);
            return base.AshxException(ex);

            //return ApiOut.Json(new { msg = "系统错误。", count });

            Tool.SqlCore.DbHelper dbHelper;

            //System(1, null);

            //dbHelper.Select(System, 1);

            Test.TSqlAttribute sqlAttribute = new Test.SelectAttribute("SELECT")
            {
                OnSqlAction = System
            };

            sqlAttribute.OnStart(dbHelper);
        }

        //[Test.TSql]
        [Test.Select("SELECT * FROM system WHERE Id=@a OrDER by id desc")]//"SELECT * FROM system WHERE Id=@a", ref int a
        public object System(object dataSet) 
        {
            return (dataSet as System.Data.DataSet).ToJSON();
        }


        public JsonOut GetSql(
            HttpContext context
            , [ApiVal(Val.Query)] int Id
            , [ApiVal(Val.Form)] string Key
            , [ApiVal(Val.Service)] Tool.SqlCore.DbHelper dbHelper
            , [ApiVal(Val.File)] IFormFile file
            //,[ApiVal(Val.Session)] int key10
            //,[ApiVal(Val.Session)] ps key11
            , [ApiVal(Val.RouteKey)] int id1
            , [ApiVal(Val.RouteKey)] string controller
            , [ApiVal(Val.RouteKey)] string action

            , [ApiVal(Val.Service)] Microsoft.Extensions.Logging.ILoggerFactory factory

            , [ApiVal(Val.Cookie)] int SessionId
            , [ApiVal(Val.Header)] string Cookie1
            , [ApiVal(Val.QueryMode)] system whereSort)
        {
            //System.Data.DataSet ds = dbHelper.Query("SELECT * FROM system WHERE Id=@a OrDER by id desc", new { a = 1, b = "2", c = true, d = DateTime.Now, e = new byte[] { 60, 254 } });

            //MySql.Data.MySqlClient.MySqlTransaction sqlConnection = dbHelper.CreateTransaction<MySql.Data.MySqlClient.MySqlTransaction>();

            //DbTransResult trans = sqlConnection.ExecuteNonQuery(dbHelper,
            //    dbHelper.GetTextParameter("INSERT INTO system(id,key_en,key_cn,value)VALUES(@Id,'1','1','1')", new { Id = 4 })
            //    , dbHelper.GetTextParameter("INSERT INTO system(id,key_en,key_cn,value)VALUES(@Id,'1','1','1')", new { Id = 5 })
            //    , dbHelper.GetTextParameter("INSERT INTO system(id,key_en,key_cn,value)VALUES(@Id,'1','1','1')", new { Id = 4 }));

            ////new SqlTextParameter("INSERT INTO [dbo].[Backpacks]([Name],[Capacity],[PropId])VALUES('1',1,1)")
            ////    , new SqlTextParameter("INSERT INTO [dbo].[Backpacks]([Name],[Capacity],[PropId])VALUES('1',1,1)")
            ////    , dbHelper.GetTextParameter("INSERT INTO [dbo].[Backpacks]([Id],[Name],[Capacity],[PropId])VALUES(@Id'1',1,1)", new { Id = 4 }));

            //if (trans.Success)
            //{
            //    //trans.Rows;//影响函数
            //}
            ////trans.Exception;//异常情况

            ////System.Data.Common.DbParameter[] dbParameter = dbHelper.GetSpParameterSet("spName");

            ////dbHelper.TransactionExecuteNonQuery

            //var da0 = dbHelper.Select("SELECT * FROM system OrDER by id desc", new { a = 1 });

            ////var da1 = dbHelper.Insert("system", new { id = 4, key_en = "market", key_cn = "可用积分价值", value = "1" });
            ////var da1_1 = dbHelper.Insert<system>(new { id = 4, key_en = "market", key_cn = "可用积分价值", value = "1" });

            ////var da2 = dbHelper.Update("system", "WHERE Id=@a", new { value = 5 }, new { a = 4 });
            ////var da2_1 = dbHelper.Update("system", "WHERE Id=1", new { value = 1 });
            ////var da2_2 = dbHelper.Update<system>("WHERE Id=1", new { value = 1 }, new { a = 1 });
            ////var da2_3 = dbHelper.Update<system>("Id=@Id", new { value = "5" }, new { Id = 4 });

            ////var da0_1 = dbHelper.Select("SELECT * FROM system OrDER by id desc", new { a = 1 });

            ////var da3 = dbHelper.Delete("system", "WHERE Id=@a", new { a = 1 });
            ////var da3_1 = dbHelper.Delete<system>("WHERE Id=@Id", new { Id = 4 });
            ////var da3_2 = dbHelper.Delete<system>("Id=@a", new { a = 1 });

            //if (!ds.IsEmpty())
            //{
            //    return ApiOut.Json(ds.Tables[0].ToDictionary());
            //}

            return ApiOut.Json(new { msg = $"暂无数据。 action:{controller}\\action:{action}\\action:{id1}", IsTask = false, count });
        }

        public class system { public string SortKey { get; init; } = "Id"; }

        public async Task<IApiOut> GetTaskSql(HttpContext context, int id =5)
        {
            //"sda".ToInt();

            System.Data.DataSet ds = dbHelper.Query("SELECT * FROM system OrDER by id desc", new { a = 1, b = 2, c = 3, d = 4 });
            if (ds.IsEmpty()) return await ApiOut.JsonAsync(new { msg = "暂无数据。", IsTask = false, count });
            return await ApiOut.JsonAsync(ds.Tables[0].ToDictionary());
            //ApiOut.View();
            //return await ApiOut.JsonView();
            //return await Task.Run(() =>
            //{
            //    System.Data.DataSet ds = dbHelper.Query("SELECT * FROM system OrDER by id desc", new { a = 1, b = 2, c = 3, d = 4 });
            //    if (!ds.IsEmpty())
            //    {
            //        return ApiOut.JsonAsyn(ds.Tables[0].ToDictionary());
            //    }
            //    return ApiOut.JsonAsyn(new { msg = "暂无数据。", IsTask = false, count });
            //});
        }

        public Task<IApiOut> GetTaskSql1(HttpContext context, int id = 5)
        {
            //"sda".ToInt();
            //var s = context.GetService<IHttpContextAccessor>();

            return  Task.FromResult<IApiOut>(ApiOut.Json(new { msg = "暂无数据。", IsTask = false, count }));
        }

        public IApiOut GetApi() => ApiOut.Json(new { msg = "最小，路由版本api。", IsTask = false, count });//HttpContext context

        public async Task<IApiOut> GetTaskApi() => await ApiOut.JsonAsync(new { msg = "最小，路由版本api。", IsTask = true, count });//HttpContext context

        public ViewOut Get()
        {
            //var d = new Dictionary<string, string> { { "1", "111" }, { "2", "222" }, { "3", "333" } };
            //d.Remove("1", "2");
            //d.TryRemove(out string key, "1", "2", "3");
            //d.AsReadOnly();

            //var d1 = new List<string> { "5", "10" };
            //string xml = d1.ToXml();
            //List<string> s = xml.Xml<List<string>>();

            object sd = new { a = "嗨！", B = 5 };
            object d = sd.GetValue("a");
            bool b = sd.SetValue("b", 10, true);

            return ApiOut.View(); }

        public async Task<IApiOut> GetTask() => await ApiOut.ViewAsync();
    }
}
