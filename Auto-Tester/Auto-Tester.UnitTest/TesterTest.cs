using NUnit.Framework;

namespace Auto_Tester.UnitTest
{
    [TestFixture()]
    public class TesterTest
    {
        [Test]
        public void ProvidedValues_Call_FakeFunctionFunction()
        {
            Tester.Test(typeof(FakeClass), "FakeFunction");
            Assert.True(true);
        }

        [Test]
        public void ProvidedValues_Call_FakeFunctionWithStringFunction()
        {
            Tester.Test(typeof(FakeClass), "FakeFunctionWithString");
            Assert.True(true);
        }
    }
}
