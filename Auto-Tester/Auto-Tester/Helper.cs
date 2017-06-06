using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Auto_Tester
{
    public static class Helper
    {
        public static BindingFlags Bindingflag =
            BindingFlags.Default | BindingFlags.IgnoreCase | BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod |
            BindingFlags.CreateInstance | BindingFlags.NonPublic;

        public static ArrayList DefaultParametersArray;

        static Helper()
        {
            DefaultParametersArray = new ArrayList();
        }

        public static Dictionary<string, object> GetDictionaryForClassType(Type type)
        {
            var dict = new Dictionary<string, object>();
            var obj = Activator.CreateInstance(type);
            dict.Add("default", obj);
            return dict;
        }

        public static object[] GetParameterValueForMethod(Dictionary<string, object> dictionary, int i, ParameterInfo parameterInfo)
        {
            object item;
            var paramarray = DefaultParametersArray.ToArray();
            if (dictionary.TryGetValue(i.ToString(), out item))
            {
                try
                {
                    dynamic value;
                    TryConvertValue(parameterInfo.ParameterType, item.ToString(), out value);
                    paramarray[parameterInfo.Position] = value;
                }
                catch (Exception)
                {
                    paramarray[parameterInfo.Position] = null;
                }
            }
            return paramarray;
        }

        public static Dictionary<string, object> GetDictionaryForParamterType(string parameterTypeName)
        {
            Dictionary<string, object> dictionary;
            Tester.DictionaryListOfAllItemTypes.TryGetValue(parameterTypeName, out dictionary);
            return dictionary;
        }

        public static void UpdateDefaultParameterArray(ParameterInfo[] parameters)
        {
            DefaultParametersArray.Clear();
            foreach (var parameter in parameters)
            {
                DefaultParametersArray.Add(DefaultGenerator.GetDefaultValue(parameter.ParameterType));
            }
        }

        public static MethodInfo ValidateInput(Type type, string methodName)
        {
            if (type == null) throw new Exception("Type cannot be null");
            if (String.IsNullOrEmpty(methodName)) throw new Exception("Method name cannot be empty");

            MethodInfo methodInfo = type.GetMethod(methodName, Bindingflag);
            if (methodInfo == null) throw new Exception("Method not available in provided type");
            return methodInfo;
        }

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
                if (String.IsNullOrEmpty(stringValue))
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