using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tool.Utils
{
    /// <summary>
    /// 获取配置文件数据
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 主配置文件名
        /// </summary>
        public const string FileName = "appsettings.json";

        /// <summary>
        /// 调试配置文件名
        /// </summary>
        public const string FileNameDevelopment = "appsettings.Development.json";

        private const string WebConfigHasNotAddKey = "当前项目配置 “.config” 下面的appSettings节点不包含，{0}字段，请检查配置文件。";

        /// <summary>
        /// 项目配置文件获取对象
        /// </summary>
        public static IConfigurationRoot Configuration { get; }

        /// <summary>
        /// 自动注入
        /// </summary>
        static AppSettings()
        {
            try
            {
                if (IsBuild(Environment.CurrentDirectory, out IConfigurationRoot configurationRoot)) 
                {
                    Configuration = configurationRoot;
                }
                else if (IsBuild(AppContext.BaseDirectory, out configurationRoot))
                {
                    Configuration = configurationRoot;
                }
                
                //var builder = new ConfigurationBuilder().AddJsonFile(fileName, false, true);
                //directory = AppContext.BaseDirectory;//.Replace("\\", "/");

                //var filePath = Path.Combine(directory, fileNameDevelopment);// $"{directory}\\{fileNameDevelopment}";
                //if (File.Exists(filePath))
                //{
                //    builder.AddJsonFile(fileNameDevelopment, false, true);
                //}
                //IConfigurationRoot configurationRoot = builder.Build();
                //Configuration = configurationRoot;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Trace.TraceError(ex.Message);
                System.Diagnostics.Debug.Write(ex.Message);
            }

            static bool IsBuild(string directory, out IConfigurationRoot configurationRoot)
            {
                string filePath = Path.Combine(directory, FileName);
                if (File.Exists(filePath))
                {
                    var builder = new ConfigurationBuilder().AddJsonFile(filePath, false, true);

                    filePath = Path.Combine(directory, FileNameDevelopment);
                    if (File.Exists(filePath))
                    {
                        builder.AddJsonFile(filePath, false, true);
                    }
                    configurationRoot = builder.Build();
                    //Configuration = configurationRoot;
                    return true;
                }
                configurationRoot = null;
                return false;
            }
        }
        


        /// <summary>
        /// 根据你提供的地址获取配置文件信息
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        /// <returns>返回指定对象</returns>
        public static IConfiguration AddJsonFile(string filePath)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(filePath, false, true);
            return builder.Build();
        }

        /// <summary>
        /// 获取appSettings的value
        /// </summary>
        /// <param name="key">名称</param>
        /// <param name="throwOnError">true 表示在找不到该键值时引发异常；false 则表示返回 null。</param>
        /// <returns>返回值，如果键值名称不存在则返回 <seealso cref="Nullable"/> </returns>
        public static string Get(string key, bool throwOnError = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            string text = Configuration?.GetSection(key).Value; //ConfigurationManager.AppSettings[key];
            if (text == null)
            {
                if (throwOnError)
                {
                    throw new Exception(string.Format(WebConfigHasNotAddKey, key));
                }
                else
                {
                    return null;
                }
            }
            return text;
        }

        /// <summary>
        /// 获取appSettings的value
        /// </summary>
        /// <param name="key">名称</param>
        /// <param name="throwOnError">true 表示在找不到该键值时引发异常；false 则表示返回 null。</param>
        /// <returns>返回值，如果键值名称不存在则返回0</returns>
        public static int GetInt(string key, bool throwOnError = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                return 0;
            }
            string text = Configuration?.GetSection(key).Value;
            if (text == null)
            {
                if (throwOnError)
                {
                    throw new Exception(string.Format(WebConfigHasNotAddKey, key));
                }
                else
                {
                    return 0;
                }
            }
            return text.ToInt();
        }

        //public static string this[string key] 
        //{
        //    _configuration[ key] 
        //}

        /// <summary>
        /// 获取具有指定键的配置子节。
        /// </summary>
        /// <param name="key">配置部分的键。</param>
        /// <returns>这个 Microsoft.Extensions.Configuration.IConfigurationSection</returns>
        public static IConfigurationSection GetSection(string key)
        {
            return Configuration?.GetSection(key);
        }

        /// <summary>
        /// 获取直接子代配置子节。
        /// </summary>
        /// <returns>配置子部分。</returns>
        public static IEnumerable<IConfigurationSection> GetChildren()
        {
            return Configuration?.GetChildren();
        }

        /// <summary>
        /// 返回Microsoft.Extensions.Primitives.IChangeToken，可用于观察重新加载此配置时。
        /// </summary>
        /// <returns>一个Microsoft.Extensions.Primitives.IChangeToken。</returns>
        public static IChangeToken GetReloadToken()
        {
            return Configuration?.GetReloadToken();
        }
    }
}
