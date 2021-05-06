using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tool.Utils
{
	/// <summary>
	/// 判读该值是否是可以被强制转换
	/// </summary>
	/// <remarks>代码由逆血提供支持</remarks>
	public sealed class TypeParse
	{
		/// <summary>
		/// 无参构造
		/// </summary>
		private TypeParse()
		{
		}

		/// <summary>
		/// 判断是不是数字
		/// </summary>
		/// <param name="strNumber">判断值 数组</param>
		/// <returns>返回<see cref="bool"/>类型</returns>
		public static bool IsNumericArray(string[] strNumber)
		{
			if (strNumber == null)
			{
				return false;
			}
			if (strNumber.Length < 1)
			{
				return false;
			}
			for (int i = 0; i < strNumber.Length; i++)
			{
				string expression = strNumber[i];
				if (!Validate.IsNumeric(expression))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 判断<see cref="long"/>中的值是不是在32位以内
		/// </summary>
		/// <param name="expression">判断值</param>
		/// <returns>返回<see cref="int"/>类型</returns>
		public static int SafeLongToInt32(long expression)
		{
			if (expression > 2147483647L)
			{
				return 2147483647;
			}
			if (expression < -2147483648L)
			{
				return -2147483648;
			}
			return (int)expression;
		}

		/// <summary>
		/// 判读该值是否是<see cref="bool"/>类型
		/// </summary>
		/// <param name="expression">判断值</param>
		/// <param name="defValue">当判断值为空时返回的值</param>
		/// <returns>返回<see cref="bool"/>类型</returns>
		public static bool StrToBool(object expression, bool defValue)
		{
			if (expression != null)
			{
				if (string.Compare(expression.ToString(), "true", true) == 0)
				{
					return true;
				}
				if (string.Compare(expression.ToString(), "false", true) == 0)
				{
					return false;
				}
			}
			return defValue;
		}

		/// <summary>
		/// 判读该值是否是<see cref="float"/>类型
		/// </summary>
		/// <param name="expression">判断值</param>
		/// <param name="defValue">当判断值为空时返回的值</param>
		/// <returns>返回<see cref="float"/>类型</returns>
		public static float StrToFloat(object expression, float defValue)
		{
			if (expression == null || expression.ToString().Length > 10)
			{
				return defValue;
			}
			float result = defValue;
			if (expression != null && Regex.IsMatch(expression.ToString(), "^([-]|[0-9])[0-9]*(\\.\\w*)?$"))
			{
				result = Convert.ToSingle(expression);
			}
			return result;
		}

		/// <summary>
		/// 判读该值是否是<see cref="int"/>类型
		/// </summary>
		/// <param name="expression">判断值</param>
		/// <param name="defValue">当判断值为空时返回的值</param>
		/// <returns>返回<see cref="int"/>类型</returns>
		public static int StrToInt(object expression, int defValue)
		{
			if (expression == null)
			{
				return defValue;
			}
			string text = expression.ToString();
			if (text.Length <= 0 || text.Length > 11 || !Regex.IsMatch(text, "^[-]?[0-9]*$"))
			{
				return defValue;
			}
			if (text.Length >= 10 && (text.Length != 10 || text[0] != '1') && (text.Length != 11 || text[0] != '-' || text[1] != '1'))
			{
				return defValue;
			}
			return Convert.ToInt32(text);
		}

		/// <summary>
		/// 转换世界公认秒数为时间格式
		/// </summary>
		/// <param name="d">秒数</param>
		/// <returns></returns>
		public static DateTime ConvertIntDateTime(double d)
		{
			//DateTime minValue = DateTime.MinValue;
			return DateTimeExtension.ToLocalTime(d, false); //TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(d);
		}

		/// <summary>
		/// 转换时间为世界公认秒数。
		/// </summary>
		/// <param name="time">时间</param>
		/// <returns></returns>
		public static double ConvertDateTimeInt(DateTime time)
		{
			//DateTime d = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			return time.ToLocalTime(false); // (time - d).TotalSeconds;
		}
	}
}
