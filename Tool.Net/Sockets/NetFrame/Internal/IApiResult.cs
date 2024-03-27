using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 接口协议
    /// </summary>
    public interface IApiResult
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public Dictionary<string, ApiValue> Keys { get; }

        /// <summary>
        /// 流资源
        /// </summary>
        public System.IO.Stream Stream { get; set; }
    }

    /// <summary>
    /// 存储对象
    /// </summary>
    public readonly struct ApiValue 
    {
        /// <summary>
        /// 空值原型
        /// </summary>
        public static readonly ApiValue Empty = "Null";

        /// <summary>
        /// 原始类型值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 当前对象原型
        /// </summary>
        public ValueType Type { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="value"></param>
        public ApiValue(object value) : this()
        {
            Type = GetValueType(value.GetType());
            this.Value = value;
        }

        /// <summary>
        /// 将对象还原成 特定值
        /// </summary>
        /// <typeparam name="T">转换的值</typeparam>
        /// <returns>得到的值</returns>
        public T GetVar<T>() 
        {
            return Value.ToVar<T>();
        }

        /// <summary>
        /// 根据 类型获取对应的枚举
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>对应的枚举</returns>
        public static ValueType GetValueType(Type type) 
        {
            return type.Name switch
            {
                "String" => ValueType.String,
                "Int32" => ValueType.Int,
                "Int64" => ValueType.Long,
                "Double" => ValueType.Double,
                "Decimal" => ValueType.Decimal,
                "Byte" => ValueType.Byte,
                "DateTime" => ValueType.DateTime,
                "List`1" => ValueType.List,
                "Dictionary`2" => ValueType.Dictionary,
                _ => throw new Exception("当前插入值，并不包含在枚举类型中！"),
            };
        }

        /// <summary>
        /// 用于表示当前类型的实际值
        /// </summary>
        public enum ValueType : byte
        {
            /// <summary>
            /// <see cref="string"/>
            /// </summary>
            String = 0,
            /// <summary>
            /// <see cref="int"/>
            /// </summary>
            Int = 1,
            /// <summary>
            /// <see cref="long"/>
            /// </summary>
            Long = 2,
            /// <summary>
            /// <see cref="double"/>
            /// </summary>
            Double = 3,
            /// <summary>
            /// <see cref="decimal"/>
            /// </summary>
            Decimal = 4,
            /// <summary>
            /// <see cref="byte"/>
            /// </summary>
            Byte = 5,
            /// <summary>
            /// <see cref="System.DateTime"/>
            /// </summary>
            DateTime = 6,
            /// <summary>
            /// <see cref="System.Collections.Generic.List{T}"/>
            /// </summary>
            List = 7,
            /// <summary>
            /// <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>
            /// </summary>
            Dictionary = 8
        }

        /// <summary>
        /// 获取值的结果
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(string value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(ApiValue value) => value.GetVar<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(DateTime value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator DateTime(ApiValue value) => value.GetVar<DateTime>();

        //public static implicit operator ApiValue(DateTimeOffset value) => new(value);

        //public static implicit operator DateTimeOffset(ApiValue value) => value.GetVar<DateTimeOffset>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(byte value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator byte(ApiValue value) => value.GetVar<byte>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(int value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator int(ApiValue value) => value.GetVar<int>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(long value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator long(ApiValue value) => value.GetVar<long>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(double value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator double(ApiValue value) => value.GetVar<double>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(decimal value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator decimal(ApiValue value) => value.GetVar<decimal>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(List<ApiValue> value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator List<ApiValue>(ApiValue value) => value.GetVar<List<ApiValue>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiValue(Dictionary<string, ApiValue> value) => new(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Dictionary<string, ApiValue>(ApiValue value) => value.GetVar<Dictionary<string, ApiValue>>();
    }
}
