using System;

namespace Auto_Tester.UnitTest
{
    class FakeClass
    {
        private void FakeFunction()
        {
            Console.WriteLine(@"Hello i am fake function");
        }
        protected void FakeFunctionWithString(string value)
        {
            var substring = value.Substring('-');
            Console.WriteLine(substring);
        }
        public void FakeFunctionWithIntNullable(int? value)
        {
            if (value != null)
            {
                var number = value.Value;
                var result = number / 0;
                Console.WriteLine(result);
            }
        }
        public void FakeFunctionWithObject(object value)
        {
            var result = (FakeClass) value;
            result.FakeFunction();
            Console.WriteLine(@"Function called");
        }
    }
}
