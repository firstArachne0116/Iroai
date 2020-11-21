using System;

namespace SmartApp.ColorPick
{
	internal static class RgbConverter
	{
        /// <summary>
        /// HEXカラーからRGBカラーへ変換する
        /// </summary>
        /// <param name="item">HEXカラー</param>
        /// <returns>RGBカラースベース</returns>
		internal static Rgb ToColor(Hex item) {
			return new Rgb {
				R = Convert.ToInt32(item.R, 16),
				G = Convert.ToInt32(item.G, 16),
				B = Convert.ToInt32(item.B, 16)
			};
		}
	}
}
