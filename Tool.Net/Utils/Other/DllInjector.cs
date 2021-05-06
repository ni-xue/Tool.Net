using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Tool.Utils.Other
{
    /// <summary>
    /// 进程状态
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public enum DllInjectionResult
    {
        /// <summary>
        /// 未找到指定的DLL路径
        /// </summary>
        DllNotFound,
        /// <summary>
        /// 未找到指定名称的进程
        /// </summary>
        GameProcessNotFound,
        /// <summary>
        /// 注入失败
        /// </summary>
        InjectionFailed,
        /// <summary>
        /// 注入成功
        /// </summary>
        Success
    }

    /// <summary>
    /// DLL注入类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class DllInjector
    {
        private static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

        /// <summary>
        /// 用来打开一个已存在的进程对象，并返回进程的句柄。
        /// </summary>
        /// <param name="dwDesiredAccess">渴望得到的访问权限（标志）</param>
        /// <param name="bInheritHandle">是否继承句柄</param>
        /// <param name="dwProcessId">进程标示符</param>
        /// <returns>返回进程的句柄。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]//声明API函数
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        /// <summary>
        /// 关闭一个内核对象。 包括文件、文件映射、进程、线程、安全和同步对象等。涉及文件处理时，这个函数通常与vb的close命令相似。应尽可能的使用close，因为它支持vb的差错控制。
        /// </summary>
        /// <param name="hObject">代表一个已打开对象handle。</param>
        /// <returns>TRUE：执行成功； FALSE：执行失败，可以调用GetLastError()获知失败原因。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        /// <summary>
        /// 功能是检索指定的动态链接库(DLL)中的输出库函数地址。
        /// </summary>
        /// <param name="hModule">DLL模块句柄</param>
        /// <param name="lpProcName">DLL中的函数。</param>
        /// <returns>返回进程的句柄。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        /// <summary>
        /// 功能是获取一个应用程序或动态链接库的模块句柄。只有在当前进程的场景中，这个句柄才会有效。
        /// </summary>
        /// <param name="lpModuleName">指定模块名，这通常是与模块的文件名相同的一个名字。例如，NOTEPAD.EXE程序的模块文件名就叫作NOTEPAD</param>
        /// <returns>只有在当前进程的场景中，这个句柄才会有效，返回进程的句柄。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// 作用是在指定进程的虚拟空间保留或提交内存区域，除非指定MEM_RESET参数，否则将该内存区域置0。
        /// </summary>
        /// <param name="hProcess">申请内存所在的进程句柄。</param>
        /// <param name="lpAddress">保留页面的内存地址；一般用NULL自动分配 。</param>
        /// <param name="dwSize">欲分配的内存大小，字节单位；注意实际分 配的内存大小是页内存大小的整数倍</param>
        /// <param name="flAllocationType">MEM_COMMIT：为特定的页面区域分配内存中或磁盘的页面文件中的物理存储 MEM_PHYSICAL ：分配物理内存（仅用于地址窗口扩展内存） MEM_RESERVE：保留进程的虚拟地址空间，而不分配任何物理存储。保留页面可通过继续调用VirtualAlloc（）而被占用 MEM_RESET ：指明在内存中由参数lpAddress和dwSize指定的数据无效 MEM_TOP_DOWN：在尽可能高的地址上分配内存（Windows 98忽略此标志） MEM_WRITE_WATCH：必须与MEM_RESERVE一起指定，使系统跟踪那些被写入分配区域的页面（仅针对Windows 98）</param>
        /// <param name="flProtect">PAGE_READONLY： 该区域为只读。如果应用程序试图访问区域中的页的时候，将会被拒绝访 PAGE_READWRITE 区域可被应用程序读写 PAGE_EXECUTE： 区域包含可被系统执行的代码。试图读写该区域的操作将被拒绝。 PAGE_EXECUTE_READ ：区域包含可执行代码，应用程序可以读该区域。 PAGE_EXECUTE_READWRITE： 区域包含可执行代码，应用程序可以读写该区域。 PAGE_GUARD： 区域第一次被访问时进入一个STATUS_GUARD_PAGE异常，这个标志要和其他保护标志合并使用，表明区域被第一次访问的权限 PAGE_NOACCESS： 任何访问该区域的操作将被拒绝 PAGE_NOCACHE： RAM中的页映射到该区域时将不会被微处理器缓存（cached) 注:PAGE_GUARD和PAGE_NOCHACHE标志可以和其他标志合并使用以进一步指定页的特征。PAGE_GUARD标志指定了一个防护页（guard page），即当一个页被提交时会因第一次被访问而产生一个one-shot异常，接着取得指定的访问权限。PAGE_NOCACHE防止当它映射到虚拟页的时候被微处理器缓存。这个标志方便设备驱动使用直接内存访问方式（DMA）来共享内存块。</param>
        /// <returns>执行成功就返回分配内存的首地址，不成功就是NULL。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// 此函数能写入某一进程的内存区域（直接写入会出Access Violation错误），故需此函数入口区必须可以访问，否则操作将失败。
        /// </summary>
        /// <param name="hProcess">由OpenProcess返回的进程句柄。 如参数传数据为 INVALID_HANDLE_VALUE 【即-1】目标进程为自身进程</param>
        /// <param name="lpBaseAddress">要写的内存首地址 再写入之前，此函数将先检查目标地址是否可用，并能容纳待写入的数据。</param>
        /// <param name="buffer">指向要写的数据的指针。</param>
        /// <param name="size">要写入的字节数。</param>
        /// <param name="lpNumberOfBytesWritten">实际数据的长度</param>
        /// <returns>非零值代表成功。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        /// <summary>
        /// 创建一个在其它进程地址空间中运行的线程(也称:创建远程线程).。
        /// </summary>
        /// <param name="hProcess">线程所属进程的进程句柄. 该句柄必须具有 PROCESS_CREATE_THREAD, PROCESS_QUERY_INFORMATION, PROCESS_VM_OPERATION, PROCESS_VM_WRITE,和PROCESS_VM_READ 访问权限.</param>
        /// <param name="lpThreadAttribute">一个指向 SECURITY_ATTRIBUTES 结构的指针, 该结构指定了线程的安全属性.</param>
        /// <param name="dwStackSize">线程初始大小,以字节为单位,如果该值设为0,那么使用系统默认大小.</param>
        /// <param name="lpStartAddress">在远程进程的地址空间中,该线程的线程函数的起始地址.</param>
        /// <param name="lpParameter">传给线程函数的参数.</param>
        /// <param name="dwCreationFlags">线程的创建标志.</param>
        /// <param name="lpThreadId">指向所创建线程ID的指针,如果创建失败,该参数为NULL.</param>
        /// <returns>如果调用成功,返回新线程句柄. 如果失败,返回NULL.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        /// <summary>
        /// 一个实例化的静态类
        /// </summary>
        private static DllInjector _instance;

        /// <summary>
        /// 实例化当前类
        /// </summary>
        public static DllInjector GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DllInjector();
                }
                return _instance;
            }
        }

        //DllInjector() { }

        /// <summary>
        /// 注入的DLL
        /// </summary>
        /// <param name="sProcName">进程名称</param>
        /// <param name="sDllPath">需要注入的DLL路径</param>
        /// <returns></returns>
        public DllInjectionResult Inject(string sProcName, string sDllPath)
        {
            //判断是否存在
            if (!File.Exists(sDllPath))
            {   //寻找这个DLL的路径不存在
                return DllInjectionResult.DllNotFound;
            }

            uint _procId = 0;//初始化一个进程ID

            Process[] _procs = Process.GetProcesses();//获取当前操作系统的所有进程
            for (int i = 0; i < _procs.Length; i++)//循环查找
            {
                if (_procs[i].ProcessName == sProcName)//是否，有匹配的进程
                {
                    _procId = (uint)_procs[i].Id;//然后赋值进程ID
                    break;
                }
            }

            if (_procId == 0)//当进程ID为0时
            {
                return DllInjectionResult.GameProcessNotFound;//返回一个未找到进程的状态
            }

            if (!bInject(_procId, sDllPath))
            {
                return DllInjectionResult.InjectionFailed;//返回注入失败。
            }

            return DllInjectionResult.Success;//返回注入成功。
        }

        /// <summary>
        /// 进行注入操作
        /// </summary>
        /// <param name="pToBeInjected">进程ID</param>
        /// <param name="sDllPath">需要注入的DLL路径</param>
        /// <returns></returns>
        private bool bInject(uint pToBeInjected, string sDllPath)
        {
            IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, pToBeInjected);

            if (hndProc == INTPTR_ZERO)
            {
                return false;
            }

            IntPtr lpLLAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");//动态链接库(DLL)中的输出库函数地址

            if (lpLLAddress == INTPTR_ZERO)
            {
                return false;
            }

            IntPtr lpAddress = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (0x1000 | 0x2000), 0X40);//虚拟化空间

            if (lpAddress == INTPTR_ZERO)
            {
                return false;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

            if (WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0) == 0)//将DLL写入进程
            {
                return false;
            }

            if (CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, lpLLAddress, lpAddress, 0, (IntPtr)null) == INTPTR_ZERO)//创建远程线程
            {
                return false;
            }

            CloseHandle(hndProc);//关闭一个内核对象。

            return true;
        }
    }
}
