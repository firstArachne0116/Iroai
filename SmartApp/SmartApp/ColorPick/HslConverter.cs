using System.Drawing;

namespace SmartApp.ColorPick
{
    /// <summary>
    /// RGBカラーからHSLカラースベースへ変換する
    /// </summary>
    /// <param name="rgb">RGBカラー</param>
    /// <returns>HSLカラースベース</returns>
    internal static class HslConverter
    {
        internal static Hsl ToColor(Rgb rgb)
        {
            Color color = System.Drawing.Color.FromArgb(rgb.R, rgb.G, rgb.B);
            float hue = color.GetHue();
            float saturation = color.GetSaturation();
            float lightness = color.GetBrightness();

            Hsl hsl = new Hsl();
            hsl.H = (int)hue;
            hsl.S = (int)(saturation * 100);
            hsl.L = (int)(lightness * 100);

            return hsl;
        }
    }
}