using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Util
{
    internal class RenderData : IRenderData
    {
        private readonly IGraphics _graphics;
        private readonly IDocumentData _documentData;
        private readonly bool _includeBackground;
        private readonly IDebugData _debugData;
        private readonly DocumentProperties _documentProperties;

        public RenderData(IGraphics graphics, XRect parentBounds, Section section, IDocumentData documentData, PageNumberInfo pageNumberInfo, IDebugData debugData, bool includeBackground, DocumentProperties documentProperties)
        {
            ParentBounds = parentBounds;
            Section = section;
            _graphics = graphics;
            _documentData = documentData;
            PageNumberInfo = pageNumberInfo;
            _debugData = debugData;
            _includeBackground = includeBackground;
            _documentProperties = documentProperties;
        }

        public XRect ParentBounds { get; private set; }
        public XRect ElementBounds { get; set; }
        public bool IncludeBackground { get { return _includeBackground; } }
        public IGraphics Graphics { get { return _graphics; } }
        public Section Section { get; private set; }
        public IDocumentData DocumentData { get { return _documentData; } }
        public PageNumberInfo PageNumberInfo { get; private set; }
        public IDebugData DebugData { get { return _debugData; } }
        public DocumentProperties DocumentProperties { get { return _documentProperties; } }
    }
}