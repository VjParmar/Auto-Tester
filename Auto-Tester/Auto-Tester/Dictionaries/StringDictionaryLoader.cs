using System;
using System.Collections.Generic;
using System.Linq;
using Auto_Tester.Properties;

namespace Auto_Tester.Dictionaries
{
    public class StringDictionaryLoader : DictionaryLoader
    {
        public StringDictionaryLoader()
        {
            DictionaryName = "String";
        }
        public override Dictionary<string, object> LoadDictionaryData()
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
            return stringDict;
        }
    }
}