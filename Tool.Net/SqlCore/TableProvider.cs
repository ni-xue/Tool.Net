using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Utils.Data;

namespace Tool.SqlCore
{
    /// <summary>
    /// 单表操作对象，系统提供，如果有特殊需求，建议重新实现接口完成。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TableProvider : ITableProvider
    {
        private readonly ArgumentException NullException;

        /// <summary>
        /// 核心数据对象
        /// </summary>
        public DbHelper Database { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 无参构造，接口自动实现
        /// </summary>
        public TableProvider()
        {
            NullException = new("在准备修改表的时候发生异常，键值对集合为空！", "错误提示：");
        }

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="database">数据源对象</param>
        /// <param name="tableName">表名</param>
        public TableProvider(DbHelper database, string tableName): this()
        {
            this.Database = database;
            this.TableName = tableName;
        }

        ///// <summary>
        ///// 有参构造
        ///// </summary>
        ///// <param name="connectionString">Sql 链接字符串</param>
        ///// <param name="tableName">表名</param>
        //public TableProvider(string connectionString, string tableName) : base(connectionString)
        //{
        //    this.TableName = "";
        //    this.TableName = tableName;
        //}

        /// <summary>
        /// 批量提交数据
        /// </summary>
        /// <param name="dataSet"><see cref="DataSet"/>对象</param>
        /// <param name="columnMapArray">列映射数组</param>
        public void BatchCommitData(DataSet dataSet, string[][] columnMapArray)
        {
            this.BatchCommitData(dataSet.Tables[0], columnMapArray);
        }

        /// <summary>
        /// 批量提交数据
        /// </summary>
        /// <param name="table"><see cref="DataTable"/>对象</param>
        /// <param name="columnMapArray">列映射数组</param>
        public void BatchCommitData(DataTable table, string[][] columnMapArray)
        {
            throw new Exception("未实现！");

            //using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(base.Database.ConnectionString))
            //{
            //    sqlBulkCopy.DestinationTableName = this.TableName;
            //    for (int i = 0; i < columnMapArray.Length; i++)
            //    {
            //        string[] array = columnMapArray[i];
            //        sqlBulkCopy.ColumnMappings.Add(array[0], array[1]);
            //    }
            //    sqlBulkCopy.WriteToServer(table);
            //    sqlBulkCopy.Close();
            //}
        }

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="dt"><see cref="DataTable"/>对象</param>
        public void CommitData(DataTable dt)
        {
            DataSet dataSet = ConstructDataSet(dt);
            Database.UpdateDataSet(dataSet, this.TableName);
        }

        /// <summary>
        /// 将<see cref="DataTable"/>对象 转 <see cref="DataSet"/>对象
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DataSet ConstructDataSet(DataTable dt)
        {
            if (dt.DataSet != null)
            {
                return dt.DataSet;
            }
            return new DataSet
            {
                Tables =
                {
                    dt
                }
            };
        }

        /// <summary>
        /// 插入新的数据 
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        /// <returns>返回受影响数</returns>
        public int Insert(Dictionary<string, object> keyValues)
        {
            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", "错误提示：");
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        /// <param name="ID">获取插入数据的ID</param>
        /// <returns>返回受影响数</returns>
        public int Insert(Dictionary<string, object> keyValues, out object ID)
        {
            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", "错误提示：");
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQuery(out ID, CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入新的数据 
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <returns>返回受影响数</returns>
        public int Insert(object prams)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", "错误提示：");
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="ID">获取插入数据的ID</param>
        /// <returns>返回受影响数</returns>
        public int Insert(object prams, out object ID)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", "错误提示：");
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQuery(out ID, CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        /// <returns>受影响行数</returns>
        public int Delete(string where)
        {
            string commandText = string.Format("DELETE FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>受影响行数</returns>
        public int Delete(string where, object prams)
        {
            string commandText = string.Format("DELETE FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQuery(commandText, prams);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        public int Update(object prams, string where)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw NullException;
            }

            List<DbParameter> parms = Database.GetUpdateParams(keyValues, out string strsql);

            string commandText = string.Format("UPDATE {0} SET {1} {2}", this.TableName, strsql, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        /// <param name="parameter">警告：where 条件的参数，切记字符串映射名不要与字段名同名</param>
        public int Update(object prams, string where, object parameter)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw NullException;
            }

            List<DbParameter> parms = Database.GetUpdateParams(keyValues, out string strsql);

            DbParameter[] dbParameters = Database.SetParameters(parameter);
            if (dbParameters != null && dbParameters.Length > 0)
            {
                parms.AddRange(dbParameters);
            }

            string commandText = string.Format("UPDATE {0} SET {1} {2}", this.TableName, strsql, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="keyValues">修改的键值对集合</param>
        /// <param name="where">指定的修改的条件</param>
        public int Update(Dictionary<string, object> keyValues, string where)
        {
            if (keyValues == null || keyValues.Count == 0)
            {
                throw NullException;
            }

            List<DbParameter> parms = Database.GetUpdateParams(keyValues, out string strsql);

            string commandText = string.Format("UPDATE {0} SET {1} {2}", this.TableName, strsql, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQuery(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public DataTable Get(string where)
        {
            string commandText = string.Format("SELECT * FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = Database.ExecuteDataSet(commandText);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public DataTable Get(string where, object prams)
        {
            string commandText = string.Format("SELECT * FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = Database.ExecuteDataSet(commandText, prams);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public DataTable Get(string where, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                throw NullException;
            }
            StringBuilder key = new();

            foreach (string field in fields)
            {
                key.AppendFormat("[{0}],", field);
            }

            string commandText = string.Format("SELECT {0} FROM {1} {2}", key.ToString(0, key.Length - 1), this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = Database.ExecuteDataSet(commandText);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public DataTable Get(string where, object prams, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                throw NullException;
            }
            StringBuilder key = new();

            foreach (string field in fields)
            {
                key.AppendFormat("[{0}],", field);
            }

            string commandText = string.Format("SELECT {0} FROM {1} {2}", key.ToString(0, key.Length - 1), this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = Database.ExecuteDataSet(commandText, prams);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取空表
        /// </summary>
        /// <returns><see cref="DataTable"/>对象</returns>
        public DataTable GetEmptyTable()
        {
            DataTable emptyTable = Database.GetEmptyTable(this.TableName);
            emptyTable.TableName = this.TableName;
            return emptyTable;
        }

        /// <summary>
        /// 新行
        /// </summary>
        /// <returns><see cref="DataRow"/>对象</returns>
        public DataRow NewRow()
        {
            DataTable emptyTable = this.GetEmptyTable();
            DataRow dataRow = emptyTable.NewRow();
            for (int i = 0; i < emptyTable.Columns.Count; i++)
            {
                dataRow[i] = DBNull.Value;
            }
            return dataRow;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象</returns>
        public T GetObject<T>(string where)
        {
            DataRow one = this.GetOne(where);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象</returns>
        public T GetObject<T>(string where, object prams)
        {
            DataRow one = this.GetOne(where, prams);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        public T GetObject<T>(string where, params string[] fields)
        {
            DataRow one = this.GetOne(where, fields);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        public T GetObject<T>(string where, object prams, params string[] fields)
        {
            DataRow one = this.GetOne(where, prams, fields);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象集合</returns>
        public IList<T> GetObjectList<T>(string where)
        {
            DataTable dataTable = this.Get(where);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象集合</returns>
        public IList<T> GetObjectList<T>(string where, object prams)
        {
            DataTable dataTable = this.Get(where, prams);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        public IList<T> GetObjectList<T>(string where, params string[] fields)
        {
            DataTable dataTable = this.Get(where, fields);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        public IList<T> GetObjectList<T>(string where, object prams, params string[] fields)
        {
            DataTable dataTable = this.Get(where, prams, fields);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回第一条数据</returns>
        public DataRow GetOne(string where)
        {
            DataTable dataTable = this.Get(where);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回第一条数据</returns>
        public DataRow GetOne(string where, object prams)
        {
            DataTable dataTable = this.Get(where, prams);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        public DataRow GetOne(string where, params string[] fields)
        {
            DataTable dataTable = this.Get(where, fields);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        public DataRow GetOne(string where, object prams, params string[] fields)
        {
            DataTable dataTable = this.Get(where, prams, fields);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 查询行数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回行数</returns>
        public int GetRecordsCount(string where)
        {
            where ??= "";
            string commandText = string.Format("SELECT COUNT(*) FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));
            return int.Parse(Database.ExecuteScalarToStr(CommandType.Text, commandText));
        }

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="row">新数据<see cref="DataRow"/>对象</param>
        public void Insert(DataRow row)
        {
            DataTable emptyTable = this.GetEmptyTable();
            try
            {
                DataRow dataRow = emptyTable.NewRow();
                for (int i = 0; i < emptyTable.Columns.Count; i++)
                {
                    dataRow[i] = row[i];
                }
                emptyTable.Rows.Add(dataRow);
                this.CommitData(emptyTable);
            }
            catch
            {
                throw;
            }
            finally
            {
                emptyTable.Rows.Clear();
                emptyTable.AcceptChanges();
            }
        }

        /// <summary>
        /// 查询行数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回行数</returns>
        public async Task<int> GetRecordsCountAsync(string where)
        {
            where ??= "";
            string commandText = string.Format("SELECT COUNT(*) FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));
            return int.Parse(await Database.ExecuteScalarToStrAsync(CommandType.Text, commandText));
        }

        /// <summary>
        /// 插入新的数据 
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        /// <returns>返回受影响数</returns>
        public Task<int> InsertAsync(Dictionary<string, object> keyValues)
        {
            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", nameof(keyValues));
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        /// <returns>返回受影响数</returns>
        public Task<SqlNonQuery> InsertIdAsync(Dictionary<string, object> keyValues)
        {
            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", nameof(keyValues));
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQueryIdAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回受影响数</returns>
        public Task<SqlNonQuery> InsertIdAsync(object prams)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", "错误提示：");
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQueryIdAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 插入新的数据 
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <returns>返回受影响数</returns>
        public Task<int> InsertAsync(object prams)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw new ArgumentException("在准备新增一行数据的时候发生异常，键值对集合为空！", "错误提示：");
            }

            List<DbParameter> parms = Database.GetInsertParams(keyValues, out string key, out string value);

            string commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, key, value);
            return Database.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        /// <returns>受影响行数</returns>
        public Task<int> DeleteAsync(string where)
        {
            string commandText = string.Format("DELETE FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQueryAsync(commandText);
        }

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>受影响行数</returns>
        public Task<int> DeleteAsync(string where, object prams)
        {
            string commandText = string.Format("DELETE FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQueryAsync(commandText, prams);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        public Task<int> UpdateAsync(object prams, string where)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw NullException;
            }

            List<DbParameter> parms = Database.GetUpdateParams(keyValues, out string strsql);

            string commandText = string.Format("UPDATE {0} SET {1} {2}", this.TableName, strsql, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        /// <param name="parameter">警告：where 条件的参数，切记字符串映射名不要与字段名同名</param>
        public Task<int> UpdateAsync(object prams, string where, object parameter)
        {
            IDictionary<string, object> keyValues = Database.SetDictionaryParam(prams);

            if (keyValues == null || keyValues.Count == 0)
            {
                throw NullException;
            }

            List<DbParameter> parms = Database.GetUpdateParams(keyValues, out string strsql);

            DbParameter[] dbParameters = Database.SetParameters(parameter);
            if (dbParameters != null && dbParameters.Length > 0)
            {
                parms.AddRange(dbParameters);
            }

            string commandText = string.Format("UPDATE {0} SET {1} {2}", this.TableName, strsql, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="keyValues">修改的键值对集合</param>
        /// <param name="where">指定的修改的条件</param>
        public Task<int> UpdateAsync(Dictionary<string, object> keyValues, string where)
        {
            if (keyValues == null || keyValues.Count == 0)
            {
                throw NullException;
            }

            List<DbParameter> parms = Database.GetUpdateParams(keyValues, out string strsql);

            string commandText = string.Format("UPDATE {0} SET {1} {2}", this.TableName, strsql, DbHelperExensions.WhereStr(where));
            return Database.ExecuteNonQueryAsync(CommandType.Text, commandText, parms.ToArray());
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public async Task<DataTable> GetAsync(string where)
        {
            string commandText = string.Format("SELECT * FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = await Database.ExecuteDataSetAsync(commandText);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public async Task<DataTable> GetAsync(string where, object prams)
        {
            string commandText = string.Format("SELECT * FROM {0} {1}", this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = await Database.ExecuteDataSetAsync(commandText, prams);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public async Task<DataTable> GetAsync(string where, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                throw NullException;
            }
            StringBuilder key = new();

            foreach (string field in fields)
            {
                key.AppendFormat("[{0}],", field);
            }

            string commandText = string.Format("SELECT {0} FROM {1} {2}", key.ToString(0, key.Length - 1), this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = await Database.ExecuteDataSetAsync(commandText);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        public async Task<DataTable> GetAsync(string where, object prams, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                throw NullException;
            }
            StringBuilder key = new();

            foreach (string field in fields)
            {
                key.AppendFormat("[{0}],", field);
            }

            string commandText = string.Format("SELECT {0} FROM {1} {2}", key.ToString(0, key.Length - 1), this.TableName, DbHelperExensions.WhereStr(where));

            DataSet dataSet = await Database.ExecuteDataSetAsync(commandText, prams);
            if (!dataSet.IsEmpty())
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取空表
        /// </summary>
        /// <returns><see cref="DataTable"/>对象</returns>
        public async Task<DataTable> GetEmptyTableAsync()
        {
            DataTable emptyTable = await Database.GetEmptyTableAsync(this.TableName);
            emptyTable.TableName = this.TableName;
            return emptyTable;
        }

        /// <summary>
        /// 新行
        /// </summary>
        /// <returns><see cref="DataRow"/>对象</returns>
        public async Task<DataRow> NewRowAsync()
        {
            DataTable emptyTable = await this.GetEmptyTableAsync();
            DataRow dataRow = emptyTable.NewRow();
            for (int i = 0; i < emptyTable.Columns.Count; i++)
            {
                dataRow[i] = DBNull.Value;
            }
            return dataRow;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象</returns>
        public async Task<T> GetObjectAsync<T>(string where)
        {
            DataRow one = await this.GetOneAsync(where);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象</returns>
        public async Task<T> GetObjectAsync<T>(string where, object prams)
        {
            DataRow one = await this.GetOneAsync(where, prams);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        public async Task<T> GetObjectAsync<T>(string where, params string[] fields)
        {
            DataRow one = await this.GetOneAsync(where, fields);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        public async Task<T> GetObjectAsync<T>(string where, object prams, params string[] fields)
        {
            DataRow one = await this.GetOneAsync(where, prams, fields);
            if (one == null)
            {
                return default;
            }
            return DataHelper.ConvertRowToObject<T>(one);
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象集合</returns>
        public async Task<IList<T>> GetObjectListAsync<T>(string where)
        {
            DataTable dataTable = await this.GetAsync(where);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象集合</returns>
        public async Task<IList<T>> GetObjectListAsync<T>(string where, object prams)
        {
            DataTable dataTable = await this.GetAsync(where, prams);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        public async Task<IList<T>> GetObjectListAsync<T>(string where, params string[] fields)
        {
            DataTable dataTable = await this.GetAsync(where, fields);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        public async Task<IList<T>> GetObjectListAsync<T>(string where, object prams, params string[] fields)
        {
            DataTable dataTable = await this.GetAsync(where, prams, fields);
            if (!dataTable.IsEmpty())
            {
                return DataHelper.ConvertDataTableToObjects<T>(dataTable);
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回第一条数据</returns>
        public async Task<DataRow> GetOneAsync(string where)
        {
            DataTable dataTable = await this.GetAsync(where);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回第一条数据</returns>
        public async Task<DataRow> GetOneAsync(string where, object prams)
        {
            DataTable dataTable = await this.GetAsync(where, prams);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        public async Task<DataRow> GetOneAsync(string where, params string[] fields)
        {
            DataTable dataTable = await this.GetAsync(where, fields);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        public async Task<DataRow> GetOneAsync(string where, object prams, params string[] fields)
        {
            DataTable dataTable = await this.GetAsync(where, prams, fields);
            if (!dataTable.IsEmpty())
            {
                return dataTable.Rows[0];
            }
            return null;
        }
    }
}
