using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 流扩展方法类，提供便捷的流数据读取功能
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class StreamExtension
    {
        /// <summary>
        /// 从流中读取所有数据并转换为字符串（同步版本）
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="encoding">字符编码，默认为UTF-8</param>
        /// <returns>从流中读取的字符串内容</returns>
        /// <exception cref="ArgumentNullException">当流为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当流不可读时抛出</exception>
        public static string GetString(this Stream stream, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("流不支持读取操作");

            // 如果流支持定位，保存原始位置以便后续恢复
            long originalPosition = 0;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0; // 重置到流的开始位置
            }

            try
            {
                // 使用StreamReader读取流中的所有文本内容
                using var reader = new StreamReader(stream, encoding, leaveOpen: true);
                return reader.ReadToEnd();
            }
            finally
            {
                // 恢复流的原始位置，确保不影响后续操作
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        /// <summary>
        /// 从流中读取所有数据并转换为字符串（异步版本）
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="encoding">字符编码，默认为UTF-8</param>
        /// <returns>表示异步操作的任务，结果是从流中读取的字符串内容</returns>
        /// <exception cref="ArgumentNullException">当流为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当流不可读时抛出</exception>
        public static async Task<string> GetStringAsync(this Stream stream, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("流不支持读取操作");

            // 如果流支持定位，保存原始位置以便后续恢复
            long originalPosition = 0;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0; // 重置到流的开始位置
            }

            try
            {
                // 使用StreamReader异步读取流中的所有文本内容
                using var reader = new StreamReader(stream, encoding, leaveOpen: true);
                return await reader.ReadToEndAsync();
            }
            finally
            {
                // 恢复流的原始位置，确保不影响后续操作
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        /// <summary>
        /// 从流中读取所有数据并转换为字节数组（同步版本）
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <returns>从流中读取的字节数组</returns>
        /// <exception cref="ArgumentNullException">当流为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当流不可读时抛出</exception>
        public static byte[] GetBytes(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("流不支持读取操作");

            // 如果流本身就是MemoryStream，直接获取字节数组以提高性能
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            // 如果流支持定位，保存原始位置以便后续恢复
            long originalPosition = 0;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0; // 重置到流的开始位置
            }

            try
            {
                // 创建内存流并将源流数据复制到其中
                using var outputMemoryStream = new MemoryStream();
                stream.CopyTo(outputMemoryStream);
                return outputMemoryStream.ToArray();
            }
            finally
            {
                // 恢复流的原始位置，确保不影响后续操作
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        /// <summary>
        /// 从流中读取所有数据并转换为字节数组（异步版本）
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <returns>表示异步操作的任务，结果是从流中读取的字节数组</returns>
        /// <exception cref="ArgumentNullException">当流为null时抛出</exception>
        /// <exception cref="InvalidOperationException">当流不可读时抛出</exception>
        public static async Task<byte[]> GetBytesAsync(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("流不支持读取操作");

            // 如果流本身就是MemoryStream，直接获取字节数组以提高性能
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            // 如果流支持定位，保存原始位置以便后续恢复
            long originalPosition = 0;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0; // 重置到流的开始位置
            }

            try
            {
                // 创建内存流并将源流数据异步复制到其中
                using var outputMemoryStream = new MemoryStream();
                await stream.CopyToAsync(outputMemoryStream);
                return outputMemoryStream.ToArray();
            }
            finally
            {
                // 恢复流的原始位置，确保不影响后续操作
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
