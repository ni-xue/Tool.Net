using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 完全保证的多线程可等待字典，表示可由多个线程同时访问的键/值对的线程安全集合。
    /// </summary>
    /// <typeparam name="TKey">字典中的键的类型。</typeparam>
    /// <typeparam name="TValue">字典中的值的类型。</typeparam>
    public class TaskConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IAsyncEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// 线程安全的字典
        /// </summary>
        private readonly ConcurrentDictionary<TKey, Task<TValue>> Dictionary;

        /// <summary>
        /// 初始化
        /// </summary>
        public TaskConcurrentDictionary()
        {
            this.Dictionary = new();
        }

        /// <summary>
        /// 获取或设置与指定的键相关联的值。
        /// </summary>
        /// <param name="key">要获取或设置的值的键。</param>
        /// <returns>位于指定索引处的键/值对。</returns>
        /// <exception cref="System.ArgumentNullException">key 为 null。</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">已检索该属性，并且集合中不存在 key。</exception>
        public TValue this[TKey key] { get { return Dictionary[key].Result; } set { Dictionary[key] = Task.FromResult(value); } }

        /// <summary>
        /// 获取一个指示 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 是否为空的值。
        /// </summary>
        /// <returns>如果 System.Collections.Concurrent.ConcurrentDictionary`2 为空，则为 true；否则为 false。</returns>
        public bool IsEmpty => Dictionary.IsEmpty;

        /// <summary>
        /// 获取包含 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中的键的集合。
        /// </summary>
        /// <returns><see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中的键的集合。</returns>
        public ICollection<TKey> Keys => Dictionary.Keys;

        /// <summary>
        /// 获取包含 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中的值的集合。 (作者并不建议使用该字段，开销有点大)
        /// </summary>
        /// <returns>包含 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中的值的集合。</returns>
        public ICollection<TValue> Values
        {
            get
            {
                var data = Dictionary.Values;
                List<TValue> data1 = new();
                foreach (var item in data)
                {
                    data1.Add(item.Result);
                }
                return data1;
            }
        }

        /// <summary>
        /// 获取包含在 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中的键/值对的数目。
        /// </summary>
        /// <returns>包含在 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中的键/值对的数目。</returns>
        /// <exception cref="System.OverflowException">字典已包含最大数目的元素 (System.Int32.MaxValue)。</exception>
        public int Count => Dictionary.Count;

        /// <summary>
        /// 获取一个值，该值指示 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 是否为只读。（无效参数）
        /// </summary>
        public bool IsReadOnly => false;

        public KeyValuePair<TKey, TValue> Current => throw new NotImplementedException();

        /// <summary>
        /// 如果该键尚不存在，则使用指定函数将键/值对添加到 <see cref="TaskConcurrentDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="key">要添加的元素的键。</param>
        /// <param name="valueFactory">用于为键生成值的函数</param>
        /// <returns>键的值。 如果字典中已存在指定的键，则为该键的现有值；如果字典中不存在指定的键，则为 valueFactory 返回的键的新值。</returns>
        /// <exception cref="System.ArgumentNullException">valueFactory 为 null。</exception>
        /// <exception cref="System.OverflowException">字典已包含最大数目的元素 (System.Int32.MaxValue)。</exception>
        public async Task<TValue> GetOrAdd(TKey key, Func<TKey, Task<TValue>> valueFactory)
        {
            var taskResult = this.Dictionary.GetOrAdd(key, valueFactory);
            return await taskResult;
        }

        /// <summary>
        /// 如果该键尚不存在，则使用指定函数将键/值对添加到 <see cref="TaskConcurrentDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="key">要添加的元素的键。</param>
        /// <param name="value">用于为键生成值的函数</param>
        /// <returns>键的值。 如果字典中已存在指定的键，则为该键的现有值；如果字典中不存在指定的键，则为新值。</returns>
        /// <exception cref="System.ArgumentNullException">value 为 null。</exception>
        /// <exception cref="System.OverflowException">字典已包含最大数目的元素 (System.Int32.MaxValue)。</exception>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            var lazyResult = this.Dictionary.GetOrAdd(key, Task.FromResult(value));
            return lazyResult.Result;
        }

        /// <summary>
        /// 从 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中移除所有的键和值。
        /// </summary>
        public void Clear()
        {
            if (Dictionary != null)
            {
                Dictionary.Clear();
            }
        }

        /// <summary>
        /// 确定 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 是否包含指定的键。
        /// </summary>
        /// <param name="key">要在 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中定位的键。</param>
        /// <returns>如果 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 包含具有指定键的元素，则为 true；否则为 false。</returns>
        /// <exception cref="System.ArgumentNullException">key 为 null。</exception>
        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 在 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中添加一个带有所提供的键和值的元素。
        /// </summary>
        /// <param name="key">用作要添加的元素的键的对象。</param>
        /// <param name="value">作为要添加的元素的值的对象。</param>
        public void Add(TKey key, TValue value)
        {
            bool Is = Dictionary.TryAdd(key, Task.FromResult(value));
            if (!Is)
            {
                throw new System.SystemException("不能插入重复键！");
            }
        }

        /// <summary>
        /// 在 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中添加一个带有所提供的键和值的元素。
        /// </summary>
        /// <param name="key">用作要添加的元素的键的对象。</param>
        /// <param name="value">作为要添加的元素的值的对象。</param>
        /// <returns>如果该键/值对已成功添加到 <see cref="TaskConcurrentDictionary{TKey, TValue}"/>，则为 true；如果该键已存在，则为 false。</returns>
        /// <exception cref="System.ArgumentNullException">value 为 null。</exception>
        /// <exception cref="System.OverflowException">字典已包含最大数目的元素 (System.Int32.MaxValue)。</exception>
        public bool TryAdd(TKey key, TValue value)
        {
            return Dictionary.TryAdd(key, Task.FromResult(value));
        }

        /// <summary>
        /// 尝试从 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中移除。
        /// </summary>
        /// <param name="key">要移除并返回的元素的键。</param>
        /// <returns>如果已成功移除对象，则为 true；否则为 false。</returns>
        public bool Remove(TKey key)
        {
            //Lazy<TValue> value = null;
            return Dictionary.TryRemove(key, out _);
        }

        /// <summary>
        /// 尝试从 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中移除。
        /// </summary>
        /// <param name="key">要移除并返回的元素的键。</param>
        /// <param name="value">当此方法返回时，将包含从 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中移除的对象；如果 key 不存在，则包含 TValue 类型。</param>
        /// <returns>如果已成功移除对象，则为 true；否则为 false。</returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            bool Is = Dictionary.TryRemove(key, out Task<TValue> value1);
            value = Is ? value1.Result : default;
            return Is;
        }

        /// <summary>
        /// 尝试从 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 获取与指定的键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="value">当此方法返回时，将包含 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中具有指定键的对象；如果操作失败，则包含默认值。</param>
        /// <returns>如果在 System.Collections.Concurrent.ConcurrentDictionary`2 中找到该键，则为 true；否则为 false。</returns>
        /// <exception cref="System.ArgumentNullException">key 为 null。</exception>
        public bool TryGetValue(TKey key, out Task<TValue> value)
        {
            bool Is = Dictionary.TryGetValue(key, out Task<TValue> value1);
            value = Is ? value1 : default;
            return Is;
        }

        /// <summary>
        ///  在 <see cref="TaskConcurrentDictionary{TKey, TValue}"/> 中添加一个带有所提供的键和值的元素。
        /// </summary>
        /// <param name="item">单个对象的键值对</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Dictionary.TryAdd(item.Key, Task.FromResult(item.Value));
        }

        /// <summary>
        /// 不被实现的。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException("不被实现的。");
        }

        /// <summary>
        /// 不被实现的。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException("不被实现的。");
        }

        /// <summary>
        /// 不被实现的。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException("不被实现的。");
        }

        /// <summary>
        /// 返回循环访问 <see cref="LazyConcurrentDictionary{TKey, TValue}"/> 的枚举数。
        /// </summary>
        /// <returns><see cref="LazyConcurrentDictionary{TKey, TValue}"/> 的一个枚举数。</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            //var data = concurrentDictionary.GetEnumerator();

            foreach (var f in Dictionary) yield return new KeyValuePair<TKey, TValue>(f.Key, f.Value.Result);
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var f in Dictionary) yield return f;
            //throw new NotImplementedException("不被实现的。");
        }

        /// <summary>
        /// 显示结果
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("TaskConcurrentDictionary<", typeof(TKey).Name, ',', typeof(TValue).Name, "> Count = ", this.Count);
        }

        /// <summary>
        /// 异步迭代器
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
#if NET6_0_OR_GREATER
            foreach (var f in Dictionary) yield return new KeyValuePair<TKey, TValue>(f.Key, await f.Value.WaitAsync(cancellationToken));
#else
            foreach (var f in Dictionary) yield return new KeyValuePair<TKey, TValue>(f.Key, await f.Value);
#endif
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            var isok = TryGetValue(key, out Task<TValue> taskvalue);
            value = taskvalue.Result;
            return isok;
        }
    }
}
