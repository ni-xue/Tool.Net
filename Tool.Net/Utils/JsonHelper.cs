using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
        public static System.Collections.ArrayList GetArray(JsonElement.ArrayEnumerator element)
        {
            System.Collections.ArrayList array = new();

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
        private static object GetString(JsonElement element)
        {
            if (element.TryGetDateTime(out DateTime dateTime))
            {
                return dateTime;
            }
            else if (element.TryGetDateTimeOffset(out DateTimeOffset dateTimeOffset))
            {
                return dateTimeOffset;
            }
            else
            {
                return element.GetString();
            }
        }

        /// <summary>
        /// 将 <see cref="JsonElement"/> 原型对象 丢入格式成任意实际结果
        /// </summary>
        /// <param name="element">原型对象</param>
        /// <returns>必然是数值的结果</returns>
        private static object GetNumber(JsonElement element)
        {
            object _obj;
            if (element.TryGetDecimal(out decimal val))
            {
                if (val % 1 == 0)//(decimal.Truncate(val) == val)
                {
                    if (val >= decimal.Zero)
                    {
                        if (val <= int.MaxValue)
                        {
                            _obj = decimal.ToInt32(val);
                        }
                        else if (val <= long.MaxValue)
                        {
                            _obj = decimal.ToInt64(val);
                        }
                        else
                        {
                            _obj = val;
                        }
                    }
                    else
                    {
                        if (val >= int.MinValue)
                        {
                            _obj = decimal.ToInt32(val);
                        }
                        else if (val >= long.MinValue)
                        {
                            _obj = decimal.ToInt64(val);
                        }
                        else
                        {
                            _obj = val;
                        }
                    }
                }
                else
                {
                    _obj = val;
                }
            }
            else
            {
                _obj = element.GetDouble();
            }

            return _obj;
        }
    }
}
