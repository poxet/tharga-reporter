using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_parsing_a_data_string_with_data_that_is_missing : AaaTest
    {
        private string _result;
        private string _input;
        private string _dataPart;

        protected override void Arrange()
        {
            _dataPart = "DataX";
            _input = string.Format("ABC {{{0}}}", _dataPart);
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

        [Test]
        public void Then_the_output_contains_missing_information()
        {
            Assert.AreEqual(string.Format("ABC [Data '{0}' is missing]", _dataPart), _result);
        }
    }
}