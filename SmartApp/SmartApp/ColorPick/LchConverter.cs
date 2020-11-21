using System;

namespace SmartApp.ColorPick
{
    internal static class LchConverter
    {
        /// <summary>
        /// RGBカラーからLCHカラースベースへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>LCHカラースベース</returns>
        internal static Lch ToColor(Rgb rgb)
        {
            Lch lch = new Lch();
            Lab lab = LabConverter.ToColor(rgb);
            var h = Math.Atan2(lab.B, lab.A);

            if (h > 0)
            {
                h = (h / Math.PI) * 180.0;
            }
            else
            {
                h = 360 - (Math.Abs(h) / Math.PI) * 180.0;
            }

            if (h < 0)
            {
                h += 360.0;
            }
            else if (h >= 360)
            {
                h -= 360.0;
            }

            lch.L = lab.L;
            lch.C = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
            lch.H = h;

            return lch;
        }
    }
}