
using System.Collections.Generic;

namespace ParkingParser2.Core
{
    public static class IniSettings
    {
        private static Dictionary<string, string> SettingDictionary = new Dictionary<string, string>();

        public static void Add(string Key, string Value)
        {
            SettingDictionary.Add(Key, Value);
        }

        public static void Edit(string Key, string NewValue)
        {
            SettingDictionary[Key] = NewValue;
        }

        public static void Remove(string Key)
        {
            SettingDictionary.Remove(Key);
        }

        public static string GetValue(string Key)
        {
            return SettingDictionary[Key];
        }
    }
}
