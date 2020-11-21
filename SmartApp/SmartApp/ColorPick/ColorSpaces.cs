using System;
using System.Text.RegularExpressions;

namespace SmartApp.ColorPick
{

    #region RGB
    public class Rgb
    {

        public int R { get; set; }

        public int G { get; set; }

        public int B { get; set; }
    }
    #endregion

    #region HEX
    public class Hex
    {
        private string _R;
        public string R
        {
            get { return _R; }
            set
            {
                _R = SetterCheck(value);
            }
        }
        private string _G;
        public string G
        {
            get { return _G; }
            set
            {
                _G = SetterCheck(value);
            }
        }
        private string _B;
        public string B
        {
            get { return _B; }
            set
            {
                _B = SetterCheck(value);
            }
        }

        private string SetterCheck(string s)
        {
            var regex = new Regex(@"^[0-9A-Fa-f]{1,2}$");
            if (!regex.IsMatch(s))
            {
                throw new FormatException();
            }
            return s;
        }

        public string Code
        {
            get { return "#" + R + G + B; }
            set
            {
                SetCode(value);
            }
        }

        private void SetCode(string value)
        {
            var regex1 = new Regex(@"^#{0,1}([0-9A-Fa-f]{1})([0-9A-Fa-f]{1})([0-9A-Fa-f]{1})$");
            var m = regex1.Match(value);
            if (m.Success)
            {
                this.R = string.Format("{0}{0}", m.Groups[1].Value);
                this.G = string.Format("{0}{0}", m.Groups[2].Value);
                this.B = string.Format("{0}{0}", m.Groups[3].Value);
                return;
            }

            var regex2 = new Regex(@"^#{0,1}([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})$");
            m = regex2.Match(value);
            if (m.Success)
            {
                this.R = m.Groups[1].Value;
                this.G = m.Groups[2].Value;
                this.B = m.Groups[3].Value;
                return;
            }

            throw new FormatException();
        }

        public Hex() { }
        public Hex(string code)
        {
            SetCode(code);
        }
    }
    #endregion

    #region HSL
    public class Hsl
    {
        private int _h;
        private int _s;
        private int _l;

        public int H
        {
            get { return this._h; }
            set
            {
                this._h = value;
                this._h = this._h > 360 ? 360 : this._h;
                this._h = this._h < 0 ? 0 : this._h;
            }
        }

        public int S
        {
            get { return this._s; }
            set
            {
                this._s = value;
                this._s = this._s > 255 ? 255 : this._s;
                this._s = this._s < 0 ? 0 : this._s;
            }
        }

        public int L
        {
            get { return this._l; }
            set
            {
                this._l = value;
                this._l = this._l > 255 ? 255 : this._l;
                this._l = this._l < 0 ? 0 : this._l;
            }
        }
    }
    #endregion

    #region HSV
    public class Hsv
    {

        private int _h;
        private int _s;
        private int _v;

        public int H
        {
            get { return this._h; }
            set
            {
                this._h = value;
                this._h = this._h > 360 ? 360 : this._h;
                this._h = this._h < 0 ? 0 : this._h;
            }
        }

        public int S
        {
            get { return this._s; }
            set
            {
                this._s = value;
                this._s = this._s > 255 ? 255 : this._s;
                this._s = this._s < 0 ? 0 : this._s;
            }
        }

        public int V
        {
            get { return this._v; }
            set
            {
                this._v = value;
                this._v = this._v > 255 ? 255 : this._v;
                this._v = this._v < 0 ? 0 : this._v;
            }
        }
    }
    #endregion

    #region CMYK
    public class Cmyk
    {

        public double C { get; set; }

        public double M { get; set; }

        public double Y { get; set; }

        public double K { get; set; }
    }
    #endregion

    #region Lab
    public class Lab
    {

        public double L { get; set; }

        public double A { get; set; }

        public double B { get; set; }
    }
    #endregion

    #region Munsell
    public class Munsell
    {

        private MunsellHue _H;
        public MunsellHue H
        {
            get { return _H; }
            set
            {
                _H = value;
                if (value.Number > 10.0)
                {
                    _H.Number = 10.0;
                }
                else if (value.Number < 0.0)
                {
                    _H.Number = 0.0;
                }
            }
        }

        public double V { get; set; }

        public double C { get; set; }

        public Munsell() { }
        public Munsell(string munsellstr) : this()
        {
            SetMunsellString(munsellstr);
        }

        public void SetMunsellString(string munsellstr)
        {
            Regex regex;
            if (munsellstr[0] == 'N')
            {
                regex = new Regex(@"N([0-9.]+)");
                var m = regex.Match(munsellstr);
                if (!m.Success) throw new FormatException();
                this.H = new MunsellHue { Base = HueBase.N };
                this.V = double.Parse(m.Groups[1].Value);
            }
            else
            {
                regex = new Regex(@"([0-9.]+)([RYGBP]+)\s([0-9.]+)\/([0-9.]+)");
                var m = regex.Match(munsellstr);
                if (!m.Success) throw new FormatException();
                this.H = new MunsellHue(double.Parse(m.Groups[1].Value), (HueBase)Enum.Parse(typeof(HueBase), m.Groups[2].Value));
                this.V = double.Parse(m.Groups[3].Value);
                this.C = double.Parse(m.Groups[4].Value);
            }
        }

        public override string ToString()
        {
            if (H.Base == HueBase.N)
            {
                return string.Format("N{0,0:0.0}", Math.Round(V, 1, MidpointRounding.AwayFromZero));
            }

            return string.Format("{0,0:0.#}{1} {2,0:0.0}/{3,0:0.#}",
                Math.Round(H.Number, 1, MidpointRounding.AwayFromZero),
                H.Base.ToString(),
                Math.Round(V, 1, MidpointRounding.AwayFromZero),
                Math.Round(C, 1, MidpointRounding.AwayFromZero)
                );
        }

        [Serializable]
        public enum HueBase
        {
            N,
            R,
            YR,
            Y,
            GY,
            G,
            BG,
            B,
            PB,
            P,
            RP
        }

        [Serializable]
        public class MunsellHue
        {

            public MunsellHue() { }
            public MunsellHue(double number, HueBase huebase)
            {
                this.Base = huebase;
                this.Number = number;
            }

            private double _Number;
            public double Number
            {
                get { return _Number; }
                set
                {
                    if (this.Base == HueBase.N)
                    {
                        return;
                    }
                    if (value == 0.0)
                    {
                        this.Base = this.Base == HueBase.R ? HueBase.RP : this.Base - 1;
                        _Number = 10.0;
                    }
                    else if (value > 10.0)
                    {
                        this.Base = this.Base == HueBase.RP ? HueBase.R : this.Base + 1;
                        if (value > 20.0)
                        {
                            _Number = 10.0;
                        }
                        else
                        {
                            _Number = value - 10.0;
                        }
                    }
                    else
                    {
                        _Number = value;
                    }
                }
            }

            public HueBase Base { get; set; }

            public static bool operator !=(MunsellHue x, MunsellHue y)
            {
                if ((object)x == null || (object)y == null)
                {
                    return (object)x != y;
                }
                else
                {
                    return (x.Number != y.Number || x.Base != y.Base);
                }
            }

            public static bool operator ==(MunsellHue x, MunsellHue y)
            {
                if (Object.ReferenceEquals(x, y))
                {
                    return true;
                }
                if ((object)x == null || (object)y == null)
                {
                    return (object)x == y;
                }
                else
                {
                    return (x.Number == y.Number && x.Base == y.Base);
                }
            }
            public override bool Equals(object x)
            {
                return this == (MunsellHue)x;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
    #endregion

    #region PCCSトーン分類

    #endregion

    #region JIS慣用

    #endregion

    #region Lch
    public class Lch
    {

        public double L { get; set; }

        public double C { get; set; }

        public double H { get; set; }
    }
    #endregion

    #region JIS慣用
    public class Yxy
    {

        public double Y1 { get; set; }

        public double X { get; set; }

        public double Y2 { get; set; }
    }
    #endregion
}