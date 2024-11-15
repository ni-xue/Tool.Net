using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Tool.Utils.Encryption;
using Tool.Web;

namespace Tool.Utils
{
    /// <summary>
    /// 常用方法类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public partial class Utility
    {
        //地球半径，单位米
        private const double EARTH_RADIUS = 6378137;

        /// <summary>
        /// 比较两个数组是否相同
        /// </summary>
        /// <param name="x">对比一</param>
        /// <param name="y">对比二</param>
        /// <returns>结果</returns>
        public static bool SequenceCompare(in ReadOnlySpan<byte> x, in ReadOnlySpan<byte> y)
        {
            if (x.Length != y.Length) return false;
            return x.SequenceEqual(y);
        }

        #region 获取整数的某一位，设置整数的某一位  

        /// <summary>  
        /// 取整数的某一位  
        /// </summary>  
        /// <param name="_Resource">要取某一位的整数</param>  
        /// <param name="_Mask">要取的位置索引，自右至左为0-7</param>  
        /// <returns>返回某一位的值（0或者1）</returns>  
        public static int GetIntegerSomeBit(int _Resource, int _Mask)
        {
            return _Resource >> _Mask & 1;
        }

        /// <summary>  
        /// 将整数的某位置为0或1  
        /// </summary>  
        /// <param name="_Mask">整数的某位</param>  
        /// <param name="a">整数</param>  
        /// <param name="flag">是否置1，TURE表示置1，FALSE表示置0</param>  
        /// <returns>返回修改过的值</returns>  
        public static int SetIntegerSomeBit(int _Mask, int a, bool flag)
        {
            if (flag)
            {
                a |= 0x1 << _Mask;
            }
            else
            {
                a &= ~(0x1 << _Mask);
            }
            return a;
        }

        #endregion

        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位：米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="size">一个数字</param>
        /// <returns>返回一个数组（每个位）</returns>
        public static IList<int> GetDistance(long size)
        {
            List<int> list = new();
            while (size > 0)
            {
                list.Add((int)(size % 10)); //计算每一位上的数字
                size /= 10; //实现位与位之间的遍历
            }
            list.Reverse();

            return list;
        }

        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位：米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat1">第一点纬度</param>        
        /// <param name="lng2">第二点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <returns>返回相隔距离</returns>
        public static double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180d;
        }

        ///// <summary>
        ///// 根据url路径获取网页源码
        ///// </summary>
        ///// <param name="sUrl">请求的URL地址</param>
        ///// <returns>返回访问内容</returns>
        //[MTAThread]
        //public static string GetWebContent(string sUrl)
        //{
        //	if (sUrl.Equals("about:blank")) return null;
        //	string strResult = "";

        //	if (!sUrl.StartsWith("http://") && !sUrl.StartsWith("https://")) { sUrl = "http://" + sUrl; }
        //	try
        //	{
        //		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
        //		//声明一个HttpWebRequest请求
        //		request.Timeout = 50000;//给了50秒超时
        //								//设置连接超时时间
        //		request.Headers.Set("Pragma", "no-cache");
        //		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //		if (response.ToString() != "")
        //		{
        //			Stream streamReceive = response.GetResponseStream();
        //			Encoding encoding = Encoding.GetEncoding("UTF-8");

        //			StreamReader streamReader = new(streamReceive, encoding);
        //			strResult = streamReader.ReadToEnd();
        //		}
        //	}
        //	catch (Exception e)
        //	{
        //		throw new Exception("出现异常:", e);
        //		//strResult = "异常提示：" + e.Message;
        //	}
        //	return strResult;
        //}

        ///// <summary>
        ///// 生成指定长度的随机密码。
        ///// </summary>
        ///// <param name="length">生成的密码的字符数。 长度必须介于 1 和 128 个字符之间。</param>
        ///// <param name="numberOfNonAlphanumericCharacters">生成的密码中非字母数字字符的最小数量 （如 @、#、！、% 等) 。</param>
        ///// <returns>返回随机密码</returns>
        ///// <exception cref="T:System.ArgumentException">length 小于 1 或大于 128 - 或 - numberOfNonAlphanumericCharacters 小于 0 或大于 length。</exception>
        //public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        //{
        //	return System.Web.Security.Membership.GeneratePassword(length, numberOfNonAlphanumericCharacters);
        //}

        ///// <summary>
        ///// 将当前网站内的.Aspx 页面 转换成静态的.html 页面
        ///// </summary>
        ///// <param name="path">.Aspx 页面的虚拟路径</param>
        ///// <param name="outPath">转换成的.html 路径</param>
        //public static void Aspx2XHtml(string path, string outPath)
        //{
        //	Page page = new Page();
        //	StringWriter stringWriter = new StringWriter();
        //	page.Server.Execute(path, stringWriter);
        //	FileStream fileStream;
        //	if (File.Exists(page.Server.MapPath(outPath)))
        //	{
        //		File.Delete(page.Server.MapPath(outPath));
        //		fileStream = File.Create(page.Server.MapPath(outPath));
        //	}
        //	else
        //	{
        //		fileStream = File.Create(page.Server.MapPath(outPath));
        //	}
        //	byte[] bytes = Encoding.UTF8.GetBytes(stringWriter.ToString());
        //	fileStream.Write(bytes, 0, bytes.Length);
        //	fileStream.Close();
        //}

        ///// <summary>
        ///// 清除页面客户端缓存
        ///// </summary>
        //public static void ClearPageClientCache()
        //{
        //	if (HttpContext.Current != null)
        //	{
        //		HttpContext.Current.Response.Buffer = false;
        //		HttpContext.Current.Response.Expires = 0;
        //		HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1.0);
        //		HttpContext.Current.Response.AddHeader("pragma", "no-cache");
        //		HttpContext.Current.Response.AddHeader("cache-control", "private");
        //		HttpContext.Current.Response.CacheControl = "no-cache";
        //		HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //		HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(true);
        //		HttpContext.Current.Response.Cookies.Clear();
        //	}
        //}

        ///// <summary>
        ///// 设置页面无缓存
        ///// </summary>
        //public static void SetPageNoCache()
        //{
        //	if (HttpContextExtension.Current != null)
        //	{
        //		HttpContextExtension.Current.Response.Buffer = true;
        //		HttpContextExtension.Current.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
        //		HttpContextExtension.Current.Response.Expires = 0;
        //		HttpContextExtension.Current.Response.CacheControl = "no-cache";
        //		HttpContextExtension.Current.Response.AppendHeader("Pragma", "No-Cache");
        //	}
        //}

        /// <summary>
        /// IP位移操作
        /// </summary>
        /// <param name="strVersion">IP地址</param>
        /// <returns></returns>
        public static int ConvertVersionStr2Int(string strVersion)
        {
            if (!Validate.IsIP(strVersion))
            {
                return 0;
            }
            string[] array = strVersion.Split('.');
            return array[0].ToInt() << 24 | array[1].ToInt() << 16 | array[2].ToInt() << 8 | array[3].ToInt();
        }

        /// <summary>
        /// 将<see cref="DataTable"/> 对象 转换成 JOSN 字符串
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <returns>JOSN 字符串</returns>
        public static StringBuilder DataTableToJson(DataTable dt)
        {
            return Utility.DataTableToJson(dt, true);
        }

        /// <summary>
        /// 将<see cref="DataTable"/> 对象 转换成 JOSN 字符串
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="dtDispose">是否释放数据源</param>
        /// <returns>JOSN 字符串</returns>
        public static StringBuilder DataTableToJson(DataTable dt, bool dtDispose)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("[\r\n");
            string[] array = new string[dt.Columns.Count];
            int num = 0;
            string text = "{{";
            foreach (DataColumn dataColumn in dt.Columns)
            {
                array[num] = dataColumn.Caption.ToLower().Trim();
                text = text + "'" + dataColumn.Caption.ToLower().Trim() + "':";
                string text2 = dataColumn.DataType.ToString().Trim().ToLower();
                if (text2.IndexOf("int") > 0 || text2.IndexOf("deci") > 0 || text2.IndexOf("floa") > 0 || text2.IndexOf("doub") > 0 || text2.IndexOf("bool") > 0)
                {
                    object obj = text;
                    text = string.Concat(new object[]
                    {
                        obj,
                        "{",
                        num,
                        "}"
                    });
                }
                else
                {
                    object obj = text;
                    text = string.Concat(new object[]
                    {
                        obj,
                        "'{",
                        num,
                        "}'"
                    });
                }
                text += ",";
                num++;
            }
            if (text.EndsWith(","))
            {
                text = text.Substring(0, text.Length - 1);
            }
            text += "}},";
            num = 0;
            object[] array2 = new object[array.Length];
            foreach (DataRow dataRow in dt.Rows)
            {
                string[] array3 = array;
                for (int i = 0; i < array3.Length; i++)
                {
                    string arg_1EF_0 = array3[i];
                    array2[num] = dataRow[array[num]].ToString().Trim().Replace("\\", "\\\\").Replace("'", "\\'");
                    string text3 = array2[num].ToString();
                    if (text3 != null)
                    {
                        if (!(text3 == "True"))
                        {
                            if (text3 == "False")
                            {
                                array2[num] = "false";
                            }
                        }
                        else
                        {
                            array2[num] = "true";
                        }
                    }
                    num++;
                }
                num = 0;
                stringBuilder.Append(string.Format(text, array2));
            }
            if (stringBuilder.ToString().EndsWith(","))
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            if (dtDispose)
            {
                dt.Dispose();
            }
            return stringBuilder.Append("\r\n];");
        }

        ///// <summary>
        ///// 转换指定的字符串，以使用 % 字符对保留字符（@、*、_、+、-、. 和 /）进行转义，并以 Unicode 表示法表示它们。
        ///// </summary>
        ///// <param name="str">要转换的字符串。</param>
        ///// <returns>其中的保留字符通过 % 字符进行转义并以 Unicode 表示法表示的 string 的新副本。</returns>
        //public static string Escape(string str)
        //{
        //	return GlobalObject.escape(str);
        //}

        /// <summary>
        /// MD5加密 （32）
        /// </summary>
        /// <param name="s">原文</param>
        /// <returns>密文</returns>
        public static string MD5(string s)
        {
            return TextEncrypt.MD5EncryptPassword(s);
        }

        /// <summary>
        /// 获取 .config 下面的 appSettings 节点内的节点配置内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            return AppSettings.Get(key);
        }

        /// <summary>
        /// 获取本地计算机的主机名。
        /// </summary>
        /// <returns>包含本地计算机的 DNS 主机名的字符串。</returns>
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// 获取操作系统版本
        /// </summary>
        /// <returns>返回十六进制字符串</returns>
        public static string GetOSVersion()
        {
            string s = Environment.OSVersion.Version.ToString();
            return BitConverter.ToString(Encoding.GetEncoding("GB2312").GetBytes(s)).Replace("-", "");
        }

        /// <summary>
        /// 从HTML获取文本
        /// </summary>
        /// <param name="HTML">HTML字符串</param>
        /// <returns>返回内容</returns>
        public static string GetTextFromHTML(string HTML) => GetTextFromHTMLRegex().Replace(HTML, "");

        /// <summary>
        /// IP 地址的长值。 例如，Big-Endian 格式的值 0x2414188f 可能为 IP 地址"143.24.20.36"。
        /// </summary>
        /// <param name="ipNumber">数字类型的IP信息</param>
        /// <returns>IP地址</returns>
        public static string Int2IP(long ipNumber)
        {
            IPAddress iPAddress = new(ipNumber);
            return iPAddress.ToString();
        }

        /// <summary>
        /// 将IP地址转换为纯数字
        /// </summary>
        /// <param name="ip">正确的IP</param>
        /// <returns>返回纯数字</returns>
        public static long IP2Int(string ip)
        {
            if (!Validate.IsIP(ip))
            {
                return -1L;
            }
            string[] array = ip.Split('.');
            long num = long.Parse(array[3]) * 16777216L;
            num += int.Parse(array[2]) * 65536;
            num += int.Parse(array[1]) * 256;
            return num + int.Parse(array[0]);
        }

        /// <summary>
        /// 验证该string数组里面是否都是可以被转为int类型的数据
        /// </summary>
        /// <param name="strNumber">string数组</param>
        /// <returns>返回<see cref="bool"/>状态</returns>
        public static bool IsNumericArray(params string[] strNumber)
        {
            return TypeParse.IsNumericArray(strNumber);
        }

        ///// <summary>
        ///// 跳转页面 （重定向）
        ///// </summary>
        ///// <param name="url">访问的网址</param>
        //public static void Redirect(string url)
        //{
        //	if (HttpContext.Current != null && !string.IsNullOrEmpty(url))
        //	{
        //		HttpContext.Current.Response.Redirect(url);
        //		HttpContext.Current.Response.StatusCode = 301;
        //		HttpContext.Current.Response.End();
        //	}
        //}

        ///// <summary>
        ///// 输出要下载的文件
        ///// </summary>
        ///// <param name="filepath">下载文件的路径</param>
        ///// <param name="filename">下载文件的名称</param>
        ///// <param name="filetype">设置输出流的 HTTP MIME 类型。</param>
        //public static void ResponseFile(string filepath, string filename, string filetype)
        //{
        //	if (HttpContextExtension.Current != null)
        //	{
        //		Stream stream = null;
        //		byte[] buffer = new byte[10000];
        //		try
        //		{
        //			stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //			long num = stream.Length;
        //			HttpContextExtension.Current.Response.ContentType = filetype;
        //			HttpContextExtension.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + Utility.UrlEncode(filename.Trim()).Replace("+", " "));
        //			while (num > 0L)
        //			{
        //				if (HttpContextExtension.Current.Response.IsClientConnected)
        //				{
        //					int num2 = stream.Read(buffer, 0, 10000);
        //					HttpContextExtension.Current.Response.OutputStream.Write(buffer, 0, num2);
        //					HttpContextExtension.Current.Response.Flush();
        //					buffer = new byte[10000];
        //					num -= (long)num2;
        //				}
        //				else
        //				{
        //					num = -1L;
        //				}
        //			}
        //		}
        //		catch (Exception ex)
        //		{
        //			HttpContextExtension.Current.Response.Write("Error : " + ex.Message);
        //		}
        //		finally
        //		{
        //			if (stream != null)
        //			{
        //				stream.Close();
        //			}
        //		}
        //		HttpContextExtension.Current.Response.End();
        //	}
        //}

        /// <summary>
        /// 查找指定目录下的所有.htm后缀的文件并返回字符串编码是UTF-8的文件路径
        /// </summary>
        /// <param name="directory">指定的目录，绝对路径</param>
        /// <returns>返回是UTF-8格式的.htm路径</returns>
        public static string[] SearchUTF8File(string directory)
        {
            StringBuilder stringBuilder = new StringBuilder();
            FileInfo[] files = new DirectoryInfo(directory).GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension.ToLower().Equals(".htm"))
                {
                    FileStream fileStream = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read);
                    bool flag = Utility.IsUTF8(fileStream);
                    fileStream.Close();
                    if (!flag)
                    {
                        stringBuilder.Append(files[i].FullName);
                        stringBuilder.Append("\r\n");
                    }
                }
            }
            return TextUtility.SplitStrArray(stringBuilder.ToString(), "\r\n");
        }

        /// <summary>
        /// 判断该文件流是否是UTF-8格式
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        private static bool IsUTF8(FileStream sbInputStream)
        {
            bool flag = true;
            long length = sbInputStream.Length;
            byte b = 0;
            int num = 0;
            while ((long)num < length)
            {
                byte b2 = (byte)sbInputStream.ReadByte();
                if ((b2 & 128) != 0)
                {
                    flag = false;
                }
                if (b == 0)
                {
                    if (b2 >= 128)
                    {
                        do
                        {
                            b2 = (byte)(b2 << 1);
                            b += 1;
                        }
                        while ((b2 & 128) != 0);
                        b -= 1;
                        if (b == 0)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((b2 & 192) != 128)
                    {
                        return false;
                    }
                    b -= 1;
                }
                num++;
            }
            return b <= 0 && !flag;
        }

        /// <summary>
        /// 判读该值是否是<see cref="bool"/>类型
        /// </summary>
        /// <param name="expression">判断值</param>
        /// <param name="defValue">当判断值为空时返回的值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            return TypeParse.StrToBool(expression, defValue);
        }

        /// <summary>
        /// 判读该值是否是<see cref="bool"/>类型
        /// </summary>
        /// <param name="expression">判断值</param>
        /// <param name="defValue">当判断值为空时返回的值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            return TypeParse.StrToBool(expression, defValue);
        }

        /// <summary>
        /// 判读该值是否是<see cref="float"/>类型
        /// </summary>
        /// <param name="strValue">判断值</param>
        /// <param name="defValue">当判断值为空时返回的值</param>
        /// <returns>返回<see cref="float"/>类型</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            return TypeParse.StrToFloat(strValue, defValue);
        }

        /// <summary>
        /// 判读该值是否是<see cref="float"/>类型
        /// </summary>
        /// <param name="strValue">判断值</param>
        /// <param name="defValue">当判断值为空时返回的值</param>
        /// <returns>返回<see cref="float"/>类型</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            return TypeParse.StrToFloat(strValue, defValue);
        }

        /// <summary>
        /// 判读该值是否是<see cref="int"/>类型
        /// </summary>
        /// <param name="expression">判断值</param>
        /// <param name="defValue">当判断值为空时返回的值</param>
        /// <returns>返回<see cref="int"/>类型</returns>
        public static int StrToInt(object expression, int defValue)
        {
            return TypeParse.StrToInt(expression, defValue);
        }

        /// <summary>
        /// 判读该值是否是<see cref="int"/>类型
        /// </summary>
        /// <param name="expression">判断值</param>
        /// <param name="defValue">当判断值为空时返回的值</param>
        /// <returns>返回<see cref="int"/>类型</returns>
        public static int StrToInt(string expression, int defValue)
        {
            return TypeParse.StrToInt(expression, defValue);
        }

        /// <summary>
        /// 根据十六进制颜色值返回<see cref="Color"/> 颜色对象
        /// </summary>
        /// <param name="color">十六进制颜色值 例如：#000000</param>
        /// <returns>返回<see cref="Color"/> 颜色对象</returns>
        public static Color ToColor(string color)
        {
            color = color.TrimStart('#');
            color = ToColorRegex().Replace(color.ToLower(), "");
            int length = color.Length;
            char[] array;
            int red;
            int green;
            int blue;
            if (length == 3)
            {
                array = color.ToCharArray();
                red = System.Convert.ToInt32(array[0].ToString() + array[0].ToString(), 16);
                green = System.Convert.ToInt32(array[1].ToString() + array[1].ToString(), 16);
                blue = System.Convert.ToInt32(array[2].ToString() + array[2].ToString(), 16);
                return Color.FromArgb(red, green, blue);
            }
            if (length != 6)
            {
                return Color.FromName(color);
            }
            array = color.ToCharArray();
            red = System.Convert.ToInt32(array[0].ToString() + array[1].ToString(), 16);
            green = System.Convert.ToInt32(array[2].ToString() + array[3].ToString(), 16);
            blue = System.Convert.ToInt32(array[4].ToString() + array[5].ToString(), 16);
            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// 获取域名下的所有IP信息
        /// </summary>
        /// <param name="Host">域名（为空时获取本机IP信息）</param>
        /// <returns></returns>
        public static async Task<IPAddress[]> GetIPAddressListAsync(string Host = null) => await Dns.GetHostAddressesAsync(Host ?? Dns.GetHostName());

        /// <summary>
        /// 获取域名下的第一个IP
        /// </summary>
        /// <param name="Host">域名（为空时获取本机IP信息）</param>
        /// <param name="family">指定获取的类型</param>
        /// <returns>存在就返回第一个，不存在null</returns>
        public static async Task<IPAddress> GetIPAddressAsync(string Host = null, AddressFamily family = AddressFamily.InterNetwork)
        {
            var ipadrs = await GetIPAddressListAsync(Host);
            foreach (var arg in ipadrs)
            {
                if (arg.AddressFamily == family) return arg;
            }
            return null;
        }

        ///// <summary>
        ///// 向客户端返回追踪信息
        ///// </summary>
        ///// <param name="test">内容</param>
        //public static void Trace(string test)
        //{
        //	string format = "<div style='border:1px solid #96C2F1;background-color: #F7F7FF;font-size:14px;font-family:宋体;text-align:right;margin: 0px auto;margin-bottom:5px;margin-right:5px;float:left; text-align:left; line-height:25px; width:800px;'><h5 style='margin: 1px;background-color:#E2EAF8;height: 24px;'>跟踪信息：</h5>{0}</div>";
        //	HttpContextExtension.Current.Response.Write(string.Format(format, test));
        //}

        ///// <summary>
        ///// 向客户端输出结果
        ///// </summary>
        ///// <param name="test">内容</param>
        //public static void TraceWhite(string test)
        //{
        //	HttpContextExtension.Current.Response.Write(test);
        //}

        ///// <summary>
        ///// 将指定字符串中 % 的已转义字符（@、*、_、+、-、. 和 /）转换成其原始格式。 已转义字符以 Unicode 表示法表示。
        ///// </summary>
        ///// <param name="str">要转换的字符串。</param>
        ///// <returns>其中的已转义字符转换为其原始格式的 string 的新副本。</returns>
        //public static string Unescape(string str)
        //{
        //	return GlobalObject.unescape(str);
        //}

        ///// <summary>
        ///// 返回一个A标签 HTML 
        ///// </summary>
        ///// <param name="text">跳转网址</param>
        ///// <returns>返回A标签</returns>
        //public static string Url2HyperLink(string text)
        //{
        //	string pattern = "(http|ftp|https):\\/\\/[\\w]+(.[\\w]+)([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])";
        //	MatchCollection matchCollection = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //	foreach (Match match in matchCollection)
        //	{
        //		text = text.Replace(match.ToString(), string.Concat(new string[]
        //		{
        //			"<a target=\"_blank\" href=\"",
        //			match.ToString(),
        //			"\">",
        //			match.ToString(),
        //			"</a>"
        //		}));
        //	}
        //	return text;
        //}

        /// <summary>
        /// 将已经为 HTTP 传输进行过 HTML 编码的字符串转换为已解码的字符串。
        /// </summary>
        /// <param name="str">要解码的字符串。</param>
        /// <returns>一个已解码的字符串。</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// 将字符串转换为 HTML 编码的字符串。
        /// </summary>
        /// <param name="str">要编码的字符串。</param>
        /// <returns>编码的字符串。</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 将已经为在 URL 中传输而编码的字符串转换为解码的字符串。
        /// </summary>
        /// <param name="str">要解码的字符串。</param>
        /// <returns>一个已解码的字符串。</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 对 URL 字符串进行编码。
        /// </summary>
        /// <param name="str">要编码的文本。</param>
        /// <returns>编码的字符串。</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        ///// <summary>
        ///// 添加Cookie缓存到客户端 
        ///// </summary>
        ///// <param name="strName">键值名</param>
        ///// <param name="strValue">键值</param>
        //public static void WriteCookie(string strName, string strValue)
        //{
        //	HttpCookie httpCookie = HttpContextExtension.Current.Request.Cookies[strName];
        //	if (httpCookie == null)
        //	{
        //		httpCookie = new HttpCookie(strName);
        //	}
        //	httpCookie.Value = strValue;
        //	HttpContextExtension.Current.Response.AppendCookie(httpCookie);
        //}

        ///// <summary>
        ///// 添加Cookie缓存到客户端 
        ///// </summary>
        ///// <param name="strName">键值名</param>
        ///// <param name="strValue">键值</param>
        ///// <param name="expires">距离过期时间（分钟）</param>
        //public static void WriteCookie(string strName, string strValue, int expires)
        //{
        //	HttpCookie httpCookie = HttpContextExtension.Current.Request.Cookies[strName];
        //	if (httpCookie == null)
        //	{
        //		httpCookie = new HttpCookie(strName);
        //	}
        //	httpCookie.Value = strValue;
        //	httpCookie.Expires = DateTime.Now.AddMinutes((double)expires);
        //	HttpContextExtension.Current.Response.AppendCookie(httpCookie);
        //}

        ///// <summary>
        ///// 添加Cookie缓存到客户端
        ///// </summary>
        ///// <param name="strName">键值名</param>
        ///// <param name="key">键值名</param>
        ///// <param name="strValue">键值</param>
        //public static void WriteCookie(string strName, string key, string strValue)
        //{
        //	HttpCookie httpCookie = HttpContextExtension.Current.Request.Cookies[strName];
        //	if (httpCookie == null)
        //	{
        //		httpCookie = new HttpCookie(strName);
        //	}
        //	httpCookie[key] = strValue;
        //	HttpContextExtension.Current.Response.AppendCookie(httpCookie);
        //}

        /// <summary>
        /// 获取Cookie中的值
        /// </summary>
        /// <param name="strName">键值名</param>
        /// <returns>返回值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContextExtension.Current.Request.Cookies != null && HttpContextExtension.Current.Request.Cookies[strName] != null)
            {
                return HttpContextExtension.Current.Request.Cookies[strName].ToString();
            }
            return "";
        }

        ///// <summary>
        ///// 获取Cookie中的值
        ///// </summary>
        ///// <param name="strName">键值名</param>
        ///// <param name="key">键值名</param>
        ///// <returns>返回值</returns>
        //public static string GetCookie(string strName, string key)
        //{
        //	if (HttpContextExtension.Current.Request.Cookies != null && HttpContextExtension.Current.Request.Cookies[strName] != null && HttpContextExtension.Current.Request.Cookies[strName][key] != null)
        //	{
        //		return HttpContextExtension.Current.Request.Cookies[strName][key].ToString();
        //	}
        //	return "";
        //}

        ///// <summary>
        ///// 当前路径
        ///// </summary>
        //public static string CurrentPath
        //{
        //	get
        //	{
        //		if (HttpContext.Current == null)
        //		{
        //			return string.Empty;
        //		}
        //		string text = HttpContext.Current.Request.Path;
        //		text = text.Substring(0, text.LastIndexOf("/"));
        //		if (text == "/")
        //		{
        //			return string.Empty;
        //		}
        //		return text;
        //	}
        //}

        ///// <summary>
        ///// 获取有关当前请求的 URL 的信息。
        ///// </summary>
        //public static string CurrentUrl
        //{
        //	get
        //	{
        //		return GameRequest.GetUrl();
        //	}
        //}

        ///// <summary>
        ///// 应用程序日志目录 (AppSettings 名 = AppLogDirectory)
        ///// </summary>
        //public static string GetAppLogDirectory
        //{
        //	get
        //	{
        //		string text = AppSettings.Get("AppLogDirectory");
        //		if (string.IsNullOrEmpty(text))
        //		{
        //			text = "AppLog";
        //		}
        //		text = TextUtility.GetFullPath(text);
        //		if (!Directory.Exists(text))
        //		{
        //			Directory.CreateDirectory(text);
        //		}
        //		return text;
        //	}
        //}

        ///// <summary>
        ///// IPDB文件路径  (AppSettings 名 = IPDBFilePath)
        ///// </summary>
        //public static string GetIPDbFilePath
        //{
        //	get
        //	{
        //		string text = AppSettings.Get("IPDBFilePath");
        //		if (string.IsNullOrEmpty(text))
        //		{
        //			return "/Config/IPData.dat";
        //		}
        //		return text;
        //	}
        //}

        ///// <summary>
        ///// 获取应用程序日志 (AppSettings 名 = WriteAppLog)
        ///// </summary>
        //public static bool GetWriteAppLog
        //{
        //	get
        //	{
        //		bool result = false;
        //		string value = AppSettings.Get("WriteAppLog");
        //		if (!string.IsNullOrEmpty(value))
        //		{
        //			result = System.Convert.ToBoolean(value);
        //		}
        //		return result;
        //	}
        //}

        ///// <summary>
        ///// 获取当前请求的原始 URL。
        ///// </summary>
        //public static string RawUrl
        //{
        //	get
        //	{
        //		return GameRequest.GetRawUrl();
        //	}
        //}

        ///// <summary>
        ///// 获取有关客户端上次请求的 URL 的信息，该请求链接到当前的 URL。
        ///// </summary>
        //public static string Referrer
        //{
        //	get
        //	{
        //		return GameRequest.GetUrlReferrer();
        //	}
        //}

        ///// <summary>
        ///// 获取服务器域
        ///// </summary>
        //public static string ServerDomain
        //{
        //	get
        //	{
        //		return GameRequest.GetServerDomain();
        //	}
        //}

        ///// <summary>
        ///// 获取用户浏览器信息
        ///// </summary>
        //public static string UserBrowser
        //{
        //	get
        //	{
        //		return GameRequest.GetUserBrowser();
        //	}
        //}

        ///// <summary>
        ///// 获取客户端访问者IP
        ///// </summary>
        //public static string UserIP
        //{
        //	get
        //	{
        //		return GameRequest.GetUserIP();
        //	}
        //}

        ///// <summary>
        ///// 检索指定枚举中常数值的数组。
        ///// </summary>
        ///// <param name="enumType">表示类型声明：类类型、接口类型、数组类型、值类型、枚举类型、类型参数、泛型类型定义，以及开放或封闭构造的泛型类型。</param>
        ///// <returns>返回常数值的数组</returns>
        //public static IList EnumToList(Type enumType)
        //{
        //	ArrayList arrayList = new ArrayList();
        //	foreach (int num in Enum.GetValues(enumType))
        //	{
        //		ListItem value = new ListItem(Enum.GetName(enumType, num), num.ToString());
        //		arrayList.Add(value);
        //	}
        //	return arrayList;
        //}

        /// <summary>
        /// 汇编版本
        /// </summary>
        public const string ASSEMBLY_VERSION = "3.1.0";

#if NET7_0_OR_GREATER
        [GeneratedRegex("</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase, "zh-CN")]
        private static partial Regex GetTextFromHTMLRegex();
        [GeneratedRegex("[g-zG-Z]")]
        private static partial Regex ToColorRegex();
#else
        private static Regex GetTextFromHTMLRegex() => new("</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase);

        private static Regex ToColorRegex() => new("[g-zG-Z]");
#endif


    }
}
