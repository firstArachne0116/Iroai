using System.Text;
using SQLite;

namespace SmartApp.Model
{
    [Table("ImageColorInfo")]
    public class ImageColorInfo
    {

        /// <summary>
        /// キー、ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ColorId { get; set; }

        /// <summary>
        /// 画像ID
        /// </summary>
        public int ImageID { get; set; }


        /// <summary>
        /// 坐标X
        /// </summary>
        public int XValue { get; set; }

        /// <summary>
        /// 坐标Y
        /// </summary>
        public int YValue { get; set; }

        /// <summary>
        /// GRB值
        /// </summary>
        public string RGBValue { get; set; }

        /// <summary>
        /// HEX值
        /// </summary>
        public string HEXValue { get; set; }

        /// <summary>
        /// HSL值
        /// </summary>
        public string HSLValue { get; set; }

        /// <summary>
        /// HSV值
        /// </summary>
        public string HSVValue { get; set; }

        /// <summary>
        /// CMYK值
        /// </summary>
        public string CMYKValue { get; set; }

        /// <summary>
        /// LAB值
        /// </summary>
        public string LABValue { get; set; }

        /// <summary>
        /// Munsell值
        /// </summary>
        public string MUNSELLValue { get; set; }

        /// <summary>
        /// PCCS值
        /// </summary>
        public string PCCSValue { get; set; }

        /// <summary>
        /// JIS值
        /// </summary>
        public string JISValue { get; set; }

        /// <summary>
        /// ズーム倍率
        /// </summary>
        public double ScaledRatio { get; set; }

        /// <summary>
        /// 坐标Xオフセット
        /// </summary>
        public double XPis { get; set; }

        /// <summary>
        /// 坐标Yオフセット
        /// </summary>
        public double YPis { get; set; }

        /// <summary>
        /// 画像とレンズの坐标Xオフセット
        /// </summary>
        public double DefX { get; set; }

        /// <summary>
        /// 画像とレンズの坐标Yオフセット
        /// </summary>
        public double DefY { get; set; }
    }
}
