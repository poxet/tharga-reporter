using System.Xml;
using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Tests.Serializing
{
    public class When_serializing_a_defailt_referencePoint : AaaTest
    {
        private XmlElement _xme;
        private ReferencePoint _other;
        private ReferencePoint _item;

        protected override void Arrange()
        {
            _item = new ReferencePoint();
            _xme = _item.ToXme();
        }

        protected override void Act()
        {
            _other = ReferencePoint.Load(_xme);
        }

        [Test]
        public void Then_Left_should_be_correct()
        {
            Assert.AreEqual(_item.Left, _other.Left);
        }

        [Test]
        public void Then_Top_should_be_correct()
        {
            Assert.AreEqual(_item.Top, _other.Top);
        }

        [Test]
        public void Then_Stack_should_be_correct()
        {
            Assert.AreEqual(_item.Stack, _other.Stack);
        }

        [Test]
        public void Then_ElementList_should_be_correct()
        {
            Assert.AreEqual(_item.ElementList, _other.ElementList);
        }

        [Test]
        public void Then_Name_should_be_correct()
        {
            Assert.AreEqual(_item.Name, _other.Name);
        }

        [Test]
        public void Then_IsBackground_should_be_correct()
        {
            Assert.AreEqual(_item.IsBackground, _other.IsBackground);
        }

        [Test]
        public void Then_string_conversion_should_be_correct()
        {
            Assert.AreEqual(_item.ToString(), _other.ToString());
        }

        [Test]
        public void Then_xml_conversion_should_be_correct()
        {
            Assert.AreEqual(_xme.OuterXml, _other.ToXme().OuterXml);
        }
    }
}