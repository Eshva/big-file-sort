#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using BigFile.Sorter.Stage3.Application;
using BigFile.Tests.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage3.Tests.Unit;

public class given_parts_builder3 : IClassFixture<IndependentFilesGeneratorFixture>, IDisposable
{
  public given_parts_builder3(IndependentFilesGeneratorFixture filesGeneratorFixture)
  {
    _filesGeneratorFixture = filesGeneratorFixture;
    _validWorkingFolder = new DirectoryInfo(Path.GetTempPath());
    _validInputFile = new FileInfo(Path.GetTempFileName());
    _validInputFile.OpenWrite().Dispose();
    _validBufferSize = 32 * 4096;
    _validMaxTempFiles = 10;
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_exceeding_number_of_lines_it_should_produce_maximum_files_with_expected_content()
  {
    const int bufferSize = 4096;
    const int maxTempFiles = 10;
    const int lineLengthInChars = 32;
    var stringLength = lineLengthInChars - 1 - 2 - Environment.NewLine.Length;
    _filesGeneratorFixture.SetLinesGeneratingParameters(
      maxNumber: 9,
      minStringLength: stringLength,
      maxStringLength: stringLength);

    const int lineCount = bufferSize / lineLengthInChars * maxTempFiles + 1;
    var sourceFile = _filesGeneratorFixture.GenerateFile(lineCount);
    var partsBuilder = new PartsBuilder3(
      _filesGeneratorFixture.Folder,
      sourceFile,
      bufferSize,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      maxTempFiles,
      sourceFile);
  }

  [Fact]
  public void
    when_build_sorted_parts_and_source_file_has_exceeding_number_of_variable_length_lines_it_should_produce_maximum_files_with_expected_content()
  {
    const int bufferSize = 4096;
    const int maxTempFiles = 10;
    var lineLengthInChars = 2 + 2 + Environment.NewLine.Length;
    _filesGeneratorFixture.SetLinesGeneratingParameters(
      maxNumber: 9,
      minStringLength: lineLengthInChars,
      maxStringLength: lineLengthInChars * 2);

    var lineCount = bufferSize / lineLengthInChars * maxTempFiles * 2;
    var sourceFile = _filesGeneratorFixture.GenerateFile(lineCount);
    var partsBuilder = new PartsBuilder3(
      _filesGeneratorFixture.Folder,
      sourceFile,
      bufferSize,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      maxTempFiles,
      sourceFile);
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_smaller_number_of_lines_it_should_produce_required_files_with_expected_content()
  {
    const int bufferSize = 4096;
    const int maxTempFiles = 10;
    const int lineLengthInChars = 32;
    var stringLength = lineLengthInChars - 1 - 2 - Environment.NewLine.Length;
    _filesGeneratorFixture.SetLinesGeneratingParameters(
      maxNumber: 9,
      minStringLength: stringLength,
      maxStringLength: stringLength);

    var expectedNumberOfFiles = maxTempFiles - 1;
    var lineCount = bufferSize / lineLengthInChars * expectedNumberOfFiles;
    var sourceFile = _filesGeneratorFixture.GenerateFile(lineCount);
    var partsBuilder = new PartsBuilder3(
      _filesGeneratorFixture.Folder,
      sourceFile,
      bufferSize,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      expectedNumberOfFiles,
      sourceFile);
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_lines_only_for_one_part_it_should_produce_only_one_file_with_expected_content()
  {
    const int bufferSize = 4096;
    const int maxTempFiles = 10;
    const int lineLengthInChars = 32;
    var stringLength = lineLengthInChars - 1 - 2 - Environment.NewLine.Length;
    _filesGeneratorFixture.SetLinesGeneratingParameters(
      maxNumber: 9,
      minStringLength: stringLength,
      maxStringLength: stringLength);

    const int expectedNumberOfFiles = 1;
    var lineCount = bufferSize / lineLengthInChars * expectedNumberOfFiles;
    var sourceFile = _filesGeneratorFixture.GenerateFile(lineCount);
    var partsBuilder = new PartsBuilder3(
      _filesGeneratorFixture.Folder,
      sourceFile,
      bufferSize,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      expectedNumberOfFiles,
      sourceFile);
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_no_lines_it_should_produce_empty_file()
  {
    const int bufferSize = 4096;
    const int maxTempFiles = 10;
    var sourceFile = new FileInfo(Path.Combine(_filesGeneratorFixture.Folder.FullName, "empty"));
    sourceFile.CreateText().Dispose();
    var partsBuilder = new PartsBuilder3(
      _filesGeneratorFixture.Folder,
      sourceFile,
      bufferSize,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      expectedNumberOfFiles: 0,
      sourceFile);
  }

  [Fact]
  public void when_build_sorted_parts_with_invalid_arguments_it_should_complain()
  {
    var partsBuilder = new PartsBuilder3(
      _validWorkingFolder,
      _validInputFile,
      _validBufferSize,
      _validMaxTempFiles);

    var sut = () => partsBuilder.BuildSortedParts(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("inputFileReader");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var sut = () => new PartsBuilder3(
      null!,
      _validInputFile,
      _validBufferSize,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("workingFolder");

    sut = () => new PartsBuilder3(
      new DirectoryInfo("folder doesn't exist"),
      _validInputFile,
      _validBufferSize,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("workingFolder")
      .And.Message.ToLower().Should().Contain("doesn't exist");

    sut = () => new PartsBuilder3(
      _validWorkingFolder,
      null!,
      _validBufferSize,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("inputFile");

    sut = () => new PartsBuilder3(
      _validWorkingFolder,
      new FileInfo("file doesn't exist"),
      _validBufferSize,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("inputFile")
      .And.Message.ToLower().Should().Contain("doesn't exist");

    sut = () => new PartsBuilder3(
      _validWorkingFolder,
      _validInputFile,
      bufferSize: 0,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("bufferSize")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new PartsBuilder3(
      _validWorkingFolder,
      _validInputFile,
      _validBufferSize,
      maxTempFiles: 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("at least 2");
  }

  public void Dispose()
  {
    if (_validInputFile.Exists) _validInputFile.Delete();
  }

  private static void CheckPartFilesCorrectness(
    List<FileInfo> partFiles,
    int expectedNumberOfFiles,
    FileInfo file)
  {
    partFiles.Should().HaveCount(expectedNumberOfFiles);

    var sourceFileReader = new StreamReader(file.OpenRead());
    try
    {
      foreach (var partFile in partFiles)
      {
        var sourceLines = new List<string>();
        var partLines = new List<string>();

        using var partFileReader = new StreamReader(partFile.OpenRead());
        while (!partFileReader.EndOfStream)
        {
          var sourceLine = sourceFileReader.ReadLine();
          if (sourceLine is not null) sourceLines.Add(sourceLine);

          var partLine = partFileReader.ReadLine();
          if (partLine is not null) partLines.Add(partLine);
        }

        partLines.Should().BeEquivalentTo(sourceLines);
        TestTools.CheckIsFileSorted(partLines);
      }
    }
    finally
    {
      sourceFileReader.Dispose();
      foreach (var partFile in partFiles)
      {
        partFile.Delete();
      }
    }
  }

  private readonly IndependentFilesGeneratorFixture _filesGeneratorFixture;
  private readonly int _validBufferSize;
  private readonly FileInfo _validInputFile;
  private readonly int _validMaxTempFiles;
  private readonly DirectoryInfo _validWorkingFolder;
}
