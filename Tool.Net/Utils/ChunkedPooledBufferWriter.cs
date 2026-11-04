using System;
using System.Buffers;
using System.Collections.Generic;

namespace Tool.Utils
{
    /// <summary>
    /// 分块池缓冲写入器，非连续内存，为内存块
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class ChunkedPooledBufferWriter<T> : IBufferWriter<T>, IDisposable
    {
        private const int DefaultChunkSize = 4096;

        private readonly int _chunkSize;
        private readonly List<RawChunk> _chunks = new();
        private RawChunk _current;
        private int _currentWritten;
        private bool _disposed;

        /// <summary>
        /// 构造内存块
        /// </summary>
        /// <param name="chunkSize">默认块大小</param>
        public ChunkedPooledBufferWriter(int chunkSize = DefaultChunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            _chunkSize = chunkSize;
        }

        /// <summary>
        /// 申请内存块
        /// </summary>
        /// <param name="count">申请大小</param>
        public void Advance(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (_current is null) throw new InvalidOperationException("Advance 前必须先 GetSpan/GetMemory");
            if (count > _current.Array.Length - _currentWritten) throw new ArgumentOutOfRangeException(nameof(count));

            _currentWritten += count;
            _current.Written = _currentWritten;
        }

        /// <summary>
        /// 从内存块上获取内存
        /// </summary>
        /// <param name="sizeHint">获取大小</param>
        /// <returns></returns>
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _current!.Array.AsMemory(_currentWritten);
        }

        /// <summary>
        /// 从内存块上获取内存
        /// </summary>
        /// <param name="sizeHint">获取大小</param>
        /// <returns></returns>
        public Span<T> GetSpan(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return _current!.Array.AsSpan(_currentWritten);
        }

        private void EnsureCapacity(int sizeHint)
        {
            if (sizeHint < 1) sizeHint = 1;

            if (_current is null)
            {
                AddChunk(Math.Max(_chunkSize, sizeHint));
                return;
            }

            if (_current.Array.Length - _currentWritten >= sizeHint) return;
            AddChunk(Math.Max(_chunkSize, sizeHint));
        }

        private void AddChunk(int minSize)
        {
            T[] rented = ArrayPool<T>.Shared.Rent(Math.Max(_chunkSize, minSize));
            var chunk = new RawChunk(rented);
            _chunks.Add(chunk);
            _current = chunk;
            _currentWritten = 0;
        }

        /// <summary>
        /// 获取一个块完整的内存块对象
        /// </summary>
        /// <returns></returns>
        public ReadOnlySequence<T> GetReadOnlySequence()
        {
            if (_chunks.Count == 0) return ReadOnlySequence<T>.Empty;

            // 重建只读链表，每块只暴露已写入部分
            Segment first = null, prev = null;
            long runningIndex = 0;

            for (int i = 0; i < _chunks.Count; i++)
            {
                var raw = _chunks[i];
                var seg = new Segment(raw.Array.AsMemory(0, raw.Written), runningIndex);
                first ??= seg;
                prev?.SetNext(seg);
                prev = seg;
                runningIndex += raw.Written;
            }

            var last = prev!;
            return new ReadOnlySequence<T>(first!, 0, last, last.Memory.Length);
        }

        /// <summary>
        /// 重置内存块，还给共享内存
        /// </summary>
        public void Reset()
        {
            foreach (var c in _chunks) c.Return();
            _chunks.Clear();
            _current = null;
            _currentWritten = 0;
        }

        /// <summary>
        /// 释放内存块
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Reset();
        }

        private sealed class RawChunk
        {
            public T[] Array;
            public int Written;
            public RawChunk(T[] array) => Array = array;
            public void Return() => ArrayPool<T>.Shared.Return(Array);
        }

        private sealed class Segment : ReadOnlySequenceSegment<T>
        {
            public Segment(ReadOnlyMemory<T> memory, long runningIndex)
            {
                Memory = memory;
                RunningIndex = runningIndex;
            }

            public void SetNext(Segment next) => Next = next;
        }
    }
}
