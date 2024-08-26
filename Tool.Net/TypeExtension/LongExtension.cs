using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 对long进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class LongExtension
    {
        /// <summary>
        /// 返回二进制流
        /// </summary>
        /// <param name="txt">Long</param>
        /// <returns>返回二进制流</returns>
        public static byte[] ToBytes(this long txt)
        {
            //if (txt == 0)
            //{
            //    throw new System.SystemException("该字符串不存在任何内容！");
            //}
            return BitConverter.GetBytes(txt);
        }

        /// <summary>
        /// 返回Int类型
        /// </summary>
        /// <param name="txt">long</param>
        /// <returns>返回Int类型</returns>
        public static int ToInt(this long txt)
        {
            return txt.ToVar<int>();
        }

        /// <summary>
        /// 当前数除以一个整数，返回一个向上取整的倍数
        /// </summary>
        /// <param name="txt">int</param>
        /// <param name="txt1">除数</param>
        /// <returns></returns>
        public static long Ceiling(this long txt, decimal txt1)
        {
            decimal b = txt / txt1;
            return (long)Math.Ceiling(b);
        }

        #region Long[] 封装方法

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">Long[]</param>
        /// <param name="txt1">新增的值</param>
        public static long[] Add(this long[] txt, long txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 查找该Long数组中是否存在该值。
        /// </summary>
        /// <param name="txt">Long[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this long[] txt, long txt1)
        {
            return txt.Contains<long>(txt1);
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static long[] GetArrayIndex(this long[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该long为空！");
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
            List<long> obj1 = new List<long>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion
    }
}
