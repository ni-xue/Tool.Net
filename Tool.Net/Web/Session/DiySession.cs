using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils.Data;
using Tool.Web.Api;

namespace Tool.Web.Session
{
    /// <summary>
    /// 用于提供实现自定义Session
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class DiySession : ISession
    {
        //private readonly LazyConcurrentDictionary<string, byte[]> _AsSession;

        private DiySessionOptions diyOptions;
        private string _id;
        private bool _isAvailable;

        /// <summary>
        /// 提供日志输出模块
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// 提供当前请求信息模块
        /// </summary>
        public HttpContext Context { get; private set; }

        internal async Task InsideInitialize(DiySessionOptions diyOptions, string id, HttpContext context, ILogger logger)
        {
            this.diyOptions = diyOptions;
            string sign = diyOptions.Sign;
            if (id.EndsWith(sign))
            {
                this._id = id[..(id.Length - sign.Length)];
                this._isAvailable = true;
            }
            else
            {
                this._id = id;
                this._isAvailable = false;
            }
            this.Context = context;
            this.Logger = logger;
            try
            {
                await this.Initialize();
                //await Task.CompletedTask;
                //this._isAvailable = true;
            }
            catch (Exception ex)
            {
                //this._isAvailable = false;
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    Logger.LogError(ex, "在完成DiySession初始化的时候发生了异常！");
                }
            }
        }

        /// <summary>
        /// 创建Session对象的认证流程
        /// </summary>
        public virtual Task Initialize() => Task.CompletedTask;

        private void Available(bool _isAvailable)
        {
            if (_isAvailable != this._isAvailable)
            {
                string newid = _isAvailable ? $"{_id}{diyOptions.Sign}" : _id;
                diyOptions.SetSessionId(Context, newid);
                this._isAvailable = _isAvailable;
            }
        }

        /// <summary>
        /// SessionId
        /// </summary>
        public string Id { get { return _id; } }

        /// <summary>
        /// Session 是否可用（可用时将自动标记，可用标志）
        /// </summary>
        public bool IsAvailable { get { return _isAvailable; } set => Available(value); }

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
