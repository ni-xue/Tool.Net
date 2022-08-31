using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace Tool
{
    /// <summary>
    /// 对object类进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class ObjectExtension
    {
        static ObjectExtension() 
        {
            Static = new(5);
        }

        /// <summary>
        /// 提供用于添加对象服务
        /// </summary>
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection Services => Utils.IocHelper.IocCore.Services;

        /// <summary>
        /// 提供用于获取注入对象的服务
        /// </summary>
        public static IServiceProvider Provider => Utils.IocHelper.IocCore.Provider;

        /// <summary>
        /// 创建用于获取服务对象
        /// <para>调用该函数，将会释放掉原本的服务</para>
        /// </summary>
        public static void BuildProvider() => Utils.IocHelper.IocCore.Build();

        /// <summary>
        /// 虚拟参数(备注：要引用 <see cref="Microsoft.CSharp"/> .dll 方可使用)
        /// </summary>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static dynamic Dynamic = new ExpandoObject();

        /// <summary>
        /// 全局公共对象 可以用于 存放任何对象 管理，存在拆箱装箱行为
        /// </summary>
        public static GlobalObj Static { get; }

        /// <summary>
        /// 获取当前上下文正在运行的当前线程
        /// </summary>
        public static Thread Thread { get { return Thread.CurrentThread; } }

        /// <summary>
        /// 获取当前进程中的所有线程
        /// </summary>
        public static System.Diagnostics.ProcessThreadCollection ProcessThreadCollection
        {
            get
            {
                System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.ProcessThreadCollection allThreads = current.Threads;
                return allThreads;
            }
        }

        /// <summary>
        /// 添加虚拟参数(备注：如果对象名存在则会在直接修改原对象名内的数据，type字段默认为true，为false时则不进行修改),注明：如果存在多线程添加同一个键值的情况，请自己使用锁解决
        /// </summary>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static void DynamicAdd(string key, object value, bool type = true)
        {
            if (Dynamic is ExpandoObject)
            {
                var add = Dynamic as IDictionary<string, object>;
                if (add.ContainsKey(key))
                {
                    if (type)
                    {
                        add[key] = value;
                    }
                }
                else
                {
                    add.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 删除全部虚拟参数
        /// </summary>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static void DynamicRemove()
        {
            if (Dynamic is ExpandoObject)
            {
                var Remove = Dynamic as IDictionary<string, object>; //((IDictionary<string, object>)Dynamic);
                Remove.Clear();
            }
        }

        /// <summary>
        /// 删除指定名称的虚拟参数（备注：如果键值不存在则不会删除任何键值，不抛出异常）
        /// </summary>
        /// <param name="key">简直名称</param>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static void DynamicRemove(string key)
        {
            if (Dynamic is ExpandoObject)
            {
                var Remove = Dynamic as IDictionary<string, object>; //((IDictionary<string, object>)Dynamic);
                if (Remove.ContainsKey(key))
                {
                    Remove.Remove(key);
                }
            }
        }

        /// <summary>
        /// 获取指定键值的内容（备注：如果键值不存在则返回null）
        /// </summary>
        /// <param name="key">简直名称</param>
        /// <returns>返回结果</returns>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static dynamic Dynamickey(string key)
        {
            return DynamicObjectkey(key);
        }

        /// <summary>
        /// 获取指定键值的内容（备注：如果键值不存在则返回null）
        /// </summary>
        /// <param name="key">简直名称</param>
        /// <returns>返回结果</returns>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static T Dynamickey<T>(string key)
        {
            if (Dynamic is ExpandoObject)
            {
                var Remove = Dynamic as IDictionary<string, object>; //((IDictionary<string, object>)Dynamic);
                //if (Remove.ContainsKey(key))
                //{
                //    return Remove[key].ToVar<T>();
                //}

                if (Remove.TryGetValue(key, out object obj))
                {
                    return obj.ToVar<T>();
                }
            }
            return default;
        }

        /// <summary>
        /// 获取指定键值的内容（备注：如果键值不存在则返回null）
        /// </summary>
        /// <param name="key">简直名称</param>
        /// <returns>返回结果</returns>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static object DynamicObjectkey(string key)
        {
            if (Dynamic is ExpandoObject)
            {
                var Remove = Dynamic as IDictionary<string, object>; //((IDictionary<string, object>)Dynamic);
                //if (Remove.ContainsKey(key))
                //{
                //    return Remove[key];
                //}

                if (Remove.TryGetValue(key, out object obj))
                {
                    return obj;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取虚拟键值下面的所有名称
        /// </summary>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static string[] DynamicKeys
        {
            get
            {
                return (Dynamic as IDictionary<string, object>).Keys.ToArray();//((IDictionary<string, object>)Dynamic)
            }
        }

        /// <summary>
        /// 获取虚拟键值下面的所有内容
        /// </summary>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static dynamic[] DynamicValues
        {
            get
            {
                return (Dynamic as IDictionary<string, object>).Values.ToArray();//((IDictionary<string, object>)Dynamic)
            }
        }

        /// <summary>
        /// 获取当前包含的虚拟参数数量
        /// </summary>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static int DynamicCount
        {
            get
            {
                return (Dynamic as IDictionary<string, object>).Count; //((IDictionary<string, object>)Dynamic)
            }
        }

        /// <summary>
        /// 判断当前虚拟对象下面是否有该参数
        /// </summary>
        /// <param name="propertyname">参数名</param>
        /// <returns></returns>
        [Obsolete("当前变量，已过时，请考虑使用 ObjectExtension.Services 使用 IOC 模式")]
        public static bool IsPropertyExist(string propertyname)
        {
            if (Dynamic is ExpandoObject)
                return (Dynamic as IDictionary<string, object>).ContainsKey(propertyname);//((IDictionary<string, object>)Dynamic)
            return Dynamic.GetType().GetProperty(propertyname) != null;
        }

        ///// <summary>
        ///// 虚拟数组(备注：要引用 <see cref="Microsoft.CSharp"/> .dll 方可使用)
        ///// </summary>
        //public static List<dynamic> Dynamics = new List<dynamic>();

        /// <summary>
        /// 实现万能的转换(备注：包含数据类型的转换也包含as的功能和(类型)的功能)
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="obj">object实体，参数，对象</param>
        /// <returns>返回被指定强转的类型。（异常：存在于强转无效）</returns>
        public static T ToVar<T>(this object obj)// where T : new()
        {
            //__reftype(T);__makeref();__refvalue();__arglist("sb",1024);TypedReference

            Type objtype = typeof(T);

            object Obj = null;
            try
            {
                if (objtype.IsGenericType && objtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    var columnType = objtype.GetGenericArguments();
                    if (columnType.Length > 0)
                    {
                        objtype = columnType[0];
                    }
                }

                Obj = Convert.ChangeType(obj, objtype);

                //TypedReference tr = new TypedReference();

                T TObj = (T)Obj;

                //tr = __makeref(TObj);

                //TypedReference.SetTypedReference(tr,Obj);

                //TObj = __refvalue(tr, T);

                return TObj;
            }
            catch //(Exception)
            {
                throw new System.SystemException(string.Format("该对象(\"{0}\")不能被强转为(\"{1}\")对象！ (备注:中途以被转至为(\"{2}\")对象。)", obj.GetType().FullName, objtype.FullName, Obj == null ? "失败" : Obj.GetType().FullName));
            }

            //return t;
        }

        /// <summary>
        /// 实现万能的转换(备注：包含数据类型的转换也包含as的功能和(类型)的功能)（T:适用于转换失败，无法转换时系统返回的默认结果，用于容错）
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="obj">object实体，参数，对象</param>
        /// <param name="devalue">转换失败时使用默认值</param>
        /// <returns>返回被指定强转的类型。（异常：存在于强转无效）</returns>
        public static T ToTryVar<T>(this object obj, T devalue)// where T : new()
        {
            Type objtype = typeof(T);
            try
            {
                if (objtype.IsGenericType && objtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    var columnType = objtype.GetGenericArguments();
                    if (columnType.Length > 0)
                    {
                        objtype = columnType[0];
                    }
                }

                object Obj = Convert.ChangeType(obj, objtype);

                T TObj = (T)Obj;

                return TObj;
            }
            catch //(Exception)
            {
                return devalue;
            }
        }

        /// <summary>
        /// 实现万能的转换(备注：包含数据类型的转换也包含as的功能和(类型)的功能)
        /// </summary>
        /// <param name="obj">object实体，参数，对象</param>
        /// <param name="type">指定类型的<see cref="Type"/></param>
        /// <param name="istype">表示强转失败时是否抛异常。返回null</param>
        /// <returns>返回被指定强转的类型。（异常：存在于强转无效）</returns>
        public static object ToVar(this object obj, Type type, bool istype = true)// where T : new()
        {
            if (type == null)
            {
                throw new System.SystemException("该type为空！");
            }
            object Obj = null;
            try
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    var columnType = type.GetGenericArguments();
                    if (columnType.Length > 0)
                    {
                        type = columnType[0];
                    }
                }

                Obj = Convert.ChangeType(obj, type);
                return Obj;
            }
            catch //(Exception)
            {
                if (istype)
                {
                    throw new System.SystemException(string.Format("该对象(\"{0}\")不能被强转为(\"{1}\")对象！ (备注:中途以被转至为(\"{2}\")对象。)", obj.GetType().FullName, type.FullName, Obj == null ? "失败" : Obj.GetType().FullName));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 实现万能的转换(备注：包含数据类型的转换也包含as的功能和(类型)的功能)
        /// </summary>
        /// <param name="obj">object实体，参数，对象</param>
        /// <param name="type">强转类型,如果为空，默认强转成该对象的Type类型(必须要是当前程序集下的对象，才能转换)</param>
        /// <returns>返回被指定强转的类型。（异常：存在于强转无效）</returns>
        public static dynamic ToVar(this object obj, string type = null)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                try
                {
                    return Convert.ChangeType(obj, obj.GetType());
                }
                catch
                {
                    throw new System.SystemException(string.Format("该对象(\"{0}\")不能被强转为(\"{1}\")对象！", "object", obj.GetType().FullName));
                }
            }

            try
            {
                Type objtype = null;

                objtype = Type.GetType(type);

                if (objtype == null)
                {
                    Assembly ass = Assembly.GetAssembly(obj.GetType());
                    //Assembly ass = Assembly.Load(type.Split('.')[0]);
                    objtype = ass.GetType(type);
                    if (objtype == null)
                    {
                        var Assemblies = ass.GetReferencedAssemblies();

                        foreach (var Assem in Assemblies)
                        {
                            Assembly ass1 = Assembly.Load(Assem.FullName);
                            objtype = ass1.GetType(type);

                            if (objtype != null)
                            {
                                break;
                            }
                        }
                    }
                }

                object Obj = null;

                try
                {
                    if (objtype.IsGenericType && objtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                        var columnType = objtype.GetGenericArguments();
                        if (columnType.Length > 0)
                        {
                            objtype = columnType[0];
                        }
                    }

                    Obj = Convert.ChangeType(obj, objtype);

                    return Obj;
                }
                catch //(Exception)
                {
                    if (objtype.FullName.Equals("System.String"))
                    {
                        return obj.ToString();
                    }
                    throw new System.SystemException(string.Format("该对象(\"{0}\")不能被强转为(\"{1}\")对象！ (备注:中途以被转至为(\"{2}\")对象。)", obj.GetType().FullName, objtype.FullName, Obj == null ? "失败" : Obj.GetType().FullName));
                }
            }
            catch (Exception)
            {
                throw new System.SystemException(string.Format("该对象(\"{0}\")不能被强转，因为遇见了设计之初未思考到的地方。)", obj.GetType().FullName));
            }


            //TypeCode typeCode = Convert.GetTypeCode(obj);
            //if (typeCode != TypeCode.Empty)
            //{
            //    try
            //    {
            //        return (T)Convert.ChangeType(obj, typeCode);
            //    }
            //    catch
            //    {
            //        throw new System.SystemException(string.Format("该对象(\"{0}\")不能被强转为(\"{1}\")对象！", obj.GetType().FullName, typeCode.ToString()));
            //    }
            //}
        }

        /// <summary>
        /// 表示在 System.Threading.Thread 上执行的方法。
        /// </summary>
        [ComVisible(true)]
        public delegate object ThreadStart();

        /// <summary>
        /// 实现单线程的访问 (示例：new ThreadStart(delegate (){}) )
        /// </summary>
        /// <param name="API"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static object MapTo(ThreadStart API)
        {
            //操作
            return API.Invoke();
        }

        /// <summary>
        /// 实现单线程的访问
        /// </summary>
        /// <param name="obj_T">表示一个类对象实力</param>
        /// <param name="Methods">表示调用的对象的一个方法名(注意该方法必须是非静态方法)</param>
        /// <param name="parameter">该方法的参数,如果该方法没有参数可以为null，如有参数必须和方法参数一致</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static object MapTo(object obj_T, string Methods, params object[] parameter)
        {
            //using
            TypeInvoke asyncInvoke = new TypeInvoke(obj_T.GetType(), obj_T);
            if (!asyncInvoke.IsMethod(Methods, true))
            {
                throw new System.SystemException(string.Format("该类：（{0}）下面不包含这个方法名：（{1}）。", asyncInvoke.ToString(), Methods));
            }
            if (asyncInvoke.GetParameter(Methods).Length != parameter.Length)
            {
                throw new System.SystemException(string.Format("该类：（{0}）下面的方法：（{1}）与指定的参数不一致。", asyncInvoke.ToString(), Methods));
            }

            //操作
            return asyncInvoke.Invoke(Methods, parameter);
        }

        /// <summary>
        /// 获取某个类下面的方法，方法对应的委托参数
        /// </summary>
        /// <param name="Methods">表示调用的对象的方法名</param>
        /// <returns></returns>
        public static ActionDispatcher<T> MapTo<T>(string Methods)
        {
            try
            {
                MethodInfo methodInfo = TypeInvoke.GetMethodInfo<T>(Methods); //Delegate

                ActionDispatcher<T> action = new(methodInfo);

                return action;
            }
            catch //(Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 转换为Xml格式字符串
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>Xml字符串</returns>
        public static string ToXml(this object obj)
        {
            using System.IO.StringWriter sw = new();//Type t = obj.GetType();
            System.Xml.Serialization.XmlSerializer serializer = new(obj.GetType());
            serializer.Serialize(sw, obj);
            sw.Close();
            return sw.ToString();
        }

        /// <summary>
        /// 转换为JSON格式字符串 
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this object obj)
        {
            return obj.ToJson(null);
        }

        /// <summary>
        /// 转换为JSON格式字符串，针对Web场景定制Json格式
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="action">委托Json任务</param>
        /// <returns>JSON字符串</returns>
        public static string ToJsonWeb(this object obj, Action<JsonSerializerOptions> action)
        {
            JsonSerializerOptions jsonSerializer = new(JsonSerializerDefaults.Web);

            action(jsonSerializer);

            return obj.ToJson(jsonSerializer);
        }


        /// <summary>
        /// 转换为JSON格式字符串 
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="jsonSerializerOptions">需要的序列化条件</param>
        /// <returns>JSON字符串</returns>
        public static string ToJson(this object obj, JsonSerializerOptions jsonSerializerOptions)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }
            return JsonSerializer.Serialize(obj, jsonSerializerOptions);
        }

        /// <summary>
        /// 将实体转换为JSON格式字符串 （再三强调，要是实体，而且是单个实体。）
        /// </summary>
        /// <param name="obj">实体</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntityToJson(this object obj)
        {
            return EntityToJson(obj);
        }

        /// <summary>
        /// 将实体转换为JSON格式字符串 （再三强调，要是实体，而且是单个实体。）
        /// </summary>
        /// <param name="obj">实体</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntityToJson(this object obj, bool IsDate)
        {
            return EntityToJson(obj, IsDate, null);
        }

        /// <summary>
        /// 将实体转换为JSON格式字符串 （再三强调，要是实体，而且是单个实体。）
        /// </summary>
        /// <param name="obj">实体</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <param name="ToDateString">Date.ToString()的写法。</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntityToJson(this object obj, bool IsDate, string ToDateString)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }

            IDictionary<string, object> keyValuePairs = obj.GetDictionary();

            if (IsDate)
            {
                Dictionary<string, object> childRow = new();

                foreach (var Pairs in keyValuePairs)
                {
                    var value = Pairs.Value;

                    if (value.GetType() == typeof(DateTime))
                    {
                        DateTime dateTime = value.ToVar<DateTime>();

                        if (!string.IsNullOrWhiteSpace(ToDateString))
                        {
                            childRow.Add(Pairs.Key, dateTime.ToString(ToDateString));
                        }
                        else
                        {
                            childRow.Add(Pairs.Key, dateTime.ToString());
                        }
                    }
                    else
                    {
                        childRow.Add(Pairs.Key, value);
                    }
                }
                return childRow.ToJson();
            }
            return keyValuePairs.ToJson();
        }

        /// <summary>
        /// 将对象转换成Base64字符串（编码）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.", DiagnosticId = "SYSLIB0011", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
        public static string ToBase64String(this object obj)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }
            //byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(obj.ToBytes());
        }

        /// <summary>
        ///  将一个object对象序列化，返回一个byte[]（重要说明：被序列化的对象必须实现 [Serializable] <see cref="ISerializable"/>特性的结构）
        /// </summary>
        /// <param name="obj">能序列化的对象</param>
        /// <returns>返回一个byte[]</returns>
        [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.", DiagnosticId = "SYSLIB0011", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
        public static byte[] ToBytes(this object obj)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }
            using System.IO.MemoryStream ms = new System.IO.MemoryStream();
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(ms, obj);
            return ms.GetBuffer();
        }

        /// <summary>
        ///  将一个object对象序列化，返回一个byte[] （升级版，有效降低内存消耗）（重要说明：被序列化的对象必须实现 struct 标记）示例：public struct 类名称
        /// </summary>
        /// <param name="obj">能序列化的对象</param>
        /// <param name="type">转换为原来类的Type</param>
        /// <returns>返回一个byte[]</returns>
        public static byte[] ToBytes(this object obj, Type type)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }
            byte[] buff = new byte[Marshal.SizeOf(obj)];
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);
            Marshal.StructureToPtr(obj, ptr, true);
            return buff;
        }

        /// <summary>
        /// 将指定的内存空间内容转换成类型
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="address">内存空间</param>
        /// <returns>返回类型对象</returns>
        public static T Read<T>(IntPtr address)
        {
            var obj = default(T);
            var tr = __makeref(obj);
            unsafe { *(IntPtr*)(&tr) = address; }
            return __refvalue(tr, T);
        }

        /// <summary>
        /// 将指定的内存空间内容转换成类型
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="address">内存空间值</param>
        /// <returns>返回类型对象</returns>
        public static T Read<T>(int address)
        {
            IntPtr Address = new IntPtr(address);
            var obj = default(T);
            var tr = __makeref(obj);
            unsafe { *(IntPtr*)(&tr) = Address; }
            return __refvalue(tr, T);
        }

        /// <summary>
        /// 获取当前对象的内存空间
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回指针</returns>
        public static IntPtr GetIntPtr(this object obj)
        {
            //_ = new GCHandle();
            try
            {
                GCHandle hander = GCHandle.Alloc(obj);

                IntPtr intPtr = GCHandle.ToIntPtr(hander);
                //hander.Free();
                return intPtr;
            }
            catch (Exception)
            {
                throw new System.SystemException(string.Format("该对象(\"{0}\")不能被固定内存指针！", obj.GetType().ToString()));
            }
            //finally
            //{
            //    if (hander.IsAllocated)
            //    {
            //        hander.Free();
            //    }
            //}
        }

        /// <summary>
        /// 获取当前对象的内存空间
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回指针</returns>
        public static int GetIntPtrInt(this object obj)
        {
            //_ = new GCHandle();
            try
            {
                GCHandle hander = GCHandle.Alloc(obj);
                return GCHandle.ToIntPtr(hander).ToString().ToInt();
            }
            catch (Exception)
            {
                //__refvalue(tr, string);
                //__reftype(obj).ToString();
                throw new System.SystemException(string.Format("该对象(\"{0}\")不能被固定内存指针！", obj.GetType().ToString()));
            }
            //finally
            //{
            //    if (hander.IsAllocated)
            //    {
            //        hander.Free();
            //    }
            //}
        }

        #region Object[] 封装方法

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">Object[]</param>
        /// <param name="txt1">新增的值</param>
        /// <returns>由于不能直接赋值，采取间接赋值方式</returns>
        public static object[] Add(this object[] txt, object txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 给数组加新的值(适用于任何数组对象)
        /// </summary>
        /// <typeparam name="T">数组原类型</typeparam>
        /// <param name="obj">Object[]源数组</param>
        /// <param name="_obj">新数组（为空，但是必须大于原数组一个下标以上）</param>
        /// <param name="T_obj">加入的新值</param>
        /// <returns></returns>
        public static void Add<T>(this object obj, [In] [Out] object _obj, object T_obj)// where T : new()
        {
            var obj_1 = obj.ToVar<T>();

            var _obj_1 = _obj.ToVar<T>();

            if ((obj_1 as Array).Length >= (_obj_1 as Array).Length)
            {
                throw new System.SystemException("新的数组必须大于当源数组一个下标以上！");
            }

            Array.Copy((obj_1 as Array), 0, (_obj_1 as Array), 0, (obj_1 as Array).Length);

            (_obj_1 as Array).SetValue(T_obj, (obj_1 as Array).Length);

            _obj = _obj_1;
        }

        /// <summary>
        /// 查找该Object数组中是否存在该值。
        /// </summary>
        /// <param name="txt">Object[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this object[] txt, object txt1)
        {
            return txt.Contains<object>(txt1);
        }

        /// <summary>
        /// 重写封装的Copy方法 (读取原数组中指定位置的内容)
        /// </summary>
        /// <param name="sourceArray">源数组对象</param>
        /// <param name="destinationArray">新数组对象</param>
        /// <param name="sourceIndex">源数据开始读取的位置</param>
        /// <param name="length">从源数组取多少？(是指从读取位置开始往后读的数量)</param>
        /// <returns>返回当前新的数组中复制了多少个下标的值</returns>
        public static int Read<T>(this object sourceArray, [In] [Out] object destinationArray, int sourceIndex, int length)
        {
            return Read<T>(sourceArray, sourceIndex, destinationArray, 0, length);
        }

        /// <summary>
        /// 重写封装的Copy方法 (读取原数组中指定位置的内容)
        /// </summary>
        /// <param name="sourceArray">源数组对象</param>
        /// <param name="sourceIndex">源数据开始读取的位置</param>
        /// <param name="destinationArray">新数组对象</param>
        /// <param name="destinationIndex">开始存储的位置</param>
        /// <param name="length">从源数组取多少？(是指从读取位置开始往后读的数量)</param>
        /// <returns>返回当前新的数组中复制了多少个下标的值</returns>
        public static int Read<T>(this object sourceArray, int sourceIndex, [In] [Out] object destinationArray, int destinationIndex, int length)
        {
            var sourceArray_1 = sourceArray.ToVar<T>();

            var destinationArray_1 = destinationArray.ToVar<T>();

            if (sourceArray_1 == null)
            {
                throw new System.SystemException($"该{typeof(T).Name}为空！");
            }
            if (sourceIndex < 0)
            {
                throw new System.SystemException("sourceIndex不能小于0，数组越界！");
            }
            if (length <= 0)
            {
                throw new System.SystemException("length不能小于或等于0，数组异常！");
            }
            if ((sourceArray_1 as Array).Length < sourceIndex)
            {
                throw new System.SystemException("sourceIndex超出了数组，数组越界！");
            }

            int Length = length <= ((sourceArray_1 as Array).Length - sourceIndex) ? length : ((sourceArray_1 as Array).Length - sourceIndex);

            if ((destinationArray as Array).Length < destinationIndex + Length)
            {
                throw new System.SystemException($"destinationArray数组容量不够必须大于或等于{destinationIndex + Length}，数组越界！");
            }

            Array.Copy((sourceArray_1 as Array), sourceIndex, (destinationArray as Array), destinationIndex, Length);

            return Length;
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static object[] GetArrayIndex(this object[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }
            if (index > count)
            {
                throw new System.SystemException("count不能小于index，数组越界！");
            }
            if (index < 0)
            {
                throw new System.SystemException("index不能小于0，数组越界！");
            }
            if (count < 0)
            {
                throw new System.SystemException("count不能小于0，数组越界！");
            }
            if (obj.Length < index)
            {
                throw new System.SystemException("index超出了数组，数组越界！");
            }
            if (obj.Length < count)
            {
                throw new System.SystemException("count超出了数组，数组越界！");
            }
            List<object> obj1 = new List<object>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion
    }
}
