using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Web.Session
{
    /// <summary>
    /// 自定义的Session对象，必须完成的实现方法
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DiySessionOptions
    {
        /// <summary>
        /// IsAvailable为true时，特点的标志
        /// </summary>
        public const string IsSign = ".1";

        private string _sign;

        /// <summary>
        /// 初始化对象
        /// </summary>
        public DiySessionOptions()
        {
            Cookie = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Unspecified
            };
            _sign = IsSign;
        }

        /// <summary>
        /// 标记值（可以设置为特定值，默认：IsSign值）
        /// </summary>
        public string Sign
        {
            get => _sign; set => _sign = string.IsNullOrWhiteSpace(value) ? IsSign : value;
        }

        /// <summary>
        /// 表明Session存储名称
        /// </summary>
        public string SessionName { get; set; } = "NiXue.Session";

        /// <summary>
        /// 自定义Session必须完成的注册流程
        /// </summary>
        /// <typeparam name="T">实现的基类</typeparam>
        public void GetDiySession<T>() where T : DiySession, new() //: new()
        {
            TypeDiySession = typeof(T);
        }

        /// <summary>
        /// 设置SessionId
        /// </summary>
        /// <param name="context">请求对象</param>
        /// <param name="sessionId">id</param>
        internal void SetSessionId(HttpContext context, string sessionId)
        {
            context.Response.Cookies.Append(SessionName, sessionId, Cookie);
        }

        //（key：返回生效的键值，isSession：是否创建Session对象）
        /// <summary>
        /// 注册一个可以自由控制的开关，以及自由规则的键值。 
        /// 默认提供SessionId值
        /// <para>返回值 为空，时取消设置SessionId行为。</para>
        /// </summary>
        public Func<HttpContext, string, Task<string>> GetKey { get; set; }

        /// <summary>
        /// 获取或设置用户（所有用户共用配置）
        /// </summary>
        public CookieOptions Cookie { get; }

        internal Type TypeDiySession { get; set; }
    }
}
