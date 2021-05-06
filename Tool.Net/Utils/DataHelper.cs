using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Tool.Utils
{
	/// <summary>
	/// 数据集帮助类
	/// </summary>
	public class DataHelper
	{
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
			IList<TEntity> list = new List<TEntity>();
			foreach (DataRow row in dt.Rows)
			{
				list.Add(DataHelper.ConvertRowToObject<TEntity>(row));
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
			return (TEntity)DataHelper.ConvertRowToObject(typeFromHandle, row);
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
			DataTable table = row.Table;
			object obj = Activator.CreateInstance(objType);
			foreach (DataColumn dataColumn in table.Columns)
			{
				PropertyInfo property = objType.GetProperty(dataColumn.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				if (property != null)
				{
					Type propertyType = property.PropertyType;
					object obj2 = null;
					bool flag = true;
					try
					{
						obj2 = TypeHelper.ChangeType(propertyType, row[dataColumn.ColumnName]);
					}
					catch
					{
						flag = false;
					}
					if (flag)
					{
						object[] args = new object[]
						{
							obj2
						};
						objType.InvokeMember(dataColumn.ColumnName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, obj, args);
					}
				}
			}
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
			sqlStatement += " ";
			IList<string> list = new List<string>();
			DataHelper.DoDistill(sqlStatement, list, paraPrefix);
			if (list.Count > 0)
			{
				string text = list[list.Count - 1].Trim();
				if (text.EndsWith("\""))
				{
					text.TrimEnd(new char[]
					{
						'"'
					});
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
			sqlStatement.TrimStart(new char[]
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
				DataHelper.DoDistill(sqlStatement.Substring(num2), paraList, paraPrefix);
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
				if (dbDataParameter.Value == null)
				{
					dbDataParameter.Value = DBNull.Value;
				}
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
			IList<string> list = new List<string>();
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
			foreach (string current in refreshFields)
			{
				string text = current;
				PropertyInfo property = type.GetProperty(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				if (property == null)
				{
					throw new DataHelper.PropertyNotFoundException(text);
				}
				Type propertyType = property.PropertyType;
				object obj = null;
				bool flag = true;
				try
				{
					obj = TypeHelper.ChangeType(propertyType, row[text]);
				}
				catch
				{
					flag = false;
				}
				if (flag)
				{
					object[] args = new object[]
					{
						obj
					};
					type.InvokeMember(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, entity, args);
				}
			}
		}

		/// <summary>
		/// 自定义错误信息输出类
		/// </summary>
		public class PropertyNotFoundException : Exception
		{
			/// <summary>
			/// 自定义错误信息输出结果
			/// </summary>
			public string TargetPropertyName { get; }

			/// <summary>
			/// 无参构造
			/// </summary>
			public PropertyNotFoundException()
			{
			}

			/// <summary>
			/// 自定义错误信息输出
			/// </summary>
			/// <param name="propertyName"></param>
			public PropertyNotFoundException(string propertyName) : base(string.Format("实体定义中未找到名为“{0}”的属性。", propertyName))
			{
				this.TargetPropertyName = propertyName;
			}
		}
	}
}
