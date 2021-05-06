using System;
using System.Data;
using System.Data.Common;

namespace Tool.SqlCore
{
    /// <summary>
    /// 对<see cref="DbTransaction"/> 对象，提供扩展支持
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DbTransactionExensions
    {
        /// <summary>
        /// SQL事物执行（增/改/删）相关操作
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">执行的SQL语句</param>
        /// <param name="parameters">携带的参数可以是Null，虚构参数</param>
        /// <returns></returns>
        public static DbTransResult ExecuteNonQuery(this DbTransaction transaction, DbHelper dbHelper, string commandText, object parameters)
        {
            return transaction.ExecuteNonQuery(dbHelper, commandText, dbHelper.SetParameters(parameters));
        }

        /// <summary>
        /// SQL事物执行（增/改/删）相关操作
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="commandText">执行的SQL语句</param>
        /// <param name="commandParameters">携带的参数可以是Null</param>
        /// <returns></returns>
        public static DbTransResult ExecuteNonQuery(this DbTransaction transaction, DbHelper dbHelper, string commandText, params DbParameter[] commandParameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("执行SQL为空！", "commandText");
            }
            return transaction.ExecuteNonQuery(dbHelper, new SqlTextParameter(commandText, commandParameters));
        }

        /// <summary>
        /// SQL事物执行（增/改/删）相关操作
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="dbHelper">数据库引擎</param>
        /// <param name="sqlTexts">SQL操作对象<see cref="SqlTextParameter"/>[]</param>
        /// <returns></returns>
        public static DbTransResult ExecuteNonQuery(this DbTransaction transaction, DbHelper dbHelper, params SqlTextParameter[] sqlTexts)
        {
            Exception exp = null;
            bool Success = false;
            int Rows = -1;
            try
            {
                Rows = dbHelper.ExecuteNonQuery(transaction, sqlTexts);
                transaction.Commit();
                Success = true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                exp = ex;
            }
            finally 
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
            }

            return new DbTransResult(Success, Rows, exp);
        }
    }

    /// <summary>
    /// 高效的多表安全操作实体
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class SqlTextParameter
    {
        /// <summary>
        /// 创建查询语句
        /// </summary>
        /// <param name="commandText">执行的SQL语句</param>
        public SqlTextParameter(string commandText): this(commandText, null) { }

        /// <summary>
        /// 创建查询语句
        /// </summary>
        /// <param name="commandText">执行的SQL语句</param>
        /// <param name="parameters">携带的参数可以是Null</param>
        public SqlTextParameter(string commandText, DbParameter[] parameters) : this(CommandType.Text, commandText, parameters) { }

        /// <summary>
        /// 创建查询语句
        /// </summary>
        /// <param name="commandType">Sql的类型</param>
        /// <param name="commandText">执行的SQL语句</param>
        /// <param name="parameters">携带的参数可以是Null</param>
        public SqlTextParameter(CommandType commandType, string commandText, DbParameter[] parameters)
        {
            CommandType = commandType;
            CommandText = commandText;
            Parameters = parameters;
        }

        /// <summary>
        /// 表示Sql的类型
        /// </summary>
        public CommandType CommandType { get; set; } = CommandType.Text;

        /// <summary>
        /// 执行的SQL语句
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// 携带的参数可以是Null
        /// </summary>
        public DbParameter[] Parameters { get; set; }
    }

    /// <summary>
    /// 用于提供事物执行情况，返回事物发生的结果
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DbTransResult
    {
        /// <summary>
        /// 对象构造函数
        /// </summary>
        /// <param name="success">完成情况</param>
        /// <param name="rows">受影响行数</param>
        /// <param name="exception">发生的异常</param>
        public DbTransResult(bool success, int rows, Exception exception) 
        {
            Success = success;
            Rows = rows;
            Exception = exception;
        }

        /// <summary>
        /// 表示执行中发生的特殊情况（异常）
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 事物是否提交成功（状态）
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 受影响行数（无需解释）
        /// </summary>
        public int Rows { get; }
    }
}