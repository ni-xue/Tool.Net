using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Utils.TaskHelper
{
    /// <summary>
    /// 实现异步Task对象的异步实现类
    /// </summary>
    public static class TaskAsyncHelper
    {
        /// <summary>
        /// 将一个异步任务对象转换成IAsyncResult对象
        /// </summary>
        /// <param name="taskFunc">一个异步对象</param>
        /// <param name="callback">异步完成时使用的回调对象</param>
        /// <param name="state">附带的数据</param>
        /// <returns>返回<see cref="IAsyncResult"/></returns>
        public static IAsyncResult BeginTask(Func<Task> taskFunc, AsyncCallback callback, object state)
        {
            Task task = taskFunc();
            if (task == null)
            {
                return null;
            }
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }
            TaskWrapperAsyncResult resultToReturn = new TaskWrapperAsyncResult(task, state);//底层封装实现
            bool isCompleted = task.IsCompleted;
            if (isCompleted)
            {
                resultToReturn.ForceCompletedSynchronously();
            }
            if (callback != null)
            {
                if (isCompleted)
                {
                    callback(resultToReturn);
                }
                else
                {
                    task.ContinueWith(delegate (Task _)
                    {
                        callback(resultToReturn);
                    });
                }
            }
            return resultToReturn;
        }

        /// <summary>
        /// 用于结束异步完成任务
        /// </summary>
        /// <param name="ar">开始任务时的<see cref="IAsyncResult"/>对象</param>
        public static void EndTask(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException("ar");
            }
            if (!(ar is TaskWrapperAsyncResult taskWrapperAsyncResult))
            {
                throw new ArgumentException("TaskAsyncHelper_ParameterInvalid", "ar");
            }
            if (!taskWrapperAsyncResult.IsCompleted)
            {
                taskWrapperAsyncResult.Task.GetAwaiter().GetResult();
            }
            taskWrapperAsyncResult.Task.Dispose();
        }
    }
}
