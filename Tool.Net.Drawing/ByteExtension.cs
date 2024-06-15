using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace Tool
{
    /// <summary>
    /// 对Byte进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static partial class ByteExtension
    {
        /// <summary>
        /// 将一个序列化后的byte[]数组还原为图片类型     
        /// </summary>
        /// <param name="Bytes">byte[]数组</param>
        /// <returns>返回一个原图片对象</returns>
        [SupportedOSPlatform("windows")]
        public static Image ToImage(this byte[] Bytes)
        {
            if (Bytes == null)
            {
                throw new System.SystemException("对象未有初始值！");
            }
            using Stream str = new MemoryStream(Bytes, 0, Bytes.Length);
            Image img = Image.FromStream(str);
            return img ?? throw new System.SystemException("不是有效的图像格式！");
        }
    }
}
