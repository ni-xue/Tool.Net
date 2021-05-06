using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 对Double进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class DoubleExtension
    {
        /// <summary>
        /// 返回二进制流
        /// </summary>
        /// <param name="txt">Double</param>
        /// <returns>返回二进制流</returns>
        public static byte[] ToBytes(this double txt)
        {
            //if (txt == 0)
            //{
            //    throw new System.SystemException("该字符串不存在任何内容！");
            //}
            return BitConverter.GetBytes(txt);
        }

        /// <summary>
        /// 将数字转换成整数，支持四舍五入，默认不四舍五入。
        /// </summary>
        /// <param name="txt">Double</param>
        /// <param name="type">true：为需要四舍五入</param>
        /// <returns>返回整数，带四舍五入</returns>
        public static int ToInt(this double txt, bool type = false)
        {
            string[] vs = txt.ToString().Split('.');
            if (type)
            {
                if (vs.Length == 2)
                {
                    if (vs[1].Substring(0, 1).ToInt() >= 5)
                    {
                        return vs[0].ToInt()+1;
                    }
                    else
                    {
                        return vs[0].ToInt();
                    }
                }
                else
                {
                    return vs[0].ToInt();
                }
            }
            else
            {
                return vs[0].ToInt();
            }
        }

        #region Double[] 封装方法

        /// <summary>
        /// 给数组加新的值
        /// </summary>
        /// <param name="txt">Double[]</param>
        /// <param name="txt1">新增的值</param>
        public static double[] Add(this double[] txt, double txt1)
        {
            var add = txt.ToList();
            add.Add(txt1);
            txt = add.ToArray();
            //txt.Initialize();
            return txt;
        }

        /// <summary>
        /// 查找该Double数组中是否存在该值。
        /// </summary>
        /// <param name="txt">Double[]</param>
        /// <param name="txt1">查找的字符</param>
        /// <returns>方法存在或不存在</returns>
        public static bool Contains(this double[] txt, double txt1)
        {
            return txt.Contains<double>(txt1);
        }

        /// <summary>
        /// 同于获取指定部分的内容
        /// </summary>
        /// <param name="obj">对象数组</param>
        /// <param name="index">从下标N开始</param>
        /// <param name="count">到下标N结束</param>
        /// <returns>返回一部分的数组内容</returns>
        public static double[] GetArrayIndex(this double[] obj, int index, int count)
        {
            if (obj == null)
            {
                throw new System.SystemException("该double为空！");
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
            List<double> obj1 = new List<double>();

            for (int i = index; i < count; i++)
            {
                obj1.Add(obj[i]);
            }
            return obj1.ToArray();
        }
        #endregion
    }
}
