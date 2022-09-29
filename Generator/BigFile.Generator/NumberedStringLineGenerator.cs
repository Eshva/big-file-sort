#region Usings

using RandomStringCreator;

#endregion

namespace BigFile.Generator;

internal class NumberedStringLineGenerator : ILineGenerator
{
  public NumberedStringLineGenerator(
    int minNumber = 0,
    int maxNumber = 0,
    string validStringCharacters = "a",
    int minStringLength = 1,
    int maxStringLength = 1,
    int predefinedStringCount = 100)
  {
    if (minNumber < 0) throw new ArgumentOutOfRangeException(nameof(minNumber));
    if (maxNumber < 0 || maxNumber < minNumber) throw new ArgumentOutOfRangeException(nameof(maxNumber));
    if (string.IsNullOrWhiteSpace(validStringCharacters))
      throw new ArgumentNullException(nameof(validStringCharacters));
    if (minStringLength <= 0) throw new ArgumentOutOfRangeException(nameof(minStringLength));
    if (maxStringLength <= 0 || maxStringLength < minStringLength) throw new ArgumentOutOfRangeException(nameof(maxStringLength));
    if (predefinedStringCount <= 0) throw new ArgumentOutOfRangeException(nameof(predefinedStringCount));

    _minNumber = minNumber;
    _maxNumber = maxNumber;
    _predefinedStringCount = predefinedStringCount;
    _predefinedStrings = new string[_predefinedStringCount];

    StringCreator randomString = new(validStringCharacters);
    for (var stringIndex = 0; stringIndex < _predefinedStringCount; stringIndex++)
    {
      _predefinedStrings[stringIndex] = randomString.Get(_randomInteger.Next(minStringLength, maxStringLength + 1));
    }
  }

  public string Generate() => $"{GetNumber()}. {GetString()}";

  private string GetString() => _predefinedStrings[_randomInteger.Next(minValue: 0, _predefinedStringCount)];

  private string GetNumber() => _randomInteger.NextInt64(_minNumber, _maxNumber + 1).ToString("D");

  private readonly int _maxNumber;
  private readonly int _minNumber;
  private readonly int _predefinedStringCount;
  private readonly string[] _predefinedStrings;
  private readonly Random _randomInteger = new();
}
