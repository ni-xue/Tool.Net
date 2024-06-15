using System.Diagnostics;
using System.Runtime.CompilerServices;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Tool.Utils
{
    /// <summary>
    /// 自定义的公共对象（重用模型）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : IDisposable where T : class, IDisposable, new()
    {
        /// <summary>
        /// The maximum number of objects to retain in the pool.
        /// </summary>
        public int MaximumRetained { get; }

        //private ConditionalWeakTable<T, ObjectWrapper> table;//WeakReference<T>
        private volatile T[] _items;
        private volatile T _firstItem;

        //private readonly Func<T> _createFunc;
        //private readonly Func<T, bool> _returnFunc;

        private readonly int _maxCapacity;
        private volatile int _numItems;
        //private T _lastItem;

        /// <summary>
        /// 初始化
        /// </summary>
        public ObjectPool() : this(Environment.ProcessorCount * 2) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="MaximumRetained">默认保留最大容量</param>
        public ObjectPool(int MaximumRetained)
        {
            this.MaximumRetained = MaximumRetained;
            this._maxCapacity = MaximumRetained - 1;
            _items = new T[_maxCapacity];
        }

        private T Create()
        {
            //Debug.WriteLine("创建构造{0}", a1);
            T _obj = new();
            return _obj;
        }

        /// <summary>
        /// 获取可用的对象
        /// </summary>
        /// <returns>返回对象</returns>
        public T Get()
        {
            ThrowIfDisposed();
            var item = _firstItem;
            if (item == null || Interlocked.CompareExchange(ref _firstItem, null, item) != item)
            {
                var items = _items;
                for (var i = 0; i < items.Length; i++)
                {
                    item = items[i];
                    if (item != null && Interlocked.CompareExchange(ref items[i], null, item) == item)
                    {
                        Interlocked.Decrement(ref _numItems);
                        return item;
                    }
                }

                item = Create();
            }

            return item;
        }

        /// <summary>
        /// 归还对象给管理器
        /// </summary>
        /// <param name="obj">对象</param>
        public void Return(T obj)
        {
            if (_disposed) { OdjectDispose(obj); return; }
            if (_firstItem != null || Interlocked.CompareExchange(ref _firstItem, obj, null) != null)
            {
                if (Interlocked.Increment(ref _numItems) <= _maxCapacity)
                {
                    var items = _items;
                    for (var i = 0; i < items.Length && Interlocked.CompareExchange(ref items[i], obj, null) != null; ++i) { }
                }
                else
                {
                    Interlocked.Decrement(ref _numItems); 
                    //Debug.WriteLine("销毁构造{0}", Interlocked.Increment(ref j1));
                    OdjectDispose(obj);
                }
            }
        }

        private static void OdjectDispose(T obj)
        {
            obj?.Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            OdjectDispose(_firstItem);
            _firstItem = null;
            foreach (var item in _items)
            {
                OdjectDispose(item);
            }
            _items = null;
            GC.SuppressFinalize(this);
        }

        bool _disposed = false;

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                ThrowObjectDisposedException();
            }

            void ThrowObjectDisposedException() => throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
