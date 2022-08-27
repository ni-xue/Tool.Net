using System.Collections.Generic;
using System.Reflection;
using System;
using Tool.Utils.ActionDelegate;

namespace Tool.Utils
{
    /// <summary>
    /// 用于提高，对象构造（只支持无参构造），对象取值，对象赋值。
    /// </summary>
    public class EntityBuilder
    {
        private readonly ClassDispatcher<object> classDispatcher;

        private readonly ClassFieldDispatcher classfieldDispatcher;

        /// <summary>
        /// 对象下公开的字段
        /// </summary>
        public PropertyInfo[] Parameters { get; set; }

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
        /// <param name="parameters">字典</param>
        public void Set(object _class, IDictionary<string, object> parameters) => classfieldDispatcher.Set(_class, parameters);

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
