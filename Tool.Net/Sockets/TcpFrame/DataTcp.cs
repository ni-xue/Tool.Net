using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;
using Tool.Web.Api;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 针对于一般处理程序，备注一般处理程序必须继承于（<see cref="DataBase"/> 类才会生效） 所有状态（用于更好的使用 自定义通讯协议规范，实现Api接口）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class DataTcp : Attribute
    {
        /// <summary>
        /// 用于实现构造(带默认参数)
        /// </summary>
        /// <param name="ID">此处Id,与绑定函数有关，绑定在方法上，为方法ID，构造函数上为类ID</param>
        public DataTcp(byte ID) { this.ActionID = ID; }

        ///// <summary>
        ///// 消息数据格式
        ///// </summary>
        //public DataTcpState ObjType { get; set; } = DataTcpState.Json;

        ///// <summary>
        ///// 表示当前请求方法可以接收实体对象
        ///// </summary>
        //public bool IsMode { get; set; } = false;

        /// <summary>
        /// 表示为该方法指定了一个名称，用于对外的访问安全
        /// </summary>
        public byte ClassID { get; internal set; }

        /// <summary>
        /// 表示为该方法指定了一个名称，用于对外的访问安全
        /// </summary>
        public byte ActionID { get; internal set; }

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
        public Method Pethod { get; internal set; } = null;

        /**
         * 当前方法的执行信息,目前只支持 <see cref="DataBase"/> 对象
         */
        internal ActionDispatcher<DataBase> Action { get; set; } = null;

        /**
         * 当前主消息的类委托,目前只支持 <see cref="DataBase"/> 对象
         */
        internal ClassDispatcher<DataBase> NewClass { get; set; } = null;

        /// <summary>
        /// 当前是函数是否支持异步
        /// </summary>
        public bool IsTask { get; internal set; }

        /**
         * 当前请求方法
         */
        internal static IReadOnlyDictionary<string, DataTcp> DicDataTcps { get; set; }//ConcurrentDictionary

        internal static readonly object @Lock = new();

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
        internal static void InitDicDataTcps<T>()
        {
            if (DataTcp.DicDataTcps == null)
            {
                lock (DataTcp.@Lock)
                {
                    if (DataTcp.DicDataTcps == null)
                    {
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

                        if (assembly != null)
                        {
                            DataTcp.DicDataTcps = GetDicDataTcps(assembly);
                        }
                    }
                }
            }
        }

        internal static bool IsTaskGoOut(Type ReturnType)
        {
            if (!object.Equals(ReturnType.BaseType, null) && object.Equals(ReturnType.BaseType, typeof(Task)))
            {
                if (ReturnType.GenericTypeArguments.Length > 0)
                {
                    return typeof(IGoOut).IsAssignableFrom(ReturnType.GenericTypeArguments[0]);
                }
            }
            return false;
        }

        private static IReadOnlyDictionary<string, DataTcp> GetDicDataTcps(Assembly assembly)
        {
            Dictionary<string, DataTcp> _dicDataTcps = new();

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (typeof(DataBase).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    var constructorInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                    if (constructorInfos.Length == 0) throw new Exception(string.Format("类：{0}，继承了【DataBase】类，但是无法获取到公开的构造函数，无法创建消息体。", type.FullName));

                    if (constructorInfos.Length > 1) throw new Exception(string.Format("类：{0}，继承了【DataBase】类，但是存在多个构造函数，无法创建消息体。", type.FullName));

                    if (constructorInfos[0].GetParameters().Length > 0) throw new Exception(string.Format("类：{0}，继承了【DataBase】类，必须是无参构造，无法创建消息体。", type.FullName));

                    if (Attribute.GetCustomAttribute(constructorInfos[0], typeof(DataTcp)) is DataTcp constructorId)
                    {
                        ActionHelper<DataBase> helper = new(type, MethodFlags.Public);

                        var _class = new ClassDispatcher<DataBase>(constructorInfos[0]);

                        foreach (ActionMethod<DataBase> info in helper)
                        {
                            if (Attribute.GetCustomAttribute(info.Action.Method, typeof(DataTcp)) is DataTcp hobbyAttr)
                            {
                                bool igo = typeof(IGoOut).IsAssignableFrom(info.Action.ReturnType), itask = IsTaskGoOut(info.Action.ReturnType);

                                if (!igo && !itask)
                                {
                                    throw new Exception(string.Format("类：{0}，[{1}]方法消息ID：{2}，返回值必须是【IGoOut】类型！", type.FullName, info.Name, hobbyAttr.ActionID));
                                }

                                hobbyAttr.ClassID = constructorId.ActionID;
                                hobbyAttr.Action = info.Action;
                                hobbyAttr.NewClass = _class;
                                hobbyAttr.IsTask = itask;
                                hobbyAttr.Pethod = TypeInvoke.GetMethod(info.Action.Method);
                                if (!_dicDataTcps.TryAdd($"{hobbyAttr.ClassID}.{hobbyAttr.ActionID}", hobbyAttr))
                                {
                                    throw new Exception(string.Format("类：{0}，[{1}]方法消息ID：{2}，出现了重复！", type.FullName, info.Name, hobbyAttr.ActionID));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("类：{0}，继承了【DataBase】类，并未在构造函数上申明【DataTcp】类，无法创建消息体。", type.FullName));
                    }
                }
            }

            return _dicDataTcps.AsReadOnly();
        }

        /// <summary>
        /// 给<see cref="DataBase"/>相关业务追加新的服务，如果ID相同，将采用替换方式
        /// </summary>
        /// <param name="assembly">需要验证接口的程序集</param>
        /// <returns>程序集中存在接口，为[true/false]</returns>
        public static bool AddDataTcps(Assembly assembly)
        {
            Dictionary<string, DataTcp> _dicDataTcps = DataTcp.DicDataTcps != null ? new(DataTcp.DicDataTcps) : new();

            var newdatatcps = GetDicDataTcps(assembly);

            if (newdatatcps.Count == 0) return false;

            foreach (var newdatatcp in newdatatcps)
            {
                if (_dicDataTcps.ContainsKey(newdatatcp.Key))
                {
                    _dicDataTcps[newdatatcp.Key] = newdatatcp.Value;
                }
                else
                {
                    _dicDataTcps.Add(newdatatcp.Key, newdatatcp.Value);
                }
            }

            lock (DataTcp.@Lock)
            {
                DataTcp.DicDataTcps = _dicDataTcps.AsReadOnly();
            }

            return true;
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

    ///// <summary>
    ///// 针对于框架数据包,输出格式的描述
    ///// </summary>
    //public enum DataTcpState
    //{
    //    /// <summary>
    //    /// <see cref="byte"/> 格式的数据包
    //    /// </summary>
    //    Byte,
    //    /// <summary>
    //    /// 通用格式数据包
    //    /// </summary>
    //    Json,
    //    /// <summary>
    //    /// 最常见的数据包
    //    /// </summary>
    //    String
    //}
}
