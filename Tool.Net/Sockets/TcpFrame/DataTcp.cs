using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 针对于一般处理程序，备注一般处理程序必须继承于（<see cref="DataBase"/> 类才会生效） 所有状态（用于更好的使用一般处理程序，实现API接口）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DataTcp : Attribute
    {
        /// <summary>
        /// 用于实现构造(带默认参数)
        /// </summary>
        public DataTcp(byte ID) { this.ID = ID; }

        /// <summary>
        /// 消息数据格式
        /// </summary>
        public DataTcpState ObjType { get; set; } = DataTcpState.Json;

        ///// <summary>
        ///// 表示当前请求方法可以接收实体对象
        ///// </summary>
        //public bool IsMode { get; set; } = false;

        /// <summary>
        /// 表示为该方法指定了一个名称，用于对外的访问安全
        /// </summary>
        public byte ID { get; }

        /// <summary>
        /// 表示该方法的名称
        /// </summary>
        public string Methods { get { return Action.Name; } }

        /// <summary>
        /// 表示该方法所包含的访问参数
        /// </summary>
        public Parameter[] Parameters { get { return Action.Parameters; } }

        /// <summary>
        /// 表示该方法的详细信息
        /// </summary>
        public Method Pethod { get; set; } = null;

        /**
         * 当前方法的执行信息,目前只支持 <see cref="DataBase"/> 对象
         */
        internal ActionDispatcher<DataBase> Action { get; set; } = null;

        /// <summary>
        /// 根据指定的自定义类获取当前接口对象上的<see cref="Attribute"/>（自定义类）
        /// </summary>
        /// <typeparam name="T">指定的<see cref="Attribute"/>（自定义类）</typeparam>
        /// <returns>返回<see cref="Attribute"/>（自定义类）</returns>
        public T GetAttribute<T>() where T : Attribute
        {
            if (Action.Method == null)
            {
                throw new System.Exception("当前方法只能在{Initialize}方法内调用。");
            }
            return Attribute.GetCustomAttribute(Action.Method, typeof(T)).ToVar<T>();
        }

        /// <summary>
        /// 获取当前接口对象上的所有<see cref="Attribute"/>（自定义类）
        /// </summary>
        /// <returns>返回所有的<see cref="Attribute"/>（自定义类）</returns>
        public Attribute[] GetAttributes()
        {
            if (Action.Method == null)
            {
                throw new System.Exception("当前方法只能在{Initialize}方法内调用。");
            }
            return Attribute.GetCustomAttributes(Action.Method);
        }

        /**
         * 获取所有接口
         */
        internal static IReadOnlyDictionary<string, DataTcp> GetDicDataTcps<T>()
        {
            Dictionary<string, DataTcp> _dicDataTcps = new Dictionary<string, DataTcp>();

            var Sources = new System.Diagnostics.StackTrace().GetFrames();

            Assembly assembly = null;

            for (int i = 0; i < Sources.Length; i++)
            {
                Type ReflectedType = Sources[i].GetMethod().ReflectedType;
                if (typeof(DataTcp) == ReflectedType) continue;
                if (typeof(T) != ReflectedType)
                {
                    assembly = ReflectedType.Assembly;
                    break;
                }
            }

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (typeof(DataBase).IsAssignableFrom(type) && type.IsClass)
                {
                    DataBase handler = Activator.CreateInstance(type) as DataBase;

                    ActionHelper<DataBase> helper = new ActionHelper<DataBase>(type, MethodFlags.Public);

                    foreach (ActionMethod<DataBase> info in helper)
                    {
                        if (Attribute.GetCustomAttribute(info.Action.Method, typeof(DataTcp)) is DataTcp hobbyAttr)
                        {
                            hobbyAttr.Action = info.Action;
                            hobbyAttr.Pethod = TypeInvoke.GetMethod(info.Action.Method);
                            _dicDataTcps.TryAdd($"{handler.ClassID}.{hobbyAttr.ID}", hobbyAttr);
                        }
                    }
                }
            }

            return _dicDataTcps.AsReadOnly();
        }

        //internal static string GetIsIpIdea(out int i, string Obj)
        //{
        //    string ip = null;
        //    for (i = 10; i < Obj.Length; i++)
        //    {
        //        if (i > 20) break;
        //        if (Obj[i] == ']')
        //        {
        //            ip = Obj.Substring(1, i - 1);
        //            break;
        //        }
        //    }

        //    return ip;
        //}
    }

    /// <summary>
    /// 针对于框架数据包,输出格式的描述
    /// </summary>
    public enum DataTcpState
    {
        /// <summary>
        /// <see cref="byte"/> 格式的数据包
        /// </summary>
        Byte,
        /// <summary>
        /// 通用格式数据包
        /// </summary>
        Json,
        /// <summary>
        /// 最常见的数据包
        /// </summary>
        String
    }
}
