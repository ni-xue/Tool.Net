using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Tool.Utils;
using Tool.Utils.ActionDelegate;

namespace Tool.Web
{
    /// <summary>
    /// 对HttpRequest进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class HttpRequestExtension
    {
        ///// <summary>
        ///// 根据前端页面表单填写的信息存入自定义实体(QueryString)
        ///// </summary>
        ///// <typeparam name="F">实体类</typeparam>
        ///// <param name="reques">HttpRequest</param>
        ///// <returns>返回对象</returns>
        //public static F QueryToMode<F>(this HttpRequest reques) where F : new()
        //{
        //    F f = (default(F) == null) ? Activator.CreateInstance<F>() : default(F);
        //    Type type = f.GetType();
        //    PropertyInfo[] properties = type.GetProperties();
        //    PropertyInfo[] array = properties;
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        PropertyInfo propertyInfo = array[i];
        //        string name = propertyInfo.Name;
        //        string value = reques.QueryString[name];
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            type.GetProperty(name).SetValue(f, Sharing.ToValueByType(propertyInfo.PropertyType.Name, value), null);
        //        }
        //    }
        //    return f;
        //}

        /// <summary>
        /// 获取客户端GET方式提交的数据
        /// </summary>
        /// <param name="request">HttpRequest 对象</param>
        /// <param name="strName">名称</param>
        /// <returns></returns>
        public static string GetQueryString(this HttpRequest request, string strName)
        {
            try
            {
                if (request.Query.TryGetValue(strName, out StringValues vs))
                {
                    return vs;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据 Agent 判断当前请求用户的设备名
        ///</summary>    
        ///<returns><see cref="Utils.UserSystem"/>枚举</returns>    
        public static Utils.UserSystem CheckAgent(this HttpRequest request)
        {
            string agent = request?.Headers["User-Agent"];

            return Utils.Validate.CheckAgent(agent);
        }


        /// <summary>
        /// 获取客户端Post方式提交的表单
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="request">HttpRequest</param>
        /// <returns>值</returns>
        public static string GetFormString(this HttpRequest request, string strName)
        {
            try
            {
                if (request.HasFormContentType)
                {
                    if (request.Form.TryGetValue(strName, out StringValues vs))
                    {
                        return vs;
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 包涵两种方式的数据（优先获取Query，获取不到时获取Form）
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <param name="strName">名称</param>
        /// <returns></returns>
        public static string GetString(this HttpRequest request, string strName)
        {
            string value = request.GetQueryString(strName);
            if (value != string.Empty)
            {
                return value;
            }
            value = request.GetFormString(strName);

            return value;
            //if (value != string.Empty)
            //{
            //    return value;
            //}
            //return string.Empty;
        }

        /// <summary>
        /// 根据前端页面表单填写的信息存入自定义实体(QueryString)
        /// </summary>
        /// <typeparam name="M">实体类</typeparam>
        /// <param name="reques">HttpRequest</param>
        /// <param name="istype">表示在转换实体过程中出现异常的处理方式，默认为true，抛出异常。</param>
        /// <returns>返回对象</returns>
        public static M GetToMode<M>(this HttpRequest reques, bool istype = true) where M : new()
        {
            return GetToMode(reques, typeof(M), istype).ToVar<M>();
        }

        /// <summary>
        /// 根据前端页面表单填写的信息存入自定义实体(Form)
        /// </summary>
        /// <typeparam name="M">实体类</typeparam>
        /// <param name="reques">HttpRequest</param>
        /// <param name="istype">表示在转换实体过程中出现异常的处理方式，默认为true，抛出异常。</param>
        /// <returns>返回对象</returns>
        public static M PostToMode<M>(this HttpRequest reques, bool istype = true) where M : new()
        {
            return PostToMode(reques, typeof(M), istype).ToVar<M>();
        }

        /// <summary>
        /// 根据前端页面表单填写的信息存入自定义实体(QueryString或Form)
        /// </summary>
        /// <typeparam name="M">实体类</typeparam>
        /// <param name="reques">HttpRequest</param>
        /// <param name="istype">表示在转换实体过程中出现异常的处理方式，默认为true，抛出异常。</param>
        /// <returns>返回对象</returns>
        public static M ALLToMode<M>(this HttpRequest reques, bool istype = true) where M : new()
        {
            return ALLToMode(reques, typeof(M), istype).ToVar<M>();
        }

        /// <summary>
        /// 根据前端页面表单填写的信息存入自定义实体(QueryString)
        /// </summary>
        /// <param name="reques">HttpRequest</param>
        /// <param name="type">实体对象的Type</param>
        /// <param name="istype">是否抛出异常</param>
        /// <returns>返回对象</returns>
        internal static object GetToMode(HttpRequest reques, Type type, bool istype = true)
        {
            try
            {
                var modeBuild = EntityBuilder.GetEntity(type);
                object m = modeBuild.New;
                try
                {
                    PropertyInfo[] properties = modeBuild.Parameters;// type.GetProperties();
                    IDictionary<string, object> keys = new Dictionary<string, object>();
                    foreach (PropertyInfo property in properties)
                    {
                        if (reques.Query.TryGetValue(property.Name, out var value))
                        {
                            AddValue(keys, value, property.PropertyType, property.Name, istype);
                        }

                        //string name = property.Name;
                        //string value = reques.Query[name];
                        //if (property.PropertyType != typeof(string))
                        //{
                        //    if (!string.IsNullOrEmpty(value))
                        //    {
                        //        type.GetProperty(name).SetValue(m, value.ToVar(property.PropertyType, istype));
                        //    }
                        //}
                        //else
                        //{
                        //    type.GetProperty(name).SetValue(m, value);
                        //}
                    }
                    if (keys.Count > 0) modeBuild.Set(m, keys);
                }
                catch (Exception){}
                
                return m;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据前端页面表单填写的信息存入自定义实体(Form)
        /// </summary>
        /// <param name="reques">HttpRequest</param>
        /// <param name="type">实体对象的Type</param>
        /// <param name="istype">是否抛出异常</param>
        /// <returns>返回对象</returns>
        internal static object PostToMode(HttpRequest reques, Type type, bool istype = true)
        {
            try
            {
                var modeBuild = EntityBuilder.GetEntity(type);
                object m = modeBuild.New;
                if (!reques.HasFormContentType) return m;
                try
                {
                    PropertyInfo[] properties = modeBuild.Parameters;// type.GetProperties();
                    IDictionary<string, object> keys = new Dictionary<string, object>();
                    foreach (PropertyInfo property in properties)
                    {
                        if (reques.Form.TryGetValue(property.Name, out var value))
                        {
                            AddValue(keys, value, property.PropertyType, property.Name, istype);
                        }

                        //string name = property.Name;
                        //string value = reques.Form[name];
                        //if (property.PropertyType != typeof(string))
                        //{
                        //    if (!string.IsNullOrEmpty(value))
                        //    {
                        //        type.GetProperty(name).SetValue(m, value.ToVar(property.PropertyType, istype));
                        //    }
                        //}
                        //else
                        //{
                        //    type.GetProperty(name).SetValue(m, value);
                        //}
                    }
                    if (keys.Count > 0) modeBuild.Set(m, keys);
                }
                catch (Exception){}
                
                return m;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据前端页面表单填写的信息存入自定义实体(QueryString或Form)
        /// </summary>
        /// <param name="reques">HttpRequest</param>
        /// <param name="type">实体对象的Type</param>
        /// <param name="istype">是否抛出异常</param>
        /// <returns>返回对象</returns>
        internal static object ALLToMode(HttpRequest reques, Type type, bool istype = true)
        {
            try
            {
                var modeBuild = EntityBuilder.GetEntity(type);
                object m = modeBuild.New;
                try
                {
                    PropertyInfo[] properties = modeBuild.Parameters;// type.GetProperties();
                    IDictionary<string, object> keys = new Dictionary<string, object>();
                    foreach (PropertyInfo property in properties)
                    {
                        if (reques.Query.TryGetValue(property.Name, out var value) || (reques.HasFormContentType && reques.Form.TryGetValue(property.Name, out value)))
                        {
                            AddValue(keys, value, property.PropertyType, property.Name, istype);
                        }

                        //string name = property.Name;
                        //string value = reques.Query[name];
                        //if (string.IsNullOrEmpty(value) && reques.HasFormContentType)
                        //{
                        //    value = reques.Form[name];
                        //}

                        //if (!string.IsNullOrEmpty(value))
                        //{
                        //    if (property.PropertyType != typeof(string))
                        //    {
                        //        type.GetProperty(name).SetValue(m, value.ToVar(property.PropertyType, istype));
                        //    }
                        //    else
                        //    {
                        //        type.GetProperty(name).SetValue(m, value);
                        //    }
                        //}
                    }
                    if (keys.Count > 0) modeBuild.Set(m, keys);
                }
                catch (Exception){}
                
                return m;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void AddValue(IDictionary<string, object> keys, string value, Type type, string name, bool istype) 
        {
            var data = value.ToVar(type, istype);
            if (data is not null)
                keys.Add(name, data); //value.Equals(StringValues.Empty)
        }

        ///// <summary>
        ///// 根据前端页面表单填写的信息存入自定义实体(Form)
        ///// </summary>
        ///// <typeparam name="T">实体类</typeparam>
        ///// <param name="reques">HttpRequest</param>
        ///// <returns>返回对象</returns>
        //public static T FromValuesToMode<T>(this HttpRequest reques) where T : new()
        //{
        //    T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
        //    Type type = t.GetType();
        //    PropertyInfo[] properties = type.GetProperties();
        //    PropertyInfo[] array = properties;
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        PropertyInfo propertyInfo = array[i];
        //        string name = propertyInfo.Name;
        //        string text = reques.Form[name];
        //        if (!string.IsNullOrEmpty(text))
        //        {
        //            text = text.FormSafeQuery();
        //            if (propertyInfo.PropertyType != typeof(string))//text.GetType().Name
        //            {
        //                //type.GetProperty(name).SetValue(t, Sharing.GetChangeType(text, propertyInfo.PropertyType), null);
        //                type.GetProperty(name).SetValue(t, text.ToVar(propertyInfo.PropertyType));
        //            }
        //            else
        //            {
        //                //type.GetProperty(name).SetValue(t, text, null);
        //                type.GetProperty(name).SetValue(t, text);
        //            }
        //        }
        //    }
        //    return t;
        //}

        /// <summary>
        /// 获取是否是下载请求
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <param name="range">相关对象信息数组</param>
        /// <param name="Length">当前流对象总大小</param>
        /// <returns></returns>
        public static bool IsRange(this HttpRequest request, out List<Range> range, long Length)// [In] [Out]
        {
            //相关实例：Range: bytes=0-4999,-4999,20000-
            //Range: bytes=-4999
            //Range: bytes=20000-

            range = new List<Range>();

            string Range = request.Headers["Range"];

            if (Range == null)
            {
                range = null;
                return false;
            }
            else
            {
                string bytes = Range.Replace("bytes=", "");

                bool IsMultiple = bytes.Contains(',');

                if (IsMultiple)
                {
                    string[] bytes1 = bytes.Split(',');//返回多个数据节点内容

                    foreach (string _bytes1 in bytes1)
                    {
                        long from, to;
                        if (_bytes1.StartsWith("-"))
                        {
                            from = Length + _bytes1.ToLong();
                            to = Length - 1;
                            range.Add(new Range(from, to));
                        }
                        else if (_bytes1.EndsWith("-"))
                        {
                            from = _bytes1.Remove(_bytes1.Length - 1).ToLong();
                            to = Length - 1;
                            range.Add(new Range(from, to));
                        }
                        else if (_bytes1.Contains('-'))
                        {
                            string[] _bytes = _bytes1.Split("-");

                            from = _bytes[0].ToLong();
                            to = _bytes[1].ToLong();
                            range.Add(new Range(from, to));
                        }
                    }
                }
                else
                {
                    long from, to;
                    if (bytes.StartsWith("-"))
                    {
                        from = Length + bytes.ToLong();
                        to = Length - 1;
                        range.Add(new Range(from, to));
                    }
                    else if (bytes.EndsWith("-"))
                    {
                        from = bytes.Remove(bytes.Length - 1).ToLong();
                        to = Length - 1;
                        range.Add(new Range(from, to));
                    }
                    else if (bytes.Contains('-'))
                    {
                        string[] _bytes = bytes.Split("-");

                        from = _bytes[0].ToLong();
                        to = _bytes[1].ToLong();
                        range.Add(new Range(from, to));
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 保存上传的文件
        /// </summary>
        /// <param name="formFile">上传资源对象</param>
        /// <param name="filename">保存文件的完整地址(当地址存在时会覆盖原有文件)</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task Save(this IFormFile formFile, string filename)
        {
            System.IO.Stream stream = formFile.OpenReadStream();

            const int size = 1024 * 512;
            long len = size > formFile.Length ? formFile.Length : size;

            if (len < 1) len = 1;
            //byte[] slice = new byte[len];

            //byte[] slice = new byte[1024 * 1024 * 2];

            //using System.IO.FileStream fileStream = System.IO.File.Create(filename, slice.Length, System.IO.FileOptions.WriteThrough);

            //long seekiength = 0;
            //do
            //{
            //    int i = await stream.ReadAsync(slice.AsMemory(0, slice.Length));
            //    await fileStream.WriteAsync(slice.AsMemory(0, i));
            //    await fileStream.FlushAsync();
            //    seekiength += i;
            //    Array.Clear(slice, 0, i);
            //} while (formFile.Length > seekiength);

            //await fileStream.DisposeAsync();

            using System.IO.FileStream fileStream = System.IO.File.Create(filename, len.ToInt(), System.IO.FileOptions.WriteThrough);
            //OpenWrite
            await HttpContextExtension.StreamMove(stream, fileStream, size);

            await fileStream.DisposeAsync();

            fileStream.Close();
        }
    }

    /// <summary>
    /// 向请求添加指定范围的字节范围标头。
    /// </summary>
    public class Range
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public Range(long from, long to)
        {
            From = from;
            To = to;
        }
        /// <summary>
        /// 开始发送数据的位置。
        /// </summary>
        public long From;
        /// <summary>
        /// 停止发送数据的位置。
        /// </summary>
        public long To;
    }
    
    ///// <summary>
    ///// 支持压缩的枚举类型
    ///// </summary>
    //public enum HttpEncoding
    //{
    //    /// <summary>
    //    /// 比较高效和通用的压缩方式（表示采用  Lempel-Ziv coding (LZ77) 压缩算法，
    //    /// 以及32位CRC校验的编码方式。这个编码方式最初由 UNIX 平台上的 gzip 程序采用。
    //    /// 出于兼容性的考虑，
    //    /// HTTP/1.1 标准提议支持这种编码方式的服务器应该识别作为别名的 x-gzip 指令。）
    //    /// </summary>
    //    gzip,
    //    /// <summary>
    //    /// 第二种压缩方式（采用 zlib 结构 (在 RFC 1950 中规定)，和 deflate 压缩算法(在 RFC 1951 中规定)。）
    //    /// </summary>
    //    deflate
    //}
}
