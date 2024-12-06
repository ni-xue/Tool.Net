-------------------------------------功能简介-----------------------------------------

1.详细AIP文档：http://tool.nixue.top/index.html

2.框架包含功能（Web，Sql，Sockets，TypeExtension，Utils）

3. 当前版本（5.4.4）为正式版，用于任何项目开发使用。 

4. 架构基于 .Net9（兼容 .Net8、.Net7、.Net6、.Net5） 采用最新技术，具有最新的技术支持。

-------------------------------------功能详解-----------------------------------------

1. Web包含，Api创建，路由构建，可替代MVC写Api，和对Http多个扩展函数方便开发。

2. Sql包含，包含常用三种数据库的默认实现，其余数据库可以根据开源地址的实现，自行实现接口，支持所有数据库操作，提供获取原始数据库操作对象，由自行实现。

3. Sockets包含，Tcp，Udp，WebSocket，NetFrame（内置实现的一种和WPF差不多的消息传输协议，后期的维护重点）（NetFrame 重点，重构，对数据包体的高度使用，提高传输性能）

4. TypeExtension包含，bool,byte,char,datetime,decimal,double,fioat,int,long,object,short,string,uint,ulong,ushort，相关扩展函数。

5. Utils包含，ActionDelegate，Data，Encryption，FtpHelper，Other，TaskHelper，Log，等等一系列帮助类型函数。

5.1 大致包含： 委托，反射，Log，验证码生成，正则验证，DataSet/DataTable/DataRow（验证，To实体？ToJson?），AppSettings（获取Core下面的配置文件：appsettings.json），等等

6. 目前功能基本满足中大型规模开发。
-------------------------------------历史功能-----------------------------------------

1. MinApi 增加ApiOut.View(); ApiOut.ViewAsyn(); 结果重新调整

1.1 MinApi：Initialize函数改进，使用户可以直接返回结果。

1.2 HttpContext 提供扩展支持获取服务（HttpContext.GetService<>()）。

1.3 MinApi：修复了错误发生时抛出异常的显示，ApiOut改为抽象类提供实现。

1.4 修复在获取From表单的接口，在请求时未发送From表单而报错的问题。

2. 增加AsSession 内置Session体积小，（services.AddAsSession(); app.UseAsSession();）

3. Sql 增加DbTransactionExensions 提供扩展支持，增加DbHelperExensions 提供扩展支持。

3.1 IDbProvider接口再次简化，更加人性化，可选择自行实现，采用了默认实现，可替换。

3.2 ITableProvider数据结构验证，使其支持主流数据库无任何问题，有使用问题，尽可提问与我。

3.3 DbHelper 提供：TransExecuteNonQuery 开启事物处理的函数。

3.4 DbHelper 提供：GetInParam 原 MakeInParam 函数更名。（优化内置结构）

3.5 DbHelper 提供：CreateCommand(),CreateCommandBuilder(),CreateConnection(),CreateDataAdapter(),CreateParameter(),CreateTransaction() 可大大由执行实现特殊效果。

4. Dictionary 给键值对增加扩展(Remove,TryRemove,AsReadOnly),分别是删除多个，删除多个不抛异常，只读。

4.1 示例：var d = new Dictionary<string, string> { { "1", "111" }, { "2", "222" }, { "3", "333" } };
            d.Remove("1", "2");
            d.TryRemove(out string key, "1", "2", "3");
            d.AsReadOnly();
			
5. Xml 的数据处理：提供扩展（ToXml，Xml）

5.1 示例：  var d1 =new List<string>{ "5","10" };
            string xml = d1.ToXml();
            List<string> s = xml.Xml<List<string>>();
			
6. 增加属性读取/修改Api，扩展方法，高效操作，（GetValue，SetValue）

6.1 示例：  object sd = new { a = "嗨！", B = 5 };
            object d = sd.GetValue("a");
            bool b = sd.SetValue("b", 10, true);//对象匿名对象无法修改。
			
7. HttpHelpers 帮助类下增加异步请求。

8. 修复AsSession 无法使用等问题，增加相关操作日志，更加详细，增加许可证，可放心使用。

9. 新增 IServiceCollection 扩展，AddObject() 提供，数据持久化注册。方便操作更快捷。

9.1 新增 IApplicationBuilder 扩展 GetObject() 提供，获取服务的快捷方式。

10. 新增Api 请求状态提供 更多, Ashx.State

11. 优化跨域部分配置，让其更加全面。[CrossDomain] 新增跨域属性配置

12. 移除 Ashx.IsSession 因为为无法实现。

13. 特别声明最新版本Api 内核采用新的方式，更快了。

14. DbHelper 新增系统日志 ILogger 对象，在您提供对象时，打程序日志。提供构造 或 SetLogger() 配置。

15. DbHelper 新增 ExecuteDataSetAsync 异步查询函数，用于生产使用，后期会提供更多的异步函数。

16. ToTryVar<T>(); 该方法解决强转结果不理想问题，支持返回默认值。（容错效果良好）
int sd1 = "你是傻逼！".ToTryVar(10);
decimal sd2 = "你是傻逼！".ToTryVar(10.00m);
double sd3 = "你是傻逼！".ToTryVar(10.00);
DateTime sd4 = "你是傻逼！".ToTryVar(DateTime.Now);
int sd = "1000000000".ToTryVar(10);

17. 优化 Json 有关的函数，目前 Json 相关内部均采用了 System.Text.Json.JsonSerializer 为了提高可用性，
提供对应的 JsonSerializerOptions 条件对象参数，方便使用，有关自定义的Json格式后续会出一个相关的Json类型转换命名空间下面将包含部分常用类型的转换。

18. 优化 ISession 对象，增加Get方法的扩展，方便简单调用。

19. 优化 string.MD5Lower()或MD5Upper()，这些内部代码过度重叠已经删减优化。

20. 优化 ExecuteNonQuery() 方法返回插入ID时，因为Id 类型为int 导致其他特殊类型id值无法获取，现已改成 object 类型

21. 优化 DbHelper 增加 GetAndSetConnectionString 函数，提供连接字符串的有效管理。

22. 优化 DbHelper 新增 ExecuteNonQueryAsync 函数，提供异步返回受影响行数。

-------------------------------------最新功能-----------------------------------------
小版本：V2.1.0
1. WebApi 部分 优化调整，提供如下写法：

public JsonOut GetSql(
            HttpContext context
            , [ApiVal(Val.Query)] int Id
            , [ApiVal(Val.Form)] string Key
            , [ApiVal(Val.Service)] Tool.SqlCore.DbHelper dbHelper
            , [ApiVal(Val.File)] IFormFile file
            //,[ApiVal(Val.Session)] int key10
            //,[ApiVal(Val.Session)] ps key11
            , [ApiVal(Val.RouteKey)] int id1
            , [ApiVal(Val.RouteKey)] string controller
            , [ApiVal(Val.RouteKey)] string action

            , [ApiVal(Val.Service)] Microsoft.Extensions.Logging.ILoggerFactory factory

            , [ApiVal(Val.Cookie)] int SessionId
            , [ApiVal(Val.Header)] string Cookie1)
        {

            return ApiOut.Json(new { msg = $"暂无数据。 \n\taction:{controller}\action:{action}\action:{id1}", IsTask = false, count });
        }

备注：[ApiVal(Val.Query)] 为本功能更新主要功能。

2. Ashx 移除 参数：IsMode，解决方案：[ApiVal(Val.AllMode)] T Key 可以完全替代原有的，实现更加完善的功能。

3. 部分Bug优化。

年前大版本：V2.2.2
1. 用于注册请求流大小限制的上限。（示例为解决上传文件的大小）
public void ConfigureServices(IServiceCollection services)
{ 
    app.SetFormOptions(optins => 
    {
      optins.MultipartBodyLengthLimit = 60000000;用于处理上传文件大小被限制的问题。
    }) 
}

2. 优化 ApiOut.View 方法，默认视图存储位置（示例：\wwwroot\Views\类名\方法名.html）

3. 新增 ApiOut.File, 下载文件流的函数。

4. 新增 IFormFile.Save 保存上传的文件

5. 新增 OnResult 函数接口，同时实现了 （ApiAshx/MinApi）两种路由模式

6. 优化 SQL 所有可以传入 where 条件的接口均允许传入（ (NOLOCK)WHERE）该参数。

7. 优化 ApiVal 增加第二个条件，允许指定Key值，处理部分无法通过代码实现的写法。（示例如下：）
 public async Task<IApiOut> Upload(
    [ApiVal(Val.File)] IFormFile file_data, 
    [ApiVal(Val.Header, ".123")] string abc, 
    [ApiVal(Val.Header, "User-Agent")] string agent)
{
    await file_data.Save(AppContext.BaseDirectory + "Upload\\" + file_data.FileName);//顺带实现了上传保存文件的示例
    return await ApiOut.JsonAsyn(new { mag = "保存成功！", agent = agent });
}

8. 新增 ApiOut.Redirect, 重定向URL的函数。

3月12日新年首个大版本：V3.0.0
1. 新增路由自定义模式 MapApiRoute

2. 新增特性 路由 [AshxRoute("url/{id?}")] 支持接口/控制，注册

3. 新增DbProviderType.SqlServer1 用于包括新的 SqlServer（SKD:Microsoft.Data.SqlClient）

4. 优化Api命名空间引用复杂问题，简化引用。

5. 优化 NetFrame 命名空间下的，全部有关部分，重新定义新的协议，支持字节流传输和字符串传输，基础协议更小。

6. ClientFrame 模块 新增 心跳功能，需要手动开启 AddKeepAlive(5);（心跳模式开启后，将会自动检查是否断开连接）

6. ApiPacket 允许发起方，发送超过配置包大小的包，系统会自动分包处理

7. 重新优化多宝协议，现在更安全，更可靠。

8. ClientFrame 转发模式，优化，现在性能可靠。

9. NetFrame 模块下存在大量与内存使用有关的资源，目前已经全部通过GC管理起来，内存泄漏无需关心，后期会着重处理有关GC部分。

10. 多个已知Bug优化。

4月05日（月）更新：V3.1.0
1. 优化自定义路由在特定模式下不生效的BUG AshxRoute

2. 移除AsSession模块，原因是因为无效，并且无用。

3. 新增DiySession模块，支持自定义实现Session，具有高度可用，可用自实现。
示例：
services.AddDiySession(d => 
{
    d.GetDiySession<DiySession>(); //DiySession必须自己实现。
});
app.UseDiySession();

4. 多个已知Bug优化。

-------------------------------------2021/05/06--------------------------------------
月度更新：V3.3.0
1. 新增 UseIgnoreUrl 拦截器 用拦截部分请求

2. 新增 ApiOut.PathViewAsync("文件夹路径") 文件夹路径对象

3. 新增 Api 输出 Json 方法允许携带序列化对象

4. 新增 AshxRouteData.GetNewJsonOptions() 方法获取全新的 JsonSerializerOptions 配置对象

5. 新增 AddAshx 下面配置 JsonOptions 变量 允许注册Api全局 JsonSerializerOptions 配置对象

6. 优化 Api底层，优化性能

7. 优化 Api 异常反馈，解决异步异常下的错误点不明确。

8. 优化全局Web异常拦截器 app.UseExceptionHandler() 异步改为异步实现。

9. 新增 JsonConverterHelper 对象，目前下面只包含时间类型对象的，如果有需要的可以向作者提交建议

10. 优化 Json 字符串转 键值对或强类型变量的值是不确定的问题，现在已经改的基本满意了

11. 优化 Sql 请求统计 类型改为 ulong 存储更大。

12. 新增 VerificCodeHelper.GetRandomCodeV2

13. 移除 VerificationCodeHelper 类 改为 VerificCodeHelper 类

14. 验证码类改进优化较多，不详细说明，多线程下无问题。

-------------------------------------2021/06/05--------------------------------------
月度更新：V3.4.0
1. 新增 DataTable扩展 ToDictionaryIf 可自定义输出结果

2. 新增 JsonConverterHelper.GetDBNullConverter() 方法，DBNull 结果将自动转化为 null 输出

3. 优化改进 app.UseExceptionHandler(AllException); 实现方法改为 异步函数，考虑合理性。

4. 新增扩展 DbHelper.Select<T>(Action<T> prams) 示例：var list = dbHelper.Select<system>(s => s.id = 1);

5. AshxException 新增接口调用参数信息，方便定位异常发生时存在的值，日志输出时默认输出， IsParameters 默认开启。

6. AshxException.ToParameters() 手动获取参数结果函数。

7. 改进实体ToDictionary(), 当字段值为 Null 特殊值时，object 自动将结果转换成 null

8. Log 日志模块改进，在 Debug 模式下，日志将打印在项目目录。

9. Log 日志打印相关对象调整，日志打印模块改为异步IO写入，提高吞吐量。

10. Tool.Utils 命名空间下，新增ThreadQueue 该命名空间下面，可用于实现简易的，多线程，对应抢单模式，先后顺序，依次完成，可等待。

11. Log 类下新增 IgnoreMethodName 对象，用于屏蔽不想再日志中看见的异常方法。（目前默认会屏蔽掉 Task 对应的方法名）Tool.Utils.Log.Instance.IgnoreMethodName.Add()

12. Log 类新增 IsMoveNext 变量，默认为true，用于自动验证堆栈方法中的异常方法，返回原有名称,如需关闭请设置为 false。

------------2021/06/10------------
较小的优化改进 V3.4.3
1. ITableProvider 下面插入这些方法的值，存在null的时候异常。 Insert Update 已优化成，自动忽略为null的参数。

2. DbHelper 下面部分转DbParameter 的接口也存在 null 异常的情况，已优化成，自动忽略为null的参数。

3. 新增 TaskOueue 异步队列任务模型，需要实例化版本。

4. 优化 Log 写指定相对文件路径时，文件夹位置不在项目中。

5. 解决因日志模块引发的堆栈溢出，原因是没有合理的使用异步IO。

6. 改进写日志，会在文件被占用时，每隔100毫秒重试一次/10 如还是占用，将输入一个 DEBUG 日志提示。

------------2021/07/21------------
久违的大版本来了 V3.6.0
1. WebApi ApiAshx 控制器 允许在构造中，使用注入服务了。

2. CrossDomain 特性，重新实现，考虑到顺序，现在已经完全无问题了。

3. ApiAshx 模式 性能大幅度提升。

4. AppendHeader() 扩展函数，支持了中文类容，符号等。

5. TypeInvoke 类，相关BUG改进，新增部分函数。

6. ActionDispatcher 类，改进，公开更多变量。

7. 新增 ClassDispatcher 类，用于提高创建对象。 方便管理，后期还会优化，增加类可用变量。

8. 新增 GoOut 类，实现至 IGoOut 接口，用于规范 DataBase 协议模型。

9. DataBase 继承类不在需要实现父类构造，改为如下示例：
    public class Class1 : DataBase
    {
        [DataTcp(1)]// 在无参构造上标注他的主ID
        public Class1()
        {

        }
    }

10. DataTcp 类，移除了 DataTcpState ObjType 枚举类型，因为他目前看来毫无意义。

11. WebServer 类，新增了事件，可自定义实现自协议，或拒绝对方。0

12. WebContext 类，公开了原本未公开的变量。

13. DataTcp 类，新增 AddDataTcps() 函数，用于注册需要注册的新接口。（可替换原有接口）

14. AddDiySession 服务，新增GetKey 委托，用于提供自定义的Session 值，或拒绝 提供Session 服务。（自由发挥吧）
     services.AddDiySession(d =>
     {
         d.GetDiySession<Test.Class>();
         d.GetKey = async (s) =>
         {
             return await Task.FromResult<(string, bool)>((null, true));
         };
     });

15. 取消了，在Web Startup 中需要引用特别的命名空间。

16. ClientFrame 类 和 ServerFrame 类，重写异步函数，让其更加合理 返回类型 Task<TcpResponse>

17. 新增 JsonVar 扩展函数 返回 JsonVar 类型 用于直接取值，简化开发流程。
var str = "{ \"result\": {\"code\":0, \"hehe\": [0,5,10] } }";
var json = str.JsonVar();
var code = json["result"]["hehe"][2].Data;

18. DiySession 类 提供 HttpContext 方便给你带来更多方便实用操作。

19. HttpContext 下 扩展 GetIP 更名为 GetUserIp 同时支持代理模式，获取IP

20. HttpContext 下 新增扩展 GetSchemeHost 用于获取请求 地址等信息，支持代理

------------2021/08/27------------
很抱歉来晚了 V3.7.0
首先说明：因为这个月作者发生了很多事情，导致了更新放缓，在这里表示非常抱歉，让支撑我的朋友们久等了。

1. 新版本中的细节优化。

2. 新增，web 终结点模式，目前属于公测阶段，可以尝试一下哦，新模式嘛。
具体操作：
services.AddAshx(o =>
{
    o.EnableEndpointRouting = true; //注意因为是公测版本，默认不开启。
    o.IsAsync = true;
    o.JsonOptions = new System.Text.Json.JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
    };

    o.JsonOptions.Converters.Add(JsonConverterHelper.GetDateConverter());

    o.JsonOptions.Converters.Add(JsonConverterHelper.GetDBNullConverter());
});
-----------------------------------------
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAshxs(routes => 
    {
        routes.MapApiRoute(areaName: "WebTestApp.Api",
            template: "Api/{controller=GetMin}/{action=GetSql}/{id1?}");

        routes.MapApiRoute(areaName: "WebTestApp.ApiView",
            template: "{controller=Heheh}/{action=Index}/{id?}");
    });
});

3. 将以前的 ObjectExtension.Dynamic 相关模块全部，增加了弃用标识。

4. 新增 ObjectExtension 下 Services 和 Provider 独立的 DI 版本，用于使用者的特殊操作。

5. 新增 IocHelper 和 IocCore 类，用于管理相关 DI

6. 以上DI都是基于 Microsoft.Extensions.DependencyInjection 提供的模块的包装简化。

7. 一些相关，方法函数值的限制。

以上是大致更新说明，详情还请执行查看api文档。

------------2022/01/22------------
很抱歉来晚了 V3.8.0

1. 本版本，主要是更新至 6.0 将多个过时接口更换。

2. 并且在之后版本 只继续维护 6.0， 5.0 的最后一个版本将定格在 3.7 版本中。

------------2022/01/24------------
V3.8.1

1. 新增 平台 限制说明。

2. 新增 6.0 过时接口提示。

3. HttpHelpers 类 重写实现，接口基本不变，因 原有接口被 6.0 视为 过时接口导致，重写，目前性能优于之前版本。

4. 补全 3.8.0 版本的提示描述文件的丢失。


------------2022/01/25------------
V3.8.2

1. 紧急修复 一个 BUG Web 路由获取时 默认项目命名空间带有“.”的项目无法获取路由


------------2022/06/30------------
V3.8.5

1. 修复 TaskOueue 在运行时 出现的 偶发性 线程停止 问题导致 任务无法继续进行，现以修复 并针对其进行优化，增加了 在无任务时的 等待驻留（一段时间） 以保证 有新任务来时 更快的处理。
2. HttpHelpers 类相关 改进, 支持获取压缩数据，https 请求等，主要以 原使用者兼容为主。
3. JsonHelper 相关 JsonVar 类型 支持 多种取值方式，简化原负责流程

------------2022/07/01------------
V3.8.8

1. 本版本 主要更新 支持.net5,.net6 因使用发现 还有不少用户 停留在 .net5 一直没有更新 因担心 .net6 的变动影响，现有功能等，故而本次更新将 继续支持 .net5，方便开发者可选方案。
2. .net5,.net6 的支持可能会在 .net7 更新后 变更成 支持：.net6,.net7 具体看情况而定，不确定 不同版本之间的 函数差异 是否 有兼容 或 过时等因数。
3. 本次 虽然 支持了两个版本 但是一切功能都是两版本均支持的 接口 所以 使用时 所有功能 都是一样的效果 更新的内容 两个版本 都将支持。
4. ObjectExtension.Static 变量 支持 多线程安全访问 获取全局对象，为偷懒和不爱管理者设计。
5. JsonHelper 相关 JsonVar 类型 支持 多种取值方式，简化原负责流程
6. 这里说一下 由于作者今年以来比较忙 所以希望大家能谅解，同时也希望大家可以一起参与建设，贡献生态。
7. 下一步更新可能优先考虑 完善 SQL 部分 功能 以及支持异步 调用等，并对 Sockets 相关部分进行优化 考虑使用 资源池 ArrayPool 以此减少 对内存 频繁操作的性能消耗，以及其余可优化问题。

------------2022/07/16------------
V3.8.9 - 小版本更新

1. 修复 HttpHelpers.PostStringAsync 函数因审查不到位导致的重复调用一次问题。
2. 优化 sql 部分 有个可以传入虚构对象或实体对象的接口，普及增加了自动验证是否为可用字典，使其可用性大大提高，可以是任意字典即键值对/哈希表。
3. 优化 HttpHelpers 下所有请求在异常时 缺少 排错问题 有关的日志，新增异常日志。
4. 优化 HttpHelpers 请求时流对象复制方式，改为拷贝，性能略优。
5. 优化 HttpHelpers 解析参数性能和效率。 50%以上 原：900+ -> 420 以内。

------------2022/08/28------------
V3.9.9 - 大版本更新（预览版·主要由于改动过大需要一定使用覆盖范围）

1. Sql 相关日志完善，在执行sql异常后，也会增加操作日志，极大解决因执行错误，却不知道执行了啥，这类问题。
2. AppSettings 获取配置文件范围优化，会优先从环境路径匹配。（好处开发模式下，在vs修改配置文件将生效）
3. 主要是优化 NetFrame 下的通讯效率，本次优化，解决了，在多线程环境下，容易出现的无限超时问题，优化。
4. NetFrame 代码执行效率 优化，事件处理优化，数据包处理优化，池合理管理优化，分包处理优化等。
5. TcpHelper 相关实际改动，主要是围绕这提升执行效率，增加IsThreadPool 字段 控制 数据包 是否采用线程池处理，增强可控性。
6. TcpHelper 优化 数据包 解析效率，解析流程 算法简化等。
8. PagerParameters 参数拼装 优化简化步骤。
9. NetFrame 下多包 来回 发送 优化，但效率不是特别高，目前采用的是 单线程 操作。
10. Http post 请求时表单参数 实体 默认可构造，Val.Files 允许获取所有上传资源。
11. 优化：DbHelper.SubPath 允许自定义，日志存放子路径。
12. 优化：ApiPacket对象传值，默认进行传输转义。
13. 优化：新增EntityBuilder对象接管原多个模块实现，提升多个模块执行效率，（仅有关对象反射相关）提升50~70%。包含，WebApi，Sql，对象转换字典等等。
14. 其他细节优化等。

------------2022/09/04------------
V4.0.0 - 正式版
1. 除以上预览版全部内容更新外。
2. DataSet.IsEmpty() 优化，改变为只要包含一个不为空的DataTable，就为false。
3. DataTable/DataRow 转实体对象，优化提升效率，取消原反射模式。
4. DiySession新增SetAvailable接口，可以设置可用性，可用性将提供，自带可用性用户端标志，可简化验证流程。
5. DictionaryExtension 新增GetDictionary和SetDictionary函数详情请查看注释
6. DbDataReader 新增 GetReader 扩展用于 直接获取 对应集合字典，提高效率。
7. 其他优化。
8. 存在的风险预警，NetFrame下多包传输模式，存在GC频繁，导致崩溃问题，崩溃来自GC内核，目前无法修复，触发条件，频繁多包传输导致GC频繁。（下个版本优先，优化此类问题）

------------2022/11/05------------
V4.1.0 - 正式版（当前版本仅优化部分上个版本遗留问题，新功能开发还在进行中，最近作者工作繁忙，时间不咋充裕）
1. 优化 获取Web请求者IP，支持X-Real-IP 和 X-Forwarded-For 双验证优化使用 X-Real-IP 并进行了有效验证。
2. 优化 DiySession Cookie 信息支持自定义等，异步 Initialize 初始化函数等
3. 优化 Api接口报错后，异常日志，显示的json格式中文不能与AddAshx 注册时 配置一致问题。
4. Udp相关模块改动，发现原设计问题，在优化改动中。（很抱歉，因作者知识储备有限，需要时间处理）
5. 优化 IsIpPort 验证计算方式，效率大幅度提升。
6. 优化 NetFrame 核心对象 由 TcpClient 改为 Socket 其余不变，不过目前，在考虑优化实现一个TcpClient更高效的通讯方式，还在测试中，尽情期待。
7. 新增 ServerFrame 类 ADD->SetIpParser函数，委托，可简化通讯转发效率，提供可配置性和控制性。
8. 新增 JsonVar增加GetJson函数，获取当前节点的Json字符串。
9. 新增 NetFrame IGoOut 接口允许 使用异步 方式调用->Task<IGoOut>
10.新增 CopyEntity 函数，全对象扩展，支持对象参数拷贝。
实例：
A a = new(); //下有 参数 a1(int), b2(string)
A b = new(); //下有 参数 b1(long), b2(string)
a.CopyEntity(b); //可拷贝 b->b2
a.CopyEntity(b, "b1=a1", "b2"); //可拷贝 b->b1, b->b2

11. 其他优化。

------------2023/01/09------------
V4.1.5 - 正式版
1. 新增支持 .net7 版本
2. 修复 JosnVar 对象在 json 字符串 中存在 \u007F 时出错问题修复
3. Tool.Sockets.NetFrame.IpParser 回调事件，提供了访问者IP信息。
4. DiySession 新增 SetId 方法 可以指定 SessionId。
5. 优化了 GetUserIp GetUserIps 在验证IP有效性时，效率的提升。

------------2023/01/12------------
V4.1.6 - 正式版
1. HttpHelpers 模块优化，取消之前的 refer 和 cookies 参数 改为 Action<HttpRequestHeaders> onheaders 委托 可以有效配置更多的信息。
2. 无 年前最后一个版本。

------------2024/01/30------------
V4.1.7 - 正式版
1. 新增支持 .net8 版本
2. 性能优化

------------2024/05/17------------
V5.0.0 - 尝鲜版
1. 完善 Tool.Sockets 命名空间下的相关的类完善。
2. 完善之前 UDP 模块下的不可用，全部开放。
3. WebSocket 部分完善客户端模块
4. 移除 System.Drawing 有个的图片模块 -> 相关验证码等 迁移到 Tool.Net.Drawing Sdk下
5. NetFrame 命名空间下模块优化，通信模块完善，更佳的内存管理，解决原大文件传输问题导致的GC崩溃问题。
6. 本版本中实现了大量值类型传递，大幅度提高了，通信效率。
7. 改动事件触发模块，采用泛型重构，提高效率。
8. Ouic模块已经可用尝鲜了，已经进行过，多种测试，但目前Quic还在草案阶段，故只能是预览版
9. 想尝试预览版使用的：请添加[RequiresPreviewFeatures]特性
10. 当前版本上线后，将会在1周内发布正式版

------------2024/05/18------------
V5.0.1 - 尝鲜版（描述预览版标记）
1. 改动连接成功后接收业务移植到独立线程，避免被客户休眠导致新连接无法进入的特殊情况。
2. EnClient or EnServer Connect 事件触发在所有任务之前，不在并行触发。
3. IGetQueOnEnum 提供 IsSuccess 和 Wait() 用于处理事件任务是否被完成。
4. 当前版本上线后，将会在1周内发布正式版

------------2024/05/19------------
V5.0.2 - 尝鲜版（描述预览版标记）
1. 改动 ServerFrame or ClientFrame 允许初始化时设置 IsThreadPool 值，针对于需要保证流顺序的场景至关重要。
2. 修复本次大改版，出现的 ServerFrame or ClientFrame 下 Receive 事件重复触发的问题。
3. 完善 UserKey 类型在接收 IP端口 字符串时，未验证问题，优化后可以自动适配。
4. 当前版本上线后，将会在1周内发布正式版

------------2024/05/24------------
V5.0.3 - 尝鲜版（描述预览版标记）
1. 修复Udp模块不可用问题。
2. 新增 P2PHelpr 模块 支持 P2P 模型 目前提供可用的公共服务版，私有实现版（）
相关实例：
-------------------------------------------TCP-------------------------------------------

P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeTcp();
P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeTcp();

TcpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true);
var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

TcpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true);
var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);

-------------------------------------------UDP-------------------------------------------

P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeUdp();
P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeUdp();

UdpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true);
var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

UdpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true);
var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);

-------------------------------------------UDP-------------------------------------------

Task.WaitAll(task0, task1);

以上为本次最大更新，以及修复部分已知问题和增强部分函数可用性。

------------2024/05/24------------
V5.0.4 - 尝鲜版（描述预览版标记）
1. 修复P2pServerAsync.GetFreeTcp(); 验证错误问题。
2. 优化验证结构，提高验证效率。

------------2024/06/01------------
V5.0.5 - 尝鲜版（描述预览版标记）
1. Tool.Utils.ActionDelegate 下相关模块加强功能。
2. ActionDispatcher 提供异步调用模型，可实现对异步函数的更高效调用。
3. WebApi 相关模块优化，完善 MapAshxs 终结点模式，支持自定义路由规则等。
4. AppSettings 支持修改配置同时更新对应相关绑定文件信息。
5. 额外优化等。

------------2024/06/03------------
V5.0.6 - 尝鲜版（描述预览版标记）
1. 当前版本为增强版本。
2. 修复原终结点模式，路由匹配规则不完善问题。
3. 将两种模式的共同部分合并，以便管理维护。
4. 简化Api调用过程，加强吞吐性能。
5. 优化部分内部变量管理等，细节优化。

------------2024/06/05------------
V5.0.7 - 尝鲜版（描述预览版标记）
1. 当前版本新增了 Val.BodyJson or Val.Body or Val.BodyString 三种 body 数据使用模式。
实例：public IApiOut Api3([ApiVal(Val.BodyJson)] List<SystemTest> test);

------------2024/06/05------------
V5.0.8 - 尝鲜版（描述预览版标记）
4. AppSettings 支持的同步修改改成了（异步队列）实现。
主要原因是因为，没有必要占用同步IO。
其次优化了，赋值支持。
实例：AppSettings.GetSection("Test:a:b").Value = new string[] { "6", "7", "8" }.ToJson();
      AppSettings.GetSection("Test:name").Value = new { a = 5, b = 9, c = "666666" }.ToJson();
      AppSettings.GetSection("Test:int").Value = "我";
      AppSettings.GetSection("Test:list:2:name").Value = "我";
      AppSettings.GetSection("Test:list:1:0").Value = "我";
同时优化了，当文件中不存在时，不调用，在文件与修改结果一样时，取消修改等等优化。
如发现不合理，请及时指出。

------------2024/06/06------------
V5.0.9 - 尝鲜版（描述预览版标记）
1. AppSettings 优化，仅可修改的文件，更新相关内存信息，不重复赋值。

------------2024/06/15------------
V5.1.0 - 尝鲜版（描述预览版标记）
1. 优化 Udp 模块断线问题，可设置超时时长。
2. 优化P2P模块连接问题优化。
3. 其余细节优化。
追加更新 06/28
1. 优化P2P模式，减少失败时的耗时，优化Tcp，P2P模式下的初始耗时，整体耗时优化。
2. 虽然整体优化了P2P模式，但目前UDP环境下，重试后依旧不成功，还在尝试寻找更优解决方案。
追加更新 06/30
1. 修复Udp模式下不开启多线程模式，无法使用问题，新增可用手动关闭Udp连接(UdpEndPoint.Close)。

------------2024/08/22------------
V5.2.0-pre0.1 - 预览版
1. 新增 Host.CreateDefaultBuilder(args).UseDiyServiceProvider() 接口，可以将框架现有容器替换掉 Web 默认容器，实现在当前框架下的高可用。
2. 新增 TextUtility 下函数，IsPrivateNetwork() 和 IpAddressInRange() 详情查看注释
3. 优化 TCP/UDP 在开启了，数据有效性，协议后，双方需要完成数据验证才能，进行通讯，此为内置实现，表现为：当前版本的，TCP/UDP 有效性数据模式不能与旧版本通信了。
4. 增强 UDP 模式整体增强，在非安全模式下，UDP服务器，已经可以完全实现收文报不等待。这会大大降低丢包延迟率等，但会对内存增加使用。
5. 增强（公测）UDP 模式，数据安全模式，重新设计，按照现主流通信模式，重写目前，已基本实现丢包重发，漏包补发，大包也不限制等。
6. 修复 上一版本TCP在断线后重连模式，存在类似于重复循环调用情况。
7. 优化 设置缓冲区的枚举 NetBufferSize 最大值 
8. 其他 其余部分优化改动。
9. 结尾 虽然当前版本已经是公测版本了，但依旧要谨慎用于正式环境，毕竟UDP不丢包模式还有为完善的部分带优化。

------------2024/08/23------------
V5.2.0-pre0.2 - 预览版
1. 修复 TCP 验证模式，当有旧版本连接，协议异常时，导致闪退问题
2. 改进 IpParserEvent 委托无法使用异步语法问题。
3. 优化 对所有通信发送string值的函数，进行了环形内存使用优化，部分注释也优化了

------------2024/08/26------------
V5.2.0-pre0.3 - 预览版
1. 修复 TCP 验证模式，当有旧版本连接，协议异常时，导致闪退问题（其他）
2. 新增部分扩展函数
3. 少量优化，目前作者，在忙着优化衍生产品，P2P大模型，这边的更新放缓了
4. 在此向支持我的小伙伴们，说声抱歉。

------------2024/08/28------------
V5.2.0-pre0.4 - 预览版
1. 修复 TCP 服务器关闭后，客户端重连只能生效一次的问题。
2. 紧急修复的BUG，非常抱歉！

------------2024/08/29------------
V5.2.0-pre0.5 - 预览版
1. 修复 UDP 服务器在不采用有效连接模式下，接收消息只触发心跳消息问题。 （属于重大BUG）
2. 优化 UDP 连接事件，全部脱离核心接收模块，保证核心不会被大量新的连接占用时间片，导致大量丢包的问题。
3. 优化 整个Net通信模块下的，线程核心，增强健硕性，防止在意料之外的情况发生时而导致崩溃问题的发生。
4. 优化 对所有的接收事件核心，在发生异常到最上层时，主动对内存进行管理回收，防止因开发者错误的程序流程，导致出现内存泄露问题的发生。
5. 后期 在此版本后，作者将完善 UDP 有效通信模式下，为完善部分的继续优化和创新。

------------2024/08/31------------
V5.2.0-pre0.6 - 预览版
1. 修复 Tool.Sockets.UdpHelper.UdpClientAsync.UdpCore 调用导致，系统崩溃问题，这种低级错误，作者我也能犯，我真的是脑子进水了
V5.2.0-pre0.7 - 预览版
1. 修复 Tool.Utils.Log 报错问题，报错原因是我本身就在改动这块的代码，但忘记，然后发布时合并掉了，发布后才发现。
2. 优化 V5.2.0-pre0.5 版本中异常捕获部分，使其更加精确的获取错误信息。
3. 优化 Tool.Utils.Log 模块下线程性能提升，仅.net6及以上版本生效。

------------2024/09/06------------
V5.2.0-pre0.8 - 预览版
1. 优化 SDK中部分静态对象，在初始化加载时，就载入内存但实际，使用者并未使用，导致过度增加内存等。
2. 优化 Net事件中间件（初衷：当初设计时，想到可以降低对通信层业务的性能影响，但忽略了特殊行为，比如用户连接进来时，可以做身份校验等，退出时可以干嘛等，这些都可能涉及等待延迟等，但所有的事件都在队列里面，就会出现，连续等待导致，后续连接一直在排队问题，针对这个问题，作者做出了优化，将允许部分事件脱离队列执行）
3. 新增 Net事件 EnumEventQueue.OnIsQueue 这个事件，可以自定义，那些事件由任务线程自行处理，还是移交给队列线程处理。（作者已将部分，默认载入进了初始配置，这些都是默认交由任务线程自行处理的。）
4. 优化 UDP客户端P2P模式下在特殊情况下，断开没有结束心跳线程问题。
5. 修复 TCP/UDP 在启用了验证协议模式，又采用P2P导致连接后协议验证失败断开问题。
6. 优化 UDP模式新实现后，旧实现移除整理相关数据结构等。

------------2024/09/25------------
V5.2.0-pre0.9 - 预览版
1. 更名 NetResponse.OnNetFrame 变更成 NetResponse.State
2. 更名 ClientFrame.SendIpIdea 变更成 ClientFrame.SendRelay
3. 更名 ClientFrame.SendIpIdeaAsync 变更成 ClientFrame.SendRelayAsync
4. 修复 UdpServerAsync.StartAsync 无法监听端口报错时，导致程序结束问题
5. 新增 ServerFrame.IsAllowRelay 属性，控制客户端是否允许转发模式
6. 新增 [DataNet(0, IsRelay = false)] IsRelay属性，控制当前方法或当前类的转发是否支持
7. 其他优化，以及是否发布V5.2正式版的预期是否达标。

------------2024/11/08------------
5.3.0-pre0.1 - 预览版
1. 当前版本主要优化升级了SQL连接部分（完善异步部分）
2. 提供更高效的获取 DataSet 的异步函数等
3. 覆盖全部Sql异步模式
4. 新增直接Sql返回字典数据 SelectDictionaryAsync/SelectDictionary
5. 其他重要功能新增
    public class Abc
    {
        public int? a { get; init; }
        public string? b { get; set; }
        public DateTime? c { get; init; } = DateTime.Now;
        public bool? d { get; set; }
        public byte? e { get; init; }
        public double? f;
        public decimal? g { get; init; }
        protected object s => throw new AggregateException();
        protected static object a1 { get; set; }
        private static object a2 { get; set; }
        public static object a3 { get; set; }
        protected static object a4;
        private static object a5;
        public static object a6;
    }
    Abc abc = new Abc();
    var dic = abc.GetDictionary();
    abc.SetDictionary(dic);
    
    abc.SetFieldValue("f", 0.1);
    abc.SetPropertyValue("a", 123456);
    abc.SetPropertyValue("a1", 0.1);
    abc.SetPropertyValue("a2", 0.2);
    abc.SetPropertyValue("a3", 0.3);
    abc.SetFieldValue("a4", 0.4);
    abc.SetFieldValue("a5", 0.5);
    abc.SetFieldValue("a6", 0.6);
    abc.GetPropertyValue("s", out var s);

  /** 目前已是最终版本，将在预览版后，同步更新正式版本，缓冲期7个工作日内 */

  ------------2024/11/11------------
5.3.1 - 正式版 - 来临了，刚好赶上光棍节哈哈哈
1. 函数改名：
SetFieldValue 改 SetFieldKey，
SetFieldValue 改 SetFieldKey，
GetPropertyValue 改 GetPropertyKey，
SetPropertyValue 改 SetPropertyKey
GetValue 和 SetValue 新版来临，采用 上面两种模式的自主匹配方案。
2. Json 字符串转对象的函数优化，使用起来更加轻松。
3. 之前.net7 之后正则性能提升了，但Sdk对应的版本并没有提升，目前已全部转为新模式，提供的正则相关函数性能提升。

其他已知问题修复。

  ------------2024/11/15------------
5.3.2 - 正式版
1. 没想到这么快就有更新了吧？.net9出来了 所以做了适配支持等。
2. sql 部分新增 SelectArrayAsync/SelectArray 返回的是二维数组表。
3. 针对二维数组表，提供了 new DataTable().CloneArray(array); 函数用来进行二维数组表与 DataTable的互转。
4. 既然是互转 DataTable.ToArray() 也提供了 转二维数组表的对应函数。
5. 其他优化，以及适配.net9的相关支持。

  ------------2024/11/15------------
5.3.3 - 正式版
1. 出乎我自己的意料，当日二更
2. 本版本主要是因为 JsonVar 对象存在数字类型丢精度问题。（作者本人用到，所以急着更新）
3. 出于性能考虑，取消掉了 JsonVar 解析json字符串时会验证string类型的数据是不是时间类型
4. 优化新增 JsonVar.TryGet() 函数可以无触发异常，查找JsonVar中是否包含想要的数据，简直是爽到爆炸。
    if (varjson1.TryGet(out JsonVar varobj1, 5, "FromNo"))
    {
        Console.WriteLine(varobj0.ToString());
    }
5. JsonVar 类型内部引用优化，性能更强。

  ------------2024/11/21------------
5.3.4 - 正式版
1. 本次更新主要是修复 private readonly string text; readonly 关键字时 GetValue 和 SetValue 等相关函数 会报错，因为它是只读的。
2. 新增 TaskQueue 静态类，该类可以让方法已队列形式排队完成，公共模板，只有一个公共调度队列线程
相关示例：
static async Task<int> GetIntAsync()
{
    return await Task.FromResult(123_456);
}
_ = Tool.Utils.ThreadQueue.TaskQueue.StaticEnqueue(GetIntAsync).ContinueWith((a) => { a.Dispose(); Console.WriteLine($"{Tool.Utils.ThreadQueue.TaskQueue.Count}\t{Tool.Utils.ThreadQueue.TaskQueue.CompleteCount}\t{Tool.Utils.ThreadQueue.TaskQueue.TotalCount}"); });
备注：可以直接放方法函数，支持各种函数

  ------------2024/12/04------------
  5.4.0 - 正式版 - 重大改变
1. Sockets 模块下的所有事件重写，目前最新版已支持给每个构造对象设置事件控制权限。（客户端部分新增重连事件）
2. ServerFrame/ClientFrame 发起数据时优化，在已经断线的情况下，不会继续等待超时。
3. Sql 部分优化Log部分为记录到连接耗时部分。
4. Sql 新增支持 DbHelper.NewDbBatch() 支持批处理模式
5. Web 部分新增了EventStream 输出模式，为web通信提供了更多可能性。
7. 对象下GetValue 和 SetValue 在有意隐藏上级变量时，会出现报错，无法使用问题，目前已修复。
6. 部分功能及实现优化，已知问题修复等。
8. 隐藏福利，在线文档已就绪初版虽然不完善，但会持续完善维护，http://tool.nixue.top/
  5.4.1 - 正式版 - 修复漏洞
1. 主要是修复 EnumEventQueue.OnInterceptor(EnServer.Receive, true); 设置后行为是颠倒的

  ------------2024/12/06------------
  5.4.2 - 正式版 - 修复漏洞
1. dbHelper.ExecuteDataSetAsync 异步获取 DataSet 对象 数据时，因被系统回收资源导致 返回时发生报错。
2. 优化异步打开Sql连接后，采用异步关闭模式。
  5.4.3 - 正式版 - 新增功能
1. 示例：(简化json 读取方案，哈哈哈)
string jsonstr = "{\"Inbound\":{}}";
JsonVar jsonVar = jsonstr.JsonVar();

foreach (var item in jsonVar)
{
    Console.WriteLine(item.Key + ":" + item.Current.ToString());
    foreach (var item0 in item.Current)
    {
        Console.WriteLine(item0.Key + ":" + item0.Current.ToString());
        foreach (var item1 in item0.Current)
        {
            Console.WriteLine(item1.Key + ":" + item1.Current.ToString());
            foreach (var item2 in item1.Current)
            {
                Console.WriteLine(item2.Key + ":" + item2.Current.ToString());
            }
        }
    }
}

-------------------------------------移除SDK-----------------------------------------
本次移除全部 Web SDK 模块，不会影响框架性能，反之可能因此提高性能。
1. Microsoft.AspNetCore.Diagnostics
2. Microsoft.AspNetCore.Http
3. Microsoft.AspNetCore.Routing
4. Microsoft.Extensions.Configuration.Json
5. Microsoft.Extensions.DependencyInjection.Abstractions

-------------------------------------后续方向-----------------------------------------

1. 完善 Sockets 部分,继续增强该区域。

-------------------------------------开发建议-----------------------------------------

工具包-逆血著作 (未经本人许可，不可擅自进行任何交易)

有任何优化建议，请联系作者，作者很愿意讨论和学习。

可以发送邮件至 1477863629@qq.com

也可加QQ群：857401501

也可以 https://github.com/ni-xue/Tool.Net 把问题提到这里。