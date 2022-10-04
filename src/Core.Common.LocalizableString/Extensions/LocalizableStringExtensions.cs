using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Core.Common
{
    /// <summary>
    /// LocalizableString extensions
    /// </summary>
    public static class LocalizableStringExtensions
    {
        /// <summary>
        /// String AsLocalizableString
        /// </summary>
        public static LocalizableString AsLocalizableString(this string s)
        {
            return new LocalizableString(s);
        }

        /// <summary>
        /// Dictionary AsLocalizableString
        /// </summary>
        public static LocalizableString AsLocalizableString(this Dictionary<string, string> dict)
        {
            LocalizableString ls;
            if (dict.Count == 1 &&
                dict.TryGetValue(string.Empty, out var value))
            {
                ls = new LocalizableString(value);
            }
            else
            {
                ls = new LocalizableString(string.Empty);
                foreach (var pair in dict)
                {
                    ls.SetString(pair.Key, pair.Value);
                }
            }

            return ls;
        }

        /// <summary>
        /// Dictionary AsLocalizableString
        /// </summary>
        public static LocalizableString AsLocalizableString(this OrderedDictionary dict)
        {
            LocalizableString ls;
            if (dict.Count == 1 &&
                dict.Contains(string.Empty))
            {
                var value = dict[string.Empty];
                ls = new LocalizableString(value?.ToString());
            }
            else
            {
                ls = new LocalizableString(string.Empty);
                foreach (DictionaryEntry pair in dict)
                {
                    ls.SetString(pair.Key.ToString(), pair.Value?.ToString());
                }
            }

            return ls;
        }

        /// <summary>
        /// Dictionary AsLocalizableString
        /// </summary>
        public static LocalizableString AsLocalizableString(this IDictionary dict)
        {
            var ordDict = new OrderedDictionary();
            foreach (DictionaryEntry pair in dict)
            {
                ordDict.Add(pair.Key.ToString(), pair.Value?.ToString());
            }

            var ls = ordDict.AsLocalizableString();

            return ls;
        }

        /// <summary>
        /// LocalizableString AsJsonCompatible (AsDictionary or AsString)
        /// </summary>
        public static object AsJsonCompatible(this LocalizableString localizableString)
        {
            var languages = localizableString.Languages;
            if (languages.Count > 0)
            {
                var dict = new OrderedDictionary();
                foreach (var language in languages)
                {
                    dict[language] = localizableString.GetCurrentString(language);
                }

                return dict;
            }

            return localizableString.StringCurrent;
        }
    }
}