using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class Template_Tests
    {
        [Test]
        public void Default_template()
        {
            //Arrange
            var template = new Template(new Section());
            var xml = template.ToXml();

            //Act
            var otherTemplate = Template.Load(xml);

            //Assert
            Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
        }   
    }
}