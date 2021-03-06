﻿using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_pre_rendering_a_table : AaaTest
    {
        private Table _table;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;
        private DocumentData _documentData;

        protected override void Arrange()
        {
            _table = new Table();
            _table.AddColumn(new TableColumn { Value = "A1", Title = "A2" });
            _table.AddColumn(new TableColumn { Value = "B1", Title = "B2" });

            _documentData = new DocumentData();
            _documentData.Add("A1", "Data_A1");
            _documentData.Add("A2", "Data_A2");
            _documentData.Add("B1", "Data_B1");
            _documentData.Add("B2", "Data_B2");

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            _graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize());
            _graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.Setup(x => x.ParentBounds).Returns(It.IsAny<XRect>());
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
            _renderDataMock.Setup(x => x.ElementBounds).Returns(It.IsAny<XRect>());
            _renderDataMock.Setup(x => x.Section).Returns(new Section());
            _renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
            _renderDataMock.Setup(x => x.DocumentData).Returns(_documentData);
        }

        protected override void Act()
        {
            _table.PreRender(_renderDataMock.Object);
        }

        [Test]
        public void Nothing_is_rendered()
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