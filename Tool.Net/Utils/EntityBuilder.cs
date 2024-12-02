using System.Collections.Generic;
using System.Reflection;
using System;
using Tool.Utils.ActionDelegate;
using System.Linq;
using Tool.Utils.Data;

namespace Tool.Utils
{
    /// <summary>
    /// 用于提高，对象构造（只支持无参构造），对象取值，对象赋值。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class EntityBuilder
    {
        private readonly ClassDispatcher<object> classDispatcher;

        private readonly ClassFieldDispatcher classfieldDispatcher;

        /// <summary>
        /// 对象下公开的字段
        /// </summary>
        public PropertyInfo[] Parameters { get; }

        /// <summary>
        /// 获取当前类所有字段字典
        /// </summary>
        public IDictionary<string, PropertyInfo> KeyParameters { get; }

        /// <summary>
        /// 是否可以构造
        /// </summary>
        public bool IsNew { get; }

        /// <summary>
        /// 对象原型
        /// </summary>
        public Type ClassType { get; }

        /// <summary>
        /// 创建构造模型对象
        /// </summary>
        /// <param name="classtype"></param>
        public EntityBuilder(Type classtype)
        {
            ClassType = classtype ?? throw new ArgumentNullException(nameof(classtype), "参数为空！");
            try
            {
                var constructorInfos = classtype.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                if (constructorInfos.Length == 0) throw new();

                for (int i = 0; i < constructorInfos.Length; i++)
                {
                    var constructor = constructorInfos[i];
                    var parameters = constructor.GetParameters();
                    if (parameters.Length == 0)
                    {
                        classDispatcher = new ClassDispatcher<object>(constructor);
                        IsNew = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                IsNew = false;
            }
            
            classfieldDispatcher = new ClassFieldDispatcher(classtype, ClassField.All);
            Parameters = classfieldDispatcher.Parameters;

            Dictionary<string, PropertyInfo> keyValuePairs = new();
            foreach (var property in Parameters)
            {
                if (!keyValuePairs.TryAdd(property.Name, property))
                {
                    keyValuePairs[property.Name] = property;
                }
            }
            KeyParameters = keyValuePairs.AsReadOnly();// Parameters.ToDictionary(p => p.Name, p => p);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public object New => IsNew ? classDispatcher.Invoke() : throw new($"无法获取{ClassType}，因为无法构造它。");

        /// <summary>
        /// 获取对象的字典数据
        /// </summary>
        /// <param name="_class">对象原型</param>
        /// <returns>字典</returns>
        public IDictionary<string, object> Get(object _class) => classfieldDispatcher.Get(_class);

        /// <summary>
        /// 赋值对象
        /// </summary>
        /// <param name="_class">对象原型</param>
        /// <param name="parameters">字典(字典如标记忽略大小写赋值，就能实现特定行为)</param>
        public void Set(object _class, IDictionary<string, object> parameters) => classfieldDispatcher.Set(_class, parameters);

        /// <summary>
        /// 根据字段名获取字段类型
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="type">字段类型</param>
        /// <returns>是否查找到</returns>
        public bool GetParameterType(string name, out Type type) 
        {
            if (KeyParameters.TryGetValue(name, out var property))
            {
                type = property.PropertyType;
                return true;
            }
            type = null;
            return false;

            //foreach (var info in Parameters)
            //{
            //    if (info.Name == name)
            //    {
            //        return info.PropertyType;
            //    }
            //}
            //return null;
        }

        /// <summary>
        /// 获取 对象 模型
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>实体模型</returns>
        public static EntityBuilder GetEntity(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), "无法将对象制作成可操作模型。对象类型为NULL");
            return StaticData.EntityObjs.GetOrAdd(type, (t) => new(t));
        }

        /// <summary>
        /// 获取 对象 模型
        /// </summary>
        /// <param name="_class">对象</param>
        /// <returns>实体模型</returns>
        public static EntityBuilder GetEntity(object _class)
        {
            return GetEntity(_class.GetType());
        }
    }
}
