﻿using System;

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
    }
}
