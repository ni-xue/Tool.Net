using System.Collections.Concurrent;
using System;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 引用对象属性构造器
    /// </summary>
    public class TypePropertyDescriptor
    {
        internal readonly ConcurrentDictionary<string, PropertyDescriptor> Propertys;

        /// <summary>
        /// 数据源Type
        /// </summary>
        public Type ClassType { get; }

        /// <summary>
        /// 初始化属性构造器
        /// </summary>
        /// <param name="type"></param>
        public TypePropertyDescriptor(Type type)
        {
            ClassType = type;
            Propertys = new();
        }

        private FieldInfo GetFieldInfo(string name)
        {
            var field = ClassType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            return field;
        }

        private PropertyInfo GetPropertyInfo(string name)
        {
            var property = ClassType.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            return property;
        }

        /// <summary>
        /// 获取指定<see cref="PropertyDescriptor"/>变量
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="property">对象财产托管调度器</param>
        /// <returns>返回变量值</returns>
        public bool TryProperty(string name, out PropertyDescriptor property)
        {
            var typeProperty = Propertys.GetOrAdd($"Property+{name}", (key) =>
            {
                var property = GetPropertyInfo(name);
                if (property == null) return default;
                return new PropertyDescriptor(ClassType, property);
            });
            property = typeProperty;
            return property is not null;
        }

        /// <summary>
        /// 获取指定<see cref="PropertyDescriptor"/>变量
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="property">对象财产托管调度器</param>
        /// <returns>返回变量值</returns>
        public bool TryField(string name, out PropertyDescriptor property)
        {
            var typefield = Propertys.GetOrAdd($"Field+{name}", (key) =>
            {
                var field = GetFieldInfo(name);
                if (field == null) return default;
                return new PropertyDescriptor(ClassType, field);
            });
            property = typefield;
            return property is not null;
        }

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以修改</param>
        /// <returns>返回变量值</returns>
        public object GetProperty(object obj, string name, out bool isexist)
        {
            if (TryProperty(name, out var typeProperty))
            {
                if (typeProperty.CanRead)
                {
                    var _obj = typeProperty.GetValue(obj);
                    isexist = true;
                    return _obj;
                }
            }
            isexist = false;
            return default;
        }

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public bool SetProperty(object obj, string name, object value)
        {
            if (TryProperty(name, out var typeProperty))
            {
                if (typeProperty.CanWrite)
                {
                    typeProperty.SetValue(obj, value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="isexist">是否可以获取</param>
        /// <returns>返回变量值</returns>
        public object GetField(object obj, string name, out bool isexist)
        {
            if (TryField(name, out var typeProperty))
            {
                if (typeProperty.CanRead)
                {
                    var _obj = typeProperty.GetValue(obj);
                    isexist = true;
                    return _obj;
                }
            }
            isexist = false;
            return default;
        }

        /// <summary>
        /// 修改指定变量值 (支持 public/private/protected) 静态时obj为null
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">变量名称</param>
        /// <param name="value">修改的值</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public bool SetField(object obj, string name, object value)
        {
            if (TryField(name, out var typeProperty))
            {
                if (typeProperty.CanWrite)
                {
                    typeProperty.SetValue(obj, value);
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 对象财产托管调度器
    /// </summary>
    public class PropertyDescriptor
    {
        /// <summary>
        /// 定义类成员赋值函数
        /// </summary>
        /// <param name="callclass"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public delegate void SetClassProperty(object callclass, object parameters);

        /// <summary>
        /// 定义类成员取值函数
        /// </summary>
        /// <param name="callclass"></param>
        /// <returns></returns>
        public delegate object GetClassProperty(object callclass);

        /// <summary>
        /// 可能存在的财产<see cref="PropertyInfo"/>
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// 可能存在的财产<see cref="FieldInfo"/>
        /// </summary>
        public FieldInfo Field { get; }

        /// <summary>
        /// 是否可写
        /// </summary>
        public bool CanWrite { get; } = false;

        /// <summary>
        /// 是否可读
        /// </summary>
        public bool CanRead { get; } = false;

        private readonly GetClassProperty GetProperty;
        private readonly SetClassProperty SetProperty;

        /// <summary>
        /// 是否是静态变量
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// 尝试获取可用的财产
        /// </summary>
        /// <param name="classtype">对象信息</param>
        /// <param name="property">财产</param>
        public PropertyDescriptor(Type classtype, PropertyInfo property)
        {
            Property = property;
            if (Property is not null)
            {
                if (Property.SetMethod is not null)
                {
                    CanWrite = true;
                    IsStatic = Property.SetMethod.IsStatic;
                    SetProperty = CreateSetProperty(classtype, Property.SetMethod, Property.PropertyType, IsStatic);
                }
                if (Property.GetMethod is not null)
                {
                    CanRead = true;
                    IsStatic = Property.GetMethod.IsStatic;
                    GetProperty = CreateGetProperty(classtype, Property.GetMethod, IsStatic);
                }
            }
        }

        /// <summary>
        /// 尝试获取可用的财产
        /// </summary>
        /// <param name="classtype">对象信息</param>
        /// <param name="fieldInfo">财产</param>
        public PropertyDescriptor(Type classtype, FieldInfo fieldInfo)
        {
            Field = fieldInfo;
            if (Field is not null)
            {
                CanWrite = true;
                CanRead = true;
                IsStatic = Field.IsStatic;

                SetProperty = CreateSetProperty(classtype, Field, Field.FieldType, IsStatic);

                GetProperty = CreateGetProperty(classtype, Field, IsStatic);
            }
        }

        private static SetClassProperty CreateSetProperty(Type classtype, object proto, Type valtype, bool isstatic)
        {
            var as0 = Expression.Parameter(typeof(object), "callclass");
            var as1 = Expression.Parameter(classtype, "callobj");
            List<Expression> blockExpressions = new();

            var source = GetExpression(blockExpressions, classtype, as0, as1, proto, isstatic);
            var as2 = Expression.Parameter(typeof(object), "parameters");
            var assign = Expression.Assign(source, Expression.Convert(as2, valtype));
            blockExpressions.Add(assign);

            var as3 = Expression.Block(new[] { as1 }, blockExpressions);
            var as4 = Expression.Lambda<SetClassProperty>(as3, as0, as2);
            return as4.Compile();
        }

        private static GetClassProperty CreateGetProperty(Type classtype, object proto, bool isstatic)
        {
            var as0 = Expression.Parameter(typeof(object), "callclass");
            var as1 = Expression.Parameter(classtype, "callobj");
            List<Expression> blockExpressions = new();

            var source = GetExpression(blockExpressions, classtype, as0, as1, proto, isstatic);

            var label = Expression.Label(Expression.Label(typeof(object)), Expression.TypeAs(source, typeof(object)));
            blockExpressions.Add(label);

            var as3 = Expression.Block(new[] { as1 }, blockExpressions);
            var as4 = Expression.Lambda<GetClassProperty>(as3, as0);
            return as4.Compile();
        }

        private static MemberExpression GetExpression(List<Expression> blockExpressions, Type classtype, ParameterExpression as0, ParameterExpression as1, object proto, bool isstatic)
        {
            MemberExpression source = null;
            if (isstatic)
            {
                if (proto is FieldInfo field)
                {
                    source = Expression.Field(null, field);
                }
                else if (proto is MethodInfo method)
                {
                    source = Expression.Property(null, method);
                }
            }
            else
            {
                blockExpressions.Add(Expression.Assign(as1, Expression.TypeAs(as0, classtype)));
                if (proto is FieldInfo field)
                {
                    source = Expression.Field(as1, field);
                }
                else if (proto is MethodInfo method)
                {
                    source = Expression.Property(as1, method);
                }
            }
            if (source is null) throw new InvalidOperationException("调用无效！");
            return source;
        }

        /// <summary>
        /// 获取指定变量值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <returns>返回变量值</returns>
        public object GetValue(object obj)
        {
            if (!CanRead) throw new InvalidOperationException("禁止读取！");
            if (GetProperty is null) throw new InvalidOperationException("无有效虚构函数，调用无效！");
            return GetProperty(obj);
        }

        /// <summary>
        /// 修改指定变量值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="value">修改的值</param>
        public void SetValue(object obj, object value)
        {
            if (!CanWrite) throw new InvalidOperationException("禁止写入！");
            if (SetProperty is null) throw new InvalidOperationException("无有效虚构函数，调用无效！");
            SetProperty(obj, value);
        }

    }
}
