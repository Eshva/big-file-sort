#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using BigFile.Sorter.Stage1.Application;
using BigFile.Tests.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage1.Tests.Unit;

public class given_parts_builder1 : IClassFixture<IndependentFilesGeneratorFixture>, IDisposable
{
  public given_parts_builder1(IndependentFilesGeneratorFixture filesGeneratorFixture)
  {
    _filesGeneratorFixture = filesGeneratorFixture;
    _validWorkingFolder = new DirectoryInfo(Path.GetTempPath());
    _validInputFile = new FileInfo(Path.GetTempFileName());
    _validInputFile.OpenWrite().Dispose();
    _validLinesPerPart = 5;
    _validMaxTempFiles = 10;
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_exceeding_number_of_lines_it_should_produce_maximum_files_with_expected_content()
  {
    const int linesPerPart = 5;
    const int maxTempFiles = 10;
    var sourceFile = _filesGeneratorFixture.GenerateFile(maxTempFiles * linesPerPart * 2);
    var partsBuilder = new PartsBuilder1(
      _filesGeneratorFixture.Folder,
      sourceFile,
      linesPerPart,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      maxTempFiles,
      sourceFile,
      linesPerPart);
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_smaller_number_of_lines_it_should_produce_required_files_with_expected_content()
  {
    const int linesPerPart = 5;
    const int maxTempFiles = 10;
    var sourceFileLineCount = (maxTempFiles - 1) * linesPerPart - 1;
    var sourceFile = _filesGeneratorFixture.GenerateFile(sourceFileLineCount);
    var partsBuilder = new PartsBuilder1(
      _filesGeneratorFixture.Folder,
      sourceFile,
      linesPerPart,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      sourceFileLineCount / linesPerPart + 1,
      sourceFile,
      linesPerPart);
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_lines_only_for_one_part_it_should_produce_only_one_file_with_expected_content()
  {
    const int linesPerPart = 5;
    const int maxTempFiles = 10;
    var sourceFileLineCount = linesPerPart - 1;
    var sourceFile = _filesGeneratorFixture.GenerateFile(sourceFileLineCount);
    var partsBuilder = new PartsBuilder1(
      _filesGeneratorFixture.Folder,
      sourceFile,
      linesPerPart,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      expectedNumberOfFiles: 1,
      sourceFile,
      linesPerPart);
  }

  [Fact]
  public void when_build_sorted_parts_and_source_file_has_no_lines_it_should_produce_empty_file()
  {
    const int linesPerPart = 5;
    const int maxTempFiles = 10;
    var sourceFile = new FileInfo(Path.Combine(_filesGeneratorFixture.Folder.FullName, "empty"));
    sourceFile.CreateText().Dispose();
    var partsBuilder = new PartsBuilder1(
      _filesGeneratorFixture.Folder,
      sourceFile,
      linesPerPart,
      maxTempFiles);

    List<FileInfo> partFiles;
    using (var inputFileReader = new StreamReader(sourceFile.OpenRead()))
    {
      partFiles = partsBuilder.BuildSortedParts(inputFileReader);
    }

    CheckPartFilesCorrectness(
      partFiles,
      expectedNumberOfFiles: 0,
      sourceFile,
      linesPerPart);
  }

  [Fact]
  public void when_build_sorted_parts_with_invalid_arguments_it_should_complain()
  {
    var partsBuilder = new PartsBuilder1(
      _validWorkingFolder,
      _validInputFile,
      _validLinesPerPart,
      _validMaxTempFiles);

    var sut = () => partsBuilder.BuildSortedParts(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("inputFileReader");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var sut = () => new PartsBuilder1(
      null!,
      _validInputFile,
      _validLinesPerPart,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("workingFolder");

    sut = () => new PartsBuilder1(
      new DirectoryInfo("folder doesn't exist"),
      _validInputFile,
      _validLinesPerPart,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("workingFolder")
      .And.Message.ToLower().Should().Contain("doesn't exist");

    sut = () => new PartsBuilder1(
      _validWorkingFolder,
      null!,
      _validLinesPerPart,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("inputFile");

    sut = () => new PartsBuilder1(
      _validWorkingFolder,
      new FileInfo("file doesn't exist"),
      _validLinesPerPart,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("inputFile")
      .And.Message.ToLower().Should().Contain("doesn't exist");

    sut = () => new PartsBuilder1(
      _validWorkingFolder,
      _validInputFile,
      linesPerPart: 0,
      _validMaxTempFiles);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("linesPerPart")
      .And.Message.ToLower().Should().Contain("at least 1");

    sut = () => new PartsBuilder1(
      _validWorkingFolder,
      _validInputFile,
      _validLinesPerPart,
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
    FileInfo file,
    int linesPerPart)
  {
    partFiles.Should().HaveCount(expectedNumberOfFiles);

    var sourceFileReader = new StreamReader(file.OpenRead());
    try
    {
      foreach (var partFile in partFiles)
      {
        var sourceLines = new List<string>(linesPerPart);
        var partLines = new List<string>(linesPerPart);
        using var partFileReader = new StreamReader(partFile.OpenRead());
        for (var lineIndex = 0; lineIndex < linesPerPart; lineIndex++)
        {
          var sourceLine = sourceFileReader.ReadLine();
          if (sourceLine is not null) sourceLines.Add(sourceLine);

          var partLine = partFileReader.ReadLine();
          if (partLine is not null) partLines.Add(partLine);
        }

        partFileReader.EndOfStream.Should().BeTrue();
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
  private readonly FileInfo _validInputFile;
  private readonly int _validLinesPerPart;
  private readonly int _validMaxTempFiles;
  private readonly DirectoryInfo _validWorkingFolder;
}
