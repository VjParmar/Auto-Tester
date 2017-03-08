using System;
using System.Reflection;

namespace Auto_UnitTester
{
    public static class Tester
    {
        public static bool Test(Type type, string methodName)
        {
            var methodInfo = ValidateInput(type, methodName);

            var parameters = methodInfo.GetParameters();
            var classInstance = Activator.CreateInstance(type, null);
            if (parameters.Length == 0)
            {
                // This works fine
                methodInfo.Invoke(classInstance, null);
            }
            else
            {
                object[] parametersArray = { "Hello" };
                          
                methodInfo.Invoke(classInstance, parametersArray);
            }

            return false;
        }

        private static MethodInfo ValidateInput(Type type, string methodName)
        {
            if (type == null) throw new Exception("Type cannot be null");
            if (string.IsNullOrEmpty(methodName)) throw new Exception("Method name cannot be empty");

            MethodInfo methodInfo = type.GetMethod(methodName);
            if (methodInfo == null) throw new Exception("Method not available in provided type");
            return methodInfo;
        }
    }
}
