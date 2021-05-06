using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Utils
{
	/// <summary>
	/// 类型助手
	/// </summary>
	public class TypeHelper
	{
		/// <summary>
		/// 根据Type,返回可能存在的值
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static object ChangeType(Type targetType, object val)
		{
			if (val == null || (targetType.IsGenericType && val.ToString() == ""))
			{
				return null;
			}
			if (targetType == val.GetType() || targetType.IsGenericType)
			{
				return val;
			}
			if (targetType == typeof(bool))
			{
				if (val.ToString() == "0")
				{
					return false;
				}
				if (val.ToString() == "1")
				{
					return true;
				}
			}
			if (targetType.IsEnum)
			{
				if (!int.TryParse(val.ToString(), out int num))
				{
					return Enum.Parse(targetType, val.ToString());
				}
				return val;
			}
			else
			{
				if (targetType == typeof(Type))
				{
					return ReflectionHelper.GetType(val.ToString());
				}
				return Convert.ChangeType(val, targetType);
			}
		}

		/// <summary>
		/// 获取类实例名称
		/// </summary>
		/// <param name="t">Type</param>
		/// <returns></returns>
		public static string GetClassSimpleName(Type t)
		{
			string[] array = t.ToString().Split(new char[]
			{
				'.'
			});
			return array[array.Length - 1].ToString();
		}

		/// <summary>
		/// 获取默认值
		/// </summary>
		/// <param name="destType"></param>
		/// <returns></returns>
		public static string GetDefaultValue(Type destType)
		{
			if (TypeHelper.IsNumbericType(destType))
			{
				return "0";
			}
			if (destType == typeof(string))
			{
				return "\"\"";
			}
			if (destType == typeof(bool))
			{
				return "false";
			}
			if (destType == typeof(DateTime))
			{
				return "DateTime.Now";
			}
			if (destType == typeof(Guid))
			{
				return "System.Guid.NewGuid()";
			}
			if (destType == typeof(TimeSpan))
			{
				return "System.TimeSpan.Zero";
			}
			return "null";
		}

		/// <summary>
		/// 按常规名称获取类型
		/// </summary>
		/// <param name="regularName"></param>
		/// <returns></returns>
		public static Type GetTypeByRegularName(string regularName)
		{
			return ReflectionHelper.GetType(regularName);
		}

		/// <summary>
		/// 获取类型常规名称
		/// </summary>
		/// <param name="destType"></param>
		/// <returns></returns>
		public static string GetTypeRegularName(Type destType)
		{
			string arg = destType.Assembly.FullName.Split(new char[]
			{
				','
			})[0];
			return string.Format("{0},{1}", destType.ToString(), arg);
		}

		/// <summary>
		/// 获取类型的常规名称
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetTypeRegularNameOf(object obj)
		{
			return TypeHelper.GetTypeRegularName(obj.GetType());
		}

		/// <summary>
		/// 是不是数据类型
		/// </summary>
		/// <param name="destDataType"></param>
		/// <returns></returns>
		public static bool IsFixLength(Type destDataType)
		{
			return TypeHelper.IsNumbericType(destDataType) || destDataType == typeof(byte[]) || destDataType == typeof(DateTime) || destDataType == typeof(bool);
		}

		/// <summary>
		/// 是数字类型
		/// </summary>
		/// <param name="destDataType"></param>
		/// <returns></returns>
		public static bool IsNumbericType(Type destDataType)
		{
			return destDataType == typeof(int) || destDataType == typeof(uint) || destDataType == typeof(double) || destDataType == typeof(short) || destDataType == typeof(ushort) || destDataType == typeof(decimal) || destDataType == typeof(long) || destDataType == typeof(ulong) || destDataType == typeof(float) || destDataType == typeof(byte) || destDataType == typeof(sbyte);
		}

		/// <summary>
		/// 是简单类型
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool IsSimpleType(Type t)
		{
			return TypeHelper.IsNumbericType(t) || t == typeof(char) || t == typeof(string) || t == typeof(bool) || t == typeof(DateTime) || t == typeof(Type) || t.IsEnum;
		}
	}
}
