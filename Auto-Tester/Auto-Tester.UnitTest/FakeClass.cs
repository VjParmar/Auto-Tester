using System;

namespace Auto_Tester.UnitTest
{
    class FakeClass
    {
        public void FakeFunction()
        {
            Console.WriteLine("Hello i am fake function");
        }
        public void FakeFunctionWithString(string value)
        {
            var substring = value.Substring('-');
            Console.WriteLine(substring);
        }
        public void FakeFunctionWithIntNullable(int? value)
        {
            var number = value.Value;
            var result = number / 0;
            Console.WriteLine(result);
        }
        public void FakeFunctionWithObject(object value)
        {
            var result = (FakeClass) value;
            result.FakeFunction();
            Console.WriteLine("Function called");
        }
    }
}
