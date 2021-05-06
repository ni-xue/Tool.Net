using System;
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
        internal static ConcurrentDictionary<string, OnAshxEvent> StaticAshxEvents = new ConcurrentDictionary<string, OnAshxEvent>();
    }
}
