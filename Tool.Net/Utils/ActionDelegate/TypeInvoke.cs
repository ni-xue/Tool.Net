using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 用于对指定的类的方法进行调用，中级封装
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class TypeInvoke : IDisposable
    {
        private Type type;

        private object obj;

        /// <summary>
        /// 获取当前被实例化的类的，名称包含命名空间
        /// </summary>
        /// <returns>返回类的名称包含命名空间</returns>
        public override string ToString()
        {
            if (type != null)
            {
                return type.FullName;
            }
            return null;
        }

        /// <summary>
        /// 获取当前被实例化的类的Type
        /// </summary>
        /// <returns>返回Type</returns>
        public new Type GetType()
        {
            return type;
        }

        /// <summary>
        /// 获取当前被实例化的类
        /// </summary>
        /// <returns>返回实例化的类</returns>
        public object GetObj()
        {
            return obj;
        }

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="type">(访问程序集下面指定的类路径)要获取的类型的程序集限定名称。 如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll  中，则提供由命名空间限定的类型名称就足够了。</param>
        public TypeInvoke(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            Assembly ass = Assembly.Load(type.Split('.')[0]);
            var t = ass.GetType(type);
            //if (t == null)
            //{
            //    throw new ArgumentNullException("未找到此类！");
            //}
            //this.type = t;
            //通过程序集名称返回Assembly对象
            //Assembly ass = Assembly.Load("Universal_Parent_Class");
            //通过DLL文件名称返回Assembly对象
            //Assembly ass = Assembly.LoadFrom("Universal_Parent_Class.dll");
            //通过Assembly获取程序集中类
            this.type = t ?? throw new ArgumentNullException("未找到此类！");
            this.obj = Activator.CreateInstance(this.type);
        }

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="type">(访问程序集下面指定的类路径)要获取的类型的程序集限定名称。 如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll  中，则提供由命名空间限定的类型名称就足够了。</param>
        /// <param name="args">与要调用构造函数的参数数量、顺序和类型匹配的参数数组。 如果 args 为空数组或 null，则调用不带任何参数的构造函数（默认构造函数）。</param>
        public TypeInvoke(string type, params object[] args)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            Assembly ass = Assembly.Load(type.Split('.')[0]);
            var t = ass.GetType(type);
            //if (t == null)
            //{
            //    throw new ArgumentNullException("未找到此类！");
            //}
            //this.type = t;
            //通过程序集名称返回Assembly对象
            //Assembly ass = Assembly.Load("Universal_Parent_Class");
            //通过DLL文件名称返回Assembly对象
            //Assembly ass = Assembly.LoadFrom("Universal_Parent_Class.dll");
            //通过Assembly获取程序集中类
            this.type = t ?? throw new ArgumentNullException("未找到此类！");
            this.obj = Activator.CreateInstance(this.type, args);
        }

        /// <summary>
        /// 加载指定路径下的dll中的类对象（无参构造）
        /// </summary>
        /// <param name="absolutepath">dll的绝对路径</param>
        /// <param name="type">访问程序集下面指定的类路径</param>
        public TypeInvoke(string absolutepath, string type)
        {
            if (string.IsNullOrEmpty(absolutepath) && string.IsNullOrEmpty(type))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            if (!File.Exists(absolutepath))
            {
                throw new System.SystemException("需要加载的dll的路径不正确，为找到要加载的dll！");
            }

            Assembly ass = Assembly.LoadFile(absolutepath);
            var t = ass.GetType(type);

            //通过Assembly获取程序集中类
            this.type = t ?? throw new ArgumentNullException("未找到此类！");
            this.obj = Activator.CreateInstance(this.type);
        }

        /// <summary>
        /// 加载指定路径下的dll中的类对象（有参构造）
        /// </summary>
        /// <param name="absolutepath">dll的绝对路径</param>
        /// <param name="type">访问程序集下面指定的类路径</param>
        /// <param name="args"></param>
        public TypeInvoke(string absolutepath, string type, params object[] args)
        {
            if (string.IsNullOrEmpty(absolutepath) && string.IsNullOrEmpty(type))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            if (!File.Exists(absolutepath))
            {
                throw new System.SystemException("需要加载的dll的路径不正确，为找到要加载的dll！");
            }

            Assembly ass = Assembly.LoadFile(absolutepath);
            var t = ass.GetType(type);

            //通过Assembly获取程序集中类
            this.type = t ?? throw new ArgumentNullException("未找到此类！");
            this.obj = Activator.CreateInstance(this.type, args);
        }
        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="type">(访问程序集下面指定的类路径)要获取的类型的程序集限定名称。 如果该类型位于当前正在执行的程序集中或者 Mscorlib.dll  中，则提供由命名空间限定的类型名称就足够了。</param>
        /// <param name="args">当前实例化的对象。</param>
        public TypeInvoke(string type, object args)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            Assembly ass = Assembly.Load(type.Split('.')[0]);
            var t = ass.GetType(type);
            //if (t == null)
            //{
            //    throw new ArgumentNullException("未找到此类！");
            //}
            //this.type = t;
            //通过程序集名称返回Assembly对象
            //Assembly ass = Assembly.Load("Universal_Parent_Class");
            //通过DLL文件名称返回Assembly对象
            //Assembly ass = Assembly.LoadFrom("Universal_Parent_Class.dll");
            //通过Assembly获取程序集中类
            this.type = t ?? throw new ArgumentNullException("未找到此类！");
            this.obj = args;
        }

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="type">表示类型声明：类类型、接口类型、数组类型、值类型、枚举类型、类型参数、泛型类型定义，以及开放或封闭构造的泛型类型。</param>
        public TypeInvoke(Type type)
        {
            this.type = type;
            this.obj = Activator.CreateInstance(this.type);
        }

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="obj">必须是对象</param>
        public TypeInvoke(object obj)
        {
            this.type = obj.GetType();
            this.obj = obj;
        }

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="type">表示类型声明：类类型、接口类型、数组类型、值类型、枚举类型、类型参数、泛型类型定义，以及开放或封闭构造的泛型类型。</param>
        /// <param name="args">与要调用构造函数的参数数量、顺序和类型匹配的参数数组。 如果 args 为空数组或 null，则调用不带任何参数的构造函数（默认构造函数）。</param>
        public TypeInvoke(Type type, params object[] args)
        {
            this.type = type;
            this.obj = Activator.CreateInstance(this.type, args);
        }

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="type">表示类型声明：类类型、接口类型、数组类型、值类型、枚举类型、类型参数、泛型类型定义，以及开放或封闭构造的泛型类型。</param>
        /// <param name="args">当前实例化的对象。</param>
        public TypeInvoke(Type type, object args)
        {
            this.type = type;
            this.obj = args;
        }

        /// <summary>
        /// 获取类下面的可调用方法
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="MethodName">方法名</param>
        /// <returns>返回<see cref="MethodInfo"/>对象</returns>
        public static MethodInfo GetMethodInfo<T>(string MethodName)
        {
            return GetMethodInfo(typeof(T), MethodName);
        }

        /// <summary>
        /// 获取类下面的可调用方法
        /// </summary>
        /// <param name="type">类Type</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="only">获取父类的吗？默认包含</param>
        /// <returns>返回<see cref="MethodInfo"/>对象</returns>
        public static MethodInfo GetMethodInfo(Type type, string MethodName, bool only = true)
        {
            MethodInfo method = type.GetMethod(MethodName, only ? BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public : BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            return method;
        }

        /// <summary>
        /// 调起该方法(“包含非公开”、“包含实例成员”和“包含公开”)
        /// </summary>
        /// <param name="name">方法名(注意该方法必须是非静态方法)</param>
        /// <returns>返回该方法执行后的结果</returns>
        public object Invoke(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            MethodInfo method = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                throw new ArgumentNullException("未找到此方法！");
            }
            var data = method?.Invoke(obj, null);

            return data;
        }

        /// <summary>
        /// 调起该方法(“包含非公开”、“包含实例成员”和“包含公开”)
        /// </summary>
        /// <param name="name">方法名(注意该方法必须是非静态方法)</param>
        /// <param name="parameters">该方法的指定参数</param>
        /// <returns>返回该方法执行后的结果</returns>
        public object Invoke(string name, params object[] parameters)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            if (parameters == null || parameters.Length == 0)
            {
                throw new ArgumentNullException("方法参数不能为空！");
            }
            MethodInfo method = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                throw new ArgumentNullException("未找到此方法！");
            }
            //MethodInfo[] info = type.GetMethods();
            var data = method?.Invoke(obj, parameters);

            return data;
        }

        /// <summary>
        /// 判断该方法是否存在 (“包含非公开”、“包含实例成员”和“包含公开”)
        /// </summary>
        /// <param name="name">方法名(注意该方法必须是非静态方法)</param>
        /// <param name="only">获取父类的吗？默认包含</param>
        /// <returns>返回是否存在</returns>
        public bool IsMethod(string name, bool only = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            MethodInfo method = type.GetMethod(name, only ? BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return method != null ? true : false;
        }

        /// <summary>
        /// 获取该方法类型(“包含非公开”、“包含实例成员”和“包含公开”)
        /// </summary>
        /// <param name="name">方法名(注意该方法必须是非静态方法)</param>
        /// <param name="only">获取父类的吗？默认包含</param>
        /// <returns>返回一个类型实体</returns>
        public Method GetMethod(string name, bool only = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            MethodInfo method = type.GetMethod(name, only ? BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (method == null)
            {
                throw new ArgumentNullException("未找到此方法！");
            }
            return new Method(method.ReflectedType.Name, method.Name, method.ReturnType, getType(method.MemberType));
        }

        /// <summary>
        /// 获取该方法类型(“包含非公开”、“包含实例成员”和“包含公开”)
        /// </summary>
        /// <param name="method">对象</param>
        /// <returns>返回一个类型实体</returns>
        public static Method GetMethod(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("对象不能为空！");
            }
            return new Method(method.ReflectedType.Name, method.Name, method.ReturnType, getType(method.MemberType));
        }

        /// <summary>
        /// 获取该类下的所有方法(“包含非公开”、“包含实例成员”和“包含公开”)
        /// </summary>
        /// <param name="only">获取父类的吗？默认包含</param>
        /// <returns>返回一个类型实体</returns>
        public Method[] GetMethods(bool only = true)
        {
            MethodInfo[] method = type.GetMethods(only ? BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (method.Length < 1 || method == null)
            {
                throw new ArgumentNullException("未找到此方法！");
            }
            List<Method> methods = new List<Method>();
            foreach (var Method in method)
            {
                methods.Add(new Method(Method.ReflectedType.Name, Method.Name, Method.ReturnType, getType(Method.MemberType)));
            }
            return methods.ToArray();
        }

        /// <summary>
        /// 获取它的类型
        /// </summary>
        /// <param name="types">类型</param>
        /// <returns>返回类型名称</returns>
        private static string getType(MemberTypes types)
        {
            switch (types)
            {
                case MemberTypes.Constructor:
                    {
                        return "构造函数";
                    }
                case MemberTypes.Event:
                    {
                        return "事件";
                    }
                case MemberTypes.Field:
                    {
                        return "字段";
                    }
                case MemberTypes.Method:
                    {
                        return "方法";
                    }
                case MemberTypes.Property:
                    {
                        return "属性";
                    }
                case MemberTypes.TypeInfo:
                    {
                        return "类型";
                    }
                case MemberTypes.Custom:
                    {
                        return "自定义成员类型";
                    }
                case MemberTypes.NestedType:
                    {
                        return "嵌套类型";
                    }
                case MemberTypes.All:
                    {
                        return "所有成员类型";
                    }
                default:
                    {
                        return "未知";
                    }
            }
        }

        /// <summary>
        /// 获取该方法需要的参数
        /// </summary>
        /// <param name="name">方法名(注意该方法必须是非静态方法)</param>
        /// <returns>返回所有的参数</returns>
        public Parameter[] GetParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            MethodInfo method = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                throw new ArgumentNullException("未找到此方法！");
            }
            ParameterInfo[] member = method.GetParameters();
            return GetParameter(member);
        }

        /// <summary>
        /// 获取该方法需要的参数
        /// </summary>
        /// <param name="method">对象</param>
        /// <returns>返回所有的参数</returns>
        public static Parameter[] GetParameter(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("对象不能为空！");
            }
            ParameterInfo[] member = method.GetParameters();
            return GetParameter(member);
        }

        /// <summary>
        /// 获取该方法需要的参数
        /// </summary>
        /// <param name="member">对象</param>
        /// <returns>返回所有的参数</returns>
        public static Parameter[] GetParameter(params ParameterInfo[] member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("对象不能为空！");
            }
            List<Parameter> members = new();
            foreach (var Member in member)
            {
                members.Add(new Parameter(Member)); //(Member.Name, Member.ParameterType.Name, Member.ParameterType.Namespace, Member.ParameterType, Member.DefaultValue)
            }
            return members.ToArray();
        }

        /// <summary>
        /// 获取该类指定的变量的值。
        /// </summary>
        /// <param name="name">变量名</param>
        /// <returns>返回该方法的值</returns>
        public object GetProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            FieldInfo finfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (finfo == null)
            {
                finfo = type.GetField(string.Format("<{0}>k__BackingField", name), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (finfo == null)
                {
                    throw new ArgumentNullException("未找到此变量！");
                }
            }
            return finfo.GetValue(obj);

            // PropertyInfo finfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            // if (finfo == null)
            // {
            //     throw new ArgumentNullException("未找到此变量！");
            // }
            // MethodInfo getinfo = finfo.GetGetMethod(true);

            //return getinfo.Invoke(obj,null);
        }

        /// <summary>
        /// 对该类指定的变量赋值。
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="parameter">值</param>
        public void SetProperty(string name, object parameter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.SystemException("传入的参数不能为空！");
            }
            if (parameter == null)
            {
                return;
            }
            FieldInfo finfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (finfo == null)
            {
                finfo = type.GetField(string.Format("<{0}>k__BackingField", name), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (finfo == null)
                {
                    finfo = type.GetField(string.Format("m_value", name), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (finfo == null)
                    {
                        throw new ArgumentNullException("未找到此变量！");
                    }
                }
            }
            finfo.SetValue(obj, parameter);


            //PropertyInfo finfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            //if (finfo == null)
            //{
            //    throw new ArgumentNullException("未找到此变量！");
            //}
            //MethodInfo setinfo = finfo.GetSetMethod(true);

            //setinfo.Invoke(obj, parameters);
        }

        /// <summary>
        /// 获取该类以及下面的所有类型（包涵，方法变量，字段，接口等等）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="only">获取父类的吗？默认包含</param>
        /// <param name="Methods">返回名称</param>
        /// <returns>返回一个类型实体</returns>
        public T[] GetAttribute<T>(out string[] Methods, bool only = true)// where T : new()
        {
            return GetAttribute<T>(out Methods, only ? BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        }

        /// <summary>
        /// 获取该类以及下面的所有类型（包涵，方法变量，字段，接口等等）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bindingFlags">自定义返回内容</param>
        /// <param name="Methods">返回名称</param>
        /// <returns>返回一个类型实体</returns>
        public T[] GetAttribute<T>(out string[] Methods, BindingFlags bindingFlags)// where T : new()
        {
            MethodInfo[] method = type.GetMethods(bindingFlags);
            if (method.Length < 1 || method == null)
            {
                throw new ArgumentNullException("未找到此方法！");
            }
            List<T> ts = new List<T>();
            List<string> meths = new List<string>();
            foreach (var Method in method)
            {
                meths.Add(Method.Name);
                var hobbyAttr = Attribute.GetCustomAttribute(Method, typeof(T)).ToVar<T>();
                ts.Add(hobbyAttr);
            }
            Methods = meths.ToArray();
            return ts.ToArray();
        }

        /// <summary>
        /// 获取该类以及下面的所有类型（包涵，方法变量，字段，接口等等）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bindingFlags">自定义返回内容</param>
        /// <returns>返回一个类型实体</returns>
        public Dictionary<string, T> GetAttribute<T>(BindingFlags bindingFlags)// where T : new()
        {
            MethodInfo[] method = type.GetMethods(bindingFlags);
            if (method.Length < 1 || method == null)
            {
                return null;
            }
            Dictionary<string, T> keyValues = new Dictionary<string, T>();
            foreach (var Method in method)
            {
                var hobbyAttr = Attribute.GetCustomAttribute(Method, typeof(T)).ToVar<T>();
                keyValues.Add(Method.Name, hobbyAttr);
            }
            return keyValues;
        }

        /// <summary>
        /// 获取该类以及下面的所有类型（字段）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>返回一个类型实体</returns>
        public Dictionary<string, T> GetAttributeParameterInfo<T>()// where T : new()
        {
            return GetAttributeParameterInfo<T>(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        }

        /// <summary>
        /// 获取该类以及下面的所有类型（字段）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bindingFlags">自定义返回内容</param>
        /// <returns>返回一个类型实体</returns>
        public Dictionary<string, T> GetAttributeParameterInfo<T>(BindingFlags bindingFlags)// where T : new()
        {
            PropertyInfo[] method = type.GetProperties(bindingFlags);
            if (method.Length < 1 || method == null)
            {
                return null;
            }
            Dictionary<string, T> keyValues = new Dictionary<string, T>();
            foreach (var Method in method)
            {
                var hobbyAttr = Attribute.GetCustomAttribute(Method, typeof(T)).ToVar<T>();
                keyValues.Add(Method.Name, hobbyAttr);
            }
            return keyValues;
        }

        /// <summary>
        /// 资源是否已被清理
        /// </summary>
        public bool IsDisposed = false;

        /// <summary>
        /// 底层资源回收
        /// </summary>
        ~TypeInvoke()
        {
            Dispose(false);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="Disposing">为（true时清空掉对象的数据）</param>
        public void Dispose(bool Disposing)
        {
            if (!IsDisposed)
            {
                if (Disposing)
                {
                    //清理托管资源
                    //清理非托管资源
                    type = null;
                    obj = null;
                    GC.SuppressFinalize(this);

                    IsDisposed = true;
                }
            }
        }
    }

    /// <summary>
    /// 获取指定类下面的所有方法，低级封装
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed partial class Method
    {
        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="ReflectedType"></param>
        /// <param name="Name"></param>
        /// <param name="ReturnType"></param>
        /// <param name="TypeName"></param>
        internal Method(string ReflectedType, string Name, Type ReturnType, string TypeName)
        {
            this.ReflectedType = ReflectedType;
            this.Name = Name;
            this.ReturnType = ReturnType;
            this.ReturnTypeName = ReturnType.Name;
            this.TypeName = TypeName;
        }

        /// <summary>
        /// 所在的类名称
        /// </summary>
        public string ReflectedType { get; }

        /// <summary>
        /// 方法的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 名称：类型
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// 方法的返回类型字符串
        /// </summary>
        public string ReturnTypeName { get; }

        /// <summary>
        /// 方法的返回类型
        /// </summary>
        public Type ReturnType { get; }
    }
}
