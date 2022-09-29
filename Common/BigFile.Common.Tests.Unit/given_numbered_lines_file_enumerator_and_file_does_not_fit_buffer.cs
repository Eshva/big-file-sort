#region Usings

using System.Text;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Common.Tests.Unit;

public class given_numbered_lines_file_enumerator_and_file_does_not_fit_buffer : NumberedLinesFileEnumeratorTests
{
  [Fact]
  public void when_file_content_does_not_fit_into_buffer_it_should_enumerate_all_lines_in_file()
  {
    const int numberOfLines = 300;
    const string stringPart = "lin";

    var content = GenerateFileWithNumberOfLines(numberOfLines, stringPart);
    using var enumerator = CreateEnumerator(content, MemorySize.FromBytes(bytes: 128));

    var lineCount = 0;
    while (enumerator.MoveNext())
    {
      var line = $"{lineCount:D4}. {stringPart}\r\n";
      enumerator.Current.EntireLine.ToString().Should().Be(line);
      enumerator.Current.LineNumber.ToString().Should().Be($"{lineCount:D4}");
      enumerator.Current.LineString.ToString().Should().Be(stringPart);

      lineCount++;
    }

    lineCount.Should().Be(numberOfLines);
  }

  [Fact]
  public void when_file_content_does_not_fit_into_buffer_and_line_length_factor_of_buffer_length_it_should_enumerate_all_lines_in_file()
  {
    const int numberOfLines = 300;
    const string stringPart = "128b/16b";

    var content = GenerateFileWithNumberOfLines(numberOfLines, stringPart);
    using var enumerator = CreateEnumerator(content, MemorySize.FromBytes(bytes: 128));

    var lineCount = 0;
    while (enumerator.MoveNext())
    {
      var line = $"{lineCount:D4}. {stringPart}\r\n";
      enumerator.Current.EntireLine.ToString().Should().Be(line);
      enumerator.Current.LineNumber.ToString().Should().Be($"{lineCount:D4}");
      enumerator.Current.LineString.ToString().Should().Be(stringPart);

      lineCount++;
    }

    lineCount.Should().Be(numberOfLines);
  }

  private string GenerateFileWithNumberOfLines(int numberOfLines, string stringPart)
  {
    var builder = new StringBuilder();
    for (var lineIndex = 0; lineIndex < numberOfLines; lineIndex++)
    {
      builder.Append($"{lineIndex:D4}. {stringPart}\r\n");
    }

    return builder.ToString();
  }
}
