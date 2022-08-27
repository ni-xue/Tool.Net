﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 表示 获取 的范围
    /// </summary>
    public enum ClassField
    {
        /// <summary>
        /// 读写
        /// </summary>
        All,
        /// <summary>
        /// 读
        /// </summary>
        Get,
        /// <summary>
        /// 写
        /// </summary>
        Set,
    }

    /// <summary>
    /// 类中字段赋值，创建一个委托，实现类调用，提高性能 (赋值/取值)
    /// </summary>
    public class ClassFieldDispatcher
    {
        private readonly SetClassField _setClassField;
        private readonly GetClassField _getClassField;

        /// <summary>
        /// 实例化对象类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 类成员
        /// </summary>
        public PropertyInfo[] Parameters { get; }

        /// <summary>
        /// 覆盖功能
        /// </summary>
        public ClassField Field { get; }

        /// <summary>
        /// 定义类成员赋值函数
        /// </summary>
        /// <param name="callclass"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public delegate void SetClassField(object callclass, IDictionary<string, object> parameters);

        /// <summary>
        /// 定义类成员取值函数
        /// </summary>
        /// <param name="callclass"></param>
        /// <returns></returns>
        public delegate IDictionary<string, object> GetClassField(object callclass);

        /// <summary>
        /// 根据类，创建对象委托
        /// </summary>
        /// <param name="classtype">类对象类型</param>
        /// <param name="classField">默认读取所有行为</param>
        public ClassFieldDispatcher(Type classtype, ClassField classField = ClassField.All)
        {
            if (classtype == null) throw new ArgumentNullException(nameof(classtype), "参数为空！");
            this.Parameters = classtype.GetProperties();
            this.Type = classtype;
            this.Field = classField;
            if (classField == ClassField.All || classField == ClassField.Set)
            {
                _setClassField = SetClassFields(classtype);
            }
            if (classField == ClassField.All || classField == ClassField.Get) 
            {
                _getClassField = GetClassFields(classtype);
            }
        }

        /// <summary>
        /// 向类传入修改集合
        /// </summary>
        /// <param name="_class">类对象</param>
        /// <param name="parameters">值集合</param>
        public void Set(object _class, IDictionary<string, object> parameters)
        {
            _setClassField?.Invoke(_class, parameters);
        }

        /// <summary>
        /// 从类中获取所有可读值
        /// </summary>
        /// <param name="_class">类对象</param>
        /// <returns>返回值集合</returns>
        public IDictionary<string, object> Get(object _class)
        {
            return _getClassField?.Invoke(_class);
        }

        ///// <summary>
        ///// 通过构造器，直接获取相关赋值委托
        ///// </summary>
        ///// <param name="classtype">类</param>
        ///// <returns>赋值委托或无法委托因为都是只读</returns>
        //public static SetClassField SetClassFields(Type classtype)
        //{
        //    if (classtype == null) throw new ArgumentNullException(nameof(classtype), "参数为空！");

        //    var propertyInfos = classtype.GetProperties();
        //    var hashtype = typeof(IDictionary<string, object>);

        //    var as0 = Expression.Parameter(typeof(object), "callclass");
        //    var as1 = Expression.Parameter(hashtype, "parameters");

        //    var as3 = Expression.Parameter(classtype, "callobj");

        //    MethodInfo getContainsKey = hashtype.GetMethod("ContainsKey");//.MakeGenericMethod(typeof(string));//创建泛型方法
        //    MethodInfo getItem = hashtype.GetMethod("get_Item");
        //    List<Expression> blockExpressions = new()
        //    {
        //        Expression.Assign(as3, Expression.Convert(as0, classtype))
        //    };
        //    foreach (var classField in propertyInfos)
        //    {
        //        if (!classField.CanWrite) continue;
        //        //var asassign = Expression.Assign(as3, Expression.Convert(as0, classtype));

        //        var source = Expression.Property(as3, classField);
        //        var target = Expression.Call(as1, getItem, Expression.Constant(classField.Name));//Expression.ArrayAccess(as1, Expression.Constant(classField.Name)); // Expression.Property(as1, classField);

        //        //var target = Expression.MakeIndex(as1, null, new[] { Expression.Constant(classField.Name) });

        //        var method = Expression.Call(as1, getContainsKey, Expression.Constant(classField.Name));

        //        var assign = Expression.Assign(source, Expression.Convert(target, classField.PropertyType));

        //        var ifassign = Expression.IfThen(method, assign);
        //        blockExpressions.Add(ifassign);
        //    }
        //    if (blockExpressions.Count == 1) return default;

        //    var as2 = Expression.Block(new[] { as3 }, blockExpressions);//new[] { as0, as1 }, 
        //    //System.Linq.Expressions.Expression.New(constructor, expressions);

        //    var as4 = Expression.Lambda<SetClassField>(as2, as0, as1);

        //    return as4.Compile();
        //}

        /// <summary>
        /// 通过构造器，直接获取相关赋值委托
        /// </summary>
        /// <param name="classtype">类</param>
        /// <returns>赋值委托或无法委托因为都是只读</returns>
        public static SetClassField SetClassFields(Type classtype)
        {
            if (classtype == null) throw new ArgumentNullException(nameof(classtype), "参数为空！");

            var propertyInfos = classtype.GetProperties();
            var hashtype = typeof(IDictionary<string, object>);

            var as0 = Expression.Parameter(typeof(object), "callclass");
            var as1 = Expression.Parameter(hashtype, "parameters");

            var as3 = Expression.Parameter(classtype, "callobj");
            var as4 = Expression.Parameter(typeof(object), "_obj");

            MethodInfo getContainsKey = hashtype.GetMethod("TryGetValue");
            List<Expression> blockExpressions = new()
            {
                Expression.Assign(as3, Expression.TypeAs(as0, classtype))//Convert
            };
            foreach (var classField in propertyInfos)
            {
                if (!classField.CanWrite) continue;

                var source = Expression.Property(as3, classField);

                var method = Expression.Call(as1, getContainsKey, Expression.Constant(classField.Name), as4);

                var assign = Expression.Assign(source, Expression.Convert(as4, classField.PropertyType));

                var ifassign = Expression.IfThen(method, assign);
                blockExpressions.Add(ifassign);
            }
            if (blockExpressions.Count == 1) return default;

            var as2 = Expression.Block(new[] { as3, as4 }, blockExpressions);

            var as5 = Expression.Lambda<SetClassField>(as2, as0, as1);

            return as5.Compile();
        }

        /// <summary>
        /// 通过构造器，直接获取相关取值委托
        /// </summary>
        /// <param name="classtype">类</param>
        /// <returns>取值委托或无法委托因为不可读</returns>
        public static GetClassField GetClassFields(Type classtype)
        {
            if (classtype == null) throw new ArgumentNullException(nameof(classtype), "参数为空！");

            var propertyInfos = classtype.GetProperties();
            var hashtype = typeof(IDictionary<string, object>);

            var as0 = Expression.Parameter(typeof(object), "callclass");

            var as1 = Expression.Parameter(hashtype, "value");
            var as2 = Expression.Parameter(classtype, "callobj");

            ConstructorInfo newHashtable = typeof(Dictionary<string, object>).GetConstructor(new[] { typeof(int) });
            MethodInfo addItem = hashtype.GetMethod("Add");
            List<Expression> blockExpressions = new()
            {
                Expression.Assign(as2, Expression.TypeAs(as0, classtype)),
                Expression.Assign(as1, Expression.New(newHashtable, Expression.Constant(propertyInfos.Length))),
            };
            foreach (var classField in propertyInfos)
            {
                if (!classField.CanRead) continue;

                var target = Expression.Call(as1, addItem, Expression.Constant(classField.Name), Expression.TypeAs(Expression.Property(as2, classField), typeof(object)));//Expression.Call(as2, classField.GetMethod)//Convert

                blockExpressions.Add(target);
            }
            if (blockExpressions.Count == 2) return default;

            blockExpressions.Add(Expression.Label(Expression.Label(hashtype), as1));

            var as3 = Expression.Block(new[] { as2, as1 }, blockExpressions);

            var as4 = Expression.Lambda<GetClassField>(as3, as0);

            return as4.Compile();
        }
    }
}