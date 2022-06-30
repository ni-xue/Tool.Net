using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace Tool.Web
{
    /// <summary>
    /// 对<see cref="HttpContext"/>进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class HttpContextExtension //: IRequiresSessionState
    {
        /// <summary>
        /// 获取全局HttpContext对象
        /// </summary>
        public static IHttpContextAccessor Accessor { get; internal set; }

        /// <summary>
        /// 获取当前http请求的HttpContext对象，带异常提示。
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                return Accessor.HttpContext ?? throw new Exception("当前无法获取到 HttpContext 对象。,可能是您未注入(示例：services.AddAshx().AddHttpContext();)", null);
            }
        }

        /// <summary>
        /// 获取当前请求地址的 主要信息（支持代理模式信息获取）
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>返回连接协议和原始主机请求地址</returns>
        public static (string scheme, string host) GetSchemeHost(this HttpContext context)
        {
            HttpRequest request = context.Request;

            if (!request.Headers.TryGetValue("X-Forwarded-Proto", out var scheme)) scheme = request.Scheme;
            if (!request.Headers.TryGetValue("X-Forwarded-Host", out var host)) host = request.Host.ToString();

            return (scheme, host);
        }

        /// <summary>
        /// 获取客户端请求的IP地址（支持代理模式信息获取）
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>返回IP地址</returns>
        public static string GetUserIp(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var ips))
            {
                return ips;
            }
            else
            {
                return context.Connection.RemoteIpAddress.ToString();
            }

            //string text = string.Empty;
            //if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_VIA"]))
            //{
            //    text = Convert.ToString(context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            //}
            //if (string.IsNullOrEmpty(text))
            //{
            //    text = Convert.ToString(context.Request.ServerVariables["REMOTE_ADDR"]);
            //}
            //if (text.Equals("::1"))
            //{
            //    text = "127.0.0.1";
            //}

            //return context.Connection.RemoteIpAddress.ToString();

            //return string.Concat(context.Connection.RemoteIpAddress.ToString(), ':', context.Connection.RemotePort);

            //string text = string.Empty;
            //text = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //string a;
            //if ((a = text) == null || a == "")
            //{
            //    text = context.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //if (text == null || text == string.Empty)
            //{
            //    text = context.Request.UserHostAddress;
            //}
            //if (text == null || !(text != string.Empty) || !Utils.Validate.IsIP(text))
            //{
            //    return "0.0.0.0";
            //}
            //return text;
        }

        /// <summary>
        /// 设置错误，并指定错误号
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StatusCode"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static Exception AddHttpException(this HttpContext context, int StatusCode, string format, params object[] args)
        {
            context.Response.StatusCode = StatusCode;
            return new Exception(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// 获取服务（在 System.IServiceProvider 从中检索服务对象。）
        /// </summary>
        /// <typeparam name="T">要获取的服务对象的类型。</typeparam>
        /// <param name="context">HttpContext</param>
        /// <returns>类型为 T 或 null 的服务对象（如果没有此类服务）。</returns>
        public static T GetService<T>(this HttpContext context)
        {
            return context.RequestServices.GetService<T>();
        }

        /// <summary>
        /// 当前路径
        /// </summary>
        public static string CurrentPath
        {
            get
            {
                if (Current == null)
                {
                    return string.Empty;
                }
                string text = Current.Request.Path;
                text = text[..text.LastIndexOf("/")];
                if (text == "/")
                {
                    return string.Empty;
                }
                return text;
            }
        }

        /// <summary>
        /// 获取有关当前请求的 URL 的信息。
        /// </summary>
        public static string CurrentUrl
        {
            get
            {
                return $"{Current.Request.Scheme}://{Current.Request.Host.Value}{Current.Request.Path.Value}"; //GameRequest.GetUrl();
            }
        }

        /// <summary>
        /// 获取有关当前请求的 域名部分 的信息。
        /// </summary>
        public static string CurrentSchemeHost
        {
            get
            {
                return $"{Current.Request.Scheme}://{Current.Request.Host.Value}/"; //GameRequest.GetUrl();
            }
        }

        ///// <summary>  
        ///// 设置全局数据缓存，如果存在仅修改值
        ///// </summary>  
        ///// <param name="context">HttpContext</param>
        ///// <param name="cacheKey">键值名称</param>
        ///// <param name="objObject">值</param>
        //public static void SetCache(this HttpContext context, string cacheKey, object objObject)
        //{
        //    if (string.IsNullOrWhiteSpace(cacheKey))
        //    {
        //        throw new System.SystemException("键值名称不能为空！");
        //    }
        //    if (context.Cache == null || context.Cache[cacheKey] == null)
        //    {
        //        context.Cache.Insert(cacheKey, objObject);
        //        return;
        //    }
        //    context.Cache[cacheKey] = objObject;
        //}

        ///// <summary>  
        ///// 设置数据缓存  
        ///// </summary>  
        ///// <param name="context">HttpContext</param>
        ///// <param name="cacheKey">键值名称</param>
        ///// <param name="objObject">值</param>
        ///// <param name="timeout">单位秒，默认7200秒</param>
        //public static void SetCache(this HttpContext context, string cacheKey, object objObject, int timeout = 7200)
        //{
        //    if (string.IsNullOrWhiteSpace(cacheKey))
        //    {
        //        throw new System.SystemException("键值名称不能为空！");
        //    }
        //    //相对过期  
        //    //context.Cache.Insert(cacheKey, objObject, null, DateTime.MaxValue, timeout, CacheItemPriority.NotRemovable, null);  
        //    //绝对过期时间  
        //    context.Cache.Insert(cacheKey, objObject, null, DateTime.Now.AddSeconds(timeout), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
        //}

        ///// <summary>  
        ///// 获取数据缓存  
        ///// </summary>  
        ///// <param name="context">HttpContext</param>
        ///// <param name="cacheKey">键</param>  
        ///// <returns>返回一个值，如果键值名称不存在则返回NULL</returns>
        //public static object GetCache(this HttpContext context, string cacheKey)
        //{
        //    if (string.IsNullOrWhiteSpace(cacheKey))
        //    {
        //        throw new System.SystemException("键值名称不能为空！");
        //    }
        //    if (context.Cache == null || context.Cache[cacheKey] == null)
        //    {
        //        return null;
        //    }
        //    return context.Cache.Get(cacheKey); ;
        //}

        ///// <summary>  
        ///// 移除指定数据缓存  
        ///// </summary>  
        ///// <param name="context">HttpContext</param>
        ///// <param name="cacheKey">键</param>
        //public static void RemoveCache(this HttpContext context, string cacheKey)
        //{
        //    if (string.IsNullOrWhiteSpace(cacheKey))
        //    {
        //        throw new System.SystemException("键值名称不能为空！");
        //    }
        //    if (context.Cache == null || context.Cache[cacheKey] == null)
        //    {
        //        return;
        //    }
        //    context.Cache.Remove(cacheKey);
        //}

        ///// <summary>  
        ///// 移除全部缓存  
        ///// </summary>  
        ///// <param name="context">HttpContext</param>
        //public static void RemoveAllCache(this HttpContext context)
        //{
        //    if (context.Cache == null)
        //    {
        //        return;
        //    }
        //    var cacheEnum = context.Cache.GetEnumerator();
        //    while (cacheEnum.MoveNext())
        //    {
        //        context.Cache.Remove(cacheEnum.Key.ToString());
        //    }
        //}

        ///// <summary>
        ///// 调用此方法可以对请求的信息进行压缩处理
        ///// </summary>
        ///// <param name="reques"><see cref="HttpContext"/>对象</param>
        ///// <returns>返回当前发起请求的用户浏览器是否支持压缩信息 状态：<see cref="bool"/>类型</returns>
        //public static bool Deflate_Gzip(this HttpContext reques)
        //{
        //    string Accept = reques.Request.Headers["Accept-Encoding"];

        //    if (string.IsNullOrWhiteSpace(Accept))
        //    {
        //        return false;
        //    }

        //    if (Accept.Contains("gzip"))
        //    {
        //        reques.Response.AppendHeader("Content-Encoding", "gzip");
        //        reques.Response.Filter = new GZipStream(reques.Response.Filter, CompressionMode.Compress);
        //        return true;
        //    }
        //    else if (Accept.Contains("deflate"))
        //    {
        //        reques.Response.AppendHeader("Content-Encoding", "deflate");
        //        reques.Response.Filter = new DeflateStream(reques.Response.Filter, CompressionMode.Compress);
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 调用此方法可以对请求的信息进行压缩处理
        ///// </summary>
        ///// <param name="reques"><see cref="HttpContext"/>对象</param>
        ///// <param name="httpEncoding">压缩枚举</param>
        ///// <returns>返回当前发起请求的用户浏览器是否支持压缩信息 状态：<see cref="bool"/>类型</returns>
        //public static bool Deflate_Gzip(this HttpContext reques, HttpEncoding httpEncoding)
        //{
        //    string Accept = reques.Request.Headers["Accept-Encoding"];

        //    if (string.IsNullOrWhiteSpace(Accept))
        //    {
        //        return false;
        //    }

        //    if (httpEncoding == HttpEncoding.gzip)
        //    {
        //        if (Accept.Contains("gzip"))
        //        {
        //            reques.Response.AppendHeader("Content-Encoding", "gzip");
        //            reques.Response.Filter = new GZipStream(reques.Response.Filter, CompressionMode.Compress);
        //            return true;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    else if (httpEncoding == HttpEncoding.deflate)
        //    {
        //        if (Accept.Contains("deflate"))
        //        {
        //            reques.Response.AppendHeader("Content-Encoding", "deflate");
        //            reques.Response.Filter = new DeflateStream(reques.Response.Filter, CompressionMode.Compress);
        //            return true;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 解开以压缩的输出流
        ///// </summary>
        ///// <param name="reques"><see cref="HttpContext"/>对象</param>
        ///// <returns>返回状态</returns>
        //public static bool DecodeDeflate_Gzip(this HttpContext reques)
        //{
        //    string Content = reques.Response.Headers["Content-Encoding"];

        //    if (string.IsNullOrWhiteSpace(Content))
        //    {
        //        return false;
        //    }

        //    if (Content.Contains("deflate"))
        //    {
        //        //reques.Response.Headers.Remove("Accept-Encoding");
        //        reques.Response.Headers.Remove("Content-Encoding");
        //        reques.Response.Filter = new DeflateStream(reques.Response.Filter, CompressionMode.Decompress);
        //        return true;
        //    }
        //    else if (Content.Contains("gzip"))
        //    {
        //        //reques.Response.Headers.Remove("Accept-Encoding");
        //        reques.Response.Headers.Remove("Content-Encoding");
        //        reques.Response.Filter = new GZipStream(reques.Response.Filter, CompressionMode.Decompress);
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 将文件流分流写入
        /// </summary>
        /// <param name="ReadStream">原文件流</param>
        /// <param name="WriteStream">更新文件流</param>
        /// <param name="minlen">最小资源大小</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task StreamMove(System.IO.Stream ReadStream, System.IO.Stream WriteStream, int minlen) 
        {
            long length = ReadStream.Length;
            long len = (minlen) > length ? length : (minlen);

            byte[] slice = new byte[len];

            long seekiength = 0;
            do
            {
                int i = await ReadStream.ReadAsync(slice.AsMemory(0, slice.Length));
                await WriteStream.WriteAsync(slice.AsMemory(0, i));
                await WriteStream.FlushAsync();
                seekiength += i;
                Array.Clear(slice, 0, i);
            } while (length > seekiength);

            await ReadStream.DisposeAsync();
        }
    }

    /// <summary>
    /// 对<see cref="ISession"/>进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class SessionExtension
    {
        /// <summary>
        /// 增加一个键值对，如果存在仅修改值
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <param name="value">值，全部以string值保存</param>
        public static void Set(this ISession session, string key, object value)
        {
            session.Set(key, value.ToString());
        }

        /// <summary>
        /// 增加一个键值对，如果存在仅修改值
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <param name="value">获得的值</param>
        public static void Set<T>(this ISession session, string key, T value) where T : new()
        {
            session.Set(key, value.ToJson());
        }

        /// <summary>
        /// 增加一个键值对，如果存在仅修改值
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <param name="value">值</param>
        public static void Set(this ISession session, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new System.SystemException("键值名称不能为空！");
            }
            session.Set(key, value.ToBytes(Encoding.UTF8));
        }

        /// <summary>
        /// 获取Session数据（无值不发生异常）
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <returns>返回一个值</returns>
        public static string Get(this ISession session, string key) 
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new System.SystemException("键值名称不能为空！");
            }

            session.TryGetValue(key, out string value_1);

            return value_1;
        }

        /// <summary>
        /// 获取Session数据（无值不发生异常）
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <returns>返回一个值</returns>
        public static T Get<T>(this ISession session, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new System.SystemException("键值名称不能为空！");
            }

            bool iskey = session.TryGetValue(key, out string value_1);
            if (iskey)
            {
                return value_1.Json<T>();
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 获取Session数据
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <param name="value">获得的值</param>
        /// <returns>返回一个值，如果键值名称不存在则返回NULL</returns>
        public static bool TryGetValue(this ISession session, string key, out string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new System.SystemException("键值名称不能为空！");
            }

            bool iskey = session.TryGetValue(key, out byte[] value_1);
            if (iskey)
            {
                value = value_1.ToByteString(Encoding.UTF8);
            }
            else
            {
                value = null;
            }
            return iskey;
        }

        /// <summary>
        /// 获取Session数据
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <param name="value">获得的值</param>
        /// <returns>返回一个值，如果键值名称不存在则返回NULL</returns>
        public static bool TryGetValue<T>(this ISession session, string key, out T value) where T : new()
        {
            bool iskey = session.TryGetValue(key, out string value_1);
            if (iskey)
            {
                value = value_1.Json<T>();
            }
            else
            {
                value = default;
            }
            return iskey;
        }

        /// <summary>
        /// 获取Session数据
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">键值名称</param>
        /// <param name="type">转换成对象的类型</param>
        /// <param name="value">获得的值</param>
        /// <returns>返回一个值，如果键值名称不存在则返回NULL</returns>
        public static bool TryGetValue(this ISession session, string key, Type type, out object value)
        {
            bool iskey = session.TryGetValue(key, out string value_1);
            if (iskey)
            {
                try
                {
                    value = System.Text.Json.JsonSerializer.Deserialize(value_1, type);
                }
                catch (Exception)
                {

                    value = null;
                }
            }
            else
            {
                value = default;
            }
            return iskey;
        }
    }
}
