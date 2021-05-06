using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Tool.SqlCore
{
	/// <summary>
	/// 获取相关Sql对应的数据类型
	/// </summary>
	public class SqlServerProvider : IDbProvider
	{
		/// <summary>
		/// 根据<see cref="Type"/>类型获取对应的类型
		/// </summary>
		/// <param name="t"><see cref="Type"/>类型</param>
		/// <returns>类型</returns>
		public object ConvertToLocalDbType(Type t)
		{
			string key = t.ToString();
			switch (key)
			{
				case "System.Boolean":
					return SqlDbType.Bit;
				case "System.DateTime":
					return SqlDbType.DateTime;
				case "System.Decimal":
					return SqlDbType.Decimal;
				case "System.Single":
					return SqlDbType.Float;
				case "System.Double":
					return SqlDbType.Float;
				case "System.Byte[]":
					return SqlDbType.Image;
				case "System.Int64":
					return SqlDbType.BigInt;
				case "System.Int32":
					return SqlDbType.Int;
				case "System.String":
					return SqlDbType.NVarChar;
				case "System.Int16":
					return SqlDbType.SmallInt;
				case "System.Byte":
					return SqlDbType.TinyInt;
				case "System.Guid":
					return SqlDbType.UniqueIdentifier;
				case "System.TimeSpan":
					return SqlDbType.Time;
				case "System.Object":
					return SqlDbType.Variant;
			}
			return SqlDbType.Int;
		}

		/// <summary>
		/// 根据<see cref="Type"/>类型获取对应的类型字符串
		/// </summary>
		/// <param name="netType"><see cref="Type"/>类型</param>
		/// <returns>类型字符串</returns>
		public string ConvertToLocalDbTypeString(Type netType)
		{
			string key = netType.ToString();
			switch (key)
			{
				case "System.Boolean":
					return "bit";
				case "System.DateTime":
					return "datetime";
				case "System.Decimal":
					return "decimal";
				case "System.Single":
					return "float";
				case "System.Double":
					return "float";
				case "System.Int64":
					return "bigint";
				case "System.Int32":
					return "int";
				case "System.String":
					return "nvarchar";
				case "System.Int16":
					return "smallint";
				case "System.Byte":
					return "tinyint";
				case "System.Guid":
					return "uniqueidentifier";
				case "System.TimeSpan":
					return "time";
				case "System.Byte[]":
					return "image";
				case "System.Object":
					return "sql_variant";
			}
			return null;
		}

		/// <summary>
		/// 验证对象信息，并填充进<see cref="SqlCommand"/>集合中
		/// </summary>
		/// <param name="cmd">参数</param>
		public void DeriveParameters(IDbCommand cmd)
		{
			if (cmd is SqlCommand)
			{
				SqlCommandBuilder.DeriveParameters(cmd as SqlCommand);
			}
		}

		/// <summary>
		/// 获取插入数据的主键ID（SQL）
		/// </summary>
		/// <returns></returns>
		public string GetLastIdSql()
		{
			return "SELECT SCOPE_IDENTITY()";
		}

		/// <summary>
		/// 获取当前对象的实例
		/// </summary>
		/// <param name="dbProviderType">根据枚举类型获取指定的数据库对象</param>
		/// <param name="dbProviderName">数据库类型定义名称</param>
		/// <returns></returns>
		public DbProviderFactory Instance(DbProviderType dbProviderType, string dbProviderName)
		{
            DbProviderFactory sql;
            if (DbProviderType.Unknown == dbProviderType)
            {
				sql = ProviderFactory.GetFactory(dbProviderName);
			}
            else
            {
				 sql = ProviderFactory.GetDbProviderFactory(dbProviderType);
			}

			if (sql == null) throw new Exception($"数据库无法连接，原因是在系统中找不到可用的程序集。");

			return sql;
		}

		/// <summary>
		/// 无用，返回true
		/// </summary>
		/// <returns></returns>
		public bool IsBackupDatabase()
		{
			return true;
		}

		/// <summary>
		/// 无用，返回true
		/// </summary>
		/// <returns></returns>
		public bool IsCompactDatabase()
		{
			return true;
		}

		/// <summary>
		/// 无用，返回true
		/// </summary>
		/// <returns></returns>
		public bool IsDbOptimize()
		{
			return true;
		}

		/// <summary>
		/// 无用，返回true
		/// </summary>
		/// <returns></returns>
		public bool IsFullTextSearchEnabled()
		{
			return true;
		}

		/// <summary>
		/// 无用，返回true
		/// </summary>
		/// <returns></returns>
		public bool IsShrinkData()
		{
			return true;
		}

		/// <summary>
		/// 无用，返回true
		/// </summary>
		/// <returns></returns>
		public bool IsStoreProc()
		{
			return true;
		}

		/// <summary>
		/// 绑定数据
		/// </summary>
		/// <param name="paraName">键</param>
		/// <param name="paraValue">值</param>
		/// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
		/// <returns></returns>
		public DbParameter MakeParam(string paraName, object paraValue, ParameterDirection direction)
		{
			Type paraType = null;
			if (paraValue != null)
			{
				paraType = paraValue.GetType();
			}
			return this.MakeParam(paraName, paraValue, direction, paraType, null);
		}

		/// <summary>
		/// 绑定数据
		/// </summary>
		/// <param name="paraName">键</param>
		/// <param name="paraValue">值</param>
		/// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
		/// <param name="paraType">类型</param>
		/// <param name="sourceColumn">源列</param>
		/// <returns></returns>
		public DbParameter MakeParam(string paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn)
		{
			return this.MakeParam(paraName, paraValue, direction, paraType, sourceColumn, 0);
		}

		/// <summary>
		/// 绑定数据
		/// </summary>
		/// <param name="paraName">键</param>
		/// <param name="paraValue">值</param>
		/// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
		/// <param name="paraType">类型</param>
		/// <param name="sourceColumn">源列</param>
		/// <param name="size">大小</param>
		/// <returns></returns>
		public DbParameter MakeParam(string paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn, int size)
		{
			SqlParameter sqlParameter = new SqlParameter
			{
				ParameterName = this.ParameterPrefix + paraName
			};
			if (paraType != null)
			{
				sqlParameter.SqlDbType = this.ConvertToLocalDbType(paraType).ToVar<SqlDbType>();
			}
			sqlParameter.Value = paraValue;
			if (sqlParameter.Value == null)
			{
				sqlParameter.Value = DBNull.Value;
			}
			sqlParameter.Direction = direction;
			if (direction != ParameterDirection.Output || paraValue != null)
			{
				sqlParameter.Value = paraValue;
			}
			if (direction == ParameterDirection.Output)
			{
				sqlParameter.Size = size;
			}
			if (sourceColumn != null)
			{
				sqlParameter.SourceColumn = sourceColumn;
			}
			return sqlParameter;
		}

		/// <summary>
		/// 就是这个符号'@'
		/// </summary>
		public string ParameterPrefix
		{
			get
			{
				return "@";
			}
		}
	}
}
