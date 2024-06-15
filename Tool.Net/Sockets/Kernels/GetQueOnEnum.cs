using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 事件处理接口
    /// </summary>
    public interface IGetQueOnEnum
    {
        /// <summary>
        /// 获取默认完成的事件结果
        /// </summary>
        public static IGetQueOnEnum Success { get; } = new GetQueOnEnum();

        /// <summary>
        /// 获取当前任务是否完成
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 等待任务完成！
        /// </summary>
        /// <returns>采用自旋模式</returns>
        public void Wait()
        {
            SpinWait.SpinUntil(wait);
            bool wait() => IsSuccess;
        }

        /// <summary>
        /// 事件触发器
        /// </summary>
        internal ValueTask Completed();
    }

    internal class GetQueOnEnum : IGetQueOnEnum
    {
        public GetQueOnEnum() => IsSuccess = true;

        /// <summary>
        /// 获取当前任务是否完成
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 事件触发器
        /// </summary>
        ValueTask IGetQueOnEnum.Completed() => ValueTask.CompletedTask;
    }

    /// <summary>
    /// 存储事件队列的类
    /// </summary>
    internal class GetQueOnEnum<T> : IGetQueOnEnum where T : Enum
    {
        /// <summary>
        /// 构造服务端或客户端事件
        /// </summary>
        /// <param name="Key">IP+端口</param>
        /// <param name="EnumAction">事件枚举</param>
        /// <param name="Completed">委托事件</param>
        public GetQueOnEnum(UserKey Key, T EnumAction, CompletedEvent<T> Completed)
        {
            this.Key = Key;
            this.EnumAction = EnumAction;
            Time = DateTime.Now;
            EnCompleted = Completed;
        }

        /// <summary>
        /// 启动指定方法
        /// </summary>
        async ValueTask IGetQueOnEnum.Completed()
        {
            //if (IsSuccess) return;
            try
            {
                //Debug.WriteLine("{0} ：{1}", ObjectExtension.Thread.Name, EnumAction);
                await EnCompleted.Invoke(Key, EnumAction, Time);
            }
            catch (Exception e)
            {
                Log.Error("Net事件调用异常", e);
            }
            finally
            {
                IsSuccess = true;
            }
        }

        /// <summary>
        /// IP+端口
        /// </summary>
        public UserKey Key { get; }

        /// <summary>
        /// 消息发生时间
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// 回调函数
        /// </summary>
        public CompletedEvent<T> EnCompleted { get; }

        /// <summary>
        /// 客户端或服务器枚举
        /// </summary>
        public T EnumAction { get; }

        /// <summary>
        /// 获取当前任务是否完成
        /// </summary>
        public bool IsSuccess { get; private set; } = false;
    }
}
