using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.TaskHelper
{
    /// <summary>
    /// 实现异步Task对象的异步实现类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
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
            TaskWrapperAsyncResult resultToReturn = new(task, state);//底层封装实现
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
                throw new ArgumentNullException(nameof(ar));
            }
            if (ar is not TaskWrapperAsyncResult taskWrapperAsyncResult)
            {
                throw new ArgumentException("TaskAsyncHelper_ParameterInvalid", nameof(ar));
            }
            if (!taskWrapperAsyncResult.IsCompleted)
            {
                taskWrapperAsyncResult.Task.GetAwaiter().GetResult();
            }
            taskWrapperAsyncResult.Task.Dispose();
        }

        /// <summary>
        /// 执行任务，超时时回调委托
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="timeoutCallback">回调委托</param>
        /// <returns>任务结果</returns>
        public static async Task ExecuteWithTimeoutAsync(Func<Task> task, TimeSpan timeout, Action timeoutCallback)
        {
            using var cts = new CancellationTokenSource();
            var timeoutTask = Task.Delay(timeout, cts.Token);

            var mainTask = task();

            if (await Task.WhenAny(mainTask, timeoutTask) == timeoutTask)
            {
                // 触发超时回调
                timeoutCallback?.Invoke();
            }
            else
            {
                // 取消超时任务
                cts.Cancel();
                await mainTask; // 确保完成主任务
            }
        }
    }
}
