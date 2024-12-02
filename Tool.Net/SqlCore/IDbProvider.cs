using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tool.SqlCore
{

    /// <summary>
    /// SQL数据化的模型接口
    /// </summary>
    /// <typeparam name="T">数据类型枚举</typeparam>
    public interface IDbProvider<T> : IDbProvider where T : Enum
    {
        /// <summary>
        /// 根据<see cref="Type"/>类型获取对应的数据库类型，请自行写实现
        /// </summary>
        /// <param name="t"><see cref="Type"/>类型</param>
        /// <returns>类型</returns>
        T ConvertToLocalDbType(Type t);// => throw new Exception("需要使用此方法，请自行写实现，针对于特殊用户数据库类型操作。");

        /// <summary>
        /// 根据<see cref="Type"/>类型获取对应的类型字符串
        /// </summary>
        /// <param name="netType"><see cref="Type"/>类型</param>
        /// <returns>类型字符串</returns>
        string ConvertToLocalDbTypeString(Type netType)
        {
            return ConvertToLocalDbType(netType).ToString();
        }
    }

    /// <summary>
    /// SQL数据化的模型接口
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public interface IDbProvider
    {
        /// <summary>
        /// 读取存储过程参数填充到 <see cref="IDbCommand"/>.Parameters 集合（内置实现采用虚构委托，如在意性能请自行实现，示例：DbCommandBuilder.DeriveParameters(cmd) 每个数据库下面都有对应的实现类。）
        /// </summary>
        /// <param name="cmd">数据库对象</param>
        void DeriveParameters(IDbCommand cmd)
        {
            string qualifiedName = cmd.GetType().AssemblyQualifiedName, typenmae = qualifiedName.Insert(qualifiedName.IndexOf(','), "Builder");

            Utils.ActionDelegate.ActionDispatcher dispatcher = StaticData.DeriveParametersObjs.GetOrAdd(typenmae, AddDeriveParameters);
            dispatcher.VoidExecute(null, cmd);

            static Utils.ActionDelegate.ActionDispatcher AddDeriveParameters(string typenmae)
            {
                try
                {
                    Type builderType = Type.GetType(typenmae);
                    MethodInfo method = builderType.GetMethod("DeriveParameters", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod) ?? throw new ArgumentException("指定的提供程序工厂不支持存储过程函数获取，请自行实现该接口。");
                    return new Utils.ActionDelegate.ActionDispatcher(method);
                }
                catch (Exception ex)
                {
                    throw new Exception("无法通过虚构函数获取到对应的实现方法，请采用手动实现接口！", ex);
                }
            }
        }

        /// <summary>
        /// 获取插入数据的主键ID（SQL）
        /// </summary>
        /// <returns></returns>
        string GetLastIdSql();

        /// <summary>
        /// 获取当前对象的实例
        /// </summary>
        /// <param name="dbProviderType">数据库类型</param>
        /// <param name="dbProviderName">数据库类型定义名称</param>
        /// <returns></returns>
        DbProviderFactory Instance(DbProviderType dbProviderType, string dbProviderName)
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
		bool IsBackupDatabase() => true;

        /// <summary>
        /// 无用，返回true
        /// </summary>
        /// <returns></returns>
		bool IsCompactDatabase() => true;

        /// <summary>
        /// 无用，返回true
        /// </summary>
        /// <returns></returns>
		bool IsDbOptimize() => true;

        /// <summary>
        /// 无用，返回true
        /// </summary>
        /// <returns></returns>
		bool IsFullTextSearchEnabled() => true;

        /// <summary>
        /// 无用，返回true
        /// </summary>
        /// <returns></returns>
		bool IsShrinkData() => true;

        /// <summary>
        /// 无用，返回true
        /// </summary>
        /// <returns></returns>
		bool IsStoreProc() => true;

        ///// <summary>
        ///// 绑定数据
        ///// </summary>
        ///// <param name="paraObj">数据库映射对象</param>
        ///// <param name="paraValue">值</param>
        ///// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
        //void MakeParam(ref DbParameter paraObj, object paraValue, ParameterDirection direction)
        //{
        //    Type paraType = paraValue.GetType();
        //    this.MakeParam(ref paraObj, paraValue, direction, paraType, null);
        //}

        ///// <summary>
        ///// 绑定数据
        ///// </summary>
        ///// <param name="paraObj">数据库映射对象</param>
        ///// <param name="paraValue">值</param>
        ///// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
        ///// <param name="paraType">类型</param>
        ///// <param name="sourceColumn">源列</param>
        //void MakeParam(ref DbParameter paraObj, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn)
        //{
        //    this.MakeParam(ref paraObj, paraValue, direction, paraType, sourceColumn, 0);
        //}

        /// <summary>
        /// 绑定数据，需要用户实现，默认不实现，内置采用系统默认类型，满足基本类型的对应
        /// </summary>
        /// <param name="paraObj">数据库映射对象</param>
        /// <param name="paraValue">值</param>
        /// <param name="direction">指定查询内的有关 <see cref="DataSet"/> 的参数的类型。</param>
        /// <param name="paraType">类型</param>
        /// <param name="sourceColumn">源列</param>
        /// <param name="size">大小</param>
        void GetParam(ref DbParameter paraObj, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn, int size) { /*无需写实现，可被用户实现*/ }

        /// <summary>
        /// 根据键值对生成 Insert 部分语法，和值添加对象
        /// </summary>
        /// <param name="database">数据库引擎</param>
        /// <param name="keyValues">数据集键值对</param>
        /// <param name="key">返回生成的部分SQL语句</param>
        /// <param name="value">返回生成的部分SQL语句</param>
        /// <returns><see cref="List{DbParameter}"/></returns>
        List<DbParameter> GetInsertParams(DbHelper database, IDictionary<string, object> keyValues, out string key, out string value) { key = null; value = null; return null; }

        /// <summary>
        /// 根据键值对生成 Update 部分语法，和值添加对象
        /// </summary>
        /// <param name="database">数据库引擎</param>
        /// <param name="keyValues">数据集键值对</param>
        /// <param name="strsql">返回生成的部分SQL语句</param>
        /// <returns><see cref="List{DbParameter}"/></returns>
        List<DbParameter> GetUpdateParams(DbHelper database, IDictionary<string, object> keyValues, out string strsql) { strsql = null; return null; }

        /// <summary>
		/// 分页核心方法，建议重写，底层默认实现 SqlServer 分页。
		/// </summary>
        /// <param name="dbHelper">数据库引擎</param>
		/// <param name="pramsPager">分页参数</param>
		/// <returns>返回分页对象实体</returns>
	    PagerSet GetPagerSet(DbHelper dbHelper, PagerParameters pramsPager)
        {
            try
            {
                List<DbParameter> list = SetPagerParameters(dbHelper, pramsPager);
                dbHelper.RunProc("WEB_PageView", list, out DataSet pageSet);
                return GetPagerSet(pramsPager, list, pageSet);
            }
            catch (Exception e)
            {
                throw GetException(e);
            }
        }

        /// <summary>
		/// 分页核心方法，建议重写，底层默认实现 SqlServer 分页。
		/// </summary>
        /// <param name="dbHelper"></param>
		/// <param name="pramsPager">分页参数</param>
		/// <returns>返回分页对象实体</returns>
	    async Task<PagerSet> GetPagerSetAsync(DbHelper dbHelper, PagerParameters pramsPager)
        {
            try
            {
                List<DbParameter> list = SetPagerParameters(dbHelper, pramsPager);
                DataSet pageSet = await dbHelper.RunProcDataSetAsync("WEB_PageView", list);
                return GetPagerSet(pramsPager, list, pageSet);
            }
            catch (Exception e)
            {
                throw GetException(e);
            }
        }

        private static List<DbParameter> SetPagerParameters(DbHelper dbHelper, PagerParameters pramsPager)
        {
            if (pramsPager.PageIndex < 0)
            {
                return null;
            }
            List<DbParameter> list = new()
            {
                dbHelper.GetInParam("IsSql", pramsPager.IsSql ? 1 : 0),
                dbHelper.GetInParam("TableName", pramsPager.Table),
                dbHelper.GetInParam("ReturnFields", PagerManager.GetFieldString(pramsPager.Fields, pramsPager.FieldAlias)),
                dbHelper.GetInParam("PageSize", pramsPager.PageSize),
                dbHelper.GetInParam("PageIndex", pramsPager.PageIndex),
                dbHelper.GetInParam("Where", pramsPager.WhereStr),
                dbHelper.GetInParam("Order", pramsPager.PKey),
                dbHelper.GetOutParam("PageCount", typeof(int)),
                dbHelper.GetOutParam("RecordCount", typeof(int))
            };
            return list;
        }

        private static PagerSet GetPagerSet(PagerParameters pramsPager, List<DbParameter> list, DataSet pageSet) 
        {
            return new PagerSet(pramsPager.PageIndex, pramsPager.PageSize, Convert.ToInt32(list[^3].Value), Convert.ToInt32(list[^2].Value), pageSet)
            {
                PageSet =
                {
                    DataSetName = $"PagerSet_{pramsPager.Table}"
                }
            };
        }

        private static Exception GetException(Exception e)
        {
            if (!e.Message.Equals("找不到存储过程 'WEB_PageView'。"))
            {
                return new Exception("调用默认分页存储过程发生异常！", e);
            }

            return new Exception("SqlServer数据库，中不存在分页SQL，其他数据库请忽略，可重新实现。", new Exception(@"
                /*********************************************************************************
                *      Function:  WEB_PageView										             *
                *      Description:                                                              *
                *             Sql2012分页存储过程												 *
                *      Finish DateTime:                                                          *
                *             2019/5/15													     *          
                *********************************************************************************/
                
                IF EXISTS (SELECT * FROM DBO.SYSOBJECTS WHERE ID = OBJECT_ID(N'[dbo].[WEB_PageView]') and OBJECTPROPERTY(ID, N'IsProcedure') = 1)
                DROP PROCEDURE [dbo].[WEB_PageView]
                GO
                
                SET QUOTED_IDENTIFIER ON
                GO
                
                SET ANSI_NULLS ON 
                GO
                
                CREATE PROCEDURE dbo.WEB_PageView
                	@IsSql			INT = 0,				-- 使用表名还是使用SQL执行分页（0：表名，1：SQL）
                	@TableName		NVARCHAR(MAX),			-- 表名
                	@ReturnFields	NVARCHAR(MAX) = '*',	-- 查询列数
                	@PageSize		INT = 10,				-- 每页数目
                	@PageIndex		INT = 1,				-- 当前页码
                	@Where			NVARCHAR(MAX) = '',		-- 查询条件
                	@Order			NVARCHAR(MAX),			-- 排序字段
                	@PageCount		INT OUTPUT,				-- 页码总数
                	@RecordCount	INT OUTPUT	        	-- 记录总数
                WITH ENCRYPTION AS
                
                --设置属性
                SET NOCOUNT ON
                
                -- 变量定义
                DECLARE @TotalRecord INT
                DECLARE @TotalPage INT
                DECLARE @CurrentPageSize INT
                DECLARE @TotalRecordForPageIndex INT
                
                BEGIN
                	IF @Where IS NULL SET @Where=N''
                	
                	-- 记录总数
                	DECLARE @countSql NVARCHAR(4000)  
                	
					IF @IsSql = 1
					BEGIN
                		SET @countSql='SELECT @TotalRecord=Count(*) FROM ('+@TableName+') AS A '+@Where
                		EXECUTE sp_executesql @countSql,N'@TotalRecord int out',@TotalRecord OUT
                	END
					ELSE
                	IF @RecordCount IS NULL
                	BEGIN
                		SET @countSql='SELECT @TotalRecord=Count(*) FROM '+@TableName+'(NOLOCK) '+@Where
                		EXECUTE sp_executesql @countSql,N'@TotalRecord int out',@TotalRecord OUT
                	END
                	ELSE
                	BEGIN
                		SET @TotalRecord=@RecordCount
                	END		
                	
                	SET @RecordCount=@TotalRecord
                	SET @TotalPage=(@TotalRecord-1)/@PageSize+1	
                	SET @CurrentPageSize=(@PageIndex-1)*@PageSize
                
                	-- 返回总页数和总记录数
                	SET @PageCount=@TotalPage
                	SET @RecordCount=@TotalRecord
                	IF @PageCount IS NULL SET @PageCount = 0
                	IF @RecordCount IS NULL SET @RecordCount = 0
                
                	-- 返回记录
                	SET @TotalRecordForPageIndex=@PageIndex*@PageSize
                	
					IF @IsSql = 1
					BEGIN
						EXEC	('SELECT *
                			FROM (SELECT TOP '+@TotalRecordForPageIndex+' '+@ReturnFields+', ROW_NUMBER() OVER ('+@Order+') AS PageView_RowNo
                			FROM ('+@TableName+ ') AS A ' + @Where +' ) AS TempPageViewTable
                			WHERE TempPageViewTable.PageView_RowNo > 
                			'+@CurrentPageSize)
                	END
					ELSE
					BEGIN
                		EXEC	('SELECT *
                			FROM (SELECT TOP '+@TotalRecordForPageIndex+' '+@ReturnFields+', ROW_NUMBER() OVER ('+@Order+') AS PageView_RowNo
                			FROM '+@TableName+ '(NOLOCK) ' + @Where +' ) AS TempPageViewTable
                			WHERE TempPageViewTable.PageView_RowNo > 
                			'+@CurrentPageSize)
                	END	
                	
	                --EXEC   ('SELECT  TOP 10 * 
	                --		  FROM (SELECT '+@ReturnFields+', ROW_NUMBER() OVER ('+@OrderBy+') AS PageView_RowNo 
	                --		  FROM   '+@TableName+ ' (NOLOCK) ' + @Where +' ) AS TempPageViewTable 
	                --		  WHERE PageView_RowNo BETWEEN '+@CurrentPageSize+' + 1 AND '+@TotalRecordForPageIndex)

                END
                RETURN 0
                
                GO"));
        }

        /// <summary>
        /// 参数表示符号 '@' 或 ':'
        /// </summary>
		string ParameterPrefix
        {
            get;
        }
    }
}
