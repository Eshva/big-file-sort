#region Usings

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Common.Tests.Unit;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class given_numbered_lines_file_enumerator_and_file_fits_buffer : NumberedLinesFileEnumeratorTests
{
  [Fact]
  public void when_single_line_file_with_windows_line_endings_enumerated_it_should_find_the_line_and_its_parts()
  {
    const string content = "1. ABC\r\n";
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(content);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_single_line_file_with_linux_line_endings_enumerated_it_should_find_the_line_and_its_parts()
  {
    const string content = "1. ABC\n";
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(content);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_single_line_file_with_mac_line_endings_enumerated_it_should_find_the_line_and_its_parts()
  {
    const string content = "1. ABC\r";
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(content);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_single_line_file_with_reversed_windows_line_endings_enumerated_it_should_find_the_line_and_its_parts()
  {
    const string content = "1. ABC\n\r";
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(content);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_multi_line_file_with_windows_line_endings_enumerated_it_should_find_all_lines_and_their_parts()
  {
    const string line1 = "1. ABC\r\n";
    const string line2 = "22. DEFGH\r\n";
    const string content = line1 + line2;
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line1);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line2);
    enumerator.Current.LineNumber.ToString().Should().Be("22");
    enumerator.Current.LineString.ToString().Should().Be("DEFGH");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_multi_line_file_with_linux_line_endings_enumerated_it_should_find_all_lines_and_their_parts()
  {
    const string line1 = "1. ABC\n";
    const string line2 = "22. DEFGH\n";
    const string content = line1 + line2;
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line1);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line2);
    enumerator.Current.LineNumber.ToString().Should().Be("22");
    enumerator.Current.LineString.ToString().Should().Be("DEFGH");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_multi_line_file_with_mac_line_endings_enumerated_it_should_find_all_lines_and_their_parts()
  {
    const string line1 = "1. ABC\r";
    const string line2 = "22. DEFGH\r";
    const string content = line1 + line2;
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line1);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line2);
    enumerator.Current.LineNumber.ToString().Should().Be("22");
    enumerator.Current.LineString.ToString().Should().Be("DEFGH");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_multi_line_file_with_reversed_windows_line_endings_enumerated_it_should_find_all_lines_and_their_parts()
  {
    const string line1 = "1. ABC\n\r";
    const string line2 = "22. DEFGH\n\r";
    const string content = line1 + line2;
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line1);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line2);
    enumerator.Current.LineNumber.ToString().Should().Be("22");
    enumerator.Current.LineString.ToString().Should().Be("DEFGH");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_multi_line_file_with_mixed_line_endings_enumerated_it_should_find_all_lines_and_their_parts()
  {
    const string line1 = "1. ABC\n";
    const string line2 = "22. DEFGH\n\r";
    const string content = line1 + line2;
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line1);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line2);
    enumerator.Current.LineNumber.ToString().Should().Be("22");
    enumerator.Current.LineString.ToString().Should().Be("DEFGH");

    enumerator.MoveNext().Should().BeFalse();
  }

  [Fact]
  public void when_multi_line_file_with_last_line_without_line_ending_enumerated_it_should_not_find_the_last_line()
  {
    const string line1 = "1. ABC\n";
    const string line2 = "22. DEFGH";
    const string content = line1 + line2;
    using var enumerator = CreateEnumerator(content);

    enumerator.MoveNext().Should().BeTrue();

    enumerator.Current.EntireLine.ToString().Should().Be(line1);
    enumerator.Current.LineNumber.ToString().Should().Be("1");
    enumerator.Current.LineString.ToString().Should().Be("ABC");

    enumerator.MoveNext().Should().BeFalse();
  }
}
