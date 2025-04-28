using Microsoft.Extensions.Logging;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using Logs = Tool.Utils.Log;

namespace Tool.Web
{
    /// <summary>
    /// 日志输出扩展
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// 注册日志写入文本服务
        /// </summary>
        /// <param name="builder">日志服务</param>
        public static void AddLogSave(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, LocalFileLoggerProvider>();
        }

        /// <summary>
        /// 日志构造容器
        /// </summary>
        public class LocalFileLoggerProvider : ILoggerProvider
        {
            /// <inheritdoc/>
            public ILogger CreateLogger(string categoryName)
            {
                return new LocalFileLogger(categoryName);
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 注册写入日志文件
        /// </summary>
        public class LocalFileLogger : ILogger
        {
            readonly string path = Path.Combine("Log", "Logger");  //Path.Combine("Log", categoryName);
            readonly string categoryName;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="categoryName">服务名</param>
            public LocalFileLogger(string categoryName)
            {
                this.categoryName = categoryName;
            }

            /// <inheritdoc/>
            public IDisposable BeginScope<TState>(TState state)
            {
                return default!;
            }

            /// <inheritdoc/>
            public bool IsEnabled(LogLevel logLevel)
            {
                if (logLevel != LogLevel.None)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <inheritdoc/>
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (IsEnabled(logLevel))
                {
                    string msg = $"{categoryName}[{eventId.Id}]\n{formatter(state, exception)}";

                    switch (logLevel)
                    {
                        //case LogLevel.Trace:
                        case LogLevel.Critical:
                        case LogLevel.Debug:
                            Logs.Debug(msg, exception, path);
                            break;
                        case LogLevel.Information:
                            Logs.Info(msg, exception, path);
                            break;
                        case LogLevel.Error:
                            Logs.Error(msg, exception, path);
                            break;
                        case LogLevel.Warning:
                            Logs.Warn(msg, exception, path);
                            break;
                        default:
                            Logs.Fatal(msg, exception, path);
                            break;
                    }
                }
            }

            //public string Level(LogLevel logLevel)
            //{
            //    return logLevel switch
            //    {
            //        LogLevel.Trace => "trce",
            //        LogLevel.Debug => "dbug",
            //        LogLevel.Information => "info",
            //        LogLevel.Warning => "warn",
            //        LogLevel.Error => "fail",
            //        LogLevel.Critical => "crit",
            //        LogLevel.None => "none",
            //        _ => "",
            //    };
            //}
        }
    }
}
