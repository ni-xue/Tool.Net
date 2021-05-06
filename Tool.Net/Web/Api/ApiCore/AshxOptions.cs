using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// Ashx 相关选项，提高使用
    /// </summary>
    public class AshxOptions
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        public AshxOptions() 
        {
            
        }

        ///// <summary>
        ///// 是否注册 全局异常对象 对象 （无用注册准备移除）
        ///// </summary>
        //public bool IsException { set; get; }

        /// <summary>
        /// 是否采用异步线程池，处理每次请求路由的过程
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 允许注册 全局<see cref="System.Text.Json.JsonSerializerOptions"/> Json 序列化条件。 默认 null
        /// </summary>
        public System.Text.Json.JsonSerializerOptions JsonOptions { get; set; }
    }
}
