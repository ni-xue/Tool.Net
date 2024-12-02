using System.Collections.Generic;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 事件控制抽象类（服务端版）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class EnServerEventDrive
    {
        /// <summary>
        /// 默认构造（公共模板信息）
        /// </summary>
        protected EnServerEventDrive()
        {
            this.noEnServer = EnumEventQueue.Instance.noEnServer;
            this.noQueueEnServer = EnumEventQueue.Instance.noQueueEnServer;
        }

        #region 事件公共枚举

        internal EnServer noEnServer;
        internal EnServer noQueueEnServer;

        #endregion

        #region 事件管理器

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <param name="state">等于true时生效，将关闭一切的相关事件</param>
        /// <returns>返回true时表示设置成功！</returns>
        public virtual bool OnInterceptor(EnServer enServer, bool state)
        {
            bool isno = !IsEvent(enServer);
            if (state)
            {
                if (isno) return false;
                noEnServer |= enServer;
            }
            else
            {
                if (!isno) return false;
                noEnServer &= enServer;
            }
            return true;
        }

        /// <summary>
        /// 设置将<see cref="EnServer"/>事件，载入或不载入
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <param name="state">等于true时，事件由队列线程完成，false时交由任务线程自行完成</param>
        /// <returns>返回true时表示设置成功！</returns>
        public virtual bool OnIsQueue(EnServer enServer, bool state)
        {
            bool isno = !IsQueue(enServer);
            if (state)
            {
                if (!isno) return false;
                noQueueEnServer &= enServer;
            }
            else
            {
                if (isno) return false;
                noQueueEnServer |= enServer;
            }
            return true;
        }

        /// <summary>
        /// 获取该事件是否会触发
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <returns><see cref="bool"/></returns>
        public virtual bool IsEvent(EnServer enServer) => (noEnServer & enServer) == 0;

        /// <summary>
        /// 获取该事件是否在队列任务中运行
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <returns><see cref="bool"/></returns>
        public virtual bool IsQueue(EnServer enServer) => (noQueueEnServer & enServer) == 0;

        /// <summary>
        /// 开启全部事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnServerEventDrive OpenAllEvent() 
        {
            noEnServer = 0;
            return this;
        }

        /// <summary>
        /// 全部事件都设置成队列事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnServerEventDrive OpenAllQueue()
        {
            noQueueEnServer = 0;
            return this;
        }

        /// <summary>
        /// 关闭全部事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnServerEventDrive CloseAllEvent()
        {
            noEnServer = EnServer.Create | EnServer.Fail | EnServer.Connect | EnServer.SendMsg | EnServer.Receive | EnServer.ClientClose | EnServer.Close | EnServer.HeartBeat;
            return this;
        }

        /// <summary>
        /// 全部事件都设置成主要事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnServerEventDrive CloseAllQueue()
        {
            noQueueEnServer = EnServer.Create | EnServer.Fail | EnServer.Connect | EnServer.SendMsg | EnServer.Receive | EnServer.ClientClose | EnServer.Close | EnServer.HeartBeat;
            return this;
        }

        #endregion
    }
}