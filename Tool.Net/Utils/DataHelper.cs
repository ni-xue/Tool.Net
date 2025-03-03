using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Tool.Utils.ActionDelegate;

namespace Tool.Utils
{
    /// <summary>
    /// 数据集帮助类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DataHelper
    {
        internal static IList<DataTableProperty> GetTablePropertys(PropertyInfo[] properties, DataColumnCollection columns)
        {
            List<DataTableProperty> result = new();
            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;
                int index = columns.IndexOf(property.Name);
                if (index != -1)
                {
                    result.Add(new() { Index = index, Property = property, DataType = columns[index].DataType });
                }
            }
            return result;
        }

        /// <summary>
        /// 将<see cref="DataTable"/>转换成实体类对象数组<see cref="IList{TEntity}"/>
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="dt"><see cref="DataTable"/></param>
        /// <returns>返回实体类对象数组</returns>
        public static IList<TEntity> ConvertDataTableToObjects<TEntity>(DataTable dt)
        {
            if (dt == null)
            {
                return null;
            }
            List<TEntity> list = new();
            Type typeFromHandle = typeof(TEntity);
            var modeBuild = EntityBuilder.GetEntity(typeFromHandle);
            var tableProperties = GetTablePropertys(modeBuild.Parameters, dt.Columns);
            foreach (DataRow row in dt.Rows)
            {
                list.Add((TEntity)ConvertRowToObject(modeBuild, tableProperties, row));
            }
            return list;
        }

        /// <summary>
        /// 将<see cref="DataRow"/>转换成实体类对象
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="row"><see cref="DataRow"/></param>
        /// <returns>返回实体类对象</returns>
        public static TEntity ConvertRowToObject<TEntity>(DataRow row)
        {
            if (row == null)
            {
                return default;
            }
            Type typeFromHandle = typeof(TEntity);
            var modeBuild = EntityBuilder.GetEntity(typeFromHandle);
            var tableProperties = GetTablePropertys(modeBuild.Parameters, row.Table.Columns);
            return (TEntity)ConvertRowToObject(modeBuild, tableProperties, row);
        }

        /// <summary>
        /// 将<see cref="DataRow"/>转换成实体类对象
        /// </summary>
        /// <param name="objType">实体类</param>
        /// <param name="row"><see cref="DataRow"/></param>
        /// <returns>返回实体类对象</returns>
        public static object ConvertRowToObject(Type objType, DataRow row)
        {
            if (row == null)
            {
                return null;
            }
            var modeBuild = EntityBuilder.GetEntity(objType);
            var tableProperties = GetTablePropertys(modeBuild.Parameters, row.Table.Columns);
            return ConvertRowToObject(modeBuild, tableProperties, row);

            //DataTable table = row.Table;
            //var modeBuild = EntityBuilder.GetEntity(objType); //object obj = Activator.CreateInstance(objType);
            //object obj = modeBuild.New;
            //var pairs = new Dictionary<string, object>();
            //foreach (var property in modeBuild.Parameters)
            //{
            //    try
            //    {
            //        int index = table.Columns.IndexOf(property.Name);
            //        if (index != -1)
            //        {
            //            object value = row[index];
            //            if (DBNull.Value != value)
            //            {
            //                object obj2 = TypeHelper.ChangeType(property.PropertyType, value);
            //                pairs.Add(property.Name, obj2);
            //            }
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}
            //if (pairs.Count > 0) modeBuild.Set(obj, pairs);

            //foreach (DataColumn dataColumn in table.Columns)
            //{
            //	PropertyInfo property = objType.GetProperty(dataColumn.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            //	if (property != null)
            //	{
            //		Type propertyType = property.PropertyType;
            //		object obj2 = null;
            //		bool flag = true;
            //		try
            //		{
            //			obj2 = TypeHelper.ChangeType(propertyType, row[dataColumn]);
            //		}
            //		catch
            //		{
            //			flag = false;
            //		}
            //		if (flag)
            //		{
            //			object[] args = new object[]
            //			{
            //				obj2
            //			};
            //			objType.InvokeMember(dataColumn.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, obj, args);
            //		}
            //	}
            //}
            //return obj;
        }

        private static object ConvertRowToObject(EntityBuilder builder, IList<DataTableProperty> tableProperties, DataRow row)
        {
            object obj = builder.New;
            var pairs = new Dictionary<string, object>(tableProperties.Count);
            foreach (var property in tableProperties)
            {
                try
                {
                    object value = row[property.Index];
                    if (DBNull.Value != value)
                    {
                        object obj2 = TypeHelper.ChangeType(property.PropertyType, value);
                        if(obj2 is not null) pairs.Add(property.Name, obj2);
                    }
                }
                catch
                {
                }
            }
            if (pairs.Count > 0) builder.Set(obj, pairs);

            return obj;
        }

        /// <summary>
        /// 提取命令参数
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <param name="paraPrefix"></param>
        /// <returns></returns>
        public static IList<string> DistillCommandParameter(string sqlStatement, string paraPrefix)
        {
            List<string> list = new();
            DataHelper.DoDistill(sqlStatement + " ", list, paraPrefix);
            if (list.Count > 0)
            {
                string text = list[^1].Trim();
                if (text.EndsWith(Path.DirectorySeparatorChar))
                {
                    text = text.TrimEnd('"');
                    list.RemoveAt(list.Count - 1);
                    list.Add(text);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <param name="paraList"></param>
        /// <param name="paraPrefix"></param>
        private static void DoDistill(string sqlStatement, IList<string> paraList, string paraPrefix)
        {
            _ = sqlStatement.TrimStart(new char[]
            {
                ' '
            });
            int num = sqlStatement.IndexOf(paraPrefix);
            if (num >= 0)
            {
                int num2 = sqlStatement.IndexOf(" ", num);
                int length = num2 - num;
                string text = sqlStatement.Substring(num, length);
                paraList.Add(text.Replace(paraPrefix, ""));
                DataHelper.DoDistill(sqlStatement[num2..], paraList, paraPrefix);
            }
        }

        /// <summary>
        /// 填充命令参数值
        /// </summary>
        /// <param name="command"></param>
        /// <param name="entityOrRow"></param>
        public static void FillCommandParameterValue(IDbCommand command, object entityOrRow)
        {
            foreach (IDbDataParameter dbDataParameter in command.Parameters)
            {
                dbDataParameter.Value = DataHelper.GetColumnValue(entityOrRow, dbDataParameter.SourceColumn);
                dbDataParameter.Value ??= DBNull.Value;
            }
        }

        /// <summary>
        /// 获取列值
        /// </summary>
        /// <param name="entityOrRow"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static object GetColumnValue(object entityOrRow, string columnName)
        {
            if (entityOrRow is DataRow dataRow)
            {
                return dataRow[columnName];
            }
            return entityOrRow.GetType().InvokeMember(columnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, entityOrRow, null);
        }

        /// <summary>
        /// 获取安全值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static object GetSafeDbValue(object val)
        {
            if (val != null)
            {
                return val;
            }
            return DBNull.Value;
        }

        /// <summary>
        /// 刷新实体字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="row"></param>
        public static void RefreshEntityFields(object entity, DataRow row)
        {
            DataTable table = row.Table;
            List<string> list = new List<string>();
            foreach (DataColumn dataColumn in table.Columns)
            {
                list.Add(dataColumn.ColumnName);
            }
            DataHelper.RefreshEntityFields(entity, row, list);
        }

        /// <summary>
        /// 刷新实体字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="row"></param>
        /// <param name="refreshFields"></param>
        public static void RefreshEntityFields(object entity, DataRow row, IList<string> refreshFields)
        {
            Type type = entity.GetType();
            DataTable table = row.Table;
            var modeBuild = EntityBuilder.GetEntity(type);
            var pairs = new Dictionary<string, object>();
            foreach (var property in modeBuild.Parameters)
            {
                try
                {
                    if (refreshFields.IndexOf(property.Name) != -1) continue;
                    int index = table.Columns.IndexOf(property.Name);
                    if (index != -1)
                    {
                        object value = row[index];
                        if (DBNull.Value != value)
                        {
                            object obj2 = TypeHelper.ChangeType(property.PropertyType, value);
                            pairs.Add(property.Name, obj2);
                        }
                    }
                }
                catch
                {
                }
            }
            if (pairs.Count > 0) modeBuild.Set(entity, pairs);

            //foreach (string current in refreshFields)
            //{
            //    string text = current;
            //    PropertyInfo property = type.GetProperty(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            //    if (property == null)
            //    {
            //        throw new DataHelper.PropertyNotFoundException(text);
            //    }
            //    Type propertyType = property.PropertyType;
            //    object obj = null;
            //    bool flag = true;
            //    try
            //    {
            //        obj = TypeHelper.ChangeType(propertyType, row[text]);
            //    }
            //    catch
            //    {
            //        flag = false;
            //    }
            //    if (flag)
            //    {
            //        object[] args = new object[]
            //        {
            //            obj
            //        };
            //        type.InvokeMember(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, entity, args);
            //    }
            //}
        }

        ///// <summary>
        ///// 自定义错误信息输出类
        ///// </summary>
        //public class PropertyNotFoundException : Exception
        //{
        //    /// <summary>
        //    /// 自定义错误信息输出结果
        //    /// </summary>
        //    public string TargetPropertyName { get; }

        //    /// <summary>
        //    /// 无参构造
        //    /// </summary>
        //    public PropertyNotFoundException()
        //    {
        //    }

        //    /// <summary>
        //    /// 自定义错误信息输出
        //    /// </summary>
        //    /// <param name="propertyName"></param>
        //    public PropertyNotFoundException(string propertyName) : base(string.Format("实体定义中未找到名为“{0}”的属性。", propertyName))
        //    {
        //        this.TargetPropertyName = propertyName;
        //    }
        //}
    }

    /// <summary>
    /// 表示一个 实体字段对应的表下标
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DataTableProperty
    {
        /// <summary>
        /// 表下标
        /// </summary>
        public int Index { get; init; }

        /// <summary>
        /// 对应的值类型
        /// </summary>
        public Type DataType { get; init; }

        /// <summary>
        /// 类字段类型
        /// </summary>
        public Type PropertyType => Property.PropertyType;

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name => Property.Name;

        /// <summary>
        /// 字段信息
        /// </summary>
        public PropertyInfo Property { get; init; }
    }
}
