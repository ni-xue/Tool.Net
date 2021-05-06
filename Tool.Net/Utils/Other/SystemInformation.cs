using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace Tool.Utils.Other
{
    /// <summary>
    /// 系统信息封装类（仅微软系统）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class SystemInformation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WinDir"></param>
        /// <param name="count"></param>
        [DllImport("kernel32.dll")]
        private static extern void GetWindowsDirectory(StringBuilder WinDir, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SysDir"></param>
        /// <param name="count"></param>
        [DllImport("kernel32")]
        private static extern void GetSystemDirectory(StringBuilder SysDir, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memibfo"></param>
        [DllImport("kernel32.dll")]
        private static extern void GlobalMemoryStatus(ref MemoryInformation memibfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memibfo"></param>
        [DllImport("kernel32.dll")]
        private static extern void GlobalMemoryStatusEx(ref MEMORYSTATUSEX memibfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpuinfo"></param>
        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo(ref CPUInformation cpuinfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stinfo"></param>
        [DllImport("kernel32.dll")]
        private static extern void GetSystemTime(ref TimeInformation stinfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DestIP"></param>
        /// <param name="SrcIP"></param>
        /// <param name="MacAddr"></param>
        /// <param name="PhyAddrLen"></param>
        /// <returns></returns>
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 DestIP, Int32 SrcIP, ref Int64 MacAddr, ref Int32 PhyAddrLen);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        [DllImport("Ws2_32.dll")]
        private static extern Int32 Inet_addr(string ipaddr);


        /// <summary>
        /// 更加详细的内存信息对象
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            /// <summary>
            /// 长度
            /// </summary>
            public uint dwLength;
            /// <summary>
            /// 内存使用率
            /// </summary>
            public uint dwMemoryLoad;
            /// <summary>
            /// 总物理内存 此处全是以字节为单位
            /// </summary>
            public ulong ullTotalPhys;
            /// <summary>
            /// 可用物理内存
            /// </summary>
            public ulong ullAvailPhys;      //可用物理内存
            /// <summary>
            /// 交换文件总大小
            /// </summary>
            public ulong ullTotalPageFile;
            /// <summary>
            /// 可用交换文件大小
            /// </summary>
            public ulong ullAvailPageFile;
            /// <summary>
            /// 总虚拟内存
            /// </summary>
            public ulong ullTotalVirtual;
            /// <summary>
            /// 可用虚拟内存大小
            /// </summary>
            public ulong ullAvailVirtual;
            /// <summary>
            /// 扩展效用虚拟现实
            /// </summary>
            public ulong ullAvailExtendedVirtual;
        }

        /// <summary>
        /// 内存信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryInformation
        {
            /// <summary>
            /// 长度
            /// </summary>
            public uint Length;

            /// <summary>
            /// 内存使用率
            /// </summary>
            public uint MemoryUtilizationRate;

            /// <summary>
            /// 总物理内存 此处全是以字节为单位
            /// </summary>
            public uint TotalPhysicalMemory;

            /// <summary>
            /// 可用物理内存
            /// </summary>
            public uint FreePhysicalMemory;

            /// <summary>
            /// 交换文件总大小
            /// </summary>
            public uint TotalSizeOfSwapFile;

            /// <summary>
            /// 可用交换文件大小
            /// </summary>
            public uint AvailableExchangeFileSize;

            /// <summary>
            /// 总虚拟内存
            /// </summary>
            public uint TotalVirtualMemory;

            /// <summary>
            /// 可用虚拟内存大小
            /// </summary>
            public uint VirtualMemorySizeAvailable;
        }

        /// <summary>
        /// cpu信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CPUInformation
        {
            /// <summary>
            /// cpu的OemId
            /// </summary>
            public uint OemId;

            /// <summary>
            /// cpu页面大小
            /// </summary>
            public uint CPUPageSize;

            /// <summary>
            /// 最小应用地址
            /// </summary>
            public uint lpMinimumApplicationAddress;

            /// <summary>
            /// 最大应用地址
            /// </summary>
            public uint lpMaximumApplicationAddress;

            /// <summary>
            /// DWACT处理器掩码
            /// </summary>
            public uint dwActiveProcessorMask;

            /// <summary>
            /// cpu个数
            /// </summary>
            public uint CPUNumber;

            /// <summary>
            /// cpu类别
            /// </summary>
            public uint CPUCategory;

            /// <summary>
            /// DWAL定位粒度
            /// </summary>
            public uint dwAllocationGranularity;

            /// <summary>
            /// cpu等级
            /// </summary>
            public uint CPULevel;

            /// <summary>
            /// cpu修正
            /// </summary>
            public uint CPUCorrection;
        }

        /// <summary>
        /// 时间结构实体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TimeInformation
        {
            /// <summary>
            /// 年份
            /// </summary>
            public ushort wYear;

            /// <summary>
            /// 月
            /// </summary>
            public ushort wMonth;

            /// <summary>
            /// 星期
            /// </summary>
            public ushort wDayOfWeek;

            /// <summary>
            /// 日
            /// </summary>
            public ushort wDay;

            /// <summary>
            /// 时，与实际时间相差8个小时
            /// </summary>
            public ushort wHour;

            /// <summary>
            /// 分
            /// </summary>
            public ushort wMinute;

            /// <summary>
            /// 秒
            /// </summary>
            public ushort wSecond;

            /// <summary>
            /// 毫秒
            /// </summary>
            public ushort wMilliseconds;
        }

        /// <summary>
        /// 内存利用率函数，返回已使用率，（单位 %，百分百）
        /// </summary>
        /// <returns>返回使用率</returns>
        public static int GetMemoryUsageRate()
        {
            try
            {
                MemoryInformation memInfor = new MemoryInformation();

                GlobalMemoryStatus(ref memInfor);

                return memInfor.MemoryUtilizationRate.ToString().ToInt();
            }
            catch (Exception)
            {

                return 0;
            }

        }

        /// <summary>
        /// 内存利用率函数，返回已使用率，（单位 %，百分百）
        /// </summary>
        /// <returns>返回使用率</returns>
        public static int GetMemoryUsageRateEx()
        {
            try
            {
                MEMORYSTATUSEX memInfor = new MEMORYSTATUSEX();

                GlobalMemoryStatusEx(ref memInfor);

                return memInfor.dwMemoryLoad.ToString().ToInt();
            }
            catch (Exception)
            {

                return 0;
            }

        }

        /// <summary>
        /// 获取系统内存大小（单位M）
        /// </summary>
        /// <returns>内存大小（单位M）</returns>
        public static int GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();   //用于查询一些如系统信息的管理对象 
            searcher.Query = new SelectQuery("Win32_PhysicalMemory ", "", new string[] { "Capacity" });//设置查询条件 
            ManagementObjectCollection collection = searcher.Get();   //获取内存容量 
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();

            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return (int)(capacity / 1024 / 1024);
        }

        /// <summary>
        /// 系统路径
        /// </summary>
        public static string GetSystemPath()
        {
            const int nChars = 128;

            StringBuilder Buff = new StringBuilder(nChars);

            GetSystemDirectory(Buff, nChars);

            return Buff.ToString();
        }

        /// <summary>
        /// window路径
        /// </summary>
        public static string GetWindowPath()
        {
            const int nChars = 128;

            StringBuilder Buff = new StringBuilder(nChars);

            GetWindowsDirectory(Buff, nChars);

            return Buff.ToString();
        }

        /// <summary>
        /// 获取内存实体对象信息
        /// </summary>
        /// <returns>返回内存实体对象</returns>
        public static MemoryInformation GetMemoryInformation()
        {
            try
            {
                MemoryInformation memInfor = new MemoryInformation();

                GlobalMemoryStatus(ref memInfor);

                return memInfor;
            }
            catch (Exception)
            {
                return new MemoryInformation();
            }
        }

        /// <summary>
        /// 获取内存实体对象信息
        /// </summary>
        /// <returns>返回内存实体对象</returns>
        public static MEMORYSTATUSEX GetMemoryInformationEx()
        {
            try
            {
                MEMORYSTATUSEX memInfor = new MEMORYSTATUSEX();

                GlobalMemoryStatusEx(ref memInfor);

                return memInfor;
            }
            catch (Exception)
            {
                return new MEMORYSTATUSEX();
            }
        }

        /// <summary>
        /// 获取cpu信息
        /// </summary>
        /// <returns></returns>
        public static CPUInformation GetCPUInformation()
        {
            try
            {
                CPUInformation memInfor = new CPUInformation();

                GetSystemInfo(ref memInfor);

                return memInfor;
            }
            catch (Exception)
            {
                return new CPUInformation();
            }

        }

        /// <summary>
        /// 系统时间信息结构体
        /// </summary>
        /// <returns></returns>
        public static TimeInformation GetSystemTimeInformation()
        {
            try
            {
                TimeInformation memInfor = new TimeInformation();

                GetSystemTime(ref memInfor);

                return memInfor;
            }
            catch (Exception)
            {
                return new TimeInformation();
            }
        }

        /// <summary>
        /// cpu序列号函数
        /// </summary>
        /// <returns></returns>
        public static string CPU_SequenceNumber()
        {
            try
            {
                string cpuInfo = "";//cpu序列号

                ManagementClass mc = new ManagementClass("Win32_Processor");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }

                moc = null;

                mc = null;

                return cpuInfo;
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 获得mac地址函数
        /// </summary>
        /// <returns>返回MAC地址</returns>
        public static string GetMAC()
        {
            try
            {
                string mac = "";

                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                    }
                    mo.Dispose();
                }
                return mac;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 获得远程目标mac地址函数
        /// </summary>
        /// <param name="ip">IP地址或域名</param>
        /// <returns>返回MAC地址</returns>
        public static string GetLongRangeMac(string ip)
        {
            StringBuilder mac = new StringBuilder();
            try
            {
                int remote = Inet_addr(ip);

                long macinfo = new long();

                int length = 6;

                SendARP(remote, 0, ref macinfo, ref length);

                string temp = Convert.ToString(macinfo, 16).PadLeft(12, '0').ToUpper();

                int x = 12;

                for (int i = 0; i < 6; i++)
                {
                    if (i == 5)
                        mac.Append(temp.Substring(x - 2, 2));
                    else
                        mac.Append(temp.Substring(x - 2, 2) + "-");
                    x -= 2;
                }
                return mac.ToString();
            }
            catch
            {
                return mac.ToString();
            }
        }

        /// <summary>
        /// 获得ip地址函数
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            try
            {
                string st = "";

                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {

                        //st=mo["IpAddress"].ToString();

                        Array ar;

                        ar = (Array)(mo.Properties["IpAddress"].Value);

                        st = ar.GetValue(0).ToString();

                        break;
                    }
                }

                moc = null;

                mc = null;

                return st;
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 获取硬盘ID函数
        /// </summary>
        /// <returns></returns>
        public static string GetHardDiskID()
        {
            try
            {
                string HDid = "";

                ManagementClass mc = new ManagementClass("Win32_DiskDrive");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }

                moc = null;

                mc = null;

                return HDid;
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 获得系统登陆用户名函数
        /// </summary>
        /// <returns></returns>
        public static string GetSystemLogonUser()
        {
            try
            {
                string st = "";

                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    st = mo["UserName"].ToString();
                }

                moc = null;

                mc = null;

                return st;
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 获得电脑类型函数
        /// </summary>
        /// <returns></returns>
        public static string GetComputerType()
        {
            try
            {
                string st = "";

                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }

                moc = null;

                mc = null;

                return st;
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 获得物理总内存函数
        /// </summary>
        /// <returns></returns>
        public static string GetTotalPhysicalMemory()
        {
            try
            {
                string st = "";

                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");

                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString();
                }

                moc = null;

                mc = null;

                return st;
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 获取当前操作系统的名称
        /// </summary>
        public static string SystemName()
        {
            try
            {
                return Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "未能找到";
            }
        }

        /// <summary>
        /// 性能显示状况1
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <param name="CounterName"></param>
        /// <returns></returns>
        public static float PerformanceDisplayStatus(string CategoryName, string CounterName)
        {
            PerformanceCounter pc = new PerformanceCounter(CategoryName, CounterName);

            //Thread.Sleep(500);//waitfor1second

            float xingneng = pc.NextValue();

            return xingneng;
        }


        /// <summary>
        /// 性能显示状况2
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <param name="CounterName"></param>
        /// <param name="InstanceName"></param>
        /// <returns></returns>
        public static float PerformanceDisplayStatus(string CategoryName, string CounterName, string InstanceName)
        {
            PerformanceCounter pc = new PerformanceCounter(CategoryName, CounterName, InstanceName);

            //Thread.Sleep(500);//waitfor1second

            float xingneng = pc.NextValue();

            return xingneng;
        }

        /// <summary>
        /// 获取当前系统版本类型
        /// </summary>
        public static string SystemEdition
        {
            get
            {
                OperatingSystem 版本信息 = Environment.OSVersion;
                switch (版本信息.Platform)
                {
                    case System.PlatformID.Win32Windows:
                        switch (版本信息.Version.Minor)
                        {
                            case 0:
                                return "Windows 95";
                            case 10:
                                if (版本信息.Version.Revision.ToString() == "2222A")
                                    return "Windows 98 Second Edition";
                                else
                                    return "Windows 98";
                            case 90:
                                return "Windows Me";
                        }
                        break;
                    case System.PlatformID.Win32NT:
                        switch (版本信息.Version.Major)
                        {
                            case 3:
                                return "Windows NT 3.51";
                            case 4:
                                return "Windows NT 4.0";
                            case 5:
                                if (版本信息.Version.Minor == 0)
                                    return "Windows 2000";
                                else
                                    return "Windows XP";
                            case 6:
                                return "Windows 8";
                            case 10:
                                return "Windows 10";
                        }
                        break;
                }
                return 版本信息.VersionString;
            }
        }

        /// <summary>
        /// 获取其他硬件，软件信息 https://msdn.microsoft.com/en-us/library/aa394084(VS.85).aspx
        /// </summary>
        /// <returns></returns>
        public static PhicnalInfo[] GetPhicnalInfo
        {
            get
            {
                List<PhicnalInfo> phicnalInfos = new List<PhicnalInfo>();

                ManagementClass osClass = new ManagementClass("Win32_Processor");//后面几种可以试一下，会有意外的收获//Win32_PhysicalMemory/Win32_Keyboard/Win32_ComputerSystem/Win32_OperatingSystem
                foreach (ManagementObject obj in osClass.GetInstances())
                {
                    PropertyDataCollection pdc = obj.Properties;
                    foreach (PropertyData pd in pdc)
                    {
                        phicnalInfos.Add(new PhicnalInfo(pd.Name, pd.Value));
                    }
                }
                return phicnalInfos.ToArray();
            }
        }

        /// <summary>
        /// 操作系统硬件信息
        /// </summary>
        public struct PhicnalInfo
        {
            /// <summary>
            /// 实例化虚构实体
            /// </summary>
            public PhicnalInfo(string Name, object Value)
            {
                this.Name = Name;
                this.Value = Value;
            }

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 内容
            /// </summary>
            public object Value { get; set; }
        }
    }
}
