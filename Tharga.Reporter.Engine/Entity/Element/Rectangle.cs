using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public sealed class Rectangle : SinglePageAreaElement
    {
        private readonly Color _defaultBorderColor = Color.Black;
        private readonly UnitValue _defaultBorderWidth = "0.1px";

        private Color? _borderColor;
        private UnitValue? _borderWidth;
        private Color? _backgroundColor;

        public Color BorderColor { get { return _borderColor ?? _defaultBorderColor; } set { _borderColor = value; } }
        public UnitValue BorderWidth { get { return _borderWidth ?? _defaultBorderWidth; } set { _borderWidth = value; } }
        public Color? BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }

        public Rectangle()
        {
            BorderWidth = UnitValue.Parse("1px");
        }

        internal override void Render(IRenderData renderData)
        {
            if (IsNotVisible(renderData)) return;

            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (!IsBackground || renderData.IncludeBackground)
            {
                var pen = new XPen(XColor.FromArgb(BorderColor), BorderWidth.GetXUnitValue(0));

                if (BackgroundColor != null)
                {
                    var brush = new XSolidBrush(XColor.FromArgb(BackgroundColor.Value));
                    renderData.Graphics.DrawRectangle(pen, brush, renderData.ElementBounds);
                }
                else
                    renderData.Graphics.DrawRectangle(pen, renderData.ElementBounds);
            }
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_backgroundColor != null)
                xme.SetAttribute("BackgroundColor", string.Format("{0}{1}{2}", _backgroundColor.Value.R.ToString("X2"), _backgroundColor.Value.G.ToString("X2"), _backgroundColor.Value.B.ToString("X2")));

            if (_borderColor != null)
                xme.SetAttribute("Color", string.Format("{0}{1}{2}", _borderColor.Value.R.ToString("X2"), _borderColor.Value.G.ToString("X2"), _borderColor.Value.B.ToString("X2")));

            if (_borderWidth != null)
                xme.SetAttribute("Thickness", _borderWidth.Value.ToString());

            return xme;
        }

        internal static Rectangle Load(XmlElement xme)
        {
            var rectangle = new Rectangle();

            rectangle.AppendData(xme);

            var xmlBackgroundColor = xme.Attributes["BackgroundColor"];
            if (xmlBackgroundColor != null)
                rectangle.BackgroundColor = xmlBackgroundColor.Value.ToColor();

            var xmlBorderColor = xme.Attributes["Color"];
            if (xmlBorderColor != null)
                rectangle.BorderColor = xmlBorderColor.Value.ToColor();

            var xmlBorderWidth = xme.Attributes["Thickness"];
            if (xmlBorderWidth != null)
                rectangle.BorderWidth = UnitValue.Parse(xmlBorderWidth.Value);

            return rectangle;
        }
    }
}