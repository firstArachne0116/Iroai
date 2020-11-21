using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections;
using static SmartApp.ColorPick.Munsell;
using System.Text.RegularExpressions;
using System.Linq;
using SmartApp.Interface;
using Xamarin.Forms;

namespace SmartApp.ColorPick
{
	[Serializable]
	public class MunsellDat
	{
		public MunsellHue H { get; set; }
		public double V { get; set; }
		public double C { get; set; }
		public Yxy Yxy { get; set; }
		public Lch Lch { get; set; }

		public MunsellDat Clone() {
			return new MunsellDat {
				H = H == null ? null : new MunsellHue { Base = H.Base, Number = H.Number },
				V = V,
				C = C,
				Yxy = Yxy == null ? null : new Yxy { Y1 = Yxy.Y1, X = Yxy.X, Y2 = Yxy.Y2 },
				Lch = Lch == null ? null : new Lch { L = Lch.L, C = Lch.C, H = Lch.H }
			};
		}
	}

	[DataContract]
	public class MunsellTable : IEnumerable<MunsellDat>
	{
		[DataMember]
		private IEnumerable<MunsellDat> items;

        public MunsellTable()
        {
            try
            {
                //Stream stream = GetResouce();
                //string text = "";
                //using (var reader = new System.IO.StreamReader(stream))
                //{
                //    text = reader.ReadToEnd();
                //}

                string text = DependencyService.Get<IData>().GetData("MunsellAll.dat");
                string[] line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length > 0)
                {
                    int count = 0;
                    IList<MunsellDat> list = new List<MunsellDat>();
                    foreach (string str in line)
                    {
                        if (count == 0) { count++; continue; } // 1行目処理しない

                        string[] part = str.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (part.Length > 0)
                        {
                            MunsellDat data = new MunsellDat();
                            Regex regex = new Regex(@"([0-9.]+)([RYGBP]+)");
                            var m = regex.Match(part[0]);
                            if (!m.Success) throw new FormatException();
                            data.H = new MunsellHue(double.Parse(m.Groups[1].Value), (HueBase)Enum.Parse(typeof(HueBase), m.Groups[2].Value));
                            data.V = System.Convert.ToDouble(part[1]);
                            data.C = System.Convert.ToDouble(part[2]);
                            Yxy yxy = new Yxy();
                            yxy.X = System.Convert.ToDouble(part[3]);
                            yxy.Y2 = System.Convert.ToDouble(part[4]);
                            yxy.Y1 = System.Convert.ToDouble(part[5]);
                            data.Yxy = yxy;
                            data.Lch = YxyToLch(yxy);
                            list.Add(data);
                        }
                    }
                    items = list.AsQueryable();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 埋込リソース取得
        /// </summary>
        /// <returns></returns>
        private Stream GetResouce()
        {
            var name = "Resources.data.MunsellAll.dat";
            Stream stream;
            var asm = Assembly.GetExecutingAssembly();

            var a = asm.GetManifestResourceNames();
            // 何もせずにアセンブリから取れたなら返す。
            stream = asm.GetManifestResourceStream(name);
            if (stream != null)
            {
                return stream;
            }

            // パスとかいじってアセンブリから取ろうとする。
            string fullname = asm.FullName;
            string[] asmname = fullname.Split(",".ToCharArray());

            if (asmname.Length >= 2)
            {
                name = name.Replace("/", ".").Replace("\\", ".");
                name = asmname[0] + "." + name;
                stream = asm.GetManifestResourceStream(name);

                return stream;
            }

            //どれもダメだったら null
            return null;
        }

        public IEnumerator<MunsellDat> GetEnumerator() {
			foreach (var m in items) {
				yield return m;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

        internal static Lch YxyToLch(Yxy yxy)
        {
            double X = yxy.Y2 == 0 ? 0 : yxy.X * (yxy.Y1 / yxy.Y2);
            double Y = yxy.Y1;
            double Z = (1.0 - yxy.X - yxy.Y2) * (yxy.Y1 / yxy.Y2);

            X /= 0.95047;
            Y /= 1.0;
            Z /= 1.08883;

            double FX = X > 0.008856f ? Math.Pow(X, 1.0f / 3.0f) : (7.787f * X + 0.137931f);
            double FY = Y > 0.008856f ? Math.Pow(Y, 1.0f / 3.0f) : (7.787f * Y + 0.137931f);
            double FZ = Z > 0.008856f ? Math.Pow(Z, 1.0f / 3.0f) : (7.787f * Z + 0.137931f);

            Lab lab = new Lab();
            lab.L = Y > 0.008856f ? (116.0f * FY - 16.0f) : (903.3f * Y);
            lab.A = 500f * (FX - FY);
            lab.B = 200f * (FY - FZ);

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

            Lch lch = new Lch();
            lch.L = lab.L;
            lch.C = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
            lch.H = h;

            return lch;
        }
	}

	
}
