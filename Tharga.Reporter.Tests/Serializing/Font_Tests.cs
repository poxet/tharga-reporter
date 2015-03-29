using System;
using System.Drawing;
using NUnit.Framework;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing
{
    [TestFixture]
    public class Font_tests
    {
        [Test]
        public void Default_Font()
        {
            //Arrange
            var font = new Font();
            var xme = font.ToXme();

            //Act
            var otherLine = Font.Load(xme);

            //Assert
            Assert.AreEqual(font.Color, otherLine.Color);
            Assert.AreEqual(font.FontName, otherLine.FontName);
            Assert.AreEqual(font.Size, otherLine.Size);
            Assert.AreEqual(font.Bold, otherLine.Bold);
            Assert.AreEqual(font.Italic, otherLine.Italic);
            Assert.AreEqual(font.Strikeout, otherLine.Strikeout);
            Assert.AreEqual(font.Underline, otherLine.Underline);
            Assert.AreEqual(font.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Font_with_all_properties_set()
        {
            //Arrange
            var font = new Font
            {
                Size = 12,
                Color = Color.Orange,
                FontName = "Some Name",
                Bold = true,
                Italic = true,
            };
            var xme = font.ToXme();

            //Act
            var otherLine = Font.Load(xme);

            //Assert
            Assert.AreEqual(font.Color.ToArgb(), otherLine.Color.ToArgb());
            Assert.AreEqual(font.FontName, otherLine.FontName);
            Assert.AreEqual(font.Size, otherLine.Size);
            Assert.AreEqual(font.Bold, otherLine.Bold);
            Assert.AreEqual(font.Italic, otherLine.Italic);
            Assert.AreEqual(font.Strikeout, otherLine.Strikeout);
            Assert.AreEqual(font.Underline, otherLine.Underline);
            Assert.AreEqual(font.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Font_with_Strikeout_set()
        {
            //Arrange
            var font = new Font
            {
                Strikeout = true,
            };
            var xme = font.ToXme();

            //Act
            var otherLine = Font.Load(xme);

            //Assert
            Assert.AreEqual(font.Color.ToArgb(), otherLine.Color.ToArgb());
            Assert.AreEqual(font.FontName, otherLine.FontName);
            Assert.AreEqual(font.Size, otherLine.Size);
            Assert.AreEqual(font.Bold, otherLine.Bold);
            Assert.AreEqual(font.Italic, otherLine.Italic);
            Assert.AreEqual(font.Strikeout, otherLine.Strikeout);
            Assert.AreEqual(font.Underline, otherLine.Underline);
            Assert.AreEqual(font.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Font_with_Unserline_set()
        {
            //Arrange
            var font = new Font
            {
                Underline = true,
            };
            var xme = font.ToXme();

            //Act
            var otherLine = Font.Load(xme);

            //Assert
            Assert.AreEqual(font.Color.ToArgb(), otherLine.Color.ToArgb());
            Assert.AreEqual(font.FontName, otherLine.FontName);
            Assert.AreEqual(font.Size, otherLine.Size);
            Assert.AreEqual(font.Bold, otherLine.Bold);
            Assert.AreEqual(font.Italic, otherLine.Italic);
            Assert.AreEqual(font.Strikeout, otherLine.Strikeout);
            Assert.AreEqual(font.Underline, otherLine.Underline);
            Assert.AreEqual(font.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Font_cannot_use_bold_with_underline()
        {
            //Arrange
            Exception exception = null;
            Font font = null;

            //Act
            try
            {
                font = new Font
                {
                    Bold = true,
                    Underline = true
                };
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //Assert
            Assert.AreEqual(typeof(InvalidOperationException), exception.ToType());
            Assert.That(font, Is.Null);
        }

        [Test]
        public void Font_cannot_use_italic_with_underline()
        {
            //Arrange
            Exception exception = null;
            Font font = null;
            
            //Act
            try
            {
                font = new Font
                {
                    Italic = true,
                    Underline = true
                };
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //Assert
            Assert.AreEqual(typeof(InvalidOperationException), exception.ToType());
            Assert.That(font, Is.Null);
        }

        [Test]
        public void Font_cannot_use_strikeout_with_underline()
        {
            //Arrange
            Exception exception = null;
            Font font = null;

            //Act
            try
            {
                font = new Font
                {
                    Strikeout = true,
                    Underline = true
                };
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //Assert
            Assert.AreEqual(typeof(InvalidOperationException), exception.ToType());
            Assert.That(font, Is.Null);
        }

        [Test]
        public void Font_cannot_use_bold_with_strikeout()
        {
            //Arrange
            Exception exception = null;
            Font font = null;

            //Act
            try
            {
                font = new Font
                {
                    Bold = true,
                    Strikeout = true
                };
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //Assert
            Assert.AreEqual(typeof(InvalidOperationException), exception.ToType());
            Assert.That(font, Is.Null);
        }

        [Test]
        public void Font_cannot_use_italic_with_strikeout()
        {
            //Arrange
            Exception exception = null;
            Font font = null;

            //Act
            try
            {
                font = new Font
                {
                    Italic = true,
                    Strikeout = true
                };
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //Assert
            Assert.AreEqual(typeof(InvalidOperationException), exception.ToType());
            Assert.That(font, Is.Null);
        }

        [Test]
        public void Font_can_use_bold_with_italic()
        {
            //Arrange
            Exception exception = null;
            Font font = null;

            //Act
            try
            {
                font = new Font
                {
                    Bold = true,
                    Italic = true
                };
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //Assert
            Assert.That(exception, Is.Null);
            Assert.That(font, Is.Not.Null);
        }
    }
}