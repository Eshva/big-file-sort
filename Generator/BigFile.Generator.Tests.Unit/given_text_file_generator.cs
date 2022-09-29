#region Usings

using System;
using System.IO;
using BigFile.Tests.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Generator.Tests.Unit;

public class given_text_file_generator
{
  public given_text_file_generator()
  {
    _generator = new TextFileGenerator(new SimilarLineGenerator());
  }

  [Fact]
  public void when_generate_file_it_should_create_file_with_expected_content()
  {
    var file = new FileInfo(Path.GetTempFileName());
    const int lineCount = 20;
    _generator.GenerateFile(file, lineCount);

    try
    {
      file.Exists.Should().BeTrue();
      var readLineCount = 0;
      using var reader = new StreamReader(file.OpenRead());
      while (!reader.EndOfStream)
      {
        reader.ReadLine().Should().Be(SimilarLineGenerator.ExpectedString);
        readLineCount++;
      }

      readLineCount.Should().Be(lineCount);
    }
    finally
    {
      file.Delete();
    }
  }

  [Fact]
  public void when_generate_file_and_file_already_exists_it_should_replace_existing_content_with_new_one_and_do_not_combine_them()
  {
    var file = new FileInfo(Path.GetTempFileName());
    var lineCount = 2000;
    _generator.GenerateFile(file, lineCount);

    try
    {
      var lines = TestTools.ReadAllSignificantLines(file.FullName);
      lines.Should().HaveCount(lineCount);

      lineCount = 100;
      _generator.GenerateFile(file, lineCount);
      lines = TestTools.ReadAllSignificantLines(file.FullName);
      lines.Should().HaveCount(lineCount);
    }
    finally
    {
      file.Delete();
    }
  }

  [Fact]
  public void when_generate_file_with_invalid_arguments_it_should_complain()
  {
    var validFile = new FileInfo(Path.GetTempFileName());

    var sut = () => _generator.GenerateFile(null!, lineCount: 20);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("file");

    sut = () => _generator.GenerateFile(validFile, lineCount: 0);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("lineCount");

    sut = () => _generator.GenerateFile(validFile, lineCount: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("lineCount");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var sut = () => new TextFileGenerator(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("lineGenerator");
  }

  private readonly TextFileGenerator _generator;
}
