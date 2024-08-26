using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 对ulong进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class UlongExtension
    {
        /// <summary>
        /// 返回二进制流
        /// </summary>
        /// <param name="txt">Ulong</param>
        /// <returns>返回二进制流</returns>
        public static byte[] ToBytes(this ulong txt)
        {
            //if (txt == 0)
            //{
            //    throw new System.SystemException("该字符串不存在任何内容！");
            //}
            return BitConverter.GetBytes(txt);
        }

        /// <summary>
        /// 当前数除以一个整数，返回一个向上取整的倍数
        /// </summary>
        /// <param name="txt">int</param>
        /// <param name="txt1">除数</param>
        /// <returns></returns>
        public static ulong Ceiling(this ulong txt, decimal txt1)
        {
            decimal b = txt / txt1;
            return (ulong)Math.Ceiling(b);
        }

        /// <summary>
        /// 原子方式+1
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ulong Increment(this ref ulong value) => Interlocked.Increment(ref value);

        /// <summary>
        /// 原子方式-1
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ulong Decrement(this ref ulong value) => Interlocked.Decrement(ref value);

        #region Ulong[] 封装方法

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">Ulong[]</param>
        /// <param name="txt1">新增的值</param>
        public static ulong[] Add(this ulong[] txt, ulong txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 查找该Ulong数组中是否存在该值。
        /// </summary>
        /// <param name="txt">Ulong[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this ulong[] txt, ulong txt1)
        {
            return txt.Contains<ulong>(txt1);
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static ulong[] GetArrayIndex(this ulong[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该ulong为空！");
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
            List<ulong> obj1 = new List<ulong>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion
    }
}
