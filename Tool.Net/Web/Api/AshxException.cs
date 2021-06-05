using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Tool.Utils.Data;

namespace Tool.Web.Api
{
    /// <summary>
    /// Api的异常处理类
    /// </summary>
    [Serializable]
    public class AshxException : Exception
    {
        /// <summary>
        /// 表示为该方法指定了一个名称，用于对外的访问安全
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 请求的一个状态
        /// </summary>
        public AshxState State { get; }

        /// <summary>
        /// 表示该方法的名称
        /// </summary>
        public string Methods { get; set; }

        /// <summary>
        /// 表示该异常是否已经处理
        /// </summary>
        public bool ExceptionHandled { get; set; }

        /// <summary>
        /// 获取可读的请求参数信息
        /// </summary>
        public IReadOnlyDictionary<string, (Val val, string name, object obj)> Parameters { get; }

        /// <summary>
        /// 表示异常输出结果是否包含详细接口调用数据
        /// </summary>
        public bool IsParameters { get; set; } = true;

        /// <summary>
        /// 有参构造方法
        /// </summary>
        /// <param name="ashx"></param>
        /// <param name="exception"></param>
        /// <param name="objs"></param>
        internal AshxException(Ashx ashx, Exception exception, object[] objs) : base($"Api异常（{ashx.Method.DeclaringType}.{ashx.Methods}）", exception) //("API异常", exception.InnerException)
        {
            if (objs != null && objs.Any())
            {
                Dictionary<string, (Val val, string name, object obj)> pairs = new();
                for (int i = 0; i < objs.Length; i++)
                {
                    pairs.Add(ashx.Parameters[i].Name, (ashx.Parameters[i].GetVal, ashx.Parameters[i].Type, objs[i]));
                }
                Parameters = pairs.AsReadOnly();
            }

            this.ID = ashx.ID;
            this.State = ashx.State;
            this.Methods = ashx.Methods;
            //this.TargetSite = ashx.Method;
            this.ExceptionHandled = false;
            //base.TargetSite = 
        }

        /// <summary>
        /// 获取接口调用参数文字版
        /// </summary>
        /// <returns>文字表达</returns>
        public string ToParameters()
        {
            if (Parameters == null) return null;

            StringBuilder builder = new();
            foreach (var pair in Parameters)
            {
                string txt = "NULL";
                switch (pair.Value.val)
                {
                    case Val.Query:
                    case Val.Form:
                    case Val.Header:
                    case Val.Cookie:
                    case Val.AllData:
                    case Val.RouteKey:
                        txt = pair.Value.obj?.ToString();
                        break;

                    case Val.AllMode:
                    case Val.QueryMode:
                    case Val.FormMode:
                        txt = pair.Value.obj?.ToJson();
                        break;
                }
                txt ??= "Null";
                builder.AppendFormat("Name:{0} | Val.{1} | ({2}){3}{4}", pair.Key, pair.Value.val, pair.Value.name, txt, Environment.NewLine);
            }
            return builder.ToString();
        }

        //// <summary>
        //// 获取引发当前异常的方法。
        //// </summary>
        //// <returns>引发当前异常的 System.Reflection.MethodBase。</returns>
        //public new MethodBase TargetSite { get; }

        //// <summary>
        //// 获取一个提供用户定义的其他异常信息的键/值对的集合。
        //// </summary>
        //// <returns>一个对象，它实现 System.Collections.IDictionary 接口并包含用户定义的键/值对的集合。 默认值为空集合。</returns>
        //public override IDictionary Data => base.Data;

        //// <summary>
        //// 获取调用堆栈上直接帧的字符串表示形式。
        //// </summary>
        //// <returns>用于描述调用堆栈的直接帧的字符串。</returns>
        //public override string StackTrace => base.StackTrace;

        //// <summary>
        //// 当在派生类中重写时，返回 System.Exception，它是一个或多个并发的异常的根源。
        //// </summary>
        //// <returns>
        //// 异常链中第一个被引发的异常。 如果当前异常的 System.Exception.InnerException 属性是 null 引用（Visual Basic
        //// 中为 Nothing），则此属性返回当前异常。</returns>
        //public override Exception GetBaseException()
        //{
        //    return base.GetBaseException();
        //}

        //// <summary>
        //// 当在派生类中重写时，用关于异常的信息设置 System.Runtime.Serialization.SerializationInfo。
        //// </summary>
        //// <param name="info">System.Runtime.Serialization.SerializationInfo，它存有有关所引发的异常的序列化对象数据。</param>
        //// <param name="context">System.Runtime.Serialization.StreamingContext，它包含有关源或目标的上下文信息。</param>
        //// <exception cref="System.ArgumentNullException">info 参数是空引用（Visual Basic 中为 Nothing）。</exception>
        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //}

        //// <summary>
        //// 获取或设置指向此异常所关联帮助文件的链接。
        //// </summary>
        //// <returns>统一资源名称 (URN) 或统一资源定位器 (URL)。</returns>
        //public override string HelpLink { get => base.HelpLink; set => base.HelpLink = value; }

        //// <summary>
        //// 获取描述当前异常的消息。
        //// </summary>
        //// <returns>统一资源名称 (URN) 或统一资源定位器 (URL)。</returns>
        //public override string Message => base.Message;

        //// <summary>
        //// 获取或设置导致错误的应用程序或对象的名称。
        //// </summary>
        //// <returns>导致错误的应用程序或对象的名称。</returns>
        //// <exception cref="System.ArgumentNullException">该对象必须为运行时 System.Reflection 对象</exception>
        //public override string Source { get => base.Source; set => base.Source = value; }

        /// <summary>
        /// 创建并返回当前异常的字符串表示形式。
        /// </summary>
        /// <returns>当前异常的字符串表示形式。</returns>
        public override string ToString()
        {
            string pr = IsParameters ? ToParameters() : string.Empty;
            return $"{Message}{Environment.NewLine}{pr}";//base.ToString();
        }
    }
}
