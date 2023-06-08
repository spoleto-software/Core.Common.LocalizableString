using System;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Common.Converters.Json
{
    /// <summary>
    /// LocalizableStringJsonConverter based on System.Text.Json. 
    /// </summary>
    public class LocalizableStringJsonConverter : JsonConverter<LocalizableString>
    {
        /// <summary>
        /// Reads LocalizableString from JSON.
        /// </summary>
        public override LocalizableString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    {
                        var dict = new OrderedDictionary();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                            {
                                break;
                            }

                            if (reader.TokenType != JsonTokenType.PropertyName)
                            {
                                throw new JsonException("JsonTokenType was not PropertyName");
                            }

                            var propertyName = reader.GetString();

                            if (propertyName == null)
                            {
                                throw new JsonException("Failed to get property name");
                            }

                            reader.Read();

                            dict.Add(propertyName, reader.GetString());
                        }

                        var ls = dict.AsLocalizableString();

                        return ls;
                    }

                case JsonTokenType.String:
                    {
                        var str = reader.GetString();
                        var ls = str.AsLocalizableString();

                        return ls;
                    }

                default:
                    throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects and string are supported.");
            }
        }

        /// <summary>
        /// Writes LocalizableString to JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, LocalizableString value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var jsonCompatible = value.AsJsonCompatible();
            switch (jsonCompatible)
            {
                case OrderedDictionary dict:
                    JsonSerializer.Serialize(writer, dict, options);
                    break;

                case string str:
                    writer.WriteStringValue(str);
                    break;

                default:
                    throw new NotSupportedException($"LocalizableString.AsJsonCompatible returned an unexpected type: {jsonCompatible?.GetType().FullName}.");
            }
        }
    }
}
