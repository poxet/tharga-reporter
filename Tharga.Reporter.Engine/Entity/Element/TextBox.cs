using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class TextBox : MultiPageAreaElement
    {
        private readonly Font _defaultFont = new Font();

        private Font _font;
        private string _fontClass;
        private string _value;
        private string _hideValue;
        private List<string[]> _pageText;

        public string Value { get { return _value ?? string.Empty; } set { _value = value; } }
        public string HideValue { get { return _hideValue ?? string.Empty; } set { _hideValue = value; } }

        public Font Font
        {
            get { return _font ?? _defaultFont; }
            set
            {
                if (!string.IsNullOrEmpty(_fontClass)) throw new InvalidOperationException("Cannot set both Font and FontClass. FontClass has already been set.");
                _font = value;
            }
        }

        public string FontClass
        {
            get { return _fontClass ?? string.Empty; }
            set
            {
                if (_font != null) throw new InvalidOperationException("Cannot set both Font and FontClass. Font has already been set.");
                _fontClass = value;
            }
        }

        internal override int PreRender(IRenderData renderData)
        {
            _pageText = new List<string[]>();

            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (!renderData.IncludeBackground && IsBackground)
                return 0;

            var font = new XFont(_font.GetName(renderData.Section), _font.GetSize(renderData.Section), _font.GetStyle(renderData.Section));
            var brush = new XSolidBrush(XColor.FromArgb(_font.GetColor(renderData.Section)));

            var text = GetValue(renderData.DocumentData, renderData.PageNumberInfo, renderData.DocumentProperties);
            var textSize = renderData.Graphics.MeasureString(text, font, XStringFormats.TopLeft);

            var left = renderData.ElementBounds.Left;
            var top = renderData.ElementBounds.Top;

            if (textSize.Width > renderData.ElementBounds.Width || text.Contains(Environment.NewLine))
            {
                //Need to set data over more than one page
                var words = text.Split(' ');

                var sb = new StringBuilder();
                var lines = new List<string>();
                foreach (var nextWord in words)
                {
                    var nws = new[] { nextWord };
                    if (nextWord.Contains(Environment.NewLine))
                    {
                        nws = nextWord.Split(Environment.NewLine.ToArray());
                        nws = nws.Select(x => x == string.Empty ? Environment.NewLine : x).ToArray();
                    }

                    foreach (var nw in nws)
                    {
                        var textSoFar = sb.ToString();
                        if (nw != Environment.NewLine)
                            sb.AppendFormat("{0} ", nw);
                        var nextTextSize = renderData.Graphics.MeasureString(sb.ToString(), font, XStringFormats.TopLeft);

                        //Now we are over the limit (Previous state will fit)
                        if (nextTextSize.Width > renderData.ElementBounds.Width || nw == Environment.NewLine)
                        {
                            if (string.IsNullOrEmpty(textSoFar))
                            {
                                //One singe word that is too long, print it anyway
                                //renderData.Gfx.DrawString(sb.ToString(), font, brush, left, top, XStringFormats.TopLeft);
                                lines.Add(sb.ToString());
                            }
                            else
                            {
                                //renderData.Gfx.DrawString(ready, font, brush, left, top, XStringFormats.TopLeft);
                                lines.Add(textSoFar);
                                sb.Clear();
                                if (nw != Environment.NewLine)
                                    sb.AppendFormat("{0} ", nw);
                            }

                            top += nextTextSize.Height;

                            if (top > renderData.ElementBounds.Bottom - nextTextSize.Height)
                            {
                                //Now we have reached the limit of the page
                                _pageText.Add(lines.ToArray());
                                lines.Clear();
                                top = renderData.ElementBounds.Top;
                            }
                        }
                    }
                }

                lines.Add(sb.ToString());
                _pageText.Add(lines.ToArray());
            }
            else
                _pageText.Add(new[] { text });
            return _pageText.Count;
        }

        internal override void Render(IRenderData renderData, int page)
        {
            if (IsNotVisible(renderData))
                return;

            if (_pageText == null) throw new InvalidOperationException("Pre-render has not been performed.");

            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (!renderData.IncludeBackground && IsBackground)
                return;

            if (renderData.DebugData != null)
                renderData.Graphics.DrawRectangle(renderData.DebugData.Pen, renderData.ElementBounds);

            var font = new XFont(_font.GetName(renderData.Section), _font.GetSize(renderData.Section), _font.GetStyle(renderData.Section));
            var brush = new XSolidBrush(XColor.FromArgb(_font.GetColor(renderData.Section)));

            var text = GetValue(renderData.DocumentData, renderData.PageNumberInfo, renderData.DocumentProperties);
            var textSize = renderData.Graphics.MeasureString(text, font, XStringFormats.TopLeft);

            var left = renderData.ElementBounds.Left;
            var top = renderData.ElementBounds.Top;

            if (_pageText.Count > page - renderData.Section.GetPageOffset())
            {
                foreach (var line in _pageText[page - renderData.Section.GetPageOffset()])
                {
                    renderData.Graphics.DrawString(line, font, brush, new XPoint(left, top), XStringFormats.TopLeft);
                    var newTextSize = renderData.Graphics.MeasureString(line, font, XStringFormats.TopLeft);
                    top += newTextSize.Height;
                }
            }
        }

        private string GetValue(IDocumentData documentData, PageNumberInfo pageNumberInfo, DocumentProperties documentProperties)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = documentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return string.Empty;
            }

            return Value.ParseValue(documentData, pageNumberInfo, documentProperties);
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_font != null)
            {
                var fontXme = _font.ToXme();
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_fontClass != null)
                xme.SetAttribute("FontClass", _fontClass);

            if (_hideValue != null)
                xme.SetAttribute("HideValue", _hideValue);

            if (_value != null)
                xme.SetAttribute("Value", _value);

            return xme;
        }

        internal static TextBox Load(XmlElement xme)
        {
            var text = new TextBox();

            text.AppendData(xme);

            foreach (XmlElement child in xme)
            {
                switch (child.Name)
                {
                    case "Font":
                        text.Font = Font.Load(child);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to text base.", child.Name));
                }
            }

            var xmlFontClass = xme.Attributes["FontClass"];
            if (xmlFontClass != null)
                text.FontClass = xmlFontClass.Value;

            var xmlHideValue = xme.Attributes["HideValue"];
            if (xmlHideValue != null)
                text.HideValue = xmlHideValue.Value;

            var xmlValue = xme.Attributes["Value"];
            if (xmlValue != null)
                text.Value = xmlValue.Value;

            return text;
        }

        //TODO: Fix!
        //public override int PageCount(IRenderData renderData)
        //{
        //    return 2;
        //}
    }
}