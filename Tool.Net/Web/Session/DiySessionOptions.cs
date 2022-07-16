﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Web.Session
{
    /// <summary>
    /// 自定义的Session对象，必须完成的实现方法
    /// </summary>
    public class DiySessionOptions
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        public DiySessionOptions()
        {

        }

        /// <summary>
        /// 表明Session存储名称
        /// </summary>
        public string SessionName { get; set; } = "NiXue.Session";

        /// <summary>
        /// 自定义Session必须完成的注册流程
        /// </summary>
        /// <typeparam name="T">实现的基类</typeparam>
        public void GetDiySession<T>() where T : DiySession, new() //: new()
        {
            TypeDiySession = typeof(T);
        }

        /// <summary>
        /// 注册一个可以自由控制的开关，以及自由规则的键值。 （key：返回生效的键值，isSession：是否创建Session对象）
        /// <para>不注册，系统使用默认的方式。</para>
        /// <para>key 为空，走默认值。</para>
        /// </summary>
        public Func<Microsoft.AspNetCore.Http.HttpContext, Task<(string key,bool isSession)>> GetKey { get; set; }

        internal Type TypeDiySession { get; set; }
    }
}