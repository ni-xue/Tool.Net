using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对DataSet进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DataSetExtension
    {
        /// <summary>
        /// 判断<see cref="DataSet"/>对象中的是否为空，行为空，对象为空（验证每张表，只要其中包含一张表有值，都成立。）
        /// </summary>
        /// <param name="data"><see cref="DataSet"/>对象</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsEmpty(this DataSet data)
        {
            if (object.Equals(data, null))
            {
                return true;
            }
            if (object.Equals(data.Tables, null))
            {
                return true;
            }
            if (data.Tables.Count == 0)
            {
                return true;
            }
            bool result = true;
            foreach (DataTable table in data.Tables)
            {
                if (!table.IsEmpty())
                {
                    result = false;
                    break;
                }
            }
            return result;
            //return object.Equals(data, null) && object.Equals(data.Tables,null) && data.Tables.Count == 0 && data.Tables[0].IsEmpty();//.Equals(null) && data.Tables[0].Rows.Count == 0
            //return data != null && data.Tables != null && (data.Tables == null || (data.Tables.Count != 0 && data.Tables[0] != null && data.Tables[0].Rows.Count != 0));
        }

        /// <summary>
        /// （DataTable）转换 <see cref="List{T}"/> 集合
        /// </summary>
        /// <param name="data">DataSet</param>
        /// <returns>返回Dictionary</returns>
        public static List<List<Dictionary<string, object>>> ToDictionary(this DataSet data)
        {
            if (data.IsEmpty()) return default;
            List<List<Dictionary<string, object>>> Jsons = new();
            foreach (DataTable Data in data.Tables)
            {
                List<Dictionary<string, object>> parentRow = new();
                Dictionary<string, object> childRow;
                foreach (DataRow row in Data.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in Data.Columns)
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                    parentRow.Add(childRow);
                }
                Jsons.Add(parentRow);
            }
            return Jsons;
        }

        /// <summary>
        /// 序列化AjaxJson（DataSet）
        /// </summary>
        /// <param name="data">DataSet</param>
        /// <returns>返回JSON字符串</returns>
        public static string[] ToJSON(this DataSet data)
        {
            return ToJSON(data, false);
        }

        /// <summary>
        /// 序列化AjaxJson（DataSet）
        /// </summary>
        /// <param name="data">DataSet</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <returns>返回JSON字符串</returns>
        public static string[] ToJSON(this DataSet data, bool IsDate)
        {
            return ToJSON(data, IsDate, null);
        }

        /// <summary>
        /// 序列化AjaxJson（DataSet）
        /// </summary>
        /// <param name="data">DataSet</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <param name="ToDateString">Date.ToString()的写法。</param>
        /// <returns>返回JSON字符串</returns>
        public static string[] ToJSON(this DataSet data, bool IsDate, string ToDateString)
        {
            if (data.IsEmpty()) return default;
            List<string> Jsons = new();
            foreach (DataTable Data in data.Tables)
            {
                List<Dictionary<string, object>> parentRow = new();
                Dictionary<string, object> childRow;
                foreach (DataRow row in Data.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in Data.Columns)
                    {
                        if (IsDate)
                        {
                            var value = row[col];

                            if (value.GetType() == typeof(DateTime))
                            {
                                DateTime dateTime = value.ToVar<DateTime>();

                                if (!string.IsNullOrWhiteSpace(ToDateString))
                                {
                                    childRow.Add(col.ColumnName, dateTime.ToString(ToDateString));
                                }
                                else
                                {
                                    childRow.Add(col.ColumnName, dateTime.ToString());
                                }
                            }
                            else
                            {
                                childRow.Add(col.ColumnName, value);
                            }
                        }
                        else
                        {
                            childRow.Add(col.ColumnName, row[col]);
                        }
                    }
                    parentRow.Add(childRow);
                }
                Jsons.Add(parentRow.ToJson());
            }
            return Jsons.ToArray();
        }
    }
}
