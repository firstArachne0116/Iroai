using System;
using System.Drawing;

namespace SmartApp.ColorPick
{
    internal static class HsvConverter
    {
        /// <summary>
        /// RGBカラーからHSVカラースベースへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>HSVカラースベース</returns>
        internal static Hsv ToColor(Rgb rgb)
        {
            double min, max, tmp, S;
            // double H, V;
            double R = rgb.R * 1.0f, G = rgb.G * 1.0f, B = rgb.B * 1.0f;

            tmp = Math.Min(R, G);
            min = Math.Min(tmp, B);
            tmp = Math.Max(R, G);
            max = Math.Max(tmp, B);

            Hsv hsv = new Hsv();
            Color color = Color.FromArgb(rgb.R, rgb.G, rgb.B);

            hsv.H = (int)color.GetHue();

            if (max == 0)
            {
                S = 0;
            }
            else
            {
                S = 1d - (1d * min / max);
            }

            hsv.S = (int)(S*100);
            hsv.V = (int)(max/255*100);



            //Hsv hsv = new Hsv();

            //// H
            //H = 0;
            //if (max == min)
            //{
            //    H = 0;
            //}
            //else if (max == R && G > B)
            //{
            //    H = 60 * (G - B) * 1.0f / (max - min) + 0;
            //}
            //else if (max == R && G < B)
            //{
            //    H = 60 * (G - B) * 1.0f / (max - min) + 360;
            //}
            //else if (max == G)
            //{
            //    H = 60 * (B - R) * 1.0f / (max - min) + 120;
            //}
            //else if (max == B)
            //{
            //    H = 60 * (R - G) * 1.0f / (max - min) + 240;
            //}
            //hsv.H = (int)H;

            //// S
            //if (max == 0)
            //{
            //    S = 0;
            //}
            //else
            //{
            //    S = (max - min) * 1.0f;
            //}
            //hsv.S = (int)(S * 255);

            //// V
            //V = max;
            //hsv.V = (int)(V * 255);

            return hsv;
        }
    }
}