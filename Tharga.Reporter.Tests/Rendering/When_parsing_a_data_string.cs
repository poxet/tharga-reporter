using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    public class When_parsing_a_data_string : AaaTest
    {
        private string _result;
        private string _input;
        private string _dataPart;
        private DocumentData _documentData;
        private string _dataValue;

        protected override void Arrange()
        {
            _dataPart = "DataX";
            _dataValue = "DataValue";
            _input = string.Format("ABC {{{0}}}", _dataPart);

            _documentData = new DocumentData();
            _documentData.Add(_dataPart, _dataValue);
        }

        protected override void Act()
        {
            _result = _input.ParseValue(_documentData, null, null);
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
        public void Then_the_output_contains_the_data()
        {
            Assert.AreEqual(string.Format("ABC {0}", _dataValue), _result);
        }
    }
}