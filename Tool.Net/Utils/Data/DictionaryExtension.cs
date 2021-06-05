using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            return source.ToIDictionary<object>() as Dictionary<string, object>;
        }

        /// <summary>
        /// 将 <see cref="IDictionary{TKey, TValue}"/>对象 转换成 <see cref="Dictionary{TKey, TValue}"/>对象
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="oldDictionary"><see cref="IDictionary{TKey, TValue}"/>对象</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> oldDictionary)
        {
            return new Dictionary<TKey, TValue>(oldDictionary);
        }

        /// <summary>
        /// 将对象转换成<see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">对象</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/></returns>
        public static IDictionary<string, T> ToIDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
            { 
                dictionary.Add(property.Name, (T)value);
            }
            else if (typeof(T) == typeof(object) && value == null)
            {
                dictionary.Add(property.Name, default);
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "无法将对象转换为字典。源对象为NULL");
        }

        /// <summary>
        /// 批量删除 <see cref="IDictionary{TKey, TValue}"/>对象 中的值（异常提示）
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

            if (!keys.TryRemove(out TKey _key, key))
            {
                throw new ArgumentNullException("某个key不存在：", $"警告：key（{_key}）不存在,无法删除！");
            }
            return true;
        }

        /// <summary>
        /// 批量删除 <see cref="IDictionary{TKey, TValue}"/>对象 中的值（异常返回值）
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="trykey">删除失败时返回无法删除的哪一项。</param>
        /// <param name="keys"><see cref="IDictionary{TKey, TValue}"/>对象</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        /// /// <param name="key">需要删除的键值集合</param>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> keys, out TKey trykey, params TKey[] key)
        {
            if (!object.Equals(key, null) && key.Length > 0)
            {
                foreach (var _key in key)
                {
                    if (!keys.Remove(_key)) 
                    {
                        trykey = _key;
                        return false; 
                    }
                }
            }
            trykey = default;
            return true;
        }

        /// <summary>
        /// 将键值对转换成只读类型键值对
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="keys">原本键值对</param>
        /// <returns>只读键值对</returns>
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> keys)
        {
            return keys is not null ? new System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(keys) : null;
        }

        /// <summary>
        /// 按照ASCII码从小到大排序（示例： 1, 2, A, B, a, b 这是格式循序）
        /// </summary>
        /// <param name="keyValuePairs">Dictionary</param>
        /// <returns>返回重新排序好的结果</returns>
        public static Dictionary<TKey, TValue> GetParamASCII<TKey, TValue>(this Dictionary<TKey, TValue> keyValuePairs)
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
            Dictionary<TKey, TValue> keyValuePairs1 = new Dictionary<TKey, TValue>();

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
