using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 定义构造函数
    /// </summary>
    /// <typeparam name="E">创建的类</typeparam>
    /// <param name="parameters">包含的参数</param>
    /// <returns></returns>
    public delegate E NewClass<out E>(object[] parameters);


    /// <summary>
    /// 根据 ConstructorInfo 对象，创建一个委托，实现类调用，提高性能，支持各种返回值
    /// </summary>
    public sealed class ClassDispatcher : ClassDispatcher<object>
    {
        /// <summary>
        /// 根据构造函数，创建对象委托
        /// </summary>
        /// <param name="constructor">构造函数对象</param>
        public ClassDispatcher(ConstructorInfo constructor) : base(constructor) { }

        /// <summary>
        /// 根据构造函数，创建对象委托
        /// </summary>
        /// <param name="classtype">对象类型</param>
        public ClassDispatcher(Type classtype) : base(classtype) { }

        /// <summary>
        /// 创建 New 对象
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>返回 New 新对象</returns>
        public T Invoke<T>(params object[] parameters)
        {   
            object obj = Invoke(parameters);
            return (T)obj;
        }
    }

    /// <summary>
    /// 根据 ConstructorInfo 对象，创建一个委托，实现类调用，提高性能，支持各种返回值
    /// </summary>
    /// <typeparam name="T">返回任何类型</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ClassDispatcher<T>
    {
        private readonly NewClass<T> _newclass;

        /// <summary>
        /// 实例化对象类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 方法参数
        /// </summary>
        public Parameter[] Parameters { get; }

        /// <summary>
        /// 根据构造函数，创建对象委托
        /// </summary>
        public ClassDispatcher() : this(typeof(T))
        {
        }

        /// <summary>
        /// 根据构造函数，创建对象委托
        /// </summary>
        /// <param name="constructor">构造函数对象</param>
        public ClassDispatcher(ConstructorInfo constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor), "参数为空！");

            this.Parameters = TypeInvoke.GetParameter(constructor.GetParameters());

            this.Type = constructor.DeclaringType;

            _newclass = GetClass(constructor);
        }

        /// <summary>
        /// 根据构造函数，创建对象委托
        /// </summary>
        /// <param name="classtype">对象类型</param>
        public ClassDispatcher(Type classtype)
        {
            if (classtype == null) throw new ArgumentNullException(nameof(classtype), "参数为空！");

            var constructorInfos = classtype.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructorInfos.Length == 0) throw new Exception(string.Format("类：{0}，无法获取到公开的构造函数，无法创建消息体。", classtype.FullName));

            if (constructorInfos.Length > 1) throw new Exception(string.Format("类：{0}，存在多个构造函数，无法创建消息体。", classtype.FullName));

            this.Parameters = TypeInvoke.GetParameter(constructorInfos[0].GetParameters());

            this.Type = classtype;

            _newclass = GetClass(constructorInfos[0]);
        }

        /// <summary>
        /// 创建 New 对象
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>返回 New 新对象</returns>
        public T Invoke(params object[] parameters)
        {
            if (this.Parameters.Length > 0)
            {
                if (parameters.Length != this.Parameters.Length)
                {
                    throw new Exception("所传参数，与实际方法参数不一致。");
                }
            }
            return _newclass(parameters);
        }

        /// <summary>
        /// 通过构造器，直接获取相关构造委托
        /// </summary>
        /// <param name="constructor">构造器</param>
        /// <returns>构造委托</returns>
        public static NewClass<T> GetClass(ConstructorInfo constructor)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor), "参数为空！");

            var as0 = System.Linq.Expressions.Expression.Parameter(typeof(object[]), "parameters");

            if (!DispatcherCore.GetParameter(out List<System.Linq.Expressions.Expression> expressions, as0, constructor.GetParameters()))
            {
                throw new ArgumentNullException(nameof(constructor), "无法创建该对象构造，构造参数存在未知问题，请将相关问题，上报给作者！");
            }

            var as1 = System.Linq.Expressions.Expression.New(constructor, expressions);

            //System.Linq.Expressions.MethodCallExpression methodCall = System.Linq.Expressions.Expression.Call(as1, null, expressions);//(MethodInfo)(MethodBase)constructorInfos[0]

            var as2 = System.Linq.Expressions.Expression.Lambda<NewClass<T>>(as1, as0);

            return as2.Compile();
        }
    }
}
