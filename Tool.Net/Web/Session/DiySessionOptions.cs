using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Web.Session
{
    /// <summary>
    /// 自定义的Session对象，必须完成的实现方法
    /// </summary>
    public class DiySessionOptions
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        public DiySessionOptions()
        {

        }

        /// <summary>
        /// 表明Session存储名称
        /// </summary>
        public string SessionName { get; set; } = "NiXue.Session";

        /// <summary>
        /// 自定义Session必须完成的注册流程
        /// </summary>
        /// <typeparam name="T">实现的基类</typeparam>
        public void GetDiySession<T>() where T : DiySession, new() //: new()
        {
            TypeDiySession = typeof(T);
        }

        internal Type TypeDiySession { get; set; }
    }
}
