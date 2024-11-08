using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Tool.SqlCore
{
    /// <summary>
    /// 单表操作对象 （接口）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public interface ITableProvider
    {
        /// <summary>
        /// 核心数据对象
        /// </summary>
        DbHelper Database { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// 初始化绑定数据对象
        /// </summary>
        /// <param name="Database">核心数据对象</param>
        /// <param name="TableName">表名</param>
        void Initialize(DbHelper Database, string TableName)
        {
            this.Database = Database;
            this.TableName = TableName;
        }

        /// <summary>
        /// 批量提交数据
        /// </summary>
        /// <param name="dataSet"><see cref="DataSet"/>对象</param>
        /// <param name="columnMapArray">列映射数组</param>
		void BatchCommitData(DataSet dataSet, string[][] columnMapArray);

        /// <summary>
        /// 批量提交数据
        /// </summary>
        /// <param name="table"><see cref="DataTable"/>对象</param>
        /// <param name="columnMapArray">列映射数组</param>
        void BatchCommitData(DataTable table, string[][] columnMapArray);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="dt"><see cref="DataTable"/>对象</param>
		void CommitData(DataTable dt);

        /// <summary>
        /// 新行
        /// </summary>
        /// <returns><see cref="DataRow"/>对象</returns>
        DataRow NewRow();

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        int Delete(string where);

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        /// <param name="prams">对虚构参数进行映射</param>
        int Delete(string where, object prams);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="keyValues">修改的键值对集合</param>
        /// <param name="where">指定的修改的条件</param>
        int Update(Dictionary<string, object> keyValues, string where);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        int Update(object prams, string where);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        /// <param name="parameter">警告：where 条件的参数，切记字符串映射名不要与字段名同名</param>
        int Update(object prams, string where, object parameter);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        DataTable Get(string where);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        DataTable Get(string where, object prams);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        DataTable Get(string where, params string[] fields);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        DataTable Get(string where, object prams, params string[] fields);

        /// <summary>
        /// 获取空表
        /// </summary>
        /// <returns><see cref="DataTable"/>对象</returns>
		DataTable GetEmptyTable();

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象</returns>
		T GetObject<T>(string where);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象</returns>
        T GetObject<T>(string where, object prams);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        T GetObject<T>(string where, params string[] fields);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        T GetObject<T>(string where, object prams, params string[] fields);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象集合</returns>
		IList<T> GetObjectList<T>(string where);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象集合</returns>
        IList<T> GetObjectList<T>(string where, object prams);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        IList<T> GetObjectList<T>(string where, params string[] fields);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        IList<T> GetObjectList<T>(string where, object prams, params string[] fields);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回第一条数据</returns>
        DataRow GetOne(string where);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回第一条数据</returns>
        DataRow GetOne(string where, object prams);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        DataRow GetOne(string where, params string[] fields);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        DataRow GetOne(string where, object prams, params string[] fields);

        /// <summary>
        /// 查询行数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回行数</returns>
        int GetRecordsCount(string where);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="row">新数据<see cref="DataRow"/>对象</param>
		void Insert(DataRow row);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        int Insert(Dictionary<string, object> keyValues);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="prams">对字符串进行映射</param>
        int Insert(object prams);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        /// <param name="ID">获取插入数据的ID</param>
        int Insert(Dictionary<string, object> keyValues, out object ID);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="ID">获取插入数据的ID</param>
        int Insert(object prams, out object ID);

        /// <summary>
        /// 查询行数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回行数</returns>
        Task<int> GetRecordsCountAsync(string where);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        Task<int> InsertAsync(Dictionary<string, object> keyValues);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="prams">对字符串进行映射</param>
        Task<int> InsertAsync(object prams);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="keyValues">键值对的数据集</param>
        Task<SqlNonQuery> InsertIdAsync(Dictionary<string, object> keyValues);

        /// <summary>
        /// 插入新的数据
        /// </summary>
        /// <param name="prams">对字符串进行映射</param>
        Task<SqlNonQuery> InsertIdAsync(object prams);

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        Task<int> DeleteAsync(string where);

        /// <summary>
        /// 删除该表数据
        /// </summary>
        /// <param name="where">指定的删除条件</param>
        /// <param name="prams">对虚构参数进行映射</param>
        Task<int> DeleteAsync(string where, object prams);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="keyValues">修改的键值对集合</param>
        /// <param name="where">指定的修改的条件</param>
        Task<int> UpdateAsync(Dictionary<string, object> keyValues, string where);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        Task<int> UpdateAsync(object prams, string where);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="prams">对虚构参数进行映射</param>
        /// <param name="where">指定的修改的条件</param>
        /// <param name="parameter">警告：where 条件的参数，切记字符串映射名不要与字段名同名</param>
        Task<int> UpdateAsync(object prams, string where, object parameter);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        Task<DataTable> GetAsync(string where);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        Task<DataTable> GetAsync(string where, object prams);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        Task<DataTable> GetAsync(string where, params string[] fields);

        /// <summary>
        /// 查询该表信息
        /// </summary>
        /// <param name="where">指定的查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns><see cref="DataTable"/>对象</returns>
        Task<DataTable> GetAsync(string where, object prams, params string[] fields);

        /// <summary>
        /// 获取空表
        /// </summary>
        /// <returns><see cref="DataTable"/>对象</returns>
		Task<DataTable> GetEmptyTableAsync();

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象</returns>
		Task<T> GetObjectAsync<T>(string where);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象</returns>
        Task<T> GetObjectAsync<T>(string where, object prams);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        Task<T> GetObjectAsync<T>(string where, params string[] fields);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象</returns>
        Task<T> GetObjectAsync<T>(string where, object prams, params string[] fields);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回实体对象集合</returns>
		Task<IList<T>> GetObjectListAsync<T>(string where);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回实体对象集合</returns>
        Task<IList<T>> GetObjectListAsync<T>(string where, object prams);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        Task<IList<T>> GetObjectListAsync<T>(string where, params string[] fields);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回实体对象集合</returns>
        Task<IList<T>> GetObjectListAsync<T>(string where, object prams, params string[] fields);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>返回第一条数据</returns>
        Task<DataRow> GetOneAsync(string where);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <returns>返回第一条数据</returns>
        Task<DataRow> GetOneAsync(string where, object prams);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        Task<DataRow> GetOneAsync(string where, params string[] fields);

        /// <summary>
        /// 获取当前查询的数据的第一行
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="prams">对字符串进行映射</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns>返回第一条数据</returns>
        Task<DataRow> GetOneAsync(string where, object prams, params string[] fields);

        /// <summary>
        /// 新行
        /// </summary>
        /// <returns><see cref="DataRow"/>对象</returns>
        Task<DataRow> NewRowAsync();
    }
}
