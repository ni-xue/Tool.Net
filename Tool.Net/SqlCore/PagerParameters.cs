using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.SqlCore
{
    /// <summary>
    /// 根据指定分页信息查询SQL
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class PagerParameters
    {
        /// <summary>
        /// SQL表查询的字段 AS 别名
        /// </summary>
        public string[] FieldAlias { get; set; }

        /// <summary>
        /// SQL表查询的字段
        /// </summary>
        public string[] Fields { get; set; }

        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 显示方式，例如：倒序，顺序（ORDER By ID DESC）
        /// </summary>
        public string PKey { get; set; }

        /// <summary>
        /// 使用表名还是使用SQL执行分页（true：SQL，false：表名）
        /// </summary>
		public bool IsSql { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
		public string Table { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
		public string WhereStr { get; set; }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        public PagerParameters()
        {
            this.PageIndex = 1;
            this.PageSize = 100;
            this.PKey = "";
            this.WhereStr = "";
            this.Table = "";
            this.IsSql = false;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="table">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="pageIndex">页索引</param>
        public PagerParameters(string table, string pkey, int pageIndex)
        {
            this.PageSize = 20;
            this.Table = table;
            this.PKey = pkey;
            this.PageIndex = pageIndex;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="table">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        public PagerParameters(string table, string pkey, int pageIndex, int pageSize) : this(table, pkey, pageIndex)
        {
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="table">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="whereStr">查询条件</param>
        public PagerParameters(string table, string pkey, int pageIndex, string whereStr) : this(table, pkey, pageIndex)
        {
            this.WhereStr = whereStr;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="table">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        public PagerParameters(string table, string pkey, string whereStr, int pageIndex, int pageSize) : this(table, pkey, pageIndex, whereStr)
        {
            this.PageSize = pageSize;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="table">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="fields">SQL表查询的字段</param>
        public PagerParameters(string table, string pkey, string whereStr, int pageIndex, int pageSize, string[] fields) : this(table, pkey, whereStr, pageIndex, pageSize)
        {
            this.Fields = fields;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="table">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="fields">SQL表查询的字段</param>
        /// <param name="fieldAlias">SQL表查询的字段 AS 别名</param>
        public PagerParameters(string table, string pkey, string whereStr, int pageIndex, int pageSize, string[] fields, string[] fieldAlias) : this(table, pkey, whereStr, pageIndex, pageSize, fields)
        {
            this.FieldAlias = fieldAlias;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="sql">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="fields">SQL表查询的字段</param>
        /// <param name="issql">使用表名还是使用SQL执行分页（true：SQL，false：表名）</param>
        public PagerParameters(string sql, string pkey, string whereStr, int pageIndex, int pageSize, string[] fields, bool issql) : this(sql, pkey, whereStr, pageIndex, pageSize)
        {
            this.Fields = fields;
            this.IsSql = issql;
        }

        /// <summary>
        /// 实例化分页对象
        /// </summary>
        /// <param name="sql">查询表名</param>
        /// <param name="pkey">显示方式，例如：倒序，顺序（ORDER By ID DESC）</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="fields">SQL表查询的字段</param>
        /// <param name="fieldAlias">SQL表查询的字段 AS 别名</param>
        /// <param name="issql">使用表名还是使用SQL执行分页（true：SQL，false：表名）</param>
        public PagerParameters(string sql, string pkey, string whereStr, int pageIndex, int pageSize, string[] fields, string[] fieldAlias, bool issql) : this(sql, pkey, whereStr, pageIndex, pageSize, fields, issql)
        {
            this.FieldAlias = fieldAlias;
        }
    }
}
