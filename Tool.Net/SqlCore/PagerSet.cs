using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Tool.Utils.Data;

namespace Tool.SqlCore
{
    /// <summary>
    /// 分页数据对象
    /// </summary>
	[Serializable]
    public class PagerSet
    {
        /// <summary>
        /// 源数据集合
        /// </summary>
		public DataSet PageSet
        {
            get;
            set;
        }

        /// <summary>
        /// 源数据集合第一条
        /// </summary>
		public DataTable PageTable
        {
            get;
            set;
        }

        /// <summary>
        /// 私有对象，延迟加载数据
        /// </summary>
        private Lazy<object> p_pageEntitys
        {
            get;
            set;
        }

        /// <summary>
        /// 获取时加载的实体类
        /// </summary>
        public object PageEntitys
        {
            get { return p_pageEntitys == null ? null : p_pageEntitys.Value; }
        }

        /// <summary>
        /// 可分为多少页（页数）
        /// </summary>
        public int PageCount
        {
            get;
            set;
        }

        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// SQL数据表总行数
        /// </summary>
        public int RecordCount
        {
            get;
            set;
        }

        /// <summary>
        /// 实例化，无参构造
        /// </summary>
        public PagerSet()
        {
            this.PageIndex = 1;
            this.PageSize = 10;
            this.PageCount = 0;
            this.RecordCount = 0;
            this.PageSet = new DataSet("PagerSet");
            this.PageTable = new DataTable("PagerTable");
        }

        /// <summary>
        /// 实例化，对象
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageCount">可分为多少页</param>
        /// <param name="recordCount">SQL数据表总行数</param>
        /// <param name="pageSet">数据对象</param>
        public PagerSet(int pageIndex, int pageSize, int pageCount, int recordCount, DataSet pageSet)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.PageCount = pageCount;
            this.RecordCount = recordCount;
            this.PageSet = pageSet;

            if (pageSet != null && pageSet.Tables.Count > 0)
            {
                PageTable = pageSet.Tables[0];

                PageTable.TableName = $"{pageSet.DataSetName}_PagerTable_1";
            }
        }

        /// <summary>
        /// 可用于验证当前<see cref="DataSet"/>，PageSet 是否非空
        /// </summary>
        /// <returns>返回状态</returns>
        public bool CheckedPageSet()
        {
            return !this.PageSet.IsEmpty();
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转换为 实体对象数组 (优化版)
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <returns>返回实体对象数组</returns>
        public void PageToEntityList<T>() where T : new()
        {
            if (this.PageTable != null)
            {
                p_pageEntitys = new Lazy<object>(() => { return PageTable.ToEntityList<T>(); });
            }
        }

        /// <summary>
        /// 返回当前<see cref="DataSet"/>，PageTable 的JSON格式字符串数组
        /// </summary>
        /// <returns>JSON格式字符串数组</returns>
        public string[] PageSetJson()
        {
            return this.PageSet.SetToJson();
        }

        /// <summary>
        /// 返回当前<see cref="DataSet"/>，PageTable 的JSON格式字符串数组
        /// </summary>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <returns>JSON格式字符串数组</returns>
        public string[] PageSetJson(bool IsDate)
        {
            return this.PageSet.SetToJson(IsDate);
        }

        /// <summary>
        /// 返回当前<see cref="DataSet"/>，PageTable 的JSON格式字符串数组
        /// </summary>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <param name="ToDateString">Date.ToString()的写法。</param>
        /// <returns>JSON格式字符串数组</returns>
        public string[] PageSetJson(bool IsDate, string ToDateString)
        {
            return this.PageSet.SetToJson(IsDate, ToDateString);
        }

        /// <summary>
        /// 返回当前<see cref="DataTable"/>，PageTable 的JSON格式字符串
        /// </summary>
        /// <returns>JSON格式字符串</returns>
        public string PageTableJson()
        {
            return this.PageTable.TableToJson();
        }

        /// <summary>
        /// 返回当前<see cref="DataTable"/>，PageTable 的JSON格式字符串
        /// </summary>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <returns>JSON格式字符串</returns>
        public string PageTableJson(bool IsDate)
        {
            return this.PageTable.TableToJson(IsDate);
        }

        /// <summary>
        /// 返回当前<see cref="DataTable"/>，PageTable 的JSON格式字符串
        /// </summary>
        /// <param name="IsDate">ToJson格式时间，启用转字符串</param>
        /// <param name="ToDateString">Date.ToString()的写法。</param>
        /// <returns>JSON格式字符串</returns>
        public string PageTableJson(bool IsDate, string ToDateString)
        {
            return this.PageTable.TableToJson(IsDate, ToDateString);
        }
    }
}
