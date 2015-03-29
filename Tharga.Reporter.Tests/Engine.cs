using System;
using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class Engine
    {
        [Test]
        public void Create_template_with_default_section()
        {
            //Arrange
            var section = new Section();

            //Act
            var template = new Template(section);

            //Assert
            Assert.AreEqual(1, template.SectionList.Count);

            Assert.IsNotNull(template.SectionList.First().Header);
            Assert.AreEqual(0, template.SectionList.First().Header.ElementList.Count);
            Assert.AreEqual(0, template.SectionList.First().Header.Height.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList.First().Header.Height.Unit);

            Assert.IsNotNull(template.SectionList.First().Footer);
            Assert.AreEqual(0, template.SectionList.First().Footer.ElementList.Count);
            Assert.AreEqual(0, template.SectionList.First().Footer.Height.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList.First().Footer.Height.Unit);

            Assert.IsNotNull(template.SectionList.First().Pane);
            Assert.AreEqual(0, template.SectionList.First().Pane.ElementList.Count);

            Assert.IsNotNull(template.SectionList.First().Margin);
            Assert.AreEqual(0, template.SectionList.First().Margin.Left.Value.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList.First().Margin.Left.Value.Unit);
            Assert.AreEqual(0, template.SectionList.First().Margin.Right.Value.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList.First().Margin.Right.Value.Unit);
            Assert.AreEqual(0, template.SectionList.First().Margin.Top.Value.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList.First().Margin.Top.Value.Unit);
            Assert.AreEqual(0, template.SectionList.First().Margin.Bottom.Value.Value);
            Assert.AreEqual(UnitValue.EUnit.Point, template.SectionList.First().Margin.Bottom.Value.Unit);
            Assert.IsNull(template.SectionList.First().Margin.Height);
            Assert.IsNull(template.SectionList.First().Margin.Width);
        }

        [Test]
        [Obsolete]
        public void Create_pdf_document()
        {
            ////Arrange
            //var template = new Template(new Section());

            ////Act
            //var byteArray = Rendering.CreatePDFDocument(template);

            ////Assert
            //Assert.IsTrue(byteArray.Length > 0);
        }

        [Test]
        public void Serialize_to_xml()
        {
            //Arrange
            //var section = Section.Create(); //TODO: Create a section builder that randomizes the construction of a document.
            //var text = Text.Create("Some text");
            //text.Font.FontName = "Times New Roman";
            //section.Header.ElementList.Add(text);
            //var template = Template.Create(section);

            ////Act
            //var xml = template.ToXml();
            //var otherTemplate = Template.Create(xml);
            //var otherXml = otherTemplate.ToXml();

            ////Assert
            //Assert.AreEqual(xml.InnerXml, otherXml.InnerXml);
            ////TODO: Also assert that the properties of the objects are equal
        }
    }
}
