using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tool.SqlCore
{
    /// <summary>
    /// 本地数据库参数缓存
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class DbParameterCache
    {
        //private readonly Stopwatch cachWatch;

        private DbParameter[] dbParameters;

        private readonly Task<DbParameter[]> taskdbParameters;

        /// <summary>
        /// 创建默认缓存
        /// </summary>
        /// <param name="parameters"></param>
        public DbParameterCache(DbParameter[] parameters)
        {
            //cachWatch = Stopwatch.StartNew();
            dbParameters = parameters;
            //taskdbParameters = Task.FromResult(dbParameters);
        }

        /// <summary>
        /// 创建等待缓存
        /// </summary>
        /// <param name="taskParameters"></param>
        public DbParameterCache(Task<DbParameter[]> taskParameters)
        {
            //cachWatch = Stopwatch.StartNew();
            taskdbParameters = taskParameters;
        }

        /// <summary>
        /// 克隆一个副本<see cref="DbParameter"/>[]
        /// </summary>
        /// <returns><see cref="DbParameter"/>[]</returns>
        public DbParameter[] CloneParameters() => CloneParameters(dbParameters);

        /// <summary>
        /// 克隆一个副本<see cref="DbParameter"/>[]
        /// </summary>
        /// <returns><see cref="DbParameter"/>[]</returns>
        public async Task<DbParameter[]> CloneParametersAsync() 
        {
            dbParameters ??= await taskdbParameters;
            return CloneParameters(dbParameters);
        }

        private static DbParameter[] CloneParameters(DbParameter[] originalParameters)
        {
            DbParameter[] array = new DbParameter[originalParameters.Length];
            int i = 0;
            int num = originalParameters.Length;
            while (i < num)
            {
                if (originalParameters[i] is ICloneable cloneable)
                {
                    array[i] = cloneable.Clone() as DbParameter;
                    i++;
                }
                //array[i] = (DbParameter)((ICloneable)originalParameters[i]).Clone();
                //i++;
            }
            return array;
        }
    }
}
