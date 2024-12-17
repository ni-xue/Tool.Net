using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 创建通用调用函数模型
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DispatcherCore
    {
        internal readonly static MethodInfo AsTask = typeof(ValueTask).GetMethod("AsTask");
        internal readonly static MethodInfo ObjectTask = typeof(DispatcherCore).GetMethod("ObjectAsync");
        internal readonly static MethodInfo ObjectValueTask = typeof(DispatcherCore).GetMethod("ObjectValueAsync");

        private static MethodInfo ObjectTaskMethod(params Type[] typeArguments) => ObjectTask.AddMake(typeArguments);
        private static MethodInfo ObjectValueTaskMethod(params Type[] typeArguments) => ObjectValueTask.AddMake(typeArguments);
        private static void GetTypeArguments(ref Type ReturnType, ref bool istask)
        {
            if (ReturnType.IsGenericType)
            {
                Type[] types = ReturnType.GetGenericArguments();
                if (types.Length > 0)
                {
                    bool isok = true;
                    if (object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(Task<>))) istask = true;
                    else if (object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(ValueTask<>))) istask = false;
                    else isok = false;
                    if (isok)
                    {
                        ReturnType = types[0];
                        return;
                    }
                }
            }
            throw new Exception("预期之外的调用！");
        }

        /// <summary>
        /// 给无形参的方法指定参数类型
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="typeArguments">指定形参</param>
        /// <returns>返回具有形参的方法</returns>
        /// <exception cref="ArgumentNullException">值为空</exception>
        public static MethodInfo AddMake(this MethodInfo method, params Type[] typeArguments)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (typeArguments is null)
            {
                throw new ArgumentNullException(nameof(typeArguments));
            }
            return method.MakeGenericMethod(typeArguments);
        }

        /// <summary>
        /// 有用转换
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<T> ObjectAsync<TResult, T>(Task<TResult> task)
        {
            TResult obj = await task;
            if (obj is T obj0)
            {
                return obj0;
            }
            //return Convert.ChangeType(obj, typeof(T));
            //return (T)(obj as object);//Convert (T)obj;
            return Unsafe.As<TResult, T>(ref obj);

            //return task.ContinueWith(Get);
            //static T Get(Task<TResult> result) 
            //{
            //    if (result.IsFaulted) throw result.Exception;
            //    TResult obj = result.Result;
            //    if (result is T obj0)
            //    {
            //        return obj0;
            //    }
            //    return Unsafe.As<TResult, T>(ref obj);
            //}
        }

        /// <summary>
        /// 有用转换
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task<T> ObjectValueAsync<TResult, T>(ValueTask<TResult> task) => ObjectAsync<TResult, T>(task.AsTask());

        internal static MethodInfo ToTask<TResult>(Type ReturnType)
        {
            Type[] typeArguments = new Type[] { ReturnType, typeof(TResult) };
            bool istask = false;
            GetTypeArguments(ref typeArguments[0], ref istask);
            return istask ? ObjectTaskMethod(typeArguments) : ObjectValueTaskMethod(typeArguments);
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="classtype">调用的返回类</param>
        /// <typeparam name="TResult">调用返回类型</typeparam>
        /// <returns>调用委托</returns>
        public static Delegate GetExecutor<TResult>(MethodInfo methodInfo, Type classtype)
        {
            return GetExecutor<object, TResult>(methodInfo, classtype);
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <typeparam name="E">调用函数使用类</typeparam>
        /// <typeparam name="TResult">调用返回类型</typeparam>
        /// <param name="methodInfo">方法对象</param>
        /// <returns>调用委托</returns>
        public static Delegate GetExecutor<E, TResult>(MethodInfo methodInfo)
        {
            return GetExecutor<E, TResult>(methodInfo, typeof(E));
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns>返回委托类型</returns>
        public static Delegate GetExecutor(MethodInfo methodInfo)
        {
            return GetExecutor(methodInfo, methodInfo.DeclaringType);
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="classtype">调用的返回类</param>
        /// <returns>调用委托</returns>
        public static Delegate GetExecutor(MethodInfo methodInfo, Type classtype)
        {
            return GetExecutor<object, object>(methodInfo, classtype);
        }

        /// <summary>
        /// 创建不同的委托（泛型方法版）
        /// </summary>
        /// <typeparam name="E">调用函数使用类</typeparam>
        /// <typeparam name="TResult">调用返回类型</typeparam>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="classtype">调用的返回类</param>
        /// <param name="typeArguments">未泛型方法提供的形参</param>
        /// <returns>调用委托</returns>
        public static Delegate GetExecutor<E, TResult>(MethodInfo methodInfo, Type classtype, params Type[] typeArguments)
        {
            if (!methodInfo.ContainsGenericParameters)
            {
                throw new Exception("当前调用的方法非泛型方法！");
            }
            return GetExecutor<E, TResult>(methodInfo.AddMake(typeArguments), classtype);
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <typeparam name="E">调用函数使用类</typeparam>
        /// <typeparam name="TResult">调用返回类型</typeparam>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="classtype">调用的返回类</param>
        /// <returns>调用委托</returns>
        public static Delegate GetExecutor<E, TResult>(MethodInfo methodInfo, Type classtype)
        {
            // Parameters to executor
            ParameterExpression controllerParameter = Expression.Parameter(classtype, "callclass");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            if (!GetParameter(out List<Expression> parameters, parametersParameter, methodInfo.GetParameters()))
            {
                System.Diagnostics.Debug.WriteLine($"方法：{methodInfo},无法创建委托进行调用。");
                return null;
            }
            // Call method
            Expression instanceCast = (!methodInfo.IsStatic) ? IsAs(controllerParameter, methodInfo.ReflectedType) : null; // Expression.Convert(controllerParameter, methodInfo.ReflectedType)
            MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameters);// = methodCall

            if (MethodIsTask(methodCall.Type, out var isvoid))
            {
                if (isvoid)
                {
                    if (MethodIsValueTask(methodCall.Type)) methodCall = Expression.Call(methodCall, AsTask);
                    Expression<VoidTaskActionExecutor<E>> lambda = Expression.Lambda<VoidTaskActionExecutor<E>>(methodCall, controllerParameter, parametersParameter);
                    VoidTaskActionExecutor<E> voidExecutor = lambda.Compile();
                    return voidExecutor;
                }
                else
                {
                    MethodCallExpression castMethodCall = IsAsTask<TResult>(methodCall);
                    Expression<TaskActionExecutor<E, TResult>> lambda = Expression.Lambda<TaskActionExecutor<E, TResult>>(castMethodCall, controllerParameter, parametersParameter);
                    return lambda.Compile();
                }
            }
            else if (IsVoid(methodCall.Type))
            {
                Expression<VoidActionExecutor<E>> lambda = Expression.Lambda<VoidActionExecutor<E>>(methodCall, controllerParameter, parametersParameter);
                return lambda.Compile();
            }
            else
            {
                Expression castMethodCall = IsAs<TResult>(methodCall);
                Expression<ActionExecutor<E, TResult>> lambda = Expression.Lambda<ActionExecutor<E, TResult>>(castMethodCall, controllerParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// 用于特殊的表达式树式
        /// </summary>
        /// <param name="expressions">返回表达参数结果</param>
        /// <param name="parametersParameter">特定结构</param>
        /// <param name="paramInfos">参数</param>
        /// <returns></returns>
        internal static bool GetParameter(out List<Expression> expressions, ParameterExpression parametersParameter, params ParameterInfo[] paramInfos)
        {
            List<Expression> parameters = new();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                Type paramterType = paramInfos[i].ParameterType;
                if (paramterType.IsByRef) paramterType = paramterType.GetElementType();

                if (paramterType.ContainsGenericParameters)
                {
                    System.Diagnostics.Debug.WriteLine($"当前调用方法，不可用，因为没有指定形参。");
                    expressions = null;
                    return false;
                }

                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));

                Expression valueCast = paramterType == typeof(object) ? valueObj : Expression.Convert(valueObj, paramterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }
            expressions = parameters;
            return true;
        }

        /// <summary>
        /// 判断一个类型是是不是 <see cref="Task"/> or <see cref="Task{TResult}"/> or <see cref="ValueTask"/> or <see cref="ValueTask{TResult}"/>
        /// </summary>
        /// <param name="ReturnType">类型</param>
        /// <param name="isvoid">有无返回值</param>
        /// <returns></returns>
        public static bool MethodIsTask(Type ReturnType, out bool isvoid)
        {
            if (ReturnType == typeof(Task) || ReturnType == typeof(ValueTask))
            {
                isvoid = true;
                return true;
            }
            if (ReturnType.IsGenericType && (object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(Task<>)) || object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(ValueTask<>))))
            {
                isvoid = false;
                return true;
            }
            isvoid = false;
            return false;
        }

        /// <summary>
        /// 判断是否是 <see cref="ValueTask"/> or <see cref="ValueTask{TResult}"/>
        /// </summary>
        /// <param name="ReturnType"></param>
        /// <returns></returns>
        public static bool MethodIsValueTask(Type ReturnType) => ReturnType == typeof(ValueTask) || (ReturnType.IsGenericType && object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(ValueTask<>)));

        /// <summary>
        /// 判断是否存在继承关系 -> T=ReturnType
        /// </summary>
        /// <typeparam name="T">原始信息</typeparam>
        /// <param name="ReturnType">判断类型</param>
        /// <returns></returns>
        public static bool IsAssignableFrom<T>(Type ReturnType) => typeof(T).IsAssignableFrom(ReturnType);

        /// <summary>
        /// 是否是含返回值异步类型
        /// </summary>
        /// <param name="ReturnType"></param>
        /// <returns></returns>
        public static bool IsTaskTuple(Type ReturnType)
        {
            if (ReturnType.IsGenericType)
            {
                if (object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(Task<>)))
                {
                    return true;
                }
                if (object.Equals(ReturnType.GetGenericTypeDefinition(), typeof(ValueTask<>)))
                {
                    return true;
                }
            }
            //if (!object.Equals(ReturnType.BaseType, null) && (object.Equals(ReturnType.BaseType, typeof(Task)) || object.Equals(ReturnType.BaseType, typeof(ValueTask))))
            //{
            //    if (ReturnType.GenericTypeArguments.Length > 0)
            //    {
            //        return typeof(IApiOut).IsAssignableFrom(ReturnType.GenericTypeArguments[0]);
            //    }
            //}
            return false;
        }

        /// <summary>
        /// 判断是否是无返回结果的异步类型
        /// </summary>
        /// <param name="ReturnType"></param>
        /// <returns></returns>
        public static bool IsTask(Type ReturnType) => ReturnType == typeof(Task) || ReturnType == typeof(ValueTask);

        /// <summary>
        /// 判断有返回值的方法
        /// </summary>
        /// <typeparam name="T">返回值是否与他有关</typeparam>
        /// <param name="method">方法信息</param>
        /// <returns>是或否</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsTask<T>(MethodInfo method)
        {
            Type ReturnType = method.ReturnType;
            if (IsTaskTuple(ReturnType))
            {
                if (ReturnType.GenericTypeArguments.Length == 1)
                {
                    Type isiApiOut = ReturnType.GenericTypeArguments[0];
                    if (typeof(T).IsAssignableFrom(isiApiOut))//.Equals(isiApiOut))
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception($"返回值类型规定提示：警告：在使用异步返回值的情况下，强制采用（Task<{nameof(T)}> or ValueTask<{nameof(T)}>）其他均视为违规操作。\n异常方法名：{method.DeclaringType.Name}.Task<{isiApiOut.Name}> {method.Name}()");
                    }
                }
                throw new Exception("返回值类型规定提示：警告返回类型过多无效！");
            }
            return false;
        }

        private static MethodCallExpression IsAsTask<TResult>(MethodCallExpression methodCall)
        {
            if (typeof(Task<TResult>) == methodCall.Type) return methodCall;
            return Expression.Call(ToTask<TResult>(methodCall.Type), methodCall);
        }

        private static Expression IsAs<TResult>(MethodCallExpression methodCall) => IsAs(methodCall, typeof(TResult));

        private static Expression IsAs(Expression methodCall, Type resulttype)
        {
            if (resulttype == methodCall.Type) return methodCall;
            return Expression.Convert(methodCall, resulttype);
            //castMethodCall = Expression.TypeAs(methodCall, typeof(TResult));
            //.Call ((WebTestApp.Class1)$callclass).A((System.Int32)$parameters[0]) .As Tool.Sockets.NetFrame.IGoOut
            //(Tool.Sockets.NetFrame.IGoOut).Call ((WebTestApp.Class1)$callclass).A((System.Int32)$parameters[0])
        }

        /// <summary>
        /// 是否是无返回值的方法
        /// </summary>
        /// <param name="ReturnType"></param>
        /// <returns></returns>
        public static bool IsVoid(Type ReturnType) => ReturnType == typeof(void);

        /// <summary>
        /// 动态获取通用模型原型
        /// </summary>
        /// <typeparam name="T">类泛型</typeparam>
        /// <typeparam name="TResult">返回值泛型</typeparam>
        /// <returns>原始模型</returns>
        public static ActionDispatcher<T, TResult> AsAction<T, TResult>(this IActionDispatcher action)  //Unsafe.As<IActionDispatcher, ActionDispatcher<T, TResult>>(ref action);
        {
            if (action is IActionDispatcher<T> obj)
            {
                return obj.AsAction<T, TResult>();
            }
            throw new Exception("提供的泛型并非是原设定类型");
        }

        /// <summary>
        /// 动态获取通用模型原型
        /// </summary>
        /// <typeparam name="T">类泛型</typeparam>
        /// <typeparam name="TResult">返回值泛型</typeparam>
        /// <returns>原始模型</returns>
        public static ActionDispatcher<T, TResult> AsAction<T, TResult>(this IActionDispatcher<T> action)// => Unsafe.As<IActionDispatcher<T>, IActionDispatcher>(ref action).AsAction<T, TResult>();
        {
            if (action is ActionDispatcher<T, TResult> obj)
            {
                return obj;
            }
            throw new Exception("提供的泛型并非是原设定类型");
        }
    }

    /// <summary>
    /// 可实现的接口，方便用于高度自实现模块
    /// </summary>
    public interface IActionDispatcher
    {
        /// <summary>
        /// 是否有返回值，默认没有
        /// </summary>
        public bool IsVoid { get; }

        /// <summary>
        /// 是否是 异步函数？
        /// </summary>
        public bool IsTask { get; }

        /// <summary>
        /// 调用接口的返回值
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 当前方法的执行信息
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// 方法参数
        /// </summary>
        public Parameter[] Parameters { get; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 是否是静态方法
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// 方法的公开类型
        /// </summary>
        public MethodFlags MethodEnum { get; }
    }

    /// <summary>
    /// 可实现的接口，方便用于高度自实现模块
    /// </summary>
    /// <typeparam name="T">类泛型</typeparam>
    public interface IActionDispatcher<in T> : IActionDispatcher
    {
        /// <summary>
        /// 调用方法无返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        public void VoidExecute(T CallClass, params object[] parameters);

        /// <summary>
        /// 调用方法无返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        public Task VoidExecuteAsync(T CallClass, object[] parameters);
    }

    #region 委托公共模型

    /// <summary>
    /// 有返回值的委托
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="callclass"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate TResult ActionExecutor<in E, out TResult>(E callclass, object[] parameters);

    /// <summary>
    /// 无返回值的委托
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <param name="callclass"></param>
    /// <param name="parameters"></param>
    public delegate void VoidActionExecutor<in E>(E callclass, object[] parameters);

    /// <summary>
    /// 有返回值的异步委托
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="callclass"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate Task<TResult> TaskActionExecutor<in E, TResult>(E callclass, object[] parameters);

    /// <summary>
    /// 无返回值的异步委托
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <param name="callclass"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate Task VoidTaskActionExecutor<in E>(E callclass, object[] parameters);

    #endregion

    /// <summary>
    /// 根据 MethodInfo 对象，创建一个委托，实现方法调用，提高性能，支持各种返回值(object)
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ActionDispatcher : ActionDispatcher<object>
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        public ActionDispatcher(MethodInfo methodInfo) : base(methodInfo) { }
    }

    /// <summary>
    /// 根据 MethodInfo 对象，创建一个委托，实现方法调用，提高性能，支持各种返回值(object)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ActionDispatcher<T> : ActionDispatcher<T, object>
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        public ActionDispatcher(MethodInfo methodInfo) : base(methodInfo) { }

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="classtype">调用类类型（明确调用类）</param>
        public ActionDispatcher(MethodInfo methodInfo, Type classtype) : base(methodInfo, classtype) { }

        /// <summary>
        /// 可不区分是否有返回值的调用方法,返回泛型值
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public E Invoke<E>(T CallClass, params object[] parameters)
        {
            object obj = Invoke(CallClass, parameters);
            if (obj is null) return default;
            return (E)obj;
        }

        /// <summary>
        /// 可不区分是否有返回值的调用方法,返回泛型值
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public async Task<E> InvokeAsync<E>(T CallClass, params object[] parameters)
        {
            object obj = await InvokeAsync(CallClass, parameters);
            return (E)obj;
        }
    }

    /// <summary>
    /// 根据 MethodInfo 对象，创建一个委托，实现方法调用，提高性能，支持各种返回值(TResult)
    /// </summary>
    /// <typeparam name="T">调用类</typeparam>
    /// <typeparam name="TResult">返回值</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ActionDispatcher<T, TResult> : IActionDispatcher<T>
    {
        private readonly ActionExecutor<T, TResult> _executor;

        private readonly VoidActionExecutor<T> _executorVoid;

        private readonly TaskActionExecutor<T, TResult> _executorAsync;

        private readonly VoidTaskActionExecutor<T> _executorVoidAsync;

        /// <summary>
        /// 是否无返回值
        /// </summary>
        public bool IsVoid { get; }

        /// <summary>
        /// 是否是 异步函数？
        /// </summary>
        public bool IsTask { get; }

        /// <summary>
        /// 调用接口的返回值
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 当前方法的执行信息
        /// </summary>
        public MethodInfo Method { get; }

        private readonly Parameter[] _parameters;

        /// <summary>
        /// 方法参数
        /// </summary>
        public Parameter[] Parameters { get { return _parameters; } }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name { get { return this.Method.Name; } }

        /// <summary>
        /// 是否是静态方法
        /// </summary>
        public bool IsStatic { get { return this.Method.IsStatic; } }

        /// <summary>
        /// 方法的公开类型
        /// </summary>
        public MethodFlags MethodEnum
        {
            get
            {
                if (this.Method.IsPublic)
                {
                    return MethodFlags.Public;
                }

                if (this.Method.IsPrivate)
                {
                    return MethodFlags.Private;
                }

                if (this.Method.IsFamily)
                {
                    return MethodFlags.Protected;
                }

                if (this.Method.IsAssembly)
                {
                    return MethodFlags.Internal;
                }

                return MethodFlags.Public;
            }
        }

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        public ActionDispatcher(MethodInfo methodInfo) : this(methodInfo, typeof(T)) { }

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="classtype">调用类类型（明确调用类）</param>
        public ActionDispatcher(MethodInfo methodInfo, Type classtype)
        {
            if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));
            if (classtype is null) throw new ArgumentNullException(nameof(classtype));

            this.ReturnType = methodInfo.ReturnType;
            this.Method = methodInfo;

            _parameters = TypeInvoke.GetParameter(methodInfo);

            Delegate @delegate = DispatcherCore.GetExecutor<T, TResult>(methodInfo, classtype);
            if (@delegate is VoidTaskActionExecutor<T> @delegate0)
            {
                this.IsVoid = true;
                this.IsTask = true;
                _executorVoidAsync = @delegate0;
            }
            else if (@delegate is TaskActionExecutor<T, TResult> @delegate1)
            {
                this.IsVoid = false;
                this.IsTask = true;
                _executorAsync = @delegate1;
            }
            else if (@delegate is VoidActionExecutor<T> @delegate2)
            {
                this.IsVoid = true;
                this.IsTask = false;
                _executorVoid = @delegate2;
            }
            else if (@delegate is ActionExecutor<T, TResult> @delegate3)
            {
                this.IsVoid = false;
                this.IsTask = false;
                _executor = @delegate3;
            }
            else
            {
                throw new Exception("构建动态委托失败！");
            }
        }

        /// <summary>
        /// 可不区分是否有返回值的调用方法
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public TResult Invoke(T CallClass, params object[] parameters)
        {
            if (IsVoid)
            {
                VoidExecute(CallClass, parameters);
                return default;
            }
            else
            {
                return Execute(CallClass, parameters);
            }
        }

        /// <summary>
        /// 可不区分是否有返回值的调用方法
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public async Task<TResult> InvokeAsync(T CallClass, params object[] parameters)
        {
            if (IsVoid)
            {
                await VoidExecuteAsync(CallClass, parameters);
                return default;
            }
            else
            {
                return await ExecuteAsync(CallClass, parameters);
            }
        }

        /// <summary>
        /// 调用方法有返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public TResult Execute(T CallClass, params object[] parameters)
        {
            IsParameters(_executor);
            IsParameters(parameters);
            return _executor(CallClass, parameters);
        }

        /// <summary>
        /// 调用方法无返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        public void VoidExecute(T CallClass, params object[] parameters)
        {
            IsParameters(_executorVoid);
            IsParameters(parameters);
            _executorVoid(CallClass, parameters);
        }

        /// <summary>
        /// 调用方法有返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public Task<TResult> ExecuteAsync(T CallClass, object[] parameters)
        {
            IsParameters(_executorAsync);
            IsParameters(parameters);
            return _executorAsync(CallClass, parameters);
        }

        /// <summary>
        /// 调用方法无返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        public Task VoidExecuteAsync(T CallClass, object[] parameters)
        {
            IsParameters(_executorVoidAsync);
            IsParameters(parameters);
            return _executorVoidAsync(CallClass, parameters);
        }

        private void IsParameters(object[] parameters)
        {
            if (_parameters.Length > 0 && parameters.Length != _parameters.Length)
            {
                throw new Exception("所传参数，与实际方法参数不一致。");
            }
        }

        private static void IsParameters(Delegate @delegate)
        {
            if (@delegate is null) throw new Exception("无法调用此函数，请更换！");
        }

        //private static ActionExecutor<T> WrapVoidAction<T>(VoidActionExecutor<T> executor)
        //{
        //    return delegate (T controller, object[] parameters)
        //    {
        //        executor(controller, parameters);
        //        return null;
        //    };
        //}

        /// <summary>
        /// 返回方法信息缩写
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string par = string.Empty;
            if (_parameters.Length > 0)
            {
                StringBuilder str = new();
                foreach (var parameter in _parameters)
                {
                    str.Append(parameter.ParameterType.Name);
                    str.Append(' ');
                    str.Append(parameter.Name);
                    str.Append(',');
                }
                par = str.ToString(0, str.Length - 1);
            }

            return $"{(IsStatic ? "static " : "")}{(IsVoid ? "void" : nameof(TResult))} {Method.DeclaringType}.{Name}({par})";
        }
    }
}
