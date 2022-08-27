using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对List进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class ListExtension
    {
        /// <summary>
        /// 将实体转换为JSON格式字符串 （再三强调，要是实体，而且是实集合必须是<see cref="List{T}"/>。）
        /// </summary>
        /// <param name="list">实体</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntitysToJson(this IList list)
        {
            return EntitysToJson(list);
        }

        /// <summary>
        /// 将实体转换为JSON格式字符串 （再三强调，要是实体，而且是实集合必须是<see cref="List{T}"/>。）
        /// </summary>
        /// <param name="list">实体</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntitysToJson(this IList list, bool IsDate)
        {
            return EntitysToJson(list, IsDate, null);
        }

        /// <summary>
        /// 将实体转换为JSON格式字符串 （再三强调，要是实体，而且是实集合必须是<see cref="List{T}"/>。）
        /// </summary>
        /// <param name="list">实体</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <param name="ToDateString">Date.ToString()的写法。</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntitysToJson(this IList list, bool IsDate, string ToDateString)
        {
            if (list == null)
            {
                throw new System.SystemException("该object为空！");
            }

            List<IDictionary<string, object>> _list = new();

            foreach (object _obj in list)
            {
                IDictionary<string, object> keyValuePairs = _obj.GetDictionary();

                if (IsDate)
                {
                    Dictionary<string, object> childRow = new();

                    foreach (var Pairs in keyValuePairs)
                    {
                        var value = Pairs.Value;

                        if (value.GetType() == typeof(DateTime))
                        {
                            DateTime dateTime = value.ToVar<DateTime>();

                            if (!string.IsNullOrWhiteSpace(ToDateString))
                            {
                                childRow.Add(Pairs.Key, dateTime.ToString(ToDateString));
                            }
                            else
                            {
                                childRow.Add(Pairs.Key, dateTime.ToString());
                            }
                        }
                        else
                        {
                            childRow.Add(Pairs.Key, value);
                        }
                    }

                    _list.Add(childRow);
                }
                else
                {
                    _list.Add(keyValuePairs);
                }
            }

            return _list.ToJson();
        }

        /// <summary>
        /// 确定是否 <see cref="T:System.Collections.Generic.List`1" /> 中的每个元素都与指定的谓词所定义的条件相匹配。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="match">条件</param>
        /// <param name="routs">List数组</param>
        /// <returns>该方法返回，成功，或失败。</returns>
        public static bool TrueForAll<T>(this IList<T> routs, Predicate<T> match) where T : new()
        {
            if (match == null)
            {
                return false;
            }
            for (int i = 0; i < routs.Count; i++)
            {
                if (match(routs[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 按照ASCII码从小到大排序（未实现）（示例： 1, 2, A, B, a, b 这是格式循序）
        /// </summary>
        /// <param name="list">List</param>
        /// <returns>返回重新排序好的结果</returns>
        public static List<T> GetParamASCII<T>(this List<T> list) where T : new()
        {
            //string[] vv = { "1", "2", "A", "a", "B", "b" };
            //Array.Sort(vv, string.CompareOrdinal); //ASCII排序

            list.Sort((x, y) => string.Compare(x.ToString(), y.ToString()));

            return list.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="list">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static List<T> GetArrayIndex<T>(this IList<T> list, int index, int count) where T : new()
        {
            if (list == null)
            {
                throw new System.SystemException("该List<T>为空！");
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
            if (list.Count < index)
            {
                throw new System.SystemException("index超出了数组，数组越界！");
            }
            if (list.Count < count)
            {
                throw new System.SystemException("count超出了数组，数组越界！");
            }
            List<T> obj1 = new();

            for (int i = index; i < count; i++)
            {
                obj1.Add(list[i]);
            }
            return obj1;
        }
    }
}
