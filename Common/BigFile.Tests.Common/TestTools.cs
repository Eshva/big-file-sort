#region Usings

#endregion

#region Usings

using FluentAssertions;

#endregion

namespace BigFile.Tests.Common;

public static class TestTools
{
  public static void CheckIsFileSorted(IReadOnlyList<string> lines)
  {
    var previousLine = new NumberedLine(lines[index: 0]);
    for (var lineIndex = 1; lineIndex < lines.Count; lineIndex++)
    {
      var currentLine = new NumberedLine(lines[lineIndex]);

      previousLine.CompareTo(currentLine).Should().BeLessOrEqualTo(
        expected: 0,
        $"Lines should be sorted in ascendant order. previous: '{previousLine}' > '{currentLine}'.");
      previousLine = currentLine;
    }
  }

  public static IReadOnlyList<string> ReadAllSignificantLines(string filePath)
  {
    var lines = new List<string>();
    using var reader = new StreamReader(File.OpenRead(filePath));
    while (!reader.EndOfStream)
    {
      var line = reader.ReadLine();
      if (string.IsNullOrWhiteSpace(line)) continue;
      lines.Add(line);
    }

    return lines.AsReadOnly();
  }
}
