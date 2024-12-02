using System;
using System.Runtime.InteropServices;

[assembly: ComVisible(true)]
[assembly: Guid("00D70EFE-32AC-43D9-96FA-A3E8CC4A79BB")]

namespace Tool.Net
{
    /// <summary>
    /// 包相关信息说明类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class Explains
    {
        /// <summary>
        /// 包体名称
        /// </summary>
        public const string PackageName = "Tool.Net";

        /// <summary>
        /// 作者名称
        /// </summary>
        public const string AuthorName = "NiXue";

        /// <summary>
        /// 作者开发说明
        /// </summary>
        public const string Description = @"该项目不会在变了，一路走来，已经经历了两个春夏秋冬，一路走来，感谢很多伙伴的支持，
也使用了几个名字了，在这里我依次罗列出：【UniversalFrame，UniversalFrame.Core】，还有一个名字这里就不提及了，这两个名字均能在NuGet上搜索到，
现在已经确定了名字‘Tool.Net’这包的名字好记且符合，框架本身定义，从.Net5开始支持下去。前面的版本请使用以前的版本，最新版本只支持.Net5及之后。";
    }
    //sn -i "nixue.Tool.pfx" VS_KEY_57DAB3E1397282F0 强命名注册(在注册目录运行vs配置工具)
}
