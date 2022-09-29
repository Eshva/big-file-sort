#region Usings

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Common.Tests.Unit;

[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
public class given_numbered_lines_file_enumerator_for_wrong_cases : NumberedLinesFileEnumeratorTests
{
  [Fact]
  public void when_move_next_and_single_line_has_no_number_part_it_should_complain()
  {
    const string content = ". ABC\r";
    using var enumerator = CreateEnumerator(content);

    var sut = () => enumerator.MoveNext();
    sut.Should().ThrowExactly<Exception>().Which.Message.ToLower().Should().Contain("number part of a line isn't an integer.");
  }

  [Fact]
  public void when_move_next_and_some_line_has_no_proper_number_and_string_parts_separator_it_should_complain()
  {
    const string content = "11.ABC\r\n";
    using var enumerator = CreateEnumerator(content);

    var sut = () => enumerator.MoveNext();
    sut.Should().ThrowExactly<Exception>().Which.Message.ToLower().Should().Contain("number and string parts separator");
  }

  [Fact]
  public void when_move_next_and_some_line_has_no_string_part_it_should_complain()
  {
    const string content = "11. \r\n";
    using var enumerator = CreateEnumerator(content);

    var sut = () => enumerator.MoveNext();
    sut.Should().ThrowExactly<Exception>().Which.Message.ToLower().Should().Contain("string part of a line is not found");
  }

  [Fact]
  public void when_move_next_and_some_line_number_part_is_not_a_valid_integer_it_should_complain()
  {
    const string content = "RR. ABC\r\n";
    using var enumerator = CreateEnumerator(content);

    var sut = () => enumerator.MoveNext();
    sut.Should().ThrowExactly<Exception>().Which.Message.ToLower().Should().Contain("isn't an integer");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var existingFile = new FileInfo(Path.GetTempFileName());
    existingFile.CreateText().Dispose();
    var validBufferSize = MemorySize.FromKilobytes(kilobytes: 4);

    try
    {
      var sut = () => new NumberedLinesFileEnumerator(null!, validBufferSize);
      sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("file");

      sut = () => new NumberedLinesFileEnumerator(new FileInfo("not existing file"), validBufferSize);
      sut.Should().ThrowExactly<ArgumentException>().WithParameterName("file")
        .And.Message.ToLower().Should().Contain("doesn't exist");

      sut = () => new NumberedLinesFileEnumerator(existingFile, MemorySize.FromBytes(bytes: 64));
      sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("bufferSize")
        .And.Message.ToLower().Should().Contain("at least 128 bytes");

      sut = () => new NumberedLinesFileEnumerator(existingFile, MemorySize.FromBytes(bytes: 320));
      sut.Should().ThrowExactly<ArgumentException>().WithParameterName("bufferSize")
        .And.Message.ToLower().Should().Contain("power of 2");
    }
    finally
    {
      existingFile.Delete();
    }
  }
}
