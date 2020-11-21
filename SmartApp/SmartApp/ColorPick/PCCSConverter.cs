using System;
using System.IO;
using System.Linq;

namespace SmartApp.ColorPick
{
	internal static class PCCSConverter
	{
		internal static readonly PCCSTable Table;

		static PCCSConverter() {
            Table = new PCCSTable();
		}

        /// <summary>
        /// RGBカラーからPCCSカラースベースへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>PCCSカラースベース</returns>
        internal static string ToColor(Rgb rgb) {

            Lch lch = LchConverter.ToColor(rgb);

            var q = Table.Select(x => new {
                diff = Math.Abs(x.Lch.L - lch.L) + Math.Abs(x.Lch.C - lch.C) + Math.Abs(x.Lch.H - lch.H),
                self = x
            });
            var min = q.Min(x => x.diff);
            var PCCS = q.Where(x => x.diff == min).FirstOrDefault().self;

            return PCCS.Tone;
		}
    }
}
