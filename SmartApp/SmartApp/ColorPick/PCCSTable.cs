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
using Xamarin.Forms;
using SmartApp.Interface;

namespace SmartApp.ColorPick
{
	[Serializable]
	public class PCCSDat
	{
        public string Tone { get; set; }
		public Rgb Rgb { get; set; }
		public Lch Lch { get; set; }

		public PCCSDat Clone() {
            return new PCCSDat {
                Tone = Tone,
                Rgb = Rgb == null ? null : new Rgb { R = Rgb.R, G = Rgb.G, B = Rgb.B },
                Lch = Lch == null ? null : new Lch { L = Lch.L, C = Lch.C, H = Lch.H }
			};
		}
	}

	[DataContract]
	public class PCCSTable : IEnumerable<PCCSDat>
	{
		[DataMember]
		private IEnumerable<PCCSDat> items;

        public PCCSTable()
        {
            try
            {
                string text = DependencyService.Get<IData>().GetData("PCCSAll.dat");
                string[] line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length > 0)
                {
                    int count = 0;
                    IList<PCCSDat> list = new List<PCCSDat>();
                    foreach (string str in line)
                    {
                        if (count == 0) { count++; continue; } // 1行目処理しない

                        string[] part = str.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (part.Length > 0)
                        {
                            PCCSDat data = new PCCSDat();
                            data.Tone = part[0].ToString();
                            Rgb rgb = new Rgb();
                            rgb.R = System.Convert.ToInt16(part[1]);
                            rgb.G = System.Convert.ToInt16(part[2]);
                            rgb.B = System.Convert.ToInt16(part[3]);
                            data.Rgb = rgb;
                            Lch lch = new Lch();
                            lch = LchConverter.ToColor(rgb);
                            data.Lch = lch;
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
            var name = "ColorPick.PCCSAll.dat";
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

        public IEnumerator<PCCSDat> GetEnumerator() {
			foreach (var m in items) {
				yield return m;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
