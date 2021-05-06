using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Tool.Utils
{
    /// <summary>
    /// 对Type进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class TypeExtension
    {
        /// <summary>
        /// 获取当前类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultForType(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 验证是否是系统变量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsType(this Type type)
        {
            if (type == typeof(string) || type == typeof(short) || type == typeof(short?) || type == typeof(int) || type == typeof(int?)
                || type == typeof(long) || type == typeof(long?) || type == typeof(byte) || type == typeof(byte?) || type == typeof(bool)
                || type == typeof(bool?) || type == typeof(char) || type == typeof(char?) || type == typeof(decimal) || type == typeof(decimal?)
                || type == typeof(double) || type == typeof(double?) || type == typeof(float) || type == typeof(float?) || type == typeof(object)
                || type == typeof(ushort) || type == typeof(ushort?) || type == typeof(uint) || type == typeof(uint?) || type == typeof(ulong)
                || type == typeof(ulong?) || type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return true;
            }

            return false;
            //if (type == typeof(short))
            //{

            //}
            //if (type == typeof(int))
            //{

            //}
            //if (type == typeof(long))
            //{

            //}
            //if (type == typeof(byte))
            //{

            //}
            //if (type == typeof(bool))
            //{

            //}
            //if (type == typeof(char))
            //{

            //}
            //if (type == typeof(decimal))
            //{

            //}
            //if (type == typeof(double))
            //{

            //}
            //if (type == typeof(float))
            //{

            //}
            //if (type == typeof(object))
            //{

            //}
            //if (type == typeof(ushort))
            //{

            //}
            //if (type == typeof(uint))
            //{

            //}
            //if (type == typeof(ulong))
            //{

            //}
            //if (type == typeof(DateTime))
            //{

            //}
        }

        /// <summary>
        /// 获取当前对象下所有属性集合
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <returns>属性集合</returns>
        public static PropertyDescriptorCollection GetProperties(this object obj)
        {
            PropertyDescriptorCollection propertyDescriptor = TypeDescriptor.GetProperties(obj);

            return propertyDescriptor;
        }

        /// <summary>
        /// 获取当前对象下所有属性集合
        /// </summary>
        /// <param name="componentType">对象源类型</param>
        /// <returns>属性集合</returns>
        public static PropertyDescriptorCollection GetProperties(Type componentType)
        {
            PropertyDescriptorCollection propertyDescriptor = TypeDescriptor.GetProperties(componentType);

            return propertyDescriptor;
        }

        /// <summary>
        /// 获取当前对象下指定名称的属性对象
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static PropertyDescriptor GetPropertieFind(this object obj, string name, bool ignoreCase = false)
        {
            PropertyDescriptor property = obj.GetProperties().Find(name, ignoreCase);

            return property;
        }

        /// <summary>
        /// 根据属性对象获取属性的值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="descriptor">属性对象</param>
        /// <returns>属性值</returns>
        public static object GetValue(this object obj, PropertyDescriptor descriptor)
        {
            return descriptor.GetValue(obj);
        }

        /// <summary>
        /// 根据属性对象修改属性的值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="descriptor">属性对象</param>
        /// <param name="value">修改属性的值</param>
        public static void SetValue(this object obj, PropertyDescriptor descriptor, object value)
        {
            descriptor.SetValue(obj, value);
        }

        /// <summary>
        /// 获取指定属性值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>返回属性值</returns>
        public static object GetValue(this object obj, string name, bool ignoreCase = false)
        {
            object _obj = default;
            PropertyDescriptor descriptor = obj.GetPropertieFind(name, ignoreCase);
            if (!object.Equals(descriptor, null))
            {
                _obj = obj.GetValue(descriptor);
            }
            return _obj;
        }

        /// <summary>
        /// 修改指定属性值
        /// </summary>
        /// <param name="obj">对象源</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">修改的值</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>返回是否查找到并进行修改</returns>
        public static bool SetValue(this object obj, string name, object value, bool ignoreCase = false)
        {
            PropertyDescriptor descriptor = obj.GetPropertieFind(name, ignoreCase);
            if (!object.Equals(descriptor, null))
            {
                obj.SetValue(descriptor, value);
                return true;
            }
            return false;
        }
    }
}
