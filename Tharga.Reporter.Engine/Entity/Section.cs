using System;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity
{
    public class Section
    {
        private readonly Font _globalDefaultFont = new Font();

        private UnitRectangle _margin;
        private string _name;
        private Font _defaultFont;
        private int? _renderPageCount;
        private int? _pageOffset;
        private Header _header;
        private Footer _footer;

        public UnitRectangle Margin { get { return _margin ?? Margins.Create(UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0")); } set { _margin = value; } }
        public Pane Pane { get; private set; }
        public string Name { get { return _name ?? string.Empty; } set { _name = value; } }
        public Font DefaultFont { get { return _defaultFont ?? _globalDefaultFont; } set { _defaultFont = value; } }

        public Header Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (value == null) throw new NullReferenceException("Cannot assign an empty header to a section.");
                _header = value;
            }
        }

        public Footer Footer
        {
            get
            {
                return _footer;
            }
            set
            {
                if (value == null) throw new NullReferenceException("Cannot assign an empty footer to a section.");
                _footer = value;
            }
        }

        public Section()
        {
            Header = new Header();
            Pane = new Pane();
            Footer = new Footer();
        }

        internal XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var section = xmd.CreateElement("Section");

            var ownerDocument = section.OwnerDocument;
            if (ownerDocument == null) throw new NullReferenceException("ownerDocument");

            if (_name != null)
                section.SetAttribute("Name", Name);

            if (_margin != null)
            {
                var xmeMargin = Margin.ToXme("Margin");
                var importedSection = ownerDocument.ImportNode(xmeMargin, true);
                section.AppendChild(importedSection);
            }

            if (_defaultFont != null)
            {
                var xmeDefaultFont = DefaultFont.ToXme("DefaultFont");
                var importedDefaultFont = ownerDocument.ImportNode(xmeDefaultFont, true);
                section.AppendChild(importedDefaultFont);
            }

            var header = Header.ToXme();
            var importedHeader = ownerDocument.ImportNode(header, true);
            section.AppendChild(importedHeader);

            var pane = Pane.ToXme();
            var importedPane = ownerDocument.ImportNode(pane, true);
            section.AppendChild(importedPane);

            var footer = Footer.ToXme();
            var importedFooter = ownerDocument.ImportNode(footer, true);
            section.AppendChild(importedFooter);

            return section;
        }

        internal static Section Load(XmlElement xmlSection)
        {
            var section = new Section();

            var name = xmlSection.Attributes["Name"];
            if (name != null)
                section.Name = name.Value;

            foreach (XmlElement child in xmlSection)
            {
                switch (child.Name)
                {
                    case "Margin":
                        section.Margin = UnitRectangle.Load(child);                         
                        break;
                    case "Header":
                        section.Header = Header.Load(child);
                        break;
                    case "Footer":
                        section.Footer = Footer.Load(child);
                        break;
                    case "Pane":
                        section.Pane = Pane.Load(child);
                        break;
                    case "DefaultFont":
                        section.DefaultFont = Font.Load(child);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to section.", child.Name));
                }
            }

            return section;
        }

        public int GetPageOffset()
        {
            return _pageOffset ?? 0;
        }

        public void SetPageOffset(int pageOffset)
        {
            _pageOffset = pageOffset;
        }

        internal void SetRenderPageCount(int renderPageCount)
        {
            _renderPageCount = renderPageCount;
        }

        internal int GetRenderPageCount()
        {
            return _renderPageCount ?? 1;
        }
    }
}
