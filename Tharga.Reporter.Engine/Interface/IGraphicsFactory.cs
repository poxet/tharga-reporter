using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IGraphicsFactory
    {
        IGraphics PrepareGraphics(PdfPage page, DocumentRenderer docRenderer, int ii);
    }
}