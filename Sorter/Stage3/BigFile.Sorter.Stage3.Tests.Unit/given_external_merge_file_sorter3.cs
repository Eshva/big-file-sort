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

public class given_external_merge_file_sorter3 : IClassFixture<IndependentFilesGeneratorFixture>
{
  public given_external_merge_file_sorter3(IndependentFilesGeneratorFixture filesGeneratorFixture)
  {
    _filesGeneratorFixture = filesGeneratorFixture;
  }

  [Fact]
  public void when_sort_big_file_in_place_it_should_produce_sorted_file()
  {
    var (inputFile, outputFile, sorter, lineCount) = CreateTestPrerequisites(
      partCount: 200,
      maxTempFiles: 10,
      kWayMergeFactor: 5);

    try
    {
      sorter.Sort(inputFile, inputFile);

      var lines = TestTools.ReadAllSignificantLines(inputFile.FullName);
      lines.Should().HaveCount(lineCount);
      TestTools.CheckIsFileSorted(lines);
    }
    finally
    {
      if (inputFile.Exists) inputFile.Delete();
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Fact]
  public void when_sort_and_merge_requires_many_passes_it_should_produce_sorted_file()
  {
    const int maxTempFiles = 7;
    var partCount = (maxTempFiles - ExternalMergeFileSorterOptions3.AlwaysPresentMergedPartsNumber) * 3;

    var (inputFile, outputFile, sorter, lineCount) = CreateTestPrerequisites(
      partCount,
      maxTempFiles,
      kWayMergeFactor: 5);

    try
    {
      sorter.Sort(inputFile, outputFile);

      var lines = TestTools.ReadAllSignificantLines(outputFile.FullName);
      lines.Should().HaveCount(lineCount);
      TestTools.CheckIsFileSorted(lines);
    }
    finally
    {
      if (inputFile.Exists) inputFile.Delete();
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Fact]
  public void when_sort_and_merge_requires_one_pass_it_should_produce_sorted_file()
  {
    const int maxTempFiles = 7;
    var partCount = maxTempFiles - ExternalMergeFileSorterOptions3.AlwaysPresentMergedPartsNumber - 1;

    var (inputFile, outputFile, sorter, lineCount) = CreateTestPrerequisites(
      partCount,
      maxTempFiles,
      kWayMergeFactor: 5);

    try
    {
      sorter.Sort(inputFile, outputFile);

      var lines = TestTools.ReadAllSignificantLines(outputFile.FullName);
      lines.Should().HaveCount(lineCount);
      TestTools.CheckIsFileSorted(lines);
    }
    finally
    {
      if (inputFile.Exists) inputFile.Delete();
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Fact]
  public void when_sort_and_input_file_content_fits_one_part_it_should_produce_sorted_file()
  {
    var (inputFile, outputFile, sorter, lineCount) = CreateTestPrerequisites(
      partCount: 1,
      maxTempFiles: 7,
      kWayMergeFactor: 5);

    try
    {
      sorter.Sort(inputFile, outputFile);

      var lines = TestTools.ReadAllSignificantLines(outputFile.FullName);
      lines.Should().HaveCount(lineCount);
      TestTools.CheckIsFileSorted(lines);
    }
    finally
    {
      if (inputFile.Exists) inputFile.Delete();
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Fact]
  public void when_sort_empty_file_it_should_produce_empty_file()
  {
    var inputFile = new FileInfo(Path.Combine(_filesGeneratorFixture.Folder.FullName, $"file{Guid.NewGuid():N}.txt"));
    inputFile.CreateText().Dispose();
    var outputFilePath = Path.Combine(
      _filesGeneratorFixture.Folder.FullName,
      Path.GetFileNameWithoutExtension(inputFile.Name) + "-sorted.txt");
    var options = new ExternalMergeFileSorterOptions3(
      _filesGeneratorFixture.Folder,
      maxTempFiles: 7,
      kWayMergeFactor: 5);
    var sorter = new ExternalMergeFileSorter3(
      new PartsBuilderFactory3(
        options.WorkingFolder,
        options.PartsBuilderBufferSize,
        options.PartFileCount),
      new PartsMergerFactory3(
        options.WorkingFolder,
        inputFile,
        options.KWayMergeFactor,
        options.PartsMergerBufferSize));
    var outputFile = new FileInfo(outputFilePath);

    try
    {
      sorter.Sort(inputFile, outputFile);
      File.Exists(outputFile.FullName).Should().BeTrue();
      outputFile.Length.Should().Be(expected: 0);
    }
    finally
    {
      if (inputFile.Exists) inputFile.Delete();
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Fact]
  public void when_sort_with_invalid_arguments_it_should_complain()
  {
    var (inputFile, _, sorter, _) = CreateTestPrerequisites(
      partCount: 1,
      maxTempFiles: 10,
      kWayMergeFactor: 5);

    var sut = () => sorter.Sort(null!, inputFile);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("inputFile");

    sut = () => sorter.Sort(inputFile, null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("outputFile");

    sut = () => sorter.Sort(new FileInfo("this file doesn't exist"), inputFile);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("inputFile")
      .And.Message.ToLower().Should().Contain("input file");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var validPartsBuilderFactory = new StubPartsBuilderFactory();
    var validPartsMergerFactory = new StubPartsMergerFactory();

    var sut = () => new ExternalMergeFileSorter3(
      null!,
      validPartsMergerFactory);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("partsBuilderFactory");

    sut = () => new ExternalMergeFileSorter3(
      validPartsBuilderFactory,
      null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("partsMergerFactory");
  }

  private (FileInfo inputFile, FileInfo outputFile, ExternalMergeFileSorter3 sorter, int lineCount) CreateTestPrerequisites(
    int partCount,
    int maxTempFiles,
    int kWayMergeFactor)
  {
    const int lineLengthInChars = 32;
    var stringLength = lineLengthInChars - 1 - 2 - Environment.NewLine.Length;
    _filesGeneratorFixture.SetLinesGeneratingParameters(
      maxNumber: 9,
      minStringLength: stringLength,
      maxStringLength: stringLength);

    var lineCount = 4096 / lineLengthInChars * partCount;

    var options = new ExternalMergeFileSorterOptions3(
      _filesGeneratorFixture.Folder,
      maxTempFiles,
      kWayMergeFactor);

    var inputFile = _filesGeneratorFixture.GenerateFile(lineCount);
    var sorter = new ExternalMergeFileSorter3(
      new PartsBuilderFactory3(
        options.WorkingFolder,
        options.PartsBuilderBufferSize,
        options.PartFileCount),
      new PartsMergerFactory3(
        options.WorkingFolder,
        inputFile,
        options.KWayMergeFactor,
        options.PartsMergerBufferSize));
    var outputFilePath = Path.Combine(
      _filesGeneratorFixture.Folder.FullName,
      Path.GetFileNameWithoutExtension(inputFile.Name) + "-sorted.txt");
    var outputFile = new FileInfo(outputFilePath);

    return (inputFile, outputFile, sorter, lineCount);
  }

  private readonly IndependentFilesGeneratorFixture _filesGeneratorFixture;

  private class StubPartsMergerFactory : IPartsMergerFactory
  {
    public IPartsMerger Create() => new StubPartsMerger();

    private class StubPartsMerger : IPartsMerger
    {
      public FileInfo MergeParts(IReadOnlyList<FileInfo> partFiles, FileInfo? mergedFile) => new("not existing file");
    }
  }

  private class StubPartsBuilderFactory : IPartsBuilderFactory
  {
    public IPartBuilder Create(FileInfo inputFile) => new StubPartsBuilder();

    private class StubPartsBuilder : IPartBuilder
    {
      public List<FileInfo> BuildSortedParts(StreamReader inputFileReader) => new(capacity: 0);
    }
  }
}
