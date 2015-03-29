using System.Xml;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class Footer : Pane
    {
        private UnitValue? _height;

        public UnitValue Height
        {
            get { return _height ?? UnitValue.Parse("0"); }
            set { _height = value; }
        }

        internal Footer()
        {
        }

        public override XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var header = xmd.CreateElement("Footer");

            if (_height != null)
                header.SetAttribute("Height", Height.ToString());

            var elms = GetElements(xmd);
            foreach (var elm in elms)
                header.AppendChild(elm);

            return header;
        }

        public static new Footer Load(XmlElement xme)
        {
            var pane = new Footer();

            var xmeHeight = xme.Attributes["Height"];
            if (xmeHeight != null)
                pane.Height = UnitValue.Parse(xmeHeight.Value);

            var elms = GetElements(xme);
            pane.ElementList.AddRange(elms);

            return pane;
        }
    }
}