using System;

namespace SmartApp.ColorPick
{
    internal class CmykConverter
    {
        /// <summary>
        /// RGBカラーからCMYKカラースベースへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>CMYKカラースベース</returns>
        internal static Cmyk ToColor(Rgb rgb)
        {
            Cmyk cmyk = new Cmyk();
            cmyk.C = 1.0d - rgb.R / 255d;
            cmyk.M = 1.0d - rgb.G / 255d;
            cmyk.Y = 1.0d - rgb.B / 255d;

            cmyk.K = Math.Min(cmyk.C, Math.Min(cmyk.M, cmyk.Y));
            if (cmyk.K == 1.0)
            {
                cmyk.C = cmyk.M = cmyk.Y = 0;
            }
            else
            {
                cmyk.C = (cmyk.C - cmyk.K) / (1.0d - cmyk.K) / 255 * 100;
                cmyk.M = (cmyk.M - cmyk.K) / (1.0d - cmyk.K) / 255 * 100;
                cmyk.Y = (cmyk.Y - cmyk.K) / (1.0d - cmyk.K) / 255 * 100;
                cmyk.K = cmyk.K / 255 * 100;
            }


            //double C, M, Y, K;


            //C = 1.0d - (double)(rgb.R * 1.0f / 255f);
            //M = 1.0d - (double)(rgb.G * 1.0f / 255f);
            //Y = 1.0d - (double)(rgb.B * 1.0f / 255f);
            //K = (double)Math.Min(Math.Min(M, Y), C);

            //cmyk.C = (C - K) / 255 * 100;
            //cmyk.M = (M - K) / 255 * 100;
            //cmyk.Y = (Y - K) / 255 * 100;
            //cmyk.K = K / 255 * 100;

            return cmyk;
        }
    }
}