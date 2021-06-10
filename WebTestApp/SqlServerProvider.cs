using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool;
using Tool.SqlCore;

namespace WebTestApp
{
    public class SqlServerProvider : IDbProvider
    {
		/// <summary>
		/// 根据<see cref="Type"/>类型获取对应的类型
		/// </summary>
		/// <param name="t"><see cref="Type"/>类型</param>
		/// <returns>类型</returns>
		public Enum ConvertToLocalDbType(Type t)
		{
			string key = t.ToString();
            return key switch
            {
                "System.Boolean" => SqlDbType.Bit,
                "System.DateTime" => SqlDbType.DateTime,
                "System.Decimal" => SqlDbType.Decimal,
                "System.Single" => SqlDbType.Float,
                "System.Double" => SqlDbType.Float,
                "System.Byte[]" => SqlDbType.Image,
                "System.Int64" => SqlDbType.BigInt,
                "System.Int32" => SqlDbType.Int,
                "System.String" => SqlDbType.NVarChar,
                "System.Int16" => SqlDbType.SmallInt,
                "System.Byte" => SqlDbType.TinyInt,
                "System.Guid" => SqlDbType.UniqueIdentifier,
                "System.TimeSpan" => SqlDbType.Time,
                "System.Object" => SqlDbType.Variant,
                _ => SqlDbType.NVarChar,
            };
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
		/// 绑定数据
		/// </summary>
		/// <param name="paraName">键</param>
		/// <param name="paraValue">值</param>
		/// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
		/// <param name="paraType">类型</param>
		/// <param name="sourceColumn">源列</param>
		/// <param name="size">大小</param>
		/// <returns></returns>
		public void GetParam(ref DbParameter paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn, int size)
		{
			SqlParameter sqlParameter = paraName as SqlParameter;
			if (paraType != null)
			{
				sqlParameter.SqlDbType = ConvertToLocalDbType(paraType).ToVar<SqlDbType>();
			}
		}


		public string ParameterPrefix => "@";


        /// <summary>
		/// 获取插入数据的主键ID（SQL）
		/// </summary>
		/// <returns></returns>
        public string GetLastIdSql()
        {
            return "SELECT SCOPE_IDENTITY()";
        }

    }
}
