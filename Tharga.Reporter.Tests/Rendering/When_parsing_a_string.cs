using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_parsing_a_string : AaaTest
    {
        private string _result;
        private string _input;

        protected override void Arrange()
        {
            _input = "ABC";
        }

        protected override void Act()
        {
            _result = _input.ParseValue(null, null, null);
        }

        [Test]
        public void Then_the_is_the_same_as_the_input()
        {
            Assert.AreEqual(_input, _result);
        }

        [Test]
        public void Then_the_output_does_not_end_with_whitespace()
        {
            Assert.AreNotEqual(' ', _result.Last());
        }
    }
}
