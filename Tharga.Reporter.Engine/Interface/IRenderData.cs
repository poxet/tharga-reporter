using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Util;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IRenderData
    {
        XRect ParentBounds { get; }
        XRect ElementBounds { get; set; }
        bool IncludeBackground { get; }
        IGraphics Graphics { get; }
        Section Section { get; }
        IDocumentData DocumentData { get; }
        PageNumberInfo PageNumberInfo { get; }
        IDebugData DebugData { get; }
        DocumentProperties DocumentProperties { get; }
    }
}