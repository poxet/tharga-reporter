using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine
{
    internal class MyGraphicsFactory : IGraphicsFactory
    {
        public IGraphics PrepareGraphics(PdfPage page, DocumentRenderer docRenderer, int ii)
        {
            var gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;
            docRenderer.RenderPage(gfx, ii + 1);
            return new MyGraphics(gfx);
        }
    }
}