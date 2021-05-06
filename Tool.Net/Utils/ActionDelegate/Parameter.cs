using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tool.Utils.ActionDelegate
{
    /// <summary>
    /// 获取指定类下面的所有方法的成员，低级封装
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class Parameter
    {
        //Member.Name, Member.ParameterType.Name, Member.ParameterType.Namespace, Member.ParameterType, Member.DefaultValue

        ///// <summary>
        ///// 初始化，并赋值
        ///// </summary>
        ///// <param name="Name"></param>
        ///// <param name="Type"></param>
        ///// <param name="SpaceType"></param>
        ///// <param name="ParameterType"></param>
        ///// <param name="DefaultValue"></param>
        //internal Parameter(string Name, string Type, string SpaceType, Type ParameterType, object DefaultValue)
        //{
        //    this.Name = Name;
        //    this.Type = Type;
        //    this.SpaceType = SpaceType;
        //    this.ParameterType = ParameterType;
        //    this.DefaultValue = DefaultValue;
        //    this.ParameterObj = ParameterType.DefaultForType();
        //    this.IsType = ParameterType.IsType();
        //}

        /// <summary>
        /// 初始化，并赋值
        /// </summary>
        /// <param name="parameter">原对象模型</param>
        public Parameter(ParameterInfo parameter)
        {
            this.GetParameter = parameter;
            this.ParameterObj = ParameterType.DefaultForType();
            this.IsType = ParameterType.IsType();
        }

        /// <summary>
        /// 复制元数据到新的对象
        /// </summary>
        /// <param name="parameter"></param>
        public Parameter(Parameter parameter) 
        {
            this.GetParameter = parameter.GetParameter;
            this.IsType = parameter.IsType;
            this.ParameterObj = parameter.ParameterObj;
        }

        /// <summary>
        /// 参数对象原型
        /// </summary>
        public ParameterInfo GetParameter { get; }

        /// <summary>
        /// 变量名称
        /// </summary>
        public string Name => GetParameter.Name;

        /// <summary>
        /// 变量类型
        /// </summary>
        public string Type => GetParameter.ParameterType.Name;

        /// <summary>
        /// 变量类型的命名空间
        /// </summary>
        public string SpaceType => GetParameter.ParameterType.Namespace;

        /// <summary>
        /// 变量类型的<see cref="System.Type"/>
        /// </summary>
        public Type ParameterType => GetParameter.ParameterType;

        /// <summary>
        /// 参数默认值
        /// </summary>
        public object DefaultValue => GetParameter.DefaultValue;

        /// <summary>
        /// 变量类型是不是：（string，short，int, long, byte, bool, char, decimal, double, float, object, ushort, uint, ulong, DateTime 类型可以为加?）时为 true
        /// </summary>
        public bool IsType { get; }

        /// <summary>
        /// 值类型初始值
        /// </summary>
        public object ParameterObj { get; }
    }
}
