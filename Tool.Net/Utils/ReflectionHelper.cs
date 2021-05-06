using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tool.Utils
{
	/// <summary>
	/// 获取<see cref="Type"/> 对象的封装类
	/// </summary>
	public class ReflectionHelper
	{
		/// <summary>
		/// 根据dll引用名索引dll的type （例如写法：Tool,Tool.Utils.ReflectionHelper）
		/// </summary>
		/// <param name="typeAndAssName">引用的绝对路径</param>
		/// <returns></returns>
		public static Type GetType(string typeAndAssName)
		{
			string[] array = typeAndAssName.Split(new char[]
			{
				','
			});
			if (array.Length < 2)
			{
				return Type.GetType(typeAndAssName);
			}
			return ReflectionHelper.GetType(array[0].Trim(), array[1].Trim());
		}

		/// <summary>
		/// 获取当前项目进程中的dll
		/// </summary>
		/// <param name="typeFullName">引用的绝对路径</param>
		/// <param name="assemblyName">dll名称</param>
		/// <returns></returns>
		public static Type GetType(string typeFullName, string assemblyName)
		{
			if (assemblyName == null)
			{
				return Type.GetType(typeFullName);
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			for (int i = 0; i < array.Length; i++)
			{
				Assembly assembly = array[i];
				if (assembly.FullName.Split(new char[]
				{
					','
				})[0].Trim() == assemblyName.Trim())
				{
					return assembly.GetType(typeFullName);
				}
			}
			Assembly assembly2 = Assembly.Load(assemblyName);
			if (assembly2 != null)
			{
				return assembly2.GetType(typeFullName);
			}
			return null;
		}
	}
}
