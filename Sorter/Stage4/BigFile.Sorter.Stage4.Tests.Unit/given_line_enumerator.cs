#region Usings

using System;
using System.Collections.Generic;
using BigFile.Sorter.Stage4.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage4.Tests.Unit;

public class given_line_enumerator
{
  [Fact]
  public void when_enumerate_windows_strings_buffer_and_the_last_line_has_new_line_separator_it_should_enumerate_all_lines_in_order()
  {
    EnumerateAsStrings(WindowsBufferWithLastLineWithNewLineSeparator).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 0, Length: 5), new(Start: 5, Length: 5), new(Start: 10, Length: 5) },
      options => options.WithStrictOrdering());
  }

  [Fact]
  public void
    when_enumerate_windows_strings_buffer_and_the_last_line_has_no_new_line_separator_it_should_enumerate_all_lines_except_unfinished()
  {
    EnumerateAsStrings(WindowsBufferWithLastLineWithoutNewLineSeparator).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 0, Length: 5), new(Start: 5, Length: 5) },
      options => options.WithStrictOrdering());
  }

  [Fact]
  public void when_enumerate_linux_strings_buffer_and_the_last_line_has_new_line_separator_it_should_enumerate_all_lines_in_order()
  {
    EnumerateAsStrings(LinuxBufferWithLastLineWithNewLineSeparator).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 0, Length: 4), new(Start: 4, Length: 4), new(Start: 8, Length: 4) },
      options => options.WithStrictOrdering());
  }

  [Fact]
  public void
    when_enumerate_linux_strings_buffer_and_the_last_line_has_no_new_line_separator_it_should_enumerate_all_lines_except_unfinished()
  {
    EnumerateAsStrings(LinuxBufferWithLastLineWithoutNewLineSeparator).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 0, Length: 4), new(Start: 4, Length: 4) },
      options => options.WithStrictOrdering());
  }

  [Fact]
  public void when_enumerate_mac_strings_buffer_and_the_last_line_has_new_line_separator_it_should_enumerate_all_lines_in_order()
  {
    EnumerateAsStrings(MacBufferWithLastLineWithNewLineSeparator).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 0, Length: 4), new(Start: 4, Length: 4), new(Start: 8, Length: 4) },
      options => options.WithStrictOrdering());
  }

  [Fact]
  public void
    when_enumerate_mac_strings_buffer_and_the_last_line_has_no_new_line_separator_it_should_enumerate_all_lines_except_unfinished()
  {
    EnumerateAsStrings(MacBufferWithLastLineWithoutNewLineSeparator).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 0, Length: 4), new(Start: 4, Length: 4) },
      options => options.WithStrictOrdering());
  }

  private IEnumerable<BufferPosition> EnumerateAsStrings(ReadOnlySpan<char> buffer)
  {
    var result = new List<BufferPosition>();
    foreach (var lineEntry in buffer.GetLinesEnumerator())
    {
      result.Add(new BufferPosition(lineEntry.Start, lineEntry.Length));
    }

    return result;
  }

  private const string WindowsBufferWithLastLineWithNewLineSeparator = "aaa\r\nbbb\r\nccc\r\n";
  private const string WindowsBufferWithLastLineWithoutNewLineSeparator = "aaa\r\nbbb\r\nccc";
  private const string LinuxBufferWithLastLineWithNewLineSeparator = "aaa\nbbb\nccc\n";
  private const string LinuxBufferWithLastLineWithoutNewLineSeparator = "aaa\nbbb\nccc";
  private const string MacBufferWithLastLineWithNewLineSeparator = "aaa\rbbb\rccc\r";
  private const string MacBufferWithLastLineWithoutNewLineSeparator = "aaa\rbbb\rccc";
}
