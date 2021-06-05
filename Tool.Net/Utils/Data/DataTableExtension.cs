using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对DataTable进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DataTableExtension
    {
        /// <summary>
        /// 判断<see cref="DataTable"/>对象中的是否为空，行为空，对象为空
        /// </summary>
        /// <param name="table"><see cref="DataTable"/>对象</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsEmpty(this DataTable table)
        {
            if (object.Equals(table, null))
            {
                return true;
            }
            if (object.Equals(table.Rows, null))
            {
                return true;
            }
            if (table.Rows.Count == 0)
            {
                return true;
            }
            if (table.Rows[0].IsEmpty())
            {
                return true;
            }
            return false;
            //return table.Equals(null) && table.Rows.Equals(null) && table.Rows.Count == 0 && table.Rows[0].IsEmpty();
            //return !object.Equals(table, null) && !object.Equals(table.Rows, null) && table.Rows.Count != 0;
        }

        /// <summary>
        /// （DataTable）转换 <see cref="Dictionary{T, I}"/> 集合
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>返回Dictionary</returns>
        public static List<Dictionary<string, object>> ToDictionary(this DataTable table)
        {
            //if (!table.IsEmpty())
            //{
            //    List<Dictionary<string, object>> parentRow = new();
            //    Dictionary<string, object> childRow;
            //    foreach (DataRow row in table.Rows)
            //    {
            //        childRow = new Dictionary<string, object>();
            //        foreach (DataColumn col in table.Columns)
            //        {
            //            childRow.Add(col.ColumnName, row[col]);
            //        }
            //        parentRow.Add(childRow);
            //    }
            //    return parentRow;
            //}
            //return default;

            return table.ToDictionaryIf(null);
        }

        /// <summary>
        /// （DataTable）转换 <see cref="Dictionary{T, I}"/> 集合（结果可自定义）
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="func">用于指定特殊结果的函数</param>
        /// <returns>返回Dictionary</returns>
        public static List<Dictionary<string, object>> ToDictionaryIf(this DataTable table, Func<string, object, object> func)
        {
            //if (func == null) throw new ArgumentNullException(nameof(func), "请实现该方法，验证版！");
            if (!table.IsEmpty())
            {
                List<Dictionary<string, object>> parentRow = new();
                Dictionary<string, object> childRow;
                foreach (DataRow row in table.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        object _obj = func?.Invoke(col.ColumnName, row[col]) ?? row[col];
                        childRow.Add(col.ColumnName, _obj);
                    }
                    parentRow.Add(childRow);
                }
                return parentRow;
            }
            return default;
        }

        /// <summary>
        /// （DataTable）转换 <see cref="object"/> 集合
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>返回dynamic[]</returns>
        public static dynamic[] ToObject(this DataTable dataTable)
        {
            return dataTable.Select().ToObject();
        }

        /// <summary>
        /// 序列化AjaxJson（DataTable）
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>返回JSON字符串</returns>
        public static string ToJSON(this DataTable table)
        {
            return ToJSON(table, false);
        }

        /// <summary>
        /// 序列化AjaxJson（DataTable）
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <returns>返回JSON字符串</returns>
        public static string ToJSON(this DataTable table, bool IsDate)
        {
            return ToJSON(table, IsDate, null);
        }

        /// <summary>
        /// 序列化AjaxJson（DataTable）
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <param name="ToDateString">Date.ToString()的写法。</param>
        /// <returns>返回JSON字符串</returns>
        public static string ToJSON(this DataTable table, bool IsDate, string ToDateString)
        {
            if (!table.IsEmpty())
            {
                List<Dictionary<string, object>> parentRow = new();
                Dictionary<string, object> childRow;
                foreach (DataRow row in table.Rows)
                {
                    childRow = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
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
                return parentRow.ToJson();
            }
            return "[]";
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转换为 实体对象 (老版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="table">数据源</param>
        /// <returns>返回实体对象</returns>
        [Obsolete("当前方法，已被弃用，有更新的方法，请使用（ToEntity<T>）方法", false)]
        public static T DataTableToEntity<T>(this DataTable table) where T : new()
        {
            if (!table.IsEmpty())
            {
                return DataHelper.ConvertRowToObject<T>(table.Rows[0]);
            }
            return default(T);
        }

        /// <summary>
        ///  将<see cref="DataTable"/>对象 转换为 <see cref="List{T}"/>实体对象 (老版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="table">数据源</param>
        /// <returns>返回<see cref="List{T}"/>实体对象</returns>
        [Obsolete("当前方法，已被弃用，有更新的方法，请使用（ToEntityList<T>）方法", false)]
        public static IList<T> DataTableToEntityList<T>(this DataTable table) where T : new()
        {
            if (!table.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(table);
            }
            return default(IList<T>);
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转换为 实体对象，不管有表中有几条数据指读出第一条 (优化版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataTable">数据源</param>
        /// <returns>返回实体对象数组</returns>
        public static T ToEntity<T>(this DataTable dataTable) where T : new()
        {
            return dataTable.ToEntity<T>(0);
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转换为 实体对象，不管有表中有几条数据只读出一条 (优化版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataTable">数据源</param>
        /// <param name="index">要读取的那一条数组的下标</param>
        /// <returns>返回实体对象数组</returns>
        public static T ToEntity<T>(this DataTable dataTable, int index) where T : new()
        {
            if (!dataTable.IsEmpty())
            {
                try
                {
                    Type type = typeof(T);
                    List<PropertyInfo> _properties = new(type.GetProperties());

                    Dictionary<PropertyInfo, DataColumn> keys = new();
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        foreach (PropertyInfo property in _properties)
                        {
                            if (property.Name.Equals(dataColumn.ColumnName, StringComparison.OrdinalIgnoreCase))
                            {
                                keys.Add(property, dataColumn);
                                _properties.Remove(property);
                                break;
                            }
                        }
                    }

                    DataRow dataRow = dataTable.Rows[index];

                    T m = Activator.CreateInstance<T>();

                    foreach (KeyValuePair<PropertyInfo, DataColumn> _keyValue in keys)
                    {
                        object value = dataRow[_keyValue.Value];

                        if (DBNull.Value != value)
                        {
                            if (_keyValue.Key.PropertyType != typeof(string))
                            {
                                _keyValue.Key.SetValue(m, value.ToVar(_keyValue.Key.PropertyType, false));
                            }
                            else
                            {
                                _keyValue.Key.SetValue(m, value);
                            }
                        }
                    }

                    return m;
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
            return default(T);
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转换为 实体对象数组 (优化版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataTable">数据源</param>
        /// <returns>返回实体对象数组</returns>
        public static IList<T> ToEntityList<T>(this DataTable dataTable) where T : new()
        {
            //if (dataTable.IsEmpty())
            //{
            //    try
            //    {
            //        //DataTable _dataTable = dataTable.Copy();

            //        Type type = typeof(T);
            //        //PropertyInfo[] properties = type.GetProperties();

            //        List<PropertyInfo> _properties = new List<PropertyInfo>(type.GetProperties());

            //        //Dictionary<string, DataColumn> keys = new Dictionary<string, DataColumn>();

            //        Dictionary<PropertyInfo, DataColumn> keys = new Dictionary<PropertyInfo, DataColumn>();
            //        foreach (DataColumn dataColumn in dataTable.Columns)
            //        {
            //            foreach (PropertyInfo property in _properties)
            //            {
            //                if (property.Name.Equals(dataColumn.ColumnName, StringComparison.OrdinalIgnoreCase))
            //                {
            //                    keys.Add(property, dataColumn);
            //                    _properties.Remove(property);
            //                    break;
            //                }
            //                //else
            //                //{
            //                //    _dataTable.Columns.Remove(dataColumn.Caption);
            //                //}
            //            }
            //        }

            //        IList<T> ts = new List<T>();

            //        foreach (DataRow dataRow in dataTable.Rows)
            //        {
            //            T m = DataRowExtension.ToEntity<T>(keys, dataRow);

            //            //foreach (KeyValuePair<PropertyInfo, DataColumn> _keyValue in keys)
            //            //{
            //            //    object value = dataRow[_keyValue.Value];//dataRow[property.Name.ToLower()]; 

            //            //    if (DBNull.Value != value)
            //            //    {
            //            //        if (_keyValue.Key.PropertyType != typeof(string))
            //            //        {
            //            //            _keyValue.Key.SetValue(m, value.ToVar(_keyValue.Key.PropertyType, false));
            //            //        }
            //            //        else if (_keyValue.Key.PropertyType != value.GetType())
            //            //        {
            //            //            _keyValue.Key.SetValue(m, value.ToVar(_keyValue.Key.PropertyType, false));
            //            //        }
            //            //        else
            //            //        {
            //            //            _keyValue.Key.SetValue(m, value);
            //            //        }
            //            //    }
            //            //}

            //            //foreach (PropertyInfo property in _properties)
            //            //{
            //            //    object value = dataRow[keys[property.Name.ToLower()]];//dataRow[property.Name.ToLower()]; 

            //            //    if (DBNull.Value != value)
            //            //    {
            //            //        if (property.PropertyType != typeof(string))
            //            //        {
            //            //            property.SetValue(m, value.ToVar(property.PropertyType, false));
            //            //        }
            //            //        else
            //            //        {
            //            //            property.SetValue(m, value);
            //            //        }
            //            //    }
            //            //}
            //            ts.Add(m);
            //        }
            //        return ts;
            //    }
            //    catch (Exception)
            //    {
            //        return default(IList<T>);
            //    }
            //}
            return dataTable.ToEntityList<T>(null);
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转换为 实体对象数组 (优化版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataTable">数据源</param>
        /// <param name="indexs">要获取的多个下标的数据</param>
        /// <returns>返回实体对象数组</returns>
        public static IList<T> ToEntityList<T>(this DataTable dataTable, params int[] indexs) where T : new()
        {
            if (!dataTable.IsEmpty())
            {
                try
                {
                    //Type type = typeof(T);

                    //List<PropertyInfo> _properties = new List<PropertyInfo>(type.GetProperties());

                    //Dictionary<PropertyInfo, DataColumn> keys = new Dictionary<PropertyInfo, DataColumn>();
                    //foreach (DataColumn dataColumn in dataTable.Columns)
                    //{
                    //    foreach (PropertyInfo property in _properties)
                    //    {
                    //        if (property.Name.Equals(dataColumn.ColumnName, StringComparison.OrdinalIgnoreCase)) 
                    //        {
                    //            keys.Add(property, dataColumn);
                    //            _properties.Remove(property);
                    //            break;
                    //        }
                    //    }
                    //}

                    Dictionary<PropertyInfo, DataColumn> keys = DataRowExtension.GetDataPropertys<T>(dataTable.Columns);

                    DataRow[] dataRows; 
                    if (indexs != null)
                    {
                        dataRows = new DataRow[indexs.Length];
                        for (int i = 0; i < dataRows.Length; i++)
                        {
                            dataRows[i] = dataTable.Rows[indexs[i]];
                        }
                    }
                    else
                    {
                        dataRows = new DataRow[dataTable.Rows.Count];
                        dataTable.Rows.CopyTo(dataRows, 0);
                    }

                    IList<T> ts = new List<T>();

                    foreach (DataRow dataRow in dataRows)
                    {
                        T m = DataRowExtension.ToEntity<T>(keys, dataRow);

                        //T m = Activator.CreateInstance<T>();

                        //foreach (KeyValuePair<PropertyInfo, DataColumn> _keyValue in keys)
                        //{
                        //    object value = dataRow[_keyValue.Value];//dataRow[property.Name.ToLower()]; 

                        //    if (DBNull.Value != value)
                        //    {
                        //        if (_keyValue.Key.PropertyType != typeof(string))
                        //        {
                        //            _keyValue.Key.SetValue(m, value.ToVar(_keyValue.Key.PropertyType, false));
                        //        }
                        //        else
                        //        {
                        //            _keyValue.Key.SetValue(m, value);
                        //        }
                        //    }
                        //}

                        ts.Add(m);
                    }
                    return ts;
                }
                catch (Exception)
                {
                    return default;
                }
            }
            return default;
        }
    }
}
