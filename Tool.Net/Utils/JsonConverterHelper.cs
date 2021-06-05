using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tool.Utils
{
    /// <summary>
    /// 获取 系统中可用的 Json 自定义转换对象
    /// </summary>
    public class JsonConverterHelper
    {
        /// <summary>
        /// 时间类型格式对象
        /// </summary>
        /// <param name="format">标准或自定义日期和时间格式字符串。</param>
        /// <returns><see cref="DateConverter"/></returns>
        public static DateConverter GetDateConverter(string format = "yyyy-MM-dd HH:mm:ss.fff")
        {
            return new DateConverter(format);
        }

        /// <summary>
        /// <see cref="DBNull"/> 将 {} 改Null输出
        /// </summary>
        /// <returns><see cref="DateConverter"/></returns>
        public static DBNullConverter GetDBNullConverter()
        {
            return new DBNullConverter();
        }
    }

    /// <summary>
    /// Json 标准或自定义日期和时间格式字符串。
    /// </summary>
    public class DateConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 标准或自定义日期和时间格式字符串。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// 注册 时间格式实例
        /// </summary>
        /// <param name="format"></param>
        public DateConverter(string format)
        {
            Format = format;
        }

        /// <summary>
        /// 将字符串转换成原数据
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParseExact(reader.GetString(), Format, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                {
                    return dateTime;
                }
            }

            throw new NotImplementedException("无法实现非 字符串 的数据。");
        }

        /// <summary>
        /// 将原数据转换成字符串
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }

        /// <summary>
        /// 验证是否支持类型
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }

    /// <summary>
    /// Json <see cref="DBNull"/> 将 {} 改Null输出
    /// </summary>
    public class DBNullConverter : JsonConverter<DBNull>
    {

        /// <summary>
        /// 注册 <see cref="DBNull"/> 将 {} 改Null输出
        /// </summary>
        public DBNullConverter()
        {

        }

        /// <summary>
        /// 将字符串转换成原数据
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DBNull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DBNull.Value;
        }

        /// <summary>
        /// 将原数据转换成字符串
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DBNull value, JsonSerializerOptions options)
        {
            writer.WriteNullValue();
        }

        /// <summary>
        /// 验证是否支持类型
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DBNull);
        }
    }
}

