using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 根据 指定类，获取该类下面指定的 <see cref="ActionDispatcher{T}"/> 集合，
    /// <see cref="ActionDispatcher{T}"/>类的扩展帮助类
    /// </summary>
    /// <typeparam name="T">类对象</typeparam>
    public sealed class ActionHelper<T> : IEnumerable
    {
        private readonly List<ActionMethod<T>> _actionDispatchers;

        /// <summary>
        /// 当前类查询的方法条件
        /// </summary>
        public MethodFlags MethodFlag { get; }

        /// <summary>
        /// 当前类下面的所有方法
        /// </summary>
        public List<ActionMethod<T>> ActionMethods { get { return _actionDispatchers; } }

        /// <summary>
        /// 可访问的方法数量
        /// </summary>
        public int Count { get { return _actionDispatchers.Count; } }

        /// <summary>
        /// 根据下标获取指定方法
        /// </summary>
        /// <param name="i">下标</param>
        /// <returns></returns>
        public ActionMethod<T> this[int i]
        {
            get
            {
                return _actionDispatchers[i];
            }
        }

        /// <summary>
        /// 获取指定类的方法（默认查找静态的和公开的）
        /// </summary>
        public ActionHelper(): this(MethodFlags.Static | MethodFlags.Public)
        {
        }

        /// <summary>
        /// 获取指定类的方法
        /// </summary>
        /// <param name="methodFlags">根据查找类型，获得相应方法</param>
        public ActionHelper(MethodFlags methodFlags)
        {
            Type type = typeof(T);

            this.MethodFlag = methodFlags;

            MethodInfo[] methodInfos = GetMethodInfos(type, methodFlags);

            this._actionDispatchers = GetMethodInfos(type, methodInfos, methodFlags);
        }

        /// <summary>
        /// 获取指定类的方法
        /// </summary>
        /// <param name="type">指定获取类的<see cref="Type"/></param>
        /// <param name="methodFlags">根据查找类型，获得相应方法</param>
        public ActionHelper(Type type, MethodFlags methodFlags)
        {
            this.MethodFlag = methodFlags;

            MethodInfo[] methodInfos = GetMethodInfos(type, methodFlags);

            this._actionDispatchers = GetMethodInfos(type, methodInfos, methodFlags);
        }

        /// <summary>
        /// 获取指定类的方法 等同于NEW
        /// </summary>
        /// <param name="methodFlags">根据查找类型，获得相应方法</param>
        /// <returns>返回要获取的类下面的方法</returns>
        public static ActionHelper<T> GetActionMethodHelper(MethodFlags methodFlags)
        {
            return new ActionHelper<T>(methodFlags);
        }

        /// <summary>
        /// 获取指定类的方法 等同于NEW
        /// </summary>
        /// <param name="type">指定获取类的<see cref="Type"/></param>
        /// <param name="methodFlags">根据查找类型，获得相应方法</param>
        /// <returns>返回要获取的类下面的方法</returns>
        public static ActionHelper<T> GetActionMethodHelper(Type type, MethodFlags methodFlags)
        {
            return new ActionHelper<T>(type, methodFlags);
        }

        /// <summary>
        /// 根据方法名获取方法
        /// </summary>
        /// <param name="name">方法名称</param>
        /// <returns>返回匹配到的方法</returns>
        public List<ActionMethod<T>> GetName(string name)
        {
            return new List<ActionMethod<T>>(_actionDispatchers.Where(s => s.Name == name));
        }

        private static List<ActionMethod<T>> GetMethodInfos(Type type, MethodInfo[] methodInfos, MethodFlags methodFlags)
        {
            List<ActionMethod<T>> actionMethods = new();

            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (!methodInfo.IsSpecialName)
                {
                    if (methodFlags.Equals(MethodFlags.Default))
                    {
                        actionMethods.Add(new ActionMethod<T>(methodInfo, type));
                    }
                    else
                    {
                        if (methodInfo.IsPublic)
                        {
                            if ((methodFlags & MethodFlags.Public) == MethodFlags.Public)
                            {
                                actionMethods.Add(new ActionMethod<T>(methodInfo, type));
                            }
                        }
                        else if (methodInfo.IsPrivate)
                        {
                            if ((methodFlags & MethodFlags.Private) == MethodFlags.Private)
                            {
                                actionMethods.Add(new ActionMethod<T>(methodInfo, type));
                            }
                        }
                        else if (methodInfo.IsFamily)
                        {
                            if ((methodFlags & MethodFlags.Protected) == MethodFlags.Protected)
                            {
                                actionMethods.Add(new ActionMethod<T>(methodInfo, type));
                            }
                        }
                        else if (methodInfo.IsAssembly)
                        {
                            if ((methodFlags & MethodFlags.Internal) == MethodFlags.Internal)
                            {
                                actionMethods.Add(new ActionMethod<T>(methodInfo, type));
                            }
                        }
                    }
                }
            }
            return actionMethods;
        }

        private static MethodInfo[] GetMethodInfos(Type type, MethodFlags methodFlags)
        {
            BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Instance;

            if (methodFlags.Equals(MethodFlags.Default))
            {
                bindingFlags |= BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                return type.GetMethods(bindingFlags);
            }

            if ((methodFlags & MethodFlags.Base) == MethodFlags.Default)
            {
                bindingFlags |= BindingFlags.DeclaredOnly;
            }

            if ((methodFlags & MethodFlags.Static) == MethodFlags.Static)
            {
                bindingFlags |= BindingFlags.Static;
            }

            if ((methodFlags & MethodFlags.Public) == MethodFlags.Public)
            {
                bindingFlags |= BindingFlags.Public;
            }

            if ((methodFlags & MethodFlags.Private) == MethodFlags.Private)
            {
                if ((bindingFlags & BindingFlags.NonPublic).ToVar<int>() == 0)
                {
                    bindingFlags |= BindingFlags.NonPublic;
                }
            }

            if ((methodFlags & MethodFlags.Protected) == MethodFlags.Protected)
            {
                if ((bindingFlags & BindingFlags.NonPublic).ToVar<int>() == 0)
                {
                    bindingFlags |= BindingFlags.NonPublic;
                }
            }

            if ((methodFlags & MethodFlags.Internal) == MethodFlags.Internal)
            {
                if ((bindingFlags & BindingFlags.NonPublic).ToVar<int>() == 0)
                {
                    bindingFlags |= BindingFlags.NonPublic;
                }
            }

            return type.GetMethods(bindingFlags);
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_actionDispatchers).GetEnumerator();
        }

        /// <summary>
        /// 返回方法信息缩写
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{typeof(T)}.Action[{Count}]";
        }
    }
}
