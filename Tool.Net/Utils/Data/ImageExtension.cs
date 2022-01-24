using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对<see cref="Image"/>进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [SupportedOSPlatform("windows")]
    public static class ImageExtension
    {
        /// <summary>
        /// 将Image转化为byte数组,使用缓存文件中转
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="imageFormat">格式（默认为（jpeg格式））</param>
        public static byte[] ToImageBytes(this Image image, ImageFormat imageFormat = null)
        {
            if (imageFormat == null) imageFormat = ImageFormat.Jpeg;
            MemoryStream stream = new();
            image.Save(stream, imageFormat);// 保存图像到文件
            using (stream)
            {
                return stream.ToArray();
            }
        }
    }
}
