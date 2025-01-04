namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 事件控制抽象类（客户端版）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class EnClientEventDrive
    {
        /// <summary>
        /// 默认构造（公共模板信息）
        /// </summary>
        protected EnClientEventDrive()
        {
            this.noEnClient = EnumEventQueue.Instance.noEnClient;
            this.noQueueEnClient = EnumEventQueue.Instance.noQueueEnClient;
        }

        #region 事件公共枚举

        private EnClient noEnClient;
        private EnClient noQueueEnClient;

        #endregion

        #region 事件管理器

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <param name="state">等于true时生效，false将关闭一切的相关事件</param>
        /// <returns>返回true时表示设置成功！</returns>
        public virtual bool OnInterceptor(EnClient enClient, bool state)
        {
            bool isno = !IsEvent(enClient);
            if (state)
            {
                if (!isno) return false;
                noEnClient &= ~enClient;
            }
            else
            {
                if (isno) return false;
                noEnClient |= enClient;
            }
            return true;
        }

        /// <summary>
        /// 设置将<see cref="EnClient"/>事件，载入或不载入
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <param name="state">等于true时，事件由队列线程完成，false时交由任务线程自行完成</param>
        /// <returns>返回true时表示设置成功！</returns>
        public virtual bool OnIsQueue(EnClient enClient, bool state)
        {
            bool isno = !IsQueue(enClient);
            if (state)
            {
                if (!isno) return false;
                noQueueEnClient &= ~enClient;
            }
            else
            {
                if (isno) return false;
                noQueueEnClient |= enClient;
            }
            return true;
        }

        /// <summary>
        /// 获取该事件是否会触发
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <returns><see cref="bool"/></returns>
        public virtual bool IsEvent(EnClient enClient) => (noEnClient & enClient) == 0;

        /// <summary>
        /// 获取该事件是否在队列任务中运行
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <returns><see cref="bool"/></returns>
        public virtual bool IsQueue(EnClient enClient) => (noQueueEnClient & enClient) == 0;

        /// <summary>
        /// 开启全部事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnClientEventDrive OpenAllEvent()
        {
            noEnClient = 0; 
            return this;
        }

        /// <summary>
        /// 全部事件都设置成队列事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnClientEventDrive OpenAllQueue()
        {
            noQueueEnClient = 0;
            return this;
        }

        /// <summary>
        /// 关闭全部事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnClientEventDrive CloseAllEvent()
        {
            noEnClient = EnClient.Connect | EnClient.Fail | EnClient.SendMsg | EnClient.Receive | EnClient.Close | EnClient.HeartBeat | EnClient.Reconnect;
            return this;
        }

        /// <summary>
        /// 全部事件都设置成主要事件
        /// </summary>
        /// <returns>返回当前对象，快速配置</returns>
        public virtual EnClientEventDrive CloseAllQueue()
        {
            noQueueEnClient = EnClient.Connect | EnClient.Fail | EnClient.SendMsg | EnClient.Receive | EnClient.Close | EnClient.HeartBeat | EnClient.Reconnect;
            return this;
        }

        #endregion
    }
}
