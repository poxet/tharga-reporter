using System;
using System.Drawing;
using System.Globalization;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine.Entity.Util
{
    public class SkipLine
    {
        private int _interval = 3;
        private UnitValue? _height;
        private Color? _borderColor;

        public int Interval
        {
            get { return _interval; }
            set
            {
                if (value < 1) throw new InvalidOperationException(string.Format("Interval needs to be larger than zero."));
                _interval = value;
            }
        }

        public UnitValue Height
        {
            get { return _height ?? "10px"; }
            set { _height = value; }
        }

        public Color? BorderColor { get { return _borderColor; } set { _borderColor = value; } }

        public XmlNode ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            xme.SetAttribute("Interval", Interval.ToString(CultureInfo.InvariantCulture));

            if (_height != null)
                xme.SetAttribute("Height", Height.ToString());


            if (_borderColor != null)
                xme.SetAttribute("BorderColor", string.Format("{0}{1}{2}", _borderColor.Value.R.ToString("X2"), _borderColor.Value.G.ToString("X2"), _borderColor.Value.B.ToString("X2")));

            return xme;
        }

        internal static SkipLine Load(XmlElement xme)
        {
            var skipLine = new SkipLine();

            var xmlInterval = xme.Attributes["Interval"];
            if (xmlInterval != null)
                skipLine.Interval = int.Parse(xmlInterval.Value);

            var xmlHeight = xme.Attributes["Height"];
            if (xmlHeight != null)
                skipLine.Height = xmlHeight.Value;
            
            var xmlBorderColor = xme.Attributes["BorderColor"];
            if (xmlBorderColor != null)
                skipLine.BorderColor = xmlBorderColor.Value.ToColor();

            return skipLine;
        }
    }
}