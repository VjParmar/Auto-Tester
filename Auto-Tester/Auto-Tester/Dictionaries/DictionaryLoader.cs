using System.Collections.Generic;

namespace Auto_Tester.Dictionaries
{
    public abstract class DictionaryLoader
    {
        public abstract Dictionary<string, object> LoadDictionaryData();
        public string DictionaryName;
    }
}