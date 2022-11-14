﻿using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Core.Common.Converters.Json;

namespace Core.Common.LocalizableString_Tests
{
    /// <summary>
    /// Tests for <see cref="LocalizableString"/>.
    /// </summary>
    public class LocalizableStringJsonConverterTests
    {
        private static readonly JavaScriptEncoder _encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

        private JsonSerializerOptions _jsonSerializerOptions;

        [SetUp]
        public void Setup()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = _encoder
            };
            _jsonSerializerOptions.Converters.Add(new LocalizableStringJsonConverter());
        }

        [Test]
        public void DeserializeCorrectLocalizableString()
        {
            // Arrange
            var ruText = "Текст на русском";
            var enText = "Text in English";
            var dict = new Dictionary<string, string>
            {
                { "ru", ruText },
                { "en", enText }
            };

            // Act
            var ls = new  LocalizableString(dict);
            var json = JsonSerializer.Serialize(ls, _jsonSerializerOptions);
            var afterJson = JsonSerializer.Deserialize<LocalizableString>(json, _jsonSerializerOptions);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(afterJson, Is.Not.Null);
                Assert.That(afterJson.GetCurrentString("ru"), Is.EqualTo(ruText));
                Assert.That(afterJson.GetCurrentString("en"), Is.EqualTo(enText));
            });
        }

        [Test]
        public void DeserializeObjectWithCorrectLocalizableString()
        {
            // Arrange
            var s = "[" +
                     "{\"Name\": \"test1\", \"Description\": \"_description1\"}, " +
                     "{\"Name\": \"test2\", \"Description\": {\"ru\": \"50% этиленвинилацетат , 31% резина, 19% Карбонат кальция\",  \"en\": \"50% ethylene-vinyl acetate, 31% rubber, 19% онат кальция\"}}" +
                    "]";

            // Act
            var objList = JsonSerializer.Deserialize<List<TestClass>>(s, _jsonSerializerOptions);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(objList, Has.Count.EqualTo(2));
            });
        }

        /// <summary>
        /// Excess special characters.
        /// </summary>
        [Test]
        public void DeserializeWrongLocalizableString()
        {
            // Arrange
            var ruText = "Текст на русском";
            var enText = "Text in \u0010English";
            var dict = new Dictionary<string, string>
            {
                { "ru", ruText },
                { "en", enText }
            };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                var ls = dict.AsLocalizableString();

                var json = JsonSerializer.Serialize(ls, _jsonSerializerOptions);
            });
        }

        /// <summary>
        /// Excess special characters.
        /// </summary>
        [Test]
        public void DeserializeObjectWithWrongLocalizableString()
        {
            // Arrange
            var s = "[" +
                     "{\"Name\": \"test1\", \"Description\": \"_description1\"}, " +
                     "{\"Name\": \"test2\", \"Description\": {\"ru\": \"50% этиленвинилацетат , 31% резина, 19% Карбонат кальция\",  \"en\": \"50% ethylene-vinyl acetate, 31% rubber, 19% онат кальция\u0010ru\u0010Карбонат кальция\"}}" +
                    "]";

            // Assert
            Assert.Throws<JsonException>(() =>
            {
                // Act
                var objList = JsonSerializer.Deserialize<List<TestClass>>(s, _jsonSerializerOptions);
            });
        }
    }
}