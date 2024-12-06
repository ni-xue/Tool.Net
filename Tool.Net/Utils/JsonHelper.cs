using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tool.Utils
{
    /// <summary>
    /// 对 <see cref="JsonSerializer"/> 的拓展类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class JsonHelper
    {
        /// <summary>
        /// 将 <see cref="JsonElement"/> 原型对象 丢入格式成任意实际结果
        /// </summary>
        /// <param name="element">原型对象</param>
        /// <returns>任意实际结果</returns>
        public static object GetReturn(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Undefined => DBNull.Value,
                JsonValueKind.Object => GetObject(element.EnumerateObject()),
                JsonValueKind.Array => GetArray(element.EnumerateArray()),
                JsonValueKind.String => GetString(element),
                JsonValueKind.Number => GetNumber(element),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => null,
            };
        }

        /// <summary>
        /// 将 <see cref="JsonElement"/> 原型对象 丢入格式成 键值对
        /// </summary>
        /// <param name="element">原型对象</param>
        /// <returns>必然是键值对的结果</returns>
        public static Dictionary<string, object> GetObject(JsonElement.ObjectEnumerator element)
        {
            Dictionary<string, object> keyValues = new();
            using (element)
            {
                foreach (var item in element)
                {
                    keyValues.Add(item.Name, GetReturn(item.Value));
                }
            }
            return keyValues;
        }

        /// <summary>
        /// 将 <see cref="JsonElement"/> 原型对象 丢入格式成 数组
        /// </summary>
        /// <param name="element">原型对象</param>
        /// <returns>必然是数组结果</returns>
        public static ArrayList GetArray(JsonElement.ArrayEnumerator element)
        {
            ArrayList array = new();
            using (element)
            {
                foreach (JsonElement item in element)
                {
                    array.Add(GetReturn(item));
                }
            }
            return array;
        }

        /// <summary>
        /// 将 <see cref="JsonElement"/> 原型对象 丢入格式成任意实际结果
        /// </summary>
        /// <param name="element">原型对象</param>
        /// <returns>必然是字符串或时间类型的结果</returns>
        private static string GetString(JsonElement element)
        {
            return element.GetString();
            //object _obj;
            //try
            //{
            //    if (element.TryGetDateTime(out DateTime dateTime))
            //    {
            //        _obj = dateTime;
            //    }
            //    else if (element.TryGetDateTimeOffset(out DateTimeOffset dateTimeOffset))
            //    {
            //        _obj = dateTimeOffset;
            //    }
            //    else
            //    {
            //        _obj = element.GetString();
            //    }
            //}
            //catch (Exception)
            //{
            //    _obj = element.GetString();
            //}

            //return _obj;
        }

        /// <summary>
        /// 将 <see cref="JsonElement"/> 原型对象 丢入格式成任意实际结果
        /// </summary>
        /// <param name="element">原型对象</param>
        /// <returns>必然是数值的结果</returns>
        private static object GetNumber(JsonElement element)
        {
            //object _obj;
            //try
            //{
            if (element.TryGetInt32(out int val0))
            {
                return val0;
            }
            else if (element.TryGetInt64(out long val1))
            {
                return val1;
            }
            else if (element.TryGetDouble(out double val2))
            {
                return val2;
            }
            else
            {
                return element.GetDecimal();
            }
            //    if (element.TryGetDecimal(out decimal val))
            //    {
            //        if (val % 1 == 0)//(decimal.Truncate(val) == val)
            //        {
            //            if (val >= decimal.Zero)
            //            {
            //                if (val <= int.MaxValue)
            //                {
            //                    _obj = decimal.ToInt32(val);
            //                }
            //                else if (val <= long.MaxValue)
            //                {
            //                    _obj = decimal.ToInt64(val);
            //                }
            //                else
            //                {
            //                    _obj = val;
            //                }
            //            }
            //            else
            //            {
            //                if (val >= int.MinValue)
            //                {
            //                    _obj = decimal.ToInt32(val);
            //                }
            //                else if (val >= long.MinValue)
            //                {
            //                    _obj = decimal.ToInt64(val);
            //                }
            //                else
            //                {
            //                    _obj = val;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            _obj = val;
            //        }
            //    }
            //    else
            //    {
            //        _obj = element.GetDouble();
            //    }
            //}
            //catch (Exception)
            //{
            //    _obj = element.GetDecimal();
            //}

            //return _obj;
        }
    }

    /// <summary>
    /// 一种获取 Json 格式数据的实现
    /// </summary>
    public readonly struct JsonVar : IEnumerable<JsonEnumerator>
    {
        /// <summary>
        /// 大概确定 Json 数据的类型
        /// </summary>
        public JsonValueKind ValueKind { get; }

        /// <summary>
        /// 当前 层 可能存在的 集合 长度 （string 会显示 字符长度）
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 当前 Json 的数据结构
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// 添加任意的数据，无规则。
        /// </summary>
        /// <param name="data">一类数据结构</param>
        public JsonVar(object data)
        {
            int count = 0;
            if (data == null)
            {
                ValueKind = JsonValueKind.Null;
            }
            else if (data is IDictionary dic)
            {
                ValueKind = JsonValueKind.Object;
                count = dic.Count;
            }
            else if (data is IList list)
            {
                ValueKind = JsonValueKind.Array;
                count = list.Count;
            }
            else if (data is bool isok)
            {
                ValueKind = isok ? JsonValueKind.True : JsonValueKind.False;
            }
            else if (data is string str)
            {
                ValueKind = JsonValueKind.String;
                count = str.Length;
            }
            else if (data.GetType().IsNumber())
            {
                ValueKind = JsonValueKind.Number;
            }
            else
            {
                ValueKind = JsonValueKind.Undefined;
            }

            this.Data = data;
            this.Count = count;
        }

        /// <summary>
        /// 通过键名获取值
        /// </summary>
        /// <param name="name">键名</param>
        /// <returns></returns>
        public JsonVar this[string name]
        {
            get
            {
                if (ValueKind is JsonValueKind.Object && Data is IDictionary dic && dic.Contains(name))
                {
                    return new JsonVar(dic[name]);
                }
                throw new Exception("对象下不存在字典结构！");
            }
        }

        /// <summary>
        /// 通过下标获取值
        /// </summary>
        /// <param name="i">下标</param>
        /// <returns></returns>
        public JsonVar this[int i]
        {
            get
            {
                if (ValueKind is JsonValueKind.Array && Data is IList list)
                {
                    return new JsonVar(list[i]);
                }
                throw new Exception("对象下不存在数组结构！");
            }
        }

        /// <summary>
        /// 将对象还原成 特定值
        /// </summary>
        /// <typeparam name="T">转换的值</typeparam>
        /// <returns>得到的值</returns>
        public T GetVar<T>()
        {
            return Data.ToVar<T>();
        }

        /// <summary>
        /// 获取当前Json集合下可能存在的数据信息
        /// </summary>
        /// <param name="jsonVar">返回存在的结果</param>
        /// <param name="keys">查找的Key和下标</param>
        /// <returns><see cref="bool"/></returns>
        public bool TryGet(out JsonVar jsonVar, params object[] keys)
        {
            try
            {
                if (keys.Length > 0)
                {
                    switch (ValueKind)
                    {
                        case JsonValueKind.Object:
                            if (keys[0] is string key)
                            {
                                return IsGet(this[key], out jsonVar, keys);
                            }
                            break;
                        case JsonValueKind.Array:
                            if (keys[0] is int i)
                            {
                                return IsGet(this[i], out jsonVar, keys);
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
            jsonVar = new(null);
            return false;

            static bool IsGet(JsonVar json, out JsonVar jsonVar, params object[] keys)
            {
                if (keys.Length > 1)
                {
                    return json.TryGet(out jsonVar, keys[1..]);
                }
                else
                {
                    jsonVar = json;
                    return true;
                }
            }
        }

        /// <summary>
        /// 获取当前对象的Json字符串
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return Data.ToJson();
        }

        /// <summary>
        /// 根据实际类型返回特定内容
        /// </summary>
        /// <returns>结果</returns>
        public override string ToString()
        {
            return ValueKind switch
            {
                JsonValueKind.Object or JsonValueKind.Array => GetJson(),
                JsonValueKind.String or JsonValueKind.Number => Data.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "null",
                _ => base.ToString(),
            };
        }

        /// <summary>
        /// 获取可以用于遍历Json的集合
        /// </summary>
        /// <returns><see cref="IEnumerator{JsonVar}"/></returns>
        /// <exception cref="Exception">不存在可以使用的集合</exception>
        public IEnumerator<JsonEnumerator> GetEnumerator()
        {
            switch (ValueKind)
            {
                case JsonValueKind.Object:
                    if (Data is IDictionary dic)
                    {
                        var enumerator = dic.GetEnumerator();
                        while (enumerator.MoveNext()) yield return new JsonEnumerator(JsonKeyKind.Key, enumerator.Key, new JsonVar(enumerator.Value));
                    }
                    break;
                case JsonValueKind.Array:
                    if (Data is IList list)
                    {
                        for (int i = 0; i < list.Count; i++) yield return new JsonEnumerator(JsonKeyKind.Index, i, new JsonVar(list[i]));
                    }
                    break;
                default:
                    throw new Exception("不存在可以使用的集合！");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// JsonEnumerator
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator JsonVar(JsonEnumerator value) => value.Current;

        /// <summary>
        /// <see cref="Dictionary{String, Object}"/>
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator JsonVar(Dictionary<string, object> value) => new(value);

        /// <summary>
        /// <see cref="Dictionary{String, Object}"/>
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Dictionary<string, object>(JsonVar value) => value.GetVar<Dictionary<string, object>>();

        /// <summary>
        /// ArrayList
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator JsonVar(ArrayList value) => new(value);

        /// <summary>
        /// ArrayList
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ArrayList(JsonVar value) => value.GetVar<ArrayList>();

        /// <summary>
        /// string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator JsonVar(string value) => new(value);

        /// <summary>
        /// string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(JsonVar value) => value.GetVar<string>();

        /// <summary>
        /// bool
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator bool(JsonVar value) => value.GetVar<bool>();

        /// <summary>
        /// int
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator int(JsonVar value) => value.GetVar<int>();

        /// <summary>
        /// long
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator long(JsonVar value) => value.GetVar<long>();

        /// <summary>
        /// double
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator double(JsonVar value) => value.GetVar<double>();

        /// <summary>
        /// decimal
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator decimal(JsonVar value) => value.GetVar<decimal>();
    }

    /// <summary>
    /// 获取集合源
    /// </summary>
    public readonly struct JsonEnumerator
    {
        /// <summary>
        /// 创建集合对象
        /// </summary>
        /// <param name="Kind">Key的类型</param>
        /// <param name="Key">键名或下标</param>
        /// <param name="Current">数据源</param>
        public JsonEnumerator(JsonKeyKind Kind, object Key, JsonVar Current)
        {
            this.Kind = Kind;
            this.Key = Key;
            this.Current = Current;
        }

        /// <summary>
        /// Key的类型
        /// </summary>
        public JsonKeyKind Kind { get; }

        /// <summary>
        /// 键名或下标
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 数据源
        /// </summary>
        public JsonVar Current { get; }
    }

    /// <summary>
    /// Key的类型
    /// </summary>
    public enum JsonKeyKind
    {
        /// <summary>
        /// 字典类型
        /// </summary>
        Key,
        /// <summary>
        /// 数组类型
        /// </summary>
        Index
    }
}
