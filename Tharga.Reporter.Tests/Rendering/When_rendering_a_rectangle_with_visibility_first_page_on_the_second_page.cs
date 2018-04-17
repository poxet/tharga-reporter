using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_rendering_a_rectangle_with_visibility_first_page_on_the_second_page : AaaTest
    {
        private Rectangle _rectangle;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;
        private XRect _elementBounds;

        protected override void Arrange()
        {
            _rectangle = new Rectangle { Visibility = PageVisibility.FirstPage };

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            _graphicsMock.Setup(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()));

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>()).Callback<XRect>(x => _elementBounds = x);
            _renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect { Width = 10, Height = 10 });
            _renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(2, 2));
        }

        protected override void Act()
        {
            _rectangle.Render(_renderDataMock.Object);
        }

        [Test]
        public void Then_the_rectangle_is_not_drawn()
        {
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Never);
        }

        [Test]
        public void Then_the_element_bounds_is_set_to_width_0()
        {
            Assert.AreEqual(0, _elementBounds.Width);
        }

        [Test]
        public void Then_the_element_bounds_is_set_to_height_0()
        {
            Assert.AreEqual(0, _elementBounds.Height);
        }
    }
}