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
        public void StringWithLanguageInConstructorWithFakeLanguageCodes()
        {
            // Arrange
            var fakeLanguage1 = "rt";
            var fakeLanguage2 = "ls";
            var text = $"{LocalizableString.StartPattern}{fakeLanguage1}{LocalizableString.StartPattern}Текст на русском{LocalizableString.EndPattern}{LocalizableString.StartPattern}{fakeLanguage2}{LocalizableString.StartPattern}Text in English{LocalizableString.EndPattern}";

            // Act
            var ls = new LocalizableString(text);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls, Is.Not.Null);
                Assert.That(ls.StringCurrent, Is.Not.Null);
                Assert.That(ls.CurrentLanguageKey, Is.EqualTo(fakeLanguage1));
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

        [Test]
        public void StringToLocalizableString()
        {
            // Arrange
            var ruText = "Текст на русском";

            // Act
            var ls = ruText.AsLocalizableString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls.OriginalString, Is.EqualTo(ruText));
                Assert.That(ls.StringCurrent, Is.EqualTo(ruText));
            });
        }

        [Test]
        public void DictionaryToLocalizableString()
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
            var ls = dict.AsLocalizableString();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls.GetCurrentString("ru"), Is.EqualTo(ruText));
                Assert.That(ls.GetCurrentString("en"), Is.EqualTo(enText));
            });
        }

        [Test]
        public void StringWithLanguageInConstructorWithTwoSameLanguageCodes()
        {
            // Arrange
            var language1 = "ru";
            var language2 = "en";
            var text = $"{LocalizableString.StartPattern}{language1}{LocalizableString.StartPattern}Текст на русском{LocalizableString.EndPattern}{LocalizableString.StartPattern}{language2}{LocalizableString.StartPattern}Text in English{LocalizableString.EndPattern}{LocalizableString.StartPattern}{language1}{LocalizableString.StartPattern}Снова текст на русском!{LocalizableString.EndPattern}";

            // Act
            var ls = new LocalizableString(text);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ls, Is.Not.Null);
                Assert.That(ls.StringCurrent, Is.Not.Null);
                Assert.That(ls.OriginalString, Is.EqualTo(text));
                Assert.That(text, Does.Contain(ls.StringCurrent));
            });
        }
    }
}