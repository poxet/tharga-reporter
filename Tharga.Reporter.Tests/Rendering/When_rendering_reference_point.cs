using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_rendering_reference_point : AaaTest
    {
        private ReferencePoint _referencePoint;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;
        private DocumentData _documentData;

        protected override void Arrange()
        {
            _documentData = new DocumentData();

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            _graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize { Height = 1000, Width = 1000 });
            _graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
            _graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()));

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _referencePoint = new ReferencePoint();
            _referencePoint.ElementList.Add(new TextBox { Value = "Some text!", Width = "10cm", Height = "10cm" });
            _referencePoint.ElementList.Add(new Line());

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect { Width = 200, Height = 200 });
            _renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
            _renderDataMock.Setup(x => x.Section).Returns(new Section());
            _renderDataMock.Setup(x => x.DocumentData).Returns(_documentData);
            _renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 1));
            _renderDataMock.Setup(x => x.DebugData).Returns((IDebugData)null);
            _renderDataMock.Setup(x => x.IncludeBackground).Returns(false);
            _renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect { Width = 200, Height = 200 });
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
            _renderDataMock.SetupGet(x => x.DocumentProperties).Returns(Mock.Of<DocumentProperties>());

            _referencePoint.PreRender(_renderDataMock.Object);
        }

        protected override void Act()
        {
            _referencePoint.Render(_renderDataMock.Object, 1);
        }

        [Test]
        public void Then_line_is_drawn()
        {
            _graphicsMock.Verify(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }

        [Test]
        public void Then_string_is_drawn()
        {
            _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Once);
        }

        [Test]
        public void Then_nothing_else_is_drawn()
        {
            _graphicsMock.Verify(x => x.DrawEllipse(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()), Times.Never());
            _graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XRect>(), It.IsAny<XStringFormat>()), Times.Never());
        }
    }
}