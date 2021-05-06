using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Tool.Utils.Data;

namespace Tool.SqlCore
{
    /// <summary>
    /// 对<see cref="DbHelper"/> 对象，提供扩展支持
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DbHelperExensions
    {
        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">携带的值<see cref="DbParameter"/>[]</param>
        /// <returns></returns>
        public static DataTable Select(this DbHelper dbHelper, string commandText, object prams = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new Exception("commandText 变量值不能为空。");
            }
            DataSet dataSet = dbHelper.Query(commandText, prams);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return default;
        }

        /// <summary>
        /// 查询多张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多条查询语句</param>
        /// <returns></returns>
        public static DataSet Select(this DbHelper dbHelper, params string[] commandTexts)
        {
            if (commandTexts == null || commandTexts.Length == 0)
            {
                throw new Exception("commandText 变量值不能为空。");
            }
            string _commandText = string.Join("; ", commandTexts);
            return dbHelper.Query(_commandText);
        }

        /// <summary>
        /// 插入一条结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="prams">携带的值<see cref="DbParameter"/>[]</param>
        /// <returns></returns>
        public static int Insert<T>(this DbHelper dbHelper, object prams) 
        {
            return dbHelper.Insert(typeof(T).Name, prams);
        }

        /// <summary>
        /// 插入一条结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="prams">携带的值<see cref="DbParameter"/>[]</param>
        /// <returns></returns>
        public static int Insert(this DbHelper dbHelper, string TableName, object prams)
        {
            Dictionary<string, object> keyValues = dbHelper.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！（传入的新增参数是空的）", "prams");
            }

            List<DbParameter> parms = dbHelper.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", TableName, key, value);
            return dbHelper.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入多条结果，可以是多张不同的表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多条插入语句</param>
        /// <returns></returns>
        public static int Insert(this DbHelper dbHelper, params string[] commandTexts)
        {
            return ExecuteNonQuery(dbHelper, commandTexts);
        }


        /// <summary>
        /// 修改单表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="where">修改的条件</param>
        /// <param name="prams">修改表的参数 Or 修改条件的参数</param>
        /// <returns></returns>
        public static int Update<T>(this DbHelper dbHelper, string where, params object[] prams)
        {
            return dbHelper.Update(typeof(T).Name, where, prams);
        }

        /// <summary>
        /// 修改单表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="where">修改的条件，无需写 WHERE 直接条件</param>
        /// <param name="prams">修改表的参数 Or 修改条件的参数</param>
        /// <returns></returns>
        public static int Update(this DbHelper dbHelper, string TableName, string where, params object[] prams)
        {
            if (prams == null || prams.Length == 0)
            {
                throw new ArgumentException("键值对集合为空！", "prams");
            }

            Dictionary<string, object> keyValues = dbHelper.SetDictionaryParam(prams[0]);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备修改表的时候发生异常，键值对集合为空！（传入的修改参数是空的）", "prams[0]");
            }

            List<DbParameter> parms = dbHelper.GetUpdateParams(keyValues, out string strsql);

            if (prams.Length > 1)
            {
                List<DbParameter> _parms = dbHelper.SetParameterList(prams[1]);
                if (_parms == null || _parms.Count == 0)
                {
                    throw new ArgumentException("在准备修改表的时候发生异常，键值对集合为空！（传入的条件参数是空的）", "prams[1]");
                }
                parms.AddRange(_parms);
            }

            string commandText = string.Format("UPDATE {0} SET {1} {2}", TableName, strsql, WhereStr(where));
            return dbHelper.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 修改多表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多条插入语句</param>
        /// <returns></returns>
        public static int Update(this DbHelper dbHelper, params string[] commandTexts)
        {
            return ExecuteNonQuery(dbHelper, commandTexts);
        }

        /// <summary>
        /// 删除单张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="where">删除的条件</param>
        /// <param name="prams">删除条件的参数</param>
        /// <returns></returns>
        public static int Delete<T>(this DbHelper dbHelper, string where, object prams)
        {
            return dbHelper.Delete(typeof(T).Name, where, prams);
        }

        /// <summary>
        /// 删除单张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="where">删除的条件</param>
        /// <param name="prams">删除条件的参数</param>
        /// <returns></returns>
        public static int Delete(this DbHelper dbHelper, string TableName, string where, object prams)
        {
             DbParameter[] parameters = dbHelper.SetParameters(prams);

            if (parameters == null || parameters.Length == 0)
            {
                throw new ArgumentException("在准备修改表的时候发生异常，prams 中无有效数据或为空！", "错误提示：");
            }

            string commandText = string.Format("DELETE FROM {0} {1}", TableName, WhereStr(where));
            return dbHelper.ExecuteNonQuery(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 删除多多张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多张表删除语句</param>
        /// <returns></returns>
        public static int Delete(this DbHelper dbHelper, params string[] commandTexts)
        {
            return ExecuteNonQuery(dbHelper,commandTexts);
        }

        /// <summary>
        /// 提供快捷方式，匿名对象实现
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">执行的SQL语句</param>
        /// <param name="prams">携带的参数可以是Null</param>
        /// <returns>返回<see cref="SqlTextParameter"/>对象</returns>
        public static SqlTextParameter GetTextParameter(this DbHelper dbHelper, string commandText, object prams) 
        {
            DbParameter[] _parameters = dbHelper.SetParameters(prams);
            return new SqlTextParameter(commandText, _parameters);
        }

        private static int ExecuteNonQuery(DbHelper dbHelper, params string[] commandTexts) 
        {
            if (commandTexts == null || commandTexts.Length == 0)
            {
                throw new Exception("commandText 变量值不能为空。");
            }
            string _commandText = string.Join("; ", commandTexts);
            return dbHelper.ExecuteNonQuery(CommandType.Text, _commandText, null);
        }

        /// <summary>
        /// 避免问题where 第一位不能为空格
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string WhereStr(string where)
        {
            if (string.IsNullOrWhiteSpace(where))
            {
                return "WHERE 1=1";
            }
            where = where.TrimStart();
            if (where.StartsWith("WHERE", StringComparison.OrdinalIgnoreCase) 
                || where.StartsWith("(NOLOCK)", StringComparison.OrdinalIgnoreCase)
                || where.StartsWith("WITH(", StringComparison.OrdinalIgnoreCase)
                || System.Text.RegularExpressions.Regex.IsMatch(where, "^(?=\\().*(?<=\\))", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return where;
            }
            else
            {
                return $"WHERE {where}";
            }
        }

        /// <summary>
        /// 根据键值对生成 Insert 部分语法，和值添加对象
        /// </summary>
        /// <param name="database">数据库引擎</param>
        /// <param name="keyValues">数据集键值对</param>
        /// <param name="key">返回生成的部分SQL语句</param>
        /// <param name="value">返回生成的部分SQL语句</param>
        /// <returns><see cref="List{DbParameter}"/></returns>
        public static List<DbParameter> GetInsertParams(this DbHelper database, Dictionary<string, object> keyValues, out string key, out string value)
        {
            StringBuilder _key = new StringBuilder(), _value = new StringBuilder();

            List<DbParameter> parms = new List<DbParameter>();

            foreach (KeyValuePair<string, object> keyValue in keyValues)
            {
                //_key.AppendFormat("[{0}],", keyValue.Key);

                switch (database.DbProviderType)
                {
                    case DbProviderType.SqlServer:
                        _key.Append('[').Append(keyValue.Key).Append("],");
                        break;
                    case DbProviderType.MySql:
                        _key.Append('`').Append(keyValue.Key).Append("`,");
                        break;
                    case DbProviderType.Oracle:
                        _key.Append('[').Append(keyValue.Key).Append("],");
                        break;
                    default:
                        _key.Append(keyValue.Key).Append(',');
                        break;
                }
                //_value.AppendFormat("@{0},", keyValue.Key);

                _value.Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');

                parms.Add(database.GetInParam(keyValue.Key, keyValue.Value));
            }

            key = _key.ToString(0, _key.Length - 1);
            value = _value.ToString(0, _value.Length - 1);
            return parms;
        }

        /// <summary>
        /// 根据键值对生成 Update 部分语法，和值添加对象
        /// </summary>
        /// <param name="database">数据库引擎</param>
        /// <param name="keyValues">数据集键值对</param>
        /// <param name="strsql">返回生成的部分SQL语句</param>
        /// <returns><see cref="List{DbParameter}"/></returns>
        public static List<DbParameter> GetUpdateParams(this DbHelper database, Dictionary<string, object> keyValues, out string strsql)
        {
            StringBuilder _value = new StringBuilder();

            List<DbParameter> parms = new List<DbParameter>();

            foreach (KeyValuePair<string, object> keyValue in keyValues)
            {
                //_value.AppendFormat("[{0}] = @{0},", keyValue.Key);
                switch (database.DbProviderType)
                {
                    case DbProviderType.SqlServer:
                        _value.Append('[').Append(keyValue.Key).Append(']').Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                        break;
                    case DbProviderType.MySql:
                        _value.Append('`').Append(keyValue.Key).Append('`').Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                        break;
                    case DbProviderType.Oracle:
                        _value.Append('[').Append(keyValue.Key).Append(']').Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                        break;
                    default:
                        _value.Append(keyValue.Key).Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                        break;
                }

                //_value.Append('[').Append(keyValue.Key).Append(']').Append('=').Append('@').Append(keyValue.Key).Append(',');

                parms.Add(database.GetInParam(keyValue.Key, keyValue.Value));
            }

            strsql = _value.ToString(0, _value.Length - 1);
            return parms;
        }

        /// <summary>
        /// 一个测试阶段的 实体转换函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<DbDataReader, T> GetReader<T>()
        {
            Delegate resDelegate;
            //if (!ExpressionCache.TryGetValue(typeof(T), out resDelegate))
            {
                // Get the indexer property of SqlDataReader 
                var indexerProperty = typeof(DbDataReader).GetProperty("Item",
                new[] { typeof(string) });
                // List of statements in our dynamic method 
                var statements = new List<System.Linq.Expressions.Expression>();
                // Instance type of target entity class 
                System.Linq.Expressions.ParameterExpression instanceParam = System.Linq.Expressions.Expression.Variable(typeof(T));
                // Parameter for the SqlDataReader object
                System.Linq.Expressions.ParameterExpression readerParam =
                     System.Linq.Expressions.Expression.Parameter(typeof(DbDataReader));

                // Create and assign new T to variable. Ex. var instance = new T(); 
                System.Linq.Expressions.BinaryExpression createInstance = System.Linq.Expressions.Expression.Assign(instanceParam,
                     System.Linq.Expressions.Expression.New(typeof(T)));
                statements.Add(createInstance);

                foreach (var property in typeof(T).GetProperties())
                {
                    // instance.Property 
                    System.Linq.Expressions.MemberExpression getProperty =
                     System.Linq.Expressions.Expression.Property(instanceParam, property);
                    // row[property] The assumption is, column names are the 
                    // same as PropertyInfo names of T 
                    System.Linq.Expressions.IndexExpression readValue =
                         System.Linq.Expressions.Expression.MakeIndex(readerParam, indexerProperty,
                        new[] { System.Linq.Expressions.Expression.Constant(property.Name) });

                    // 为属性赋值
                    System.Linq.Expressions.BinaryExpression assignProperty = System.Linq.Expressions.Expression.Assign(getProperty,
                        System.Linq.Expressions.Expression.Convert(readValue, property.PropertyType));

                    statements.Add(assignProperty);
                }
                var returnStatement = instanceParam;
                statements.Add(returnStatement);

                var body = System.Linq.Expressions.Expression.Block(instanceParam.Type,
                    new[] { instanceParam }, statements.ToArray());

                var lambda =
                 System.Linq.Expressions.Expression.Lambda<Func<DbDataReader, T>>(body, readerParam);
                resDelegate = lambda.Compile();

                // 将动态方法保存到缓存中
                //ExpressionCache[typeof(T)] = resDelegate;
            }
            return (Func<DbDataReader, T>)resDelegate;
        }
    }
}
