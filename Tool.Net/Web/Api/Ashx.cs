using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;
using Tool.Web.Api.ApiCore;

namespace Tool.Web.Api
{
    /// <summary>
    /// 针对于一般处理程序，备注一般处理程序必须继承于（<see cref="ApiAshx"/> 类才会生效） 所有状态（用于更好的使用API）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]//| AttributeTargets.Property
    public class Ashx : Attribute
    {
        /// <summary>
        /// 用于实现构造(带默认参数)
        /// </summary>
        public Ashx() { }

        /// <summary>
        /// 请求的一个状态
        /// </summary>
        public AshxState State { get; set; } = AshxState.All;

        ///// <summary>
        ///// 设置一个Session对象状态，默认是完全控制（注明：该参数只支持 <see cref="ApiAshx"/>/<see cref="MinApi"/> 的新路由方式支持）（声明：由于迁移至core模块，现在是中间件管理，这里配置无法生效）
        ///// </summary>
        //public AshxSessionState IsSession { get; set; } = AshxSessionState.Open;

        ///// <summary>
        ///// 表示当前请求方法可以接收实体对象
        ///// </summary>
        //public bool IsMode { get; set; } = false;

        /// <summary>
        /// 表示当前方法是否包含在被调起方法内。（注明：第一次设置的时候生效，后期修改无效。）
        /// </summary>
        public bool IsMethods { get; set; } = true;

        /// <summary>
        /// 表示为该方法指定了一个名称，用于对外的访问安全。（注明：第一次设置的时候生效，后期修改无效。）
        /// </summary>
        public string ID { get; set; } = null;

        /// <summary>
        /// 表示当前请求是否支持跨域请求，设置您的跨域对象
        /// </summary>
        public CrossDomain CrossDomain { get; internal set; } = null;

        /// <summary>
        /// 获取当前调用方法是不是事件方法，声明事件方法必须使用 <see cref="OnAshxEvent"/> 类作为返回值。
        /// </summary>
        public bool IsOnAshxEvent { get; private set; } = false;

        /// <summary>
        /// 标注当前请求是否是异步方法（注明：该参数只支持 <see cref="ApiAshx"/>/<see cref="MinApi"/> 的新路由方式支持）
        /// </summary>
        public bool IsTask { get; private set; } = false;

        /// <summary>
        /// 标注当前请求是否是请求的最小API
        /// </summary>
        public bool IsMinApi { get; internal set; } = false;

        /// <summary>
        /// 表示该方法的名称
        /// </summary>
        public string Methods { get { return Action.Name; } }

        /// <summary>
        /// 表示该方法所包含的访问参数
        /// </summary>
        public ApiParameter[] Parameters { get; internal set; }

        /// <summary>
        /// 当前方法的执行信息
        /// </summary>
        internal MethodInfo Method { get { return Action.Method; } }

        /// <summary>
        /// 当前方法的执行信息,目前只支持 <see cref="IAshxAction"/> 对象
        /// </summary>
        internal ActionDispatcher<IAshxAction> Action { get; set; } = null;

        /// <summary>
        /// 获取当前方法上包含的自定义类
        /// </summary>
        internal IReadOnlyDictionary<Type, Attribute> _keyAttributes { get; set; } = null;//Dictionary

        /// <summary>
        /// 根据指定的自定义类获取当前接口对象上的<see cref="Attribute"/>（自定义类）
        /// </summary>
        /// <typeparam name="T">指定的<see cref="Attribute"/>（自定义类）</typeparam>
        /// <returns>返回<see cref="Attribute"/>（自定义类）</returns>
        public T GetAttribute<T>() where T : Attribute
        {
            if (Method == null)
            {
                throw new System.Exception("当前方法只能在{Initialize}方法内调用。");
            }
            return Attribute.GetCustomAttribute(Method, typeof(T)).ToVar<T>();
        }

        /// <summary>
        /// 获取当前接口对象上的所有<see cref="Attribute"/>（自定义类）
        /// </summary>
        /// <returns>返回所有的<see cref="Attribute"/>（自定义类）</returns>
        public Attribute[] GetAttributes()
        {
            if (Method == null)
            {
                throw new System.Exception("当前方法只能在{Initialize}方法内调用。");
            }
            return Attribute.GetCustomAttributes(Method);
        }

        /// <summary>
        /// 获取自定义类，根据<see cref="Type"/>获取 （缓存效率更高。）
        /// </summary>
        /// <typeparam name="T">指定的<see cref="Attribute"/>（自定义类）</typeparam>
        /// <param name="value">返回的类</param>
        /// <returns>返回<see cref="Attribute"/>（自定义类）</returns>
        public bool TryGetValue<T>(out T value) where T : Attribute
        {
            var _attribute = TryGetValue(typeof(T), out Attribute value1);
            value = value1 as T;
            return _attribute;
        }

        /// <summary>
        /// 获取自定义类，根据<see cref="Type"/>获取 （缓存效率更高。）
        /// </summary>
        /// <param name="key">自定义类的<see cref="Type"/></param>
        /// <param name="value">返回的类</param>
        /// <returns>返回<see cref="Attribute"/>（自定义类）</returns>
        public bool TryGetValue(Type key, out Attribute value)
        {
            return _keyAttributes.TryGetValue(key, out value);
        }

        /// <summary>
        /// 第一次绑定方法控制数据
        /// </summary>
        internal void GetKeyAttributes(bool isMin)
        {
            this.IsMinApi = isMin;

            if (this.IsMinApi)
            {
                this.IsTask = AshxExtension.IsTaskApiOut(Method.ReturnType);// Method.ReturnType == typeof(Task<IApiOut>)
            }
            else
            {
                this.IsTask = Method.ReturnType == typeof(Task) || Method.ReturnType == typeof(OnAshxEvent);//(Method.ReturnType == typeof(Task) || Method.ReturnType == typeof(OnAshxEvent) ? true : false);
            }



            this.IsOnAshxEvent = Method.ReturnType == typeof(OnAshxEvent); //(Method.ReturnType == typeof(OnAshxEvent) ? true : false);

            //lock (this)
            //{
            //_keyAttributes = new Dictionary<Type, Attribute>();
            try
            {
                Dictionary<Type, Attribute> _attrikeys = new();
                var Attributes = GetAttributes();
                foreach (Attribute attribute in Attributes)
                {
                    if (attribute is not Ashx)
                    {
                        _attrikeys.Add(attribute.GetType(), attribute);
                    }
                }

                _keyAttributes = _attrikeys.AsReadOnly();
            }
            catch
            {
            }
            //}
        }
    }

    /// <summary>
    /// 针对于继承 <see cref="ApiAshx"/> 的类，用于事件方法接口，需要另一个模块驱动的消息接口，可以使用
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class OnAshxEvent : IDisposable
    {
        /// <summary>
        /// 完成该有的配置
        /// </summary>
        /// <param name="Event">当前长连接要执行的消息方法</param>
        public OnAshxEvent(Action<OnAshxEvent> Event): this(Event, StringExtension.GetGuid())
        {
        }

        /// <summary>
        /// 完成该有的配置
        /// </summary>
        /// <param name="Event">当前长连接要执行的消息方法</param>
        /// <param name="GuId">配置的访问事件ID</param>
        public OnAshxEvent(Action<OnAshxEvent> Event, string GuId)
        {
            if (string.IsNullOrWhiteSpace(GuId))
            {
                throw new ArgumentException("GuId 字段不能为空！", nameof(GuId));
            }
            this.GuId = GuId;
            this.ActionEvent = Event;
            //this.ManualReset = new ManualResetEvent(false);
        }

        /// <summary>
        /// 类似于ID,必须是唯一的，用于被调用
        /// </summary>
        public string GuId { get; set; } = null;

        /// <summary>
        /// 用户数据
        /// </summary>
        public object Data { get; set; } = null;

        /// <summary>
        /// 表示当前长连接可以维持的毫秒数 （默认60秒）
        /// </summary>
        public int DelayTime { get; set; } = 60 * 1000;

        /// <summary>
        /// 是否先向客户端输出空包，将不会阻塞线程
        /// </summary>
        public bool IsFlush { get; set; } = true;

        /// <summary>
        /// 获取或设置输出流的 HTTP MIME 类型。
        /// </summary>
        /// <remarks>输出流的 HTTP MIME 类型。 默认值为“text/html”。</remarks>
        public string ContentType { get; set; } = "text/html;";

        /// <summary>
        /// 用于表示当前事件的执行情况
        /// </summary>
        public OnAshxEventState OnAshx { get; internal set; } = OnAshxEventState.Default;

        /// <summary>
        /// 当前长连接要执行的消息方法
        /// </summary>
        internal Action<OnAshxEvent> ActionEvent { get; set; } = null;

        /// <summary>
        /// 当前对象绑定的线程信号
        /// </summary>
        internal ManualResetEvent ManualReset { get; set; } = null;

        /// <summary>
        /// 触发已有的事件
        /// </summary>
        /// <param name="GuId">事件ID</param>
        /// <returns>返回是否存在</returns>
        public static bool IsStartEvent(string GuId)
        {
            return IsStartEvent(GuId, null);
        }

        /// <summary>
        /// 触发已有的事件
        /// </summary>
        /// <param name="GuId">事件ID</param>
        /// <param name="Data">传入的数据</param>
        /// <returns>返回是否存在</returns>
        public static bool IsStartEvent(string GuId, object Data)
        {
            if (string.IsNullOrWhiteSpace(GuId))
            {
                throw new ArgumentException("值不能为空。", nameof(GuId));
            }
            if (StaticData.AshxEvents.TryRemove(GuId, out OnAshxEvent onAshxEvent))
            {
                onAshxEvent.OnAshx = OnAshxEventState.Success;
                onAshxEvent.Data = Data ?? onAshxEvent.Data;
                onAshxEvent.ManualReset?.Set();
                return true;
            }
            return false;
        }

        internal void Revive() => ActionEvent(this);

        /// <summary>
        /// 释放由 <see cref="OnAshxEvent"/> 类的当前实例使用的所有资源。
        /// </summary>
        public void Dispose()
        {
            if (ManualReset is null) return;
            ManualReset.Dispose();
            ManualReset = null;
            ActionEvent = null;
            Data = null;
            GuId = null;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 针对于Api需要接受相应的状态
    /// </summary>
    //[System.Runtime.InteropServices.ComVisible(true)]
    //[Flags]
    public enum AshxState : byte
    {
        /// <summary>
        /// 该状态表示都包含。
        /// </summary>
        All = 0,
        /// <summary>
        /// 只请求页面的首部。
        /// </summary>
        Head = 1,
        /// <summary>
        /// 该状态表示 get通过地址栏传输。（get参数有长度限制（受限于url长度））
        /// </summary>
        Get = 2,
        /// <summary>
        /// 该状态表示 post通过报文传输。（post无限制）
        /// </summary>
        Post = 4,
        /// <summary>
        /// 从客户端向服务器传送的数据取代指定的文档的内容。
        /// </summary>
        Put = 8,
        /// <summary>
        /// 实体中包含一个表，表中说明与该URI所表示的原内容的区别。
        /// </summary>
        Patch = 16,
        /// <summary>
        /// 请求服务器删除指定的页面。
        /// </summary>
        Delete = 32
    }

    ///// <summary>
    ///// 针对于一般处理程序,设置一个Session对象状态
    ///// </summary>
    //public enum AshxSessionState
    //{
    //    /// <summary>
    //    /// 为请求启用完全的读写会话状态行为。 
    //    /// </summary>
    //    Open = 1,
    //    /// <summary>
    //    /// 为请求启用只读会话状态。 
    //    /// </summary>
    //    ReadOnly = 2,
    //    /// <summary>
    //    /// 未启用会话状态来处理请求。
    //    /// </summary>
    //    Close = 3
    //}

    /// <summary>
    /// 返回客户端的类型
    /// </summary>
    public enum WriteType
    {
        /// <summary>
        /// (文本页面)text/html
        /// </summary>
        Html,
        /// <summary>
        /// JSON（JavaScript Object Notation）application/json
        /// </summary>
        Json,
        /// <summary>
        /// 可扩展标记语言（英语：eXtensible Markup Language，简称: XML），是一种标记语言。 application/xml
        /// </summary>
        Xml,
        /// <summary>
        /// 文本输出。text/plain
        /// </summary>
        Text
    }

    /// <summary>
    /// 对异步事件类的状态
    /// </summary>
    public enum OnAshxEventState
    {
        /// <summary>
        /// 表示，无任何动作！
        /// </summary>
        Default,
        /// <summary>
        /// 表示，被触发了！
        /// </summary>
        Success,
        /// <summary>
        /// 表示，超时了！
        /// </summary>
        Timeout,
        /// <summary>
        /// 表示，因出现新的相同的ID链接，前一个将被强制关闭，以保证唯一性！
        /// </summary>
        OnlyID,
    }

    /// <summary>
    /// 指定允许其他域名访问，可跨域
    /// <para>相关配置：</para>
    /// <para>'Access-Control-Allow-Origin:http://172.20.0.206'  一般用法（*，指定域，动态设置），3是因为*不允许携带认证头和cookies</para>
    /// <para>'Access-Control-Allow-Credentials:true'  是否允许后续请求携带认证信息（cookies）,该值只能是true,否则不返回</para>
    /// <para>'Access-Control-Allow-Methods:HEAD,GET,POST,PUT,PATCH,DELETE'  允许的请求类型</para>
    /// <para>'Access-Control-Allow-Headers:x-requested-with,content-type'  允许的请求头字段</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]//| AttributeTargets.Property /// <para>'Access-Control-Max-Age: 1800'  预检结果缓存时间,也就是上面说到的缓存啦</para>
    public class CrossDomain : Attribute
    {
        /// <summary>
        /// 是否允许后续请求携带认证信息（cookies）,该值只能是true,否则不返回
        /// </summary>
        public bool Credentials { get; set; } = false;

        /// <summary>
        /// 表示当前跨域请求运行的域名
        /// </summary>
        public string Origin { get; set; } = "*";

        /// <summary>
        /// 允许的请求类型 可选：HEAD,GET,POST,PUT,PATCH,DELETE
        /// </summary>
        public string Methods { get; set; } = null;

        /// <summary>
        /// 允许的请求头字段 例如：content-type
        /// </summary>
        public string Headers { get; set; } = null;

        ///// <summary>
        ///// 允许的请求缓存 （缓存时间 1800 常见）
        ///// </summary>
        //public uint MaxAge { get; set; } = 0;
    }
}
