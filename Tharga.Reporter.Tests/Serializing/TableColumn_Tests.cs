using NUnit.Framework;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class TableColumn_Tests
    {
        [Test]
        public void Default_Table()
        {
            //Arrange
            var table = new TableColumn("A", "2cm", Table.WidthMode.Specific, Table.Alignment.Left, "***");
            var xme = table.ToXme();

            //Act
            var otherLine = TableColumn.Load(xme);

            //Assert
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Align, otherLine.Align);
            Assert.AreEqual(table.DisplayName, otherLine.DisplayName);
            Assert.AreEqual(table.Hide, otherLine.Hide);
            Assert.AreEqual(table.HideValue, otherLine.HideValue);
            Assert.AreEqual(table.WidthMode, otherLine.WidthMode);
            Assert.AreEqual(table.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}