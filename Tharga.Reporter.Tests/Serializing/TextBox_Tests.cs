using System.Drawing;
using MigraDoc.Rendering;
using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class TextBox_Tests
    {
        [Test]
        public void Default_Text()
        {
            //Arrange
            var text = new TextBox();
            var xme = text.ToXme();

            //Act
            var otherLine = TextBox.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Font.FontName, otherLine.Font.FontName);
            Assert.AreEqual(text.Font.Size, otherLine.Font.Size);
            Assert.AreEqual(text.Font.Color, otherLine.Font.Color);
            Assert.AreEqual(text.FontClass, otherLine.FontClass);
            Assert.AreEqual(text.HideValue, otherLine.HideValue);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.Value, otherLine.Value);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Text_with_fontclass()
        {
            //Arrange
            var text = new TextBox
            {
                FontClass = "Yahoo",
            };
            var xme = text.ToXme();

            //Act
            var otherLine = TextBox.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Font.FontName, otherLine.Font.FontName);
            Assert.AreEqual(text.Font.Size, otherLine.Font.Size);
            Assert.AreEqual(text.Font.Color.ToArgb(), otherLine.Font.Color.ToArgb());
            Assert.AreEqual(text.FontClass, otherLine.FontClass);
            Assert.AreEqual(text.HideValue, otherLine.HideValue);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.Value, otherLine.Value);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Text_with_all_propreties_set()
        {
            //Arrange
            var text = new TextBox
            {
                IsBackground = true,
                Name = "Rea Padda",
                Right = UnitValue.Parse("1px"),
                Width = UnitValue.Parse("10cm"),
                Bottom = UnitValue.Parse("2px"),
                Top = UnitValue.Parse("3px"),
                HideValue = "ABC",
                Value = "Bob Loblaw",
                Font = new Font
                {
                    FontName = "Verdana",
                    Color = Color.MistyRose,
                    Size = 13,
                },
            };
            var xme = text.ToXme();

            //Act
            var otherLine = TextBox.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Font.FontName, otherLine.Font.FontName);
            Assert.AreEqual(text.Font.Size, otherLine.Font.Size);
            Assert.AreEqual(text.Font.Color.ToArgb(), otherLine.Font.Color.ToArgb());
            Assert.AreEqual(text.FontClass, otherLine.FontClass);
            Assert.AreEqual(text.HideValue, otherLine.HideValue);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.Value, otherLine.Value);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        [Ignore("Can't gain access to internal stuff.")]
        public void When_using_onte_template_and_rendering_several_times_with_different_data()
        {
            var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize());
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize());
            graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()));

            var graphicsFactoryMock = new Mock<IGraphicsFactory>(MockBehavior.Strict);
            graphicsFactoryMock.Setup(x => x.PrepareGraphics(It.IsAny<PdfPage>(), It.IsAny<DocumentRenderer>(), It.IsAny<int>())).Returns(graphicsMock.Object);

            //Arrange
            var section = new Section();
            section.Pane.ElementList.Add(new TextBox { Value = "{Data}", Width = "2cm", Height = "2cm" });
            var templage = new Template(section);
            var data1 = new DocumentData();
            data1.Add("Data", "AAA BBB CCC DDD EEE FFF GGG HHH III JJJ KKK LLL MMM NNN OOO PPP QQQ RRR SSS TTT");
            var renderer1 = new Renderer(graphicsFactoryMock.Object, templage, data1);
            var binary1 = renderer1.GetPdfBinary();

            var data2 = new DocumentData();
            data2.Add("Data", "AAA BBB CCC DDD EEE FFF GGG HHH III JJJ KKK LLL MMM NNN OOO PPP QQQ RRR SSS TTT UUU VVV XXX YYY ZZZ");
            var renderer2 = new Renderer(graphicsFactoryMock.Object, templage, data2);

            //Act
            var binary2 = renderer2.GetPdfBinary();

            //Assert
        }
    }
}