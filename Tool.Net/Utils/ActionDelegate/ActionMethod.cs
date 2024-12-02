using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 调用方法信息
    /// </summary>
    /// <typeparam name="T">类对象</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ActionMethod<T>
    {
        private readonly Type ClassType;

        private readonly ActionDispatcher<T> _action;

        /// <summary>
        /// 方法委托
        /// </summary>
        public ActionDispatcher<T> Action { get { return _action; } }

        /// <summary>
        /// 方法参数
        /// </summary>
        public Parameter[] Parameters { get { return Action.Parameters; } }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name { get { return Action.Name; } }

        /// <summary>
        /// 是否是静态方法
        /// </summary>
        public bool IsStatic { get { return Action.IsStatic; } }

        /// <summary>
        /// 是不是继承类方法
        /// </summary>
        public bool IsBase { get { return ClassType != Action.Method.DeclaringType; } }

        /// <summary>
        /// 是否是 异步函数？
        /// </summary>
        public bool IsTask { get { return Action.IsTask; } }

        /// <summary>
        /// 是否无返回值
        /// </summary>
        public bool IsVoid { get { return Action.IsVoid; } }

        /// <summary>
        /// 方法的公开类型
        /// </summary>
        public MethodFlags MethodEnum { get { return Action.MethodEnum; } }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="methodInfo">方法</param>
        public ActionMethod(MethodInfo methodInfo)
        {
            ClassType = typeof(T);

            _action = new ActionDispatcher<T>(methodInfo);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="methodInfo">方法</param>
        /// <param name="ClassType">类的<see cref="Type"/></param>
        public ActionMethod(MethodInfo methodInfo, Type ClassType)
        {
            this.ClassType = ClassType;

            _action = new ActionDispatcher<T>(methodInfo);
        }

        /// <summary>
        /// 返回方法信息缩写
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _action.ToString();
        }
    }
}
