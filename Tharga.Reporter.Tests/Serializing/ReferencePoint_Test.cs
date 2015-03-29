using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Text = Tharga.Reporter.Engine.Entity.Element.Text;

namespace Tharga.Reporter.Tests.Serializing
{
    [TestFixture]
    public class ReferencePoint_test
    {
        [Test]
        public void Text_with_all_propreties_set()
        {
            //Arrange
            var referencePoint = new ReferencePoint
                {
                    IsBackground = true,
                    Name = "Rea Padda",
                    Left = UnitValue.Parse("10cm"),
                    Top = UnitValue.Parse("3px"),
                    Stack = ReferencePoint.StackMethod.Vertical,
                    ElementList = new ElementList
                        {
                            new Image(),
                            new Line(),
                            new Rectangle(),
                            new Table(),
                            new Text(),
                            new TextBox(),
                        },
                };
            var xme = referencePoint.ToXme();

            //Act
            var other = ReferencePoint.Load(xme);

            //Assert
            Assert.AreEqual(referencePoint.Left, other.Left);
            Assert.AreEqual(referencePoint.Top, other.Top);
            Assert.AreEqual(referencePoint.Stack, other.Stack);
            Assert.AreEqual(referencePoint.ElementList.Count, other.ElementList.Count);
            Assert.AreEqual(referencePoint.Name, other.Name);
            Assert.AreEqual(referencePoint.IsBackground, other.IsBackground);
            Assert.AreEqual(referencePoint.Name, other.Name);
            Assert.AreEqual(referencePoint.ToString(), other.ToString());
            Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
        }
    }
}