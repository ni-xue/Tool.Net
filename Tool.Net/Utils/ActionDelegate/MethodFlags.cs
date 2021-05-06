using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 方法查询的定义
    /// </summary>
    [Flags]
    public enum MethodFlags : short
    {
        /// <summary>
        /// 不指定绑定标志。（默认获取全部）
        /// </summary>
        Default = 0,
        /// <summary>
        /// 静态方法
        /// </summary>
        Static = 1,
        /// <summary>
        /// 是否包含父类
        /// </summary>
        Base = 2,
        /// <summary>
        /// 公开方法
        /// </summary>
        Public = 4,
        /// <summary>
        /// 私有方法
        /// </summary>
        Private = 8,
        /// <summary>
        /// 继承方法（受保护）
        /// </summary>
        Protected = 16,
        /// <summary>
        /// 当前程序集方法（内部）
        /// </summary>
        Internal = 32
    }
}
