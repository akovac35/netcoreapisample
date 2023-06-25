using NUnit.Framework;

namespace WebApiTests.Integration
{
    [TestFixture]
    [NonParallelizable]
    public abstract class TestBase
    {
        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {

        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
        }
    }
}
