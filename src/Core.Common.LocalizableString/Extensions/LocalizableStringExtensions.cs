using System;
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
                    var currentString = localizableString.GetCurrentString(language);
                    EnsureIsJsonCompatible(currentString, localizableString.OriginalString);

                    dict[language] = currentString;
                }

                return dict;
            }

            var oneLanguageString = localizableString.StringCurrent;
            EnsureIsJsonCompatible(oneLanguageString, oneLanguageString);
            
            return oneLanguageString;
        }

        /// <summary>
        /// Throws an exception if the current string is not json compatible (the string contains excess special characters used in <see cref="LocalizableString"/>).
        /// </summary>
        private static void EnsureIsJsonCompatible(string str, string original)
        {
            if (str.IndexOf(LocalizableString.StartPattern, StringComparison.Ordinal) >= 0)
                throw new ArgumentException($"The string contains an excess {nameof(LocalizableString)}.{nameof(LocalizableString.StartPattern)} character in <{str}>.{Environment.NewLine}The string original: <{original}>.");

            if (str.IndexOf(LocalizableString.EndPattern, StringComparison.Ordinal) >= 0)
                throw new ArgumentException($"The string contains an excess {nameof(LocalizableString)}.{nameof(LocalizableString.EndPattern)} character in <{str}>.{Environment.NewLine}The string original: <{original}>.");
        }
    }
}