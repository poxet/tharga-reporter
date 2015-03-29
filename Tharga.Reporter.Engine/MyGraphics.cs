using System.Diagnostics.CodeAnalysis;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine
{
    [ExcludeFromCodeCoverage]
    internal class MyGraphics : IGraphics
    {
        private readonly XGraphics _gfx;

        public MyGraphics(XGraphics gfx)
        {
            _gfx = gfx;
        }

        public XSize MeasureString(string s, XFont font)
        {
            return _gfx.MeasureString(s, font);
        }

        public XSize MeasureString(string s, XFont font, XStringFormat format)
        {
            return _gfx.MeasureString(s, font, format);
        }

        public void DrawString(string s, XFont font, XBrush brush, XPoint point)
        {
            _gfx.DrawString(s, font, brush, point);
        }

        public void DrawString(string s, XFont font, XBrush brush, XPoint point, XStringFormat format)
        {
            _gfx.DrawString(s, font, brush, point, format);
        }

        public void DrawString(string s, XFont font, XBrush brush, XRect rect, XStringFormat format)
        {
            _gfx.DrawString(s, font, brush, rect, format);
        }

        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {
            _gfx.DrawLine(pen, x1, y1, x2, y2);
        }

        public void DrawRectangle(XPen pen, XRect rect)
        {
            _gfx.DrawRectangle(pen, rect);
        }

        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            _gfx.DrawRectangle(pen, brush, rect);
        }

        public void DrawEllipse(XPen pen, double x, double y, int width, int height)
        {
            _gfx.DrawEllipse(pen, x, y, width, height);
        }

        public void DrawImage(XImage image, XRect rect)
        {
            _gfx.DrawImage(image, rect);
        }
    }
}