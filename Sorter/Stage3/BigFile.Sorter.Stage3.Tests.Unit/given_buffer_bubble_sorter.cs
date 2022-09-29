#region Usings

using System;
using BigFile.Sorter.Stage3.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage3.Tests.Unit;

public class given_buffer_bubble_sorter
{
  public given_buffer_bubble_sorter()
  {
    _sorter = new BufferBubbleSorter(new StubStringComparer());
  }

  [Fact]
  public void when_sort_it_should_return_sorted_list_of_string_positions_in_buffer()
  {
    _sorter.Sort(WindowsString).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 10, Length: 5), new(Start: 5, Length: 5), new(Start: 0, Length: 5) },
      options => options.WithStrictOrdering());
    _sorter.Sort(LinuxString).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 8, Length: 4), new(Start: 4, Length: 4), new(Start: 0, Length: 4) },
      options => options.WithStrictOrdering());
    _sorter.Sort(MacString).Should().BeEquivalentTo(
      new BufferPosition[] { new(Start: 8, Length: 4), new(Start: 4, Length: 4), new(Start: 0, Length: 4) },
      options => options.WithStrictOrdering());
  }

  private readonly BufferBubbleSorter _sorter;

  private const string WindowsString = "aaa\r\nAAA\r\n000\r\n";
  private const string LinuxString = "aaa\nAAA\n000\n";
  private const string MacString = "aaa\rAAA\r000\r";

  private class StubStringComparer : IStringSpanComparer
  {
    public int Compare(ReadOnlySpan<char> left, ReadOnlySpan<char> right) => left.CompareTo(right, StringComparison.Ordinal);
  }
}
