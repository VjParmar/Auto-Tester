using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Auto_Tester.Dictionaries;

namespace Auto_Tester
{
    public static class Tester
    {
        private static BindingFlags bindingflag =
            BindingFlags.Default | BindingFlags.IgnoreCase | BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod |
            BindingFlags.CreateInstance | BindingFlags.NonPublic;

        public static Dictionary<string, Dictionary<string, object>> DictionaryListOfAllItemTypes;
        private static ArrayList _defaultParametersarray;

        static Tester()
        {
            DictionaryListOfAllItemTypes = new Dictionary<string, Dictionary<string, object>>();
            _defaultParametersarray = new ArrayList();
            var dictionaryLoaderList = new List<DictionaryLoader>
            {
                new StringDictionaryLoader(),
                new Int32DictionaryLoader()
            };
            LoadDefaultDictinaries(dictionaryLoaderList);
        }

        private static void LoadDefaultDictinaries(List<DictionaryLoader> dictionaryLoaderList)
        {
            foreach (var dictionaryLoader in dictionaryLoaderList)
            {
                DictionaryListOfAllItemTypes.Add(dictionaryLoader.DictionaryName, dictionaryLoader.LoadDictionaryData());
            }
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
            var type = parameter.ParameterType;
            var paramname = parameter.ParameterType.Name;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null) paramname = underlyingType.Name;
            }
            var dictionary = GetDictionaryForParamterType(paramname);
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
                    parametersArray = GetParameterValueForMethod(dictionary, i + 1, parameter);
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
            var paramvalue = "";
            if (parametersArray.Length > 0 && parametersArray[parameter.Position] != null)
            {
                paramvalue = parametersArray[parameter.Position].ToString();
            }

            throw new Exception(
                $"Error occured while calling  {Environment.NewLine} Class - {classInstance.GetType().Name} {Environment.NewLine} " +
                $"Method - {methodInfo}  {Environment.NewLine} Parameter value - {paramvalue}",
                ex);
        }

        private static object[] GetParameterValueForMethod(Dictionary<string, object> dictionary, int i, ParameterInfo parameterInfo)
        {
            object item;
            var paramarray = _defaultParametersarray.ToArray();
            if (dictionary.TryGetValue(i.ToString(), out item))
            {
                try
                {
                    dynamic value;
                    TryConvertValue(parameterInfo.ParameterType, item.ToString(), out value);
                    paramarray[parameterInfo.Position] = value;
                }
                catch (Exception ex)
                {
                    paramarray[parameterInfo.Position] = null;
                }
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

            MethodInfo methodInfo = type.GetMethod(methodName, bindingflag);
            if (methodInfo == null) throw new Exception("Method not available in provided type");
            return methodInfo;
        }

        public static void Test(Type type)
        {
            MethodInfo[] methodInfoArray = type.GetMethods(bindingflag);
            foreach (var methodInfo in methodInfoArray)
            {
                Test(type, methodInfo.Name);
            }
        }
        
        //Try Parse using Reflection
        public static bool TryConvertValue(Type targetType, string stringValue, out object convertedValue)
        {

            if (targetType == typeof(string))
            {
                convertedValue = Convert.ChangeType(stringValue, typeof(object));
                return true;
            }
            var nullableType = targetType.IsGenericType &&
                           targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullableType)
            {
                if (string.IsNullOrEmpty(stringValue))
                {
                    convertedValue = default(object);
                    return true;
                }
                targetType = new NullableConverter(targetType).UnderlyingType;
            }

            Type[] argTypes = { typeof(string), targetType.MakeByRefType() };
            var tryParseMethodInfo = targetType.GetMethod("TryParse", argTypes);
            if (tryParseMethodInfo == null)
            {
                convertedValue = default(object);
                return false;
            }

            object[] args = { stringValue, null };
            var successfulParse = (bool)tryParseMethodInfo.Invoke(null, args);
            if (!successfulParse)
            {
                convertedValue = default(object);
                return false;
            }

            convertedValue = args[1];
            return true;
        }
    }
}