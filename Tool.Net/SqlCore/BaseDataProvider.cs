using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.SqlCore
{
    /// <summary>
    /// 数据库访问底层父类，继承皆可实现高效开发访问
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class BaseDataProvider
    {
        /// <summary>
        /// 链接字符串
        /// </summary>
        protected internal string ConnectionString { get { return Database.ConnectionString; } }

        /// <summary>
        /// 获取当前访问的数据库类型
        /// </summary>
        protected internal DbProviderType DbProviderType { get { return Database.DbProviderType; } }

        /// <summary>
        /// 获取当前访问的数据库类型
        /// </summary>
        protected internal string DbProviderName { get { return Database.DbProviderName; } }

        /// <summary>
        /// 数据底层类
        /// </summary>
        protected internal DbHelper Database { get; }

        /// <summary>
        /// 是否启动SQL日志打印
        /// </summary>
        protected bool IsSqlLog { get { return Database.IsSqlLog; } set { Database.IsSqlLog = value; } }

        ///// <summary>
        ///// 分页类
        ///// </summary>
        //protected internal PagerManager PagerHelper { get; }

        /// <summary>
        /// 初始化 底层 默认为SqlServer 数据库的访问
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProviderName">数据库类型定义名称</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        protected internal BaseDataProvider(string connectionString, string dbProviderName, IDbProvider dbProvider)
        {
            this.Database = new DbHelper(connectionString, dbProviderName, dbProvider);
            //this.PagerHelper = new PagerManager(this.Database);
        }

        /// <summary>
        /// 初始化 底层 
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProviderType">数据库类型</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        protected internal BaseDataProvider(string connectionString, DbProviderType dbProviderType, IDbProvider dbProvider)
        {
            this.Database = new DbHelper(connectionString, dbProviderType, dbProvider);
            //this.PagerHelper = new PagerManager(this.Database);
        }

        /// <summary>
        /// 初始化 底层 默认为SqlServer 数据库的访问
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProviderName">数据库类型定义名称</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        /// <param name="logger">用于打印程序日志，可以为null</param>
        protected internal BaseDataProvider(string connectionString, string dbProviderName, IDbProvider dbProvider, ILogger logger)
        {
            this.Database = new DbHelper(connectionString, dbProviderName, dbProvider, logger);
            //this.PagerHelper = new PagerManager(this.Database);
        }

        /// <summary>
        /// 初始化 底层 
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbProviderType">数据库类型</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        /// <param name="logger">用于打印程序日志，可以为null</param>
        protected internal BaseDataProvider(string connectionString, DbProviderType dbProviderType, IDbProvider dbProvider, ILogger logger)
        {
            this.Database = new DbHelper(connectionString, dbProviderType, dbProvider, logger);
            //this.PagerHelper = new PagerManager(this.Database);
        }

        /// <summary>
        /// 初始化 底层
        /// </summary>
        /// <param name="database">操作对象</param>
        protected internal BaseDataProvider(DbHelper database)
        {
            this.Database = database;
            //this.PagerHelper = new PagerManager(this.Database);
        }

        /// <summary>
        /// 分页，用于SQL对象的分页
        /// </summary>
        /// <param name="prams"></param>
        /// <returns></returns>
        protected virtual PagerSet GetPagerSet(PagerParameters prams)
        {
            return this.Database.Provider.GetPagerSet(this.Database, prams); // this.PagerHelper.GetPagerSet(prams);
        }

        ///// <summary>
        ///// 分页二
        ///// </summary>
        ///// <param name="prams"></param>
        ///// <returns></returns>
        //protected virtual PagerSet GetPagerSet2(PagerParameters prams)
        //{
        //    return this.PagerHelper.GetPagerSet2(prams);
        //}

        /// <summary>
        /// 表信息对象，用于对单张表的处理
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        protected virtual ITableProvider GetTableProvider(string tableName)
        {
            return new TableProvider(this.Database, tableName);
        }


        /// <summary>
        /// 表信息对象，用于对单张表的处理，可以通过自己实现接口来完成效果。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <typeparam name="T">自己实现的单表操作类</typeparam>
        /// <returns></returns>
        protected virtual ITableProvider GetTableProvider<T>(string tableName) where T : ITableProvider
        {
            ITableProvider provider = Activator.CreateInstance<T>();
            provider.Initialize(this.Database, tableName);
            return provider;//new T(this.Database, tableName);
        }
    }

    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DbProviderType : byte
    {
        /// <summary>
        /// 表示未知数据库类型，指特殊指定数据库
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// SqlServer 数据库 （SDK:System.Data.SqlClient）
        /// </summary>
        SqlServer,
        /// <summary>
        /// SqlServer 数据库 （SKD:Microsoft.Data.SqlClient）
        /// </summary>
        SqlServer1,
        /// <summary>
        /// MySql 数据库
        /// </summary>
        MySql,
        /// <summary>
        /// Oracle 数据库
        /// </summary>
        Oracle,
        /// <summary>
        /// SQLite 数据库
        /// </summary>
        SQLite,
        /// <summary>
        /// OleDb 数据库(例如：xls格式的文件)
        /// </summary>
        OleDb,
        ///// <summary>
        ///// ODBC 数据库
        ///// </summary>
        //ODBC,
        ///// <summary>
        ///// Firebird 数据库
        ///// </summary>
        //Firebird,
        ///// <summary>
        ///// PostgreSql 数据库
        ///// </summary>
        //PostgreSql,
        ///// <summary>
        ///// DB2 数据库
        ///// </summary>
        //DB2,
        ///// <summary>
        ///// Informix 数据库
        ///// </summary>
        //Informix,
        ///// <summary>
        ///// SqlServerCe 数据库
        ///// </summary>
        //SqlServerCe
    }
}
