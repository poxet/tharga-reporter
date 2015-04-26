using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public sealed class Line : SinglePageAreaElement
    {
        private readonly Color _defaultColor = Color.Black;
        private readonly UnitValue _defaultThickness = "0.1px";

        private Color? _color;
        private UnitValue? _thickness;
        private string _hideValue;

        public Color Color { get { return _color ?? _defaultColor; } set { _color = value; } }
        public UnitValue Thickness { get { return _thickness ?? _defaultThickness; } set { _thickness = value; } }
        public string HideValue { get { return _hideValue ?? string.Empty; } set { _hideValue = value; } }

        internal override void Render(IRenderData renderData)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = renderData.DocumentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return;
            }

            if (IsNotVisible(renderData)) return;

            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (renderData.IncludeBackground || !IsBackground)
            {
                var borderWidth = UnitValue.Parse(Thickness);
                var pen = new XPen(XColor.FromArgb(Color), borderWidth.ToXUnit(0));

                if (HorixontalSwap(renderData.ParentBounds))
                    renderData.Graphics.DrawLine(pen, renderData.ElementBounds.Right, renderData.ElementBounds.Top, renderData.ElementBounds.Left, renderData.ElementBounds.Bottom);
                else if (VerticalSwap(renderData.ParentBounds))
                    renderData.Graphics.DrawLine(pen, renderData.ElementBounds.Left, renderData.ElementBounds.Bottom, renderData.ElementBounds.Right, renderData.ElementBounds.Top);
                else
                    renderData.Graphics.DrawLine(pen, renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Right, renderData.ElementBounds.Bottom);
            }
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_color != null)
                xme.SetAttribute("Color", string.Format("{0}{1}{2}", _color.Value.R.ToString("X2"), _color.Value.G.ToString("X2"), _color.Value.B.ToString("X2")));

            if (_thickness != null)
                xme.SetAttribute("Thickness", Thickness.ToString());

            if (_hideValue != null)
                xme.SetAttribute("HideValue", _hideValue);

            return xme;
        }

        internal static Line Load(XmlElement xme)
        {
            var line = new Line();

            line.AppendData(xme);

            var xmlBorderColor = xme.Attributes["Color"];
            if (xmlBorderColor != null)
                line.Color = xmlBorderColor.Value.ToColor();

            var xmlHideValue = xme.Attributes["HideValue"];
            if (xmlHideValue != null)
                line.HideValue = xmlHideValue.Value;

            var xmlAttribute = xme.Attributes["IsBackground"];
            if (xmlAttribute != null)
                line.IsBackground = bool.Parse(xmlAttribute.Value);

            var xmlName = xme.Attributes["Name"];
            if (xmlName != null)
                line.Name = xmlName.Value;

            var xmlThickness = xme.Attributes["Thickness"];
            if (xmlThickness != null)
                line.Thickness = xmlThickness.Value;

            return line;
        }
    }
}