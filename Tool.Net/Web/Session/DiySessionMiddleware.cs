using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace Tool.Web.Session
{
    /// <summary>
    /// AsSession中间件
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DiySessionMiddleware //: IMiddleware
    {
        //private LazyConcurrentDictionary<string, DiySession> AsSessionList { get; }

        private readonly RequestDelegate Next;

        private readonly ILogger Logger;

        private readonly Type TypeDiySession;

        private readonly ClassDispatcher<DiySession> NewDiySession;

        private readonly Func<HttpContext, Task<(string key, bool isSession)>> GetKey;

        /// <summary>
        /// 表明Session存储名称
        /// </summary>
        public readonly string SessionName;

        /// <summary>
        /// 创建AsSession协议
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="sessionOptions"></param>
        public DiySessionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, DiySessionOptions sessionOptions)//,RequestDelegate next ILoggerFactory loggerFactory, IDataProtectionProvider dataProtectionProvider, ISessionStore sessionStore, IOptions<SessionOptions> options
        {
            Next = next;
            Logger = loggerFactory.CreateLogger("DiySession");
            this.NewDiySession = new(sessionOptions.TypeDiySession);
            this.TypeDiySession = sessionOptions.TypeDiySession;
            this.SessionName = sessionOptions.SessionName;
            this.GetKey = sessionOptions.GetKey;
            //AsSessionList = new LazyConcurrentDictionary<string, DiySession>();
        }

        ///// <summary>
        ///// 处理每次请求,配置DiySession
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //public async Task Invoke(HttpContext context)
        //{
        //    //await Task.Run(() => 
        //    //{
        //    //    context.Session = new AsSession();
        //    //});
        //    AsSession session;

        //    if (context.Request.Cookies.TryGetValue(SessionName, out string value))
        //    {
        //        if (!AsSessionList.TryGetValue(value, out session))
        //        {
        //            session = new AsSession(value);
        //            AsSessionList.TryAdd(value, session);
        //        }
        //    }
        //    else
        //    {
        //        value = StringExtension.GetGuid();
        //        context.Response.Cookies.Append(SessionName, value);
        //        session = new AsSession(value);
        //        AsSessionList.TryAdd(value, session);
        //    }

        //    context.Session = session;

        //    await Next?.Invoke(context);
        //    //throw null;
        //}

        /// <summary>
        /// 处理每次请求,配置AsSession
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)//, RequestDelegate next/// <param name="next"></param>
        {

            //if (context.Request.Cookies.TryGetValue(SessionName, out string value))
            //{
            //    if (!AsSessionList.TryGetValue(value, out session))
            //    {
            //        AddAsSession(out session, value);
            //        //session = new AsSession(value);
            //        //AsSessionList.TryAdd(value, session);
            //    }
            //}
            //else
            //{
            //    value = StringExtension.GetGuid();
            //    context.Response.Cookies.Append(SessionName, value, new CookieOptions() 
            //    { 
            //        HttpOnly  = true,
            //        IsEssential = true,
            //        //Expires = DateTime.UtcNow.AddMinutes(20),// new DateTime(TimeSpan.FromMinutes(20).Ticks),
            //        SameSite = SameSiteMode.Unspecified
            //    });
            //    AddAsSession(out session, value);
            //    //session = new AsSession(value);
            //    //AsSessionList.TryAdd(value, session);
            //}

            if (!context.Request.Cookies.TryGetValue(SessionName, out string value))
            {
                if (GetKey != null)
                {
                    var (key, isSession) = await GetKey?.Invoke(context);
                    if (isSession)
                    {
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            key = StringExtension.GetGuid();
                        }
                        value = key;
                    }
                    else
                    {
                        await Next?.Invoke(context);
                        return;
                    }
                }
                else
                {
                    value = StringExtension.GetGuid();
                }
              
                context.Response.Cookies.Append(SessionName, value, new CookieOptions()
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Unspecified
                });
            }

            AddAsSession(out DiySession session, value);
            context.Session = session;
            await Next?.Invoke(context);
        }

        private void AddAsSession(out DiySession session, string value) 
        {
            //InsideInitialize
            session = this.NewDiySession.Invoke(); // Activator.CreateInstance(this.TypeDiySession) as DiySession;  //this.SessionOptions.OnGetSession(value);//new DiySession(value);
            session.Logger = this.Logger;
            session.InsideInitialize(value);
            //AsSessionList.TryAdd(value, session);

            if (session.IsAvailable)
            {
                Debug(value);
            }
        }

        private void Debug(string value)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("SessionID: {Key} 初始化已完成！", value);
            }
        }
    }
}
