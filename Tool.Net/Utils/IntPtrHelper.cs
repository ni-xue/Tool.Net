using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Tool.Utils
{
    /// <summary>
    /// 关于内存地址读写的操作帮助类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class IntPtrHelper
    {
        #region API

        //从指定内存中读取字节集数据
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        //从指定内存中写入字节集数据
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, int[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

        //打开一个已存在的进程对象，并返回进程的句柄
        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        //关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。
        [DllImport("kernel32.dll")]
        private static extern void CloseHandle(IntPtr hObject);

        #endregion

        #region 使用方法

        /// <summary>
        /// 根据进程名称获取进程ID
        /// </summary>
        /// <param name="processName">进程名字</param>
        /// <returns></returns>
        public static int GetPidByProcessName(string processName)
        {
            Process[] arrayProcess = Process.GetProcessesByName(processName);
            foreach (Process p in arrayProcess)
            {
                return p.Id;
            }
            return 0;
        }

        /// <summary>
        /// 读取内存中的值
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processName">进程名</param>
        /// <returns></returns>
        public static int ReadMemoryValue(int baseAddress, string processName)
        {
            try
            {
                byte[] buffer = new byte[4];
                //获取缓冲区地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName));
                //将制定内存中的值读入缓冲区
                ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
                //从非托管内存中读取一个 32 位带符号整数。
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 读取内存中的值
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static int ReadMemoryValue(int baseAddress, int processId)
        {
            try
            {
                byte[] buffer = new byte[4];
                //获取缓冲区地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, processId);
                //将制定内存中的值读入缓冲区
                ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
                //从非托管内存中读取一个 32 位带符号整数。
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 读取内存中的值
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processName">进程名</param>
        /// <returns></returns>
        public static int ReadMemoryValue(IntPtr baseAddress, string processName)
        {
            try
            {
                byte[] buffer = new byte[4];
                //获取缓冲区地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName));
                //将制定内存中的值读入缓冲区
                ReadProcessMemory(hProcess, baseAddress, byteAddress, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
                //从非托管内存中读取一个 32 位带符号整数。
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 读取内存中的值
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static int ReadMemoryValue(IntPtr baseAddress, int processId)
        {
            try
            {
                byte[] buffer = new byte[4];
                //获取缓冲区地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, processId);
                //将制定内存中的值读入缓冲区
                ReadProcessMemory(hProcess, baseAddress, byteAddress, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
                //从非托管内存中读取一个 32 位带符号整数。
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将值写入指定内存地址中
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processName">进程名</param>
        /// <param name="value">写入的值</param>
        public static void WriteMemoryValue(int baseAddress, string processName, int value)
        {
            try
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName));
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, (IntPtr)baseAddress, new int[] { value }, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
            }
            catch { }
        }

        /// <summary>
        /// 将值写入指定内存地址中
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processId">进程ID</param>
        /// <param name="value">写入的值</param>
        public static void WriteMemoryValue(int baseAddress, int processId, int value)
        {
            try
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, processId);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, (IntPtr)baseAddress, new int[] { value }, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
            }
            catch { }
        }

        /// <summary>
        /// 将值写入指定内存地址中
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processName">进程名</param>
        /// <param name="value">写入的值</param>
        public static void WriteMemoryValue(IntPtr baseAddress, string processName, int value)
        {
            try
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName));
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, baseAddress, new int[] { value }, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
            }
            catch { }
        }

        /// <summary>
        /// 将值写入指定内存地址中
        /// </summary>
        /// <param name="baseAddress">内存地址</param>
        /// <param name="processId">进程ID</param>
        /// <param name="value">写入的值</param>
        public static void WriteMemoryValue(IntPtr baseAddress, int processId, int value)
        {
            try
            {
                //打开一个已存在的进程对象  0x1F0FFF 最高权限
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, processId);
                //从指定内存中写入字节集数据
                WriteProcessMemory(hProcess, baseAddress, new int[] { value }, 4, IntPtr.Zero);
                //关闭操作
                CloseHandle(hProcess);
            }
            catch { }
        }

        /// <summary>
        /// 将字符串类型转int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int GetInt(string s)
        {
            int n = 0;
            int.TryParse(s, out n);
            if (n <= 0)
            {
                n = 100;
            }
            return n;
        }

        #endregion
    }
}
