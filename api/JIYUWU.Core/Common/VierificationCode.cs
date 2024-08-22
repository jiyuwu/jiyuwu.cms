﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    public static class VierificationCode
    {
        private static readonly string[] _chars = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "P", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static string RandomText()
        {
            string code = "";//产生的随机数
            int temp = -1;
            Random rand = new Random();
            for (int i = 1; i < 5; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(61);
                if (temp != -1 && temp == t)
                {
                    return RandomText();
                }
                temp = t;
                code += _chars[t];
            }
            return code;
        }
        public static string CreateBase64Imgage(string code)
        {
            return VierificationCodeServices.CreateBase64Image(code);
            //Random random = new Random();
            ////验证码颜色集合
            //Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            ////验证码字体集合
            //string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

            //using var img = new Bitmap((int)code.Length * 18, 32);
            //using var g = Graphics.FromImage(img);
            //g.Clear(Color.White);//背景设为白色

            ////在随机位置画背景点
            //for (int i = 0; i < 100; i++)
            //{
            //    int x = random.Next(img.Width);
            //    int y = random.Next(img.Height);
            //    g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            //}
            ////验证码绘制在g中
            //for (int i = 0; i < code.Length; i++)
            //{
            //    int cindex = random.Next(7);//随机颜色索引值
            //    int findex = random.Next(5);//随机字体索引值
            //    Font f = new Font(fonts[findex], 15, FontStyle.Bold);//字体
            //    Brush b = new SolidBrush(c[cindex]);//颜色
            //    int ii = 4;
            //    if ((i + 1) % 2 == 0)//控制验证码不在同一高度
            //    {
            //        ii = 2;
            //    }
            //    g.DrawString(code.Substring(i, 1), f, b, 3 + (i * 12), ii);//绘制一个验证字符
            //}
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    img.Save(stream, ImageFormat.Jpeg);
            //    byte[] b = stream.ToArray();
            //    return Convert.ToBase64String(stream.ToArray());
            //}
        }
    }
    public static class VierificationCodeServices
    {        //验证码字体集合
        private static readonly string[] fonts = null;

        static VierificationCodeServices()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fonts = new string[] { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            }
            else
            {
                fonts = new string[] { "Arial", "Arial", "宋体", "宋体" };
            }
        }

        private static readonly SKColor[] colors = { SKColors.Black, SKColors.Green, SKColors.Brown };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string CreateBase64Image(string code)
        {
            var random = new Random();
            var info = new SKImageInfo((int)code.Length * 18, 32);
            using var bitmap = new SKBitmap(info);
            using var canvas = new SKCanvas(bitmap);

            canvas.Clear(SKColors.White);

            using var pen = new SKPaint();
            pen.FakeBoldText = true;
            pen.Style = SKPaintStyle.Fill;
            pen.TextSize = 20;// 0.6f * info.Width * pen.TextSize / pen.MeasureText(code);

            //绘制随机字符
            for (int i = 0; i < code.Length; i++)
            {
                pen.Color = random.GetRandom(colors);//随机颜色索引值

                pen.Typeface = SKTypeface.FromFamilyName(random.GetRandom(fonts), 700, 20, SKFontStyleSlant.Italic);//配置字体
                var point = new SKPoint()
                {
                    X = i * 16,
                    Y = 22// info.Height - ((i + 1) % 2 == 0 ? 2 : 4),

                };
                canvas.DrawText(code.Substring(i, 1), point, pen);//绘制一个验证字符

            }

            // 绘制噪点
            var points = Enumerable.Range(0, 100).Select(
                _ => new SKPoint(random.Next(bitmap.Width), random.Next(bitmap.Height))
            ).ToArray();
            canvas.DrawPoints(
                SKPointMode.Points,
                points,
                pen);

            //绘制贝塞尔线条
            for (int i = 0; i < 2; i++)
            {
                var p1 = new SKPoint(0, 0);
                var p2 = new SKPoint(0, 0);
                var p3 = new SKPoint(0, 0);
                var p4 = new SKPoint(0, 0);

                var touchPoints = new SKPoint[] { p1, p2, p3, p4 };

                using var bPen = new SKPaint();
                bPen.Color = random.GetRandom(colors);
                bPen.Style = SKPaintStyle.Stroke;

                using var path = new SKPath();
                path.MoveTo(touchPoints[0]);
                path.CubicTo(touchPoints[1], touchPoints[2], touchPoints[3]);
                canvas.DrawPath(path, bPen);
            }
            return bitmap.ToBase64String(SKEncodedImageFormat.Png);
        }

        public static T GetRandom<T>(this Random random, T[] tArray)
        {
            if (random == null) random = new Random();
            return tArray[random.Next(tArray.Length)];
        }

        /// <summary>
        /// SKBitmap转Base64String
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToBase64String(this SKBitmap bitmap, SKEncodedImageFormat format)
        {
            using var memStream = new MemoryStream();
            using var wstream = new SKManagedWStream(memStream);
            bitmap.Encode(wstream, format, 32);
            memStream.TryGetBuffer(out ArraySegment<byte> buffer);
            return $"{Convert.ToBase64String(buffer.Array, 0, (int)memStream.Length)}";
        }
    }
}