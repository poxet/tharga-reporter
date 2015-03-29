namespace Tharga.Reporter.Tests.Serializing
{
    public abstract class AaaTest
    {
        protected AaaTest()
        {
            Arrange();
            Act();
        }

        //[TestFixtureSetUp]
        protected abstract void Arrange();
        protected abstract void Act();
    }
}