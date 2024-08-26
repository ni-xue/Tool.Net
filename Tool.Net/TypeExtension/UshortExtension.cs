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
    /// 对ushort进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class UshortExtension
    {
        /// <summary>
        /// 返回二进制流
        /// </summary>
        /// <param name="txt">Ushort</param>
        /// <returns>返回二进制流</returns>
        public static byte[] ToBytes(this ushort txt)
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
        public static ushort Ceiling(this ushort txt, decimal txt1)
        {
            decimal b = txt / txt1;
            return (ushort)Math.Ceiling(b);
        }

        /// <summary>
        /// 原子方式+1
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ushort Increment(this ref ushort value) => (ushort)Interlocked.Increment(ref Unsafe.As<ushort, int>(ref value));

        /// <summary>
        /// 原子方式-1
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ushort Decrement(this ref ushort value) => (ushort)Interlocked.Decrement(ref Unsafe.As<ushort, int>(ref value));

        #region Ushort[] 封装方法

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">Ushort[]</param>
        /// <param name="txt1">新增的值</param>
        public static ushort[] Add(this ushort[] txt, ushort txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 查找该Ushort数组中是否存在该值。
        /// </summary>
        /// <param name="txt">Ushort[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this ushort[] txt, ushort txt1)
        {
            return txt.Contains<ushort>(txt1);
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static ushort[] GetArrayIndex(this ushort[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该ushort为空！");
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
            List<ushort> obj1 = new List<ushort>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion
    }
}
