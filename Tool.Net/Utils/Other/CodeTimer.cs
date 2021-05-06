using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Tool.Utils.Other
{
    /// <summary>
    /// 该类 <see cref="T:CodeTimer"/> 有助于在控制台方便时间码测试。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class CodeTimer
    {
        #region Utils

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        #endregion

        /// <summary>
        /// 初始化 <see cref="T:CodeTimer"/>.
        /// </summary>
        private static void Initialize()
        {
            if (Process.GetCurrentProcess().PriorityClass != ProcessPriorityClass.High)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
            if (Thread.CurrentThread.Priority != ThreadPriority.Highest)
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }
            //Time(string.Empty, 1, () => { });
        }

        /// <summary>
        /// Times 测试。
        /// </summary>
        /// <param name="iteration">迭代运行指定的行动。</param>
        /// <param name="action">操作运行。</param>
        public static string Time(int iteration, Action action)
        {
            return Time(string.Empty, iteration, i => action());
        }

        /// <summary>
        /// Times 测试。
        /// </summary>
        /// <param name="iteration">迭代运行指定的行动。</param>
        /// <param name="action">操作运行。</param>
        public static string Time(int iteration, Action<int> action)
        {
            return Time(string.Empty, iteration, action);
        }

        /// <summary>
        /// Times 测试。
        /// </summary>
        /// <param name="name">当前测试的名称。</param>
        /// <param name="iteration">迭代运行指定的行动。</param>
        /// <param name="action">操作运行。</param>
        public static string Time(string name, int iteration, Action action)
        {
            return Time(name, iteration, i => action());
        }

        /// <summary>
        /// Times 测试。
        /// </summary>
        /// <param name="name">当前测试的名称。</param>
        /// <param name="iteration">迭代运行指定的行动。</param>
        /// <param name="action">操作运行。</param>
        public static string Time(string name, int iteration, Action<int> action)
        {
            return Time(name, iteration, action, false);
        }

        /// <summary>
        /// Times 测试。
        /// </summary>
        /// <param name="name">当前测试的名称。</param>
        /// <param name="averageTime">等于运行时间除以迭代数量</param>
        /// <param name="iteration">迭代运行指定的行动。可以理解为循环几次执行的方法</param>
        /// <param name="action">操作运行。</param>
        public static string Time(string name, int iteration, Action<int> action, bool averageTime = false)
        {
            Initialize();

            if (name == null || iteration <= 0 || action == null)
                throw new System.SystemException("该字符串不存在任何内容！");

            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            var watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCycleCount();
            for (int i = 0; i < iteration; i++)
            {
                action(i);
            }
            ulong cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            Console.ForegroundColor = currentForeColor;

            StringBuilder stringBuilder = new StringBuilder();

            if (averageTime)
                stringBuilder.AppendFormat("\tTime 运行:\t{0} ms", (watch.ElapsedMilliseconds / iteration).ToString("N0"));
            else
                stringBuilder.AppendFormat("\tTime 运行:\t{0} ms", watch.ElapsedMilliseconds.ToString("N0"));

            stringBuilder.AppendFormat("\tCPU Cycles:\t{0}", cpuCycles.ToString("N0"));

            for (int i = 0; i <= GC.MaxGeneration; i++)//系统当前支持的最大代数。
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                stringBuilder.AppendFormat("\tGen " + i + ": \t\t{0}", count);
            }

            Console.WriteLine(stringBuilder.ToString());

            return stringBuilder.ToString();
        }
    }
}
