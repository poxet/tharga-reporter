using System;
using System.Globalization;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity.Util
{
    public class SkipLine
    {
        private int _interval = 3;
        private UnitValue? _height;

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

        public XmlNode ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            xme.SetAttribute("Interval", Interval.ToString(CultureInfo.InvariantCulture));

            if (_height != null)
                xme.SetAttribute("Height", Height.ToString());

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

            return skipLine;
        }
    }
}