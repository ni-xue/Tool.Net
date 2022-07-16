﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Tool.Web.Api;

namespace Tool
{
    /// <summary>
    /// 静态数据类，用于存放核心数据对象的类
    /// </summary>
    internal class StaticData
    {
        /// <summary>
        /// 当前全局的 <see cref="OnAshxEvent"/> 对象集合
        /// </summary>
        internal static ConcurrentDictionary<string, OnAshxEvent> StaticAshxEvents = new();
    }

    /// <summary>
    /// 全局公共对象，支持线程安全访问
    /// </summary>
    public readonly struct GlobalObj
    {
        /// <summary>
        /// 线程安全对象，可供不想 存储对象的朋友简单化对象管理
        /// </summary>
        public ConcurrentDictionary<string, object> OrigObj { get; }

        /// <summary>
        /// 预估初始对象大小
        /// </summary>
        public GlobalObj(int capacity)
        {
//#if net5
//            OrigObj = new(10, capacity);
//#else
//            OrigObj = new(10, capacity);
//#endif

            OrigObj = new(10, capacity);
        }

        /// <summary>
        /// 向对象添加键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>状态</returns>
        public bool Add(string key, object value)
        {
            return OrigObj.TryAdd(key, value);
        }

        /// <summary>
        /// 从对象中获取现有键值
        /// </summary>
        /// <param name="key">键名称</param>
        /// <param name="value">现有值</param>
        /// <returns>状态</returns>
        public bool Get(string key, out object value)
        {
            return OrigObj.TryGetValue(key, out value);
        }

        /// <summary>
        /// 从对象中获取现有键值
        /// </summary>
        /// <typeparam name="T">原对象类型</typeparam>
        /// <param name="key">键名称</param>
        /// <param name="value">现有值</param>
        /// <returns>状态</returns>
        public bool Get<T>(string key, out T value)
        {
            if (OrigObj.TryGetValue(key, out object origvalue))
            {
                value = origvalue.ToVar<T>();
                return true;
            }
            value = default;
            return false;
        }
    }
}