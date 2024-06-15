using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;
using Tool.Web.Api;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 针对于一般处理程序，备注一般处理程序必须继承于（<see cref="DataBase"/> 类才会生效） 所有状态（用于更好的使用 自定义通讯协议规范，实现Api接口）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class DataNet : Attribute
    {
        /// <summary>
        /// 用于实现构造(带默认参数)
        /// </summary>
        /// <param name="ID">此处Id,与绑定函数有关，绑定在方法上，为方法ID，构造函数上为类ID</param>
        public DataNet(byte ID) { this.ActionID = ID; }

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
        internal ActionDispatcher<DataBase, IGoOut> Action { get; set; } = null;

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
        internal static IReadOnlyDictionary<ushort, DataNet> DicDataTcps { get; set; }//ConcurrentDictionary

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
            if (DataNet.DicDataTcps == null)
            {
                lock (StateObject.Lock)
                {
                    if (DataNet.DicDataTcps == null)
                    {
                        var Sources = new System.Diagnostics.StackTrace().GetFrames();

                        Assembly assembly = null;

                        for (int i = 0; i < Sources.Length; i++)
                        {
                            Type ReflectedType = Sources[i].GetMethod().ReflectedType;
                            if (typeof(DataNet) == ReflectedType) continue;
                            if (typeof(T) != ReflectedType)
                            {
                                assembly = ReflectedType.Assembly;
                                break;
                            }
                        }

                        if (assembly != null)
                        {
                            DataNet.DicDataTcps = GetDicDataTcps(assembly);
                        }
                    }
                }
            }
        }

        private static bool IsGoOut(Type ReturnType) => DispatcherCore.IsAssignableFrom<IGoOut>(ReturnType);

        private static bool IsTaskGoOut(MethodInfo method) => DispatcherCore.IsTask<IGoOut>(method);

        private static IReadOnlyDictionary<ushort, DataNet> GetDicDataTcps(Assembly assembly)
        {
            Dictionary<ushort, DataNet> _dicDataTcps = new();

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (typeof(DataBase).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    var constructorInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                    if (constructorInfos.Length == 0) throw new Exception(string.Format("类：{0}，继承了【DataBase】类，但是无法获取到公开的构造函数，无法创建消息体。", type.FullName));

                    if (constructorInfos.Length > 1) throw new Exception(string.Format("类：{0}，继承了【DataBase】类，但是存在多个构造函数，无法创建消息体。", type.FullName));

                    if (constructorInfos[0].GetParameters().Length > 0) throw new Exception(string.Format("类：{0}，继承了【DataBase】类，必须是无参构造，无法创建消息体。", type.FullName));

                    if (Attribute.GetCustomAttribute(constructorInfos[0], typeof(DataNet)) is DataNet constructorId)
                    {
                        var _class = new ClassDispatcher<DataBase>(constructorInfos[0]);
                        MethodInfo[] method = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        foreach (MethodInfo info in method)
                        {
                            if (Attribute.GetCustomAttribute(info, typeof(DataNet)) is DataNet hobbyAttr)
                            {
                                bool itask = IsTaskGoOut(info), igo = IsGoOut(info.ReturnType);
                                if (!itask && !igo)
                                {
                                    throw new Exception(string.Format("类：{0}，[{1}]方法消息ID：{2}，返回值必须是【IGoOut】类型！", type.FullName, info.Name, hobbyAttr.ActionID));
                                }

                                hobbyAttr.ClassID = constructorId.ActionID;
                                hobbyAttr.Action = new(info);
                                hobbyAttr.NewClass = _class;
                                hobbyAttr.IsTask = itask;
                                hobbyAttr.Pethod = TypeInvoke.GetMethod(info);

                                var actionKey = BitConverter.ToUInt16(new byte[] { hobbyAttr.ClassID, hobbyAttr.ActionID });
                                //Debug.WriteLine($"16位数：{actionKey}");
                                if (!_dicDataTcps.TryAdd(actionKey, hobbyAttr))
                                {
                                    throw new Exception(string.Format("类：{0}，[{1}]方法消息ID：{2}，出现了重复！", type.FullName, info.Name, hobbyAttr.ActionID));
                                }
                            }
                        }

                        //ActionHelper<DataBase> helper = new(type, MethodFlags.Public);
                        //foreach (ActionMethod<DataBase> info in helper)
                        //{
                        //    if (Attribute.GetCustomAttribute(info.Action.Method, typeof(DataNet)) is DataNet hobbyAttr)
                        //    {
                        //        bool igo = typeof(IGoOut).IsAssignableFrom(info.Action.ReturnType), itask = IsTaskGoOut(info.Action.ReturnType);

                        //        if (!igo && !itask)
                        //        {
                        //            throw new Exception(string.Format("类：{0}，[{1}]方法消息ID：{2}，返回值必须是【IGoOut】类型！", type.FullName, info.Name, hobbyAttr.ActionID));
                        //        }

                        //        hobbyAttr.ClassID = constructorId.ActionID;
                        //        hobbyAttr.Action = info.Action;
                        //        hobbyAttr.NewClass = _class;
                        //        hobbyAttr.IsTask = itask;
                        //        hobbyAttr.Pethod = TypeInvoke.GetMethod(info.Action.Method);

                        //        var actionKey = BitConverter.ToUInt16(new byte[] { hobbyAttr.ClassID, hobbyAttr.ActionID });
                        //        //Debug.WriteLine($"16位数：{actionKey}");
                        //        if (!_dicDataTcps.TryAdd(actionKey, hobbyAttr))
                        //        {
                        //            throw new Exception(string.Format("类：{0}，[{1}]方法消息ID：{2}，出现了重复！", type.FullName, info.Name, hobbyAttr.ActionID));
                        //        }
                        //    }
                        //}
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
            Dictionary<ushort, DataNet> _dicDataTcps = DataNet.DicDataTcps != null ? new(DataNet.DicDataTcps) : new();

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

            lock (StateObject.Lock)
            {
                DataNet.DicDataTcps = _dicDataTcps.AsReadOnly();
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
