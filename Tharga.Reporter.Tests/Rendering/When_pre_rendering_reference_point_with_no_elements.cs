using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_pre_rendering_reference_point_with_no_elements : AaaTest
    {
        private ReferencePoint _referencePoint;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;
        private DocumentData _documentData;

        protected override void Arrange()
        {
            _documentData = new DocumentData();

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _referencePoint = new ReferencePoint();

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.Setup(x => x.ParentBounds).Returns(It.IsAny<XRect>());
            _renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
            _renderDataMock.Setup(x => x.Section).Returns(new Section());
            _renderDataMock.Setup(x => x.DocumentData).Returns(_documentData);
            _renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));
            _renderDataMock.Setup(x => x.DebugData).Returns((IDebugData)null);
            _renderDataMock.Setup(x => x.IncludeBackground).Returns(false);
            _renderDataMock.Setup(x => x.ElementBounds).Returns(It.IsAny<XRect>());
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
            _renderDataMock.SetupGet(x => x.DocumentProperties).Returns(Mock.Of<DocumentProperties>());
        }

        protected override void Act()
        {
            _referencePoint.PreRender(_renderDataMock.Object);
        }

        [Test]
        public void Then_nothing_is_rendered()
        {
            _graphicsMock.Verify(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawEllipse(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()), Times.Never());
            _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Never());
            _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()), Times.Never());
        }
    }
}