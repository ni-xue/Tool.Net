using System;
using System.Buffers;
using Tool.Sockets.Kernels;

namespace Tool.Utils
{
    /// <summary>
    /// 提供内存连续模型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class MemorySegment<T> : ReadOnlySequenceSegment<T>
    {
        /// <summary>
        /// 创建单一连续内存
        /// </summary>
        public MemorySegment() : this(ReadOnlyMemory<T>.Empty) { }

        /// <summary>
        /// 创建单一连续内存
        /// </summary>
        /// <param name="array">内存</param>
        public MemorySegment(T[] array) : this(new ReadOnlyMemory<T>(array)) { }

        /// <summary>
        /// 创建单一连续内存
        /// </summary>
        /// <param name="array">内存</param>
        /// <param name="start">开始读取位置</param>
        /// <param name="length">读取长度</param>
        public MemorySegment(T[] array, int start, int length) : this(new ReadOnlyMemory<T>(array, start, length)) { }

        /// <summary>
        /// 创建单一连续内存
        /// </summary>
        /// <param name="memory">内存</param>
        public MemorySegment(in ReadOnlyMemory<T> memory)
        {
            Memory = memory;
            LongLength = memory.Length;
        }

        private MemorySegment(IMemoryOwner<T> owner, in ReadOnlyMemory<T> memory, MemorySegment<T> next)
        {
            _dataOwner = owner;
            Memory = _dataOwner is not null ? _dataOwner.Memory[..memory.Length] : memory;
            LongLength = memory.Length;
            EndNext = this;
            next.Next = this;

            //long totalLength = next.LongLength;

            //while (true)
            //{
            //    next.LongLength += LongLength;
            //    next.Rank++;

            //    if (next.Next is MemorySegment<T> current)//&& current is not null
            //    {
            //        current.EndNext = this;
            //        next = current;
            //    }
            //    else
            //    {
            //        //RunningIndex += next.RunningIndex + LongLength;
            //        RunningIndex += totalLength;
            //        next.Next = this;
            //        break;
            //    }
            //}
        }

        private MemorySegment<T> SetSequenceCore(in ReadOnlyMemory<T> memory, bool isCopy)
        {
            IMemoryOwner<T> owner = default;
            if (isCopy)
            {
                owner = MemoryPool<T>.Shared.Rent(memory.Length);
                memory.CopyTo(owner.Memory);
            }
            if (LongLength == 0)
            {
                _dataOwner = owner;
                Memory = isCopy ? _dataOwner.Memory[..memory.Length] : memory;
                LongLength = memory.Length;
            }
            else
            {
                EndNext = new MemorySegment<T>(owner, in memory, EndNext ?? this);
                LongLength += memory.Length;
                Rank++;
            }
            return this;
        }

        /// <summary>
        /// 获取节点的最底层
        /// </summary>
        public MemorySegment<T> EndNext { get; private set; }
        //{
        //    get
        //    {
        //        MemorySegment<T> current = Next as MemorySegment<T>;
        //        while (current is not null && current.Next is not null) current = current.Next as MemorySegment<T>;
        //        return current ?? this;
        //    }
        //}

        /// <summary>
        /// 获取连续内存的总长度
        /// </summary>
        public long LongLength { get; private set; }

        /// <summary>
        /// 获取连续内存的总长度
        /// </summary>
        public int Length => (int)LongLength;

        /// <summary>
        /// 获取连续内存是否为空
        /// </summary>
        public bool IsEmpty => Memory.IsEmpty;

        /// <summary>
        /// 层级数
        /// </summary>
        public int Rank { get; private set; }

        private IMemoryOwner<T> _dataOwner;

        /// <summary>
        /// 添加连接的内存数据
        /// </summary>
        /// <param name="memory">内存</param>
        /// <returns>新的连续内存</returns>
        public MemorySegment<T> Append(in ReadOnlyMemory<T> memory) => SetSequenceCore(in memory, false);

        /// <summary>
        /// 添加连接的内存数据
        /// </summary>
        /// <param name="memory">内存</param>
        /// <returns>新的连续内存</returns>
        public MemorySegment<T> Append(T[] memory) => SetSequenceCore(memory, false);

        /// <summary>
        /// 复制一份内存到连续内存中
        /// </summary>
        /// <param name="memory">内存</param>
        public void Copy(in ReadOnlyMemory<T> memory) => SetSequenceCore(in memory, true);

        ///// <summary>
        ///// 创建可读的连续<see cref="ReadOnlySequence{T}"/>（倒叙串联）
        ///// </summary>
        ///// <returns>返回<see cref="ReadOnlySequence{T}"/></returns>
        //public ReadOnlySequence<T> ToLittleReadOnlySequence() => ToLittleReadOnlySequence(0, Memory.Length);

        ///// <summary>
        ///// 创建可读的连续<see cref="ReadOnlySequence{T}"/>（倒叙串联）
        ///// </summary>
        ///// <param name="startIndex">开始位置</param>
        ///// <param name="endIndex">结尾位置</param>
        ///// <returns>返回<see cref="ReadOnlySequence{T}"/></returns>
        //public ReadOnlySequence<T> ToLittleReadOnlySequence(int startIndex, int endIndex) => new(this.EndNext, startIndex, this, endIndex);

        ///// <summary>
        ///// 创建可读的连续<see cref="ReadOnlySequence{T}"/>（顺序串联）
        ///// </summary>
        ///// <returns>返回<see cref="ReadOnlySequence{T}"/></returns>
        //public ReadOnlySequence<T> ToBigReadOnlySequence() => ToBigReadOnlySequence(0, Memory.Length);

        ///// <summary>
        ///// 创建可读的连续<see cref="ReadOnlySequence{T}"/>（顺序串联）
        ///// </summary>
        ///// <param name="startIndex">开始位置</param>
        ///// <param name="endIndex">结尾位置</param>
        ///// <returns>返回<see cref="ReadOnlySequence{T}"/></returns>
        //public ReadOnlySequence<T> ToBigReadOnlySequence(int startIndex, int endIndex) => new(this, startIndex, this.EndNext, endIndex);

        /// <summary>
        /// 创建可读的连续<see cref="ReadOnlySequence{T}"/>（顺序串联）
        /// </summary>
        /// <returns>返回<see cref="ReadOnlySequence{T}"/></returns>
        public ReadOnlySequence<T> ToReadOnlySequence() => ToReadOnlySequence(0, this.EndNext.Length);

        /// <summary>
        /// 创建可读的连续<see cref="ReadOnlySequence{T}"/>（顺序串联）
        /// </summary>
        /// <param name="startIndex">开始位置</param>
        /// <param name="endIndex">结尾位置</param>
        /// <returns>返回<see cref="ReadOnlySequence{T}"/></returns>
        public ReadOnlySequence<T> ToReadOnlySequence(int startIndex, int endIndex) => GetSegmentFinal(startIndex, endIndex);

        private ReadOnlySequence<T> GetSegmentFinal(int startIndex, int endIndex)
        {
            long totalLength = LongLength, firstLength = Memory.Length;
            int rank = Rank;
            if (this.Next is MemorySegment<T> next)
            {
                while (true)
                {
                    next.RunningIndex = LongLength - next.LongLength;
                    totalLength = next.LongLength = totalLength - firstLength;
                    next.Rank = --rank;
                    next.EndNext = this.EndNext;

                    firstLength = next.Memory.Length;
                    if (next.Next is MemorySegment<T> current) next = current; else break;
                }
            }

            return new(this, startIndex, this.EndNext, endIndex);
        }

        /// <summary>
        /// 清空当前连续内存
        /// </summary>
        public void Empty()
        {
            MemorySegment<T> next = this;
            while (true)
            {
                next._dataOwner?.Dispose();
                if (next.Next is MemorySegment<T> current) next = current; else break;
            }

            Next = null;
            EndNext = null;
            Memory = null;
            LongLength = 0;
            RunningIndex = 0;
        }

        /// <summary>
        /// 获取相关描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Rank:{Rank} LongLength:{LongLength}";
        }

    }
}
