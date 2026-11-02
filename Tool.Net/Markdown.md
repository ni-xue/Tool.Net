-------------------------------------功能简介-----------------------------------------

1.详细AIP文档：http://tool.nixue.top/index.html

2.框架包含功能（Web，Sql，Sockets，TypeExtension，Utils）

3. 当前版本（5.7.0+）为正式包，用于进行实际线上项目开发。 

4. 架构基于 .Net10.0（兼容 .Net9、.Net8、.Net7、.Net6、.Net5） 采用最新技术，具有最新的技术支持。

-------------------------------------功能详解-----------------------------------------

1. Web包含，Api创建，路由构建，可替代MVC写Api，和对Http多个扩展函数方便开发。

2. Sql包含，包含常用三种数据库的默认实现，其余数据库可以根据开源地址的实现，自行实现接口，支持所有数据库操作，提供获取原始数据库操作对象，由自行实现。

3. Sockets包含，Tcp，Udp，WebTcp(WebSocket)，NetFrame（内置实现的一种和gRPC/WCF差不多的消息传输协议，后期的维护重点）（NetFrame 重点，重构，对数据包体的高度使用，提高传输性能）

3.1 内置实现了 Trojan 协议，可以让 TCP/UDP HTTP 代理该通道

3.2 内置实现，高可用 P2P 模式，开箱即用版，可自行部署信标服务器，内置免费版的

4. TypeExtension包含，bool,byte,char,datetime,decimal,double,fioat,int,long,object,short,string,uint,ulong,ushort，相关扩展函数。

5. Utils包含，ActionDelegate，Data，Encryption，FtpHelper，Other，TaskHelper，Log，等等一系列帮助类型函数。

5.1 大致包含： 委托，反射，Log，验证码生成，正则验证，DataSet/DataTable/DataRow（验证，To实体？ToJson?），AppSettings（获取Core下面的配置文件：appsettings.json），等等

6. 目前功能基本满足小规模开发。