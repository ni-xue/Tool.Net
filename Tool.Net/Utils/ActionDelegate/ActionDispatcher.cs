using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 根据 MethodInfo 对象，创建一个委托，实现方法调用，提高性能，支持各种返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ActionDispatcher<T>
    {
        private readonly ActionExecutor<T> _executor;

        private readonly VoidActionExecutor<T> _executorVoid;

        /// <summary>
        /// 是否有返回值，默认没有
        /// </summary>
        public readonly bool IsVoid;

        /// <summary>
        /// 调用接口的返回值
        /// </summary>
        public readonly Type ReturnType;

        /// <summary>
        /// 当前方法的执行信息
        /// </summary>
        public readonly MethodInfo Method;

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
        public ActionDispatcher(MethodInfo methodInfo)
        {
            this.IsVoid = methodInfo.ReturnType == typeof(void);

            this.ReturnType = methodInfo.ReturnType;

            this.Method = methodInfo;

            _parameters = TypeInvoke.GetParameter(methodInfo);

            if (this.IsVoid)
            {
                _executorVoid = GetExecutor<T>(methodInfo) as VoidActionExecutor<T>;
            }
            else
            {
                _executor = GetExecutor<T>(methodInfo) as ActionExecutor<T>;
            }
            //_executor = GetExecutor<T>(methodInfo);
            //MethodInfo = methodInfo;
        }

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="type">类类型</param>
        public ActionDispatcher(MethodInfo methodInfo, Type type)
        {
            this.IsVoid = methodInfo.ReturnType == typeof(void);

            this.ReturnType = methodInfo.ReturnType;

            this.Method = methodInfo;

            _parameters = TypeInvoke.GetParameter(methodInfo);

            if (this.IsVoid)
            {
                _executorVoid = GetExecutor(methodInfo, type) as VoidActionExecutor<T>;
            }
            else
            {
                _executor = GetExecutor(methodInfo, type) as ActionExecutor<T>;
            }
            //_executor = GetExecutor<T>(methodInfo);
            //MethodInfo = methodInfo;
        }

        /// <summary>
        /// 有返回值的委托
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="callclass"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public delegate object ActionExecutor<in E>(E callclass, object[] parameters);

        /// <summary>
        /// 无返回值的委托
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="callclass"></param>
        /// <param name="parameters"></param>
        public delegate void VoidActionExecutor<in E>(E callclass, object[] parameters);

        //// <summary>
        //// 
        //// </summary>
        //public MethodInfo MethodInfo { get; private set; }

        ///// <summary>
        ///// 调用方法有返回值(注明：如调用的是静态方法，类参数可为 null)
        ///// </summary>
        ///// <param name="CallClass">调用方法的类</param>
        ///// <param name="parameters">参数</param>
        ///// <returns>返回方法的返回值</returns>
        //public object Execute(T CallClass, object[] parameters)
        //{
        //    return _executor(CallClass, parameters);
        //}

        ///// <summary>
        ///// 调用方法无返回值(注明：如调用的是静态方法，类参数可为 null)
        ///// </summary>
        ///// <param name="CallClass">调用方法的类</param>
        ///// <param name="parameters">参数</param>
        //public void VoidExecute(T CallClass, object[] parameters)
        //{
        //    _executorVoid(CallClass, parameters);
        //}

        /// <summary>
        /// 可不区分是否有返回值的调用方法
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public object Invoke(T CallClass, params object[] parameters)
        {
            if (IsVoid)
            {
                VoidExecute(CallClass, parameters);
                return null;
            }
            else
            {
                return Execute(CallClass, parameters);
            }
        }

        /// <summary>
        /// 可不区分是否有返回值的调用方法,返回泛型值
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public E Invoke<E>(T CallClass, params object[] parameters)
        {
            if (IsVoid)
            {
                VoidExecute(CallClass, parameters);
                return default(E);
            }
            else
            {
                return (E)Execute(CallClass, parameters);
            }
        }

        /// <summary>
        /// 调用方法有返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回方法的返回值</returns>
        public object Execute(T CallClass, params object[] parameters)
        {
            if (_parameters.Length > 0)
            {
                if (parameters.Length != _parameters.Length)
                {
                    throw new Exception("所传参数，与实际方法参数不一致。");
                }
            }
            return _executor(CallClass, parameters);
        }

        /// <summary>
        /// 调用方法无返回值(注明：如调用的是静态方法，类参数可为 null)
        /// </summary>
        /// <param name="CallClass">调用方法的类</param>
        /// <param name="parameters">参数</param>
        public void VoidExecute(T CallClass, params object[] parameters)
        {
            if (_parameters.Length > 0)
            {
                if (parameters.Length != _parameters.Length)
                {
                    throw new Exception("所传参数，与实际方法参数不一致。");
                }
            }
            _executorVoid(CallClass, parameters);
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <param name="methodInfo">方法对象</param>
        /// <param name="type">调用的返回类</param>
        /// <returns></returns>
        public static object GetExecutor(MethodInfo methodInfo, Type type)
        {
            // Parameters to executor
            ParameterExpression controllerParameter = Expression.Parameter(type, "callclass");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            if (!GetParameter(out List<Expression> parameters, parametersParameter, methodInfo.GetParameters()))
            {
                System.Diagnostics.Debug.WriteLine($"方法：{methodInfo},无法创建委托进行调用。");
                return null;
            }

            //List<Expression> parameters = new List<Expression>();
            //ParameterInfo[] paramInfos = methodInfo.GetParameters();
            //for (int i = 0; i < paramInfos.Length; i++)
            //{
            //    ParameterInfo paramInfo = paramInfos[i];

            //    BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
            //    UnaryExpression valueCast;
            //    if (paramInfo.ParameterType.IsByRef)
            //    {
            //        try
            //        {
            //            string fullName = $"{ paramInfo.ParameterType.FullName.Substring(0, paramInfo.ParameterType.FullName.Length - 1) }, {paramInfo.ParameterType.Assembly}";
            //            Type type = Type.GetType(fullName);
            //            valueCast = Expression.Convert(valueObj, type);
            //        }
            //        catch (Exception)
            //        {
            //            System.Diagnostics.Debug.WriteLine($"方法：{methodInfo.ToString()},无法创建委托进行调用。");
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);
            //    }

            //    // valueCast is "(Ti) parameters[i]"
            //    parameters.Add(valueCast);
            //}

            // Call method
            UnaryExpression instanceCast = (!methodInfo.IsStatic) ? Expression.Convert(controllerParameter, methodInfo.ReflectedType) : null;
            MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameters);//= methodCall 

            // methodCall is "((TController) controller) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                Expression<VoidActionExecutor<T>> lambda = Expression.Lambda<VoidActionExecutor<T>>(methodCall, controllerParameter, parametersParameter);
                VoidActionExecutor<T> voidExecutor = lambda.Compile();
                //_executorVoid = voidExecutor;
                return voidExecutor; //WrapVoidAction(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutor signature
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<ActionExecutor<T>> lambda = Expression.Lambda<ActionExecutor<T>>(castMethodCall, controllerParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <typeparam name="E">调用的返回类</typeparam>
        /// <param name="methodInfo">方法对象</param>
        /// <returns></returns>
        public static object GetExecutor<E>(MethodInfo methodInfo)
        {
            // Parameters to executor
            ParameterExpression controllerParameter = Expression.Parameter(typeof(E), "callclass");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            if (!GetParameter(out List<Expression> parameters, parametersParameter, methodInfo.GetParameters()))
            {
                System.Diagnostics.Debug.WriteLine($"方法：{methodInfo},无法创建委托进行调用。");
                return null;
            }

            //List<Expression> parameters = new List<Expression>();
            //ParameterInfo[] paramInfos = methodInfo.GetParameters();
            //for (int i = 0; i < paramInfos.Length; i++)
            //{
            //    ParameterInfo paramInfo = paramInfos[i];

            //    BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
            //    UnaryExpression valueCast;
            //    if (paramInfo.ParameterType.IsByRef)
            //    {
            //        try
            //        {
            //            string fullName = $"{ paramInfo.ParameterType.FullName.Substring(0, paramInfo.ParameterType.FullName.Length - 1) }, {paramInfo.ParameterType.Assembly}";
            //            Type type = Type.GetType(fullName);
            //            valueCast = Expression.Convert(valueObj, type);
            //        }
            //        catch (Exception)
            //        {
            //            System.Diagnostics.Debug.WriteLine($"方法：{methodInfo.ToString()},无法创建委托进行调用。");
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);
            //    }

            //    // valueCast is "(Ti) parameters[i]"
            //    parameters.Add(valueCast);
            //}

            // Call method
            UnaryExpression instanceCast = (!methodInfo.IsStatic) ? Expression.Convert(controllerParameter, methodInfo.ReflectedType) : null;
            MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameters);// = methodCall

            // methodCall is "((TController) controller) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                Expression<VoidActionExecutor<E>> lambda = Expression.Lambda<VoidActionExecutor<E>>(methodCall, controllerParameter, parametersParameter);
                VoidActionExecutor<E> voidExecutor = lambda.Compile();
                //_executorVoid = voidExecutor;
                return voidExecutor; //WrapVoidAction(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutor signature
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<ActionExecutor<E>> lambda = Expression.Lambda<ActionExecutor<E>>(castMethodCall, controllerParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// 创建不同的委托
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns>返回委托类型</returns>
        public static object GetExecutor(MethodInfo methodInfo)
        {
            // Parameters to executor
            ParameterExpression controllerParameter = Expression.Parameter(methodInfo.DeclaringType, "callclass");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            if (!GetParameter(out List<Expression> parameters, parametersParameter, methodInfo.GetParameters()))
            {
                System.Diagnostics.Debug.WriteLine($"方法：{methodInfo},无法创建委托进行调用。");
                return null;
            }

            //List<Expression> parameters = new List<Expression>();
            //ParameterInfo[] paramInfos = methodInfo.GetParameters();
            //for (int i = 0; i < paramInfos.Length; i++)
            //{
            //    ParameterInfo paramInfo = paramInfos[i];

            //    BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
            //    UnaryExpression valueCast;
            //    if (paramInfo.ParameterType.IsByRef)
            //    {
            //        try
            //        {
            //            string fullName = $"{ paramInfo.ParameterType.FullName.Substring(0, paramInfo.ParameterType.FullName.Length - 1) }, {paramInfo.ParameterType.Assembly}";
            //            Type type = Type.GetType(fullName);
            //            valueCast = Expression.Convert(valueObj, type);
            //        }
            //        catch (Exception)
            //        {
            //            System.Diagnostics.Debug.WriteLine($"方法：{methodInfo.ToString()},无法创建委托进行调用。");
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);
            //    }

            //    // valueCast is "(Ti) parameters[i]"
            //    parameters.Add(valueCast);
            //}

            // Call method
            UnaryExpression instanceCast = (!methodInfo.IsStatic) ? Expression.Convert(controllerParameter, methodInfo.ReflectedType) : null;
            MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameters);// = methodCall

            // methodCall is "((TController) controller) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                Expression<VoidActionExecutor<T>> lambda = Expression.Lambda<VoidActionExecutor<T>>(methodCall, controllerParameter, parametersParameter);
                VoidActionExecutor<T> voidExecutor = lambda.Compile();
                //_executorVoid = voidExecutor;
                return voidExecutor; //WrapVoidAction(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutor signature
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<ActionExecutor<T>> lambda = Expression.Lambda<ActionExecutor<T>>(castMethodCall, controllerParameter, parametersParameter);
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
            //ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                ParameterInfo paramInfo = paramInfos[i];

                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast;
                if (paramInfo.ParameterType.IsByRef)
                {
                    try
                    {
                        string fullName = $"{ paramInfo.ParameterType.FullName[0..^1] }, {paramInfo.ParameterType.Assembly}";
                        Type type = Type.GetType(fullName);
                        valueCast = Expression.Convert(valueObj, type);
                    }
                    catch //(Exception)
                    {
                        expressions = null;
                        return false;
                    }
                }
                else
                {
                    valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);
                }

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            expressions = parameters;
            return true;
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
                foreach (var parameter in _parameters)
                {
                    par += $"{parameter.ParameterType.Name} {parameter.Name},";
                }
                par = par[0..^1];
            }

            return $"{(IsStatic ? "static " : "")}{(IsVoid ? "void" : "object")} {Method.DeclaringType}.{Name}({par})";
        }
    }
}
