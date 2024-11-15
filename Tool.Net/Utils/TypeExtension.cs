﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tool.Utils
{
    /// <summary>
    /// 对Type进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class TypeExtension
    {
        #region Action委托集合

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public static async Task InvokeAsync(this Action @delegate)
        {
            void result() => @delegate();
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1>(this Action<T1> @delegate, T1 arg1)
        {
            void result() => @delegate(arg1);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2>(this Action<T1, T2> @delegate, T1 arg1, T2 arg2)
        {
            void result() => @delegate(arg1, arg2);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3>(this Action<T1, T2, T3> @delegate, T1 arg1, T2 arg2, T3 arg3)
        {
            void result() => @delegate(arg1, arg2, arg3);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            void result() => @delegate(arg1, arg2, arg3, arg4);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            void result() => @delegate(arg1, arg2, arg3, arg4, arg5);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            void result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            void result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            void result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            await Task.Run(result);
        }

        /// <summary>
        /// 创建异步模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <param name="arg9"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            void result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            await Task.Run(result);
        }

        #endregion

        #region Func委托集合

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<TResult>(this Func<TResult> @delegate)
        {
            TResult result() => @delegate();
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, TResult>(this Func<T1, TResult> @delegate, T1 arg1)
        {
            TResult result() => @delegate(arg1);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, TResult>(this Func<T1, T2, TResult> @delegate, T1 arg1, T2 arg2)
        {
            TResult result() => @delegate(arg1, arg2);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3)
        {
            TResult result() => @delegate(arg1, arg2, arg3);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            TResult result() => @delegate(arg1, arg2, arg3, arg4);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            TResult result() => @delegate(arg1, arg2, arg3, arg4, arg5);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            TResult result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            TResult result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            TResult result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return await Task.Run(result);
        }

        /// <summary>
        /// 创建异步返回模式
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="delegate"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <param name="arg9"></param>
        /// <returns></returns>
        public static async Task<TResult> InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> @delegate, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            TResult result() => @delegate(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return await Task.Run(result);
        }

        #endregion

        /// <summary>
        /// 获取当前类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultForType(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 验证是否是字符类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsString(this Type type)
        {
            if (type == typeof(string) || type == typeof(char) || type == typeof(char?))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证是否是数字类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNumber(this Type type)
        {
            if (type == typeof(short) || type == typeof(short?)
                || type == typeof(int) || type == typeof(int?)
                || type == typeof(long) || type == typeof(long?)
                || type == typeof(byte) || type == typeof(byte?)
                || type == typeof(decimal) || type == typeof(decimal?)
                || type == typeof(double) || type == typeof(double?)
                || type == typeof(float) || type == typeof(float?)
                || type == typeof(ushort) || type == typeof(ushort?)
                || type == typeof(uint) || type == typeof(uint?)
                || type == typeof(ulong) || type == typeof(ulong?))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证是否是系统变量
        /// </summary>
        /// <param name="type"></param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsType(this Type type)
        {
            if (type.IsString() || type.IsNumber()
                || type == typeof(bool) || type == typeof(bool?) || type == typeof(object)
                || type == typeof(DateTime) || type == typeof(DateTime?)
                || type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否是字典类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsDictionary(this Type type)
        {
            if (type.IsGenericType)
            {
                Type type1 = type.GetGenericTypeDefinition();
                if (typeof(IDictionary).IsAssignableFrom(type1) || typeof(IDictionary<,>).IsAssignableFrom(type1))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 获取当前对象下所有属性集合
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <returns>属性集合</returns>
        public static PropertyDescriptorCollection GetProperties(this object obj)
        {
            PropertyDescriptorCollection propertyDescriptor = TypeDescriptor.GetProperties(obj);
            return propertyDescriptor;
        }

        /// <summary>
        /// 获取当前对象下所有属性集合
        /// </summary>
        /// <param name="componentType">对象源类型</param>
        /// <returns>属性集合</returns>
        public static PropertyDescriptorCollection GetProperties(Type componentType)
        {
            PropertyDescriptorCollection propertyDescriptor = TypeDescriptor.GetProperties(componentType);
            return propertyDescriptor;
        }

        /// <summary>
        /// 获取当前对象下指定名称的属性对象
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static PropertyDescriptor GetPropertieFind(this object obj, string name, bool ignoreCase = false)
        {
            PropertyDescriptor property = obj.GetProperties().Find(name, ignoreCase);
            return property;
        }

        /// <summary>
        /// 根据属性对象获取属性的值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="descriptor">属性对象</param>
        /// <returns>属性值</returns>
        public static object GetValue(this object obj, PropertyDescriptor descriptor)
        {
            return descriptor.GetValue(obj);
        }

        /// <summary>
        /// 根据属性对象修改属性的值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="descriptor">属性对象</param>
        /// <param name="value">修改属性的值</param>
        public static void SetValue(this object obj, PropertyDescriptor descriptor, object value)
        {
            descriptor.SetValue(obj, value);
        }

        /// <summary>
        /// 获取指定属性值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>返回属性值</returns>
        public static object GetValue(this object obj, string name, bool ignoreCase)
        {
            object _obj = default;
            PropertyDescriptor descriptor = obj.GetPropertieFind(name, ignoreCase);
            if (!object.Equals(descriptor, null))
            {
                _obj = obj.GetValue(descriptor);
            }
            return _obj;
        }

        /// <summary>
        /// 修改指定属性值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">修改的值</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetValue(this object obj, string name, object value, bool ignoreCase)
        {
            PropertyDescriptor descriptor = obj.GetPropertieFind(name, ignoreCase);
            if (!object.Equals(descriptor, null))
            {
                obj.SetValue(descriptor, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取指定属性值（新模式）获取不到时 会抛出异常
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <returns>返回属性值</returns>
        /// <exception cref="Exception">字段不存在时会报错！</exception>
        public static object GetValue(this object obj, string name)
        {
            object _obj = obj.GetPropertyKey(name, out bool isexit);
            if (isexit)
            {
                return _obj;
            }
            _obj = obj.GetFieldKey(name, out isexit);
            if (isexit)
            {
                return _obj;
            }
            throw new Exception($"{obj.GetType().FullName}.{name} 字段不存在！");
        }

        /// <summary>
        /// 修改指定属性值（新模式）
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetValue(this object obj, string name, object value)
        {
            bool isexit = obj.SetPropertyKey(name, value);
            if (!isexit)
            {
                return obj.SetFieldKey(name, value); ;
            }
            return isexit;
        }

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public static object GetFieldKey(this object obj, string name, out bool isexist) => obj.GetFieldKey(obj.GetType(), name, out isexist);

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetFieldKey(this object obj, string name, object value) => obj.SetFieldKey(obj.GetType(), name, value);

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public static object GetFieldKey<T>(this object obj, string name, out bool isexist) => obj.GetFieldKey(typeof(T), name, out isexist);

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetFieldKey<T>(this object obj, string name, object value) => obj.SetFieldKey(typeof(T), name, value);

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="type">类型</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public static object GetFieldKey(this object obj, Type type, string name, out bool isexist)
        {
            var typeProperty = type.GetPropertys();
            return typeProperty.GetField(obj, name, out isexist);
        }

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="type">类型</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetFieldKey(this object obj, Type type, string name, object value)
        {
            var typeProperty = type.GetPropertys();
            return typeProperty.SetField(obj, name, value);
        }

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public static object GetPropertyKey(this object obj, string name, out bool isexist) => obj.GetPropertyKey(obj.GetType(), name, out isexist);

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetPropertyKey(this object obj, string name, object value) => obj.SetPropertyKey(obj.GetType(), name, value);

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public static object GetPropertyKey<T>(this object obj, string name, out bool isexist) => obj.GetPropertyKey(typeof(T), name, out isexist);

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetPropertyKey<T>(this object obj, string name, object value) => obj.SetPropertyKey(typeof(T), name, value);

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="type">类型</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public static object GetPropertyKey(this object obj, Type type, string name, out bool isexist)
        {
            var typeProperty = type.GetPropertys();
            return typeProperty.GetProperty(obj, name, out isexist);
        }

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="type">类型</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetPropertyKey(this object obj, Type type, string name, object value)
        {
            var typeProperty = type.GetPropertys();
            return typeProperty.SetProperty(obj, name, value);
        }

        /// <summary>
        /// 获取公共管理的属性构造器
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性构造器</returns>
        public static ActionDelegate.TypePropertyDescriptor GetPropertys(this Type type)
        {
            var typeProperty = StaticData.Propertys.GetOrAdd(type, (type) =>
            {
                return new ActionDelegate.TypePropertyDescriptor(type);
            });
            return typeProperty;
        }
    }
}
