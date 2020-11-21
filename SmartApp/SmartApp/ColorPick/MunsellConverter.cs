using System;
using System.IO;
using System.Linq;
using static SmartApp.ColorPick.Munsell;

namespace SmartApp.ColorPick
{
	internal static class MunsellConverter
	{
		internal static readonly MunsellTable Table;

		static MunsellConverter() {
            Table = new MunsellTable();
		}

        /// <summary>
        /// RGBカラーからMUNSELLカラースベースへ変換する
        /// </summary>
        /// <param name="rgb">RGBカラー</param>
        /// <returns>MUNSELLカラースベース</returns>
        internal static Munsell ToColor(Rgb rgb) {

            Munsell item = new Munsell();

            Lch lch = LchConverter.ToColor(rgb);

            if (rgb.R == rgb.G && rgb.G == rgb.B)
            {
                Lab lab = LabConverter.ToColor(rgb);
                item.H = new MunsellHue { Base = HueBase.N };
                item.V = Math.Round(ConvertLtoV(lab.L), 1);
                return item;
            }

            var q = Table.Select(x => new {
                diff = Math.Abs(x.Lch.L - lch.L) + Math.Abs(x.Lch.C - lch.C) + Math.Abs(x.Lch.H - lch.H),
                self = x
            });
            var min = q.Min(x => x.diff);
            var munsell = q.Where(x => x.diff == min).FirstOrDefault().self;

            if (min < 3.0)
            {
                item.H = munsell.H;
                item.V = munsell.V;
                item.C = munsell.C;
                return item;
            }

            var hue = new MunsellHue { Base = munsell.H.Base, Number = munsell.H.Number };
            MunsellHue newHue;
            if (munsell.Lch.H > lch.H)
            {
                hue.Number -= 2.5;
                newHue = new MunsellHue { Base = hue.Base, Number = hue.Number };
            }
            else
            {
                hue.Number += 2.5;
                newHue = new MunsellHue { Base = munsell.H.Base, Number = munsell.H.Number };
            }
            var munsellX = FindMunsell(hue, munsell.V, munsell.C, true);

            newHue.Number += Math.Round((lch.H - Math.Min(munsell.Lch.H, munsellX.Lch.H))
                / Math.Abs(munsell.Lch.H - munsellX.Lch.H) * 2.5, 1, MidpointRounding.AwayFromZero);


            double newChroma;
            //彩度max min
            var c = Table.Where(x => x.H == munsell.H && x.V == munsell.V)
                .GroupBy(x => x.V).Select(x => new { min = x.Min(y => y.C), max = x.Max(y => y.C) }).First();

            if (c.min < munsell.C && munsell.C < c.max)
            {
                var chroma = munsell.Lch.C > lch.C ? munsell.C - 2.0 : munsell.C + 2.0;
                var munsellY = FindMunsell(munsell.H, munsell.V, chroma);
                newChroma = Math.Round(Math.Min(munsell.C, munsellY.C) + (lch.C - Math.Min(munsell.Lch.C, munsellY.Lch.C))
                                / Math.Abs(munsell.Lch.C - munsellY.Lch.C) * 2.0, 1, MidpointRounding.AwayFromZero);
            }
            else
            {
                newChroma = Math.Round(munsell.C / munsell.Lch.C * lch.C, 1, MidpointRounding.AwayFromZero);
            }

            var newValue = Math.Round(ConvertLtoV(lch.L), 1, MidpointRounding.AwayFromZero);

            item.H = newHue;
            item.C = newChroma;
            item.V = newValue;

            return item;
		}

        private static double ConvertVtoL(double v) {
			return -0.0421 * Math.Pow(v, 2.0) + 10.527 * v - 0.1402;
		}
		private static double ConvertLtoV(double l) {
			if(l < 5.0) {
				return 0.0977 * Math.Pow(l, 0.9988);
			}
			else {
				return 0.0846 * Math.Pow(l, 1.0342);
			}
		}

		private static MunsellDat FindMunsell(MunsellHue hue, double value, double chroma,bool retry=false) {
			var rec = Table.Where(x => x.H.Base == hue.Base && x.H.Number == hue.Number && x.V == value && x.C == chroma).FirstOrDefault();

			if (rec!=null) {
				return rec.Clone();
			}
			else {
				if (!retry) return null;
				//見つからなかった場合は彩度を下げて再検索
				for (var i = chroma - 2.0; i >= 2.0; i -= 2.0) {
					var m = Table.Where(x => x.H == hue && x.V == value && x.C == i).FirstOrDefault();
					if (m != null) {
						return m.Clone();
					}
				}
				return null;
			}
		}
    }
}
