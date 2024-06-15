using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对Dictionary进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 将 <see cref="IDictionary{TKey, TValue}"/>对象 拷贝创建新 <see cref="Dictionary{TKey, TValue}"/>对象
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="oldDictionary"><see cref="IDictionary{TKey, TValue}"/>对象</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        public static Dictionary<TKey, TValue> NewDictionary<TKey, TValue>(this IDictionary<TKey, TValue> oldDictionary)
        {
            return new Dictionary<TKey, TValue>(oldDictionary);
        }

        /// <summary>
        /// 将对象转换成<see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="source">对象</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/></returns>
        public static IDictionary<string, object> ToIDictionary(this object source)
        {
            return source.ToIDictionary<object>();
        }

        /// <summary>
        /// 将对象转换成<see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="source">对象</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        public static Dictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();// as Dictionary<string, object>;
        }

        /// <summary>
        /// 将对象转换成<see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="source">对象</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/></returns>
        public static IDictionary<string, T> ToIDictionary<T>(this object source)
        {
            return source.ToDictionary<T>();
        }

        /// <summary>
        /// 将对象转换成<see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">对象</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/></returns>
        public static Dictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "无法将对象转换为字典。源对象为NULL");

            var dictionary = new Dictionary<string, T>();
            var entityBuilder = EntityBuilder.GetEntity(source);
            foreach (PropertyInfo property in entityBuilder.Parameters) //PropertyDescriptor property is TypeDescriptor.GetProperties(source)
                AddPropertyToDictionary(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyInfo property, object source, Dictionary<string, T> dictionary) //PropertyDescriptor property
        {
            object value = property.GetValue(source);
            if (IsOfType(value, out T data))
            {
                dictionary.Add(property.Name, data);
            }
            //else if (typeof(T) == typeof(object) && value == null)
            //{
            //    dictionary.Add(property.Name, default);
            //}
        }

        private static bool IsOfType<T>(object value, out T data)
        {
            if (value is T _data)
            {
                data = _data;
                return true;
            }
            if (typeof(T) == typeof(object))
            {
                data = (T)value;
                return true;
            }
            data = default;
            return false;
        }

        /// <summary>
        /// 获取对象结果集<see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="source">对象</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/>对象结果集</returns>
        public static IDictionary<string, object> GetDictionary(this object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "无法将对象转换为字典。源对象为NULL");
            var entityBuilder = EntityBuilder.GetEntity(source);
            return entityBuilder.Get(source);
        }

        /// <summary>
        /// 给对象赋值，使用字典赋值
        /// </summary>
        /// <param name="source">对象</param>
        /// <param name="parameters">赋值键值对</param>
        public static void SetDictionary(this object source, IDictionary<string, object> parameters)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "无法将对象转换为字典。源对象为NULL");
            var entityBuilder = EntityBuilder.GetEntity(source);
            entityBuilder.Set(source, parameters);
        }

        /// <summary>
        /// 批量删除 <see cref="IDictionary{TKey, TValue}"/>对象 出现不包含的会返回 false
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="keys"><see cref="IDictionary{TKey, TValue}"/>对象</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        /// <param name="key">需要删除的键值集合</param>
        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> keys, params TKey[] key)
        {
            //if (!object.Equals(key, null) && key.Length > 0)
            //{
            //    foreach (var _key in key)
            //    {
            //        if (!keys.Remove(_key)) throw new ArgumentNullException("某个key不存在：", $"警告：对外key（{_key}）不存在,无法删除！");
            //    }
            //}

            bool type = true;
            if (!keys.TryRemove(out _, key))
            {
                type = false;
                //throw new ArgumentNullException("某个key不存在：", $"警告：key（{_key}）不存在,无法删除！");
            }
            return type;
        }

        /// <summary>
        /// 批量删除 <see cref="IDictionary{TKey, TValue}"/>对象 中的值
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="trykey">删除失败时返回无法删除的哪些项。</param>
        /// <param name="keys"><see cref="IDictionary{TKey, TValue}"/>对象</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        /// /// <param name="key">需要删除的键值集合</param>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> keys, out TKey[] trykey, params TKey[] key)
        {
            bool type = true;
            trykey = default;
            if (!object.Equals(key, null) && key.Length > 0)
            {
                List<TKey> keys1 = new();
                foreach (var _key in key)
                {
                    if (!keys.Remove(_key))
                    {
                        keys1.Add(_key);

                        type = false;
                        //return false; 
                    }
                }
                if (keys1.Any()) trykey = keys1.ToArray();
            }
            return type;
        }

#if  NET5_0 || NET6_0

        /// <summary>
        /// 将键值对转换成只读类型键值对
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="keys">原本键值对</param>
        /// <returns>只读键值对</returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> keys)
        {
            return keys is not null ? new ReadOnlyDictionary<TKey, TValue>(keys) : null;
        }

#endif

        /// <summary>
        /// 按照ASCII码从小到大排序（示例： 1, 2, A, B, a, b 这是格式循序）
        /// </summary>
        /// <param name="keyValuePairs">Dictionary</param>
        /// <returns>返回重新排序好的结果</returns>
        public static Dictionary<TKey, TValue> GetParamAscii<TKey, TValue>(this Dictionary<TKey, TValue> keyValuePairs)
        {
            var vDic = keyValuePairs.OrderBy(x => x.Key.ToString(), new ComparerTest()).ToDictionary(x => x.Key, y => y.Value);
            //var param = keyValuePairs.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
            //(from objDic in keyValuePairs orderby objDic.Key ascending select objDic);
            //Array.Sort(keyValues, string.CompareOrdinal); //ASCII排序
            return vDic;
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="keyValuePairs">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static Dictionary<TKey, TValue> GetArrayIndex<TKey, TValue>(this Dictionary<TKey, TValue> keyValuePairs, int index, int count) where TKey : new() where TValue : new()
        {
            if (keyValuePairs == null)
            {
                throw new System.SystemException("该Dictionary<TKey, TValue>为空！");
            }
            if (index > count)
            {
                throw new System.SystemException("count不能小于index，数组越界！");
            }
            if (index < 0)
            {
                throw new System.SystemException("index不能小于0，数组越界！");
            }
            if (count < 0)
            {
                throw new System.SystemException("count不能小于0，数组越界！");
            }
            if (keyValuePairs.Count < index)
            {
                throw new System.SystemException("index超出了数组，数组越界！");
            }
            if (keyValuePairs.Count < count)
            {
                throw new System.SystemException("count超出了数组，数组越界！");
            }
            Dictionary<TKey, TValue> keyValuePairs1 = new();

            int Index = 0;

            foreach (var keyValue in keyValuePairs)
            {
                if (Index >= index && Index <= count)
                {
                    keyValuePairs1.Add(keyValue.Key, keyValue.Value);
                }
                if (Index >= count)
                {
                    break;
                }
                Index++;
                //keyValue.
            }
            return keyValuePairs1;
        }
    }

    class ComparerTest : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
