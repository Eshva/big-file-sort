#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Generator.Tests.Unit;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class given_numbered_string_line_generator_when_generate_line
{
  [Fact]
  public void when_generate_line_it_should_produce_string_with_expected_format()
  {
    var sut = new NumberedStringLineGenerator(
      minNumber: 1,
      maxNumber: 1,
      "a",
      minStringLength: 10,
      maxStringLength: 10,
      predefinedStringCount: 1);
    var line = sut.Generate();
    line.Should().Be("1. aaaaaaaaaa");
  }

  [Fact]
  public void when_generate_line_it_should_produce_strings_with_expected_diversity()
  {
    var sut = new NumberedStringLineGenerator(
      minNumber: 1,
      maxNumber: 1,
      "a",
      minStringLength: 1,
      maxStringLength: 2,
      predefinedStringCount: 10);

    var uniqueLines = new HashSet<string>();
    for (var turn = 0; turn < 10; turn++)
    {
      var line = sut.Generate();
      if (uniqueLines.Contains(line)) continue;
      uniqueLines.Add(line);
    }

    uniqueLines.Should().HaveCount(expected: 2);
  }

  [Fact]
  public void when_construct_with_valid_arguments_it_should_produce_instance()
  {
    var sut = () => new NumberedStringLineGenerator(
      minNumber: 0,
      maxNumber: 10000,
      "abcdefghijklmnopqrstuvwxyz",
      minStringLength: 20,
      maxStringLength: 100);

    sut.Should().NotThrow();
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var sut = () => new NumberedStringLineGenerator(minNumber: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("minNumber");

    sut = () => new NumberedStringLineGenerator(maxNumber: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxNumber");

    sut = () => new NumberedStringLineGenerator(minNumber: 10, maxNumber: 9);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxNumber");

    sut = () => new NumberedStringLineGenerator(validStringCharacters: null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("validStringCharacters");

    sut = () => new NumberedStringLineGenerator(validStringCharacters: string.Empty);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("validStringCharacters");

    sut = () => new NumberedStringLineGenerator(validStringCharacters: " \t\n");
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("validStringCharacters");

    sut = () => new NumberedStringLineGenerator(minStringLength: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("minStringLength");

    sut = () => new NumberedStringLineGenerator(minStringLength: 0);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("minStringLength");

    sut = () => new NumberedStringLineGenerator(maxStringLength: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxStringLength");

    sut = () => new NumberedStringLineGenerator(maxStringLength: 0);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxStringLength");

    sut = () => new NumberedStringLineGenerator(minStringLength: 10, maxStringLength: 9);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxStringLength");

    sut = () => new NumberedStringLineGenerator(predefinedStringCount: 0);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("predefinedStringCount");
  }
}
