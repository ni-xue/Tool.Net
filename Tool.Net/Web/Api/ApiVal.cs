﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Web.Api
{
    /// <summary>
    /// 用于 Api 请求参数，类型定义，区分不同阐述的值。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public class ApiVal : Attribute
    {
        /// <summary>
        /// 请求指定参数的类型
        /// </summary>
        public Val State { get; }

        /// <summary>
        /// 获取设置，当前获取对象的名称（针对于名称奇特，无法用代码表示的名称）
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取设置，获取值名称是否生效。
        /// </summary>
        public bool IsName { get; } = false;

        /// <summary>
        /// 设置当前参数的值来源
        /// </summary>
        /// <param name="bodyState">值来源类型</param>
        public ApiVal(Val bodyState) 
        {
            this.State = bodyState;
        }

        /// <summary>
        /// 设置当前参数的值来源
        /// </summary>
        /// <param name="bodyState">值来源类型</param>
        /// <param name="name">获取值实际对应的Key</param>
        public ApiVal(Val bodyState, string name)
        {
            this.State = bodyState;
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.Name = name;
                this.IsName = true;
            }
        }
    }

    /// <summary>
    /// 值来源的类型
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public enum Val : byte
    {
        /// <summary>
        /// URL ? 之后 传入的数据 支持其他类型强转。
        /// </summary>
        Query = 0,
        /// <summary>
        /// 表单提交数据 支持其他类型强转。
        /// </summary>
        Form = 1,
        /// <summary>
        /// 将Query和Form 的值，找到提供给 实体类（接收实体类）
        /// </summary>
        AllMode = 2,
        /// <summary>
        /// 将Query 的值，找到提供给 实体类（接收实体类）
        /// </summary>
        QueryMode = 3,
        /// <summary>
        /// 将Form 的值，找到提供给 实体类（接收实体类）
        /// </summary>
        FormMode = 4,
        /// <summary>
        /// 获取Header下面的某个键的值，支持其他类型强转。
        /// </summary>
        Header = 5,
        /// <summary>
        /// 获取Cookie下面的某个键的值，支持其他类型强转。
        /// </summary>
        Cookie = 6,
        /// <summary>
        /// 获取上传的文件对象，接收对象必须是<see cref="Microsoft.AspNetCore.Http.IFormFile"/> 
        /// </summary>
        File = 7,//IFormFileCollection
        /// <summary>
        /// 获取注册的对象（ServiceProvider）
        /// </summary>
        Service = 8,
        /// <summary>
        /// 目前 支持 String 和 byte[] , Json 自动转对象，以及其他类型强转。
        /// </summary>
        Session = 9,
        /// <summary>
        /// 只包含 Query 和 Form 的值 支持其他类型强转。
        /// </summary>
        AllData = 10,
        /// <summary>
        /// 获取请求的路由格式 指定的键 的值 支持其他类型强转。
        /// </summary>
        RouteKey = 11  
    }
}