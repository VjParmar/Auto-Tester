using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Auto_Tester.Properties;

namespace Auto_Tester
{
    public static class Tester
    {
        public static Dictionary<string, Dictionary<string, object>> DictionaryListOfAllItemTypes = new Dictionary<string, Dictionary<string, object>>();
        private static ArrayList _defaultParametersarray;
        static Tester()
        {
            DictionaryListOfAllItemTypes.Add("String", LoadStringDictionaryData());
            _defaultParametersarray = new ArrayList();
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
                _defaultParametersarray.Clear();
                foreach (var parameter in parameters)
                {
                    _defaultParametersarray.Add(DefaultGenerator.GetDefaultValue(parameter.ParameterType));
                }

                foreach (var parameter in parameters)
                {
                    InvokeMethod(classInstance, methodInfo, parameter);
                }
            }

            return false;
        }

        private static void InvokeMethod(object classInstance, MethodInfo methodInfo, ParameterInfo parameter)
        {
            object[] parametersArray = { };
            var dictionary = GetDictionaryForParamterType(parameter.ParameterType.Name);
            if (dictionary == null)
            {
                InvokeMethodWithDefaultValues(classInstance, methodInfo, parameter, parametersArray);
                return;
            }

            InvokeMethodWithDictionaryValues(classInstance, methodInfo, parameter, dictionary, parametersArray);
        }

        private static void InvokeMethodWithDictionaryValues(object classInstance, MethodInfo methodInfo,
            ParameterInfo parameter, Dictionary<string, object> dictionary, object[] parametersArray)
        {
            for (var i = 0; i < dictionary.Count; i++)
            {
                try
                {
                    parametersArray = GetParameterValueForMethod(dictionary, i + 1, parameter.Position);
                    methodInfo.Invoke(classInstance, parametersArray);
                }
                catch (Exception ex)
                {
                    ThrowException(classInstance, methodInfo, parameter, parametersArray, ex);
                }
            }
        }

        private static void InvokeMethodWithDefaultValues(object classInstance, MethodInfo methodInfo, ParameterInfo parameter,
            object[] parametersArray)
        {
            try
            {
                parametersArray = _defaultParametersarray.ToArray();
                methodInfo.Invoke(classInstance, parametersArray);
            }
            catch (Exception ex)
            {
                ThrowException(classInstance, methodInfo, parameter, parametersArray, ex);
            }
        }

        private static void ThrowException(object classInstance, MethodInfo methodInfo, ParameterInfo parameter,
            object[] parametersArray, Exception ex)
        {
            var paramvalue = parametersArray[parameter.Position] ?? "null";
            throw new Exception(
                $"Error occured while calling  {Environment.NewLine} Class - {classInstance.GetType().Name} {Environment.NewLine} " +
                $"Method - {methodInfo}  {Environment.NewLine} Parameter value - {paramvalue}",
                ex);
        }

        private static object[] GetParameterValueForMethod(Dictionary<string, object> dictionary, int i, int parameterPosition)
        {
            object item;
            var paramarray = _defaultParametersarray.ToArray();
            if (dictionary.TryGetValue(i.ToString(), out item))
            {
                paramarray[parameterPosition] = item;
            }
            return paramarray;
        }

        private static Dictionary<string, object> GetDictionaryForParamterType(string parameterTypeName)
        {
            Dictionary<string, object> dictionary;
            DictionaryListOfAllItemTypes.TryGetValue(parameterTypeName, out dictionary);
            return dictionary;
        }

        private static MethodInfo ValidateInput(Type type, string methodName)
        {
            if (type == null) throw new Exception("Type cannot be null");
            if (string.IsNullOrEmpty(methodName)) throw new Exception("Method name cannot be empty");

            MethodInfo methodInfo = type.GetMethod(methodName);
            if (methodInfo == null) throw new Exception("Method not available in provided type");
            return methodInfo;
        }

        private static Dictionary<string, object> LoadStringDictionaryData()
        {
            List<string> words = Resources.StringDictionary.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var stringDict = new Dictionary<string, object>();
            foreach (var word in words)
            {
                var strings = word.Split('-');
                var key = strings[0];
                var arr = strings.Skip(1).ToArray();
                var value = string.Join("-", arr);
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


public class DefaultGenerator
{
    public static object GetDefaultValue(Type parameter)
    {
        var defaultGeneratorType =
          typeof(DefaultGenerator<>).MakeGenericType(parameter);

        return defaultGeneratorType.InvokeMember(
          "GetDefault",
          BindingFlags.Static |
          BindingFlags.Public |
          BindingFlags.InvokeMethod,
          null, null, new object[0]);
    }
}

public class DefaultGenerator<T>
{
    public static T GetDefault()
    {
        return default(T);
    }
}