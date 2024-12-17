using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Core.Common
{
    /// <summary>
    /// LocalizableString extensions.
    /// </summary>
    public static class LocalizableStringExtensions
    {
        /// <summary>
        /// Multi-language Json friendly start pattern.
        /// </summary>
        public const string JsonFriendlyStartPattern = "$0010$";

        /// <summary>
        /// Multi-language Json friendly end pattern.
        /// </summary>
        public const string JsonFriendlyEndPattern = "$0011$";

        /// <summary>
        /// Converts string to <see cref="LocalizableString"/>.
        /// </summary>
        public static LocalizableString AsLocalizableString(this string s)
        {
            return new LocalizableString(s);
        }

        /// <summary>
        /// Converts Dictionary to <see cref="LocalizableString"/>.
        /// </summary>
        public static LocalizableString AsLocalizableString(this Dictionary<string, string> dict)
            => new(dict);

        /// <summary>
        /// Converts Dictionary to <see cref="LocalizableString"/>.
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
                var sb = new StringBuilder();
                foreach (DictionaryEntry pair in dict)
                {
                    sb.Append(LocalizableString.StartPattern)
                        .Append(pair.Key)
                        .Append(LocalizableString.StartPattern)
                        .Append(pair.Value?.ToString())
                        .Append(LocalizableString.EndPattern);
                }

                return new LocalizableString(sb.ToString());
            }

            return ls;
        }

        /// <summary>
        /// Converts Dictionary to <see cref="LocalizableString"/>.
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
        /// Converts LocalizableString to json compatible object (AsDictionary or AsString).
        /// </summary>
        public static object AsJsonCompatible(this LocalizableString localizableString)
        {
            if (localizableString.HaveMultipleLanguages)
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
            if (str.IndexOf(LocalizableString.StartPattern, StringComparison.Ordinal) >= 0
                && str.IndexOf(LocalizableString.EndPattern, StringComparison.Ordinal) >= 0)
            {
                str = StringReplace(str, LocalizableString.StartPattern, JsonFriendlyStartPattern);
                str = StringReplace(str, LocalizableString.EndPattern, JsonFriendlyEndPattern);

                original = StringReplace(original, LocalizableString.StartPattern, JsonFriendlyStartPattern);
                original = StringReplace(original, LocalizableString.EndPattern, JsonFriendlyEndPattern);

                throw new ArgumentException($"The string contains excess {nameof(LocalizableString)}.{nameof(LocalizableString.StartPattern)} and {nameof(LocalizableString)}.{nameof(LocalizableString.EndPattern)} characters in <{str}>.{Environment.NewLine}The string original: <{original}>.{Environment.NewLine}The {nameof(LocalizableString)}.{nameof(LocalizableString.StartPattern)} and {nameof(LocalizableString)}.{nameof(LocalizableString.EndPattern)} characters are replaced for JSON compatibility with symbols: {JsonFriendlyStartPattern}, {JsonFriendlyEndPattern}.");
            }

            if (str.IndexOf(LocalizableString.StartPattern, StringComparison.Ordinal) >= 0)
            {
                str = StringReplace(str, LocalizableString.StartPattern, JsonFriendlyStartPattern);

                original = StringReplace(original, LocalizableString.StartPattern, JsonFriendlyStartPattern);
                original = StringReplace(original, LocalizableString.EndPattern, JsonFriendlyEndPattern);

                throw new ArgumentException($"The string contains an excess {nameof(LocalizableString)}.{nameof(LocalizableString.StartPattern)} character in <{str}>.{Environment.NewLine}The string original: <{original}>.{Environment.NewLine}The {nameof(LocalizableString)}.{nameof(LocalizableString.StartPattern)} and {nameof(LocalizableString)}.{nameof(LocalizableString.EndPattern)} characters are replaced for JSON compatibility with symbols: {JsonFriendlyStartPattern}, {JsonFriendlyEndPattern}.");
            }

            if (str.IndexOf(LocalizableString.EndPattern, StringComparison.Ordinal) >= 0)
            {
                str = StringReplace(str, LocalizableString.EndPattern, JsonFriendlyEndPattern);

                original = StringReplace(original, LocalizableString.StartPattern, JsonFriendlyStartPattern);
                original = StringReplace(original, LocalizableString.EndPattern, JsonFriendlyEndPattern);

                throw new ArgumentException($"The string contains an excess {nameof(LocalizableString)}.{nameof(LocalizableString.EndPattern)} character in <{str}>.{Environment.NewLine}The string original: <{original}>.{Environment.NewLine}The {nameof(LocalizableString)}.{nameof(LocalizableString.StartPattern)} and {nameof(LocalizableString)}.{nameof(LocalizableString.EndPattern)} characters are replaced for JSON compatibility with symbols: {JsonFriendlyStartPattern}, {JsonFriendlyEndPattern}.");
            }
        }

        private static string StringReplace(string str, string oldValue, string newValue)
        {
#if NETSTANDARD
            return str.Replace(oldValue, newValue);
#else
            return str.Replace(oldValue, newValue, StringComparison.Ordinal);
#endif
        }
    }
}