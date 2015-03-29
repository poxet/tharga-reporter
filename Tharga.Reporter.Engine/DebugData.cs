using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine
{
    internal class DebugData : IDebugData
    {
        public DebugData()
        {
            Pen = new XPen(XColor.FromArgb(System.Drawing.Color.Blue), 0.1);
            Brush = new XSolidBrush(XColor.FromArgb(System.Drawing.Color.Blue));
            Font = new XFont("Verdana", 10);
        }

        public XPen Pen { get; private set; }
        public XBrush Brush { get; private set; }
        public XFont Font { get; private set; }
    }
}