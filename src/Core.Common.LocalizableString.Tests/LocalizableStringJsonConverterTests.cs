using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Core.Common.Converters.Json;

namespace Core.Common.LocalizableString_Tests
{
    /// <summary>
    /// Tests for <see cref="LocalizableStringJsonConverter"/>.
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
            var ruText = "Текст на Русском";
            var enText = "Text in English";
            var esText = "Texto en Español";
            var dict = new Dictionary<string, string>
            {
                { "ru", ruText },
                { "en", enText },
                { "es", esText }
            };

            // Act
            var ls = new LocalizableString(dict);
            var obj = new TestClass
            {
                Name = "name",
                Description = ls
            };
            var json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
            Console.WriteLine(json);
            var afterJson = JsonSerializer.Deserialize<TestClass>(json, _jsonSerializerOptions);
            var description = afterJson.Description;
            var afterJsonRuText = description.GetCurrentString("ru");
            var afterJsonEnText = description.GetCurrentString("en");
            var afterJsonEsText = description.GetCurrentString("es");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(afterJsonRuText, Is.EqualTo(ruText));
                Assert.That(afterJsonEnText, Is.EqualTo(enText));
                Assert.That(afterJsonEsText, Is.EqualTo(esText));
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
            var ruText = "Текст на Русском";
            var enText = $"Text in {LocalizableString.StartPattern}English";
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
                var obj = new TestClass
                {
                    Name = "name12345",
                    Description = ls
                };

                var json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
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