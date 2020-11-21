using System;
using System.IO;

namespace SmartApp.ColorPick
{

    public class ColorCode
    {
        #region RGB、HSL、HSV、CMYK、Lab、Munsell、PCCSトーン分類、JIS慣用 取得
        /// <summary>
        /// HEX文字列からRGBカラーへ変換する
        /// </summary>
        /// <param name="sHex">HEX文字列</param>
        /// <returns>RGBカラー</returns>
        public Rgb getRgb(string sHex)
        {
            Hex hex = new Hex(sHex);
            return RgbConverter.ToColor(hex);
        }

        /// <summary>
        /// RGBカラーからRGB文字列へ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>RGB文字列</returns>
        public string getRgbString(Rgb rgb)
        {
            return rgb.R.ToString() + "," + rgb.G.ToString() + "," + rgb.B.ToString();
        }

        /// <summary>
        /// RGBカラーからHSL文字列へ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>HSL文字列</returns>
        public string getHslString(Rgb rgb)
        {
            Hsl hsl = HslConverter.ToColor(rgb);
            return hsl.H.ToString() + "," + hsl.S.ToString() + "%," + hsl.L.ToString() + "%";
        }

        /// <summary>
        /// RGBカラーからHSV文字列へ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>HSV文字列</returns>
        public string getHsvString(Rgb rgb)
        {
            Hsv hsv = HsvConverter.ToColor(rgb);
            return hsv.H.ToString() + " " + hsv.S.ToString() + "% " + hsv.V.ToString() + "%";
        }

        /// <summary>
        /// RGBカラーからCMYK文字列へ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>CMYK文字列</returns>
        public string getCmykString(Rgb rgb)
        {
            Cmyk cmyk = CmykConverter.ToColor(rgb);
            return "C" + ((int)(cmyk.C * 255)).ToString() + "%"
                + " M" + ((int)(cmyk.M * 255)).ToString() + "%"
                + " Y" + ((int)(cmyk.Y * 255)).ToString() + "%"
                + " K" + ((int)(cmyk.K * 255)).ToString() + "%";
        }

        /// <summary>
        /// RGBカラーからLAB文字列へ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>LAB文字列</returns>
        public string getLabString(Rgb rgb)
        {
            Lab lab = LabConverter.ToColor(rgb);
            return "L" + ((int)lab.L).ToString()
                + " A" + ((int)lab.A).ToString()
                + " B" + ((int)lab.B).ToString();
        }

        /// <summary>
        /// RGBカラーからMunsell文字列へ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>Munsell文字列</returns>
        public string getMunsellString(Rgb rgb)
        {
     
            var r = new ColorMine.ColorSpaces.Rgb { R = rgb.R, G = rgb.G, B = rgb.B };
            var m = r.To<ColorMine.ColorSpaces.Munsell>();
            //Munsell munsell = MunsellConverter.ToColor(rgb);
            //return munsell.ToString();
            return m.ToString();
        }

        /// <summary>
        /// RGBカラーからPCCSのトーンへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>PCCSのトーン</returns>
        public string getPCCSTone(Rgb rgb)
        {
            return PCCSConverter.ToColor(rgb);
        }
        #endregion
    }
}
