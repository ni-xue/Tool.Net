using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tool.Utils.ThreadQueue;

namespace Tool.Utils
{
    /// <summary>
    /// 获取配置文件数据 (允许修改原文件异步队列式更新)
    /// </summary>
    public sealed class AppSettings
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
        /// 获取当前配置文件的路径
        /// </summary>
        public static string CurrentDirectory { get; }

        /// <summary>
        /// 自动注入
        /// </summary>
        static AppSettings()
        {
            try
            {
                if (IsBuild(Environment.CurrentDirectory, out IConfigurationRoot configurationRoot))
                {
                    CurrentDirectory = Environment.CurrentDirectory;
                    Configuration = configurationRoot;
                }
                else if (IsBuild(AppContext.BaseDirectory, out configurationRoot))
                {
                    CurrentDirectory = AppContext.BaseDirectory;
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
                    var builder = new ConfigurationBuilder().Add(WritableJsonConfigurationSource.AddProvider(filePath));//.AddJsonFile(filePath, false, true);

                    filePath = Path.Combine(directory, FileNameDevelopment);
                    if (File.Exists(filePath))
                    {
                        builder.Add(WritableJsonConfigurationSource.AddProvider(filePath));//.AddJsonFile(filePath, false, true);
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

        /// <summary>
        /// 获取key值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] => Configuration[key];

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

        /// <summary>
        /// 注册修改 IConfiguration 值的事件 （将取消默认实现的同步修改文件）
        /// </summary>
        public static event Action<string, string> SetEvent;

        internal static bool IsSetEvent => SetEvent is not null;

        internal static bool OnSet(string key, string value)
        {
            if (SetEvent is not null)
            {
                SetEvent(key, value);
                return true;
            }
            return false;
        }
    }

    internal class WritableJsonConfigurationProvider : JsonConfigurationProvider
    {
        private readonly TaskQueue<ValueTuple<string, string>> taskOueue;
        private readonly string fileFullPath;

        public WritableJsonConfigurationProvider(WritableJsonConfigurationSource source) : base(source)
        {
            fileFullPath = base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
            taskOueue = new(SetAsync);
        }

        public override void Set(string key, string value)
        {
            //base.Set(key, value);
            taskOueue.Add((key, value));
        }

        private bool Exists(string key, string isvalue)
        {
            if (base.TryGet(key, out string val))
            {
                return string.Equals(val, isvalue, StringComparison.Ordinal);
            }
            else
            {
                //ArrayList arrayList = new();
                //Dictionary<string, string> keys = new();
                foreach (var _key in Data.Keys)
                {
                    if (_key.StartsWith(key, StringComparison.OrdinalIgnoreCase))// && base.TryGet(_key, out string _val))
                    {
                        //string sectionKey = ConfigurationPath.GetSectionKey(_key);
                        //if (sectionKey)
                        //{

                        //}
                        return true; //keys.Add(_key, _val);
                    }
                }
            }
            return false;
        }

        private async ValueTask SetAsync(ValueTuple<string, string> obj)
        {
            string key = obj.Item1, value = obj.Item2;

            //var iskey = _configuration.GetSection(key);
            //if (iskey.Exists() && !string.Equals(iskey.Value, value))
            if (Exists(key, value))
            {
                if (AppSettings.OnSet(key, value)) return;
                using FileStream fileStream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using JsonDocument jsonDocument = await JsonDocument.ParseAsync(fileStream, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
                if (JsonHelper.GetReturn(jsonDocument.RootElement) is Dictionary<string, object> keys)
                {
                    if (SetValue(keys, key, value))
                    {
                        fileStream.SetLength(0);
                        await JsonSerializer.SerializeAsync(fileStream, keys, GetOptions());//using StreamWriter streamWriter = new(fileStream);//await streamWriter.WriteAsync(keys.ToJson(GetOptions()));
                    }
                }
            }
        }

        private static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            };
            options.Converters.Add(JsonConverterHelper.GetDateConverter());
            options.Converters.Add(JsonConverterHelper.GetDBNullConverter());
            return options;
        }

        private static object RestoreType(string value)
        {
            try
            {
                if (value.StartsWith('{') || value.StartsWith('['))
                {
                    return value.JsonObject(new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
                }
                else
                {
                    if (value.EqualsNotCase("true")) return true;
                    if (value.EqualsNotCase("false")) return false;
                    if (value.EqualsNotCase("null")) return null;
                    if (long.TryParse(value, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var result0)) return result0;
                    if (double.TryParse(value, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var result1)) return result1;
                    return value;
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        private static bool IsEqualsValue(object val, string value)
        {
            if (val is Dictionary<string, object> or ArrayList)
            {
                return val.ToJson() != value;
            }
            return string.Concat(val) != value;
        }

        private static bool SetValue(Dictionary<string, object> keys, ReadOnlySpan<char> key, string value)
        {
            string name = key.ToString();
            if (keys.TryGetValue(name, out object val))
            {
                if (IsEqualsValue(val, value))
                {
                    keys[name] = RestoreType(value);
                    return true;
                }
            }
            else
            {
                int i = key.IndexOf(':');
                if (i != -1)
                {
                    if (keys.TryGetValue(key[..i].ToString(), out object _val))
                    {
                        if (_val is Dictionary<string, object> dic)
                        {
                            return SetValue(dic, key[(i + 1)..], value);
                        }
                        else if (_val is ArrayList list)
                        {
                            return SetValue(list, key[(i + 1)..], value);
                        }
                    }
                }
            }
            return false;
        }

        private static bool SetValue(ArrayList keys, ReadOnlySpan<char> key, string value)
        {
            if (TryGetList(keys, key, out int i, out object obj))
            {
                if (IsEqualsValue(obj, value))
                {
                    keys[i] = RestoreType(value);
                    return true;
                }
            }
            else
            {
                i = key.IndexOf(':');
                if (i != -1)
                {
                    if (TryGetList(keys, key[..i], out _, out object _val))
                    {
                        if (_val is Dictionary<string, object> dic)
                        {
                            return SetValue(dic, key[(i + 1)..], value);
                        }
                        else if (_val is ArrayList list)
                        {
                            return SetValue(list, key[(i + 1)..], value);
                        }
                    }
                }
            }
            return false;
        }

        private static bool TryGetList(ArrayList keys, ReadOnlySpan<char> key, out int i, out object obj)
        {
            if (int.TryParse(key, out i) && i < keys.Count)
            {
                obj = keys[i];
                return true;
            }
            i = -1;
            obj = null;
            return false;
        }
    }
    internal class WritableJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            this.EnsureDefaults(builder);
            return new WritableJsonConfigurationProvider(this);
        }

        public static IConfigurationSource AddProvider(string path)
        {
            WritableJsonConfigurationSource source = new()
            {
                FileProvider = null,
                Path = path,
                Optional = false,
                ReloadOnChange = true,
            };

            source.ResolveFileProvider();
            return source;// new WritableJsonConfigurationProvider(source);
        }
    }
}
