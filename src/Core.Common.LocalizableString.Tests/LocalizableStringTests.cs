namespace Core.Common.LocalizableString_Tests
{
    /// <summary>
    /// Tests for <see cref="LocalizableString"/>.
    /// </summary>
    public class LocalizableStringTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void StringInConstructor()
        {
            // Arrange
            var ruText = "Текст на русском";

            // Act
            var ls = new LocalizableString(ruText);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls.OriginalString, Is.EqualTo(ruText));
                Assert.That(ls.StringCurrent, Is.EqualTo(ruText));
            });
        }

        [Test]
        public void StringWithLanguageInConstructor()
        {
            // Arrange
            var text = $"{LocalizableString.StartPattern}ru{LocalizableString.StartPattern}Текст на русском{LocalizableString.EndPattern}{LocalizableString.StartPattern}en{LocalizableString.StartPattern}Text in English{LocalizableString.EndPattern}";

            // Act
            var ls = new LocalizableString(text);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls.OriginalString, Is.EqualTo(text));
                Assert.That(text, Does.Contain(ls.StringCurrent));
            });
        }

        [Test]
        public void DictionaryInConstructor()
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

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls.GetCurrentString("ru"), Is.EqualTo(ruText));
                Assert.That(ls.GetCurrentString("en"), Is.EqualTo(enText));
            });
        }
    }
}