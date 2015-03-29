using System.Drawing;
using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Font = Tharga.Reporter.Engine.Entity.Font;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;
using Text = Tharga.Reporter.Engine.Entity.Element.Text;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class Section_Test
    {
        [Test]
        public void Section_with_a_name()
        {
            //Arrange
            const string name = "ABC";
            var section = new Section
            {
                Name = name
            };
            var template = new Template(section);
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(section.DefaultFont.FontName, otherTemplate.SectionList.First().DefaultFont.FontName);
            Assert.AreEqual(section.DefaultFont.Size, otherTemplate.SectionList.First().DefaultFont.Size);
            Assert.AreEqual(section.DefaultFont.Color, otherTemplate.SectionList.First().DefaultFont.Color);
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
            Assert.AreEqual(name, otherTemplate.SectionList.First().Name);
        }

        [Test]
        public void Section_with_a_defaultFont()
        {
            //Arrange
            var section = new Section
            {
                DefaultFont = new Font
                {
                    FontName = "Times",
                    Size = 11,
                    Color = Color.MediumTurquoise
                }
            };
            var xme = section.ToXme();

            //Act
            var other = Section.Load(xme);

            //Assert
            Assert.AreEqual(section.Name, other.Name);
            Assert.AreEqual(section.DefaultFont.FontName, other.DefaultFont.FontName);
            Assert.AreEqual(section.DefaultFont.Size, other.DefaultFont.Size);
            Assert.AreEqual(section.DefaultFont.Color.ToArgb(), other.DefaultFont.Color.ToArgb());
            Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
        }

        [Test]
        public void Section_with_a_margin()
        {
            //Arrange
            var margin = new UnitRectangle { Top = UnitValue.Parse("1cm"), Left = UnitValue.Parse("2px"), Bottom = UnitValue.Parse("30mm"), Right = UnitValue.Parse("4cm") };
            var section = new Section
            {
                Margin = margin
            };
            var template = new Template(section);
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
            Assert.AreEqual(margin.Left, otherTemplate.SectionList.First().Margin.Left);
            Assert.AreEqual(margin.Top, otherTemplate.SectionList.First().Margin.Top);
            Assert.AreEqual(margin.Right, otherTemplate.SectionList.First().Margin.Right);
            Assert.AreEqual(margin.Bottom, otherTemplate.SectionList.First().Margin.Bottom);
            Assert.AreEqual(margin.Width, otherTemplate.SectionList.First().Margin.Width);
            Assert.AreEqual(margin.Height, otherTemplate.SectionList.First().Margin.Height);
            Assert.AreEqual(margin, otherTemplate.SectionList.First().Margin);
        }

        [Test]
        public void Section_with_header_margin()
        {
            //Arrange
            var section = new Section();
            section.Footer.Height = UnitValue.Parse("4inch");

            var template = new Template(section);
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
            Assert.IsTrue(template.SectionList.First().Header.Height.Equals(otherTemplate.SectionList.First().Header.Height));
            Assert.IsTrue(template.SectionList.First().Header.Height == otherTemplate.SectionList.First().Header.Height);
            Assert.AreEqual(template.SectionList.First().Header.Height, otherTemplate.SectionList.First().Header.Height);
        }

        [Test]
        public void Section_with_footer_margin()
        {
            //Arrange
            var section = new Section();
            section.Footer.Height = UnitValue.Parse("40");

            var template = new Template(section);
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
            Assert.AreEqual(template.SectionList.First().Footer.Height, otherTemplate.SectionList.First().Footer.Height);
        }

        [Test]
        public void Section_with_panes_with_element()
        {
            //Arrange
            var section = new Section();
            section.Header.ElementList.Add(new Line { Left = UnitValue.Parse("1cm") });
            section.Pane.ElementList.Add(new Line { Left = UnitValue.Parse("2cm") });
            section.Footer.ElementList.Add(new Line { Left = UnitValue.Parse("3cm") });

            var template = new Template(section);
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
            Assert.AreEqual(template.SectionList.First().Header.ElementList.First().Left, otherTemplate.SectionList.First().Header.ElementList.First().Left);
            Assert.AreEqual(template.SectionList.First().Pane.ElementList.First().Left, otherTemplate.SectionList.First().Pane.ElementList.First().Left);
            Assert.AreEqual(template.SectionList.First().Footer.ElementList.First().Left, otherTemplate.SectionList.First().Footer.ElementList.First().Left);
        }

        [Test]
        public void Section_with_all_types_of_elements()
        {
            //Arrange
            var section = new Section();
            section.Pane.ElementList.Add(new Image());
            section.Pane.ElementList.Add(new Line());
            section.Pane.ElementList.Add(new Rectangle());
            section.Pane.ElementList.Add(new Table());
            section.Pane.ElementList.Add(new Text());
            section.Pane.ElementList.Add(new TextBox());
            section.Pane.ElementList.Add(new ReferencePoint());
            section.Pane.ElementList.Add(new BarCode());

            var template = new Template(section);
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
        }
    }
}