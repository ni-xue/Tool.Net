using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tool.Utils;
using Tool.Utils.Data;
using IsolationLevel = System.Data.IsolationLevel;

namespace Tool.SqlCore
{
    /// <summary>
    /// Sql 核心 操作底层
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DbHelper
    {
        #region Sql 公开变量

        /// <summary>
        /// 在开启数据库日志模式后，将日志打印至该路径下。
        /// </summary>
        public const string LogPath = "Log/Sql/";

        /// <summary>
        /// 链接Sql字符串
        /// </summary>
        protected internal string ConnectionString
        {
            get
            {
                return this.m_connectionstring;
            }
            set
            {
                this.m_connectionstring = value;
            }
        }

        /// <summary>
        /// 获取当前访问的数据库类型
        /// </summary>
        protected internal string DbProviderName => this.m_dbProviderName;

        /// <summary>
        /// 获取当前访问的数据库类型
        /// </summary>
        protected internal DbProviderType DbProviderType => this.m_dbProviderType;

        /// <summary>
        /// 数据工厂对象
        /// </summary>
        public DbProviderFactory Factory
        {
            get
            {
                if (this.m_factory == null)
                {
                    lock (this.lockHelper)
                    {
                        if (this.m_factory == null)
                        {
                            this.m_factory = this.Provider.Instance(this.DbProviderType, this.DbProviderName);
                        }
                        else
                        {
                            return this.m_factory;
                        }
                    }
                }
                return this.m_factory;
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="DbCommand"/>.CommandTimeout 最大等待时长（秒）
        /// </summary>
        public int CommandTimeout { get; set; } = 30;

        /// <summary>
        /// IDB提供商
        /// </summary>
        public IDbProvider Provider => this.m_provider;

        /// <summary>
        /// 是否启动SQL日志打印
        /// </summary>
        public bool IsSqlLog { get; set; } = true;

        /// <summary>
        /// 用于支持打印调试日志，必须在开启IsSqlLog 并 Logger 有值时才能使用。
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// 打印模式，true = 采用HTML打印模式，false = 采用txt打印模式
        /// </summary>
        [Obsolete("因模式维护成本较高，且这种方式显示并不美观，将不在维护！", true)]
        public bool IsSqlLogHtml { get; set; } = false;

        /// <summary>
        /// 打印日志的子路径段
        /// </summary>
        public string SubPath
        {
            get => log_subPath; set
            {
                for (int q = 0; q < value.Length; q++)
                {
                    var length = value.Length - 1;
                    var w = value[q];

                    if (!(((q > 0 || q < length) && w == '.') || 'A' <= w && w <= 'Z' || 'a' <= w && w <= 'z' || '0' <= w && w <= '9'))
                    {
                        throw new FormatException("字符串只能包含英文和数字和点。");
                    }
                    else if ((q == 0 || q == length) && w == '.')
                    {
                        throw new FormatException("开头或结尾处不能出现‘.’");
                    }
                }
                log_subPath = $"/{value}";
            }
        }

        /// <summary>
        /// 查询计数（请求计数超过最大后会重新计数）
        /// </summary>
        public ulong QueryCount { get => m_queryCount; set => m_queryCount = value; }

        #endregion

        #region 公共获取SQL对象部分

        /// <summary>
        /// 返回实现 <see cref="DbCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbCommand"/> 的新实例。</returns>
        public DbCommand CreateCommand() => this.Factory.CreateCommand();

        /// <summary>
        /// 返回实现 <see cref="DbCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns> 的新实例。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DbCommand CreateCommand(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var command = CreateCommand();
            var objs = SetDictionaryParam(prams);
            command.SetDictionary(objs);
            return command;
        }

        /// <summary>
        /// 返回实现 <see cref="DbCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbCommand"/> 实现类</typeparam>
        /// <returns><see cref="DbCommand"/> 的新实例。</returns>
        public T CreateCommand<T>() where T : DbCommand => CreateCommand() as T;

        /// <summary>
        /// 返回实现 <see cref="DbCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns> 的新实例。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T CreateCommand<T>(object prams) where T : DbCommand => CreateCommand(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbConnection"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbConnection"/> 的新实例。</returns>
        public DbConnection CreateConnection() => this.Factory.CreateConnection();

        /// <summary>
        /// 返回实现 <see cref="DbConnection"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbConnection"/> 的新实例。</returns>
        public DbConnection CreateConnection(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var connection = CreateConnection();
            var objs = SetDictionaryParam(prams);
            connection.SetDictionary(objs);
            return connection;
        }

        /// <summary>
        /// 返回实现 <see cref="DbConnection"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbConnection"/> 实现类</typeparam>
        /// <returns><see cref="DbConnection"/> 的新实例。</returns>
        public T CreateConnection<T>() where T : DbConnection => CreateConnection() as T;

        /// <summary>
        /// 返回实现 <see cref="DbConnection"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbConnection"/> 实现类</typeparam>
        /// <returns><see cref="DbConnection"/> 的新实例。</returns>
        public T CreateConnection<T>(object prams) where T : DbConnection => CreateConnection(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbConnectionStringBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbConnectionStringBuilder"/> 的新实例。</returns>
        public DbConnectionStringBuilder CreateConnectionStringBuilder() => this.Factory.CreateConnectionStringBuilder();

        /// <summary>
        /// 返回实现 <see cref="DbConnectionStringBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbConnectionStringBuilder"/> 的新实例。</returns>
        public DbConnectionStringBuilder CreateConnectionStringBuilder(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var connectionStringBuilder = CreateConnectionStringBuilder();
            var objs = SetDictionaryParam(prams);
            connectionStringBuilder.SetDictionary(objs);
            return connectionStringBuilder;
        }

        /// <summary>
        /// 返回实现 <see cref="DbConnectionStringBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbConnectionStringBuilder"/> 实现类</typeparam>
        /// <returns><see cref="DbConnectionStringBuilder"/> 的新实例。</returns>
        public T CreateConnectionStringBuilder<T>() where T : DbConnectionStringBuilder => CreateConnectionStringBuilder() as T;

        /// <summary>
        /// 返回实现 <see cref="DbConnectionStringBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbConnectionStringBuilder"/> 实现类</typeparam>
        /// <returns><see cref="DbConnectionStringBuilder"/> 的新实例。</returns>
        public T CreateConnectionStringBuilder<T>(object prams) where T : DbConnectionStringBuilder => CreateConnectionStringBuilder(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbParameter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbParameter"/> 的新实例。</returns>
        public DbParameter CreateParameter() => this.Factory.CreateParameter();

        /// <summary>
        /// 返回实现 <see cref="DbParameter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbParameter"/> 的新实例。</returns>
        public DbParameter CreateParameter(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var dbParameter = CreateParameter();
            var objs = SetDictionaryParam(prams);
            dbParameter.SetDictionary(objs);
            return dbParameter;
        }

        /// <summary>
        /// 返回实现 <see cref="DbParameter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbParameter"/> 实现类</typeparam>
        /// <returns><see cref="DbParameter"/> 的新实例。</returns>
        public T CreateParameter<T>() where T : DbParameter => CreateParameter() as T;

        /// <summary>
        /// 返回实现 <see cref="DbParameter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbParameter"/> 实现类</typeparam>
        /// <returns><see cref="DbParameter"/> 的新实例。</returns>
        public T CreateParameter<T>(object prams) where T : DbParameter => CreateParameter(prams) as T;

#if NET7_0_OR_GREATER

        /// <summary>
        /// 返回实现 <see cref="DbDataSource"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbDataSource"/> 的新实例。</returns>
        public DbDataSource CreateDataSource() => CreateDataSource(ConnectionString);


        /// <summary>
        /// 返回实现 <see cref="DbDataSource"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbDataSource"/> 的新实例。</returns>
        public DbDataSource CreateDataSource(string ConnectionString) => this.Factory.CreateDataSource(ConnectionString);


        /// <summary>
        /// 返回实现 <see cref="DbDataSource"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbDataSource"/> 实现类</typeparam>
        /// <returns><see cref="DbDataSource"/> 的新实例。</returns>
        public T CreateDataSource<T>() where T : DbDataSource => CreateDataSource() as T;

        /// <summary>
        /// 返回实现 <see cref="DbDataSource"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbDataSource"/> 实现类</typeparam>
        /// <returns><see cref="DbDataSource"/> 的新实例。</returns>
        public T CreateDataSource<T>(string ConnectionString) where T : DbDataSource => CreateDataSource(ConnectionString) as T;

#endif

#if NET6_0_OR_GREATER

        /// <summary>
        /// 返回实现 <see cref="DbBatch"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbBatch"/> 的新实例。</returns>
        public DbBatch CreateBatch()
        {
            if (!this.Factory.CanCreateBatch)
            {
                throw new ArgumentException("指定的提供程序工厂不支持DbBatch。");
            }
            return this.Factory.CreateBatch();
        }

        /// <summary>
        /// 返回实现 <see cref="DbBatch"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbBatch"/> 的新实例。</returns>
        public DbBatch CreateBatch(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var batch = CreateBatch();
            var objs = SetDictionaryParam(prams);
            batch.SetDictionary(objs);
            return batch;
        }

        /// <summary>
        /// 返回实现 <see cref="DbBatch"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbBatch"/> 实现类</typeparam>
        /// <returns><see cref="DbBatch"/> 的新实例。</returns>
        public T CreateBatch<T>() where T : DbBatch => CreateBatch() as T;

        /// <summary>
        /// 返回实现 <see cref="DbBatch"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbBatch"/> 实现类</typeparam>
        /// <returns><see cref="DbBatch"/> 的新实例。</returns>
        public T CreateBatch<T>(object prams) where T : DbBatch => CreateBatch(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbBatchCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbBatchCommand"/> 的新实例。</returns>
        public DbBatchCommand CreateBatchCommand()
        {
            //if (!this.Factory.CanCreateBatchCommand)
            //{
            //    throw new ArgumentException("指定的提供程序工厂不支持DbBatchCommand。");
            //}
            return this.Factory.CreateBatchCommand();
        }

        /// <summary>
        /// 返回实现 <see cref="DbBatchCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbBatchCommand"/> 的新实例。</returns>
        public DbBatchCommand CreateBatchCommand(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var batch = CreateBatchCommand();
            var objs = SetDictionaryParam(prams);
            batch.SetDictionary(objs);
            return batch;
        }

        /// <summary>
        /// 返回实现 <see cref="DbBatchCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbBatchCommand"/> 实现类</typeparam>
        /// <returns><see cref="DbBatchCommand"/> 的新实例。</returns>
        public T CreateBatchCommand<T>() where T : DbBatchCommand => CreateBatchCommand() as T;

        /// <summary>
        /// 返回实现 <see cref="DbBatchCommand"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbBatchCommand"/> 实现类</typeparam>
        /// <returns><see cref="DbBatchCommand"/> 的新实例。</returns>
        public T CreateBatchCommand<T>(object prams) where T : DbBatchCommand => CreateBatchCommand(prams) as T;

#endif

        /// <summary>
        /// 返回实现 <see cref="DbCommandBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbCommandBuilder"/> 的新实例。</returns>
        public DbCommandBuilder CreateCommandBuilder()
        {
            if (!this.Factory.CanCreateCommandBuilder)
            {
                throw new ArgumentException("指定的提供程序工厂不支持DbCommandBuilder。");
            }
            return this.Factory.CreateCommandBuilder();
        }

        /// <summary>
        /// 返回实现 <see cref="DbCommandBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbCommandBuilder"/> 的新实例。</returns>
        public DbCommandBuilder CreateCommandBuilder(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var commandBuilder = CreateCommandBuilder();
            var objs = SetDictionaryParam(prams);
            commandBuilder.SetDictionary(objs);
            return commandBuilder;
        }

        /// <summary>
        /// 返回实现 <see cref="DbCommandBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbCommandBuilder"/> 实现类</typeparam>
        /// <returns><see cref="DbCommandBuilder"/> 的新实例。</returns>
        public T CreateCommandBuilder<T>() where T : DbCommandBuilder => CreateCommandBuilder() as T;

        /// <summary>
        /// 返回实现 <see cref="DbCommandBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbCommandBuilder"/> 实现类</typeparam>
        /// <returns><see cref="DbCommandBuilder"/> 的新实例。</returns>
        public T CreateCommandBuilder<T>(object prams) where T : DbCommandBuilder => CreateCommandBuilder(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbDataAdapter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbDataAdapter"/> 的新实例。</returns>
        public DbDataAdapter CreateDataAdapter()
        {
            if (!this.Factory.CanCreateDataAdapter)
            {
                throw new ArgumentException("指定的提供程序工厂不支持DbDataAdapter。");
            }
            return this.Factory.CreateDataAdapter();
        }

        /// <summary>
        /// 返回实现 <see cref="DbDataAdapter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbDataAdapter"/> 的新实例。</returns>
        public DbDataAdapter CreateDataAdapter(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var dataAdapter = CreateDataAdapter();
            var objs = SetDictionaryParam(prams);
            dataAdapter.SetDictionary(objs);
            return dataAdapter;
        }

        /// <summary>
        /// 返回实现 <see cref="DbDataAdapter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbDataAdapter"/> 实现类</typeparam>
        /// <returns><see cref="DbDataAdapter"/> 的新实例。</returns>
        public T CreateDataAdapter<T>() where T : DbDataAdapter => CreateDataAdapter() as T;

        /// <summary>
        /// 返回实现 <see cref="DbDataAdapter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbDataAdapter"/> 实现类</typeparam>
        /// <returns><see cref="DbDataAdapter"/> 的新实例。</returns>
        public T CreateDataAdapter<T>(object prams) where T : DbDataAdapter => CreateDataAdapter(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbDataSourceEnumerator"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbDataSourceEnumerator"/> 的新实例。</returns>
        public DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            if (!this.Factory.CanCreateDataSourceEnumerator)
            {
                throw new ArgumentException("指定的提供程序工厂不支持DbDataSourceEnumerator。");
            }
            return this.Factory.CreateDataSourceEnumerator();
        }

        /// <summary>
        /// 返回实现 <see cref="DbDataSourceEnumerator"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <returns><see cref="DbDataSourceEnumerator"/> 的新实例。</returns>
        public DbDataSourceEnumerator CreateDataSourceEnumerator(object prams)
        {
            ThrowIfNull(prams, nameof(prams));
            var dataSourceEnumerator = CreateDataSourceEnumerator();
            var objs = SetDictionaryParam(prams);
            dataSourceEnumerator.SetDictionary(objs);
            return dataSourceEnumerator;
        }

        /// <summary>
        /// 返回实现 <see cref="DbDataSourceEnumerator"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbDataSourceEnumerator"/> 实现类</typeparam>
        /// <returns><see cref="DbDataSourceEnumerator"/> 的新实例。</returns>
        public T CreateDataSourceEnumerator<T>() where T : DbDataSourceEnumerator => CreateDataAdapter() as T;

        /// <summary>
        /// 返回实现 <see cref="DbDataSourceEnumerator"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="prams">给对象赋值提供的 字典/对象</param>
        /// <typeparam name="T">指定数据库的 <see cref="DbDataSourceEnumerator"/> 实现类</typeparam>
        /// <returns><see cref="DbDataSourceEnumerator"/> 的新实例。</returns>
        public T CreateDataSourceEnumerator<T>(object prams) where T : DbDataSourceEnumerator => CreateDataAdapter(prams) as T;

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public DbTransaction CreateTransaction()
        {
            DbConnection dbConnection = NewDbConnection();
            dbConnection.Open();
            return dbConnection.BeginTransaction();
        }

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbTransaction"/> 实现类</typeparam>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public T CreateTransaction<T>() where T : DbTransaction => CreateTransaction() as T;

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="isolationLevel">指定的事物类型</param>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public DbTransaction CreateTransaction(IsolationLevel isolationLevel)
        {
            DbConnection dbConnection = NewDbConnection();
            dbConnection.Open();
            return dbConnection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbTransaction"/> 实现类</typeparam>
        /// <param name="isolationLevel">指定的事物类型</param>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public T CreateTransaction<T>(IsolationLevel isolationLevel) where T : DbTransaction => CreateTransaction(isolationLevel) as T;

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public async Task<DbTransaction> CreateTransactionAsync()
        {
            DbConnection dbConnection = NewDbConnection();
            await dbConnection.OpenAsync();
            return await dbConnection.BeginTransactionAsync();
        }

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbTransaction"/> 实现类</typeparam>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public async Task<T> CreateTransactionAsync<T>() where T : DbTransaction => await CreateTransactionAsync() as T;

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <param name="isolationLevel">指定的事物类型</param>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public async Task<DbTransaction> CreateTransactionAsync(IsolationLevel isolationLevel)
        {
            DbConnection dbConnection = NewDbConnection();
            await dbConnection.OpenAsync();
            return await dbConnection.BeginTransactionAsync(isolationLevel);
        }

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbTransaction"/> 实现类</typeparam>
        /// <param name="isolationLevel">指定的事物类型</param>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public async Task<T> CreateTransactionAsync<T>(IsolationLevel isolationLevel) where T : DbTransaction => await CreateTransactionAsync(isolationLevel) as T;

        #endregion

        #region 构造函数部分

        /// <summary>
        /// 有参构造，创建内置包含数据库对象
        /// </summary>
        /// <param name="connString">Sql链接字符串</param>
        /// <param name="dbProviderType">数据库类型</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        public DbHelper(string connString, DbProviderType dbProviderType, IDbProvider dbProvider)
        {
            this.BuildConnection(connString, dbProviderType, dbProviderType.ToString(), dbProvider, null);
        }

        /// <summary>
        /// 有参构造，创建自定义数据库对象
        /// </summary>
        /// <param name="connString">Sql链接字符串</param>
        /// <param name="dbProviderName">数据库类型定义名称</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        public DbHelper(string connString, string dbProviderName, IDbProvider dbProvider)
        {
            if (string.IsNullOrWhiteSpace(dbProviderName))
            {
                throw new Exception("数据库类型定义名称，不能为空。");
            }
            this.BuildConnection(connString, DbProviderType.Unknown, dbProviderName, dbProvider, null);
        }

        /// <summary>
        /// 有参构造，创建内置包含数据库对象
        /// </summary>
        /// <param name="connString">Sql链接字符串</param>
        /// <param name="dbProviderType">数据库类型</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        /// <param name="logger">用于打印程序日志，可以为null</param>
        public DbHelper(string connString, DbProviderType dbProviderType, IDbProvider dbProvider, ILogger logger)
        {
            this.BuildConnection(connString, dbProviderType, dbProviderType.ToString(), dbProvider, logger);
        }

        /// <summary>
        /// 有参构造，创建自定义数据库对象
        /// </summary>
        /// <param name="connString">Sql链接字符串</param>
        /// <param name="dbProviderName">数据库类型定义名称</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        /// <param name="logger">用于打印程序日志，可以为null</param>
        public DbHelper(string connString, string dbProviderName, IDbProvider dbProvider, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(dbProviderName))
            {
                throw new Exception("数据库类型定义名称，不能为空。");
            }
            this.BuildConnection(connString, DbProviderType.Unknown, dbProviderName, dbProvider, logger);
        }

        #endregion

        #region Sql 公共帮助函数

        /// <summary>
        /// 将匿名对象转换成<see cref="DbParameter"/>[]对象集合
        /// </summary>
        /// <param name="parameter">匿名对象</param>
        /// <returns><see cref="DbParameter"/>[]对象集合</returns>
        public List<DbParameter> SetParameterList(object parameter)
        {
            var keys = SetDictionaryParam(parameter, out bool isnull);
            return SetParameterList(keys, isnull);
        }

        /// <summary>
        /// 将匿名对象转换成<see cref="DbParameter"/>[]对象集合
        /// </summary>
        /// <param name="parameter"><see cref="IDictionary{String,Object}"/>对象</param>]
        /// <param name="isnull">是否允许为空</param>
        /// <returns><see cref="DbParameter"/>[]对象集合</returns>
        public List<DbParameter> SetParameterList(IDictionary<string, object> parameter, bool isnull = false)
        {
            if (parameter != null && parameter.Count > 0)
            {
                List<DbParameter> parms = new();

                foreach (var pair in parameter)
                {
                    if (pair.Value != null || isnull)
                        parms.Add(this.GetInParam(pair.Key, pair.Value));
                }

                return parms;
            }
            return null;
        }

        /// <summary>
        /// 将匿名对象转换成<see cref="Dictionary{String,Object}"/>对象集合
        /// </summary>
        /// <param name="parameter">匿名对象</param>
        /// <returns><see cref="Dictionary{String,Object}"/>对象集合</returns>
        public IDictionary<string, object> SetDictionaryParam(object parameter)
        {
            var keys = SetDictionaryParam(parameter, out bool isnull);
            if (keys is not null && !isnull)
            {
                List<string> strings = new();
                foreach (var pair in keys)
                {
                    if (pair.Value == null) strings.Add(pair.Key);
                }
                keys.Remove(strings.ToArray());
            }
            return keys;
        }

        /// <summary>
        /// 将匿名对象转换成<see cref="Dictionary{String,Object}"/>对象集合
        /// </summary>
        /// <param name="parameter">匿名对象</param>
        /// <param name="isnull">解析对象是否是字典，字典将保留空字典值</param>
        /// <returns><see cref="Dictionary{String,Object}"/>对象集合</returns>
        private static IDictionary<string, object> SetDictionaryParam(object parameter, out bool isnull)
        {
            if (parameter != null)
            {
                if (parameter is IDictionary<string, object> dic)
                {
                    isnull = true;
                    return dic;
                }
                if (parameter is IDictionary dictionary)
                {
                    Dictionary<string, object> keyValuePairs = new();
                    foreach (DictionaryEntry keys in dictionary)
                    {
                        keyValuePairs.Add(keys.Key.ToString(), keys.Value);
                    }
                    isnull = true;
                    return keyValuePairs;
                }
                isnull = false;
                return parameter.GetDictionary();

                //Type type = parameter.GetType();
                //PropertyInfo[] _properties = type.GetProperties();
                //if (_properties.Length > 0)
                //{
                //    foreach (PropertyInfo property in _properties)
                //    {
                //        object Value = property.GetValue(parameter);

                //        keyValuePairs.Add(property.Name, Value);
                //    }
                //    return keyValuePairs;
                //}
            }
            isnull = false;
            return default;
        }

        /// <summary>
        /// 将匿名对象转换成<see cref="DbParameter"/>[]对象集合
        /// </summary>
        /// <param name="parameter">匿名对象</param>
        /// <returns><see cref="DbParameter"/>[]对象集合</returns>
        public DbParameter[] SetParameters(object parameter)
        {
            List<DbParameter> dbs = SetParameterList(parameter);
            return dbs?.ToArray();
        }

        /// <summary>
        /// 提供一个通道用于替换日志或关闭日志。
        /// </summary>
        /// <param name="logger">用于打印程序日志，可以为null</param>
        public void SetLogger(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// 获取当前连接字符串或修改
        /// </summary>
        /// <param name="connectionstring">为null时，不修改。</param>
        /// <returns></returns>
        public string GetAndSetConnectionString(string connectionstring = null)
        {
            if (connectionstring != null)
            {
                m_connectionstring = connectionstring;
            }
            return m_connectionstring;
        }

        #endregion

        #region Sql 私有帮助函数

        /// <summary>
        /// 建造Connection 链接Sql对象
        /// </summary>
        /// <param name="connectionString">Sql链接字符串</param>
        /// <param name="dbProviderType">数据库类型</param>
        /// <param name="dbProviderName">数据库类型定义名称</param>
        /// <param name="dbProvider">实现SQL对象类，具有高扩展性</param>
        /// <param name="logger">用于打印程序日志，可以为null</param>
        private void BuildConnection(string connectionString, DbProviderType dbProviderType, string dbProviderName, IDbProvider dbProvider, ILogger logger)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw Throw("请检查数据库连接信息，当前数据库连接信息为空。");
            }

            this.Logger = logger;

            this.m_connectionstring = connectionString;
            this.m_dbProviderType = dbProviderType;
            this.m_dbProviderName = dbProviderName;
            this.m_provider = dbProvider;
            this.m_queryCount = 0;
        }

        private static void AssignParameterValues(DbParameter[] commandParameters, DataRow dataRow)
        {
            if (commandParameters != null && dataRow != null)
            {
                int num = 0;
                for (int i = 0; i < commandParameters.Length; i++)
                {
                    DbParameter dbParameter = commandParameters[i];
                    if (dbParameter.ParameterName == null || dbParameter.ParameterName.Length <= 1)
                    {
                        throw Throw(string.Format("请提供参数{0}一个有效的名称{1}.", num, dbParameter.ParameterName));
                    }
                    if (dataRow.Table.Columns.IndexOf(dbParameter.ParameterName[1..]) != -1)
                    {
                        dbParameter.Value = dataRow[dbParameter.ParameterName[1..]];
                    }
                    num++;
                }
            }
        }

        private static void AssignParameterValues(DbParameter[] commandParameters, object[] parameterValues)
        {
            if (commandParameters != null && parameterValues != null)
            {
                if (commandParameters.Length != parameterValues.Length)
                {
                    throw Throw("参数值个数与参数不匹配。");
                }
                int i = 0;
                int num = commandParameters.Length;
                while (i < num)
                {
                    if (parameterValues[i] is IDbDataParameter dbDataParameter)
                    {
                        if (dbDataParameter.Value == null)
                        {
                            commandParameters[i].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[i].Value = dbDataParameter.Value;
                        }
                    }
                    else if (parameterValues[i] == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = parameterValues[i];
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// 附加参数
        /// </summary>
        /// <param name="command">SQL数据对象基类</param>
        /// <param name="commandParameters">要附加的参数对象数组</param>
        private static void AttachParameters(DbCommand command, DbParameter[] commandParameters)
        {
            ThrowIfNull(command, nameof(command));
            if (commandParameters != null)
            {
                foreach (DbParameter dbParameter in commandParameters)
                {
                    if (dbParameter != null)
                    {
                        if ((dbParameter.Direction == ParameterDirection.InputOutput || dbParameter.Direction == ParameterDirection.Input) && dbParameter.Value == null)
                        {
                            dbParameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(dbParameter);
                    }
                }
                //for (int i = 0; i < commandParameters.Length; i++)
                //{
                //    DbParameter dbParameter = commandParameters[i];
                //    if (dbParameter != null)
                //    {
                //        if ((dbParameter.Direction == ParameterDirection.InputOutput || dbParameter.Direction == ParameterDirection.Input) && dbParameter.Value == null)
                //        {
                //            dbParameter.Value = DBNull.Value;
                //        }
                //        command.Parameters.Add(dbParameter);
                //    }
                //}
            }
        }

        ///// <summary>
        ///// 克隆一个副本
        ///// </summary>
        ///// <param name="originalParameters">源对象数据</param>
        ///// <returns></returns>
        //private static DbParameter[] CloneParameters(DbParameter[] originalParameters)
        //{
        //    DbParameter[] array = new DbParameter[originalParameters.Length];
        //    int i = 0;
        //    int num = originalParameters.Length;
        //    while (i < num)
        //    {
        //        if (originalParameters[i] is ICloneable cloneable)
        //        {
        //            array[i] = cloneable.Clone() as DbParameter;
        //            i++;
        //        }
        //        //array[i] = (DbParameter)((ICloneable)originalParameters[i]).Clone();
        //        //i++;
        //    }
        //    return array;
        //}

        private void IsNullConnectionString() => ThrowIfNullString(ConnectionString, nameof(ConnectionString));

        private static void ThrowIfNull(object argument, string paramName)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(argument, paramName);
#else
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
#endif
        }

        private static void ThrowIfNullString(string isstr, string paramName)
        {
            if (isstr == null || isstr.Length == 0)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ThrowIfNullTransaction(DbTransaction transaction)
        {
            ThrowIfNull(transaction, nameof(transaction));
            if (transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
        }

        #endregion

        #region 公共SQL部分公开函数

        /// <summary>
        /// 添加缓存参数集
        /// </summary>
        /// <param name="commandText">缓存数据集的名称</param>
        /// <param name="commandParameters">缓存的数据集</param>
        public void SetCacheParameterSet(string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            ThrowIfNullString(commandText, nameof(commandText));
            string key = this.ConnectionString + ":" + commandText;
            this.m_paramcache.AddOrUpdate(key, Add, Update);
            DbParameterCache Add(string key) => new(commandParameters);
            DbParameterCache Update(string key, DbParameterCache _) => Add(key);
            //this.m_paramcache[key] = commandParameters;
        }

        /// <summary>
        /// 获取缓存的参数集，每次返回的都是克隆数据
        /// </summary>
        /// <param name="commandText">缓存数据集的名称</param>
        /// <returns></returns>
        public DbParameter[] GetCachedParameterSet(string commandText)
        {
            IsNullConnectionString();
            ThrowIfNullString(commandText, nameof(commandText));
            string key = this.ConnectionString + ":" + commandText;
            if (m_paramcache.TryGetValue(key, out var parameterCache))
            {
                return parameterCache.CloneParameters();
            }
            return null;
            //if (this.m_paramcache[key] is not DbParameter[] array)
            //{
            //    return null;
            //}
            //return CloneParameters(array);
        }

        /// <summary>
        /// 清空缓存的参数集（会清空所有的参数信息）
        /// </summary>
        /// <returns></returns>
        public void EmptyCachedParameterSet()
        {
            IsNullConnectionString();
            this.m_paramcache.Clear();
        }

        /// <summary>
        /// 返回SQL数据对象基类
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="sourceColumns">字符串映射对象</param>
        /// <returns></returns>
        public DbCommand CreateCommand(DbConnection connection, string spName, params string[] sourceColumns)
        {
            ThrowIfNullString(spName, nameof(spName));
            DbCommand dbCommand = CreateCommand();
            dbCommand.CommandTimeout = CommandTimeout;
            dbCommand.CommandText = spName;
            dbCommand.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            dbCommand.CommandType = CommandType.StoredProcedure;
            if (sourceColumns != null && sourceColumns.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                for (int i = 0; i < sourceColumns.Length; i++)
                {
                    spParameterSet[i].SourceColumn = sourceColumns[i];
                }
                AttachParameters(dbCommand, spParameterSet);
            }
            return dbCommand;
        }

        /// <summary>
        /// 返回SQL数据对象基类
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="sourceColumns">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DbCommand> CreateCommandAsync(DbConnection connection, string spName, params string[] sourceColumns)
        {
            ThrowIfNullString(spName, nameof(spName));
            DbCommand dbCommand = CreateCommand();
            dbCommand.CommandTimeout = CommandTimeout;
            dbCommand.CommandText = spName;
            dbCommand.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            dbCommand.CommandType = CommandType.StoredProcedure;
            if (sourceColumns != null && sourceColumns.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                for (int i = 0; i < sourceColumns.Length; i++)
                {
                    spParameterSet[i].SourceColumn = sourceColumns[i];
                }
                AttachParameters(dbCommand, spParameterSet);
            }
            return dbCommand;
        }

        /// <summary>
        /// 获取当前存储过程执行所需要的参数
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含的返回值参数</param>
        /// <returns></returns>
        private DbParameter[] DiscoverSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            using (connection)
            {
                using DbCommand dbCommand = connection.CreateCommand();
                dbCommand.CommandTimeout = CommandTimeout;
                dbCommand.CommandText = spName;
                dbCommand.CommandType = CommandType.StoredProcedure;
                connection.Open();
                this.Provider.DeriveParameters(dbCommand);
                connection.Close();
                if (!includeReturnValueParameter)
                {
                    dbCommand.Parameters.RemoveAt(0);
                }
                DbParameter[] array = new DbParameter[dbCommand.Parameters.Count];
                dbCommand.Parameters.CopyTo(array, 0);
                DbParameter[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    DbParameter dbParameter = array2[i];
                    dbParameter.Value = DBNull.Value;
                }
                return array;
            }
        }

        /// <summary>
        /// 获取当前存储过程执行所需要的参数
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含的返回值参数</param>
        /// <returns></returns>
        private async Task<DbParameter[]> DiscoverSpParameterSetAsync(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            await using (connection)
            {
                await using DbCommand dbCommand = connection.CreateCommand();
                dbCommand.CommandTimeout = CommandTimeout;
                dbCommand.CommandText = spName;
                dbCommand.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();
                this.Provider.DeriveParameters(dbCommand);
                await connection.CloseAsync();
                if (!includeReturnValueParameter)
                {
                    dbCommand.Parameters.RemoveAt(0);
                }
                DbParameter[] array = new DbParameter[dbCommand.Parameters.Count];
                dbCommand.Parameters.CopyTo(array, 0);
                DbParameter[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    DbParameter dbParameter = array2[i];
                    dbParameter.Value = DBNull.Value;
                }
                return array;
            }
        }

        /// <summary>
        /// 回收当前数据核心对象一切资源
        /// </summary>
        public void ResetDbProvider()
        {
            this.m_connectionstring = null;
            this.m_factory = null;
            this.m_provider = null;
            this.m_dbProviderName = null;
            this.m_dbProviderType = DbProviderType.Unknown;
        }

        #endregion

        #region Sql 分页调用接口

        /// <summary>
        /// 分页函数 实现至 IDbProvider 接口 GetPagerSet 方法
        /// </summary>
        /// <param name="pager">相关参数</param>
        /// <returns></returns>
        public PagerSet GetPagerSet(PagerParameters pager)
        {
            return Provider.GetPagerSet(this, pager);
        }

        /// <summary>
        /// 分页函数 实现至 IDbProvider 接口 GetPagerSet 方法
        /// </summary>
        /// <param name="pager">相关参数</param>
        /// <returns></returns>
        public Task<PagerSet> GetPagerSetAsync(PagerParameters pager)
        {
            return Provider.GetPagerSetAsync(this, pager);
        }

        #endregion

        #region Sql 公共查询函数 返回（DataSet or 实体） Query

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回数据集合</returns>
        public DataSet Query(string commandText)
        {
            return Query(commandText, null);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">SQL参数</param>
        /// <returns>返回数据集合</returns>
        public DataSet Query(string commandText, object parameter)
        {
            DbParameter[] commandParameters = SetParameters(parameter);
            return ExecuteDataSet(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回一个实体</returns>
        public T Query<T>(string commandText) where T : new()
        {
            return Query<T>(commandText, null);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">SQL参数</param>
        /// <returns>返回一个实体</returns>
        public T Query<T>(string commandText, object parameter) where T : new()
        {
            DataSet data = Query(commandText, parameter);

            if (Validate.CheckedDataSet(data))
            {
                return data.Tables[0].Rows[0].ToEntity<T>();
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回一个实体数组</returns>
        public IList<T> QueryList<T>(string commandText) where T : new()
        {
            return QueryList<T>(commandText, null);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">SQL参数</param>
        /// <returns>返回一个实体数组</returns>
        public IList<T> QueryList<T>(string commandText, object parameter) where T : new()
        {
            DataSet data = Query(commandText, parameter);

            if (Validate.CheckedDataSet(data))
            {
                return data.Tables[0].ToEntityList<T>();
            }
            else
            {
                return default;
            }
        }

        #endregion

        #region Sql 公共查询函数 返回（DataSet or 实体） QueryAsync

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回数据集合</returns>
        public Task<DataSet> QueryAsync(string commandText)
        {
            return QueryAsync(commandText, null);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">SQL参数</param>
        /// <returns>返回数据集合</returns>
        public Task<DataSet> QueryAsync(string commandText, object parameter)
        {
            DbParameter[] commandParameters = SetParameters(parameter);
            return ExecuteDataSetAsync(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回一个实体</returns>
        public Task<T> QueryAsync<T>(string commandText) where T : new()
        {
            return QueryAsync<T>(commandText, null);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">SQL参数</param>
        /// <returns>返回一个实体</returns>
        public async Task<T> QueryAsync<T>(string commandText, object parameter) where T : new()
        {
            DataSet data = await QueryAsync(commandText, parameter);

            if (Validate.CheckedDataSet(data))
            {
                return data.Tables[0].Rows[0].ToEntity<T>();
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回一个实体数组</returns>
        public Task<IList<T>> QueryListAsync<T>(string commandText) where T : new()
        {
            return QueryListAsync<T>(commandText, null);
        }

        /// <summary>
        /// 根据SQL语句，查询返回查询结果。
        /// </summary>
        /// <typeparam name="T">转换的实体</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameter">SQL参数</param>
        /// <returns>返回一个实体数组</returns>
        public async Task<IList<T>> QueryListAsync<T>(string commandText, object parameter) where T : new()
        {
            DataSet data = await QueryAsync(commandText, parameter);

            if (Validate.CheckedDataSet(data))
            {
                return data.Tables[0].ToEntityList<T>();
            }
            else
            {
                return default;
            }
        }

        #endregion

        #region Sql 公共执行返回查询结果 采用转实体方式 ExecuteObject

        /// <summary>
        /// 执行查询SQL返回查询结果，转换为对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public T ExecuteObject<T>(string commandText)
        {
            DataSet dataSet = this.ExecuteDataSet(commandText);
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果，转换为对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示字符串映射参数</param>
        /// <returns></returns>
        public T ExecuteObject<T>(string commandText, object prams)
        {
            DataSet dataSet = this.ExecuteDataSet(CommandType.Text, commandText, SetParameters(prams));
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果，转换为对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public T ExecuteObject<T>(string commandText, List<DbParameter> prams)
        {
            DataSet dataSet = this.ExecuteDataSet(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果集合，转换为<see cref="IList{T}"/>对象集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public IList<T> ExecuteObjectList<T>(string commandText)
        {
            DataSet dataSet = this.ExecuteDataSet(commandText);
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果集合，转换为<see cref="IList{T}"/>对象集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示字符串映射参数</param>
        /// <returns></returns>
        public IList<T> ExecuteObjectList<T>(string commandText, object prams)
        {
            DataSet dataSet = this.ExecuteDataSet(CommandType.Text, commandText, SetParameters(prams));
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果集合，转换为<see cref="IList{T}"/>对象集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示 <see cref="List{DbCommand}"/> 的参数</param>
        /// <returns></returns>
        public IList<T> ExecuteObjectList<T>(string commandText, List<DbParameter> prams)
        {
            DataSet dataSet = this.ExecuteDataSet(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        #endregion

        #region Sql 公共执行返回查询结果 采用转实体方式 ExecuteObjectAsync

        /// <summary>
        /// 执行查询SQL返回查询结果，转换为对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public async Task<T> ExecuteObjectAsync<T>(string commandText)
        {
            DataSet dataSet = await this.ExecuteDataSetAsync(commandText);
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果，转换为对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示字符串映射参数</param>
        /// <returns></returns>
        public async Task<T> ExecuteObjectAsync<T>(string commandText, object prams)
        {
            DataSet dataSet = await this.ExecuteDataSetAsync(CommandType.Text, commandText, SetParameters(prams));
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果，转换为对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public async Task<T> ExecuteObjectAsync<T>(string commandText, List<DbParameter> prams)
        {
            DataSet dataSet = await this.ExecuteDataSetAsync(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果集合，转换为<see cref="IList{T}"/>对象集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public async Task<IList<T>> ExecuteObjectListAsync<T>(string commandText)
        {
            DataSet dataSet = await this.ExecuteDataSetAsync(commandText);
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果集合，转换为<see cref="IList{T}"/>对象集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示字符串映射参数</param>
        /// <returns></returns>
        public async Task<IList<T>> ExecuteObjectListAsync<T>(string commandText, object prams)
        {
            DataSet dataSet = await this.ExecuteDataSetAsync(CommandType.Text, commandText, SetParameters(prams));
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 执行查询SQL返回查询结果集合，转换为<see cref="IList{T}"/>对象集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">表示 <see cref="List{DbCommand}"/> 的参数</param>
        /// <returns></returns>
        public async Task<IList<T>> ExecuteObjectListAsync<T>(string commandText, List<DbParameter> prams)
        {
            DataSet dataSet = await this.ExecuteDataSetAsync(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        #endregion

        #region Sql 公共查询函数 返回（DataSet） ExecuteDataSet

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string commandText)
        {
            return this.ExecuteDataSet(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string commandText, object prams)
        {
            return this.ExecuteDataSet(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            return this.ExecuteDataSet(commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, object prams)
        {
            return ExecuteDataSet(commandType, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="commandParameters">参数对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            return this.ExecuteDataSet(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteDataSet(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteDataSet(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataSet(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteDataSet(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteDataSet(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteDataSet(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataSet(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteDataSet(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private DataSet ExecuteDataSet(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            using DbCommand dbCommand = CreateCommand();

            Stopwatch watch = GetStopwatch();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out flag);
                using DbDataAdapter dbDataAdapter = CreateDataAdapter();
                dbDataAdapter.SelectCommand = dbCommand;
                DataSet dataSet = new();
                dbDataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) connection.Close();
            }
        }

        #endregion

        #region Sql 公共查询函数 返回（DataSet） ExecuteDataSetAsync (因DataAdapter无异步模式，所有这里更换支持异步的DataReader)

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(string commandText)
        {
            return this.ExecuteDataSetAsync(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(string commandText, object prams)
        {
            return this.ExecuteDataSetAsync(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText)
        {
            return this.ExecuteDataSetAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText, object prams)
        {
            return ExecuteDataSetAsync(commandType, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="commandParameters">参数对象</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            await using DbConnection dbConnection = NewDbConnection();
            //await dbConnection.OpenAsync();
            return await this.ExecuteDataSetAsync(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteDataSetAsync(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteDataSetAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteDataSetAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteDataSetAsync(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteDataSetAsync(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteDataSetAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteDataSetAsync(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDataSetAsync(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteDataSetAsync(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private async Task<DataSet> ExecuteDataSetAsync(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters)
        {
            var watch = GetStopwatch();
            if (transaction is null && connection.State is not ConnectionState.Open) await connection.OpenAsync();
            using var reader = await this.ExecuteReaderAsync(connection, transaction, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.External, watch: watch);
            var dataSet = await reader.GetDataSetAsync();
            return dataSet;
        }

        #endregion

        #region Sql 公共查询存储过程 返回（DataSet） ExecuteDataSetTypedParams

        /// <summary>
        /// 根据SQL获取<see cref="DataSet"/>
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数</param>
        /// <returns></returns>
        public DataSet ExecuteDatasetTypedParams(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataSet(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataSet(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL获取<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数</param>
        /// <returns></returns>
        public DataSet ExecuteDatasetTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataSet(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataSet(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL获取<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数</param>
        /// <returns></returns>
        public DataSet ExecuteDatasetTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataSet(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataSet(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共查询存储过程 返回（DataSet） ExecuteDataSetTypedParamsAsync

        /// <summary>
        /// 根据SQL获取<see cref="DataSet"/>
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDatasetTypedParamsAsync(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteDataSetAsync(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteDataSetAsync(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL获取<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDatasetTypedParamsAsync(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteDataSetAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteDataSetAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL获取<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDatasetTypedParamsAsync(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteDataSetAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteDataSetAsync(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回受影响行数 返回（int） ExecuteNonQuery

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">字符串映射对象</param>
        /// <returns>受影响行数</returns>
        public int ExecuteNonQuery(string commandText, object prams)
        {
            return this.ExecuteNonQuery(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, object prams)
        {
            return this.ExecuteNonQuery(commandType, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            return this.ExecuteNonQuery(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteNonQuery(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private int ExecuteNonQuery(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            using DbCommand dbCommand = CreateCommand();

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out flag);
                return dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) connection.Close();
            }
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns>返回受影响行数</returns>
        public int ExecuteNonQuery(DbTransaction transaction, params SqlTextParameter[] sqlTexts)
        {
            ThrowIfNullTransaction(transaction);
            if (sqlTexts == null)
            {
                throw new ArgumentNullException(nameof(sqlTexts), "多条执行SQL对象为空。");
            }
            int result = 0;
            using DbCommand dbCommand = CreateCommand();

            foreach (SqlTextParameter sqlText in sqlTexts)
            {
                Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
                bool iserror = false;
                string guid = string.Empty;
                try
                {
                    PrepareCommand(dbCommand, CommandTimeout, transaction.Connection, transaction, sqlText.CommandType, sqlText.CommandText, sqlText.Parameters, out _);
                    result += dbCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw GetException(ex, ref iserror, ref guid);
                }
                finally
                {
                    AddQueryDetail(dbCommand.CommandText, watch, sqlText.Parameters, iserror, guid);
                    dbCommand.Parameters.Clear();
                }
            }
            return result;
        }

        #endregion

        #region Sql 公共执行返回受影响行数 返回（Task<int>） ExecuteNonQueryAsync

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(string commandText)
        {
            return this.ExecuteNonQueryAsync(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">字符串映射对象</param>
        /// <returns>受影响行数</returns>
        public Task<int> ExecuteNonQueryAsync(string commandText, object prams)
        {
            return this.ExecuteNonQueryAsync(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText)
        {
            return this.ExecuteNonQueryAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">字符串映射对象</param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, object prams)
        {
            return this.ExecuteNonQueryAsync(commandType, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            await using DbConnection dbConnection = NewDbConnection();
            //await dbConnection.OpenAsync();
            return await this.ExecuteNonQueryAsync(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQueryAsync(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQueryAsync(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteNonQueryAsync(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private async Task<int> ExecuteNonQueryAsync(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            using DbCommand dbCommand = CreateCommand();

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                flag = await OpenCommandAsync(connection);
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out _);
                return await dbCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> ExecuteNonQueryAsync(DbTransaction transaction, params SqlTextParameter[] sqlTexts)
        {
            ThrowIfNullTransaction(transaction);
            if (sqlTexts == null)
            {
                throw new ArgumentNullException(nameof(sqlTexts), "多条执行SQL对象为空。");
            }
            int result = 0;
            using DbCommand dbCommand = CreateCommand();

            foreach (SqlTextParameter sqlText in sqlTexts)
            {
                Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
                bool iserror = false;
                string guid = string.Empty;
                try
                {
                    await OpenCommandAsync(transaction.Connection);
                    PrepareCommand(dbCommand, CommandTimeout, transaction.Connection, transaction, sqlText.CommandType, sqlText.CommandText, sqlText.Parameters, out _);
                    result += dbCommand.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw GetException(ex, ref iserror, ref guid);
                }
                finally
                {
                    AddQueryDetail(dbCommand.CommandText, watch, sqlText.Parameters, iserror, guid);
                    dbCommand.Parameters.Clear();
                }
            }
            return result;
        }

        #endregion

        #region Sql 公共执行返回受影响行数 内置事物版本 TransactionExecuteNonQuery -- 以优化

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType">执行类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public DbTransResult TransExecuteNonQuery(CommandType commandType, string commandText, object commandParameters)
        {
            return TransExecuteNonQuery(commandType, commandText, SetParameters(commandParameters));
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType">执行类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public DbTransResult TransExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return TransExecuteNonQuery(new SqlTextParameter(commandType, commandText, commandParameters));
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns></returns>
        public DbTransResult TransExecuteNonQuery(params SqlTextParameter[] sqlTexts)
        {
            IsNullConnectionString();
            using DbConnection dbConnection = NewDbConnection();
            dbConnection.Open();

            DbTransaction transaction = dbConnection.BeginTransaction();

            return transaction.ExecuteNonQuery(this, sqlTexts);
        }

        #endregion

        #region Sql 公共执行返回受影响行数 内置事物版本 TransactionExecuteNonQueryAsync -- 以优化

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType">执行类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public Task<DbTransResult> TransExecuteNonQueryAsync(CommandType commandType, string commandText, object commandParameters)
        {
            return TransExecuteNonQueryAsync(commandType, commandText, SetParameters(commandParameters));
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType">执行类型</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public Task<DbTransResult> TransExecuteNonQueryAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return TransExecuteNonQueryAsync(new SqlTextParameter(commandType, commandText, commandParameters));
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns></returns>
        public async Task<DbTransResult> TransExecuteNonQueryAsync(params SqlTextParameter[] sqlTexts)
        {
            IsNullConnectionString();
            await using DbConnection dbConnection = NewDbConnection();
            await dbConnection.OpenAsync();

            DbTransaction transaction = await dbConnection.BeginTransactionAsync();

            return await transaction.ExecuteNonQueryAsync(this, sqlTexts);
        }

        #endregion

        #region Sql 公共执行返回受影响行数 用于新增可返回插入的主键ID ExecuteNonQuery

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(out id, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            using DbConnection dbConnection = NewDbConnection();
            return this.ExecuteNonQuery(out id, dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, string commandText)
        {
            return this.ExecuteNonQuery(out id, CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(out id, connection, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(out id, transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(out id, connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="id">返回第一行第一列的id</param>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out object id, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteNonQuery(out id, transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private int ExecuteNonQuery(out object id, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            if (this.Provider.GetLastIdSql().Trim() == "")
            {
                throw new ArgumentNullException("GetLastIdSql is \"\"");
            }
            using DbCommand dbCommand = CreateCommand();
            commandText = $"{commandText} {this.Provider.GetLastIdSql()} AS ID";//, @@rowcount AS Count 

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out flag);
                using var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                reader.Read();
                int result = reader.RecordsAffected;
                id = reader.GetValue(0);
                reader.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) connection.Close();
            }
        }

        #endregion

        #region Sql 公共执行返回受影响行数 用于新增可返回插入的主键ID ExecuteNonQueryIdAsync

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>返回第一行第一列的id</returns>
        public Task<SqlNonQuery> ExecuteNonQueryIdAsync(CommandType commandType, string commandText)
        {
            return this.ExecuteNonQueryIdAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns>返回第一行第一列的id</returns>
        public async Task<SqlNonQuery> ExecuteNonQueryIdAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            await using DbConnection dbConnection = NewDbConnection();
            //await dbConnection.OpenAsync();
            return await this.ExecuteNonQueryIdAsync(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>返回第一行第一列的id</returns>
        public Task<SqlNonQuery> ExecuteNonQueryIdAsync(string commandText)
        {
            return this.ExecuteNonQueryIdAsync(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>返回第一行第一列的id</returns>
        public Task<SqlNonQuery> ExecuteNonQueryIdAsync(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQueryIdAsync(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>返回第一行第一列的id</returns>
        public Task<SqlNonQuery> ExecuteNonQueryIdAsync(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQueryIdAsync(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns>返回第一行第一列的id</returns>
        public Task<SqlNonQuery> ExecuteNonQueryIdAsync(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQueryIdAsync(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns>返回第一行第一列的id</returns>
        public Task<SqlNonQuery> ExecuteNonQueryIdAsync(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteNonQueryIdAsync(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private async Task<SqlNonQuery> ExecuteNonQueryIdAsync(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            if (this.Provider.GetLastIdSql().Trim() == "")
            {
                throw new ArgumentNullException("GetLastIdSql is \"\"");
            }
            using DbCommand dbCommand = CreateCommand();
            commandText = $"{commandText} {this.Provider.GetLastIdSql()} AS ID";//, @@rowcount AS Count 

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                flag = await OpenCommandAsync(connection);
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out _);

                using var reader = await dbCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                await reader.ReadAsync();
                var nonQuery = new SqlNonQuery { RowsCount = reader.RecordsAffected, Id = reader.GetValue(0) };
                await reader.CloseAsync();
                return nonQuery;
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) await connection.CloseAsync();
            }
        }


        #endregion

        #region Sql 公共执行返回受影响行数 调用存储过程 ExecuteNonQueryTypedParams

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public int ExecuteNonQueryTypedParams(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public int ExecuteNonQueryTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public int ExecuteNonQueryTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回受影响行数 调用存储过程 ExecuteNonQueryTypedParamsAsync

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryTypedParamsAsync(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteNonQueryAsync(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteNonQueryAsync(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryTypedParamsAsync(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteNonQueryAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteNonQueryAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryTypedParamsAsync(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（DbDataReader）ExecuteReader

        /// <summary>
        /// 执行SQL返回数据流
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return this.ExecuteReader(commandType, commandText, null);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string spName, params object[] parameterValues)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteReader(this.ConnectionString, new object[]
                {
                    CommandType.StoredProcedure,
                    spName,
                    spParameterSet
                });
            }
            return this.ExecuteReader(this.ConnectionString, new object[]
            {
                CommandType.StoredProcedure,
                spName
            });
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            DbConnection dbConnection = null;
            DbDataReader result;
            try
            {
                dbConnection = NewDbConnection();
                var watch = GetStopwatch();
                dbConnection.Open();
                result = this.ExecuteReader(dbConnection, null, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.Internal, watch: watch);
            }
            catch
            {
                dbConnection?.Close();
                throw;
            }
            return result;
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteReader(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteReader(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteReader(connection, null, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.External);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return this.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.External);
        }

        private DbDataReader ExecuteReader(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, DbHelper.DbConnectionOwnership connectionOwnership, Stopwatch watch = null)
        {
            ThrowIfNull(connection, nameof(connection));
            using DbCommand dbCommand = CreateCommand();

            watch ??= GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false, flag1 = true;
            string guid = string.Empty;
            try
            {
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out flag);

                DbDataReader dbDataReader = connectionOwnership == DbHelper.DbConnectionOwnership.External
                    ? dbCommand.ExecuteReader()
                    : dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

                foreach (DbParameter dbParameter in dbCommand.Parameters)
                {
                    if (dbParameter.Direction != ParameterDirection.Input)
                    {
                        flag1 = false;
                        break;
                    }
                }

                return dbDataReader;
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                if (flag1) dbCommand.Parameters.Clear();
                if (flag) connection.Close();
            }
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（DbDataReader）ExecuteReaderAsync

        /// <summary>
        /// 执行SQL返回数据流
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string commandText)
        {
            return this.ExecuteReaderAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderAsync(string spName, params object[] parameterValues)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteReaderAsync(this.ConnectionString, new object[]
                {
                    CommandType.StoredProcedure,
                    spName,
                    spParameterSet
                });
            }
            return await this.ExecuteReaderAsync(this.ConnectionString, new object[]
            {
                CommandType.StoredProcedure,
                spName
            });
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            DbConnection dbConnection = null;
            DbDataReader result;
            try
            {
                dbConnection = NewDbConnection();
                var watch = GetStopwatch();
                await dbConnection.OpenAsync();
                result = await this.ExecuteReaderAsync(dbConnection, null, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.Internal, watch: watch);
            }
            catch
            {
                await dbConnection?.CloseAsync();
                throw;
            }
            return result;
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<DbDataReader> ExecuteReaderAsync(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteReaderAsync(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderAsync(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<DbDataReader> ExecuteReaderAsync(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteReaderAsync(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderAsync(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<DbDataReader> ExecuteReaderAsync(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteReaderAsync(connection, null, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.External);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<DbDataReader> ExecuteReaderAsync(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return this.ExecuteReaderAsync(transaction.Connection, transaction, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.External);
        }

        private async Task<DbDataReader> ExecuteReaderAsync(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, DbHelper.DbConnectionOwnership connectionOwnership, Stopwatch watch = null)
        {
            ThrowIfNull(connection, nameof(connection));
            using DbCommand dbCommand = CreateCommand();

            watch ??= GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false, flag1 = true;
            string guid = string.Empty;
            try
            {
                flag = await OpenCommandAsync(connection);
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out _);

                DbDataReader dbDataReader = connectionOwnership == DbHelper.DbConnectionOwnership.External
                    ? await dbCommand.ExecuteReaderAsync()
                    : await dbCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                foreach (DbParameter dbParameter in dbCommand.Parameters)
                {
                    if (dbParameter.Direction != ParameterDirection.Input)
                    {
                        flag1 = false;
                        break;
                    }
                }

                return dbDataReader;
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                if (flag1) dbCommand.Parameters.Clear();
                if (flag && transaction is null) await connection.CloseAsync();
            }
        }

        #endregion

        #region Sql 公共执行返回查询结果 调用存储过程 返回（DbDataReader）ExecuteReaderTypedParams

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
		public DbDataReader ExecuteReaderTypedParams(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(this.ConnectionString, new object[]
                {
                    CommandType.StoredProcedure,
                    spName,
                    spParameterSet
                });
            }
            return this.ExecuteReader(this.ConnectionString, new object[]
            {
                CommandType.StoredProcedure,
                spName
            });
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReaderTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public DbDataReader ExecuteReaderTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回查询结果 调用存储过程 返回（DbDataReader）ExecuteReaderTypedParamsAsync

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderTypedParamsAsync(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteReaderAsync(this.ConnectionString, new object[]
                {
                    CommandType.StoredProcedure,
                    spName,
                    spParameterSet
                });
            }
            return await this.ExecuteReaderAsync(this.ConnectionString, new object[]
            {
                CommandType.StoredProcedure,
                spName
            });
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderTypedParamsAsync(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行存储过程返回数据流
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<DbDataReader> ExecuteReaderTypedParamsAsync(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（object）ExecuteScalar

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(commandType, commandText, null);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            return this.ExecuteScalar(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public object ExecuteScalar(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public object ExecuteScalar(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
		public object ExecuteScalar(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public object ExecuteScalar(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteScalar(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteScalar(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private object ExecuteScalar(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            using DbCommand dbCommand = CreateCommand();

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out flag);
                return dbCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) connection.Close();
            }
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（object）ExecuteScalarAsync

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(CommandType commandType, string commandText)
        {
            return this.ExecuteScalarAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            return await this.ExecuteScalarAsync(dbConnection, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteScalarAsync(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(DbConnection connection, string spName, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteScalarAsync(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
		public async Task<object> ExecuteScalarAsync(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return await this.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteScalarAsync(connection, null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 返回一个数据对象
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNullTransaction(transaction);
            return ExecuteScalarAsync(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        private async Task<object> ExecuteScalarAsync(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            using DbCommand dbCommand = CreateCommand();

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                flag = await OpenCommandAsync(connection);
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out _);
                return await dbCommand.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) await connection.CloseAsync();
            }
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（string）ExecuteScalarToStr

        /// <summary>
        /// 执行SQL返回string类型的数据
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public string ExecuteScalarToStr(CommandType commandType, string commandText)
        {
            object obj = this.ExecuteScalar(commandType, commandText);
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }

        /// <summary>
        /// 执行SQL返回string类型的数据
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public string ExecuteScalarToStr(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            object obj = this.ExecuteScalar(commandType, commandText, commandParameters);
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（string）ExecuteScalarToStrAsync

        /// <summary>
        /// 执行SQL返回string类型的数据
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public async Task<string> ExecuteScalarToStrAsync(CommandType commandType, string commandText)
        {
            object obj = await this.ExecuteScalarAsync(commandType, commandText);
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }

        /// <summary>
        /// 执行SQL返回string类型的数据
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public async Task<string> ExecuteScalarToStrAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            object obj = await this.ExecuteScalarAsync(commandType, commandText, commandParameters);
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }

        #endregion

        #region Sql 公共执行返回查询结果 调用存储过程 返回（object）ExecuteScalarTypedParams

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public object ExecuteScalarTypedParams(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public object ExecuteScalarTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public object ExecuteScalarTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回查询结果 调用存储过程 返回（object）ExecuteScalarTypedParamsAsync

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarTypedParamsAsync(string spName, DataRow dataRow)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteScalarAsync(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteScalarAsync(CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarTypedParamsAsync(DbConnection connection, string spName, DataRow dataRow)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarTypedParamsAsync(DbTransaction transaction, string spName, DataRow dataRow)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNullString(spName, nameof(spName));
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = await this.GetSpParameterSetAsync(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return await this.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return await this.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（void）但是带入（DataSet）有返回 FillDataSet

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames">对应的表名称</param>
        public void FillDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            IsNullConnectionString();
            ThrowIfNull(dataSet, nameof(dataSet));
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            this.FillDataSet(dbConnection, commandType, commandText, dataSet, tableNames);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        public void FillDataSet(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            IsNullConnectionString();
            ThrowIfNull(dataSet, nameof(dataSet));
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            this.FillDataSet(dbConnection, spName, dataSet, tableNames, parameterValues);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="commandParameters">字符串映射对象</param>
        public void FillDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            IsNullConnectionString();
            ThrowIfNull(dataSet, nameof(dataSet));
            using DbConnection dbConnection = NewDbConnection();
            //dbConnection.Open();
            this.FillDataSet(dbConnection, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        public void FillDataSet(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            this.FillDataSet(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        public void FillDataSet(DbConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNull(dataSet, nameof(dataSet));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                this.FillDataSet(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
                return;
            }
            this.FillDataSet(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        public void FillDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            this.FillDataSet(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="parameterValues">字符串映射对象</param>
		public void FillDataSet(DbTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            ThrowIfNullTransaction(transaction);
            ThrowIfNull(dataSet, nameof(dataSet));
            ThrowIfNullString(spName, nameof(spName));
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                this.FillDataSet(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
                return;
            }
            this.FillDataSet(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="commandParameters">字符串映射对象</param>
        public void FillDataSet(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            this.FillDataSet(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="commandParameters">字符串映射对象</param>
        public void FillDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            this.FillDataSet(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        private void FillDataSet(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNull(dataSet, nameof(dataSet));
            using DbCommand dbCommand = CreateCommand();

            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            bool iserror = false, flag = false;
            string guid = string.Empty;
            try
            {
                PrepareCommand(dbCommand, CommandTimeout, connection, transaction, commandType, commandText, commandParameters, out flag);
                using DbDataAdapter dbDataAdapter = CreateDataAdapter();
                dbDataAdapter.SelectCommand = dbCommand;
                if (tableNames != null && tableNames.Length > 0)
                {
                    string text = "Table";
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        if (tableNames[i] == null || tableNames[i].Length == 0)
                        {
                            throw new ArgumentException("tableNames参数必须包含表的列表，如果提供的值为null或空字符串。", nameof(tableNames));
                        }
                        dbDataAdapter.TableMappings.Add(text, tableNames[i]);
                        text += (i + 1).ToString();
                    }
                }
                dbDataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                throw GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters, iserror, guid);
                dbCommand.Parameters.Clear();
                if (flag && transaction is null) connection.Close();
            }
        }

        #endregion

        #region 执行存储过程部分 返回（int or 实体） RunProc

        /// <summary>
        /// 执行存储过程返回受影响行数
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
        public int RunProc(string procName)
        {
            return this.ExecuteNonQuery(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="reader">返回 <see cref="DbDataReader"/> 对象</param>
        public void RunProc(string procName, out DbDataReader reader)
        {
            reader = this.ExecuteReader(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="ds">返回 <see cref="DataSet"/> 对象</param>
        public void RunProc(string procName, out DataSet ds)
        {
            ds = this.ExecuteDataSet(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="obj">返回 <see cref="object"/> 对象</param>
        public void RunProc(string procName, out object obj)
        {
            obj = this.ExecuteScalar(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public int RunProc(string procName, List<DbParameter> prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteNonQuery(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="DbParameter"/>[] 的参数</param>
        /// <returns></returns>
        public int RunProc(string procName, DbParameter[] prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteNonQuery(CommandType.StoredProcedure, procName, prams);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <param name="reader">返回 <see cref="DbDataReader"/> 对象</param>
        public void RunProc(string procName, List<DbParameter> prams, out DbDataReader reader)
        {
            prams.Add(this.GetReturnParam());
            reader = this.ExecuteReader(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 根据存储过程，返回<see cref="DataSet"/>数据
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <param name="ds">返回<see cref="DataSet"/>数据</param>
        public void RunProc(string procName, List<DbParameter> prams, out DataSet ds)
        {
            prams.Add(this.GetReturnParam());
            ds = this.ExecuteDataSet(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <param name="obj">返回 <see cref="object"/> 对象</param>
        public void RunProc(string procName, List<DbParameter> prams, out object obj)
        {
            prams.Add(this.GetReturnParam());
            obj = this.ExecuteScalar(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
        public T RunProcObject<T>(string procName)
        {
            this.RunProc(procName, out DataSet dataSet);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public T RunProcObject<T>(string procName, List<DbParameter> prams)
        {
            this.RunProc(procName, prams, out DataSet dataSet);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行存储过程返回结果 <see cref="IList{T}"/>数据集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
        public IList<T> RunProcObjectList<T>(string procName)
        {
            this.RunProc(procName, out DataSet dataSet);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 执行存储过程返回结果 <see cref="IList{T}"/>数据集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public IList<T> RunProcObjectList<T>(string procName, List<DbParameter> prams)
        {
            this.RunProc(procName, prams, out DataSet dataSet);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        #endregion

        #region 执行存储过程部分 返回（int or 实体） RunProcAsync

        /// <summary>
        /// 执行存储过程返回受影响行数
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
        public Task<int> RunProcAsync(string procName)
        {
            return this.ExecuteNonQueryAsync(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <remarks>返回 <see cref="DbDataReader"/> 对象</remarks>
        public Task<DbDataReader> RunProcDataReaderAsync(string procName)
        {
            return this.ExecuteReaderAsync(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <remarks>返回 <see cref="DataSet"/> 对象</remarks>
        public Task<DataSet> RunProcDataSetAsync(string procName)
        {
            return this.ExecuteDataSetAsync(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns>返回 <see cref="object"/> 对象</returns>
        public Task<object> RunProcExecuteScalarAsync(string procName)
        {
            return this.ExecuteScalarAsync(CommandType.StoredProcedure, procName, null);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public Task<int> RunProcAsync(string procName, List<DbParameter> prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteNonQueryAsync(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="DbParameter"/>[] 的参数</param>
        /// <returns></returns>
        public Task<int> RunProcAsync(string procName, DbParameter[] prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteNonQueryAsync(CommandType.StoredProcedure, procName, prams);
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns>返回 <see cref="DbDataReader"/> 对象</returns>
        public Task<DbDataReader> RunProcDataReaderAsync(string procName, List<DbParameter> prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteReaderAsync(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 根据存储过程，返回<see cref="DataSet"/>数据
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">参数</param>
        /// <returns>返回<see cref="DataSet"/>数据</returns>
        public Task<DataSet> RunProcDataSetAsync(string procName, List<DbParameter> prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteDataSetAsync(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns>返回 <see cref="object"/> 对象</returns>
        public Task<object> RunProcExecuteScalarAsync(string procName, List<DbParameter> prams)
        {
            prams.Add(this.GetReturnParam());
            return this.ExecuteScalarAsync(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
        public async Task<T> RunProcObjectAsync<T>(string procName)
        {
            DataSet dataSet = await this.RunProcDataSetAsync(procName);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行存储过程返回结果
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public async Task<T> RunProcObjectAsync<T>(string procName, List<DbParameter> prams)
        {
            DataSet dataSet = await this.RunProcDataSetAsync(procName, prams);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]);
            }
            return default;
        }

        /// <summary>
        /// 执行存储过程返回结果 <see cref="IList{T}"/>数据集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
        public async Task<IList<T>> RunProcObjectListAsync<T>(string procName)
        {
            DataSet dataSet = await this.RunProcDataSetAsync(procName);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 执行存储过程返回结果 <see cref="IList{T}"/>数据集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">表示 <see cref="List{DbParameter}"/> 的参数</param>
        /// <returns></returns>
        public async Task<IList<T>> RunProcObjectListAsync<T>(string procName, List<DbParameter> prams)
        {
            DataSet dataSet = await this.RunProcDataSetAsync(procName, prams);
            if (!dataSet.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
        }

        #endregion

        #region 执行拆分命令 ExecuteCommandWithSplitter

        /// <summary>
        /// 执行拆分命令 拆分符（\r\nGO\r\n）
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        public void ExecuteCommandWithSplitter(string commandText)
        {
            this.ExecuteCommandWithSplitter(commandText, "\r\nGO\r\n");
        }

        /// <summary>
        /// 执行拆分命令
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="splitter">拆分符</param>
        public void ExecuteCommandWithSplitter(string commandText, string splitter)
        {
            int num = 0;
            do
            {
                int num2 = commandText.IndexOf(splitter, num);
                int length = ((num2 > num) ? num2 : commandText.Length) - num;
                string text = commandText.Substring(num, length);
                if (text.Trim().Length > 0)
                {
                    this.ExecuteNonQuery(CommandType.Text, text);
                }
                if (num2 == -1)
                {
                    break;
                }
                num = num2 + splitter.Length;
            }
            while (num < commandText.Length);
        }

        #endregion

        #region 执行拆分命令 ExecuteCommandWithSplitterAsync

        /// <summary>
        /// 执行拆分命令 拆分符（\r\nGO\r\n）
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        public Task ExecuteCommandWithSplitterAsync(string commandText)
        {
            return this.ExecuteCommandWithSplitterAsync(commandText, "\r\nGO\r\n");
        }

        /// <summary>
        /// 执行拆分命令
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="splitter">拆分符</param>
        public async Task ExecuteCommandWithSplitterAsync(string commandText, string splitter)
        {
            int num = 0;
            do
            {
                int num2 = commandText.IndexOf(splitter, num);
                int length = ((num2 > num) ? num2 : commandText.Length) - num;
                string text = commandText.Substring(num, length);
                if (text.Trim().Length > 0)
                {
                    await this.ExecuteNonQueryAsync(CommandType.Text, text);
                }
                if (num2 == -1)
                {
                    break;
                }
                num = num2 + splitter.Length;
            }
            while (num < commandText.Length);
        }

        #endregion

        #region DataSet 对象操作数据库

        ///// <summary>
        ///// 更新数据集
        ///// </summary>
        ///// <param name="dataSet">没有用到</param>
        ///// <param name="tableName">表名</param>
        //public void UpdateByDataSet(DataSet dataSet, string tableName)
        //{
        //    DbDataAdapter dbDataAdapter = CreateDataAdapter();
        //    dbDataAdapter.SelectCommand.CommandText = string.Format("Select * from {0} ORDER BY DayID DESC", tableName);
        //    DbCommandBuilder dbCommandBuilder = CreateCommandBuilder();
        //    dbCommandBuilder.DataAdapter.SelectCommand.Connection = CreateConnection();
        //    DataSet dataSet2 = new DataSet();
        //    dbDataAdapter.Fill(dataSet2);
        //    dataSet2.Tables[0].Rows[0][1] = "107";
        //    dbDataAdapter.Update(dataSet2);
        //}

        /// <summary>
        /// 更新数据集
        /// </summary>
        /// <param name="dataSet">更改成的数据集</param>
        /// <param name="tableName">表名</param>
        public void UpdateDataSet(DataSet dataSet, string tableName)
        {
            string commandText = string.Format("Select * from {0} where 1=0", tableName);
            DbCommandBuilder dbCommandBuilder = CreateCommandBuilder();
            dbCommandBuilder.DataAdapter = CreateDataAdapter();
            dbCommandBuilder.DataAdapter.SelectCommand = CreateCommand();
            dbCommandBuilder.DataAdapter.DeleteCommand = CreateCommand();
            dbCommandBuilder.DataAdapter.InsertCommand = CreateCommand();
            dbCommandBuilder.DataAdapter.UpdateCommand = CreateCommand();
            dbCommandBuilder.DataAdapter.SelectCommand.CommandText = commandText;
            dbCommandBuilder.DataAdapter.SelectCommand.Connection = NewDbConnection();
            dbCommandBuilder.DataAdapter.DeleteCommand.Connection = NewDbConnection();
            dbCommandBuilder.DataAdapter.InsertCommand.Connection = NewDbConnection();
            dbCommandBuilder.DataAdapter.UpdateCommand.Connection = NewDbConnection();
            this.UpdateDataSet(dbCommandBuilder.GetInsertCommand(), dbCommandBuilder.GetDeleteCommand(), dbCommandBuilder.GetUpdateCommand(), dataSet, tableName);
        }

        /// <summary>
        /// 更新数据集
        /// </summary>
        /// <param name="insertCommand"></param>
        /// <param name="deleteCommand"></param>
        /// <param name="updateCommand"></param>
        /// <param name="dataSet"></param>
        /// <param name="tableName">表名</param>
        public void UpdateDataSet(DbCommand insertCommand, DbCommand deleteCommand, DbCommand updateCommand, DataSet dataSet, string tableName)
        {
            ThrowIfNullString(tableName, nameof(tableName));
            using DbDataAdapter dbDataAdapter = CreateDataAdapter();
            dbDataAdapter.UpdateCommand = updateCommand ?? throw new ArgumentNullException(nameof(updateCommand));
            dbDataAdapter.InsertCommand = insertCommand ?? throw new ArgumentNullException(nameof(insertCommand));
            dbDataAdapter.DeleteCommand = deleteCommand ?? throw new ArgumentNullException(nameof(deleteCommand));
            dbDataAdapter.Update(dataSet, tableName);
            dataSet.AcceptChanges();
        }

        #endregion

        #region 特殊公开情况，可能后期移除

        /// <summary>
        /// 根据表名返回当前这张表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetEmptyTable(string tableName)
        {
            string commandText = string.Format("SELECT * FROM {0} WHERE 1=0", tableName);
            return this.ExecuteDataSet(commandText).Tables[0];
        }

        /// <summary>
        /// 根据表名返回当前这张表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<DataTable> GetEmptyTableAsync(string tableName)
        {
            string commandText = string.Format("SELECT * FROM {0} WHERE 1=0", tableName);
            var dataset = await this.ExecuteDataSetAsync(commandText);
            return dataset.Tables[0];
        }

        #endregion

        #region 私有其他部分

        internal DbConnection NewDbConnection()
        {
            DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
            return dbConnection;
        }

        /// <summary>
        /// 获取执行时间器
        /// </summary>
        /// <returns></returns>
        internal Stopwatch GetStopwatch()
        {
            Stopwatch watch = null;
            if (IsSqlLog)
            {
                watch = Stopwatch.StartNew();
            }
            return watch;
        }

        /// <summary>
        /// 获取执行时间器
        /// </summary>
        /// <returns></returns>
        internal Exception GetException(Exception exception, ref bool iserror, ref string guid)
        {
            if (IsSqlLog)
            {
                iserror = true;
                guid = Guid.NewGuid().ToString();
                return new Exception($"跟踪Id:{guid}", exception);
            }
            return exception;
        }

        /// <summary>
        /// 增加SQL请求日志
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="watch">时间测量器</param>
        /// <param name="cmdParams">执行参数</param>
        /// <param name="iserror">是否执行时异常</param>
        /// <param name="guid">有guid时异常</param>
        private void AddQueryDetail(string commandText, Stopwatch watch, DbParameter[] cmdParams, bool iserror, string guid)
        {
            watch.Stop();
            if (IsSqlLog)
            {
                string sqltext = GetQueryDetail(commandText, watch.ElapsedMilliseconds, cmdParams, guid);// IsSqlLogHtml,
                string logpath = $"{LogPath}{DbProviderName}{log_subPath}";//{Logger}
                if (iserror)
                {
                    Log.Error(sqltext, logpath);
                }
                else
                {
                    Log.Debug(sqltext, logpath);
                }
                Info(sqltext, iserror);
            }
            m_queryCount.Increment();
            //unchecked
            //{
            //    this.QueryCount++;
            //}
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// 增加SQL请求日志
        /// </summary>
        /// <param name="dbBatchCommands">批处理命令</param>
        /// <param name="watch">时间测量器</param>
        /// <param name="iserror">是否执行时异常</param>
        /// <param name="guid">有guid时异常</param>
        internal void AddQueryDetail(DbBatchCommandCollection dbBatchCommands, Stopwatch watch, bool iserror, string guid)
        {
            watch.Stop();
            if (IsSqlLog)
            {
                StringBuilder sqllog = new();
                sqllog.AppendLine($"SQL执行情况：{guid}");
                sqllog.Append("  耗时：").Append(watch.ElapsedMilliseconds).AppendLine("毫秒");
                foreach (var command in dbBatchCommands)
                {
                    sqllog.Append(GetSqlLog(command));
                }
                string sqltext = sqllog.ToString(0, sqllog.Length - Environment.NewLine.Length);
                string logpath = $"{LogPath}{DbProviderName}{log_subPath}";//{Logger}
                if (iserror)
                {
                    Log.Error(sqltext, logpath);
                }
                else
                {
                    Log.Debug(sqltext, logpath);
                }
                Info(sqltext, iserror);
            }
            m_queryCount.Increment();

            static StringBuilder GetSqlLog(DbBatchCommand batchCommand)
            {
                StringBuilder sqllog = new();

                sqllog.Append("  执行命令：").AppendLine(batchCommand.CommandText);

                if (batchCommand.Parameters != null && batchCommand.Parameters.Count > 0)
                {
                    sqllog.AppendLine("  执行参数：");
                    foreach (DbParameter cmdParam in batchCommand.Parameters)
                    {
                        sqllog.AppendFormat("  参数名：{0}，类型：{1}，值：{2}{3}", cmdParam.ParameterName, cmdParam.DbType, cmdParam.Value, Environment.NewLine);
                    }
                }

                return sqllog;
            }
        }
#endif

        private void Info(string sqltext, bool iserror)
        {
            if (Logger is not null)
            {
                var loglevel = iserror ? LogLevel.Error : LogLevel.Information;
                if (Logger.IsEnabled(loglevel))
                {
                    Logger.Log(loglevel, "{Sqltext}", sqltext);
                }
            }
        }

        /// <summary>
        /// SQL执行完成后的日志生成
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="ElapsedMilliseconds">执行毫秒</param>
        /// <param name="cmdParams">执行参数</param>
        /// <param name="guid">id</param>
        /// <returns>返回日志</returns>
        private static string GetQueryDetail(string commandText, long ElapsedMilliseconds, DbParameter[] cmdParams, string guid)// bool IsSqlLogHtml,
        {
            //if (IsSqlLogHtml)
            //{
            //    const string text = "<tr style=\"background: rgb(255, 255, 255) none repeat scroll 0%; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">";
            //    StringBuilder text2 = new(), text3 = new(), text4 = new();
            //    string arg = "";
            //    if (cmdParams != null && cmdParams.Length > 0)
            //    {
            //        for (int i = 0; i < cmdParams.Length; i++)
            //        {
            //            DbParameter dbParameter = cmdParams[i];
            //            if (dbParameter != null)
            //            {
            //                text2.Append($"<td>{dbParameter.ParameterName}</td>");
            //                text3.Append($"<td>{dbParameter.DbType}</td>");
            //                text4.Append($"<td>{dbParameter.Value}</td>");
            //            }
            //        }
            //        arg = string.Format("<table width=\"100%\" cellspacing=\"1\" cellpadding=\"0\" style=\"background: rgb(255, 255, 255) none repeat scroll 0%; margin-top: 5px; font-size: 12px; display: block; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">{0}{1}</tr>{0}{2}</tr>{0}{3}</tr></table>", new object[]
            //        {
            //            text,
            //            text2,
            //            text3,
            //            text4
            //        });
            //    }
            //    if (!string.IsNullOrEmpty(guid))
            //    {
            //        guid = $"<div style=\"font-size: 12px; float: right; width: 100px; margin-bottom: 5px;\"><b>UUID:</b>{guid}</div>";
            //    }
            //    return string.Concat("<center><div style=\"border: 1px solid black; margin: 2px; padding: 1em; text-align: left; width: 96%; clear: both;\"><div style=\"font-size: 12px; float: right; width: 100px; margin-bottom: 5px;\"><b>TIME:</b> ", ElapsedMilliseconds, " 毫秒</div>", guid, "<span style=\"font-size: 12px;\">", commandText, arg, "</span></div><br /></center>");
            //}
            //else
            //{
            StringBuilder sqllog = new();

            sqllog.AppendLine($"SQL执行情况：{guid}");
            sqllog.Append("  耗时：").Append(ElapsedMilliseconds).AppendLine("毫秒");
            sqllog.Append("  执行命令：").AppendLine(commandText);

            if (cmdParams != null && cmdParams.Length > 0)
            {
                sqllog.AppendLine("  执行参数：");
                foreach (DbParameter cmdParam in cmdParams)
                {
                    sqllog.AppendFormat("  参数名：{0}，类型：{1}，值：{2}{3}", cmdParam.ParameterName, cmdParam.DbType, cmdParam.Value, Environment.NewLine);
                }
            }

            return sqllog.ToString(0, sqllog.Length - Environment.NewLine.Length);
            //}
        }

        /// <summary>
        /// 异常提示
        /// </summary>
        /// <param name="message">提示信息</param>
        private static Exception Throw(string message)
        {
            return new Exception("SQL执行过程发生的异常：" + message);
        }

        #endregion

        #region 获取指定存储过程名称的参数  返回（DbParameter[]） GetSpParameterSet

        /// <summary>
        /// 根据存储过程名获得存储过程所需要的参数
        /// </summary>
        /// <param name="spName">储过程名</param>
        /// <returns></returns>
        public DbParameter[] GetSpParameterSet(string spName)
        {
            return this.GetSpParameterSet(spName, false);
        }

        /// <summary>
        /// 根据存储过程名获得存储过程所需要的参数
        /// </summary>
        /// <param name="spName">储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含的返回值参数</param>
        /// <returns></returns>
        public DbParameter[] GetSpParameterSet(string spName, bool includeReturnValueParameter)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            DbParameter[] spParameterSetInternal;
            using (DbConnection dbConnection = NewDbConnection())
            {
                spParameterSetInternal = this.GetSpParameterSetInternal(dbConnection, spName, includeReturnValueParameter);
            }
            return spParameterSetInternal;
        }

        internal DbParameter[] GetSpParameterSet(DbConnection connection, string spName)
        {
            return this.GetSpParameterSet(connection, spName, false);
        }

        internal DbParameter[] GetSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            ThrowIfNull(connection, nameof(connection));
            DbParameter[] spParameterSetInternal = default;
            if (connection is ICloneable cloneable)
            {
                using DbConnection dbConnection = cloneable.Clone() as DbConnection;
                spParameterSetInternal = this.GetSpParameterSetInternal(dbConnection, spName, includeReturnValueParameter);
            }
            return spParameterSetInternal;
        }

        private DbParameter[] GetSpParameterSetInternal(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            string key = $"{connection.ConnectionString}:{spName}{(includeReturnValueParameter ? ":包含ReturnValue参数" : "")}";

            DbParameterCache array = m_paramcache.GetOrAdd(key, Get);

            DbParameterCache Get(string key)
            {
                return new DbParameterCache(this.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter));
            }
            //if (this.m_paramcache[key] is not DbParameter[] array)
            //{
            //    DbParameter[] array2 = this.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
            //    this.m_paramcache[key] = array2;
            //    array = array2;
            //}
            return array.CloneParameters();
        }

        #endregion

        #region 获取指定存储过程名称的参数  返回（DbParameter[]） GetSpParameterSetAsync

        /// <summary>
        /// 根据存储过程名获得存储过程所需要的参数
        /// </summary>
        /// <param name="spName">储过程名</param>
        /// <returns></returns>
        public Task<DbParameter[]> GetSpParameterSetAsync(string spName)
        {
            return this.GetSpParameterSetAsync(spName, false);
        }

        /// <summary>
        /// 根据存储过程名获得存储过程所需要的参数
        /// </summary>
        /// <param name="spName">储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含的返回值参数</param>
        /// <returns></returns>
        public Task<DbParameter[]> GetSpParameterSetAsync(string spName, bool includeReturnValueParameter)
        {
            IsNullConnectionString();
            ThrowIfNullString(spName, nameof(spName));
            using DbConnection dbConnection = NewDbConnection();
            return this.GetSpParameterSetInternalAsync(dbConnection, spName, includeReturnValueParameter);
        }

        internal Task<DbParameter[]> GetSpParameterSetAsync(DbConnection connection, string spName)
        {
            return this.GetSpParameterSetAsync(connection, spName, false);
        }

        internal Task<DbParameter[]> GetSpParameterSetAsync(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            ThrowIfNull(connection, nameof(connection));
            if (connection is ICloneable cloneable)
            {
                using DbConnection dbConnection = cloneable.Clone() as DbConnection;
                return this.GetSpParameterSetInternalAsync(dbConnection, spName, includeReturnValueParameter);
            }
            throw new Exception("无法克隆DbConnection进行存储过程参数查询！");
        }

        private async Task<DbParameter[]> GetSpParameterSetInternalAsync(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            ThrowIfNull(connection, nameof(connection));
            ThrowIfNullString(spName, nameof(spName));
            string key = $"{connection.ConnectionString}:{spName}{(includeReturnValueParameter ? ":包含ReturnValue参数" : "")}";

            DbParameterCache array = m_paramcache.GetOrAdd(key, Get);
            DbParameterCache Get(string key)
            {
                return new DbParameterCache(this.DiscoverSpParameterSetAsync(connection, spName, includeReturnValueParameter));
            }

            //if (this.m_paramcache[key] is not DbParameter[] array)
            //{
            //    DbParameter[] array2 = await this.DiscoverSpParameterSetAsync(connection, spName, includeReturnValueParameter);
            //    this.m_paramcache[key] = array2;
            //    array = array2;
            //}
            return await array.CloneParametersAsync();
        }

        #endregion

        #region SQL参数绑定函数集 返回（DbParameter） GetInParam

        /// <summary>
        /// 绑定数据 （例如：@id=1）
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraValue">数据</param>
        /// <returns></returns>
        public DbParameter GetInParam(string paraName, object paraValue)
        {
            return this.GetParam(paraName, paraValue, ParameterDirection.Input);
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraType">类型</param>
        /// <returns></returns>
        public DbParameter GetOutParam(string paraName, Type paraType)
        {
            return this.GetParam(paraName, null, ParameterDirection.Output, paraType, "");
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraType">类型</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public DbParameter GetOutParam(string paraName, Type paraType, int size)
        {
            return this.GetParam(paraName, null, ParameterDirection.Output, paraType, "", size);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraType">类型</param>
        /// <param name="paraValue">数据</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public DbParameter GetOutParam(string paraName, object paraValue, Type paraType, int size)
        {
            return this.GetParam(paraName, paraValue, ParameterDirection.Output, paraType, "", size);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraValue">数据</param>
        /// <param name="direction">数据库对应类型</param>
        /// <returns></returns>
        public DbParameter GetParam(string paraName, object paraValue, ParameterDirection direction)
        {
            //return this.Provider.MakeParam(GetDbParameter(paraName), paraValue, direction);
            Type paraType = paraValue?.GetType();
            return GetParam(paraName, paraValue, direction, paraType, null);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraValue">数据</param>
        /// <param name="direction">数据库对应类型</param>
        /// <param name="paraType">类型</param>
        /// <param name="sourceColumn">源列</param>
        /// <returns></returns>
        public DbParameter GetParam(string paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn)
        {
            //return this.Provider.MakeParam(GetDbParameter(paraName), paraValue, direction, paraType, sourceColumn);
            return GetParam(paraName, paraValue, direction, paraType, sourceColumn, 0);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="paraName">名字</param>
        /// <param name="paraValue">数据</param>
        /// <param name="direction">数据库对应类型</param>
        /// <param name="paraType">类型</param>
        /// <param name="sourceColumn">源列</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public DbParameter GetParam(string paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn, int size)
        {
            DbParameter dbParameter = GetDbParameter(paraName, paraValue, direction, sourceColumn, size);
            this.Provider.GetParam(ref dbParameter, paraValue, direction, paraType, sourceColumn, size);
            return dbParameter;
        }

        private DbParameter GetDbParameter(string paraName, object paraValue, ParameterDirection direction, string sourceColumn, int size)
        {
            DbParameter dbParameter = CreateParameter();
            dbParameter.ParameterName = this.Provider.ParameterPrefix + paraName;
            if (direction == ParameterDirection.Output) dbParameter.Size = size;
            if (sourceColumn != null) dbParameter.SourceColumn = sourceColumn;
            dbParameter.Value = paraValue ?? DBNull.Value;
            dbParameter.Direction = direction;
            return dbParameter;
        }

        /// <summary>
        /// 获取存储过程执行，返回结果参数 , 默认参数：ReturnValue。
        /// </summary>
        /// <returns></returns>
        public DbParameter GetReturnParam()
        {
            return this.GetReturnParam("ReturnValue");
        }

        /// <summary>
        /// 获取存储过程执行，返回结果参数。
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <returns></returns>
        public DbParameter GetReturnParam(string paraName)
        {
            return this.GetParam(paraName, 0, ParameterDirection.ReturnValue);
        }

        #endregion

        #region 私有SQL验证部分

        private static void PrepareCommand(DbCommand command, int CommandTimeout, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, out bool mustCloseConnection)
        {
            ThrowIfNull(command, nameof(command));
            ThrowIfNullString(commandText, nameof(commandText));
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            command.CommandTimeout = CommandTimeout;
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
                }
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        private static async Task<bool> OpenCommandAsync(DbConnection connection)
        {
            bool mustCloseConnection;
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                await connection.OpenAsync();
            }
            else
            {
                mustCloseConnection = false;
            }
            return mustCloseConnection;
        }

        #endregion

        #region 私有变量

        private readonly object lockHelper = new();

        /// <summary>
        /// 请求计数
        /// </summary>
        private ulong m_queryCount;

        /// <summary>
        /// 连接字符串
        /// </summary>
		private string m_connectionstring;

        /// <summary>
        /// 子路径
        /// </summary>
        private string log_subPath = string.Empty;

        /// <summary>
        /// 数据库类型对象
        /// </summary>
        private DbProviderType m_dbProviderType;

        /// <summary>
        /// 数据库类型对象
        /// </summary>
        private string m_dbProviderName;

        private DbProviderFactory m_factory;

        //private readonly Hashtable m_paramcache = Hashtable.Synchronized(new());

        private readonly ConcurrentDictionary<string, DbParameterCache> m_paramcache = new();

        private IDbProvider m_provider;

        #endregion

        #region 私有枚举类

        private enum DbConnectionOwnership
        {
            Internal,
            External
        }

        #endregion
    }
}
