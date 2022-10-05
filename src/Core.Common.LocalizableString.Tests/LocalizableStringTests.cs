namespace Core.Common.LocalizableString_Tests
{
    public class Tests
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
            Assert.AreEqual(ruText, ls.OriginalString);
            Assert.AreNotEqual(ruText, ls.StringCurrent);
        }

        [Test]
        public void StringWithLanguageInConstructor()
        {
            // Arrange
            var text = $"{LocalizableString.StartPattern}ru{LocalizableString.StartPattern}Текст на русском{LocalizableString.EndPattern}{LocalizableString.StartPattern}en{LocalizableString.StartPattern}Text in English{LocalizableString.EndPattern}";

            // Act
            var ls = new LocalizableString(text);

            // Assert
            Assert.AreEqual(text, ls.OriginalString);
            Assert.That(text, Does.Contain(ls.StringCurrent));
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
            Assert.AreEqual(ruText, ls.GetCurrentString("ru"));
            Assert.AreEqual(enText, ls.GetCurrentString("en"));
        }
    }
}