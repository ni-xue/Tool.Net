using Tool.Utils;
using Tool.Web.Api;

namespace Tool.Docs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAshx(action =>
            {
                action.EnableEndpointRouting = true;
            });
            var app = builder.Build();

            app.UseIgnoreUrl("Views");

            app.UseStaticFiles();
            app.UseRouting();

            app.MapAshxs(delegate (EndpointDataSource routes)
            {
                routes.MapApiRoute("Tool.Docs", "{controller=user}/{action=index}/{id?}");
            });

            app.Run(AppSettings.Get("server.urls"));
        }
    }
    public class User : MinApi
    {
        [Web.Routing.AshxRoute("Index.html")]
        public async Task<IApiOut> Index() => await ApiOut.ViewAsync("Views\\Index.html");

        [Web.Routing.AshxRoute("toc.html")]
        public async Task<IApiOut> Toc() => await ApiOut.ViewAsync("Views\\toc.html");

        [Web.Routing.AshxRoute("introduction.html")]
        public async Task<IApiOut> Introduction() => await ApiOut.ViewAsync("Views\\introduction.html");

        [Web.Routing.AshxRoute("getting-started.html")]
        public async Task<IApiOut> Gettingstarted() => await ApiOut.ViewAsync("Views\\getting-started.html");
    }
}
