namespace WebApiTests
{
    [TestFixture]
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
