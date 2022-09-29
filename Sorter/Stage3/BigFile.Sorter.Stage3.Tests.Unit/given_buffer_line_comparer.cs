#region Usings

using System;
using BigFile.Sorter.Stage3.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage3.Tests.Unit;

public class given_buffer_line_comparer
{
  public given_buffer_line_comparer()
  {
    _comparer = new BufferLineComparer();
  }

  [Fact]
  public void when_compare_properly_formatted_numbered_lines_it_should_return_expected_result()
  {
    _comparer.Compare("123. aaa", "123. aaa").Should().Be(expected: 0);
    _comparer.Compare("123. aaa", "123. baa").Should().BeLessThan(expected: 0);
    _comparer.Compare("123. aaa", "124. aaa").Should().BeLessThan(expected: 0);
    _comparer.Compare("123. baa", "123. aaa").Should().BeGreaterThan(expected: 0);
    _comparer.Compare("124. aaa", "123. aaa").Should().BeGreaterThan(expected: 0);
  }

  [Fact]
  public void when_compare_not_properly_formatted_string_it_should_complain()
  {
    var sut = () => _comparer.Compare(ProperNumberedLine, NoDotInLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("right")
      .And.Message.ToLower().Should().Contain("no dot");
    sut = () => _comparer.Compare(NoDotInLine, ProperNumberedLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("left")
      .And.Message.ToLower().Should().Contain("no dot");

    sut = () => _comparer.Compare(ProperNumberedLine, NoSpaceAfterNumberLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("right")
      .And.Message.ToLower().Should().Contain("no space after dot");
    sut = () => _comparer.Compare(NoSpaceAfterNumberLine, ProperNumberedLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("left")
      .And.Message.ToLower().Should().Contain("no space after dot");

    sut = () => _comparer.Compare(ProperNumberedLine, NoNumberInLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("right")
      .And.Message.ToLower().Should().Contain("no number");
    sut = () => _comparer.Compare(NoNumberInLine, ProperNumberedLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("left")
      .And.Message.ToLower().Should().Contain("no number");

    sut = () => _comparer.Compare(ProperNumberedLine, NoStringPartInLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("right")
      .And.Message.ToLower().Should().Contain("no string part");
    sut = () => _comparer.Compare(NoStringPartInLine, ProperNumberedLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("left")
      .And.Message.ToLower().Should().Contain("no string part");

    sut = () => _comparer.Compare(ProperNumberedLine, NotPrefixedWithNumberLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("right")
      .And.Message.ToLower().Should().Contain("not prefixed with number");
    sut = () => _comparer.Compare(NotPrefixedWithNumberLine, ProperNumberedLine);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("left")
      .And.Message.ToLower().Should().Contain("not prefixed with number");
  }

  private readonly BufferLineComparer _comparer;
  private const string ProperNumberedLine = "111. String";
  private const string NoDotInLine = "No dot in line";
  private const string NoSpaceAfterNumberLine = "111.No space after number";
  private const string NoNumberInLine = ". String";
  private const string NotPrefixedWithNumberLine = "a. String";
  private const string NoStringPartInLine = "111. ";
}
