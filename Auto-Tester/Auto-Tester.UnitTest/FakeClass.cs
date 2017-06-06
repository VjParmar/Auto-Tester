using System;

namespace Auto_Tester.UnitTest
{
    class FakeClass
    {
        private void FakeFunction()
        {
            Console.WriteLine(@"Fake function called");
        }
        protected void FakeFunctionWithString(string value)
        {
            var substring = value.Substring('-');
            Console.WriteLine(substring);
            Console.WriteLine(@"Fake Function With String Function called");
        }
        public void FakeFunctionWithIntNullable(int? value)
        {
            if (value != null)
            {
                var number = value.Value;
                var result = number / 1;
                Console.WriteLine(result);
            }
            Console.WriteLine(@"Fake Function With Int Nullable Function called");
        }
        public void FakeFunctionWithObject(FakeModel value)
        {
            var res = value.Name.Substring(0);
            var num = value.Number;
            Console.WriteLine(@"Fake Function With Object Function called");
        }
    }
}
