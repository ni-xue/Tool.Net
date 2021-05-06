using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Web.Routing
{
    /// <summary>
    /// 对Api接口以及控制器使用路由配置
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]//| AttributeTargets.Property
    public class AshxRouteAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        public AshxRouteAttribute(string template) 
        {
            this.Template = template;
        }

        /// <summary>
        /// 路由规则
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string Name { get; set; }
    }
}
