using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils.ActionDelegate;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// Api 调用函数 参数值范围可控
    /// </summary>
    public class ApiParameter : Parameter
    {
        /// <summary>
        /// 获取当前函数参数的值类型
        /// </summary>
        public Val GetVal { get; }

        /// <summary>
        /// 获取当前实际生效的Key名称
        /// </summary>
        public string KeyName { get; }

        /// <summary>
        /// 构造Api验证参数
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="state">类型</param>
        public ApiParameter(Parameter parameter, AshxState state) : base(parameter)
        {
            ApiVal apiVal = Attribute.GetCustomAttribute(this.GetParameter, typeof(ApiVal)).ToVar<ApiVal>();
            //parameter.ParameterType.Assembly

            if (apiVal != null)
            {
                this.KeyName = apiVal.IsName ? apiVal.Name : Name;

                if (apiVal.State == Val.File && typeof(Microsoft.AspNetCore.Http.IFormFile) != ParameterType)
                {
                    throw new Exception($"初始化 Api ({GetParameter.Member.DeclaringType.FullName}.{GetParameter.Member.Name}) 接口时，因参数名：{Name}，被定义为 Val.File 类型，但是参数类型并非 IFormFile，故无法实现。");
                }

                if (apiVal.State == Val.Files && typeof(Microsoft.AspNetCore.Http.IFormFileCollection) != ParameterType)
                {
                    throw new Exception($"初始化 Api ({GetParameter.Member.DeclaringType.FullName}.{GetParameter.Member.Name}) 接口时，因参数名：{Name}，被定义为 Val.Files 类型，但是参数类型并非 IFormFileCollection，故无法实现。");
                }
                
                GetVal = apiVal.State;
            }
            else
            {
                GetVal = state switch
                {
                    AshxState.Get => Val.Query,
                    AshxState.Post => Val.Form,
                    _ => Val.AllData,
                };

                this.KeyName = Name;
            }
        }
    }
}
