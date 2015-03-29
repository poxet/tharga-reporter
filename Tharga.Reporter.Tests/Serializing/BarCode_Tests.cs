using System.Drawing;
using NUnit.Framework;
using Tharga.Reporter.Engine.Entity.Element;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class BarCode_Tests
    {
        [Test]
        public void Default_BarCode()
        {
            //Arrange
            var text = new BarCode();
            var xme = text.ToXme();

            //Act
            var otherLine = BarCode.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);            
            Assert.AreEqual(text.Name, otherLine.Name);            
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.Code, text.Code);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void BarCode_with_all_propreties_set()
        {
            //Arrange
            var text = new BarCode
                {
                    Left = "1cm",
                    Top = "2cm",
                    Width = "10cm",
                    Height = "3cm",
                    IsBackground = true,
                    Name = "MyBarCode",         
                    Code = "123",
                };
            var xme = text.ToXme();

            //Act
            var otherLine = BarCode.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.Code, text.Code);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}