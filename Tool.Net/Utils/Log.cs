using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils
{
    /// <summary>
    /// 日志工具类
    /// </summary>
    public sealed class Log
    {
        ///日志文件路径 
        public const string LogFilePath = @"Log\";//Error/

        /// <summary>
        /// 记录消息Queue
        /// </summary>
        private readonly ConcurrentQueue<FlashLogMessage> _que;

        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEvent _mre;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly StringBuilder _log = new();

        /// <summary>
        /// 日志
        /// </summary>
        private static readonly Log _flashLog = new();

        ///// <summary>
        ///// 日志线程
        ///// </summary>
        //private readonly Thread _logthread;

        /// <summary>
        /// 日志线程
        /// </summary>
        private readonly Task _logtask;

        /// <summary>
        /// 日志任务是否已经启动了（没有启动时 false，启动后发生意外时 false， 其余时候一直都是 true ）
        /// </summary>
        public bool IsAlive { get; private set; } = false;

        /// <summary>
        /// 当前的存放地址
        /// </summary>
        private readonly string _directory;

        ///// <summary>
        ///// 当前锁
        ///// </summary>
        //private readonly object _lockobj;

        /// <summary>
        /// 用于打印异常信息时，忽略掉堆栈中的部分方法。
        /// </summary>
        public HashSet<string> IgnoreMethodName { get; }

        /// <summary>
        /// 是否在方法名为《MoveNext》时验证是否是异步函数生成，获取显示实际名称
        /// </summary>
        public bool IsMoveNext { set; get; }

        private Log()
        {
            _directory = Path.Combine(Environment.CurrentDirectory, LogFilePath); //AppDomain.CurrentDomain.BaseDirectory

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            IsMoveNext = true;

            IgnoreMethodName = new(new string[]
            {
                "System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw",
                "System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess",
                "System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification",
                "System.Runtime.CompilerServices.TaskAwaiter`1.GetResult"
            });

            //if (!configFile.Exists)
            //{
            //    throw new Exception("未配置log4net配置文件！");
            //}

            // 设置日志配置文件路径
            //XmlConfigurator.Configure(configFile);

            //_lockobj = new object();

            _que = new ConcurrentQueue<FlashLogMessage>();
            _mre = new ManualResetEvent(false);

            //System.Diagnostics.Debug.Write("日志模块结束!");

            _logtask = Task.Run(WriteLog).ContinueWith((task) =>
            {
                IsAlive = false;
                System.Diagnostics.Debug.Fail("日志模块结束!");
            });//TaskCreationOptions.LongRunning

            //_logthread = new Thread(asynclog)
            //{
            //    Name = "日志线程",
            //    IsBackground = true,//false
            //    Priority = ThreadPriority.Lowest
            //};

            //有点可怕，可以获取到当前对象详情信息
            //var sd = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

            //_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 实现单例,不建议直接调用。
        /// </summary>
        public static Log Instance => _flashLog;
        //{
        //    get
        //    {
        //        if (_flashLog == null)
        //        {
        //            lock (_lockobj)
        //            {
        //                if (_flashLog == null)
        //                {
        //                    _flashLog = new Log();
        //                }
        //            }
        //        }
        //        return _flashLog;
        //    }
        //}

        ///// <summary>
        ///// 另一个线程记录日志，只在程序初始化时调用一次
        ///// </summary>
        //public void Register()
        //{
        //    Thread t = new Thread(new ThreadStart(WriteLog));
        //    t.IsBackground = false;
        //    t.Start();
        //}

        /// <summary>
        /// 从队列中写日志至磁盘
        /// </summary>
        private async Task WriteLog()
        {
            Thread.CurrentThread.Name ??= "日志线程";

            IsAlive = true;

            while (true)
            {
                // 等待信号通知
                _mre.WaitOne();

                // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
                while (!_que.IsEmpty && _que.TryDequeue(out FlashLogMessage msg))
                {
                    // 判断日志等级，然后写日志
                    switch (msg.Level)
                    {
                        case FlashLogLevel.Debug:
                            _log.Append($"[Debug { msg.LogDateTime }]\r\n");
                            _log.Append($"  描述：{ msg.Message }\r\n");

                            if (msg.Exception != null)
                            {
                                ToStringExceptions(msg.Exception);
                                //var stackframes = StackFrame(msg.Exception);
                                //foreach (StackFrame stackframe in stackframes)
                                //{
                                //    _log.Append($" 当前堆栈异常方法名：{stackframe.GetMethod().Name}\r\n");
                                //    _log.Append($" 异常方法：{msg.Exception.TargetSite.DeclaringType.FullName} .{msg.Exception.TargetSite.Name}\r\n");//DeclaringType.FullName
                                //    _log.Append($" 异常行号：{stackframe.GetFileLineNumber()},{stackframe.GetFileColumnNumber()}\r\n"); //stackframe.GetFileName()获取文件名
                                //    _log.Append($" 异常提示：{msg.Exception.Message}\r\n");//DeclaringType.FullName
                                //}
                            }

                            await Write("Debug", msg.LogFilePath);
                            break;
                        case FlashLogLevel.Info:
                            _log.Append($"[Info { msg.LogDateTime }]\r\n");
                            _log.Append($"  描述：{ msg.Message }\r\n");

                            if (msg.Exception != null)
                            {
                                ToStringExceptions(msg.Exception);
                                //var stackframes = StackFrame(msg.Exception);
                                //foreach (StackFrame stackframe in stackframes)
                                //{
                                //    _log.Append($" 当前堆栈异常方法名：{stackframe.GetMethod().Name}\r\n");
                                //    _log.Append($" 异常方法：{msg.Exception.TargetSite.DeclaringType.FullName} .{msg.Exception.TargetSite.Name}\r\n");//DeclaringType.FullName
                                //    _log.Append($" 异常行号：{stackframe.GetFileLineNumber()},{stackframe.GetFileColumnNumber()}\r\n");
                                //    _log.Append($" 异常提示：{msg.Exception.Message}\r\n");//DeclaringType.FullName
                                //}
                            }

                            await Write("Info", msg.LogFilePath);
                            break;
                        case FlashLogLevel.Error:
                            _log.Append($"[Error { msg.LogDateTime }]\r\n");
                            _log.Append($"  描述：{ msg.Message }\r\n");

                            if (msg.Exception != null)
                            {
                                ToStringExceptions(msg.Exception);
                                //var stackframes = StackFrame(msg.Exception);
                                //foreach (StackFrame stackframe in stackframes)
                                //{
                                //    _log.Append($" 当前堆栈异常方法名：{stackframe.GetMethod().Name}\r\n");
                                //    _log.Append($" 异常方法：{msg.Exception.TargetSite.DeclaringType.FullName} .{msg.Exception.TargetSite.Name}\r\n");//DeclaringType.FullName
                                //    _log.Append($" 异常行号：{stackframe.GetFileLineNumber()},{stackframe.GetFileColumnNumber()}\r\n");
                                //    _log.Append($" 异常提示：{msg.Exception.Message}\r\n");//DeclaringType.FullName
                                //}
                            }

                            await Write("Error", msg.LogFilePath);
                            break;
                        case FlashLogLevel.Warn:
                            _log.Append($"[Warn { msg.LogDateTime }]\r\n");
                            _log.Append($"  描述：{ msg.Message }\r\n");

                            if (msg.Exception != null)
                            {
                                ToStringExceptions(msg.Exception);
                                //var stackframes = StackFrame(msg.Exception);
                                //foreach (StackFrame stackframe in stackframes)
                                //{
                                //    _log.Append($" 当前堆栈异常方法名：{stackframe.GetMethod().Name}\r\n");
                                //    _log.Append($" 异常方法：{msg.Exception.TargetSite.DeclaringType.FullName} .{msg.Exception.TargetSite.Name}\r\n");//DeclaringType.FullName
                                //    _log.Append($" 异常行号：{stackframe.GetFileLineNumber()},{stackframe.GetFileColumnNumber()}\r\n");
                                //    _log.Append($" 异常提示：{msg.Exception.Message}\r\n");//DeclaringType.FullName
                                //}
                            }

                            await Write("Warn", msg.LogFilePath);
                            break;
                        case FlashLogLevel.Fatal:
                            _log.Append($"[Fatal { msg.LogDateTime }]\r\n");
                            _log.Append($"  描述：{ msg.Message }\r\n");

                            if (msg.Exception != null)
                            {
                                ToStringExceptions(msg.Exception);
                                //var stackframes = StackFrame(msg.Exception);
                                //foreach (StackFrame stackframe in stackframes)
                                //{
                                //    _log.Append($" 当前堆栈异常方法名：{stackframe.GetMethod().Name}\r\n");
                                //    _log.Append($" 异常方法：{msg.Exception.TargetSite.DeclaringType.FullName} .{msg.Exception.TargetSite.Name}\r\n");//DeclaringType.FullName
                                //    _log.Append($" 异常行号：{stackframe.GetFileLineNumber()},{stackframe.GetFileColumnNumber()}\r\n");
                                //    _log.Append($" 异常提示：{msg.Exception.Message}\r\n");//DeclaringType.FullName
                                //}
                            }

                            await Write("Fatal", msg.LogFilePath);
                            break;
                    }
                }

                // 重新设置信号
                _mre.Reset();
                //Thread.Sleep(1);

                //await Task.Delay(1);
            }
        }

        private static StackFrame[] StackFrame(Exception exception)
        {
            StackTrace st = new(exception, true);
            return st.GetFrames();
        }

        private void ToStringExceptions(Exception exception)
        {
            Exception Exception = exception;
        Aex:
            try
            {
                bool IsException = false;
                do
                {
                    IsException = false;
                    var stackframes = StackFrame(Exception);

                    if (Exception.TargetSite != null)
                    {
                        _log.Append($"  异常方法：{ (Exception.TargetSite.DeclaringType != null ? Exception.TargetSite.DeclaringType.FullName : "无") }.{Exception.TargetSite.Name}\r\n");//DeclaringType.FullName

                        if (stackframes != null)
                        {
                            if (stackframes.LongLength > 1)
                            {
                                Array.Reverse(stackframes);
                            }
                            int i = 1;//下标
                            foreach (StackFrame stackframe in stackframes)
                            {
                                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == stackframe.GetILOffset()) break;

                                var method = stackframe.GetMethod();
                                //System.Runtime.CompilerServices.IAsyncStateMachine System.Runtime.CompilerServices.CompilerGeneratedAttribute

                                string _namespace = method.DeclaringType.Namespace ?? "", _name = method?.DeclaringType.Name, _methodname = method.Name, outname = string.Empty;

                                if (IsMoveNext && _methodname.Equals("MoveNext") && _name[0].Equals('<'))
                                {
                                    int head = 1, last = _name.LastIndexOf('>');
                                    last = last != -1 ? last : _name.Length;
                                    outname = $"{_namespace}.{method.DeclaringType?.ReflectedType?.Name}.{_name[head..last]}";
                                }
                                else
                                {
                                    outname = $"{_namespace}.{_name}.{_methodname}";
                                }

                                if (IgnoreMethodName.Contains(outname)) continue;

                                _log.Append($"    （{i}）当前堆栈异常方法名：{outname}\r\n");
                                int lineNumber = stackframe.GetFileLineNumber();
                                int columnNumber = stackframe.GetFileColumnNumber();
                                if (lineNumber > 0 || columnNumber > 0)
                                {
                                    _log.Append($"    异常行号：{lineNumber},下标位置：{columnNumber}\r\n");
                                }
                                i++;
                            }
                        }

                        _log.Append($"  异常提示：{Exception.Message}\r\n");//DeclaringType.FullName
                    }
                    else
                    {
                        //_log.Append($"  异常方法：没有方法信息\r\n");//DeclaringType.FullName//无法获取，出现该情况一般是自定义异常的原因
                        //if (Exception.GetType() == typeof(AshxException))
                        //{
                        //    AshxException ashxException = Exception as AshxException;
                        //    _log.Append($"  异常方法：{ (ashxException.TargetSite.DeclaringType != null ? ashxException.TargetSite.DeclaringType.FullName : "无") }.{ashxException.TargetSite.Name}\r\n");//DeclaringType.FullName
                        //}
                        //else
                        //{
                        //    _log.Append($"  异常方法：无法获取，出现该情况一般是自定义异常的原因\r\n");//DeclaringType.FullName
                        //}

                        _log.Append($"  异常提示：{Exception}\r\n");
                    }

                    if (Exception.InnerException != null)
                    {
                        _log.Append("-----------------------------------------------------------------------------------------------\r\n");
                        Exception = Exception.InnerException;
                        IsException = true;
                    }
                } while (IsException);
            }
            catch (Exception e)
            {
                Exception = e;
                _log.Clear();
                _log.Append("  特殊情况，在获取任务异常信息的时候，发生了错误，下面是错误引发的原因。  \r\n");
                goto Aex;

                //bool IsException;
                //do
                //{
                //    IsException = false;
                //    var stackframes = StackFrame(Exception);

                //    if (Exception.TargetSite != null)
                //    {
                //        _log.Append($"  异常方法：{ (Exception.TargetSite.DeclaringType != null ? Exception.TargetSite.DeclaringType.FullName : "无") }.{Exception.TargetSite.Name}\r\n");//DeclaringType.FullName
                //    }
                //    else
                //    {
                //        _log.Append($"  异常方法：没有方法信息\r\n");//DeclaringType.FullName //无法获取，出现该情况一般是自定义异常的原因
                //        //if (Exception.GetType() == typeof(AshxException))
                //        //{
                //        //    AshxException ashxException = Exception as AshxException;
                //        //    _log.Append($"  异常方法：{ (ashxException.TargetSite.DeclaringType != null ? ashxException.TargetSite.DeclaringType.FullName : "无") }.{ashxException.TargetSite.Name}\r\n");//DeclaringType.FullName
                //        //}
                //        //else
                //        //{
                //        //    _log.Append($"  异常方法：无法获取，出现该情况一般是自定义异常的原因\r\n");//DeclaringType.FullName
                //        //}
                //    }

                //    if (stackframes != null)
                //    {
                //        if (stackframes.LongLength > 1)
                //        {
                //            Array.Reverse(stackframes);
                //        }
                //        int i = 1;//下标
                //        foreach (StackFrame stackframe in stackframes)
                //        {
                //            _log.Append($"    （{i}）当前堆栈异常方法名：{stackframe.GetMethod().Name}\r\n");
                //            int lineNumber = stackframe.GetFileLineNumber();
                //            int columnNumber = stackframe.GetFileColumnNumber();
                //            if (lineNumber > 0 || columnNumber > 0)
                //            {
                //                _log.Append($"    异常行号：{lineNumber},下标位置：{columnNumber}\r\n");
                //            }
                //            i++;
                //        }
                //    }

                //    _log.Append($"  异常提示：{Exception.Message}\r\n");//DeclaringType.FullName

                //    if (Exception.InnerException != null)
                //    {
                //        _log.Append("-----------------------------------------------------------------------------------------------\r\n");
                //        Exception = Exception.InnerException;
                //        IsException = true;
                //    }
                //} while (IsException);
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="LevelName">日志类型</param>
        /// <param name="LogFilePath">提供的是绝对路径</param>
        private async Task Write(string LevelName, string LogFilePath)
        {
            int tryi = 0;

            A:
            try
            {
                tryi++;
                string directory = string.Empty;

                if (string.IsNullOrWhiteSpace(LogFilePath))
                {
                    directory = Path.Combine(_directory, $@"{LevelName}\");
                }
                else
                {
                    directory = Path.Combine(LogFilePath, $@"{LevelName}\");
                }

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                // 检查当前日志文件大小 超过30M则创建一个新的日志文件
                //string fullPath = $"{directory}{LevelName}Log{DateTime.Now:yyyy-MM}.log";
                uint i = 0;
                string fullPath = FileManager.GetCurrentLogName(directory, LevelName, ref i, out string sparepath);//fullPath
                FileInfo fi = new(fullPath);
                if (fi.Exists && fi.Length > 30 * 1024 * 1024)
                    fullPath = sparepath;//$"{directory}{LevelName}Log{DateTime.Now:yyyy-MM}[{i}].log";

                // 写入日志
                await File.AppendAllTextAsync(fullPath, $"{_log}———————————————————————————————————————\r\n", Encoding.Unicode);

                _log.Clear();
            }
            catch (Exception e)
            {
                if (tryi <= 10 && e.Message.EndsWith("because it is being used by another process."))
                {
                    Thread.Sleep(100);
                    goto A;
                }
                else
                {
                    System.Diagnostics.Debug.Write("日志打印异常：" + e);
                    _log.Clear();
                }
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志文本</param>
        /// <param name="level">等级</param>
        /// <param name="ex">Exception</param>
        /// <param name="logFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public void EnqueueMessage(string message, FlashLogLevel level, Exception ex = null, string logFilePath = null)
        {
            try
            {
                //lock (_lockobj)
                //{
                    if (_logtask.IsCompleted) throw new Exception("日志线程，在未知原因情况下结束了！");
                    //{
                    //    _logtask.Start();
                    //}

                    //if (!_logthread.IsAlive && _logthread.ThreadState == System.Threading.ThreadState.Unstarted)
                    //{
                    //    _logthread.Start(); //Stopped
                    //}

                    //if ((level == FlashLogLevel.Debug)
                    // || (level == FlashLogLevel.Error)
                    // || (level == FlashLogLevel.Fatal)
                    // || (level == FlashLogLevel.Info)
                    // || (level == FlashLogLevel.Warn))
                    //{
                    if (!_mre.SafeWaitHandle.IsClosed) { 
                        _que.Enqueue(new FlashLogMessage
                        {
                            LogDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                            Message = message,
                            Level = level,
                            Exception = ex,
                            LogFilePath = logFilePath
                        });
                        _mre.Set();// 通知线程往磁盘中写日志
                    }
                    //}
                //}
            }
            catch (Exception e) when (!e.Message.Equals("日志线程，在未知原因情况下结束了！"))
            {
                //EnqueueMessage(e.Message, level, e, logFilePath);
                System.Diagnostics.Debug.Write("日志打印异常：" + e);
            }
        }

        /// <summary>
        /// 验证路径是否存在不正常的情况，并返回正常的路径信息
        /// </summary>
        /// <param name="LogFilePath"></param>
        /// <returns></returns>
        private static string GetLogFilePath(string LogFilePath)
        {
            if (Path.HasExtension(LogFilePath))
            {
                throw new System.Exception("使用的路径中包括文件扩展名，请纠正，只能使用路径文件夹。");
            }
            try
            {
                if (Path.IsPathRooted(LogFilePath))
                {
                    if (!Directory.Exists(LogFilePath))
                    {
                        Directory.CreateDirectory(LogFilePath);
                    }
                }
                else
                {
                    LogFilePath = Path.Combine(Environment.CurrentDirectory, LogFilePath);//AppDomain.CurrentDomain.BaseDirectory
                    if (!Directory.Exists(LogFilePath))
                    {
                        Directory.CreateDirectory(LogFilePath);
                    }
                }
            }
            catch
            {
                LogFilePath = null;
            }

            return LogFilePath;
        }

        /// <summary>
        /// 一般日志输出
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Debug(string msg)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Debug);
        }

        /// <summary>
        /// 一般日志输出
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Debug(string msg, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Debug, null, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 一般日志输出
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        public static void Debug(string msg, Exception ex)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Debug, ex);
        }

        /// <summary>
        /// 一般日志输出
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Debug(string msg, Exception ex, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Debug, ex, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 异常错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Error(string msg)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Error);
        }

        /// <summary>
        /// 异常错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Error(string msg, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Error, null, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 异常错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        public static void Error(string msg, Exception ex)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Error, ex);
        }

        /// <summary>
        /// 异常错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Error(string msg, Exception ex, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Error, ex, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 致命的错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Fatal(string msg)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Fatal);
        }

        /// <summary>
        /// 致命的错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Fatal(string msg, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Fatal, null, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 致命的错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        public static void Fatal(string msg, Exception ex)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Fatal, ex);
        }

        /// <summary>
        /// 致命的错误
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Fatal(string msg, Exception ex, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Fatal, ex, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Info(string msg)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Info);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Info(string msg, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Info, null, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        public static void Info(string msg, Exception ex)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Info, ex);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Info(string msg, Exception ex, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Info, ex, GetLogFilePath(LogFilePath));
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Warn(string msg)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Warn);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Warn(string msg, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Warn, null, GetLogFilePath(LogFilePath));
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        public static void Warn(string msg, Exception ex)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Warn, ex);
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        /// <param name="LogFilePath">提供的路径可以是相对路径也可以是绝对路径</param>
        public static void Warn(string msg, Exception ex, string LogFilePath)
        {
            Instance.EnqueueMessage(msg, FlashLogLevel.Warn, ex, GetLogFilePath(LogFilePath));
        }
    }

    /// <summary>
    /// 日志等级
    /// </summary>
    public enum FlashLogLevel
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug,
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 异常
        /// </summary>
        Error,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 致命的
        /// </summary>
        Fatal
    }

    /// <summary>
    /// 日志内容
    /// </summary>
    public record FlashLogMessage
    {
        /// <summary>
        /// 记录时间
        /// </summary>
        public string LogDateTime { get; init; }
        /// <summary>
        /// 输出内容
        /// </summary>
        public string Message { get; init; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public FlashLogLevel Level { get; init; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; init; }
        /// <summary>
        /// 日志存放路径，默认为空，是默认路径可以填写（绝对的日志存放路径）
        /// </summary>
        public string LogFilePath { get; init; }
    }
}
