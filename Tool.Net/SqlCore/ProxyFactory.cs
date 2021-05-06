using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Tool.SqlCore
{
	/// <summary>
	/// 底层添加数据库方法
	/// </summary>
	/// <remarks>代码由逆血提供支持</remarks>
	public sealed class ProxyFactory
	{
		/// <summary>
		/// 初始化
		/// </summary>
		private ProxyFactory()
		{
		}

		/// <summary>
		/// 添加数据库方法类以及连接字符串
		/// </summary>
		/// <param name="objtype">数据访问类</param>
		/// <param name="key">名称</param>
		/// <param name="ptypes">链接字符串</param>
		private static void CreateHandler(Type objtype, string key, Type[] ptypes)
		{
			lock (typeof(ProxyFactory))
			{
				if (!ProxyFactory.m_Handlers.ContainsKey(key))
				{
					DynamicMethod dynamicMethod = new DynamicMethod(key, typeof(object), new Type[]
					{
						typeof(object[])
					}, typeof(ProxyFactory).Module);
					ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
					ConstructorInfo constructor = objtype.GetConstructor(ptypes);
					iLGenerator.Emit(OpCodes.Nop);
					for (int i = 0; i < ptypes.Length; i++)
					{
						iLGenerator.Emit(OpCodes.Ldarg_0);
						iLGenerator.Emit(OpCodes.Ldc_I4, i);
						iLGenerator.Emit(OpCodes.Ldelem_Ref);
						if (ptypes[i].IsValueType)
						{
							iLGenerator.Emit(OpCodes.Unbox_Any, ptypes[i]);
						}
						else
						{
							iLGenerator.Emit(OpCodes.Castclass, ptypes[i]);
						}
					}
					iLGenerator.Emit(OpCodes.Newobj, constructor);
					iLGenerator.Emit(OpCodes.Ret);
					ProxyFactory.CreateInstanceHandler value = (ProxyFactory.CreateInstanceHandler)dynamicMethod.CreateDelegate(typeof(ProxyFactory.CreateInstanceHandler));
					ProxyFactory.m_Handlers.Add(key, value);
				}
			}
		}

		/// <summary>
		/// 添加数据库
		/// </summary>
		/// <typeparam name="T">数据访问类</typeparam>
		/// <returns></returns>
		public static T CreateInstance<T>()
		{
			return ProxyFactory.CreateInstance<T>(null);
		}

		/// <summary>
		/// 添加数据库
		/// </summary>
		/// <typeparam name="T">数据访问类</typeparam>
		/// <param name="parameters">数据库连接字符串</param>
		/// <returns></returns>
		public static T CreateInstance<T>(params object[] parameters)
		{
			Type typeFromHandle = typeof(T);
			Type[] parameterTypes = ProxyFactory.GetParameterTypes(parameters);
			string key = typeof(T).FullName + "_" + ProxyFactory.GetKey(parameterTypes);
			if (!ProxyFactory.m_Handlers.ContainsKey(key))
			{
				ProxyFactory.CreateHandler(typeFromHandle, key, parameterTypes);
			}
			return (T)((object)ProxyFactory.m_Handlers[key](parameters));
		}

		/// <summary>
		/// 将Type[] 集合转换成字符串
		/// </summary>
		/// <param name="types">Type[] 集合</param>
		/// <returns></returns>
		private static string GetKey(params Type[] types)
		{
			if (types == null || types.Length == 0)
			{
				return "null";
			}
			return string.Concat((object[])types);
		}

		/// <summary>
		/// 返回一个Type[] 集合
		/// </summary>
		/// <param name="parameters">数据库连接字符串集合</param>
		/// <returns></returns>
		private static Type[] GetParameterTypes(params object[] parameters)
		{
			if (parameters == null)
			{
				return new Type[0];
			}
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].GetType();
			}
			return array;
		}

		/// <summary>
		/// 存储所有的数据库类
		/// </summary>
		private static readonly Dictionary<string, ProxyFactory.CreateInstanceHandler> m_Handlers = new Dictionary<string, ProxyFactory.CreateInstanceHandler>();

		/// <summary>
		/// 实现回调数据接口
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public delegate object CreateInstanceHandler(object[] parameters);
	}
}
