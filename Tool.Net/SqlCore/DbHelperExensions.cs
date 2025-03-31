using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;
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
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
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
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static IList<IDictionary<string, object>> SelectDictionary(this DbHelper dbHelper, string commandText, object prams = null)
        {
            return dbHelper.SelectDictionary(CommandType.Text, commandText, prams);
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="command">执行模式</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static IList<IDictionary<string, object>> SelectDictionary(this DbHelper dbHelper, CommandType command, string commandText, object prams = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new Exception("commandText 变量值不能为空。");
            }

            IDictionary<string, object> dic = dbHelper.SetDictionaryParam(prams);
            using var dataReader = dbHelper.ExecuteReader(command, commandText, dbHelper.SetParameterList(dic)?.ToArray());
            return dataReader.GetListHash();
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static System.Collections.ArrayList SelectArray(this DbHelper dbHelper, string commandText, object prams = null)
        {
            return dbHelper.SelectArray(CommandType.Text, commandText, prams);
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="command">执行模式</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static System.Collections.ArrayList SelectArray(this DbHelper dbHelper, CommandType command, string commandText, object prams = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new Exception("commandText 变量值不能为空。");
            }

            IDictionary<string, object> dic = dbHelper.SetDictionaryParam(prams);
            using var dataReader = dbHelper.ExecuteReader(command, commandText, dbHelper.SetParameterList(dic)?.ToArray());
            return dataReader.GetReaderArray();
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="prams">查询条件</param>
        /// <returns></returns>
        public static IList<T> Select<T>(this DbHelper dbHelper, Action<T> prams = null) where T : new()
        {
            string where;
            var dic = default(IDictionary<string, object>);
            if (prams != null)
            {
                var t = new T();

                dic = t.GetDictionary();

                prams?.Invoke(t);

                var dic1 = t.GetDictionary();

                StringBuilder @string = new("1=1");

                foreach (var pair in dic1)
                {
                    if (pair.Value == null)
                    {
                        dic.Remove(pair.Key);
                        continue;
                    }
                    if (pair.Value.Equals(dic[pair.Key]))
                    {
                        dic.Remove(pair.Key);
                        continue;
                    }
                    dic[pair.Key] = pair.Value;

                    switch (dbHelper.DbProviderType)
                    {
                        case DbProviderType.Oracle:
                        case DbProviderType.SqlServer:
                        case DbProviderType.MySql:
                            @string.AppendFormat(" AND {0}={1}{2}", pair.Key, dbHelper.Provider.ParameterPrefix, pair.Key);
                            break;
                        case DbProviderType.OleDb:
                            @string.AppendFormat(" AND {0}=?", pair.Key);
                            break;
                        default:
                            @string.Append(pair.Key).Append('=').Append(dbHelper.Provider.ParameterPrefix).Append(pair.Key).Append(',');
                            break;
                    }
                }

                dic1.Clear();

                where = @string.ToString();
            }
            else
            {
                where = "1=1";
            }

            string sql = $"SELECT * FROM {typeof(T).Name} WHERE {where}";
            using DataSet dataSet = dbHelper.ExecuteDataSet(commandType: CommandType.Text, sql, dbHelper.SetParameterList(dic)?.ToArray());
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0].ToEntityList<T>();
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
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static int Insert<T>(this DbHelper dbHelper, object prams) where T : new()
        {
            return dbHelper.Insert(typeof(T).Name, prams);
        }

        /// <summary>
        /// 插入一条结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static int Insert(this DbHelper dbHelper, string TableName, object prams)
        {
            IDictionary<string, object> keyValues = dbHelper.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！（传入的新增参数是空的）", nameof(prams));
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
        /// <param name="prams">修改表的参数 Or 修改条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static int Update<T>(this DbHelper dbHelper, string where, params object[] prams) where T : new()
        {
            return dbHelper.Update(typeof(T).Name, where, prams);
        }

        /// <summary>
        /// 修改单表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="where">修改的条件，无需写 WHERE 直接条件</param>
        /// <param name="prams">修改表的参数 Or 修改条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static int Update(this DbHelper dbHelper, string TableName, string where, params object[] prams)
        {
            if (prams == null || prams.Length == 0)
            {
                throw new ArgumentException("键值对集合为空！", nameof(prams));
            }

            IDictionary<string, object> keyValues = dbHelper.SetDictionaryParam(prams[0]);

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
        /// <param name="prams">删除条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static int Delete<T>(this DbHelper dbHelper, string where, object prams) where T : new()
        {
            return dbHelper.Delete(typeof(T).Name, where, prams);
        }

        /// <summary>
        /// 删除单张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="where">删除的条件</param>
        /// <param name="prams">删除条件的参数,可以是任何类型的键值对</param>
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
            return ExecuteNonQuery(dbHelper, commandTexts);
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static async Task<DataTable> SelectAsync(this DbHelper dbHelper, string commandText, object prams = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new Exception("commandText 变量值不能为空。");
            }
            DataSet dataSet = await dbHelper.QueryAsync(commandText, prams);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return default;
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static async Task<IList<IDictionary<string, object>>> SelectDictionaryAsync(this DbHelper dbHelper, string commandText, object prams = null)
        {
            return await dbHelper.SelectDictionaryAsync(CommandType.Text, commandText, prams);
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="command">执行模式</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static async Task<IList<IDictionary<string, object>>> SelectDictionaryAsync(this DbHelper dbHelper, CommandType command, string commandText, object prams = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new Exception("commandText 变量值不能为空。");
            }

            IDictionary<string, object> dic = dbHelper.SetDictionaryParam(prams);
            using var dataReader = await dbHelper.ExecuteReaderAsync(command, commandText, dbHelper.SetParameterList(dic)?.ToArray());
            return await dataReader.GetListHashAsync();
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static async Task<System.Collections.ArrayList> SelectArrayAsync(this DbHelper dbHelper, string commandText, object prams = null)
        {
            return await dbHelper.SelectArrayAsync(CommandType.Text, commandText, prams);
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="command">执行模式</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static async Task<System.Collections.ArrayList> SelectArrayAsync(this DbHelper dbHelper, CommandType command, string commandText, object prams = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new Exception("commandText 变量值不能为空。");
            }

            IDictionary<string, object> dic = dbHelper.SetDictionaryParam(prams);
            using var dataReader = await dbHelper.ExecuteReaderAsync(command, commandText, dbHelper.SetParameterList(dic)?.ToArray());
            return await dataReader.GetReaderArrayAsync();
        }

        /// <summary>
        /// 查询单张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="prams">查询条件</param>
        /// <returns></returns>
        public static async Task<IList<T>> SelectAsync<T>(this DbHelper dbHelper, Action<T> prams = null) where T : new()
        {
            string where;
            var dic = default(IDictionary<string, object>);
            if (prams != null)
            {
                var t = new T();

                dic = t.GetDictionary();

                prams?.Invoke(t);

                var dic1 = t.GetDictionary();

                StringBuilder @string = new("1=1");

                foreach (var pair in dic1)
                {
                    if (pair.Value == null)
                    {
                        dic.Remove(pair.Key);
                        continue;
                    }
                    if (pair.Value.Equals(dic[pair.Key]))
                    {
                        dic.Remove(pair.Key);
                        continue;
                    }
                    dic[pair.Key] = pair.Value;

                    switch (dbHelper.DbProviderType)
                    {
                        case DbProviderType.Oracle:
                        case DbProviderType.SqlServer:
                        case DbProviderType.MySql:
                            @string.AppendFormat(" AND {0}={1}{2}", pair.Key, dbHelper.Provider.ParameterPrefix, pair.Key);
                            break;
                        case DbProviderType.OleDb:
                            @string.AppendFormat(" AND {0}=?", pair.Key);
                            break;
                        default:
                            @string.Append(pair.Key).Append('=').Append(dbHelper.Provider.ParameterPrefix).Append(pair.Key).Append(',');
                            break;
                    }
                }

                dic1.Clear();

                where = @string.ToString();
            }
            else
            {
                where = "1=1";
            }

            string sql = $"SELECT * FROM {typeof(T).Name} WHERE {where}";
            using DataSet dataSet = await dbHelper.ExecuteDataSetAsync(commandType: CommandType.Text, sql, dbHelper.SetParameterList(dic)?.ToArray());
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0].ToEntityList<T>();
            }
            return default;
        }

        /// <summary>
        /// 查询多张表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多条查询语句</param>
        /// <returns></returns>
        public static Task<DataSet> SelectAsync(this DbHelper dbHelper, params string[] commandTexts)
        {
            if (commandTexts == null || commandTexts.Length == 0)
            {
                throw new Exception("commandText 变量值不能为空。");
            }
            string _commandText = string.Join("; ", commandTexts);
            return dbHelper.QueryAsync(_commandText);
        }

        /// <summary>
        /// 插入一条结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static Task<int> InsertAsync<T>(this DbHelper dbHelper, object prams) where T : new()
        {
            return dbHelper.InsertAsync(typeof(T).Name, prams);
        }

        /// <summary>
        /// 插入一条结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="prams">实体类，虚构对象,任何类型的键值对</param>
        /// <returns></returns>
        public static Task<int> InsertAsync(this DbHelper dbHelper, string TableName, object prams)
        {
            IDictionary<string, object> keyValues = dbHelper.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！（传入的新增参数是空的）", nameof(prams));
            }

            List<DbParameter> parms = dbHelper.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", TableName, key, value);
            return dbHelper.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入多条结果，可以是多张不同的表
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多条插入语句</param>
        /// <returns></returns>
        public static Task<int> InsertAsync(this DbHelper dbHelper, params string[] commandTexts)
        {
            return ExecuteNonQueryAsync(dbHelper, commandTexts);
        }


        /// <summary>
        /// 修改单表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="where">修改的条件</param>
        /// <param name="prams">修改表的参数 Or 修改条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static Task<int> UpdateAsync<T>(this DbHelper dbHelper, string where, params object[] prams) where T : new()
        {
            return dbHelper.UpdateAsync(typeof(T).Name, where, prams);
        }

        /// <summary>
        /// 修改单表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="where">修改的条件，无需写 WHERE 直接条件</param>
        /// <param name="prams">修改表的参数 Or 修改条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static Task<int> UpdateAsync(this DbHelper dbHelper, string TableName, string where, params object[] prams)
        {
            if (prams == null || prams.Length == 0)
            {
                throw new ArgumentException("键值对集合为空！", nameof(prams));
            }

            IDictionary<string, object> keyValues = dbHelper.SetDictionaryParam(prams[0]);

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
            return dbHelper.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 修改多表结果
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多条插入语句</param>
        /// <returns></returns>
        public static Task<int> UpdateAsync(this DbHelper dbHelper, params string[] commandTexts)
        {
            return ExecuteNonQueryAsync(dbHelper, commandTexts);
        }

        /// <summary>
        /// 删除单张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="where">删除的条件</param>
        /// <param name="prams">删除条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static Task<int> DeleteAsync<T>(this DbHelper dbHelper, string where, object prams) where T : new()
        {
            return dbHelper.DeleteAsync(typeof(T).Name, where, prams);
        }

        /// <summary>
        /// 删除单张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="TableName">表名</param>
        /// <param name="where">删除的条件</param>
        /// <param name="prams">删除条件的参数,可以是任何类型的键值对</param>
        /// <returns></returns>
        public static Task<int> DeleteAsync(this DbHelper dbHelper, string TableName, string where, object prams)
        {
            DbParameter[] parameters = dbHelper.SetParameters(prams);

            if (parameters == null || parameters.Length == 0)
            {
                throw new ArgumentException("在准备修改表的时候发生异常，prams 中无有效数据或为空！", "错误提示：");
            }

            string commandText = string.Format("DELETE FROM {0} {1}", TableName, WhereStr(where));
            return dbHelper.ExecuteNonQueryAsync(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 删除多多张表数据
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandTexts">多张表删除语句</param>
        /// <returns></returns>
        public static Task<int> DeleteAsync(this DbHelper dbHelper, params string[] commandTexts)
        {
            return ExecuteNonQueryAsync(dbHelper, commandTexts);
        }

        /// <summary>
        /// 提供快捷方式，匿名对象实现
        /// </summary>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">执行的SQL语句</param>
        /// <param name="prams">携带的参数可以是Null,可以是任何类型的键值对</param>
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

        private static Task<int> ExecuteNonQueryAsync(DbHelper dbHelper, params string[] commandTexts)
        {
            if (commandTexts == null || commandTexts.Length == 0)
            {
                throw new Exception("commandText 变量值不能为空。");
            }
            string _commandText = string.Join("; ", commandTexts);
            return dbHelper.ExecuteNonQueryAsync(CommandType.Text, _commandText, null);
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
                || Validate.regex_SqlWhere.IsMatch(where))
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
        public static List<DbParameter> GetInsertParams(this DbHelper database, IDictionary<string, object> keyValues, out string key, out string value)
        {
            List<DbParameter> parms = database.Provider.GetInsertParams(database, keyValues, out key, out value) ?? new();
            if (parms.Count == 0)
            {
                StringBuilder _key = new(), _value = new();

                foreach (KeyValuePair<string, object> keyValue in keyValues)
                {
                    if (keyValue.Value == null) continue;
                    switch (database.DbProviderType)
                    {
                        case DbProviderType.Oracle:
                        case DbProviderType.SqlServer:
                        case DbProviderType.SqlServer1:
                        case DbProviderType.OleDb:
                            _key.Append('[').Append(keyValue.Key).Append("],");
                            break;
                        case DbProviderType.MySql:
                            _key.Append('`').Append(keyValue.Key).Append("`,");
                            break;
                        default:
                            _key.Append(keyValue.Key).Append(',');
                            break;
                    }
                    switch (database.DbProviderType)
                    {
                        case DbProviderType.OleDb:
                            _value.Append('?').Append(',');
                            break;
                        default:
                            _value.Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                            break;
                    }

                    parms.Add(database.GetInParam(keyValue.Key, keyValue.Value));
                }

                key = _key.ToString(0, _key.Length - 1);
                value = _value.ToString(0, _value.Length - 1);
            }
            return parms;
        }

        /// <summary>
        /// 根据键值对生成 Update 部分语法，和值添加对象
        /// </summary>
        /// <param name="database">数据库引擎</param>
        /// <param name="keyValues">数据集键值对</param>
        /// <param name="strsql">返回生成的部分SQL语句</param>
        /// <returns><see cref="List{DbParameter}"/></returns>
        public static List<DbParameter> GetUpdateParams(this DbHelper database, IDictionary<string, object> keyValues, out string strsql)
        {
            List<DbParameter> parms = database.Provider.GetUpdateParams(database, keyValues, out strsql) ?? new();
            if (parms.Count == 0)
            {
                StringBuilder _value = new();

                foreach (KeyValuePair<string, object> keyValue in keyValues)
                {
                    if (keyValue.Value == null) continue;
                    switch (database.DbProviderType)
                    {
                        case DbProviderType.Oracle:
                        case DbProviderType.SqlServer:
                        case DbProviderType.SqlServer1:
                            _value.Append('[').Append(keyValue.Key).Append(']').Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                            break;
                        case DbProviderType.MySql:
                            _value.Append('`').Append(keyValue.Key).Append('`').Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                            break;
                        case DbProviderType.OleDb:
                            _value.Append('[').Append(keyValue.Key).Append(']').Append('=').Append('?').Append(',');
                            break;
                        default:
                            _value.Append(keyValue.Key).Append('=').Append(database.Provider.ParameterPrefix).Append(keyValue.Key).Append(',');
                            break;
                    }
                    parms.Add(database.GetInParam(keyValue.Key, keyValue.Value));
                }

                strsql = _value.ToString(0, _value.Length - 1);
            }
            return parms;
        }

        /// <summary>
        /// 一个测试阶段的 实体转换函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<DbDataReader, T> GetReader<T>()
        {
            Func<DbDataReader, T> resDelegate;
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
            return resDelegate;
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为集合字典
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <param name="isnull">是否处理Null值，true时将不包含在字典中</param>
        /// <returns>返回可读集合字典</returns>
        public static IList<IDictionary<string, object>> GetListHash(this DbDataReader dataReader, bool isnull = false)
        {
            //if (dataReader.IsClosed)
            {
                List<IDictionary<string, object>> keys = new();
                while (dataReader.Read())
                {
                    Dictionary<string, object> pairs = new(dataReader.FieldCount, StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        bool isNull = dataReader.IsDBNull(i);

                        if (isNull && !isnull)
                        {
                            pairs.Add(dataReader.GetName(i), null);
                        }
                        else
                        {
                            pairs.Add(dataReader.GetName(i), dataReader.GetValue(i));
                        }
                    }
                    keys.Add(pairs);
                }

                return keys;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为集合字典
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <param name="isnull">是否处理Null值，true时将不包含在字典中</param>
        /// <returns>返回可读集合字典</returns>
        public static async Task<IList<IDictionary<string, object>>> GetListHashAsync(this DbDataReader dataReader, bool isnull = false)
        {
            //if (dataReader.IsClosed)
            {
                List<IDictionary<string, object>> keys = new();
                while (await dataReader.ReadAsync())
                {
                    Dictionary<string, object> pairs = new(dataReader.FieldCount, StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        bool isNull = await dataReader.IsDBNullAsync(i);

                        if (isNull && !isnull)
                        {
                            pairs.Add(dataReader.GetName(i), null);
                        }
                        else
                        {
                            pairs.Add(dataReader.GetName(i), dataReader.GetValue(i));
                        }
                    }
                    keys.Add(pairs);
                }

                return keys;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为 二维数组集合
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <param name="isnull">是否处理Null值，true时将不包含在二维数组中</param>
        /// <returns>返回可读二维数组集合</returns>
        public static System.Collections.ArrayList GetReaderArray(this DbDataReader dataReader, bool isnull = false)
        {
            //if (dataReader.IsClosed)
            {
                System.Collections.ArrayList keys = new();
                while (dataReader.Read())
                {
                    object[] objects = new object[dataReader.FieldCount];
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        bool isNull = dataReader.IsDBNull(i);
                        if (isNull && !isnull)
                        {
                            objects[i] = null;
                        }
                        else
                        {
                            objects[i] = dataReader.GetValue(i);
                        }
                    }
                    keys.Add(objects);
                }
                keys.Insert(0, GetEmptyDictionaryKey(dataReader));
                return keys;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为 二维数组集合
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <param name="isnull">是否处理Null值，true时将不包含在二维数组中</param>
        /// <returns>返回可读二维数组集合</returns>
        public static async Task<System.Collections.ArrayList> GetReaderArrayAsync(this DbDataReader dataReader, bool isnull = false)
        {
            //if (dataReader.IsClosed)
            {
                System.Collections.ArrayList keys = new();
                while (await dataReader.ReadAsync())
                {
                    object[] objects = new object[dataReader.FieldCount];
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        bool isNull = await dataReader.IsDBNullAsync(i);
                        if (isNull && !isnull)
                        {
                            objects[i] = null;
                        }
                        else
                        {
                            objects[i] = dataReader.GetValue(i);
                        }
                    }
                    keys.Add(objects);
                }
                keys.Insert(0, GetEmptyDictionaryKey(dataReader));
                return keys;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为 <see cref="DataSet"/>
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <returns>返回可读集合字典</returns>
        public static DataSet GetDataSet(this DbDataReader dataReader)
        {
            //if (dataReader.IsClosed)
            {
                DataSet dataSet = new();
                do
                {
                    dataSet.Tables.Add(dataReader.GetDataTable());
                } while (dataReader.NextResult());
                return dataSet;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为 <see cref="DataTable"/>
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <returns>返回可读集合字典</returns>
        public static DataTable GetDataTable(this DbDataReader dataReader)
        {
            //if (dataReader.IsClosed)
            {
                DataTable dataTable = dataReader.GetDataColumnSchema();
                while (dataReader.Read())
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        bool isNull = dataReader.IsDBNull(i);
                        dataRow[i] = isNull ? DBNull.Value : dataReader.GetValue(i);//dataReader.GetName(i)
                    }
                    dataTable.Rows.Add(dataRow);
                }
                return dataTable;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为 <see cref="DataSet"/>
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <returns>返回可读集合字典</returns>
        public static async Task<DataSet> GetDataSetAsync(this DbDataReader dataReader)
        {
            //if (dataReader.IsClosed)
            {
                DataSet dataSet = new();
                do
                {
                    dataSet.Tables.Add(await dataReader.GetDataTableAsync());
                } while (await dataReader.NextResultAsync());
                return dataSet;
            }
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>对象，中数据转换为 <see cref="DataTable"/>
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <returns>返回可读集合字典</returns>
        public static async Task<DataTable> GetDataTableAsync(this DbDataReader dataReader)
        {
            //if (dataReader.IsClosed)
            {
                DataTable dataTable = await dataReader.GetDataColumnSchemaAsync();
                while (await dataReader.ReadAsync())
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        bool isNull = await dataReader.IsDBNullAsync(i);
                        dataRow[i] = isNull ? DBNull.Value : dataReader.GetValue(i);//dataReader.GetName(i)
                    }
                    dataTable.Rows.Add(dataRow);
                }
                return dataTable;
            }
        }

        /// <summary>
        /// 获取DataTable表信息不含数据
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <returns></returns>
        public static DataTable GetDataColumnSchema(this DbDataReader dataReader)
        {
            return GetEmptyDataTable(dataReader.GetColumnSchema());
        }

        /// <summary>
        /// 获取DataTable表信息不含数据
        /// </summary>
        /// <param name="dataReader">原数据对象</param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataColumnSchemaAsync(this DbDataReader dataReader)
        {
            return GetEmptyDataTable(await dataReader.GetColumnSchemaAsync());
        }

        private static DataTable GetEmptyDataTable(ReadOnlyCollection<DbColumn> columns)
        {
            DataTable dataTable = new();
            foreach (DbColumn column in columns)
            {
                dataTable.Columns.Add(new DataColumn
                {
                    AllowDBNull = column.AllowDBNull ?? true,
                    DataType = column.DataType,
                    ColumnName = column.ColumnName,
                    Unique = column.IsUnique ?? false,
                    ReadOnly = column.IsReadOnly == false,
                });
            }
            return dataTable;
        }

        private static Dictionary<string, string> GetEmptyDictionaryKey(DbDataReader dataReader)
        {
            Dictionary<string, string> keys = new(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                keys.Add(dataReader.GetName(i), dataReader.GetFieldType(i).FullName);
            }
            return keys;
        }
    }

#if NET6_0_OR_GREATER

    /// <summary>
    /// 对<see cref="DbBatch"/> 对象，提供扩展支持
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DbBatchExensions
    {
        /// <summary>
        /// 创建一个新的框架内置<see cref="DiyDbBatch"/>
        /// </summary>
        /// <param name="dbHelper">数据db</param>
        /// <returns>返回<see cref="DiyDbBatch"/></returns>
        /// <exception cref="Exception">执行脚本为空！</exception>
        public static DiyDbBatch NewDbBatch(this DbHelper dbHelper)
        {
            if (dbHelper is null) throw new Exception("dbHelper 变量值不能为空。");
            return new DiyDbBatch(dbHelper);
        }

        /// <summary>
        /// 创建一个新的框架内置<see cref="DiyDbBatch"/> 带事务版本
        /// </summary>
        /// <param name="dbHelper">数据db</param>
        /// <param name="isolationLevel">指定事务类型</param>
        /// <returns>返回<see cref="DiyDbBatch"/></returns>
        /// <exception cref="Exception">执行脚本为空！</exception>
        public static DiyDbBatch NewDbBatch(this DbHelper dbHelper, IsolationLevel isolationLevel)
        {
            if (dbHelper is null) throw new Exception("dbHelper 变量值不能为空。");
            return new DiyDbBatch(dbHelper, isolationLevel);
        }

        /// <summary>
        /// 对批量执行sql提供参数化支持
        /// </summary>
        /// <param name="dbHelper">数据db</param>
        /// <param name="dbBatch">批量操作对象</param>
        /// <param name="command">命令模式</param>
        /// <param name="commandText">脚本</param>
        /// <param name="prams">可能包含的参数</param>
        /// <returns>返回<see cref="DbBatch"/></returns>
        /// <exception cref="Exception">执行脚本为空！</exception>
        public static DbBatch AddDbBatchCommand(this DbHelper dbHelper, DbBatch dbBatch, CommandType command, string commandText, object prams = null)
        {
            if (dbHelper is null) throw new Exception("dbHelper 变量值不能为空。");
            IDictionary<string, object> dic = dbHelper.SetDictionaryParam(prams);
            return dbBatch.AddDbBatchCommand(command, commandText, dbHelper.SetParameterList(dic)?.ToArray());
        }

        /// <summary>
        /// 对批量执行sql提供参数化支持
        /// </summary>
        /// <param name="dbBatch">批量操作对象</param>
        /// <param name="command">命令模式</param>
        /// <param name="commandText">脚本</param>
        /// <param name="commandParameters">可能包含的参数</param>
        /// <returns>返回<see cref="DbBatch"/></returns>
        /// <exception cref="Exception">执行脚本为空！</exception>
        public static DbBatch AddDbBatchCommand(this DbBatch dbBatch, CommandType command, string commandText, DbParameter[] commandParameters)
        {
            if (dbBatch is null) throw new Exception("dbBatch 变量值不能为空。");
            if (string.IsNullOrWhiteSpace(commandText)) throw new Exception("commandText 变量值不能为空。");
            var batchCommand = dbBatch.CreateBatchCommand();
            batchCommand.CommandText = commandText;
            batchCommand.CommandType = command;
            batchCommand.AttachParameters(commandParameters);
            dbBatch.BatchCommands.Add(batchCommand);
            return dbBatch;
        }

        /// <summary>
        /// 附加参数（批量模式）
        /// </summary>
        /// <param name="command">SQL数据对象基类</param>
        /// <param name="commandParameters">要附加的参数对象数组</param>
        public static void AttachParameters(this DbBatchCommand command, DbParameter[] commandParameters)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
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
            }
        }
    }

    /// <summary>
    /// 框架内置Db批处理公共模型
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DiyDbBatch : IDisposable, IAsyncDisposable
    {
        private bool disposedValue;
        private bool isPrepare;

        private readonly IsolationLevel isolationLevel;
        private readonly bool isTransaction;
        private DbBatch dbBatch;
        private DbHelper dbHelper;

        /// <summary>
        /// 创建批处理DB
        /// </summary>
        /// <param name="dbHelper">数据中心</param>
        public DiyDbBatch(DbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            this.isTransaction = false;

            var batch = dbHelper.CreateBatch();
            var connection = dbHelper.NewDbConnection();
            batch.Connection = connection;
            dbBatch = batch;
        }

        /// <summary>
        /// 创建批处理DB带事务
        /// </summary>
        /// <param name="dbHelper">数据中心</param>
        /// <param name="isolationLevel">事务类型</param>
        public DiyDbBatch(DbHelper dbHelper, IsolationLevel isolationLevel) : this(dbHelper)
        {
            this.isTransaction = true;
            this.isolationLevel = isolationLevel;
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get => dbBatch.Timeout; set => dbBatch.Timeout = value; }

        /// <summary>
        /// 批处理任务集合
        /// </summary>
        public DbBatchCommandCollection DbBatchCommands => dbBatch.BatchCommands;

        /// <summary>
        /// 连接对象
        /// </summary>
        public DbConnection Connection => dbBatch.Connection;

        /// <summary>
        /// 事务对象
        /// </summary>
        public DbTransaction Transaction => dbBatch.Transaction;

        /// <summary>
        /// 创建新的批处理任务
        /// </summary>
        /// <returns></returns>
        public DbBatchCommand CreateDbBatchCommand() => dbBatch.CreateBatchCommand();

        /// <summary>
        /// 对批量执行sql提供参数化支持
        /// </summary>
        /// <param name="command">命令模式</param>
        /// <param name="commandText">脚本</param>
        /// <param name="prams">可能包含的参数</param>
        /// <exception cref="Exception">执行脚本为空！</exception>
        public void AddDbBatchCommand(CommandType command, string commandText, object prams = null)
        {
            dbHelper.AddDbBatchCommand(dbBatch, command, commandText, prams);
        }

        /// <summary>
        /// 对批量执行sql提供参数化支持
        /// </summary>
        /// <param name="command">命令模式</param>
        /// <param name="commandText">脚本</param>
        /// <param name="commandParameters">可能包含的参数</param>
        /// <returns>返回<see cref="DbBatch"/></returns>
        /// <exception cref="Exception">执行脚本为空！</exception>
        public void AddDbBatchCommand(CommandType command, string commandText, DbParameter[] commandParameters)
        {
            dbHelper.AddDbBatchCommand(dbBatch, command, commandText, commandParameters);
        }

        private Stopwatch Prepare()
        {
            if (isPrepare) throw new Exception("无法重复调用！");
            isPrepare = true;
            Stopwatch watch = dbHelper.GetStopwatch();
            Connection.Open();
            if (isTransaction)
            {
                dbBatch.Transaction = Connection.BeginTransaction(isolationLevel);
            }
            dbBatch.Prepare();
            return watch;
        }

        private async Task<Stopwatch> PrepareAsync(CancellationToken cancellationToken = default)
        {
            if (isPrepare) throw new Exception("无法重复调用！");
            isPrepare = true;
            Stopwatch watch = dbHelper.GetStopwatch();
            await Connection.OpenAsync(cancellationToken);
            if (isTransaction)
            {
                dbBatch.Transaction = await Connection.BeginTransactionAsync(isolationLevel, cancellationToken);
            }
            await dbBatch.PrepareAsync(cancellationToken);
            return watch;
        }

        /// <summary>
        /// 取消批处理任务
        /// </summary>
        public void Cancel() => dbBatch.Cancel();

        /// <summary>
        /// 执行批处理任务并返回受影响行数
        /// </summary>
        /// <returns>返回受影响行数</returns>
        public int ExecuteNonQuery()
        {
            Stopwatch watch = null;
            bool iserror = false;
            string guid = string.Empty;
            try
            {
                watch = Prepare();
                return dbBatch.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw dbHelper.GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                if (watch is not null) dbHelper.AddQueryDetail(DbBatchCommands, watch, iserror, guid);
            }
        }

        /// <summary>
        /// 执行批处理任务并返回受影响行数（异步）
        /// </summary>
        /// <param name="cancellationToken">可取消</param>
        /// <returns>返回受影响行数</returns>
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
        {
            Stopwatch watch = null;
            bool iserror = false;
            string guid = string.Empty;
            try
            {
                watch = await PrepareAsync(cancellationToken);
                return await dbBatch.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw dbHelper.GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                if (watch is not null) dbHelper.AddQueryDetail(DbBatchCommands, watch, iserror, guid);
            }
        }

        /// <summary>
        /// 执行批处理任务并返回第一行的第一列
        /// </summary>
        /// <returns>返回第一行的第一列</returns>
        public object ExecuteScalar()
        {
            Stopwatch watch = null;
            bool iserror = false;
            string guid = string.Empty;
            try
            {
                watch = Prepare();
                return dbBatch.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw dbHelper.GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                if (watch is not null) dbHelper.AddQueryDetail(DbBatchCommands, watch, iserror, guid);
            }
        }

        /// <summary>
        /// 执行批处理任务并返回第一行的第一列（异步）
        /// </summary>
        /// <param name="cancellationToken">可取消</param>
        /// <returns>返回第一行的第一列</returns>
        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken = default)
        {
            Stopwatch watch = null;
            bool iserror = false;
            string guid = string.Empty;
            try
            {
                watch = await PrepareAsync(cancellationToken);
                return await dbBatch.ExecuteScalarAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw dbHelper.GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                if (watch is not null) dbHelper.AddQueryDetail(DbBatchCommands, watch, iserror, guid);
            }
        }

        /// <summary>
        /// 执行批处理任务并返回<see cref="DbDataReader"/>
        /// </summary>
        /// <param name="behavior">提供查询结果及其对数据库影响的描述。</param>
        /// <returns><see cref="DbDataReader"/></returns>
        public DbDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        {
            Stopwatch watch = null;
            bool iserror = false;
            string guid = string.Empty;
            try
            {
                watch = Prepare();
                return dbBatch.ExecuteReader(behavior);
            }
            catch (Exception ex)
            {
                throw dbHelper.GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                if (watch is not null) dbHelper.AddQueryDetail(DbBatchCommands, watch, iserror, guid);
            }
        }

        /// <summary>
        /// 执行批处理任务并返回<see cref="DbDataReader"/>（异步）
        /// </summary>
        /// <param name="behavior">提供查询结果及其对数据库影响的描述。</param>
        /// <param name="cancellationToken">可取消</param>
        /// <returns><see cref="DbDataReader"/></returns>
        public async Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior = CommandBehavior.Default, CancellationToken cancellationToken = default)
        {
            Stopwatch watch = null;
            bool iserror = false;
            string guid = string.Empty;
            try
            {
                watch = await PrepareAsync(cancellationToken);
                return await dbBatch.ExecuteReaderAsync(behavior, cancellationToken);
            }
            catch (Exception ex)
            {
                throw dbHelper.GetException(ex, ref iserror, ref guid);
            }
            finally
            {
                if (watch is not null) dbHelper.AddQueryDetail(DbBatchCommands, watch, iserror, guid);
            }
        }

        /// <summary>
        /// 回收相关资源
        /// </summary>
        public void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                dbHelper = null;
                Connection.Dispose();
                Transaction?.Dispose();
                dbBatch.Dispose();
                dbBatch = null;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 回收相关资源（异步）
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                dbHelper = null;
                await Connection.DisposeAsync();
                if (Transaction is not null) await Transaction.DisposeAsync();
                await dbBatch.DisposeAsync();
                dbBatch = null;
                GC.SuppressFinalize(this);
            }
        }
    }
#endif

    /// <summary>
    /// 异步返回带有主键的影响信息
    /// </summary>
    public record SqlNonQuery
    {
        /// <summary>
        /// 影响行数
        /// </summary>
        public int RowsCount { get; set; }

        /// <summary>
        /// 主键Id
        /// </summary>
        public object Id { get; set; }
    }
}
