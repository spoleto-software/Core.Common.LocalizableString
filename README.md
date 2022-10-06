# Core.Common.LocalizableString
Functionality to store multilingual values in String. 
The main object is `LocalizableString`.
This object has corresponding JSON converters based on System.Text.Json.
Also there are available extensions to convert ``String``, ``Dictionary`` to ``LocalizableString``.

This library helps make your application multilingual!

The example:
```
var rawString = "\u0010ru\u0010текст на русском\u0011\u0010en\u0010text in english\u0011\u0010il\u0010טקסט בעברית\u0011";
var multiString = new LocalizableString(rawString);
// OR
var multiString = (LocalizableString) rawString;

var ruString = multiString.GetCurrentString("ru"); // gets Russian translation
var enString = multiString.GetCurrentString("en"); // gets English translation
```

## More examples
You can find more examples in the unit tests:

https://github.com/spoleto-software/Core.Common.LocalizableString/tree/main/src/Core.Common.LocalizableString.Tests