namespace BigFile.Tests.Common;

public class NumberedLine : IComparable<NumberedLine>
{
  public NumberedLine(string rawLine)
  {
    if (string.IsNullOrWhiteSpace(rawLine)) throw new ArgumentNullException(nameof(rawLine));

    (_number, _string) = Parse(rawLine);
  }

  public int CompareTo(NumberedLine? other)
  {
    if (other == null) return 1;

    var stringsCompareResult = string.Compare(
      _string,
      other._string,
      StringComparison.Ordinal);
    return stringsCompareResult != 0 ? stringsCompareResult : _number.CompareTo(other._number);
  }

  public override string ToString() => $"{_number:D}. {_string}";

  private static (int, string) Parse(string rawLine)
  {
    var dotIndex = rawLine.IndexOf(value: '.');
    if (dotIndex == -1) throw new ArgumentException("Invalid line format. Dot is missing.", nameof(rawLine));

    if (!int.TryParse(rawLine[..dotIndex], out var number))
      throw new ArgumentException("Invalid line format. Can't parse number.", nameof(rawLine));

    return (number, rawLine[(dotIndex + 2)..]);
  }

  private readonly int _number;
  private readonly string _string;
}
