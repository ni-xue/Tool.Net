using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool;
using Tool.Web;
using Tool.Web.Api;
using Tool.Web.Routing;

namespace WebTestApp.ApiView
{
    [AshxRoute(template: "he/{action=Index}/{id?}")]
    public class Heheh : MinApi
    {
        protected override IApiOut Initialize(AshxRouteData ashxRoute)
        {
            //if (ashxRoute.Action != "Download")
            //{
            //    return ApiOut.Redirect("Download/cs.html");
            //}
            return null;
        }

        protected override IApiOut AshxException(AshxException ex)
        {
            //for (int i = 0; i < 10000; i++)
            //{
            //    Tool.Utils.Log.Error("异常日志", ex);
            //}

            Tool.Utils.Log.Error("异常日志", ex);

            //ex.IsParameters = true;
            ex.ExceptionHandled = true;
            return null;//base.AshxException(ex);
        }

        protected override void OnResult(AshxRouteData ashxRoute)
        {
            //ApiOut.Redirect("");
        }

        //[AshxRoute(template: "小鸟/{id?}")]
        [AshxRoute(template: "小鸟/{id=cs}.html")]
        //public IApiOut Index([ApiVal(Val.RouteKey)] string id)
        //{
        //    "hhh".ToInt();

        //    return ApiOut.View(id + ".html");// ApiOut.ViewAsyn();  //
        //}

        public async Task<IApiOut> Index(string p, int a, decimal c, [ApiVal(Val.AllMode)] Api.ps app, [ApiVal(Val.RouteKey)] string d,
            [ApiVal(Val.Service)] AshxRouteData e,
            [ApiVal(Val.RouteKey)] string id = "cs")
        {
            "hhh".ToInt();
            
            return await ApiOut.PathViewAsync(id);// ApiOut.ViewAsyn();  //
        }

        [AshxRoute(template: "小鸟/{id?}/{id1?}")]
        public async Task<IApiOut> Cs([ApiVal(Val.RouteKey)] string action, [ApiVal(Val.RouteKey)] string id, [ApiVal(Val.RouteKey)] string id1)
        {
            return await ApiOut.WriteAsync(action + id + id1);
        }

        //Api/{controller=Heheh}/{action=Download}/{id1?}

        //[AshxRoute(template: "小鸟/{id1=index}.html")]
        public async Task<IApiOut> Download([ApiVal(Val.RouteKey)] string id,
            [ApiVal(Val.Service)] Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHost)
        {
             return await ApiOut.FileAsync(id, System.IO.File.OpenRead(webHost.WebRootPath + "\\Download\\" + id));
        }

        public async Task<IApiOut> Upload(
            [ApiVal(Val.File)] IFormFile file_data, 
            [ApiVal(Val.Header, ".123")] string abc, 
            [ApiVal(Val.Header, "User-Agent")] string agent)
        {
            await file_data.Save(AppContext.BaseDirectory + "Upload\\" + file_data.FileName);
            return await ApiOut.JsonAsync(new { mag = "保存成功！" });
        }

    }
}
