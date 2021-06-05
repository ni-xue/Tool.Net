using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tool;
using Tool.SqlCore;
using Tool.Utils.Data;
using Tool.Web;

namespace WebTestApp.Controllers
{
    public class HomeController : Controller
    {
        public HttpClient httpClient { get; set; }

        private readonly DbHelper dbHelper;
        //[FromBody]
        public HomeController(DbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            //this.httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            Response.AppendHeader("a我", "abc123我是谁？");
            //Response.AppendCookie("a", "abc123我是谁？");

            System.Data.DataSet ds = await dbHelper.ExecuteDataSetAsync("SELECT * FROM system OrDER by id desc", new { a = 1, b = 2, c = 3, d = 4 });

            var list = dbHelper.Select<Test.system>(s => s.key_cn = "可用积分价值");

            //var reader = dbHelper.ExecuteReader(System.Data.CommandType.Text, "SELECT * FROM system OrDER by id desc");

            if (ds.IsEmpty()) return Json(new { msg = "暂无数据。", IsTask = false });
            //"hhh".ToInt();
            //HttpContext.Session

            var data = ds.Tables[0].ToDictionaryIf((key, val) =>
            {
                switch (key)
                {
                    case "value":
                        return 0;
                    case string i and "s" when i.Contains("s"):

                        return 0;
                    default:
                        break;
                }

                if (key == "value")
                {
                    return 0;
                }
                return val;
            });


            return Json(data);
        }
    }
}