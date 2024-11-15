using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Tool.Web;

namespace Tool.Utils
{
    /// <summary>
    /// 提供技术支持的类库
    /// </summary>
    public partial class TextUtility
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        private TextUtility()
        {
        }

        /// <summary>
        /// 创建身份验证的秘钥
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CreateAuthStr(int len)
        {
            StringBuilder stringBuilder = new();
            Random random = new();
            for (int i = 0; i < len; i++)
            {
                int num = random.Next();
                if (num % 2 == 0)
                {
                    stringBuilder.Append((char)(48 + (ushort)(num % 10)));
                }
                else
                {
                    stringBuilder.Append((char)(65 + (ushort)(num % 26)));
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 创建身份验证的秘钥
        /// </summary>
        /// <param name="len">长度</param>
        /// <param name="onlyNum">复杂强度</param>
        /// <returns></returns>
        public static string CreateAuthStr(int len, bool onlyNum)
        {
            if (!onlyNum)
            {
                return TextUtility.CreateAuthStr(len);
            }
            StringBuilder stringBuilder = new();
            Random random = new();
            for (int i = 0; i < len; i++)
            {
                int num = random.Next();
                stringBuilder.Append((char)(48 + (ushort)(num % 10)));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 创建身份验证的秘钥
        /// </summary>
        /// <param name="length"></param>
        /// <param name="isuseNum">带数字</param>
        /// <param name="isuseLow">带字母小写</param>
        /// <param name="isuseUpp">带字母大写</param>
        /// <param name="isuseSpe">带上其他符号</param>
        /// <param name="custom">自带字符串</param>
        /// <returns></returns>
        public static string CreateRandom(int length, bool isuseNum, bool isuseLow, bool isuseUpp, bool isuseSpe, string custom)
        {
            Random random = new(TextUtility.GetNewSeed());
            StringBuilder text = new();
            StringBuilder text2 = new(custom);
            if (isuseNum)
            {
                text2.Append("0123456789");
            }
            if (isuseLow)
            {
                text2.Append("abcdefghijklmnopqrstuvwxyz");
            }
            if (isuseUpp)
            {
                text2.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }
            if (isuseSpe)
            {
                text2.Append("!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~");
            }
            for (int i = 0; i < length; i++)
            {
                text.Append(text2[random.Next(0, text2.Length - 1)]);
            }
            return text.ToString();
        }

        /// <summary>
        /// 创建随机小写
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CreateRandomLowercase(int len)
        {
            StringBuilder stringBuilder = new();
            Random random = new();
            for (int i = 0; i < len; i++)
            {
                int num = random.Next();
                stringBuilder.Append((char)(97 + (ushort)(num % 26)));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CreateRandomNum(int len)
        {
            StringBuilder stringBuilder = new();
            Random random = new((int)DateTime.Now.Ticks);
            for (int i = 0; i < len; i++)
            {
                int num = random.Next();
                stringBuilder.Append((char)(48 + (ushort)(num % 10)));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CreateRandomNum2(int len)
        {
            StringBuilder stringBuilder = new();
            Random random = new(TextUtility.GetNewSeed());
            for (int i = 0; i < len; i++)
            {
                int num = random.Next();
                stringBuilder.Append((char)(48 + (ushort)(num % 10)));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 获取新种子
        /// </summary>
        /// <returns></returns>
        public static int GetNewSeed()
        {
            Span<byte> array = stackalloc byte[4];
            TextUtility.rng.GetBytes(array);
            return BitConverter.ToInt32(array);
        }

        /// <summary>
        /// 创建临时密码
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string CreateTemporaryPassword(int length)
        {
            string text = Guid.NewGuid().ToString("N");
            for (int i = 0; i < length / 32; i++)
            {
                text += Guid.NewGuid().ToString("N");
            }
            return text.Substring(0, length);
        }

        /// <summary>
        /// 将字符串从左边开始删除
        /// </summary>
        /// <param name="originalVal">字符串</param>
        /// <param name="cutLength">从左开始删除几位</param>
        /// <returns></returns>
        public static string CutLeft(string originalVal, int cutLength)
        {
            if (string.IsNullOrEmpty(originalVal))
            {
                return string.Empty;
            }
            if (cutLength < 1)
            {
                return originalVal;
            }
            byte[] bytes = Encoding.Default.GetBytes(originalVal);
            if (bytes.Length <= cutLength)
            {
                return originalVal;
            }
            int num = cutLength;
            int[] array = new int[cutLength];
            int num2 = 0;
            for (int i = 0; i < cutLength; i++)
            {
                if (bytes[i] > 127)
                {
                    num2++;
                    if (num2 == 3)
                    {
                        num2 = 1;
                    }
                }
                else
                {
                    num2 = 0;
                }
                array[i] = num2;
            }
            if (bytes[cutLength - 1] > 127 && array[cutLength - 1] == 1)
            {
                num = cutLength + 1;
            }
            byte[] array2 = new byte[num];
            Array.Copy(bytes, array2, num);
            return Encoding.Default.GetString(array2);
        }

        /// <summary>
        /// 将字符串从右边开始删除
        /// </summary>
        /// <param name="originalVal">字符串</param>
        /// <param name="cutLength">从右开始删除几位</param>
        /// <returns></returns>
        public static string CutRight(string originalVal, int cutLength)
        {
            if (cutLength < 0)
            {
                cutLength = Math.Abs(cutLength);
            }
            if (originalVal.Length <= cutLength)
            {
                return originalVal;
            }
            return originalVal.Substring(originalVal.Length - cutLength);
        }

        /// <summary>
        /// 减少字符串数量
        /// </summary>
        /// <param name="originalVal">字符串</param>
        /// <param name="startIndex">从第几位开始</param>
        /// <returns></returns>
        public static string CutString(string originalVal, int startIndex)
        {
            return TextUtility.CutString(originalVal, startIndex, originalVal.Length);
        }

        /// <summary>
        /// 减少字符串数量
        /// </summary>
        /// <param name="originalVal">字符串</param>
        /// <param name="startIndex">从第几位开始</param>
        /// <param name="cutLength">到第几位结束</param>
        /// <returns></returns>
        public static string CutString(string originalVal, int startIndex, int cutLength)
        {
            if (startIndex >= 0)
            {
                if (cutLength < 0)
                {
                    cutLength *= -1;
                    if (startIndex - cutLength < 0)
                    {
                        cutLength = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex -= cutLength;
                    }
                }
                if (startIndex > originalVal.Length)
                {
                    return "";
                }
            }
            else
            {
                if (cutLength < 0 || cutLength + startIndex <= 0)
                {
                    return "";
                }
                cutLength += startIndex;
                startIndex = 0;
            }
            if (originalVal.Length - startIndex < cutLength)
            {
                cutLength = originalVal.Length - startIndex;
            }
            string result;
            try
            {
                result = originalVal.Substring(startIndex, cutLength);
            }
            catch
            {
                result = originalVal;
            }
            return result;
        }

        /// <summary>
        /// 切割字符串
        /// </summary>
        /// <param name="originalVal">字符串</param>
        /// <param name="cutLength">去掉几位</param>
        /// <returns></returns>
        public static string CutStringProlongSymbol(string originalVal, int cutLength)
        {
            if (originalVal.Length <= cutLength)
            {
                return originalVal;
            }
            return TextUtility.CutLeft(originalVal, cutLength) + TextUtility.PROLONG_SYMBOL;
        }

        /// <summary>
        /// 切割字符串加上延长符号
        /// </summary>
        /// <param name="originalVal">字符串</param>
        /// <param name="cutLength">去掉几位</param>
        /// <param name="prolongSymbol">延长符号</param>
        /// <returns></returns>
        public static string CutStringProlongSymbol(string originalVal, int cutLength, string prolongSymbol)
        {
            if (string.IsNullOrEmpty(prolongSymbol))
            {
                prolongSymbol = TextUtility.PROLONG_SYMBOL;
            }
            return TextUtility.CutLeft(originalVal, cutLength) + prolongSymbol;
        }

        /// <summary>
        /// 减少字符串标题
        /// </summary>
        /// <param name="content">字符串标题</param>
        /// <param name="cutLength">减少几位</param>
        /// <returns></returns>
        public static string CutStringTitle(object content, int cutLength)
        {
            string text = CutStringTitleRegex().Replace(content.ToString(), "");
            if (text.Length > cutLength && text.Length > 2)
            {
                text = string.Concat(text.AsSpan(0, cutLength - 2), "...");
            }
            if (text.IndexOf('<') > -1)
            {
                text = text.Remove(text.LastIndexOf('<'), text.Length - text.LastIndexOf('<'));
            }
            return text;
        }

        /// <summary>
        /// 验证字符串是否为空，或是NULl，""，空格
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool EmptyTrimOrNull(string text)
        {
            return text == null || text.Trim().Length == 0;
        }

        /// <summary>
        /// 格式的IP
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="fields">选择格式类型</param>
        /// <returns></returns>
        public static string FormatIP(string ip, int fields)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return "(未记录)";
            }
            if (fields > 3)
            {
                return ip;
            }
            if (ip.Contains(':'))
            {
                return "(不支持ipv6)";
            }
            string[] array = ip.Split(new char[]
            {
                '.'
            });
            if (array.Length != 4)
            {
                return "(未记录)";
            }
            if (fields == 3)
            {
                return string.Concat(new string[]
                {
                    array[0],
                    ".",
                    array[1],
                    ".",
                    array[2],
                    ".*"
                });
            }
            if (fields == 2)
            {
                return array[0] + "." + array[1] + ".*.*";
            }
            if (fields == 1)
            {
                return array[0] + ".*.*.*";
            }
            return "*.*.*.*";
        }

        /// <summary>
        /// 电子邮件编码
        /// </summary>
        /// <param name="originalStr">电子邮件字符串</param>
        /// <returns></returns>
        public static string EmailEncode(string originalStr)
        {
            string text = TextUtility.TextEncode(originalStr).Replace("@", "&#64;").Replace(".", "&#46;");
            return TextUtility.JoinString("<a href='mailto:", new string[]
            {
                text,
                "'>",
                text,
                "</a>"
            });
        }

        /// <summary>
        /// 获取电子邮件主机名
        /// </summary>
        /// <param name="strEmail">电子邮件字符串</param>
        /// <returns></returns>
        public static string GetEmailHostName(string strEmail)
        {
            if (string.IsNullOrEmpty(strEmail) || strEmail.IndexOf("@") < 0)
            {
                return string.Empty;
            }
            return strEmail.Substring(strEmail.LastIndexOf("@") + 1).ToLower();
        }

        /// <summary>
        /// 设置货币格式
        /// </summary>
        /// <param name="money">货币数量</param>
        /// <returns></returns>
        public static string FormatMoney(decimal money)
        {
            return money.ToString("0.00");
        }

        /// <summary>
        /// 日期和时间差异
        /// </summary>
        /// <param name="todate">开始日期</param>
        /// <param name="fodate">结束日期</param>
        public static string[] DiffDateAndTime(object todate, object fodate)
        {
            string[] array = new string[2];
            double value = (DateTime.Parse(todate.ToString()) - DateTime.Parse(fodate.ToString())).TotalSeconds / 86400.0;
            value.ToString();
            int length = value.ToString().Length;
            int num = value.ToString().LastIndexOf(".");
            int num2 = (int)Math.Round(value, 10);
            int num3 = (int)(double.Parse("0" + value.ToString().Substring(num, length - num)) * 24.0);
            array[0] = num2.ToString();
            array[1] = num3.ToString();
            return array;
        }

        /// <summary>
        /// 日期和时间差异
        /// </summary>
        /// <param name="todate">开始日期</param>
        /// <param name="fodate">结束日期</param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="v5"></param>
        /// <param name="v6"></param>
        /// <returns></returns>
        public static string DiffDateAndTime(object todate, object fodate, string v1, string v2, string v3, string v4, string v5, string v6)
        {
            TimeSpan timeSpan = DateTime.Parse(todate.ToString()) - DateTime.Parse(fodate.ToString());
            int num = (int)timeSpan.TotalDays / 365;
            int num2 = (int)((timeSpan.TotalDays / 365.0 - (double)((int)(timeSpan.TotalDays / 365.0))) * 12.0);
            int num3 = timeSpan.Days - num * 365 - num2 * 30;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            string text = "";
            if (num != 0)
            {
                text = text + num.ToString() + v1;
            }
            if (num2 != 0)
            {
                text = text + num2.ToString() + v2;
            }
            if (num3 != 0)
            {
                text = text + num3.ToString() + v3;
            }
            if (hours != 0)
            {
                text = text + hours.ToString() + v4;
            }
            if (minutes != 0)
            {
                text = text + minutes.ToString() + v5;
            }
            if (num == 0 && num2 == 0 && num3 == 0 && hours == 0 && minutes == 0)
            {
                return v6;
            }
            return text;
        }

        /// <summary>
        /// 差异日期天数
        /// </summary>
        /// <param name="oneDateTime">时间对象</param>
        /// <returns></returns>
        public static int DiffDateDays(DateTime oneDateTime)
        {
            TimeSpan timeSpan = DateTime.Now - oneDateTime;
            if (timeSpan.TotalDays > 2147483647.0)
            {
                return 2147483647;
            }
            if (timeSpan.TotalSeconds < -2147483648.0)
            {
                return -2147483648;
            }
            return (int)timeSpan.TotalDays;
        }

        /// <summary>
        /// 差异日期天数
        /// </summary>
        /// <param name="oneDateTime">时间字符串</param>
        /// <returns></returns>
        public static int DiffDateDays(string oneDateTime)
        {
            if (string.IsNullOrEmpty(oneDateTime))
            {
                return 0;
            }
            return TextUtility.DiffDateDays(DateTime.Parse(oneDateTime));
        }

        /// <summary>
        /// 根据时间对象返回字符串的大概信息（例如：1年前，1月前等等）
        /// </summary>
        /// <param name="dateSpan">时间对象</param>
        /// <returns></returns>
        public static string FormatDateSpan(object dateSpan)
        {
            DateTime d = (DateTime)dateSpan;
            TimeSpan timeSpan = DateTime.Now - d;
            if (timeSpan.TotalDays >= 365.0)
            {
                return string.Format("{0} 年前", (int)(timeSpan.TotalDays / 365.0));
            }
            if (timeSpan.TotalDays >= 30.0)
            {
                return string.Format("{0} 月前", (int)(timeSpan.TotalDays / 30.0));
            }
            if (timeSpan.TotalDays >= 7.0)
            {
                return string.Format("{0} 周前", (int)(timeSpan.TotalDays / 7.0));
            }
            if (timeSpan.TotalDays >= 1.0)
            {
                return string.Format("{0} 天前", (int)timeSpan.TotalDays);
            }
            if (timeSpan.TotalHours >= 1.0)
            {
                return string.Format("{0} 小时前", (int)timeSpan.TotalHours);
            }
            if (timeSpan.TotalMinutes >= 1.0)
            {
                return string.Format("{0} 分钟前", (int)timeSpan.TotalMinutes);
            }
            return "1 分钟前";
        }

        /// <summary>
        /// 几种时间格式
        /// </summary>
        /// <param name="oneDateVal">时间对象</param>
        /// <param name="formatType">根据ID返回指定的类型</param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime oneDateVal, int formatType)
        {
            double value = 0.0;
            DateTime dateTime = oneDateVal.AddHours(value);
            switch (formatType)
            {
                case 2:
                    return dateTime.ToShortDateString();
                case 3:
                    return dateTime.ToString("yyyy年MM月dd日 HH点mm分ss秒");
                case 4:
                    return dateTime.ToString("yyyy年MM月dd日");
                case 5:
                    return dateTime.ToString("yyyy年MM月dd日 HH点mm分");
                case 6:
                    return dateTime.ToString("yyyy-MM-dd HH:mm");
                case 7:
                    return dateTime.ToString("yy年MM月dd日 HH点mm分");
                default:
                    return dateTime.ToString();
            }
        }

        /// <summary>
        /// 格式日期时间
        /// </summary>
        /// <param name="oneDateVal">一个日期值</param>
        /// <param name="formatType">格式日期</param>
        /// <returns></returns>
        public static string FormatDateTime(string oneDateVal, int formatType)
        {
            return TextUtility.FormatDateTime(DateTime.Parse(oneDateVal), formatType);
        }

        /// <summary>
        /// 根据时间戳返回 天时分秒
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string FormatSecondSpan(long second)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((double)second);
            string text;
            if (timeSpan.Days > 0)
            {
                text = timeSpan.Days.ToString() + "天";
            }
            else
            {
                text = string.Empty;
            }
            if (timeSpan.Hours > 0)
            {
                text = text + timeSpan.Hours.ToString() + "时";
            }
            if (timeSpan.Minutes > 0)
            {
                text = text + timeSpan.Minutes.ToString() + "分";
            }
            if (timeSpan.Seconds > 0)
            {
                text = text + timeSpan.Seconds.ToString() + "秒";
            }
            return text;
        }

        /// <summary>
        /// 获取当前日期时间长字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeLongString()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyyMMddHHmmss") + now.Millisecond.ToString("000");
        }

        /// <summary>
        /// 获取当前日期时间长字符串
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string GetDateTimeLongString(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = string.Empty;
            }
            return prefix + TextUtility.GetDateTimeLongString();
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="originalVal">原始字符串</param>
        /// <param name="lastStr">追加字符串</param>
        /// <returns></returns>
        public static string AddLast(string originalVal, string lastStr)
        {
            if (originalVal.EndsWith(lastStr))
            {
                return originalVal;
            }
            return originalVal + lastStr;
        }

        /// <summary>
        /// 根据当前输入的相对于项目的路径返回绝对路径
        /// </summary>
        /// <param name="strPath">相对路径</param>
        /// <returns></returns>
        public static string GetFullPath(string strPath)
        {
            string text = TextUtility.AddLast(AppDomain.CurrentDomain.BaseDirectory, "\\");
            if (strPath.IndexOf(":") < 0)
            {
                string text2 = strPath.Replace("..\\", "");
                if (text2 != strPath)
                {
                    int num = (strPath.Length - text2.Length) / "..\\".Length + 1;
                    for (int i = 0; i < num; i++)
                    {
                        text = text.Substring(0, text.LastIndexOf("\\"));
                    }
                    text2 = "\\" + text2;
                }
                strPath = text + text2;
            }
            return strPath;
        }

        ///// <summary>
        ///// 获取映射路径
        ///// </summary>
        ///// <param name="folderPath">相对路径</param>
        ///// <returns></returns>
        //public static string GetMapPath(string folderPath)
        //{
        //	if (folderPath.IndexOf(":\\") > 0)
        //	{
        //		return TextUtility.AddLast(folderPath, "\\");
        //	}
        //	if (folderPath.StartsWith("~/"))
        //	{
        //		return TextUtility.AddLast(HttpContextExtension.Current.Server.MapPath(folderPath), "\\");
        //	}
        //	string str = HttpContextExtension.Current.Request.ApplicationPath + "/";
        //	return TextUtility.AddLast(HttpContextExtension.Current.Server.MapPath(str + folderPath), "\\");
        //}

        ///// <summary>
        ///// 获得绝对的路径
        ///// </summary>
        ///// <param name="strPath">相对路径</param>
        ///// <returns></returns>
        //public static string GetRealPath(string strPath)
        //{
        //	if (string.IsNullOrEmpty(strPath))
        //	{
        //		throw new Exception("strPath 不能为空！");
        //	}
        //	HttpContext httpContext = null;
        //	try
        //	{
        //		httpContext = HttpContext.Current;
        //	}
        //	catch
        //	{
        //		httpContext = null;
        //	}
        //	if (httpContext != null)
        //	{
        //		return httpContext.Server.MapPath(strPath);
        //	}
        //	string text = Path.Combine(strPath, "");
        //	string arg_56_0 = text.StartsWith("\\\\") ? text.Remove(0, 2) : text;
        //	return AppDomain.CurrentDomain.BaseDirectory + Path.Combine(strPath, "");
        //}

        /// <summary>
        /// 验证一个字符串数组里面是否包含一个字符串
        /// </summary>
        /// <param name="matchStr">一个字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <returns>返回状态</returns>
        public static bool InArray(string matchStr, string[] strArray)
        {
            if (!string.IsNullOrEmpty(matchStr))
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (matchStr == strArray[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 验证一个字符串数组里面是否包含一个字符串
        /// </summary>
        /// <param name="matchStr">一个字符串</param>
        /// <param name="originalStr">分割字符</param>
        /// <param name="separator">可以被分割字符分割的字符串</param>
        /// <returns>返回状态</returns>
        public static bool InArray(string matchStr, string originalStr, string separator)
        {
            string[] array = TextUtility.SplitStrArray(originalStr, separator);
            for (int i = 0; i < array.Length; i++)
            {
                if (matchStr == array[i])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 在数组中
        /// </summary>
        /// <param name="matchStr">匹配字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool InArray(string matchStr, string[] strArray, bool ignoreCase)
        {
            return TextUtility.InArrayIndexOf(matchStr, strArray, ignoreCase) >= 0;
        }

        /// <summary>
        /// 在数组中
        /// </summary>
        /// <param name="matchStr">匹配字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <param name="separator">分离器</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static bool InArray(string matchStr, string strArray, string separator, bool ignoreCase)
        {
            return TextUtility.InArray(matchStr, TextUtility.SplitStrArray(strArray, separator), ignoreCase);
        }

        /// <summary>
        /// 在数组索引中
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static int InArrayIndexOf(string originalStr, string[] strArray)
        {
            return TextUtility.InArrayIndexOf(originalStr, strArray, true);
        }

        /// <summary>
        /// 在数组索引中
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static int InArrayIndexOf(string originalStr, string[] strArray, bool ignoreCase)
        {
            for (int i = 0; i < strArray.Length; i++)
            {
                if (ignoreCase)
                {
                    if (originalStr.ToLower() == strArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else if (originalStr == strArray[i])
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 在IP数组中是否包含该IP
        /// </summary>
        /// <param name="ip">验证IP</param>
        /// <param name="ipArray">IP集合</param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] ipArray)
        {
            if (!string.IsNullOrEmpty(ip) && Validate.IsIP(ip))
            {
                string[] array = TextUtility.SplitStrArray(ip, ".");
                for (int i = 0; i < ipArray.Length; i++)
                {
                    string[] array2 = TextUtility.SplitStrArray(ipArray[i], ".");
                    int num = 0;
                    for (int j = 0; j < array2.Length; j++)
                    {
                        if (array2[j] == "*")
                        {
                            return true;
                        }
                        if (array.Length <= j || array2[j] != array[j])
                        {
                            break;
                        }
                        num++;
                    }
                    if (num == 4)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// JavaScript编码
        /// </summary>
        /// <param name="obj">原始字符串</param>
        /// <returns></returns>
        public static string JavaScriptEncode(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return TextUtility.JavaScriptEncode(obj.ToString());
        }

        /// <summary>
        /// JavaScript编码
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <returns></returns>
        public static string JavaScriptEncode(string originalStr)
        {
            if (string.IsNullOrEmpty(originalStr))
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder(originalStr);
            return stringBuilder.Replace("\\", "\\\\").Replace("/", "\\/").Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", "\r").Replace("\r", "\\r").ToString();
        }

        /// <summary>
        /// 等同于Join方法，就是增加了验证
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Join(string separator, params string[] value)
        {
            return TextUtility.JoinString(separator, value);
        }

        /// <summary>
        /// 等同于Join方法，就是增加了验证
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string JoinString(params string[] value)
        {
            return TextUtility.JoinString(string.Empty, value);
        }

        /// <summary>
        /// 等同于Join方法，就是增加了验证
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string JoinString(string separator, params string[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.Length == 0)
            {
                return string.Empty;
            }
            return string.Join(separator, value);
        }

        /// <summary>
        /// 获取字符串长度
        /// </summary>
        /// <param name="originalVal"></param>
        /// <returns></returns>
        public static int Length(string originalVal)
        {
            return Encoding.Default.GetBytes(originalVal).Length;
        }

        /// <summary>
        /// 正则替换标记
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="specialChares">特殊字符</param>
        /// <param name="entityClasses">实体类</param>
        /// <returns></returns>
        public static string RegexReplaceTags(string originalStr, string specialChares, params object[] entityClasses)
        {
            for (int i = 0; i < entityClasses.Length; i++)
            {
                object obj = entityClasses[i];
                PropertyInfo[] properties = obj.GetType().GetProperties();
                for (int j = 0; j < properties.Length; j++)
                {
                    PropertyInfo propertyInfo = properties[j];
                    string name = propertyInfo.Name;
                    string pattern = specialChares + name + specialChares;
                    string replacement = propertyInfo.GetValue(obj, null).ToString();
                    originalStr = Regex.Replace(originalStr, pattern, replacement, RegexOptions.IgnoreCase);
                }
            }
            return originalStr;
        }

        /// <summary>
        /// 感觉有问题，可以自己玩一下
        /// </summary>
        /// <param name="repeatStr"></param>
        /// <param name="repeatCount"></param>
        /// <returns></returns>
        public static string RepeatStr(string repeatStr, int repeatCount)
        {
            StringBuilder stringBuilder = new StringBuilder(repeatCount);
            for (int i = 0; i < repeatCount; i++)
            {
                stringBuilder.Append(repeatStr);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 移除掉字符串中所有的中文
        /// </summary>
        /// <param name="originalVal"></param>
        /// <returns></returns>
        public static string ReplaceCnChar(string originalVal)
        {
            if (string.IsNullOrEmpty(originalVal))
            {
                return string.Empty;
            }
            return ReplaceCnCharRegex().Replace(originalVal, "");
        }

        /// <summary>
        /// 移除字符串中存在的所有符号
        /// </summary>
        /// <param name="originalVal"></param>
        /// <returns></returns>
        public static string ReplaceLuceneSpecialChar(string originalVal)
        {
            if (string.IsNullOrEmpty(originalVal))
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder(originalVal);
            stringBuilder.Replace("+", string.Empty);
            stringBuilder.Replace("-", string.Empty);
            stringBuilder.Replace("&&", string.Empty);
            stringBuilder.Replace("||", string.Empty);
            stringBuilder.Replace("!", string.Empty);
            stringBuilder.Replace("(", string.Empty);
            stringBuilder.Replace(")", string.Empty);
            stringBuilder.Replace("{", string.Empty);
            stringBuilder.Replace("}", string.Empty);
            stringBuilder.Replace("[", string.Empty);
            stringBuilder.Replace("]", string.Empty);
            stringBuilder.Replace("^", string.Empty);
            stringBuilder.Replace("\"", string.Empty);
            stringBuilder.Replace("~", string.Empty);
            stringBuilder.Replace("*", string.Empty);
            stringBuilder.Replace("?", string.Empty);
            stringBuilder.Replace(":", string.Empty);
            stringBuilder.Replace("\\", string.Empty);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 替换字符串使用字符串数组
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="sc">要替换字符串数组</param>
        /// <returns></returns>
        public static string ReplaceStrUseSC(string originalStr, StringCollection sc)
        {
            if (string.IsNullOrEmpty(originalStr))
            {
                return string.Empty;
            }
            foreach (string current in sc)
            {
                originalStr = Regex.Replace(originalStr, current, "*".PadLeft(current.Length, '*'), RegexOptions.IgnoreCase);
            }
            return originalStr;
        }

        /// <summary>
        /// 替换字符串使用字符串数组
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="sc">要替换字符串数组</param>
        /// <returns></returns>
        public static string ReplaceStrUseSC(string originalStr, string[] sc)
        {
            if (string.IsNullOrEmpty(originalStr))
            {
                return string.Empty;
            }
            for (int i = 0; i < sc.Length; i++)
            {
                string text = sc[i];
                originalStr = Regex.Replace(originalStr, text, "*".PadLeft(text.Length, '*'), RegexOptions.IgnoreCase);
            }
            return originalStr;
        }

        /// <summary>
        /// 替换字符串，使用其他字符串代替
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="replacedStr">要替换字符串</param>
        /// <param name="replaceStr">替换为的字符串</param>
        /// <returns></returns>
        public static string ReplaceStrUseStr(string originalStr, string replacedStr, string replaceStr)
        {
            if (string.IsNullOrEmpty(originalStr))
            {
                return string.Empty;
            }
            return Regex.Replace(originalStr, replacedStr, replaceStr, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 拆分成字符串数组
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="separator">分离器</param>
        /// <returns></returns>
        public static string[] SplitStrArray(string originalStr, string separator)
        {
            if (originalStr.IndexOf(separator) < 0)
            {
                return new string[]
                {
                    originalStr
                };
            }
            return Regex.Split(originalStr, separator.Replace(".", "\\."), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 拆分字符串使用行
        /// </summary>
        /// <param name="originalContent">原始内容</param>
        /// <param name="splitLines">拆分线条</param>
        /// <returns></returns>
        public static string SplitStrUseLines(string originalContent, int splitLines)
        {
            if (string.IsNullOrEmpty(originalContent))
            {
                return string.Empty;
            }
            int num = 0;
            int num2 = originalContent.Length - 5;
            int i;
            for (i = 1; i < num2; i++)
            {
                if (originalContent.Substring(i, 6).Equals("<br />", StringComparison.OrdinalIgnoreCase))
                {
                    num++;
                }
                if (originalContent.Substring(i, 5).Equals("<br/>", StringComparison.OrdinalIgnoreCase))
                {
                    num++;
                }
                if (originalContent.Substring(i, 4).Equals("<br>", StringComparison.OrdinalIgnoreCase))
                {
                    num++;
                }
                if (originalContent.Substring(i, 3).Equals("<p>", StringComparison.OrdinalIgnoreCase))
                {
                    num++;
                }
                if (num >= splitLines)
                {
                    break;
                }
            }
            if (num >= splitLines)
            {
                string result;
                if (i == num2)
                {
                    result = originalContent[..(i - 1)];
                }
                else
                {
                    result = originalContent[..i];
                }
                return result;
            }
            return originalContent;
        }

        /// <summary>
        /// 拆分字符串使用 例如：123456，SplitStrUseStr("123456",".") = .1.2.3.4.5.6.
        /// </summary>
        /// <param name="originalStr">原字符</param>
        /// <param name="separator">分离器</param>
        /// <returns></returns>
        public static string SplitStrUseStr(string originalStr, string separator)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(separator);
            for (int i = 0; i < originalStr.Length; i++)
            {
                stringBuilder.Append(originalStr[i]);
                stringBuilder.Append(separator);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// SQL编码
        /// </summary>
        /// <param name="strSQL">原本SQL</param>
        /// <returns></returns>
        public static string SqlEncode(string strSQL)
        {
            if (string.IsNullOrEmpty(strSQL))
            {
                return string.Empty;
            }
            return strSQL.Trim().Replace("'", "''");
        }

        /// <summary>
        /// 文本解码
        /// </summary>
        /// <param name="originalStr">原始字符</param>
        /// <returns></returns>
        public static string TextDecode(string originalStr)
        {
            StringBuilder stringBuilder = new(originalStr);
            stringBuilder.Replace("<br/><br/>", "\r\n");
            stringBuilder.Replace("<br/>", "\r");
            stringBuilder.Replace("<p></p>", "\r\n\r\n");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 文本编码
        /// </summary>
        /// <param name="originalStr">原始字符</param>
        /// <returns></returns>
        public static string TextEncode(string originalStr)
        {
            if (string.IsNullOrEmpty(originalStr))
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new(originalStr);
            stringBuilder.Replace("\r\n", "<br />");
            stringBuilder.Replace("\n", "<br />");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 首字母转换 小写
        /// </summary>
        /// <param name="originalVal">原始值</param>
        /// <returns></returns>
        public static string TransformFirstToLower(string originalVal)
        {
            if (string.IsNullOrEmpty(originalVal))
            {
                return originalVal;
            }
            if (originalVal.Length >= 2)
            {
                return string.Concat(originalVal[..1].ToLower(), originalVal.AsSpan(1));
            }
            return originalVal.ToUpper();
        }

        /// <summary>
        /// 首字母转换 大写
        /// </summary>
        /// <param name="originalVal">原始值</param>
        /// <returns></returns>
        public static string TransformFirstToUpper(string originalVal)
        {
            if (string.IsNullOrEmpty(originalVal))
            {
                return originalVal;
            }
            if (originalVal.Length >= 2)
            {
                return string.Concat(originalVal[..1].ToUpper(), originalVal.AsSpan(1));
            }
            return originalVal.ToUpper();
        }

        /// <summary>
        /// 返回相隔天数
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetDaysDate(DateTime date)
        {
            DateTime d = Convert.ToDateTime("1900-01-01");
            return (date - d).Days;
        }

        /// <summary>
        /// 返回指定天数的时间
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeByDays(int days)
        {
            return Convert.ToDateTime("1900-01-01").AddDays((double)days);
        }

        /// <summary>
        /// 取出网址中的域名。
        /// </summary>
        /// <param name="url">字符串</param>
        /// <returns></returns>
        public static string CutUrlReturnPath(string url)
        {
            Regex regex = CutUrlReturnPathRegex();
            return regex.Replace(url, "/");
        }

        /// <summary>
        /// 根据传入ipv4地址，检测是否是局域网IP
        /// </summary>
        /// <param name="ipv4Address">ipv4地址</param>
        /// <returns>true/false</returns>
        public static bool IsPrivateNetwork(string ipv4Address)
        {
            if (IPAddress.TryParse(ipv4Address, out var ip))
            {
                return IsPrivateNetwork(ip);
            }
            return false;
        }

        /// <summary>
        /// 根据传入ipv4地址，检测是否是局域网IP
        /// </summary>
        /// <param name="ipv4Address">ipv4地址</param>
        /// <returns>true/false</returns>
        public static bool IsPrivateNetwork(IPAddress ipv4Address)
        {
            if (ipv4Address.AddressFamily is System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = ipv4Address.GetAddressBytes();
                if (ipBytes[0] == 10) return true;
                if (ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31) return true;
                if (ipBytes[0] == 192 && ipBytes[1] == 168) return true;
            }
            return false;
        }

        /// <summary>
        /// 判断IP地址在不在某个IP地址段（仅支持IPV4）
        /// </summary>
        /// <param name="input">需要判断的IP地址</param>
        /// <param name="begin">起始地址</param>
        /// <param name="ends">结束地址</param>
        /// <returns></returns>
        public static bool IpAddressInRange(string input, string begin, string ends)
        {
            uint current = IPToID(input);
            return current >= IPToID(begin) && current <= IPToID(ends);
        }

        /// <summary>
        /// IP地址转换成数字
        /// </summary>
        /// <param name="addr">IP地址</param>
        /// <returns>数字,输入无效IP地址返回0</returns>
        private static uint IPToID(string addr)
        {
            if (!IPAddress.TryParse(addr, out var ip) || ip.AddressFamily is not System.Net.Sockets.AddressFamily.InterNetwork) return 0;
            Span<byte> bytes = stackalloc byte[4];
            ip.TryWriteBytes(bytes, out _);
            if (BitConverter.IsLittleEndian) bytes.Reverse();
            return BitConverter.ToUInt32(bytes);
        }

        ///// <summary>
        ///// 设置查询值返回URL
        ///// </summary>
        ///// <param name="queryName">名称</param>
        ///// <param name="newValues">新值</param>
        ///// <returns></returns>
        //public static string SetQueryValueReturnUrl(string queryName, string newValues)
        //{
        //	NameValueCollection queryString = HttpContextExtension.Current.Request.QueryString;
        //	string[] allKeys = queryString.AllKeys;
        //	string text = HttpContextExtension.Current.Request.Url.GetLeftPart(UriPartial.Path);
        //	text += "?";
        //	if (allKeys.Length > 0)
        //	{
        //		for (int i = 0; i < allKeys.Length; i++)
        //		{
        //			string str = queryString.GetValues(i)[0].ToString();
        //			if (allKeys[i] == queryName)
        //			{
        //				str = newValues;
        //			}
        //			if (i != 0)
        //			{
        //				text += "&";
        //			}
        //			text = text + allKeys[i] + "=" + str;
        //		}
        //	}
        //	if (Array.IndexOf<string>(allKeys, queryName) == -1)
        //	{
        //		if (allKeys.Length > 0)
        //		{
        //			string text2 = text;
        //			text = string.Concat(new string[]
        //			{
        //				text2,
        //				"&",
        //				queryName,
        //				"=",
        //				newValues
        //			});
        //		}
        //		else
        //		{
        //			text = text + queryName + "=" + newValues;
        //		}
        //	}
        //	return text;
        //}

        private static readonly string PROLONG_SYMBOL = "...";

        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();//RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

#if NET7_0_OR_GREATER
        [GeneratedRegex("<[^>]+>")]
        private static partial Regex CutStringTitleRegex();
        [GeneratedRegex("[^\\u4E00-\\u9FA5]")]
        private static partial Regex ReplaceCnCharRegex();
        [GeneratedRegex("^(http:\\/\\/||https:\\/\\/)[A-Za-z0-9_:.]*\\/")]
        private static partial Regex CutUrlReturnPathRegex();
#else

        private static Regex CutStringTitleRegex() => new("<[^>]+>");

        private static Regex ReplaceCnCharRegex() => new("[^\\u4E00-\\u9FA5]");

        private static Regex CutUrlReturnPathRegex() => new("^(http:\\/\\/||https:\\/\\/)[A-Za-z0-9_:.]*\\/");
#endif
    }
}
