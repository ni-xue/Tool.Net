using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Tool.Utils;
using Tool.Utils.Encryption;
using Tool.Utils.Other;

namespace Tool //万能属性公有父类
{
    /// <summary>
    /// 对string类进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class StringExtension
    {

        static StringExtension()
        {

        }

        #region String 封装方法

        /// <summary>
        /// 将字符串进行SHA256加密
        /// </summary>
        /// <param name="sha256_txt">String</param>
        /// <returns>返回SHA256</returns>
        public static string SHA256(this string sha256_txt)
        {
            return TextEncrypt.SHA256(sha256_txt);
        }

        /// <summary>
        /// 将字符串进行SHA1加密
        /// </summary>
        /// <param name="sha_txt">String</param>
        /// <returns>返回SHA1</returns>
        public static string SHA1(this string sha_txt)
        {
            return TextEncrypt.SHA1EncryptPassword(sha_txt);
        }

        /// <summary>
        /// 获取加密的MD5，大写的（特意备注：这里是指将你输入的字符串加密成MD5后转为大写，有些傻子以为是先将字符串转大写后加密）
        /// </summary>
        /// <param name="md5_txt">String</param>
        /// <param name="IsMD5">表示加密为：32位或者16位（默认32位）</param>
        /// <returns>返回MD5</returns>
        public static string MD5Upper(this string md5_txt, bool IsMD5 = true)
        {
            if (string.IsNullOrWhiteSpace(md5_txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }

            //MD5加密
            return TextEncrypt.MD5EncryptPassword(md5_txt, IsMD5 ? MD5ResultMode.Strong : MD5ResultMode.Weak);
            //var md5 = System.Security.Cryptography.MD5.Create();
            //if (!IsMD5)
            //{
            //    return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(md5_txt)), 4, 8).Replace("-", "").ToUpper();
            //}
            //var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(md5_txt));
            //var sb = new StringBuilder();
            //foreach (byte b in bs)
            //{
            //    sb.Append(b.ToString("x2"));
            //}
            ////所有字符转为大写
            //return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 获取加密的MD5，小写的（特意备注：这里是指将你输入的字符串加密成MD5后转为小写，有些傻子以为是先将字符串转小写后加密）
        /// </summary>
        /// <param name="md5_txt">String</param>
        /// <param name="IsMD5">表示加密为：32位或者16位（默认32位）</param>
        /// <returns>返回MD5</returns>
        public static string MD5Lower(this string md5_txt, bool IsMD5 = true)
        {
            //MD5加密
            return md5_txt.MD5Upper(IsMD5).ToLower();
            //var md5 = System.Security.Cryptography.MD5.Create();
            //if (!IsMD5)
            //{
            //    return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(md5_txt)), 4, 8).Replace("-", "").ToLower();
            //}
            //var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(md5_txt));
            //var sb = new StringBuilder();
            //foreach (byte b in bs)
            //{
            //    sb.Append(b.ToString("x2"));
            //}
            ////所有字符转为小写
            //return sb.ToString().ToLower();
        }

        /// <summary>
        /// 返回的字符串数组包含此字符串中的子字符串（由指定字符串数组的元素分隔）。 参数指定是否返回空数组元素。
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="separator">指定字符串数组的元素分隔</param>
        /// <returns>返回一个数组</returns>
        public static string[] Split(this string txt, params string[] separator)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            return txt.Split(separator, StringSplitOptions.None);
        }

        /// <summary>
        /// 对吧字符串是否相同，注明：不区分大小写的。
        /// </summary>
        /// <param name="txt">字符串</param>
        /// <param name="txt1">对比字符串</param>
        /// <returns>返回bool类型</returns>
        public static bool EqualsNotCase(this string txt, string txt1)
        {
            return string.Equals(txt, txt1, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回Int类型
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>返回Int类型</returns>
        public static int ToInt(this string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (int.TryParse(txt, out int time))
            {
                return time; //int.Parse(txt);
            }
            else
            {
                throw new System.SystemException("该字符串不是可转换的Int格式！");
            }
        }

        /// <summary>
        /// 返回Int类型
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>返回Int类型</returns>
        public static long ToLong(this string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (long.TryParse(txt, out long time))
            {
                return time; //long.Parse(txt);
            }
            else
            {
                throw new System.SystemException("该字符串不是可转换的Long格式！");
            }
        }

        /// <summary>
        /// 返回时间类型
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>返回时间类型</returns>
        public static DateTime ToDateTime(this string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (DateTime.TryParse(txt, out DateTime time))
            {
                return time; //DateTime.Parse(txt);
            }
            else
            {
                throw new System.SystemException("该字符串不是可转换的时间格式！");
            }
        }

        /// <summary>
        /// 返回十进制数
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>返回十进制数</returns>
        public static decimal ToDecimal(this string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (decimal.TryParse(txt, out decimal de))
            {
                return de; //decimal.Parse(txt);
            }
            else
            {
                throw new System.SystemException("该字符串不是可转换的十进制数格式！");
            }
        }

        /// <summary>
        /// 返回双精度浮点数
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>返回双精度浮点数</returns>
        public static double ToDouble(this string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (double.TryParse(txt, out double de))
            {
                return de; //double.Parse(txt);
            }
            else
            {
                throw new System.SystemException("该字符串不是可转换的双精度浮点数格式！");
            }
        }

        /// <summary>
        /// 返回二进制流
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>返回二进制流</returns>
        public static byte[] ToBytes(this string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            return Encoding.Default.GetBytes(txt);
        }

        /// <summary>
        /// 返回二进制流
        /// </summary>
        /// <param name="txt">String</param>
        /// <param name="encoding">指定格式</param>
        /// <returns>返回二进制流</returns>
        public static byte[] ToBytes(this string txt, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new SystemException("该字符串不存在任何内容！");
            }
            return encoding.GetBytes(txt);
        }

        #region JSON

        /// <summary>
        /// 转换成虚构实体对象
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>转换成虚构实体对象</returns>
        public static dynamic JsonDynamic(this string txt)
        {
            return string.IsNullOrWhiteSpace(txt)
                ? throw new SystemException("该字符串不存在任何内容！")
                : GetObj(txt);//string json = System.Text.Json.JsonSerializer.Serialize<object>(new { i });

            static dynamic GetObj(string txt) 
            {
                JsonDocument jsonDocument = JsonDocument.Parse(txt);
                using (jsonDocument)
                {
                    return JsonHelper.GetReturn(jsonDocument.RootElement);
                }
            }
        }

        /// <summary>
        /// 一种获取 Json 格式数据的实现
        /// </summary>
        /// <param name="txt">Json 格式字符串</param>
        /// <returns>转换成特殊结构对象，用于获取值</returns>
        public static JsonVar JsonVar(this string txt) 
        {
            return new(txt.JsonDynamic());
        }

        /// <summary>
        /// 转换成Dictionary对象
        /// </summary>
        /// <param name="txt">String</param>
        /// <returns>转换成Dictionary对象</returns>
        public static Dictionary<string, object> Json(this string txt)
        {
            //JavaScriptSerializer js = new JavaScriptSerializer();
            return string.IsNullOrWhiteSpace(txt)
                ? throw new SystemException("该字符串不存在任何内容！")
                : GetObj(txt);

            static Dictionary<string, object> GetObj(string txt)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(txt, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });

                using (jsonDocument)
                {
                    JsonElement RootElement = jsonDocument.RootElement;
                    if (RootElement.ValueKind == JsonValueKind.Array)
                    {
                        throw new SystemException("输入的Json字符串为数组格式无法转换成键值对！");
                    }

                    return (Dictionary<string, object>)JsonHelper.GetReturn(RootElement);
                }

                //Dictionary<string, JsonDocument> pairs = JsonSerializer.Deserialize<Dictionary<string, JsonDocument>>(txt);

                //Dictionary<string, object> keyValues = new();
                //if (pairs.Count > 0)
                //{
                //    foreach (KeyValuePair<string, JsonDocument> item in pairs)
                //    {
                //        using (item.Value)
                //        {
                //            keyValues.Add(item.Key, GetReturn(item.Value.RootElement));
                //        }
                //    }
                //    pairs.Clear();
                //}

                //return keyValues;
            }
        }

        /// <summary>
        /// 转换成实体类
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="txt">String</param>
        /// <returns>转换成实体类</returns>
        public static T Json<T>(this string txt)
        {
            return txt.Json<T>(null);
        }

        /// <summary>
        /// 转换成实体类
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="txt">String</param>
        /// <param name="jsonSerializerOptions">需要的序列化条件</param>
        /// <returns>转换成实体类</returns>
        public static T Json<T>(this string txt, JsonSerializerOptions jsonSerializerOptions)
        {
            return string.IsNullOrWhiteSpace(txt)
                ? throw new SystemException("该字符串不存在任何内容！")
                : JsonSerializer.Deserialize<T>(txt, jsonSerializerOptions);
        }

        /// <summary>
        /// 转换成实体数组
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="txt">String</param>
        /// <returns>转换成实体数组</returns>
        public static List<T> JsonList<T>(this string txt)
        {
            return txt.JsonList<T>(null);
        }

        /// <summary>
        /// 转换成实体数组
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="txt">String</param>
        /// <param name="jsonSerializerOptions">需要的序列化条件</param>
        /// <returns>转换成实体数组</returns>
        public static List<T> JsonList<T>(this string txt, JsonSerializerOptions jsonSerializerOptions)
        {
            return string.IsNullOrWhiteSpace(txt)
                ? throw new SystemException("该字符串不存在任何内容！")
                : JsonSerializer.Deserialize<List<T>>(txt, jsonSerializerOptions);
        }

        #endregion

        #region XML

        /// <summary>
        /// 将Xml格式字符串转换为对象
        /// </summary>
        /// <param name="txt">要转类型XML字符串</param>
        /// <returns>对象</returns>
        public static T Xml<T>(this string txt) where T : class
        {
            //using System.IO.StringWriter sw = new System.IO.StringWriter();
            //Type t = obj.GetType();
            //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            //serializer.Serialize(sw, obj);
            //sw.Close();
            //return sw.ToString();
            
            using StringReader sr = new(txt);
            XmlSerializer serializer = new(typeof(T));
            return (T)serializer.Deserialize(sr);
        }

        #endregion

        /// <summary>
        /// 使用指定的编码对象对 string 字符串进行编码。
        /// </summary>
        /// <param name="txt">string</param>
        /// <returns>返回编码结果</returns>
        public static string StringEncode(this string txt)
        {
            return txt.StringEncode(Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定的编码对象对 string 字符串进行编码。
        /// </summary>
        /// <param name="txt">string</param>
        /// <param name="encoding">指定编码格式</param>
        /// <returns>返回编码结果</returns>
        public static string StringEncode(this string txt, Encoding encoding)
        {
            return System.Web.HttpUtility.UrlEncode(txt, encoding);
        }

        /// <summary>
        /// 使用指定的编码对象对 string 字符串进行解码。
        /// </summary>
        /// <param name="txt">string</param>
        /// <returns>返回解码结果</returns>
        public static string StringDecode(this string txt)
        {
            return txt.StringDecode(Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定的编码对象对 string 字符串进行解码。
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="encoding">指定编码格式</param>
        /// <returns>返回解码结果</returns>
        public static string StringDecode(this string txt, Encoding encoding)
        {
            return System.Web.HttpUtility.UrlDecode(txt, encoding);
        }

        /// <summary>
        /// 对当前字符串进行脏字验证
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="texts">自定义脏字数组</param>
        /// <returns>存在返回，true否则为false</returns>
        public static bool DirtyContainsAny(this string text, params string[] texts)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (texts.Length == 0)
            {
                throw new System.SystemException("自定义脏字数组不能为空");
            }
            List<string> keywords = texts.ToList();
            KeywordSearch ks = new KeywordSearch(keywords);
            return ks.Contains(text);
        }

        /// <summary>
        /// 对当前字符串进行脏字验证
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="dirty">自定义脏字字符串以“|”分割</param>
        /// <returns>存在返回，true否则为false</returns>
        public static bool DirtyContainsAny(this string text, string dirty)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (string.IsNullOrWhiteSpace(dirty))
            {
                throw new System.SystemException("自定义脏字字符串不能为空！");
            }
            string[] keywords = dirty.Split('|');
            if (keywords.Length == 0)
            {
                throw new System.SystemException("无法转换成有效的数组！");
            }
            return DirtyContainsAny(text, keywords);
        }

        /// <summary>
        /// 对当前字符串进行脏字验证，返回存在的所有脏字，及下标
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="texts">自定义脏字数组</param>
        /// <returns>返回存在的所有脏字，及下标</returns>
        public static object DirtyContainsAnys(this string text, params string[] texts)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (texts.Length == 0)
            {
                throw new System.SystemException("自定义脏字数组不能为空");
            }
            List<string> keywords = texts.ToList();
            KeywordSearch ks = new KeywordSearch(keywords);
            var Dirtylist = ks.FindAllKeywords(text);
            return Dirtylist.ToJson();
        }

        /// <summary>
        /// 对当前字符串进行脏字验证，返回存在的所有脏字，及下标
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="dirty">自定义脏字字符串以“|”分割</param>
        /// <returns>返回存在的所有脏字，及下标</returns>
        public static object DirtyContainsAnys(this string text, string dirty)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (string.IsNullOrWhiteSpace(dirty))
            {
                throw new System.SystemException("自定义脏字字符串不能为空！");
            }
            string[] keywords = dirty.Split('|');
            if (keywords.Length == 0)
            {
                throw new System.SystemException("无法转换成有效的数组！");
            }
            return DirtyContainsAnys(text, keywords);
        }

        /// <summary>
        /// 对当前字符串进行脏字检测，并净化
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="texts">自定义脏字数组</param>
        /// <param name="symbol">将存在的脏字替换为？？？</param>
        /// <returns>返回被净化后字符串</returns>
        public static string DirtyDetection(this string text, string[] texts, char symbol)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (texts.Length == 0)
            {
                throw new System.SystemException("自定义脏字数组不能为空");
            }
            List<string> keywords = texts.ToList();
            KeywordSearch ks = new KeywordSearch(keywords);
            return ks.FilterKeywords(text, symbol);
        }

        /// <summary>
        /// 对当前字符串进行脏字检测，并净化
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="texts">自定义脏字数组</param>
        /// <returns>返回被净化后字符串</returns>
        public static string DirtyDetection(this string text, string[] texts)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (texts.Length == 0)
            {
                throw new System.SystemException("自定义脏字数组不能为空");
            }
            return DirtyDetection(text, texts, ' ');
        }

        /// <summary>
        /// 对当前字符串进行脏字检测，并净化
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="dirty">自定义脏字字符串以“|”分割</param>
        /// <returns>返回被净化后字符串</returns>
        public static string DirtyDetection(this string text, string dirty)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (string.IsNullOrWhiteSpace(dirty))
            {
                throw new System.SystemException("自定义脏字字符串不能为空！");
            }
            string[] keywords = dirty.Split('|');
            if (keywords.Length == 0)
            {
                throw new System.SystemException("无法转换成有效的数组！");
            }
            return DirtyDetection(text, keywords, ' ');
        }

        /// <summary>
        /// 对当前字符串进行脏字检测，并净化
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="dirty">自定义脏字字符串以“|”分割</param>
        /// <param name="symbol">将存在的脏字替换为？？？</param>
        /// <returns>返回被净化后字符串</returns>
        public static string DirtyDetection(this string text, string dirty, char symbol)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            if (string.IsNullOrWhiteSpace(dirty))
            {
                throw new System.SystemException("自定义脏字字符串不能为空！");
            }
            string[] keywords = dirty.Split('|');
            if (keywords.Length == 0)
            {
                throw new System.SystemException("无法转换成有效的数组！");
            }
            return DirtyDetection(text, keywords, symbol);
        }

        /// <summary>
        /// 默认包含的（防止注入类型）
        /// </summary>
        public const string sqlStr = "dbo.|.dbo.|break;|case |when |between |then |chr |add |alter |create |net |cmd=|while |count |union |from |use |and |top |or |iframe |script |insert |delete |select |update |exec |char |varchar |mid |drop |declare |commit |rollback | tran|truncate | where | in |cursor |exec |begin |open |xp_|sp_|master |--|0x";

        /// <summary>
        /// 过滤非法关键字（使用自带常量）
        /// </summary>
        /// <param name="keyword">带验证的SQL</param>
        /// <returns></returns>
        public static bool SQLFilter(this string keyword)
        {
            bool flag = true;
            try
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToUpper();
                    string[] sqlStrArr = sqlStr.ToUpper().Split('|');
                    foreach (string strChild in sqlStrArr)
                    {
                        if (keyword.IndexOf(strChild) != -1)
                        {
                            flag = false; break;
                        }
                    }
                }
            }
            catch
            {
                flag = false;
            }
            return (flag);
        }

        /// <summary>
        /// 过滤非法关键字，这个可以按照项目灵活配置
        /// </summary>
        /// <param name="keyword">带验证的SQL</param>
        /// <param name="sqlStr">验证的防注入类型字符串，以“|”区分</param>
        /// <returns></returns>
        public static bool SQLFilter(this string keyword, string sqlStr)
        {
            bool flag = true;
            try
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToUpper();
                    string[] sqlStrArr = sqlStr.ToUpper().Split('|');
                    foreach (string strChild in sqlStrArr)
                    {
                        if (keyword.IndexOf(strChild) != -1)
                        {
                            flag = false; break;
                        }
                    }
                }
            }
            catch
            {
                flag = false;
            }
            return (flag);
        }

        /// <summary>
        /// 过滤非法关键字，这个可以按照项目灵活配置
        /// </summary>
        /// <param name="keyword">带验证的SQL</param>
        /// <param name="sqlStr">验证的防注入类型（必须大写）</param>
        /// <returns></returns>
        public static bool SQLFilter(this string keyword, params string[] sqlStr)
        {
            bool flag = true;
            try
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToUpper();
                    string[] sqlStrArr = sqlStr;
                    foreach (string strChild in sqlStrArr)
                    {
                        if (keyword.IndexOf(strChild) != -1)
                        {
                            flag = false; break;
                        }
                    }
                }
            }
            catch
            {
                flag = false;
            }
            return (flag);
        }


        /// <summary>
        /// 将对Base64字符串换成象转（解码）
        /// </summary>
        /// <param name="value">Base64字符串</param>
        /// <returns>返回一个源对象</returns>
        public static byte[] UnBase64String(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new System.SystemException("该字符串不存在任何内容！");
            }
            return Convert.FromBase64String(value);
        }

        /// <summary>
        /// 将字符串中的中文转成拼音
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>返回完整的拼音</returns>
        public static string ConvertHzToPz_Gb2312(this string value)
        {
            return ConvertHzToPzGb2312.Convert(value);
        }

        /// <summary> 
        /// 获得一个字符串的汉语拼音码 (首字母)
        /// </summary> 
        /// <param name="value">字符串</param> 
        /// <returns>汉语拼音码,该字符串只包含大写的英文字母</returns> 
        public static string StrToPinyin(this string value)
        {
            return Utils.Other.StrToPinyin.GetChineseSpell(value);
        }

        /// <summary>
        /// 是不是数字
        /// </summary>
        /// <param name="strNumber">字符串</param>
        /// <returns>返回bool类型</returns>
        internal static bool IsNumber(this string strNumber)
        {
            return !string.IsNullOrEmpty(strNumber) && new System.Text.RegularExpressions.Regex("^([0-9])[0-9]*(\\.\\w*)?$").IsMatch(strNumber.Trim());
        }

        #endregion


        #region String[] 封装方法

        /// <summary>
        /// 将String数组转成Int数组
        /// </summary>
        /// <param name="txt">String[]</param>
        /// <returns>注释：如出现无法转换的，这抛出异常</returns>
        public static int[] ToInts(this string[] txt)
        {
            List<int> vs = new List<int>();
            foreach (string val in txt)
            {
                vs.Add(val.ToInt());
            }
            return vs.ToArray();

            //return txt.Select(a => System.Text.RegularExpressions.Regex.IsMatch(a, @"^(\-|\+)?\d+(\d+)?$") ? Convert.ToInt32(a) : 0).ToArray();
        }

        // <summary>
        // 该方法为string内部方法。（具体作用我也没有尝试）
        // </summary>
        // <param name="txt">string</param>
        // <param name="length">数组长度</param>
        // <returns>方法内容未知</returns>
        //[System.Security.SecurityCritical]
        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
        //public static extern string FastAllocateString(this string[] txt, int length);

        #region include "Image_Paramid_Argb_Templete.cs"


        #endregion


        /// <summary>
        /// 修改当前数组的某个下标的内容
        /// </summary>
        /// <param name="txts">源数据</param>
        /// <param name="ChangeTxt">修改的内容</param>
        /// <param name="length">下标位置</param>
        public static void Change([In][Out] this string[] txts, string ChangeTxt, int length)
        {
            //string[] _txts = new string[txts.Length + 1];

            //Array.Copy(txts, 0, _txts, 0, txts.Length);

            //_txts[txts.Length] = AddTxt;

            txts.SetValue(ChangeTxt, length);
        }

        ///// <summary>
        ///// 给数组加新的值
        ///// </summary>
        ///// <param name="txt">string[]</param>
        ///// <param name="txt1">新增的值</param>
        ///// <returns>返回Add后的新数组</returns>
        //public static unsafe void Add(this string[] txt, string txt1)
        //{
        //    var txts = txt.ToList();
        //    txts.Add(txt1);

        //    TypedReference typedReference = __makeref(txt); //default(TypedReference);

        //    TypedReference.SetTypedReference(typedReference, txts.ToArray());

        //    //txt.SetValue(txt1, txt.Length - 1);// + 1
        //    //_Add(ref txt,txt1);
        //    //unsafe
        //    //{
        //    //unsafe
        //    //}

        //    //int[] arr1= { 1, 2, 3 };

        //    //string text = txt.FastAllocateString(txt.Length);
        //    //fixed (string* txts = text)
        //    //{
        //    //char* ptr = txts;

        //    //}

        //    //int*[] arr = &arr1;//stackalloc int[arr1.Length];

        //    //string*[] d1 = ->txt;

        //    //arr[0]->ToString();

        //    //var last = (void*)txt;

        //    //txt.Initialize();
        //}

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txts">string[]</param>
        /// <param name="_txts">新数组（为空，但是必须大于原数组一个下标以上）</param>
        /// <param name="txt">新增的值</param>
        public static void Add(this string[] txts, [In][Out] string[] _txts, string txt)
        {
            if (txts.Length >= _txts.Length)
            {
                throw new System.SystemException("新的数组必须大于当源数组一个下标以上！");
            }

            Array.Copy(txts, 0, _txts, 0, txts.Length);

            _txts[txts.Length] = txt;
        }

        /// <summary>
        /// 查找该string数组中是否存在该值。
        /// </summary>
        /// <param name="txt">string[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this string[] txt, string txt1)
        {
            return txt.Contains<string>(txt1);
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static string[] GetArrayIndex(this string[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该string为空！");
            }
            if (index > count)
            {
                throw new System.SystemException("count不能小于index，数组越界！");
            }
            if (index < 0)
            {
                throw new System.SystemException("index不能小于0，数组越界！");
            }
            if (count < 0)
            {
                throw new System.SystemException("count不能小于0，数组越界！");
            }
            if (obj.Length < index)
            {
                throw new System.SystemException("index超出了数组，数组越界！");
            }
            if (obj.Length < count)
            {
                throw new System.SystemException("count超出了数组，数组越界！");
            }
            List<string> obj1 = new List<string>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }

        /// <summary>
        /// 将文件转换为byte数组（如果没有找到则返回零的数组）
        /// </summary>
        /// <param name="path">文件地址(绝对路径)</param>
        /// <returns>转换后的byte数组（如果没有找到则返回零的数组）</returns>
        public static byte[] ToFileBytes(this string path)
        {
            if (!File.Exists(path))
            {
                return new byte[0];
            }
            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];
            using (FileStream fs = fi.OpenRead())
            {
                fs.Read(buff, 0, fs.Length.ToVar<int>());
            }
            return buff;
        }


        #endregion


        #region String 相关方法，非封装

        /// <summary>
        /// 由连字符分隔的32位随机数
        /// </summary>
        /// <param name="type">返回两个结果，true是去掉分隔符的，false是未去掉的</param>
        /// <returns></returns>
        public static string GetGuid(bool type = true)
        {
            //Guid guid = new Guid();
            Guid guid = Guid.NewGuid();
            if (type)
            {
                //string guidstr = guid.ToString();

                //char[] _guids = new char[32];
                //int a = 0;

                ////StringBuilder builder = new StringBuilder(32);

                //for (int i = 0; i < guidstr.Length; i++)
                //{
                //    if (i == 8 || i == 13 || i == 18 || i == 23) continue;

                //    _guids[a++] = guidstr[i];
                //    //builder.Append(guidstr[i]);
                //}

                //guidstr = string.Concat(guidstr.Substring(0,7),guidstr.Substring(9, 4),guidstr.Substring(14, 4),guidstr.Substring(19, 4),guidstr.Substring(24));
                return guid.ToString("N");// string.Concat(guidstr.Substring(0, 7), guidstr.Substring(9, 4), guidstr.Substring(14, 4), guidstr.Substring(19, 4), guidstr.Substring(24));// string.Concat(_guids); //builder.ToString(); // guid.ToString().Replace("-", "");
            }
            return guid.ToString();
        }

        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= b + 1;

            //byte[] gi = Guid.NewGuid().ToByteArray();
            //for (int a = 0; a < 16; a++)
            //{
            //    i *= gi[a] + 1;
            //}
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>  
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>  
        /// <returns></returns>  
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        #endregion
    }
}
