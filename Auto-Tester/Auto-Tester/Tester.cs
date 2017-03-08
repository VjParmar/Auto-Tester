using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using Auto_Tester.Properties;

namespace Auto_Tester
{
    public static class Tester
    {
        public static StringDictionary StringDictionary;

        static Tester()
        {
            StringDictionary = LoadStringDictionaryData();
        }

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
                foreach (DictionaryEntry item in StringDictionary)
                {
                    try
                    {
                        object[] parametersArray = {item.Value};
                        methodInfo.Invoke(classInstance, parametersArray);
                    }
                    catch (Exception ex)
                    {
                        throw  new Exception($"Error occured while calling  {Environment.NewLine} Class - {type.Name} {Environment.NewLine} Method - {methodInfo}  {Environment.NewLine} Parameter value - {item.Value}",ex);
                    }
                }
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

        private static StringDictionary LoadStringDictionaryData()
        {
            List<string> words = Resources.StringDictionary.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var stringDict = new StringDictionary();
            foreach (var word in words)
            {
                var strings = word.Split('-');
                var key = strings[0];
                var arr = strings.Skip(1).ToArray();
                var value = string.Join("", arr);
                if (value.ToLower() == "null")
                {
                    stringDict.Add(key, null);
                }
                else
                {
                    stringDict.Add(key, value);
                }
            }
            //Resources.StringDictionary; ;
            return stringDict;
        }
    }
}
