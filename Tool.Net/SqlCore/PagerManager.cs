using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Tool.SqlCore
{
	/// <summary>
	/// SQL分页核心类，目前暂时已被废除，后续会考虑实际情况，补充几种数据库的分页SQL。
	/// </summary>
	/// <remarks>代码由逆血提供支持</remarks>
	public class PagerManager
	{
		///// <summary>
		///// 实例化
		///// </summary>
		///// <param name="dbHelper">Sql核心对象</param>
		//public PagerManager(DbHelper dbHelper)
		//{
		//	this.m_dbHelper = dbHelper;
		//}

		///// <summary>
		///// 实例化 默认为SqlServer数据库
		///// </summary>
		///// <param name="connectionString">Sql连接字符串</param>
		///// <param name="dbProviderType">访问的数据库类型</param>
		//public PagerManager(string connectionString, DbProviderType dbProviderType)
		//{
		//	this.m_dbHelper = new DbHelper(connectionString, dbProviderType);
		//}

		///// <summary>
		///// 实例化 默认为SqlServer数据库
		///// </summary>
		///// <param name="prams">分页信息</param>
		///// <param name="dbHelper">Sql核心对象</param>
		//public PagerManager(PagerParameters prams, DbHelper dbHelper)
		//{
		//	this.m_prams = prams;
		//	this.m_dbHelper = dbHelper;
		//}

		///// <summary>
		///// 实例化 默认为SqlServer数据库
		///// </summary>
		///// <param name="prams">分页信息</param>
		///// <param name="connectionString">Sql连接字符串</param>
		//public PagerManager(PagerParameters prams, string connectionString)
		//{
		//	this.m_prams = prams;
		//	this.m_dbHelper = new DbHelper(connectionString);
		//	if (prams.CacherSize > 0)
		//	{
		//		this.m_fixedCacher = new Dictionary<int, PagerSet>(prams.CacherSize);
		//	}
		//}

		///// <summary>
		///// 暂时无用的方法
		///// </summary>
		///// <param name="index"></param>
		///// <param name="pagerSet"></param>
		//private void CacheObject(int index, PagerSet pagerSet)
		//{
		//	if (this.m_fixedCacher != null)
		//	{
		//		this.m_fixedCacher.Add(index, pagerSet);
		//		return;
		//	}
		//	if (this.m_prams.CacherSize > 0)
		//	{
		//		this.m_fixedCacher = new Dictionary<int, PagerSet>(this.m_prams.CacherSize)
		//		{
		//			{ index, pagerSet }
		//		};
		//	}
		//}

		///// <summary>
		///// 获取分页对象方法
		///// </summary>
		///// <param name="index"></param>
		///// <returns></returns>
		//private PagerSet GetCachedObject(int index)
		//{
		//	if (this.m_fixedCacher == null)
		//	{
		//		return null;
		//	}
		//	if (!this.m_fixedCacher.ContainsKey(index))
		//	{
		//		return null;
		//	}
		//	return this.m_fixedCacher[index];
		//}

		/// <summary>
		/// 将查询SQL字段的对象拼接
		/// </summary>
		/// <param name="fields">字段数组</param>
		/// <param name="fieldAlias">别名的字段数组</param>
		/// <returns>返回查询的SQL字段字符串</returns>
		public static string GetFieldString(string[] fields, string[] fieldAlias)
		{
			fields ??= new string[]{ "*" };
			StringBuilder str = new();
            for (int i = 0; i < fields.Length; i++)
            {
				str.AppendFormat(" {0}{1}, ", fields[i], fieldAlias == null ? string.Empty : $" as {fieldAlias[i]}");
            }
			str.Insert(str.Length - 2, ' ');
            return str.ToString(0, str.Length - 2);

			//string text = "";
			//if (fieldAlias == null)
			//{
			//	for (int i = 0; i < fields.Length; i++)
			//	{
			//		text = $"{text} {fields[i]}";
			//		if (i != fields.Length - 1)
			//		{
			//			text += " , ";
			//		}
			//		else
			//		{
			//			text += " ";
			//		}
			//	}
			//	return text;
			//}
			//for (int i = 0; i < fields.Length; i++)
			//{
			//	text = $"{text} {fields[i]}";
			//	if (fieldAlias[i] != null)
			//	{
			//		text = $"{text} as {fieldAlias[i]}";
			//	}
			//	if (i != fields.Length - 1)
			//	{
			//		text += " , ";
			//	}
			//	else
			//	{
			//		text += " ";
			//	}
			//}
			//return text;
		}

		///// <summary>
		///// 获取分页实体对象
		///// </summary>
		///// <returns></returns>
		//public PagerSet GetPagerSet()
		//{
		//	return this.GetPagerSet(this.m_prams);
		//}

		//public PagerSet GetPagerSet(PagerParameters pramsPager)
		//{
		//	if (this.m_prams == null)
		//	{
		//		this.m_prams = pramsPager;
		//	}
		//	if (pramsPager.PageIndex < 0)
		//	{
		//		return null;
		//	}
		//	List<DbParameter> list = new List<DbParameter>
		//	{
		//		this.m_dbHelper.MakeInParam("TableName", pramsPager.Table),
		//		this.m_dbHelper.MakeInParam("ReturnFields", this.GetFieldString(pramsPager.Fields, pramsPager.FieldAlias)),
		//		this.m_dbHelper.MakeInParam("PageSize", pramsPager.PageSize),
		//		this.m_dbHelper.MakeInParam("PageIndex", pramsPager.PageIndex),
		//		this.m_dbHelper.MakeInParam("Where", pramsPager.WhereStr),
		//		this.m_dbHelper.MakeInParam("Orderfld", pramsPager.PKey),
		//		this.m_dbHelper.MakeInParam("OrderType", pramsPager.Ascending ? 0 : 1),
		//		this.m_dbHelper.MakeOutParam("PageCount", typeof(int)),
		//		this.m_dbHelper.MakeOutParam("RecordCount", typeof(int))
		//	};
		//	DataSet pageSet = new DataSet();
		//	return new PagerSet(pramsPager.PageIndex, pramsPager.PageSize, Convert.ToInt32(list[list.Count - 3].Value), Convert.ToInt32(list[list.Count - 2].Value), pageSet)
		//	{
		//		PageSet = 
		//		{
		//			DataSetName = "PagerSet_" + pramsPager.Table
		//		}
		//	};
		//}

		///// <summary>
		///// 分页核心方法
		///// </summary>
		///// <param name="pramsPager">分页参数</param>
		///// <returns>返回分页对象实体</returns>
		//public PagerSet GetPagerSet(PagerParameters pramsPager)
		//{
		//	//if (this.m_prams == null)
		//	//{
		//	//	this.m_prams = pramsPager;
		//	//}
		//	if (pramsPager.PageIndex < 0)
		//	{
		//		return null;
		//	}
		//	List<DbParameter> list = new List<DbParameter>
		//	{
		//		this.m_dbHelper.MakeInParam("IsSql", pramsPager.IsSql ? 1 : 0),
		//		this.m_dbHelper.MakeInParam("TableName", pramsPager.Table),
		//		this.m_dbHelper.MakeInParam("ReturnFields", GetFieldString(pramsPager.Fields, pramsPager.FieldAlias)),
		//		this.m_dbHelper.MakeInParam("PageSize", pramsPager.PageSize),
		//		this.m_dbHelper.MakeInParam("PageIndex", pramsPager.PageIndex),
		//		this.m_dbHelper.MakeInParam("Where", pramsPager.WhereStr),
		//		this.m_dbHelper.MakeInParam("Order", pramsPager.PKey),
		//		this.m_dbHelper.MakeOutParam("PageCount", typeof(int)),
		//		this.m_dbHelper.MakeOutParam("RecordCount", typeof(int))
		//	};
		//	this.m_dbHelper.RunProc("WEB_PageView", list, out DataSet pageSet);
		//	return new PagerSet(pramsPager.PageIndex, pramsPager.PageSize, Convert.ToInt32(list[list.Count - 3].Value), Convert.ToInt32(list[list.Count - 2].Value), pageSet)
		//	{
		//		PageSet =
		//		{
		//			DataSetName = "PagerSet_" + pramsPager.Table
		//		}
		//	};
		//}

		///// <summary>
		///// 私有底层对象
		///// </summary>
		//private readonly DbHelper m_dbHelper;

		///// <summary>
		///// 私有分页对象
		///// </summary>
		//private IDictionary<int, PagerSet> m_fixedCacher;

		///// <summary>
		///// 私有分页查询对象
		///// </summary>
		//private PagerParameters m_prams;
	}
}
