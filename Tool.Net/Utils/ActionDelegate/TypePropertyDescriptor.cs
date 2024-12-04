using System.Collections.Concurrent;
using System;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 引用对象属性构造器
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TypePropertyDescriptor
    {
        internal readonly ConcurrentDictionary<string, PropertyDescriptor> Propertys;

        /// <summary>
        /// 获取该对象的变量
        /// </summary>
        public FieldInfo[] Fields { get; }

        /// <summary>
        /// 获取当前类所有变量字典
        /// </summary>
        public IDictionary<string, FieldInfo> KeyFields { get; }

        /// <summary>
        /// 获取该对象的成员
        /// </summary>
        public PropertyInfo[] PropertyInfos { get; }

        /// <summary>
        /// 获取当前类所有成员字典
        /// </summary>
        public IDictionary<string, PropertyInfo> KeyParameters { get; }

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
            if (type == null) throw new ArgumentNullException(nameof(type));
            ClassType = type;
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            Fields = ClassType.GetFields(bindingFlags);
            PropertyInfos = ClassType.GetProperties(bindingFlags);
            KeyFields = GetOnlyFieldInfos(ClassType, Fields);
            KeyParameters = ClassFieldDispatcher.GetOnlyPropertys(ClassType, PropertyInfos);
            Propertys = new();
        }

        private bool GetFieldInfo(string name, out FieldInfo field)
        {
            return KeyFields.TryGetValue(name, out field);
        }

        private bool GetPropertyInfo(string name, out PropertyInfo property)
        {
            return KeyParameters.TryGetValue(name, out property);
        }

        private static Dictionary<string, FieldInfo> GetOnlyFieldInfos(Type classType, FieldInfo[] fieldInfos)
        {
            Dictionary<string, FieldInfo> keys = new();
            foreach (var classField in fieldInfos)
            {
                if (!keys.TryAdd(classField.Name, classField))
                {
                    if (classType == classField.DeclaringType)
                    {
                        keys[classField.Name] = classField;
                    }
                }
            }
            return keys;
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
                try
                {
                    if (GetPropertyInfo(name, out var property))
                    {
                        return new PropertyDescriptor(ClassType, property);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("无法获取成员", ex);
                }
                return default;
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
                try
                {
                    if (GetFieldInfo(name, out var field))
                    {
                        return new PropertyDescriptor(ClassType, field);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("无法获取变量", ex);
                }
                return default;
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
    /// 相关类型枚举描述
    /// </summary>
    public enum PropertyEnum
    {
        /// <summary>
        /// <see cref="PropertyInfo"/> 类属性字段
        /// </summary>
        Property,
        /// <summary>
        /// <see cref="FieldInfo"/> 类字段
        /// </summary>
        Field
    }

    /// <summary>
    /// 对象财产托管调度器
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class PropertyDescriptor//<T> where T : MemberInfo
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
        /// 类信息基类<see cref="PropertyInfo"/>或<see cref="FieldInfo"/>
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// 财产描述
        /// </summary>
        public PropertyEnum TypeEnum { get; }

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
            Member = property;
            if (property is not null)
            {
                TypeEnum = PropertyEnum.Property;
                if (property.SetMethod is not null)
                {
                    CanWrite = true;
                    IsStatic = property.SetMethod.IsStatic;
                    SetProperty = CreateSetProperty(classtype, property.SetMethod, property.PropertyType, IsStatic);
                }
                if (property.GetMethod is not null)
                {
                    CanRead = true;
                    IsStatic = property.GetMethod.IsStatic;
                    GetProperty = CreateGetProperty(classtype, property.GetMethod, IsStatic);
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
            Member = fieldInfo;
            if (fieldInfo is not null)
            {
                TypeEnum = PropertyEnum.Field;
                CanWrite = fieldInfo is { IsInitOnly: false, IsLiteral: false };
                CanRead = true;
                IsStatic = fieldInfo.IsStatic;
                if (CanWrite)
                {
                    SetProperty = CreateSetProperty(classtype, fieldInfo, fieldInfo.FieldType, IsStatic);
                }
                GetProperty = CreateGetProperty(classtype, fieldInfo, IsStatic);
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
