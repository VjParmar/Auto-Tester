using NUnit.Framework;

namespace Auto_Tester.UnitTest
{
    [TestFixture()]
    public class TesterTest
    {
        [Test]
        public void ProvidedValues_Call_All_FakeFunction()
        {
            Tester.LoadDictinaries
            Tester.Test(typeof(FakeClass));
            Assert.True(true);
        }

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

        [Test]
        public void ProvidedValues_Call_FakeFunctionWithIntNullableFunction()
        {
            Tester.Test(typeof(FakeClass), "FakeFunctionWithIntNullable");
            Assert.True(true);
        }

        [Test]
        public void ProvidedValues_Call_FakeFunctionWithObjectFunction()
        {
            Tester.Test(typeof(FakeClass), "FakeFunctionWithObject");
            Assert.True(true);
        }
    }
}
