using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_parsing_a_string_that_ends_with_whitespace : AaaTest
    {
        private string _result;
        private string _input;

        protected override void Arrange()
        {
            _input = "A ";
        }

        protected override void Act()
        {
            _result = _input.ParseValue(null, null, null);
        }

        [Test]
        public void Then_the_output_differs()
        {
            Assert.AreNotEqual(_input, _result);
        }

        [Test]
        public void Then_the_output_does_not_end_with_whitespace()
        {
            Assert.AreNotEqual(' ', _result.Last());
        }
    }
}