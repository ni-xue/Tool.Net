using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        /// <param name="isbody">是否已获取body</param>
        public ApiParameter(Parameter parameter, AshxState state, ref bool isbody) : base(parameter)
        {
            if (isbody) throw new Exception($"初始化 Api ({GetParameter.Member.DeclaringType.FullName}.{GetParameter.Member.Name}) 接口时，因存在多种 body 枚举值，故无法实现。");
            if (Attribute.GetCustomAttribute(this.GetParameter, typeof(ApiVal)) is ApiVal apiVal)
            {
                GetVal = apiVal.State;
                this.KeyName = apiVal.IsName ? apiVal.Name : Name;

                switch (GetVal)
                {
                    case Val.File:
                        TryState<Microsoft.AspNetCore.Http.IFormFile>(Val.File);
                        break;
                    case Val.Files:
                        TryState<Microsoft.AspNetCore.Http.IFormFileCollection>(Val.Files);
                        break;
                    case Val.Body:
                        isbody = true;
                        if (TryIsType<System.IO.Stream>() && TryIsType<System.IO.Pipelines.PipeReader>() && TryIsType<Memory<byte>>())
                        {
                            TypeThrow(Val.Body, $"{nameof(System.IO.Stream)} 或 {nameof(System.IO.Pipelines.PipeReader)} 或 {nameof(Memory<byte>)}");
                        }
                        break;
                    case Val.BodyJson:
                        isbody = true;
                        if (TryIsType<Utils.JsonVar>() && !ParameterType.IsClass)
                        {
                            TypeThrow(Val.BodyJson, $"{nameof(Utils.JsonVar)} 或 Json 实体类对象");
                        }
                        break;
                    case Val.BodyString:
                        isbody = true;
                        TryState<string>(Val.BodyString);
                        break;
                    default:
                        break;
                }
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

        private bool TryIsType<T>() => typeof(T) != ParameterType;

        private void TypeThrow<T>(Val isval) => TypeThrow(isval, nameof(T));

        private void TypeThrow(Val isval, string name) => throw new Exception($"初始化 Api ({GetParameter.Member.DeclaringType.FullName}.{GetParameter.Member.Name}) 接口时，因参数名：{Name}，被定义为 Val.{isval} 类型，但是参数类型并非 {name}，故无法实现。");

        private void TryState<T>(Val isval)
        {
            if (TryIsType<T>()) TypeThrow<T>(isval);
        }
    }
}
