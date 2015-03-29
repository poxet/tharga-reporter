using System;
using NUnit.Framework;

namespace Tharga.Reporter.Tests
{
    public abstract class AaaTest
    {
        protected abstract void Arrange();
        protected abstract void Act();

        protected Type ThrownExceptionType { get { return ThrownException == null ? null : ThrownException.GetType(); } }
        protected Type ExpectedExceptionType { get; set; }
        protected Exception ThrownException { get; set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            Arrange();

            try
            {
                Act();
            }
            catch (Exception ex)
            {
                if (ex.GetType() != ExpectedExceptionType)
                {
                    throw;
                }

                ThrownException = ex;
            }
        }

        [TestFixtureTearDown]
        public virtual void Teardown()
        {
        }
    }
}