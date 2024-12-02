using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// Ashx 相关选项，提高使用
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
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
        /// 是否使用终结点模式路由 (目前公测阶段 3.0 功能完善版)
        /// </summary>
        public bool EnableEndpointRouting { get; set; } = false;

        /// <summary>
        /// 允许注册 全局<see cref="System.Text.Json.JsonSerializerOptions"/> Json 序列化条件。 默认 AshxOptions.JsonOptionsDefault 值
        /// </summary>
        public System.Text.Json.JsonSerializerOptions JsonOptions { get; set; }

        /// <summary>
        /// 默认Json序列化配置
        /// </summary>
        public static System.Text.Json.JsonSerializerOptions JsonOptionsDefault
        {
            get
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    //IgnoreReadOnlyFields = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
                };
                options.Converters.Add(Utils.JsonConverterHelper.GetDateConverter());
                options.Converters.Add(Utils.JsonConverterHelper.GetDBNullConverter());
                return options;
            }
        }
    }
}
