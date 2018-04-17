using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_rendering_a_barcode : AaaTest
    {
        private XRect _elementBounds;
        private BarCode _barCode;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;

        protected override void Arrange()
        {
            _barCode = new BarCode { Code = "ABC123" };

            var documentData = new Mock<IDocumentData>(MockBehavior.Strict);

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            _graphicsMock.Setup(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()));

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect { Width = 20, Height = 20 });
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>()).Callback<XRect>(x => _elementBounds = x);
            _renderDataMock.Setup(x => x.DocumentData).Returns(documentData.Object);
            _renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect { Width = 10, Height = 10 });
            _renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
            _renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));
        }

        protected override void Act()
        {
            _barCode.Render(_renderDataMock.Object);
        }

        [Test]
        public void Then_the_image_should_be_drawn()
        {
            _graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Once);
        }

        [Test]
        public void Then_the_element_bounds_is_set_to_some_width()
        {
            Assert.AreNotEqual(0, _elementBounds.Width);
        }

        [Test]
        public void Then_the_element_bounds_is_set_to_some_height()
        {
            Assert.AreNotEqual(0, _elementBounds.Height);
        }
    }
}