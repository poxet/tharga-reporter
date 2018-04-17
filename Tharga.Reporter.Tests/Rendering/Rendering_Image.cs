using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class Rendering_Image
    {
        private Mock<IGraphics> ArrangeGraphics()
        {
            var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            graphicsMock.Setup(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()));
            graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()));
            graphicsMock.Setup(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()));
            return graphicsMock;
        }

        private Mock<IRenderData> ArrangeRenderData(IGraphics graphics)
        {
            var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect { Width = 10, Height = 10 });
            renderDataMock.Setup(x => x.DocumentData).Returns((DocumentData)null);
            renderDataMock.Setup(x => x.Section).Returns(Mock.Of<Section>());
            renderDataMock.Setup(x => x.Graphics).Returns(graphics);
            renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
            renderDataMock.SetupGet(x => x.ElementBounds).Returns(new XRect { Width = 10, Height = 10 });
            renderDataMock.SetupGet(x => x.IncludeBackground).Returns(true);
            return renderDataMock;
        }

        [TestCase(Image.ECacheMode.Preferably, false)]
        [TestCase(Image.ECacheMode.Preferably, true)]
        [TestCase(Image.ECacheMode.Never, false)]
        [TestCase(Image.ECacheMode.Never, true)]
        [TestCase(Image.ECacheMode.Always, false)]
        [TestCase(Image.ECacheMode.Always, true)]
        public void When_rendering_image_with_http(Image.ECacheMode cacheMode, bool isBackground)
        {
            //Arrange
            var image = new Image
            {
                Source = "http://www.thargelion.se/images/thargelion-logo.png",
                CacheMode = cacheMode,
                IsBackground = isBackground
            };

            var graphicsMock = ArrangeGraphics();
            var renderDataMock = ArrangeRenderData(graphicsMock.Object);

            //Act
            image.Render(renderDataMock.Object);

            //Assert
            graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Once);
            graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Never);
            graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
        }

        [TestCase(Image.ECacheMode.Preferably, false)]
        [TestCase(Image.ECacheMode.Preferably, true)]
        [TestCase(Image.ECacheMode.Never, false)]
        [TestCase(Image.ECacheMode.Never, true)]
        [TestCase(Image.ECacheMode.Always, false)]
        [TestCase(Image.ECacheMode.Always, true)]
        public void When_rendering_image_with_local_file(Image.ECacheMode cacheMode, bool isBackground)
        {
            //Arrange
            var image = new Image
            {
                Source = @"Resources\thargelion-logo.png",
                CacheMode = cacheMode,
                IsBackground = isBackground
            };

            var graphicsMock = ArrangeGraphics();
            var renderDataMock = ArrangeRenderData(graphicsMock.Object);

            //Act
            image.Render(renderDataMock.Object);

            //Assert
            graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Once);
            graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Never);
            graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
        }

        [TestCase(Image.ECacheMode.Preferably, false)]
        [TestCase(Image.ECacheMode.Preferably, true)]
        [TestCase(Image.ECacheMode.Never, false)]
        [TestCase(Image.ECacheMode.Never, true)]
        [TestCase(Image.ECacheMode.Always, false)]
        [TestCase(Image.ECacheMode.Always, true)]
        public void When_rendering_image_with_empty_source(Image.ECacheMode cacheMode, bool isBackground)
        {
            //Arrange
            var image = new Image
            {
                CacheMode = cacheMode,
                IsBackground = isBackground
            };

            var graphicsMock = ArrangeGraphics();
            var renderDataMock = ArrangeRenderData(graphicsMock.Object);

            //Act
            image.Render(renderDataMock.Object);

            //Assert
            graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Once);
            graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Once);
        }

        [TestCase(Image.ECacheMode.Preferably, false)]
        [TestCase(Image.ECacheMode.Preferably, true)]
        [TestCase(Image.ECacheMode.Never, false)]
        [TestCase(Image.ECacheMode.Never, true)]
        [TestCase(Image.ECacheMode.Always, false)]
        [TestCase(Image.ECacheMode.Always, true)]
        public void When_rendering_image_with_missing_file(Image.ECacheMode cacheMode, bool isBackground)
        {
            //Arrange
            var image = new Image
            {
                Source = @"C:\missing\image.png",
                CacheMode = cacheMode,
                IsBackground = isBackground
            };

            var graphicsMock = ArrangeGraphics();
            var renderDataMock = ArrangeRenderData(graphicsMock.Object);

            //Act
            image.Render(renderDataMock.Object);

            //Assert
            graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Once);
            graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Once);
        }

        [TestCase(Image.ECacheMode.Preferably, false)]
        [TestCase(Image.ECacheMode.Preferably, true)]
        [TestCase(Image.ECacheMode.Never, false)]
        [TestCase(Image.ECacheMode.Never, true)]
        [TestCase(Image.ECacheMode.Always, false)]
        [TestCase(Image.ECacheMode.Always, true)]
        public void When_rendering_image_with_missing_http(Image.ECacheMode cacheMode, bool isBackground)
        {
            //Arrange
            var image = new Image
            {
                Source = @"http://some.missing/image.png",
                CacheMode = cacheMode,
                IsBackground = isBackground
            };

            var graphicsMock = ArrangeGraphics();
            var renderDataMock = ArrangeRenderData(graphicsMock.Object);

            //Act
            image.Render(renderDataMock.Object);

            //Assert
            graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Once);
            graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Once);
        }

        [TestCase(Image.ECacheMode.Preferably, false)]
        [TestCase(Image.ECacheMode.Preferably, true)]
        [TestCase(Image.ECacheMode.Never, false)]
        [TestCase(Image.ECacheMode.Never, true)]
        [TestCase(Image.ECacheMode.Always, false)]
        [TestCase(Image.ECacheMode.Always, true)]
        public void When_rendering_image_with_nonsense_source(Image.ECacheMode cacheMode, bool isBackground)
        {
            //Arrange
            var image = new Image
            {
                Source = "NON_SENSE_SOURCE_DATA",
                CacheMode = cacheMode,
                IsBackground = isBackground
            };

            var graphicsMock = ArrangeGraphics();
            var renderDataMock = ArrangeRenderData(graphicsMock.Object);

            //Act
            image.Render(renderDataMock.Object);

            //Assert
            graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            graphicsMock.Verify(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()), Times.Once);
            graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Once);
        }

        //TODO: Background images not rendered when background images are not included
        //TODO: Background images rendered when background images are included
        //TODO: Non background images rendered when background images are not included
        //TODO: Non background images rendered when background images are included
    }
}