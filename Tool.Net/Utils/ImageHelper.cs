﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Tool.Utils
{
    /// <summary>
    /// 图片压缩帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 获取或设置包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串。例如：image/jpeg
        /// </summary>
        /// <param name="mime_type"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(string mime_type)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i <= imageEncoders.Length; i++)
            {
                if (imageEncoders[i].MimeType == mime_type)
                {
                    return imageEncoders[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="file_name"></param>
        /// <param name="level"></param>
        public static void SaveJpg(Image image, string file_name, int level)
        {
            try
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)level);
                ImageCodecInfo encoderInfo = ImageHelper.GetEncoderInfo("image/jpeg");


                File.Delete(file_name);
                image.Save(file_name, encoderInfo, encoderParameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static MemoryStream SaveJpgToStream(Image image, int level)
        {
            try
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)level);
                ImageCodecInfo encoderInfo = ImageHelper.GetEncoderInfo("image/jpeg");
                MemoryStream strem = new MemoryStream();
                image.Save(strem, encoderInfo, encoderParameters);
                return strem;


            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="file_name"></param>
        /// <param name="max_size"></param>
        /// <returns></returns>
        public static int SaveJpgAtFileSize(Image image, string file_name, long max_size)
        {
            int result;
            try
            {
                for (int i = 100; i > 5; i -= 5)
                {
                    ImageHelper.SaveJpg(image, file_name, i);
                    if (ImageHelper.GetFileSize(file_name) <= max_size)
                    {
                        result = i;
                        return result;
                    }
                }
                result = 5;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static long GetFileSize(string file_name)
        {
            long length;
            try
            {
                length = new FileInfo(file_name).Length;
            }
            catch (Exception)
            {
                throw;
            }
            return length;
        }

        /// <summary>
        /// 收缩变形
        /// </summary>
        /// <param name="iSource">图片</param>
        /// <param name="maxWidth">压缩宽</param>
        /// <param name="maxHeight">压缩高</param>
        /// <returns></returns>
        public static Bitmap ShrinkageImg(Image iSource, int maxWidth = 800, int maxHeight = 0)
        {
            int sW = iSource.Width, sH = iSource.Height;
            if (maxWidth != 0 && maxWidth < sW)
            {
                sW = maxWidth;
                sH = iSource.Height * maxWidth / iSource.Width;
            }
            else if (maxHeight != 0 && maxHeight < sH)
            {
                sH = maxHeight;
                sW = iSource.Width * maxHeight / iSource.Height;
            }
            Bitmap ob = new Bitmap(sW, sH);
            Graphics g = Graphics.FromImage(ob);
            try
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(iSource, new Rectangle(0, 0, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
                return ob;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                g.Dispose();
                iSource.Dispose();
            }

        }

        /// <summary>
        /// 从路径中加载图片文件
        /// </summary>
        /// <param name="file_name">图片文件路径</param>
        /// <returns></returns>
        public static Bitmap LoadBitmap(string file_name)
        {
            Bitmap result;
            try
            {
                Bitmap bitmap2;
                using (Bitmap bitmap = new Bitmap(file_name))
                {
                    bitmap2 = new Bitmap(bitmap.Width, bitmap.Height);
                    using Graphics graphics = Graphics.FromImage(bitmap2);
                    Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    graphics.DrawImage(bitmap, rectangle, rectangle, GraphicsUnit.Pixel);
                }
                result = bitmap2;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 图片转<see cref="byte"/>[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            byte[] result;
            try
            {
                ImageFormat rawFormat = image.RawFormat;
                using MemoryStream memoryStream = new MemoryStream();
                if (rawFormat.Equals(ImageFormat.Jpeg))
                {
                    image.Save(memoryStream, ImageFormat.Jpeg);
                }
                else if (rawFormat.Equals(ImageFormat.Png))
                {
                    image.Save(memoryStream, ImageFormat.Png);
                }
                else if (rawFormat.Equals(ImageFormat.Bmp))
                {
                    image.Save(memoryStream, ImageFormat.Bmp);
                }
                else if (rawFormat.Equals(ImageFormat.Gif))
                {
                    image.Save(memoryStream, ImageFormat.Gif);
                }
                else if (rawFormat.Equals(ImageFormat.Icon))
                {
                    image.Save(memoryStream, ImageFormat.Icon);
                }
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Seek(0L, SeekOrigin.Begin);
                memoryStream.Read(array, 0, array.Length);
                result = array;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 从路径中加载图片文件
        /// </summary>
        /// <param name="FilePath">图片文件路径</param>
        /// <returns></returns>
        public static byte[] ImageToBytes(string FilePath)
        {
            byte[] result;
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                using (Image image = Image.FromFile(FilePath))
                {
                    using Bitmap bitmap = new Bitmap(image);
                    bitmap.Save(memoryStream, image.RawFormat);
                }
                result = memoryStream.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 将<see cref="byte"/>[]转图片，并设置大小
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="maxWidth">压缩宽</param>
        /// <param name="maxHeight">压缩高</param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer, int maxWidth = 800, int maxHeight = 0)
        {
            Image result;
            try
            {
                MemoryStream stream = new MemoryStream(buffer);
                Image image = Image.FromStream(stream);
                result = ShrinkageImg(image, maxWidth, maxHeight);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 将<see cref="byte"/>[]转<see cref="Bitmap"/>对象
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            finally
            {
                stream.Close();
            }
        }

    }
}
