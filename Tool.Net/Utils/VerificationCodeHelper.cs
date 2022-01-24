using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace Tool.Utils
{
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class VerificCodeHelper
    {
        /// <summary>
        /// 验证码 宽
        /// </summary>
        public const int Width = 62;

        /// <summary>
        /// 验证码 高
        /// </summary>
        public const int Height = 21;

        private static readonly Random _random = new();

        //private static readonly string[] BrushName = new string[]
        //{    "OliveDrab",
        //     "ForestGreen",
        //     "DarkCyan",
        //     "LightSlateGray",
        //     "RoyalBlue",
        //     "SlateBlue",
        //     "DarkViolet",
        //     "MediumVioletRed",
        //     "IndianRed",
        //     "Firebrick",
        //     "Chocolate",
        //     "Peru",
        //     "Goldenrod"
        //};

        /// <summary>
        /// 当前生效的噪点集合 Brushes 对象下有的才能用,加入对象名称
        /// </summary>
        public static HashSet<string> BrushNames { get; } = new()
        {
            "OliveDrab",
            "ForestGreen",
            "DarkCyan",
            "LightSlateGray",
            "RoyalBlue",
            "SlateBlue",
            "DarkViolet",
            "MediumVioletRed",
            "IndianRed",
            "Firebrick",
            "Chocolate",
            "Peru",
            "Goldenrod"
        };

        private static readonly string[] FontItems = new string[]
        {   "Arial",
            "Helvetica",
            "Geneva",
            "sans-serif",
            "Verdana"
         };

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="codeStr">要生成的验证码字符串</param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public static byte[] GetVCode(string codeStr)
        {
            Color _brushName;

            using Bitmap img = new(Width, Height);
            // _code = GetRandomCode();
            // System.Web.HttpContext.Current.Session["vcode"] = _code;
            using Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);//绘画背景颜色

            using (Font font = GetFont())
            {
                using Brush brush = GetBrush(out _brushName);
                Paint_Text(g, font, brush, codeStr);// 绘画文字
            }
            // g.DrawString(strCode, new Font("微软雅黑", 15), Brushes.Blue, new PointF(5, 2));// 绘画文字
            Paint_TextStain(img, _brushName);// 绘画噪音点

            g.DrawRectangle(Pens.DarkGray, 0, 0, Width - 1, Height - 1);//绘画边框
            using System.IO.MemoryStream ms = new();
            //将图片 保存到内存流中
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            //将内存流 里的 数据  转成 byte 数组 返回
            return ms.ToArray();
        }

        /// <summary>
        /// 绘画文字
        /// </summary>
        /// <param name="g"></param>
        /// <param name="brush"></param>
        /// <param name="font"></param>
        /// <param name="code"></param>
        [SupportedOSPlatform("windows")]
        private static void Paint_Text(Graphics g, Font font, Brush brush, string code)
        {
            g.DrawString(code, font, brush, 3, 1);
        }

        /// <summary>
        /// 绘画文字噪音点
        /// </summary>
        /// <param name="b"></param>
        /// <param name="brushName"></param>
        [SupportedOSPlatform("windows")]
        private static void Paint_TextStain(Bitmap b, Color brushName)
        {
            for (int n = 0; n < 30; n++)
            {
                int x = _random.Next(Width);
                int y = _random.Next(Height);
                b.SetPixel(x, y, brushName);
            }
        }

        /// <summary>
        /// 随机取一个字体
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private static Font GetFont()
        {
            int fontIndex = _random.Next(0, FontItems.Length);
            FontStyle fontStyle = GetFontStyle(_random.Next(0, 2));
            return new Font(FontItems[fontIndex], 12, fontStyle);
        }

        /// <summary>
        /// 随机取一个笔刷
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        private static Brush GetBrush(out Color brushName)
        {
            Brush brush;
            string[] brushs = BrushNames.ToArray();
            if (brushs.Length == 0)
            {
                brush = Brushes.OliveDrab;
                brushName = Color.OliveDrab;
            }
            else
            {
                int brushIndex = _random.Next(0, brushs.Length);
                brushName = Color.FromName(brushs[brushIndex]);

                if (brushName.IsKnownColor)
                {
                    brush = new SolidBrush(brushName);
                }
                else
                {
                    brush = Brushes.OliveDrab;
                    brushName = Color.OliveDrab;
                }
            }

            return brush;//.Clone() as Brush;
        }

        ///// <summary>
        ///// 绘画背景颜色
        ///// </summary>
        ///// <param name="g"></param>
        //private static void Paint_Background(Graphics g)
        //{
        //    g.Clear(BackColor);
        //}

        /// <summary>
        /// 取一个字体的样式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static FontStyle GetFontStyle(int index)
        {
            return index switch
            {
                0 => FontStyle.Bold,
                1 => FontStyle.Italic,
                _ => FontStyle.Regular,
            };
        }

        /// <summary>
        /// 取得一个 4 位的随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRandomCode(int length = 4)
        {
            if (length > 10)
            {
                throw new Exception("length 大于10");
            }
            if (length < 4)
            {
                throw new Exception("length 小于4");
            }
            return StringExtension.GetGuid().Substring(0, length);
        }

        /// <summary>
        /// 取得一个 个位数的（+*）的随机算数
        /// </summary>
        /// <param name="val">算数结果</param>
        /// <returns>算数例子</returns>
        public static string GetRandomCodeV2(out int val)
        {
            int val1 = _random.Next(1, 10);
            int symbol = _random.Next(0, 2);
            int val2 = _random.Next(1, 10);
            string str = string.Empty;
            switch (symbol)
            {
                case 0:
                    str = $"{val1}+{val2}=?";
                    val = val1 + val2;
                    break;
                case 1:
                    str = $"{val1}*{val2}=?";
                    val = val1 * val2;
                    break;
                //case 2:
                //    str = $"{val1}-{2}=?";
                //    val = val1 + val2;
                //    break;
                //case 3:
                //    str = $"{val1}/{2}=?";
                //    val = val1 / val2;
                //    break;
                default:
                    val = -1;
                    break;
            }
            return str;
        }
    }
}
