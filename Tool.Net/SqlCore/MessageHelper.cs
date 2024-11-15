using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils;

namespace Tool.SqlCore
{
    /// <summary>
    /// 存储过程操作类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class MessageHelper
    {
        /// <summary>
        /// 根据存储过程返回一个<see cref="Message"/>对象
        /// </summary>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessage(List<DbParameter> prams)
        {
            return new Message(prams);//(TypeParse.StrToInt(prams[prams.Count - 1].Value, -1), prams[prams.Count - 2].Value.ToString());
        }

        #region 同步存储过程执行函数

        /// <summary>
        /// 根据存储过程返回一个<see cref="object"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessage(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            database.RunProc(procName, prams);
            return MessageHelper.GetMessage(prams);
        }

        /// <summary>
        /// 根据存储过程返回一个<see cref="object"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessage(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            database.RunProc(procName, commandParameters);
            return MessageHelper.GetMessage(commandParameters);
        }

        /// <summary>
        /// 根据存储过程返回一个<see cref="DataSet"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessageForDataSet(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            database.RunProc(procName, prams, out DataSet entity);
            Message message = MessageHelper.GetMessage(prams);
            if (message.MessageID == 0)
            {
                message.AddEntity(entity);
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个<see cref="DataSet"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessageForDataSet(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            database.RunProc(procName, commandParameters, out DataSet entity);
            Message message = MessageHelper.GetMessage(commandParameters);
            if (message.MessageID == 0)
            {
                message.AddEntity(entity);
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessageForObject<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            database.RunProc(procName, prams, out DataSet dataSet);
            Message message = MessageHelper.GetMessage(prams);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessageForObject<T>(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            database.RunProc(procName, commandParameters, out DataSet dataSet);
            Message message = MessageHelper.GetMessage(commandParameters);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象数组
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessageForObjectList<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            database.RunProc(procName, prams, out DataSet dataSet);
            Message message = MessageHelper.GetMessage(prams);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象数组
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static Message GetMessageForObjectList<T>(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            database.RunProc(procName, commandParameters, out DataSet dataSet);
            Message message = MessageHelper.GetMessage(commandParameters);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个泛型对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型对象</returns>
        public static T GetObject<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            return database.RunProcObject<T>(procName, prams);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型对象</returns>
        public static T GetObject<T>(this DbHelper database, string procName, object prams)
        {
            return GetObject<T>(database, procName, prams, out _);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameters">返回<see cref="DbParameter"/>[]对象集合</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>泛型对象</returns>
        public static T GetObject<T>(this DbHelper database, string procName, object prams, out List<DbParameter> parameters, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            parameters = commandParameters;
            return database.RunProcObject<T>(procName, commandParameters);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型数组对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型数组对象</returns>
        public static IList<T> GetObjectList<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            return database.RunProcObjectList<T>(procName, prams);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型数组对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型数组对象</returns>
        public static IList<T> GetObjectList<T>(this DbHelper database, string procName, object prams)
        {
            return GetObjectList<T>(database, procName, prams, out _);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型数组对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameters">返回<see cref="DbParameter"/>[]对象集合</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>泛型数组对象</returns>
        public static IList<T> GetObjectList<T>(this DbHelper database, string procName, object prams, out List<DbParameter> parameters, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            parameters = commandParameters;
            return database.RunProcObjectList<T>(procName, commandParameters);
        }

        #endregion

        #region 异步存储过程执行函数

        /// <summary>
        /// 根据存储过程返回一个<see cref="object"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageAsync(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            await database.RunProcAsync(procName, prams);
            return MessageHelper.GetMessage(prams);
        }

        /// <summary>
        /// 根据存储过程返回一个<see cref="object"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageAsync(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            await database.RunProcAsync(procName, commandParameters);
            return MessageHelper.GetMessage(commandParameters);
        }

        /// <summary>
        /// 根据存储过程返回一个<see cref="DataSet"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageForDataSetAsync(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            DataSet entity = await database.RunProcDataSetAsync(procName, prams);
            Message message = MessageHelper.GetMessage(prams);
            if (message.MessageID == 0)
            {
                message.AddEntity(entity);
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个<see cref="DataSet"/>对象
        /// </summary>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageForDataSetAsync(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            DataSet entity = await database.RunProcDataSetAsync(procName, commandParameters);
            Message message = MessageHelper.GetMessage(commandParameters);
            if (message.MessageID == 0)
            {
                message.AddEntity(entity);
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageForObjectAsync<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            DataSet dataSet = await database.RunProcDataSetAsync(procName, prams);
            Message message = MessageHelper.GetMessage(prams);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageForObjectAsync<T>(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            DataSet dataSet = await database.RunProcDataSetAsync(procName, commandParameters);
            Message message = MessageHelper.GetMessage(commandParameters);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertRowToObject<T>(dataSet.Tables[0].Rows[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象数组
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageForObjectListAsync<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            DataSet dataSet = await database.RunProcDataSetAsync(procName, prams);
            Message message = MessageHelper.GetMessage(prams);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个实体对象数组
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>返回存储过程对象<see cref="Message"/></returns>
        public static async Task<Message> GetMessageForObjectListAsync<T>(this DbHelper database, string procName, object prams, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            DataSet dataSet = await database.RunProcDataSetAsync(procName, commandParameters);
            Message message = MessageHelper.GetMessage(commandParameters);
            if (message.MessageID == 0)
            {
                message.AddEntity(DataHelper.ConvertDataTableToObjects<T>(dataSet.Tables[0]));
            }
            return message;
        }

        /// <summary>
        /// 根据存储过程返回一个泛型对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型对象</returns>
        public static Task<T> GetObjectAsync<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            return database.RunProcObjectAsync<T>(procName, prams);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型对象</returns>
        public static Task<T> GetObjectAsync<T>(this DbHelper database, string procName, object prams)
        {
            return GetObjectAsync<T>(database, procName, prams, out _);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameters">返回<see cref="DbParameter"/>[]对象集合</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>泛型对象</returns>
        public static Task<T> GetObjectAsync<T>(this DbHelper database, string procName, object prams, out List<DbParameter> parameters, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            parameters = commandParameters;
            return database.RunProcObjectAsync<T>(procName, commandParameters);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型数组对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型数组对象</returns>
        public static Task<IList<T>> GetObjectListAsync<T>(this DbHelper database, string procName, List<DbParameter> prams)
        {
            prams ??= new List<DbParameter>();
            return database.RunProcObjectListAsync<T>(procName, prams);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型数组对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <returns>泛型数组对象</returns>
        public static Task<IList<T>> GetObjectListAsync<T>(this DbHelper database, string procName, object prams)
        {
            return GetObjectListAsync<T>(database, procName, prams, out _);
        }

        /// <summary>
        /// 根据存储过程返回一个泛型数组对象
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="database">数据底层对象</param>
        /// <param name="procName">存储过程名</param>
        /// <param name="prams">存储过程参数</param>
        /// <param name="parameters">返回<see cref="DbParameter"/>[]对象集合</param>
        /// <param name="parameter">具有返回值的参数</param>
        /// <returns>泛型数组对象</returns>
        public static Task<IList<T>> GetObjectListAsync<T>(this DbHelper database, string procName, object prams, out List<DbParameter> parameters, params DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterParams(prams, parameter);
            parameters = commandParameters;
            return database.RunProcObjectListAsync<T>(procName, commandParameters);
        }

        #endregion

        /// <summary>
        /// 将两个<see cref="DbParameter"/>[]组装到一起
        /// </summary>
        /// <param name="database">数据核心对象</param>
        /// <param name="prams"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static List<DbParameter> SetParameterParams(this DbHelper database, object prams, DbParameter[] parameter)
        {
            List<DbParameter> commandParameters = database.SetParameterList(prams);
            commandParameters ??= new List<DbParameter>();
            if (parameter != null && parameter.Length > 0)
            {
                foreach (DbParameter _parameter in parameter)
                {
                    commandParameters.Add(_parameter);
                }
            }
            return commandParameters;
        }
    }
}
