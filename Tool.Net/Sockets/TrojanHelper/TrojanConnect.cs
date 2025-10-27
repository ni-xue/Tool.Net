using System.Net;

namespace Tool.Sockets.TrojanHelper
{

    /// <summary>
    /// 用于创建 Trojan 连接协议的信息
    /// </summary>
    public class TrojanConnect
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="host">连接源</param>
        /// <param name="port">端口</param>
        /// <param name="password">密码</param>
        public TrojanConnect(string host, int port, string password)
        {
            Host = host;
            Port = port;
            Password = password;
        }

        /// <summary>
        /// 高级构造
        /// </summary>
        /// <param name="address">设置源ip</param>
        /// <param name="host">连接源</param>
        /// <param name="port">端口</param>
        /// <param name="password">密码</param>
        public TrojanConnect(IPAddress address, string host, int port, string password) : this(host, port, password)
        {
            Address = address;
        }

        /// <summary>
        /// 源地址
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// 源地址IP
        /// </summary>
        public IPAddress Address { get; internal set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password {  get; }

        /// <summary>
        /// 获取字符串内容
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}
