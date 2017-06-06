using System;
using System.Collections.Generic;
using System.Reflection;
using Auto_Tester.Dictionaries;

namespace Auto_Tester
{
    public static class Tester
    {
        public static Dictionary<string, Dictionary<string, object>> DictionaryListOfAllItemTypes;

        static Tester()
        {
            DictionaryListOfAllItemTypes = new Dictionary<string, Dictionary<string, object>>();
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
            var methodInfo = Helper.ValidateInput(type, methodName);

            var parameters = methodInfo.GetParameters();
            var classInstance = Activator.CreateInstance(type, null);
            if (parameters.Length == 0)
            {
                // This works fine
                methodInfo.Invoke(classInstance, null);
            }
            else
            {
                Helper.UpdateDefaultParameterArray(parameters);

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
            Dictionary<string, object> dictionary;
            if (type.IsClass)
            {
                dictionary = Helper.GetDictionaryForClassType(type);
            }
            else
            {
                dictionary = Helper.GetDictionaryForParamterType(paramname);
            }
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
                    parametersArray = Helper.GetParameterValueForMethod(dictionary, i + 1, parameter);
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
                parametersArray = Helper.DefaultParametersArray.ToArray();
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

            paramvalue = paramvalue == "" ? "Empty or Null" : paramvalue;
            throw new Exception(
                $"Error occured while calling  {Environment.NewLine} Class - {classInstance.GetType().Name} {Environment.NewLine} " +
                $"Method - {methodInfo}  {Environment.NewLine} Parameter value - {paramvalue}",
                ex);
        }

        public static void Test(Type type)
        {
            var methodInfoArray = type.GetMethods(Helper.Bindingflag);
            foreach (var methodInfo in methodInfoArray)
            {
                Test(type, methodInfo.Name);
            }
        }
    }
}