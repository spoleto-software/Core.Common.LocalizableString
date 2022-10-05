using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Core.Common
{
    /// <summary>
    /// Multi-languages string.
    /// </summary>
    [Serializable]
    [DataContract]
    public class LocalizableString : IComparable, ICloneable, IConvertible, IComparable<string>, IEnumerable<char>, IEnumerable, IEquatable<string>
    {
        [IgnoreDataMember]
        private string _stringCurrent;
        
        [DataMember(Name = "StringOriginal")]
        private string _stringOriginal;
        
        [IgnoreDataMember]
        private string _currentLanguageKey;// key for the current language
        
        [IgnoreDataMember]
        private bool _haveMultipleLanguages;
        
        [NonSerialized]
        [IgnoreDataMember]
        private List<string> _languages;

        /// <summary>
        /// DefaultLanguageKey
        /// </summary>
        public const string DefaultLanguageKey = "ru";

        /// <summary>
        /// Multi-language start pattern.
        /// </summary>
        public const string StartPattern = "\u0010";//NCHAR(0016) is MSSQL equivalent.

        /// <summary>
        /// Multi-language end pattern.
        /// </summary>
        public const string EndPattern = "\u0011";//NCHAR(17) is MSSQL equivalent.

        //"\u0010ru\u0010текст на русском\u0011\u0010en\u0010text in english\u0011\u0010il\u0010טקסט בעברית\u0011"
        private const string Pattern = StartPattern + "([a-z]){2}" + StartPattern;

        /// <summary>
        /// HaveMultipleLanguages
        /// </summary>
        [IgnoreDataMember]
        public bool HaveMultipleLanguages => _haveMultipleLanguages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="originalString"></param>
        public LocalizableString(string originalString)
        {
            ProcessOriginalString(originalString);
        }

        /// <summary>
        /// Constructor with the language-value items.
        /// </summary>
        /// <param name="dictionary">The dictionary with the language-value items.</param>
        public LocalizableString(Dictionary<string, string> dictionary)
        {
            if (dictionary.Count == 1 &&
                dictionary.TryGetValue(string.Empty, out var value))
            {
                _stringOriginal = _stringCurrent = value;
                _haveMultipleLanguages = false;
            }
            else
            {
                _stringOriginal = _stringCurrent = string.Empty;
                foreach (var pair in dictionary)
                {
                    SetString(pair.Key, pair.Value);
                }
            }
        }

        [OnDeserialized]
        private void DeserializationInitializer(StreamingContext ctx)
        {
            ProcessOriginalString(_stringOriginal);
        }

        private void ProcessOriginalString(string s)
        {
            if (s == null) return;

            if (!Regex.IsMatch(s, Pattern))
            {
                _stringOriginal = _stringCurrent = s;
                _haveMultipleLanguages = false;
            }
            else
            {
                _haveMultipleLanguages = true;
                _stringOriginal = s;

                _stringCurrent = GetString(_stringOriginal, true);

                //17.12.2014 NadymovOleg: закоментировал below код. Т.к. если в строке нет значения для DefaultLanguageKey, то будет ошибка!
                //Например, для строки = "\u0010en\u0010text in english\u0011" и DefaultLanguageKey = "ru" будет ошибка
                //Для этого случая просто берем первое значение. Логика в GetString.

                ////Set default language to russian
                //var matchRu = matches.Cast<Match>().First(match => match.Value.Contains(DefaultLanguageKey));
                //_stringCurrent = s.Substring(matchRu.Index + 4, s.IndexOf(EndPattern, matchRu.Index + 4) - matchRu.Index - 4); 
            }
        }

        /// <summary>
        /// Returns the string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _stringCurrent;
        }

        /// <summary>
        /// ToString for languageCode
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public string ToString(string languageCode)
        {
            return ToString(languageCode, false);
        }

        /// <summary>
        /// ToString for languageCode with getFirstValueIfNotExists option
        /// </summary>
        public string ToString(string languageCode, bool getFirstValueIfNotExists)
        {
            string res;
            if (HaveMultipleLanguages)
                res = GetString(OriginalString, getFirstValueIfNotExists, languageCode);
            else
                res = StringCurrent;

            return res;
        }

        /// <summary>
        /// User-defined conversion from LocalizableString to String 
        /// </summary>
        public static implicit operator String(LocalizableString s)
        {
            return Equals(s, null) ? null : s.ToString();
        }

        /// <summary>
        /// User-defined conversion from String to LocalizableString 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static implicit operator LocalizableString(String s)
        {
            return new LocalizableString(s);
        }

        /// <summary>
        /// operator !=
        /// </summary>
        public static bool operator !=(LocalizableString a, LocalizableString b)
        {
            return String.CompareOrdinal(a, b) != 0;
        }

        /// <summary>
        /// operator ==
        /// </summary>
        public static bool operator ==(LocalizableString a, LocalizableString b)
        {
            return String.CompareOrdinal(a, b) == 0;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            var l = (LocalizableString)obj;
            return (OriginalString == l.OriginalString);

            //if (_stringCurrent == null)
            //    return false;
            //else
            //    return _stringCurrent.Equals(obj);
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return _stringCurrent == null ? 0 : _stringCurrent.GetHashCode();
        }

        /// <summary>
        /// CompareTo
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(string other)
        {

            if (other == null && _stringCurrent == null) return 0;
            if (other == null) return 1;
            if (_stringCurrent == null) return -1;

            return _stringCurrent.CompareTo(other);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(string other)
        {
            return _stringCurrent.Equals(other);
        }

        /// <summary>
        /// Clone
        /// </summary>
        public object Clone()
        {
            if (_stringCurrent == null)
                return null;
            else
                return ((ICloneable)_stringCurrent).Clone();
        }

        //todo:check obj.ToString()
        /// <summary>
        /// CompareTo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if ((obj == null || obj.ToString() == null) && _stringCurrent != null) return 1;
            if ((obj == null && _stringCurrent == null) || (obj.ToString() == null && _stringCurrent == null)) return 0;
            if (_stringCurrent == null) return -1;

            return ((IComparable)_stringCurrent).CompareTo(obj.ToString());
        }





        #region interface delegation
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_stringCurrent).GetEnumerator();
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            return _stringCurrent.GetEnumerator();
        }
        /// <summary>
        /// GetTypeCode
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            if (_stringCurrent == null)
                return TypeCode.Empty;
            return (_stringCurrent).GetTypeCode();
        }

        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToBoolean(provider);
        }

        /// <summary>
        /// ToChar
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToChar(provider);
        }

        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToSByte(provider);
        }

        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToByte(provider);
        }

        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToInt16(provider);
        }

        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToUInt16(provider);
        }

        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToInt32(provider);
        }

        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToUInt32(provider);
        }

        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToInt64(provider);
        }

        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToUInt64(provider);
        }

        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToSingle(provider);
        }

        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToDouble(provider);
        }

        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToDecimal(provider);
        }

        /// <summary>
        /// ToDateTime
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToDateTime(provider);
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            if (_stringCurrent == null)
                return null;
            return ((IConvertible)_stringCurrent).ToString(provider);
        }

        /// <summary>
        /// ToType
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)_stringCurrent).ToType(conversionType, provider);
        }
        #endregion

        /// <summary>
        /// Length
        /// </summary>
        [IgnoreDataMember]
        public int Length => _stringCurrent.Length;

        /// <summary>
        /// Empty
        /// </summary>
        public static LocalizableString Empty => "";

        /// <summary>
        /// OriginalString
        /// </summary>
        [IgnoreDataMember]
        public string OriginalString => _haveMultipleLanguages ? _stringOriginal : _stringCurrent;

        /// <summary>
        /// Текущий ключ языка.
        /// Двухбуквенный, например, ru, en, de.
        /// </summary>
        [IgnoreDataMember]
        public string CurrentLanguageKey
        {
            get
            {
                if (_currentLanguageKey == null)
                    _currentLanguageKey = (CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName;

                return _currentLanguageKey;
            }
            set
            {
                if (!_haveMultipleLanguages)
                {
                    _currentLanguageKey = value;
                    _haveMultipleLanguages = true;

                    _stringOriginal = StartPattern + _currentLanguageKey + StartPattern + _stringCurrent + EndPattern;
                }
                else
                {
                    _currentLanguageKey = value;
                    _stringCurrent = GetString(_stringOriginal);
                }

                _languages = null;
            }
        }

        /// <summary>
        /// StringCurrent
        /// </summary>
        [IgnoreDataMember]
        public string StringCurrent
        {
            get => _stringCurrent;
            set
            {
                if (_stringCurrent != value)
                {
                    if (!_haveMultipleLanguages)
                    {
                        _stringOriginal = _stringCurrent = value;
                    }
                    else
                    {
                        SetString(CurrentLanguageKey, value);
                    }
                }
            }
        }

        /// <summary>
        /// GetString
        /// </summary>
        /// <param name="s"></param>
        /// <param name="getFirstValueIfNotExists"></param>
        /// <param name="codeLanguage"></param>
        /// <returns></returns>
        private string GetString(string s, bool getFirstValueIfNotExists = false, string codeLanguage = null)
        {
            if (codeLanguage == null) codeLanguage = CurrentLanguageKey;

            var matches = Regex.Matches(s, Pattern);

            foreach (var match in matches.Cast<Match>().Where(match => match.Value.Contains(codeLanguage)))
            {
                return s.Substring(match.Index + 4, s.IndexOf(EndPattern, match.Index + 4, StringComparison.Ordinal) - match.Index - 4);
            }

            if (getFirstValueIfNotExists)
            {
                foreach (var match in matches.Cast<Match>().OrderBy(m => (m.Value.Contains(DefaultLanguageKey) ? 1 : 2)))
                {
                    return s.Substring(match.Index + 4, s.IndexOf(EndPattern, match.Index + 4, StringComparison.Ordinal) - match.Index - 4);
                }
            }

            return null;
        }

        /// <summary>
        /// Метод для установки значения в указанном языке.
        /// Строка всегда дополняется штуками вроде "\u0010", даже если в строке присутствует всего один язык.
        /// </summary>
        /// <param name="languageKey"></param>
        /// <param name="text"></param>
        public void SetString(string languageKey, string text)
        {
            var stringCurrentPattern = StartPattern + languageKey + StartPattern + "(.|\n)*?" + EndPattern;
            string replacement = null;
            if (!String.IsNullOrEmpty(text)) replacement = StartPattern + languageKey + StartPattern + text + EndPattern;

            if (Regex.IsMatch(_stringOriginal, stringCurrentPattern))
            {
                if (replacement == null)
                {
                    var matches = Regex.Matches(_stringOriginal, stringCurrentPattern);
                    foreach (Match match in matches)
                    {
                        _stringOriginal = _stringOriginal.Replace(match.Value, String.Empty);
                    }
                }
                else
                {
                    _stringOriginal = Regex.Replace(_stringOriginal, stringCurrentPattern, replacement);
                }
            }
            else
            {
                if (replacement != null)
                {
                    if (_stringOriginal == text)
                        _stringOriginal = replacement;
                    else
                        _stringOriginal += replacement;
                }
            }

            _stringCurrent = GetString(_stringOriginal);
            _haveMultipleLanguages = _stringCurrent != _stringOriginal;
            _languages = null;
        }


        /// <summary>
        /// Метод для установки значения в указанном языке.
        /// Строка всегда дополняется штуками вроде "\u0010", даже если в строке присутствует всего один язык.
        /// </summary>
        /// <param name="text"></param>
        public void SetCurrentString(string text)
        {
            SetString(CurrentLanguageKey, text);
        }

        /// <summary>
        /// Get current string by language key
        /// </summary>
        /// <param name="languageKey"></param>
        public string GetCurrentString(string languageKey)
        {
            if (!String.IsNullOrEmpty(_stringOriginal))
            {
                var matches = Regex.Matches(_stringOriginal, Pattern);

                foreach (var match in matches.Cast<Match>().Where(match => match.Value.Contains(languageKey)))
                {
                    return _stringOriginal.Substring(match.Index + 4, _stringOriginal.IndexOf(EndPattern, match.Index + 4, StringComparison.Ordinal) - match.Index - 4);
                }
            }

            return null;
        }

        /// <summary>
        /// Language codes in current string
        /// </summary>
        /// <returns></returns>
        [IgnoreDataMember]
        public List<String> Languages
        {
            get
            {
                if (_languages == null)
                {
                    if (!String.IsNullOrEmpty(_stringOriginal))
                    {
                        var matches = Regex.Matches(_stringOriginal, Pattern);

                        _languages = matches.Cast<Match>().Select(m => m.Value.Replace(StartPattern, String.Empty)).ToList();
                    }
                    else
                    {
                        _languages = new List<String>();
                    }
                }

                return _languages;
            }
            private set { }
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(string value)
        {
            return _stringCurrent.Contains(value);
        }

        /// <summary>
        /// Replace
        /// </summary>
        /// <param name="oldChar"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public LocalizableString Replace(char oldChar, char newChar)
        {
            return _stringCurrent.Replace(oldChar, newChar);
        }

        /// <summary>
        /// Replace
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public LocalizableString Replace(string oldValue, string newValue)
        {
            return _stringCurrent.Replace(oldValue, newValue);
        }

        /// <summary>
        /// Trim
        /// </summary>
        /// <returns></returns>
        public LocalizableString Trim()
        {
            return _stringCurrent.Trim();
        }

        /// <summary>
        /// ToLower
        /// </summary>
        /// <returns></returns>
        public LocalizableString ToLower()
        {
            return _stringCurrent.ToLower();
        }

        /// <summary>
        /// ToUpper
        /// </summary>
        /// <returns></returns>
        public LocalizableString ToUpper()
        {
            return _stringCurrent.ToUpper();
        }

        /// <summary>
        /// IndexOf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(String value)
        {
            return _stringCurrent.IndexOf(value);
        }

        /// <summary>
        /// IndexOf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(char value)
        {
            return _stringCurrent.IndexOf(value);
        }

        /// <summary>
        /// IndexOf
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(String value, StringComparison comparisonType)
        {
            return _stringCurrent.IndexOf(value, comparisonType);
        }

    }
}