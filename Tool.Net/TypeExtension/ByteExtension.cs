using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;

namespace Tool
{
    /// <summary>
    /// 对Byte进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static partial class ByteExtension
    {
        #region byte 封装方法

        #endregion

        /// <summary>
        /// 原子方式+1
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte Increment(this ref byte value) => (byte)Interlocked.Increment(ref Unsafe.As<byte, int>(ref value));

        /// <summary>
        /// 原子方式-1
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte Decrement(this ref byte value) => (byte)Interlocked.Decrement(ref Unsafe.As<byte, int>(ref value));

        #region byte[] 封装方法

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <param name="Bytes"></param>         
        /// <returns>返回一个原对象</returns> 
        [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.", DiagnosticId = "SYSLIB0011", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
        public static object BytesToObject(this byte[] Bytes)
        {
            if (Bytes == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Bytes))
            {
                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原 （升级版，有效降低内存消耗）
        /// </summary>
        /// <param name="Bytes">数据流</param>       
        /// <returns>返回一个原对象</returns> 
        public static T BytesToObject<T>(this byte[] Bytes)
        {
            if (Bytes == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(Bytes, 0);
            return Marshal.PtrToStructure<T>(ptr);
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原 （升级版，有效降低内存消耗）
        /// </summary>
        /// <param name="Bytes">数据流</param>       
        /// <param name="type">转换为原来类的Type</param>
        /// <returns>返回一个原对象</returns> 
        public static object BytesToObject(this byte[] Bytes, Type type)
        {
            if (Bytes == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(Bytes, 0);
            return Marshal.PtrToStructure(ptr, type);
        }

        /// <summary>
        /// 将byte数组转换为文件并保存到指定地址(绝对路径)（备注：如果该文件存在，将会被替换）
        /// </summary>
        /// <param name="buff">byte数组</param>
        /// <param name="savepath">保存地址</param>
        public static void ToBytesFile(this byte[] buff, string savepath)
        {
            if (File.Exists(savepath))
            {
                File.Delete(savepath);
            }
            using FileStream fs = new(savepath, FileMode.CreateNew);
            using BinaryWriter bw = new(fs);
            bw.Write(buff, 0, buff.Length);
        }

        /// <summary>
        /// 将对象转换成Base64字符串（编码）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToBase64(this byte[] obj)
        {
            if (obj == null)
            {
                throw new System.SystemException("该object为空！");
            }
            //byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(obj);
        }

        /// <summary>
        /// 转换为Int类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Int类型</returns>
        public static int ToInt(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToInt32(bt, 0);
        }

        /// <summary>
        /// 转换为Double类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Double类型</returns>
        public static double ToDouble(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToDouble(bt, 0);
        }

        /// <summary>
        /// 转换为16进制 例如“7F-2C-4A”。
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为16进制</returns>
        public static string ToStrings(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToString(bt, 0);
        }

        /// <summary>
        /// 转换为String类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为String类型</returns>
        public static string ToByteString(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return Encoding.Default.GetString(bt);
        }

        /// <summary>
        /// 转换为String类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>转换为String类型</returns>
        public static string ToByteString(this byte[] bt, Encoding encoding)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return encoding.GetString(bt);
        }

        /// <summary>
        /// 转换为Bool类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Bool类型</returns>
        public static bool ToBool(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToBoolean(bt, 0);
        }

        /// <summary>
        /// 转换为Char类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Char类型</returns>
        public static char ToChar(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToChar(bt, 0);
        }

        /// <summary>
        /// 转换为Short类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Short类型</returns>
        public static short ToShort(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToInt16(bt, 0);
        }

        /// <summary>
        /// 转换为Long类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Long类型</returns>
        public static long ToLong(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToInt64(bt, 0);
        }

        /// <summary>
        /// 转换为Float类型
        /// </summary>
        /// <param name="bt">byte[]</param>
        /// <returns>转换为Float类型</returns>
        public static float ToFloat(this byte[] bt)
        {
            if (bt == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            return BitConverter.ToSingle(bt, 0);
        }

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">bool[]</param>
        /// <param name="txt1">新增的值</param>
        public static byte[] Add(this byte[] txt, byte txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 查找该byte数组中是否存在该值。
        /// </summary>
        /// <param name="txt">byte[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this byte[] txt, byte txt1)
        {
            return txt.Contains<byte>(txt1);
        }

        /// <summary>
        /// 重写封装的Copy方法
        /// </summary>
        /// <param name="sourceArray">源数组对象</param>
        /// <param name="destinationArray">新数组对象</param>
        /// <param name="sourceIndex">源数据开始读取的位置</param>
        /// <param name="length">从源数组取多少？(是指从读取位置开始往后读的数量)</param>
        /// <returns>返回当前新的数组中复制了多少个下标的值</returns>
        public static int Read(this byte[] sourceArray, [In][Out] byte[] destinationArray, int sourceIndex, int length)
        {
            return Read(sourceArray, sourceIndex, destinationArray, 0, length);
        }

        /// <summary>
        /// 重写封装的Copy方法
        /// </summary>
        /// <param name="sourceArray">源数组对象</param>
        /// <param name="sourceIndex">源数据开始读取的位置</param>
        /// <param name="destinationArray">新数组对象</param>
        /// <param name="destinationIndex">开始存储的位置</param>
        /// <param name="length">从源数组取多少？(是指从读取位置开始往后读的数量)</param>
        /// <returns>返回当前新的数组中复制了多少个下标的值</returns>
        public static int Read(this byte[] sourceArray, int sourceIndex, [In][Out] byte[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new System.SystemException("该byte为空！");
            }
            if (sourceIndex < 0)
            {
                throw new System.SystemException("sourceIndex不能小于0，数组越界！");
            }
            if (length <= 0)
            {
                throw new System.SystemException("length不能小于或等于0，数组异常！");
            }
            if (sourceArray.Length < sourceIndex)
            {
                throw new System.SystemException("sourceIndex超出了数组，数组越界！");
            }

            int Length = length <= (sourceArray.Length - sourceIndex) ? length : (sourceArray.Length - sourceIndex);

            if (destinationArray.Length < destinationIndex + Length)
            {
                throw new System.SystemException($"destinationArray数组容量不够必须大于或等于{destinationIndex + Length}，数组越界！");
            }

            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, Length);

            return Length;
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static byte[] GetArrayIndex(this byte[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该byte为空！");
            }
            if (index > count)
            {
                throw new System.SystemException("count不能小于index，数组越界！");
            }
            if (index < 0)
            {
                throw new System.SystemException("index不能小于0，数组越界！");
            }
            if (count < 0)
            {
                throw new System.SystemException("count不能小于0，数组越界！");
            }
            if (obj.Length < index)
            {
                throw new System.SystemException("index超出了数组，数组越界！");
            }
            if (obj.Length < count)
            {
                throw new System.SystemException("count超出了数组，数组越界！");
            }
            List<byte> obj1 = new();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion

        #region Memory<byte>

        /// <summary> 
        /// 将<see cref="Memory{T}"/>转换成<see cref="ArraySegment{T}"/>（不是拷贝）
        /// </summary>
        /// <param name="memory">数据流</param>       
        /// <returns>返回<see cref="ArraySegment{T}"/></returns> 
        public static ArraySegment<T> AsArraySegment<T>(this Memory<T> memory) => AsArraySegment((ReadOnlyMemory<T>)memory);

        /// <summary> 
        /// 将<see cref="Memory{T}"/>转换成<see cref="ArraySegment{T}"/>（不是拷贝）
        /// </summary>
        /// <param name="memory">数据流</param>       
        /// <returns>返回<see cref="ArraySegment{T}"/></returns> 
        public static ArraySegment<T> AsArraySegment<T>(this ReadOnlyMemory<T> memory)
        {
            if (memory.IsEmpty)
            {
                //throw new System.SystemException("对象未有初始值！");
                return ArraySegment<T>.Empty;
            }
            if (!MemoryMarshal.TryGetArray(memory, out var segment))
            {
                var field = typeof(ReadOnlyMemory<T>).GetField("_object", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                segment = Unsafe.As<T[]>(field.GetValue(memory));
            }
            return segment;
        }

        #endregion
    }
}
