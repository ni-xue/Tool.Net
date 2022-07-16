using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Utils.Data;

namespace Tool.SqlCore
{
    /// <summary>
    /// Sql 核心 操作底层
    /// </summary>
    public class DbHelper
    {
        #region Sql 公开变量

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
        protected internal string DbProviderName
        {
            get
            {
                return this.m_dbProviderName;
            }
            //set
            //{
            //    this.m_dbProviderName = value;
            //}
        }


        /// <summary>
        /// 获取当前访问的数据库类型
        /// </summary>
        protected internal DbProviderType DbProviderType
        {
            get
            {
                return this.m_dbProviderType;
            }
            //set
            //{
            //    this.m_dbProviderType = value;
            //}
        }

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
        /// IDB提供商
        /// </summary>
        public IDbProvider Provider
        {
            get
            {
                //if (this.m_provider == null)
                //{
                //    lock (this.lockHelper)
                //    {
                //        if (this.m_provider == null)
                //        {
                //            try
                //            {
                //                this.m_provider = new SqlServerProvider();
                //                //this.m_provider = (IDbProvider)Activator.CreateInstance(Type.GetType("Game.Kernel.SqlServerProvider, Game.Kernel", false, true));
                //            }
                //            catch
                //            {
                //                throw Throw("SqlServerProvider 数据库访问器创建失败！");
                //            }
                //        }
                //    }
                //}
                return this.m_provider;
            }
        }

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
        public bool IsSqlLogHtml { get; set; } = false;

        /// <summary>
        /// 查询计数
        /// </summary>
        public ulong QueryCount { get; set; }

        ///// <summary>
        ///// 程序运行过程中的所有增删改查操作的耗时详情
        ///// </summary>
        //public static List<string> QueryDetail
        //{
        //    get
        //    {
        //        return DbHelper.m_querydetail;
        //    }
        //    set
        //    {
        //        DbHelper.m_querydetail = value;
        //    }
        //}

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
        /// <typeparam name="T">指定数据库的 <see cref="DbCommand"/> 实现类</typeparam>
        /// <returns><see cref="DbCommand"/> 的新实例。</returns>
        public T CreateCommand<T>() where T : DbCommand => CreateCommand() as T;

        /// <summary>
        /// 返回实现 <see cref="DbCommandBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbCommandBuilder"/> 的新实例。</returns>
        public DbCommandBuilder CreateCommandBuilder() => this.Factory.CreateCommandBuilder();

        /// <summary>
        /// 返回实现 <see cref="DbCommandBuilder"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbCommandBuilder"/> 实现类</typeparam>
        /// <returns><see cref="DbCommandBuilder"/> 的新实例。</returns>
        public T CreateCommandBuilder<T>() where T : DbCommandBuilder => CreateCommandBuilder() as T;

        /// <summary>
        /// 返回实现 <see cref="DbConnection"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbConnection"/> 的新实例。</returns>
        public DbConnection CreateConnection() => this.Factory.CreateConnection();

        /// <summary>
        /// 返回实现 <see cref="DbConnection"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbConnection"/> 实现类</typeparam>
        /// <returns><see cref="DbConnection"/> 的新实例。</returns>
        public T CreateConnection<T>() where T : DbConnection => CreateConnection() as T;

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
        /// <typeparam name="T">指定数据库的 <see cref="DbDataAdapter"/> 实现类</typeparam>
        /// <returns><see cref="DbDataAdapter"/> 的新实例。</returns>
        public T CreateDataAdapter<T>() where T : DbDataAdapter => CreateDataAdapter() as T;

        /// <summary>
        /// 返回实现 <see cref="DbParameter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbParameter"/> 的新实例。</returns>
        public DbParameter CreateParameter() => this.Factory.CreateParameter();

        /// <summary>
        /// 返回实现 <see cref="DbParameter"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <typeparam name="T">指定数据库的 <see cref="DbParameter"/> 实现类</typeparam>
        /// <returns><see cref="DbParameter"/> 的新实例。</returns>
        public T CreateParameter<T>() where T : DbParameter => CreateParameter() as T;

        /// <summary>
        /// 返回实现 <see cref="DbTransaction"/> 类的提供程序类的一个新实例。
        /// </summary>
        /// <returns><see cref="DbTransaction"/> 的新实例。</returns>
        public DbTransaction CreateTransaction()
        {
            DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
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
            DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
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
            if (parameter != null)
            {
                List<DbParameter> parms = new();

                Type type = parameter.GetType();

                if (type.IsAssignableTo(typeof(IDictionary)))
                {
                    foreach (DictionaryEntry keys in parameter as IDictionary)
                    {
                            parms.Add(this.GetInParam(keys.Key?.ToString(), keys.Value));
                    }
                    return parms;
                }
                else
                {
                    PropertyInfo[] _properties = type.GetProperties();

                    if (_properties.Length > 0)
                    {
                        foreach (PropertyInfo property in _properties)
                        {
                            object Value = property.GetValue(parameter);
                            if (Value != null) //因为此处直接将空值忽略了导致的一个问题后面考虑修复
                                parms.Add(this.GetInParam(property.Name, Value));
                        }
                        return parms;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 将匿名对象转换成<see cref="DbParameter"/>[]对象集合
        /// </summary>
        /// <param name="parameter"><see cref="Dictionary{String,Object}"/>对象</param>
        /// <returns><see cref="DbParameter"/>[]对象集合</returns>
        public List<DbParameter> SetParameterList(Dictionary<string, object> parameter)
        {
            if (parameter != null && parameter.Count > 0)
            {
                List<DbParameter> parms = new();

                foreach (var pair in parameter)
                {
                    if (pair.Value != null)
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
        public Dictionary<string, object> SetDictionaryParam(object parameter)
        {
            if (parameter != null)
            {
                Dictionary<string, object> keyValuePairs = new();

                Type type = parameter.GetType();

                if (type.IsAssignableTo(typeof(IDictionary)))
                {
                    foreach (DictionaryEntry keys in parameter as IDictionary)
                    {
                        keyValuePairs.Add(keys.Key?.ToString(), keys.Value);
                    }
                    return keyValuePairs;
                }
                else
                {
                    PropertyInfo[] _properties = type.GetProperties();

                    if (_properties.Length > 0)
                    {
                        foreach (PropertyInfo property in _properties)
                        {
                            object Value = property.GetValue(parameter);

                            keyValuePairs.Add(property.Name, Value);
                        }
                        return keyValuePairs;
                    }
                }
            }
            return null;
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

            //LoggerFactoryExtensions

            this.Logger = logger;

            //Action<ILogger,Exception> action = LoggerMessage.Define(LogLevel.Information, new EventId(1, "Sql"), "");

            //action()

            this.m_connectionstring = connectionString;
            this.m_dbProviderType = dbProviderType;
            this.m_dbProviderName = dbProviderName;
            this.m_provider = dbProvider;
            this.QueryCount = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <param name="dataRow"></param>
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
                    if (dataRow.Table.Columns.IndexOf(dbParameter.ParameterName.Substring(1)) != -1)
                    {
                        dbParameter.Value = dataRow[dbParameter.ParameterName.Substring(1)];
                    }
                    num++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <param name="parameterValues"></param>
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
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
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

        /// <summary>
        /// 克隆一个副本
        /// </summary>
        /// <param name="originalParameters">源对象数据</param>
        /// <returns></returns>
        private static DbParameter[] CloneParameters(DbParameter[] originalParameters)
        {
            DbParameter[] array = new DbParameter[originalParameters.Length];
            int i = 0;
            int num = originalParameters.Length;
            while (i < num)
            {
                array[i] = (DbParameter)((ICloneable)originalParameters[i]).Clone();
                i++;
            }
            return array;
        }

        private static ArgumentNullException NullConnectionString => new ArgumentNullException(nameof(ConnectionString));

        #endregion

        #region 公共SQL部分公开函数

        /// <summary>
        /// 添加缓存参数集
        /// </summary>
        /// <param name="commandText">缓存数据集的名称</param>
        /// <param name="commandParameters">缓存的数据集</param>
        public void SetCacheParameterSet(string commandText, params DbParameter[] commandParameters)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException(nameof(commandText));
            }
            string key = this.ConnectionString + ":" + commandText;
            this.m_paramcache[key] = commandParameters;
        }

        /// <summary>
        /// 获取缓存的参数集，每次返回的都是克隆数据
        /// </summary>
        /// <param name="commandText">缓存数据集的名称</param>
        /// <returns></returns>
        public DbParameter[] GetCachedParameterSet(string commandText)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException(nameof(commandText));
            }
            string key = this.ConnectionString + ":" + commandText;
            if (this.m_paramcache[key] is not DbParameter[] array)
            {
                return null;
            }
            return CloneParameters(array);
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
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            DbCommand dbCommand = CreateCommand();
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
        /// 获取当前存储过程执行所需要的参数
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含的返回值参数</param>
        /// <returns></returns>
        private DbParameter[] DiscoverSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            DbCommand dbCommand = connection.CreateCommand();
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
            return ExecuteDataset(CommandType.Text, commandText, commandParameters);
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

        #region ExecuteCommandWithSplitter 无法归类

        /// <summary>
        /// 执行拆分命令
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
        /// <param name="splitter">验证</param>
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

        #region Sql 公共查询函数 返回（DataSet） ExecuteDataSet

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string commandText)
        {
            return this.ExecuteDataset(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string commandText, object prams)
        {
            return this.ExecuteDataset(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            return this.ExecuteDataset(commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText, object prams)
        {
            return ExecuteDataset(commandType, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="commandParameters">参数对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            DataSet result;
            using (DbConnection dbConnection = CreateConnection())
            {
                dbConnection.ConnectionString = this.ConnectionString;
                dbConnection.Open();
                result = this.ExecuteDataset(dbConnection, commandType, commandText, commandParameters);
            }
            return result;
        }

        #region 查询结果异步模块

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(string commandText)
        {
            return await this.ExecuteDataSetAsync(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(string commandText, object prams)
        {
            return await this.ExecuteDataSetAsync(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText)
        {
            return await this.ExecuteDataSetAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(CommandType commandType, string commandText, object prams)
        {
            return await ExecuteDataSetAsync(commandType, commandText, SetParameters(prams));
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }

            async Task<DataSet> ExecuteDataset()
            {
                DataSet result;
                using (DbConnection dbConnection = CreateConnection())
                {
                    dbConnection.ConnectionString = this.ConnectionString;
                    await dbConnection.OpenAsync();//.Wait()
                    result = this.ExecuteDataset(dbConnection, commandType, commandText, commandParameters);
                }
                return result;
            }

            return await ExecuteDataset();//Task.Run(ExecuteDataset);
        }

        #endregion

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库链接对象</param>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">Sql字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteDataset(connection, commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out bool flag);
            DataSet result;
            using (DbDataAdapter dbDataAdapter = CreateDataAdapter())
            {
                dbDataAdapter.SelectCommand = dbCommand;
                DataSet dataSet = new();

                Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();

                dbDataAdapter.Fill(dataSet);

                AddQueryDetail(dbCommand.CommandText, watch, commandParameters);
                //DbHelper.m_querydetail.Add(DbHelper.GetQueryDetail(dbCommand.CommandText, now, now2, commandParameters));
                dbCommand.Parameters.Clear();
                if (flag)
                {
                    connection.Close();
                }
                result = dataSet;
            }
            return result;
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteDataset(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("提交事务不能为空");
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", "提交事务");
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        }

        /// <summary>
        /// 执行SQL获取<see cref="DataSet"/>数据源
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("提交事务不能为空");
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", "提交事务");
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out _);
            DataSet result;
            using (DbDataAdapter dbDataAdapter = CreateDataAdapter())
            {
                dbDataAdapter.SelectCommand = dbCommand;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                dbCommand.Parameters.Clear();
                result = dataSet;
            }
            return result;
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataset(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(CommandType.StoredProcedure, spName);
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            int result;
            using (DbConnection dbConnection = CreateConnection())
            {
                dbConnection.ConnectionString = this.ConnectionString;
                dbConnection.Open();
                result = this.ExecuteNonQuery(dbConnection, commandType, commandText, commandParameters);
            }
            return result;
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out bool flag);
            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            int result = dbCommand.ExecuteNonQuery();
            AddQueryDetail(dbCommand.CommandText, watch, commandParameters);
            //DbHelper.m_querydetail.Add(DbHelper.GetQueryDetail(dbCommand.CommandText, now, now2, commandParameters));
            dbCommand.Parameters.Clear();
            if (flag)
            {
                connection.Close();
            }
            return result;
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out _);
            int result = dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
            return result;
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns>返回受影响行数</returns>
        public int ExecuteNonQuery(DbTransaction transaction, params SqlTextParameter[] sqlTexts)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (sqlTexts == null)
            {
                throw new ArgumentNullException("多条执行SQL对象为空。", "sqlTexts");
            }
            int result = 0;
            using DbCommand dbCommand = CreateCommand();
            foreach (SqlTextParameter sqlText in sqlTexts)
            {
                PrepareCommand(dbCommand, transaction.Connection, transaction, sqlText.CommandType, sqlText.CommandText, sqlText.Parameters, out bool flag);
                result += dbCommand.ExecuteNonQuery();
                dbCommand.Parameters.Clear();
            }
            return result;
        }

        #endregion

        #region Sql 公共执行返回受影响行数 返回（Task<int>） ExecuteNonQuery

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText)
        {
            return await this.ExecuteNonQueryAsync(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">字符串映射对象</param>
        /// <returns>受影响行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText, object prams)
        {
            return await this.ExecuteNonQueryAsync(CommandType.Text, commandText, SetParameters(prams));
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText)
        {
            return await this.ExecuteNonQueryAsync(commandType, commandText, null);
        }

        /// <summary>
        /// 根据SQL返回受影响行数(异步等待)
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="prams">字符串映射对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, object prams)
        {
            return await this.ExecuteNonQueryAsync(commandType, commandText, SetParameters(prams));
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            int result;
            using (DbConnection dbConnection = CreateConnection())
            {
                dbConnection.ConnectionString = this.ConnectionString;
                await dbConnection.OpenAsync();
                result = await this.ExecuteNonQueryAsync(dbConnection, commandType, commandText, commandParameters);
            }
            return result;
        }

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandParameters">字符串映射对象</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out bool flag);
            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            int result = await dbCommand.ExecuteNonQueryAsync();
            AddQueryDetail(dbCommand.CommandText, watch, commandParameters);
            //DbHelper.m_querydetail.Add(DbHelper.GetQueryDetail(dbCommand.CommandText, now, now2, commandParameters));
            dbCommand.Parameters.Clear();
            if (flag)
            {
                connection.Close();
            }
            return result;
        }

        #endregion

        #region Sql 公共执行返回受影响行数 内置事物版本 TransactionExecuteNonQuery （打算移除的部分）-- 以优化

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
            //using (DbConnection dbConnection = CreateConnection())
            //{
            //    dbConnection.ConnectionString = this.ConnectionString;
            //    dbConnection.Open();

            //    DbTransaction transaction = dbConnection.BeginTransaction();

            //    DbCommand dbCommand = CreateCommand();
            //    try
            //    {
            //        dbCommand.Transaction = transaction;

            //        this.PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out bool flag);
            //        result = dbCommand.ExecuteNonQuery();
            //        dbCommand.Parameters.Clear();
            //        transaction.Commit();
            //    }
            //    catch (Exception e)
            //    {
            //        transaction.Rollback();
            //        throw e;
            //    }
            //    finally
            //    {
            //        dbCommand.Parameters.Clear();
            //        transaction.Dispose();
            //    }
            //}
            return TransExecuteNonQuery(new SqlTextParameter(commandType, commandText, commandParameters));
        }

        ///// <summary>
        ///// 根据SQL返回受影响行数
        ///// </summary>
        ///// <param name="commandType">执行类型</param>
        ///// <param name="commandText">SQL语句</param>
        ///// <param name="commandParameters">参数</param>
        ///// <returns></returns>
        //public int TransactionExecuteNonQuerys(CommandType commandType, IList<string> commandText, IList<object> commandParameters)
        //{
        //    IList<DbParameter[]> dbs = new List<DbParameter[]>();
        //    if (commandParameters != null)
        //    {
        //        foreach (object obj in commandParameters)
        //        {
        //            dbs.Add(SetParameters(obj));
        //        }
        //    }
        //    return TransactionExecuteNonQuerys(commandType, commandText, dbs);
        //}

        /// <summary>
        /// 根据SQL返回受影响行数
        /// </summary>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns></returns>
        public DbTransResult TransExecuteNonQuery(params SqlTextParameter[] sqlTexts)
        {
            using DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
            dbConnection.Open();

            DbTransaction transaction = dbConnection.BeginTransaction();

            return transaction.ExecuteNonQuery(this, sqlTexts);
            //    try
            //    {
            //        for (int i = 0; i < commandText.Count; i++)
            //        {
            //            DbCommand dbCommand = CreateCommand();
            //            dbCommand.Transaction = transaction;

            //            this.PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText[i], commandParameters[i], out bool flag);
            //            result = dbCommand.ExecuteNonQuery();
            //            dbCommand.Parameters.Clear();
            //        }

            //        transaction.Commit();
            //    }
            //    catch (Exception e)
            //    {
            //        transaction.Rollback();
            //        throw e;
            //    }
            //    finally
            //    {
            //        transaction.Dispose();
            //    }
            //}
            //return result;
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            int result;
            using (DbConnection dbConnection = CreateConnection())
            {
                dbConnection.ConnectionString = this.ConnectionString;
                dbConnection.Open();
                result = this.ExecuteNonQuery(out id, dbConnection, commandType, commandText, commandParameters);
            }
            return result;
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (this.Provider.GetLastIdSql().Trim() == "")
            {
                throw new ArgumentNullException("GetLastIdSql is \"\"");
            }
            DbCommand dbCommand = CreateCommand();
            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();

            commandText = $"{commandText} {this.Provider.GetLastIdSql()} AS ID";//, @@rowcount AS Count 

            PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out bool flag);

            var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            reader.Read();
            int result = reader.RecordsAffected;
            id = reader.GetValue(0);
            //int result = dbCommand.ExecuteNonQuery();
            //object obj = dbCommand.ExecuteScalar();
            dbCommand.Parameters.Clear();
            //dbCommand.CommandType = CommandType.Text;
            //dbCommand.CommandText = this.Provider.GetLastIdSql();
            //object obj = dbCommand.ExecuteScalar();
            //if (obj is System.DBNull)
            //{
            //    id = 0;
            //}
            //else
            //{
            //    int.TryParse(obj.ToString(), out id);
            //}

            reader.Close();
            AddQueryDetail(dbCommand.CommandText, watch, commandParameters);
            //DbHelper.m_querydetail.Add(DbHelper.GetQueryDetail(dbCommand.CommandText, now, now2, commandParameters));
            if (flag)
            {
                connection.Close();
            }
            return result;
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            DbCommand dbCommand = CreateCommand();

            commandText = $"{commandText} {this.Provider.GetLastIdSql()} AS ID";
            PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out _);
            var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            reader.Read();
            int result = reader.RecordsAffected;
            id = reader.GetValue(0);
            dbCommand.Parameters.Clear();
            //dbCommand.CommandType = CommandType.Text;
            //dbCommand.CommandText = this.Provider.GetLastIdSql();
            //id = int.Parse(dbCommand.ExecuteScalar().ToString());
            //if (obj is System.DBNull)
            //{
            //    id = 0;
            //}
            //else
            //{
            //    int.TryParse(obj.ToString(), out id);
            //}
            reader.Close();
            return result;
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
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
            DataSet dataSet = this.ExecuteDataset(commandText);
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
            DataSet dataSet = this.ExecuteDataset(CommandType.Text, commandText, SetParameters(prams));
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
            DataSet dataSet = this.ExecuteDataset(CommandType.Text, commandText, prams.ToArray());
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
            DataSet dataSet = this.ExecuteDataset(commandText);
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
            DataSet dataSet = this.ExecuteDataset(CommandType.Text, commandText, SetParameters(prams));
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
            DataSet dataSet = this.ExecuteDataset(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(dataSet))
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]);
            }
            return null;
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            DbConnection dbConnection = null;
            DbDataReader result;
            try
            {
                dbConnection = CreateConnection();
                dbConnection.ConnectionString = this.ConnectionString;
                dbConnection.Open();
                result = this.ExecuteReader(dbConnection, null, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.Internal);
            }
            catch
            {
                if (dbConnection != null)
                {
                    dbConnection.Close();
                }
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            return this.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, DbHelper.DbConnectionOwnership.External);
        }

        private DbDataReader ExecuteReader(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, DbHelper.DbConnectionOwnership connectionOwnership)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            bool flag = false;
            DbCommand dbCommand = CreateCommand();
            DbDataReader result;
            try
            {
                PrepareCommand(dbCommand, connection, transaction, commandType, commandText, commandParameters, out flag);
                Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
                DbDataReader dbDataReader;
                if (connectionOwnership == DbHelper.DbConnectionOwnership.External)
                {
                    dbDataReader = dbCommand.ExecuteReader();
                }
                else
                {
                    dbDataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
                AddQueryDetail(dbCommand.CommandText, watch, commandParameters);
                //DbHelper.m_querydetail.Add(DbHelper.GetQueryDetail(dbCommand.CommandText, now, now2, commandParameters));
                bool flag2 = true;
                foreach (DbParameter dbParameter in dbCommand.Parameters)
                {
                    if (dbParameter.Direction != ParameterDirection.Input)
                    {
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    dbCommand.Parameters.Clear();
                }
                result = dbDataReader;
            }
            catch
            {
                if (flag)
                {
                    connection.Close();
                }
                throw;
            }
            return result;
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            object result;
            using (DbConnection dbConnection = CreateConnection())
            {
                dbConnection.ConnectionString = this.ConnectionString;
                dbConnection.Open();
                result = this.ExecuteScalar(dbConnection, commandType, commandText, commandParameters);
            }
            return result;
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out bool flag);
            object result = dbCommand.ExecuteScalar();
            dbCommand.Parameters.Clear();
            if (flag)
            {
                connection.Close();
            }
            return result;
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out _);
            Stopwatch watch = GetStopwatch(); //Stopwatch.StartNew();
            object result = dbCommand.ExecuteScalar();
            AddQueryDetail(dbCommand.CommandText, watch, commandParameters);
            //DbHelper.m_querydetail.Add(DbHelper.GetQueryDetail(dbCommand.CommandText, now, now2, commandParameters));
            dbCommand.Parameters.Clear();
            return result;
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

        #region Sql 公共执行返回查询结果 调用存储过程 返回（object）ExecuteScalarTypedParams

        /// <summary>
        /// 执行SQL返回object类型的数据
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">参数对象</param>
        /// <returns></returns>
        public object ExecuteScalarTypedParams(string spName, DataRow dataRow)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        #endregion

        #region Sql 公共执行返回查询结果 返回（void）但是带入（DataSet）有返回 FillDataset

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames">对应的表名称</param>
        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }
            using DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
            dbConnection.Open();
            this.FillDataset(dbConnection, commandType, commandText, dataSet, tableNames);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        public void FillDataset(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }
            using DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
            dbConnection.Open();
            this.FillDataset(dbConnection, spName, dataSet, tableNames, parameterValues);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="commandParameters">字符串映射对象</param>
        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }
            using DbConnection dbConnection = CreateConnection();
            dbConnection.ConnectionString = this.ConnectionString;
            dbConnection.Open();
            this.FillDataset(dbConnection, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        public void FillDataset(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            this.FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="parameterValues">字符串映射对象</param>
        public void FillDataset(DbConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                this.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
                return;
            }
            this.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType"><see cref="CommandType"/>对象</param>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        public void FillDataset(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            this.FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 通过SQL获取数据对象<see cref="DataSet"/>
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">返回的数据对象</param>
        /// <param name="tableNames"><see cref="DataSet"/>对象中的列名</param>
        /// <param name="parameterValues">字符串映射对象</param>
		public void FillDataset(DbTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", nameof(transaction));
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            if (parameterValues != null && parameterValues.Length > 0)
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                this.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
                return;
            }
            this.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
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
        public void FillDataset(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            this.FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
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
        public void FillDataset(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            this.FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        private void FillDataset(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }
            DbCommand dbCommand = CreateCommand();
            PrepareCommand(dbCommand, connection, transaction, commandType, commandText, commandParameters, out bool flag);
            using (DbDataAdapter dbDataAdapter = CreateDataAdapter())
            {
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
                dbCommand.Parameters.Clear();
            }
            if (flag)
            {
                connection.Close();
            }
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
            return this.ExecuteDataset(commandText).Tables[0];
        }

        #endregion

        #region 私有其他部分

        /// <summary>
        /// 获取执行时间器
        /// </summary>
        /// <returns></returns>
        private Stopwatch GetStopwatch()
        {
            Stopwatch watch = null;
            if (IsSqlLog)
            {
                watch = Stopwatch.StartNew();
            }
            return watch;
        }

        /// <summary>
        /// 增加SQL请求日志
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="watch">时间测量器</param>
        /// <param name="cmdParams">执行参数</param>
        private void AddQueryDetail(string commandText, Stopwatch watch, DbParameter[] cmdParams)
        {
            if (IsSqlLog)
            {
                watch.Stop();
                string sqltext = DbHelper.GetQueryDetail(commandText, watch.ElapsedMilliseconds, cmdParams, IsSqlLogHtml);
                Log.Debug(sqltext, LogPath + DbProviderName);
                Info(sqltext);
                //DbHelper.m_querydetail.Add();
            }
            if (this.QueryCount < ulong.MaxValue) this.QueryCount++;
        }

        private void Info(string sqltext)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation(sqltext);
            }
        }

        /// <summary>
        /// SQL执行完成后的日志生成
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="ElapsedMilliseconds">执行毫秒</param>
        /// <param name="cmdParams">执行参数</param>
        /// <param name="IsSqlLogHtml">打印模式</param>
        /// <returns>返回日志</returns>
        private static string GetQueryDetail(string commandText, long ElapsedMilliseconds, DbParameter[] cmdParams, bool IsSqlLogHtml)
        {
            if (IsSqlLogHtml)
            {
                const string text = "<tr style=\"background: rgb(255, 255, 255) none repeat scroll 0%; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">";
                StringBuilder text2 = new(), text3 = new(), text4 = new();
                string arg = "";
                if (cmdParams != null && cmdParams.Length > 0)
                {
                    for (int i = 0; i < cmdParams.Length; i++)
                    {
                        DbParameter dbParameter = cmdParams[i];
                        if (dbParameter != null)
                        {
                            text2.Append($"<td>{dbParameter.ParameterName}</td>");
                            text3.Append($"<td>{dbParameter.DbType}</td>");
                            text4.Append($"<td>{dbParameter.Value}</td>");
                        }
                    }
                    arg = string.Format("<table width=\"100%\" cellspacing=\"1\" cellpadding=\"0\" style=\"background: rgb(255, 255, 255) none repeat scroll 0%; margin-top: 5px; font-size: 12px; display: block; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">{0}{1}</tr>{0}{2}</tr>{0}{3}</tr></table>", new object[]
                    {
                        text,
                        text2,
                        text3,
                        text4
                    });
                }
                return string.Concat("<center><div style=\"border: 1px solid black; margin: 2px; padding: 1em; text-align: left; width: 96%; clear: both;\"><div style=\"font-size: 12px; float: right; width: 100px; margin-bottom: 5px;\"><b>TIME:</b> ", ElapsedMilliseconds, " 毫秒</div><span style=\"font-size: 12px;\">", commandText, arg, "</span></div><br /></center>");
            }
            else
            {
                StringBuilder sqllog = new();

                sqllog.AppendLine("SQL执行情况：");
                sqllog.Append("  耗时：").Append(ElapsedMilliseconds).AppendLine("毫秒");
                sqllog.Append("  执行命令：").AppendLine(commandText);

                if (cmdParams != null && cmdParams.Length > 0)
                {
                    sqllog.AppendLine("  执行参数：");
                    foreach (DbParameter cmdParam in cmdParams)
                    {
                        sqllog.AppendFormat("  参数名：{0}，类型：{1}，值：{2}{3}", cmdParam.ParameterName, cmdParam.DbType, cmdParam.Value, Environment.NewLine);

                        //sqllog.Append(cmdParam.ParameterName).Append(',').Append(cmdParam.DbType).Append(',').AppendLine(cmdParam.Value.ToString());
                    }
                }

                return sqllog.ToString(0, sqllog.Length - Environment.NewLine.Length);
            }
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
            if (this.ConnectionString == null || this.ConnectionString.Length == 0)
            {
                throw NullConnectionString;
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            DbParameter[] spParameterSetInternal;
            using (DbConnection dbConnection = CreateConnection())
            {
                dbConnection.ConnectionString = this.ConnectionString;
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
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            DbParameter[] spParameterSetInternal;
            using (DbConnection dbConnection = (DbConnection)((ICloneable)connection).Clone())
            {
                spParameterSetInternal = this.GetSpParameterSetInternal(dbConnection, spName, includeReturnValueParameter);
            }
            return spParameterSetInternal;
        }

        private DbParameter[] GetSpParameterSetInternal(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException(nameof(spName));
            }
            string key = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":包含ReturnValue参数" : "");
            if (this.m_paramcache[key] is not DbParameter[] array)
            {
                DbParameter[] array2 = this.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                this.m_paramcache[key] = array2;
                array = array2;
            }
            return CloneParameters(array);
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

        private static void PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (commandText == null || commandText.Length == 0)
            {
                throw new ArgumentNullException(nameof(commandText));
            }
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("事务被回滚或提交，请提供一个打开的事务。", "事物提交");
                }
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
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
            ds = this.ExecuteDataset(CommandType.StoredProcedure, procName, null);
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
            ds = this.ExecuteDataset(CommandType.StoredProcedure, procName, prams.ToArray());
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
            dbCommandBuilder.DataAdapter.SelectCommand.Connection = CreateConnection();
            dbCommandBuilder.DataAdapter.DeleteCommand.Connection = CreateConnection();
            dbCommandBuilder.DataAdapter.InsertCommand.Connection = CreateConnection();
            dbCommandBuilder.DataAdapter.UpdateCommand.Connection = CreateConnection();
            dbCommandBuilder.DataAdapter.SelectCommand.Connection.ConnectionString = this.ConnectionString;
            dbCommandBuilder.DataAdapter.DeleteCommand.Connection.ConnectionString = this.ConnectionString;
            dbCommandBuilder.DataAdapter.InsertCommand.Connection.ConnectionString = this.ConnectionString;
            dbCommandBuilder.DataAdapter.UpdateCommand.Connection.ConnectionString = this.ConnectionString;
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
            if (tableName == null || tableName.Length == 0)
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            using DbDataAdapter dbDataAdapter = CreateDataAdapter();
            dbDataAdapter.UpdateCommand = updateCommand ?? throw new ArgumentNullException(nameof(updateCommand));
            dbDataAdapter.InsertCommand = insertCommand ?? throw new ArgumentNullException(nameof(insertCommand));
            dbDataAdapter.DeleteCommand = deleteCommand ?? throw new ArgumentNullException(nameof(deleteCommand));
            dbDataAdapter.Update(dataSet, tableName);
            dataSet.AcceptChanges();
        }

        #endregion

        #region 私有变量

        private readonly object lockHelper = new object();

        /// <summary>
        /// 连接字符串
        /// </summary>
		private string m_connectionstring;

        /// <summary>
        /// 数据库类型对象
        /// </summary>
        private DbProviderType m_dbProviderType;

        /// <summary>
        /// 数据库类型对象
        /// </summary>
        private string m_dbProviderName;

        private DbProviderFactory m_factory;

        private readonly Hashtable m_paramcache = Hashtable.Synchronized(new Hashtable());

        private IDbProvider m_provider;

        /// <summary>
        /// 在开启数据库日志模式后，将日志打印至该路径下。
        /// </summary>
        public const string LogPath = "Log/Sql/";

        ///// <summary>
        ///// 程序运行过程中的所有增删改查操作的耗时详情
        ///// </summary>
        //private static List<string> m_querydetail = new List<string>();

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
