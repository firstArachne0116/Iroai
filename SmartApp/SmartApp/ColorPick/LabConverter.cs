using System;

namespace SmartApp.ColorPick
{
    internal static class LabConverter
    {
        /// <summary>
        /// RGBカラーからLABカラースベースへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>LABカラースベース</returns>
        internal static Lab ToColor(Rgb rgb)
        {
            double B = Gamma(rgb.B * 1.0f / 255);
            double G = Gamma(rgb.G * 1.0f / 255);
            double R = Gamma(rgb.R * 1.0f / 255);
            double X = 0.412453 * R + 0.357580 * G + 0.180423 * B;
            double Y = 0.212671 * R + 0.715160 * G + 0.072169 * B;
            double Z = 0.019334 * R + 0.119193 * G + 0.950227 * B;

            X/= 0.95047;
            Y/= 1.0;
            Z/= 1.08883;

            double FX = X > 0.008856f ? Math.Pow(X, 1.0f / 3.0f) : (7.787f * X + 0.137931f);
            double FY = Y > 0.008856f ? Math.Pow(Y, 1.0f / 3.0f) : (7.787f * Y + 0.137931f);
            double FZ = Z > 0.008856f ? Math.Pow(Z, 1.0f / 3.0f) : (7.787f * Z + 0.137931f);

            Lab lab = new Lab();
            lab.L = Y > 0.008856f ? (116.0f * FY - 16.0f) : (903.3f * Y);
            lab.A = 500f * (FX - FY);
            lab.B = 200f * (FY - FZ);

            return lab;
        }

        internal static double Gamma(double x)
        {
            return x > 0.04045 ? Math.Pow((x + 0.055f) / 1.055f, 2.4f) : x / 12.92;
        }
    }
}