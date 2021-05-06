using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils.Data;

namespace Tool.Web.Session
{
    /// <summary>
    /// 用于提供实现自定义Session
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class DiySession : ISession
    {
        //private readonly LazyConcurrentDictionary<string, byte[]> _AsSession;

        private string _id;
        private bool _isAvailable;

        /// <summary>
        /// 提供的日志输出模块
        /// </summary>
        public ILogger Logger { get; internal set; }

        internal void InsideInitialize(string id)
        {
            this._id = id;
            try
            {
                this.Initialize();
                this._isAvailable = true;
            }
            catch (Exception ex)
            {
                this._isAvailable = false;
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    Logger.LogError(ex, "在完成DiySession初始化的时候发生了异常！");
                }
            }
        }

        /// <summary>
        /// 创建Session对象的必要流程
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// SessionId
        /// </summary>
        public string Id { get { return _id; } }

        /// <summary>
        /// Session 是否可以
        /// </summary>
        public bool IsAvailable { get { return _isAvailable; } }

        /// <summary>
        /// 提供 Session 的全部键
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> GetKeys();

        /// <summary>
        /// 清空 Session  的全部键值
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 删除 Session 指定的键值
        /// </summary>
        /// <param name="key">指定的键</param>
        public abstract void Remove(string key);

        /// <summary>
        /// 添加键值的方法
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public abstract void Set(string key, byte[] value);

        /// <summary>
        /// 获取键值的方法
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>返回是否存在</returns>
        public abstract bool TryGetValue(string key, out byte[] value);

        IEnumerable<string> ISession.Keys => this.GetKeys();

        void ISession.Clear()
        {
            //    _AsSession.Clear();
            this.Clear();
        }

        void ISession.Remove(string key)
        {
            //_AsSession.Remove(key);
            this.Remove(key);
        }

        void ISession.Set(string key, byte[] value)
        {
            //if(!_AsSession.TryAdd(key, value)) 
            //{
            //    _AsSession[key] = value;
            //}
            this.Set(key, value);
        }

        bool ISession.TryGetValue(string key, out byte[] value)
        {
            //return _AsSession.TryGetValue(key, out value);
            return this.TryGetValue(key, out value);
        }

        /// <summary>
        /// 显示说明
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("DiySession Count = ", GetKeys().Count());
        }

        /// <summary>
        /// 无用了
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ISession.CommitAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 无用了
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ISession.LoadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
