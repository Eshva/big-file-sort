#region Usings

using System;
using BigFile.Sorter.Stage1.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage1.Tests.Unit;

public class given_numbered_line
{
  [Fact]
  public void when_compare_one_instance_it_should_produce_expected_result()
  {
    new NumberedLine("1. A this one is less").CompareTo(new NumberedLine("1. B this one is greater")).Should().BeNegative();
    new NumberedLine("1. B this one is greater").CompareTo(new NumberedLine("1. A this one is less")).Should().BePositive();
    new NumberedLine("1. Equal").CompareTo(new NumberedLine("1. Equal")).Should().Be(expected: 0);
    new NumberedLine("1. NOT EQUAL").CompareTo(new NumberedLine("1. not equal")).Should().NotBe(unexpected: 0);

    new NumberedLine("1. look on numbers").CompareTo(new NumberedLine("2. look on numbers")).Should().BeNegative();
    new NumberedLine("2. look on numbers").CompareTo(new NumberedLine("1. look on numbers")).Should().BePositive();
  }

  [Fact]
  public void when_construct_with_not_properly_formatted_string_it_should_complain()
  {
    var sut = () => new NumberedLine("string");
    sut.Should().ThrowExactly<ArgumentException>()
      .WithParameterName("rawLine")
      .And.Message.ToLower().Should().Contain("dot");

    sut = () => new NumberedLine("99p. string");
    sut.Should().ThrowExactly<ArgumentException>()
      .WithParameterName("rawLine")
      .And.Message.ToLower().Should().Contain("number");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var sut = () => new NumberedLine(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("rawLine");

    sut = () => new NumberedLine(string.Empty);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("rawLine");

    sut = () => new NumberedLine(" \t\n");
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("rawLine");
  }
}
