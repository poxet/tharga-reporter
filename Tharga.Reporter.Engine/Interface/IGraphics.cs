using PdfSharp.Drawing;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IGraphics
    {
        XSize MeasureString(string s, XFont font);
        XSize MeasureString(string s, XFont font, XStringFormat format);
        void DrawString(string s, XFont font, XBrush brush, XPoint point);
        void DrawString(string s, XFont font, XBrush brush, XPoint point, XStringFormat format);
        void DrawString(string s, XFont font, XBrush brush, XRect rect, XStringFormat format);        
        void DrawLine(XPen pen, double x1, double y1, double x2, double y2);
        void DrawRectangle(XPen pen, XRect rect);
        void DrawRectangle(XPen pen, XBrush brush, XRect rect);
        void DrawEllipse(XPen pen, double x, double y, int width, int height);
        void DrawImage(XImage image, XRect rect);
    }
}