using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对DataRow进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DataRowExtension
    {
        /// <summary>
        /// 判断<see cref="DataRow"/>对象中的是否为空，行为空，对象为空
        /// </summary>
        /// <param name="dataRow"><see cref="DataRow"/>对象</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsEmpty(this DataRow dataRow)
        {
            if (object.Equals(dataRow, null))
            {
                return true;
            }
            //if (dataRow.IsNull(0))
            //{

            //}
            if (dataRow.ItemArray.Length == 0)
            {
                return true;
            }
            return false;
            //return dataRow.Equals(null) && dataRow.ItemArray.Length == 0;
            //return !object.Equals(dataRow, null) && dataRow.ItemArray.Length != 0;
        }

        /// <summary>
        /// （DataRow）转换 <see cref="Dictionary{I, T}"/> 集合
        /// </summary>
        /// <param name="dataRow">DataRow</param>
        /// <returns>返回Dictionary</returns>
        public static Dictionary<string, object> ToDictionary(this DataRow dataRow)
        {
            if (dataRow.IsEmpty()) return default;
            Dictionary<string, object> childRow = new();

            foreach (DataColumn col in dataRow.Table.Columns)
            {
                childRow.Add(col.ColumnName, dataRow[col]);
            }
            return childRow;
        }

        /// <summary>
        /// 将<see cref="DataRow"/>对象 转换为 实体对象（旧版本）
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataRow">数据源</param>
        /// <returns>返回实体对象</returns>
        [Obsolete("当前方法，已被弃用，有更新的方法，请使用（ToEntity<T>）方法", false)]
        public static T DataRowToEntity<T>(this DataRow dataRow) where T : new()
        {
            if (dataRow.IsEmpty()) return default;
            return DataHelper.ConvertRowToObject<T>(dataRow);
        }

        /// <summary>
        /// 将<see cref="DataRow"/>对象 转换为 实体对象（新版本）
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataRow">数据源</param>
        /// <returns>返回实体对象</returns>
        public static T ToEntity<T>(this DataRow dataRow) where T : new()
        {
            if (!dataRow.IsEmpty())
            {
                try
                {
                    var modeBuild = EntityBuilder.GetEntity(typeof(T));
                    var tableProperties = DataHelper.GetTablePropertys(modeBuild.Parameters, dataRow.Table.Columns);

                    T m = DataRowExtension.ToEntity<T>(modeBuild, tableProperties, dataRow);

                    return m;

                    //Dictionary<string, DataColumn> keys = new Dictionary<string, DataColumn>();
                    //foreach (DataColumn dataColumn in dataRow.Table.Columns)
                    //{
                    //    keys.Add(dataColumn.ColumnName.ToLower(), dataColumn);
                    //}

                    //Type type = typeof(T);
                    //T m = Activator.CreateInstance<T>();
                    //PropertyInfo[] properties = type.GetProperties();
                    //foreach (PropertyInfo property in properties)
                    //{
                    //    if (keys.ContainsKey(property.Name.ToLower()))
                    //    {
                    //        object value = dataRow[keys[property.Name.ToLower()]];

                    //        if (DBNull.Value != value)
                    //        {
                    //            if (property.PropertyType != typeof(string))
                    //            {
                    //                property.SetValue(m, value.ToVar(property.PropertyType, false));
                    //            }
                    //            else if (property.PropertyType != value.GetType())
                    //            {
                    //                property.SetValue(m, value.ToVar(property.PropertyType, false));
                    //            }
                    //            else
                    //            {
                    //                property.SetValue(m, value);
                    //            }
                    //        }
                    //    }
                    //}
                    //return m;
                }
                catch (Exception)
                {
                    return default;
                }
            }
            return default;
        }

        /// <summary>
        /// 将<see cref="DataRowCollection"/> 对象 转换为 实体对象数组 (优化版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dataRows">数据源</param>
        /// <returns>返回实体对象数组</returns>
        public static T[] ToEntityList<T>(this DataRowCollection dataRows) where T : new()
        {
            try
            {
                T[] ts = new T[dataRows.Count];
                var modeBuild = EntityBuilder.GetEntity(typeof(T));
                IList<DataTableProperty> tableProperties = null;
                for (int i = 0; i < ts.Length; i++)
                {
                    DataRow dataRow = dataRows[i];
                    if (!dataRow.IsEmpty())
                    {
                        tableProperties ??= DataHelper.GetTablePropertys(modeBuild.Parameters, dataRow.Table.Columns);
                        //Dictionary<PropertyInfo, DataColumn> keys = DataRowExtension.GetDataPropertys<T>(dataRow.Table.Columns);

                        //T m = DataRowExtension.ToEntity<T>(keys, dataRow);

                        T m = DataRowExtension.ToEntity<T>(modeBuild, tableProperties, dataRow);

                        ts[i] = m;
                    }
                }

                return ts;
                //Dictionary<string, DataColumn> keys = new Dictionary<string, DataColumn>();
                //foreach (DataColumn dataColumn in dataRow.Table.Columns)
                //{
                //    keys.Add(dataColumn.ColumnName.ToLower(), dataColumn);
                //}

                //Type type = typeof(T);
                //T m = Activator.CreateInstance<T>();
                //PropertyInfo[] properties = type.GetProperties();
                //foreach (PropertyInfo property in properties)
                //{
                //    if (keys.ContainsKey(property.Name.ToLower()))
                //    {
                //        object value = dataRow[keys[property.Name.ToLower()]];

                //        if (DBNull.Value != value)
                //        {
                //            if (property.PropertyType != typeof(string))
                //            {
                //                property.SetValue(m, value.ToVar(property.PropertyType, false));
                //            }
                //            else if (property.PropertyType != value.GetType())
                //            {
                //                property.SetValue(m, value.ToVar(property.PropertyType, false));
                //            }
                //            else
                //            {
                //                property.SetValue(m, value);
                //            }
                //        }
                //    }
                //}
                //return m;
            }
            catch (Exception)
            {
                return default;
            }
        }

        //internal static Dictionary<PropertyInfo, int> GetDataPropertys<T>(DataColumnCollection Columns)
        //{
        //    Type type = typeof(T);

        //    List<PropertyInfo> _properties = new(type.GetProperties());

        //    Dictionary<PropertyInfo, DataColumn> keys = new();
        //    foreach (DataColumn dataColumn in Columns)
        //    {
        //        foreach (PropertyInfo property in _properties)
        //        {
        //            if (property.Name.Equals(dataColumn.ColumnName, StringComparison.OrdinalIgnoreCase))
        //            {
        //                keys.Add(property, dataColumn);
        //                _properties.Remove(property);
        //                break;
        //            }
        //        }
        //    }

        //    return keys;
        //}

        internal static T ToEntity<T>(EntityBuilder builder, IList<DataTableProperty> tableProperties, DataRow row) where T : new()
        {
            object obj = builder.New;
            var pairs = new Dictionary<string, object>();
            foreach (var property in tableProperties)
            {
                object value = row[property.Index];
                if (DBNull.Value != value)
                {
                    object obj2;
                    if (property.Property.PropertyType != value.GetType() || property.Property.PropertyType != typeof(string))
                    {
                        obj2 = value.ToVar(property.Property.PropertyType, false);
                    }
                    else
                    {
                        obj2 = value;
                    }
                    pairs.Add(property.Property.Name, obj2);
                }
            }
            if (pairs.Count > 0) builder.Set(obj, pairs);


            //T m = Activator.CreateInstance<T>();

            //foreach (KeyValuePair<PropertyInfo, DataColumn> _keyValue in prkeys)
            //{
            //    object value = dataRow[_keyValue.Value];//dataRow[property.Name.ToLower()]; 

            //    if (DBNull.Value != value)
            //    {
            //        if (_keyValue.Key.PropertyType != typeof(string))
            //        {
            //            _keyValue.Key.SetValue(m, value.ToVar(_keyValue.Key.PropertyType, false));
            //        }
            //        else if (_keyValue.Key.PropertyType != value.GetType())
            //        {
            //            _keyValue.Key.SetValue(m, value.ToVar(_keyValue.Key.PropertyType, false));
            //        }
            //        else
            //        {
            //            _keyValue.Key.SetValue(m, value);
            //        }
            //    }
            //}

            return (T)obj;
        }


        /// <summary>
        /// （DataRow[]）转换 <see cref="Dictionary{I, T}"/> 集合
        /// </summary>
        /// <param name="dataRows">DataRow[]</param>
        /// <returns>返回List{Dictionary{string, object}}</returns>
        public static List<Dictionary<string, object>> ToDictionary(this DataRow[] dataRows)
        {
            List<Dictionary<string, object>> parentRow = new();
            Dictionary<string, object> childRow;

            foreach (DataRow row in dataRows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in row.Table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return parentRow;
        }

        /// <summary>
        /// （DataRow）转换 <see cref="object"/> 对象
        /// </summary>
        /// <param name="dataRows">DataRow</param>
        /// <returns>返回dynamic</returns>
        public static dynamic ToObject(this DataRow dataRows)
        {
            if (dataRows.IsEmpty()) return null;
            //return new DataRow[] { dataRows }.ToObject(0);

            dynamic Dynamic = new ExpandoObject();

            if (Dynamic is ExpandoObject)
            {
                var add = ((IDictionary<string, object>)Dynamic);

                foreach (DataColumn col in dataRows.Table.Columns)
                {
                    add.Add(col.ColumnName, dataRows[col]);
                }
            }

            return Dynamic;
        }

        /// <summary>
        /// （DataRowCollection）转换 <see cref="object"/> 集合
        /// </summary>
        /// <param name="dataRows">DataRowCollection</param>
        /// <returns>返回dynamic[]</returns>
        public static dynamic[] ToObject(this DataRowCollection dataRows)
        {
            object[] Dynamics = new object[dataRows.Count];

            int i = 0;
            foreach (DataRow row in dataRows)
            {
                Dynamics[i] = row.ToObject();
                i++;
            }
            return Dynamics;
        }

        /// <summary>
        /// （DataRowCollection）转换 <see cref="object"/> 
        /// </summary>
        /// <param name="dataRows">DataRowCollection</param>
        /// <param name="index">要读取的那一条数组的下标</param>
        /// <returns>返回dynamic</returns>
        public static dynamic ToObject(this DataRowCollection dataRows, int index)
        {
            DataRow row = dataRows[index];
            return row.ToObject();
        }
    }
}
