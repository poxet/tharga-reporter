using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class UnitValue_implicit_string_parse
    {
        [Test]
        public void From_string()
        {
            //Arrange
            var val1 = UnitValue.Parse("10cm");
            UnitValue val2;
            
            //Act
            val2 = "10cm";

            //Assert
            Assert.AreEqual(val1, val2);
        }

        [Test]
        public void To_string()
        {
            //Arrange
            var val1 = "10cm";

            //Act
            var val2 = UnitValue.Parse("10cm");

            //Assert
            Assert.AreEqual(val1, (string)val2);
        }
    }
}