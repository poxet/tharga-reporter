using System;
using System.Drawing;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine.Entity
{
    public class Font
    {
        private readonly Color _defaultColor = Color.Black;

        private string _fontName;
        private int? _size;
        private Color? _color;
        private bool? _bold;
        private bool? _italic;
        private bool? _underline;
        private bool? _strikeout;

        public string FontName { get { return _fontName ?? string.Empty; } set { _fontName = value; } }
        public int Size { get { return _size ?? 10; } set { _size = value; } }
        public Color Color { get { return _color ?? _defaultColor; } set { _color = value; } }

        public bool Bold
        {
            get { return _bold ?? false; }
            set
            {
                if (Underline) throw new InvalidOperationException("Cannot use Bold, Underline is set to true.");
                if (Strikeout) throw new InvalidOperationException("Cannot use Bold, Strikeout is set to true.");
                _bold = value;
            }
        }

        public bool Italic
        {
            get { return _italic ?? false; }
            set
            {
                if (Underline) throw new InvalidOperationException("Cannot use Italic, Underline is set to true.");
                if (Strikeout) throw new InvalidOperationException("Cannot use Italic, Strikeout is set to true.");
                _italic = value;
            }
        }

        public bool Underline
        {
            get { return _underline ?? false; }
            set
            {
                if (Bold) throw new InvalidOperationException("Cannot use Underline, Bold is set to true.");
                if (Italic) throw new InvalidOperationException("Cannot use Underline, Italic is set to true.");
                if (Strikeout) throw new InvalidOperationException("Cannot use Underline, Strikeout is set to true.");
                _underline = value;
            }
        }

        public bool Strikeout
        {
            get { return _strikeout ?? false; }
            set
            {
                if (Bold) throw new InvalidOperationException("Cannot use Strikeout, Bold is set to true.");
                if (Italic) throw new InvalidOperationException("Cannot use Strikeout, Italic is set to true.");
                if (Underline) throw new InvalidOperationException("Cannot use Strikeout, Underline is set to true.");
                _strikeout = value;
            }
        }

        internal XmlElement ToXme(string elementName = null)
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(elementName ?? GetType().ToShortTypeName());

            if (_color != null)
                xme.SetAttribute("Color", string.Format("{0}{1}{2}", _color.Value.R.ToString("X2"), _color.Value.G.ToString("X2"), _color.Value.B.ToString("X2")));

            if (_fontName != null)
                xme.SetAttribute("FontName", _fontName);

            if (_size != null)
                xme.SetAttribute("Size", _size.ToString());

            if (_bold != null)
                xme.SetAttribute("Bold", _bold.ToString());

            if (_italic != null)
                xme.SetAttribute("Italic", _italic.ToString());

            if (_strikeout != null)
                xme.SetAttribute("Strikeout", _strikeout.ToString());

            if (_underline != null)
                xme.SetAttribute("Underline", _underline.ToString());

            return xme;
        }

        internal static Font Load(XmlElement xme)
        {
            var line = new Font();

            var xmlBorderColor = xme.Attributes["Color"];
            if (xmlBorderColor != null)
                line.Color = xmlBorderColor.Value.ToColor();

            var xmlFontName = xme.Attributes["FontName"];
            if (xmlFontName != null)
                line.FontName = xmlFontName.Value;

            var xmlSize = xme.Attributes["Size"];
            if (xmlSize != null)
                line.Size = int.Parse(xmlSize.Value);

            var xmlBold = xme.Attributes["Bold"];
            if (xmlBold != null)
                line.Bold = bool.Parse(xmlBold.Value);

            var xmlItalic = xme.Attributes["Italic"];
            if (xmlItalic != null)
                line.Italic = bool.Parse(xmlItalic.Value);

            var xmlStrikeout = xme.Attributes["Strikeout"];
            if (xmlStrikeout != null)
                line.Strikeout = bool.Parse(xmlStrikeout.Value);

            var xmlUnderline = xme.Attributes["Underline"];
            if (xmlUnderline != null)
                line.Underline = bool.Parse(xmlUnderline.Value);

            return line;
        }
    }
}