#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using BigFile.Common;
using BigFile.Sorter.Stage2.Application;
using BigFile.Sorter.Stage2.Domain;
using BigFile.Tests.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage2.Tests.Unit;

public class given_external_merge_file_sorter2 : IClassFixture<IndependentFilesGeneratorFixture>
{
  public given_external_merge_file_sorter2(IndependentFilesGeneratorFixture filesGeneratorFixture)
  {
    _filesGeneratorFixture = filesGeneratorFixture;
  }

  [Fact]
  public void when_sort_big_file_in_place_it_should_produce_sorted_file()
  {
    var maxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 500);
    var maxTempFiles = 10;
    var kWayMergeFactor = 5;
    var lineCount = 100_000;

    var (inputFile, outputFile, sorter) = CreateTestPrerequisites(
      lineCount,
      maxMemoryUsage,
      maxTempFiles,
      kWayMergeFactor);

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
    var maxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 100);
    var maxTempFiles = 7;
    var kWayMergeFactor = 5;
    var partCount = maxTempFiles - ExternalMergeFileSorterOptions2.AlwaysPresentMergedPartsNumber;
    var lineCount = new LinesPerPartCalculator().Calculate(maxMemoryUsage, partCount) * partCount * 5;

    var (inputFile, outputFile, sorter) = CreateTestPrerequisites(
      lineCount,
      maxMemoryUsage,
      maxTempFiles,
      kWayMergeFactor);

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
    var maxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 100);
    var maxTempFiles = 7;
    var kWayMergeFactor = 5;
    var partCount = maxTempFiles - ExternalMergeFileSorterOptions2.AlwaysPresentMergedPartsNumber;
    var lineCount = new LinesPerPartCalculator().Calculate(maxMemoryUsage, partCount) * partCount;

    var (inputFile, outputFile, sorter) = CreateTestPrerequisites(
      lineCount,
      maxMemoryUsage,
      maxTempFiles,
      kWayMergeFactor);

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
    var maxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 100);
    var maxTempFiles = 7;
    var kWayMergeFactor = 5;
    var partCount = maxTempFiles - ExternalMergeFileSorterOptions2.AlwaysPresentMergedPartsNumber;
    var lineCount = new LinesPerPartCalculator().Calculate(maxMemoryUsage, partCount);

    var (inputFile, outputFile, sorter) = CreateTestPrerequisites(
      lineCount,
      maxMemoryUsage,
      maxTempFiles,
      kWayMergeFactor);

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
    var maxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 100);
    var maxTempFiles = 7;
    var kWayMergeFactor = 5;
    var partCount = maxTempFiles - ExternalMergeFileSorterOptions2.AlwaysPresentMergedPartsNumber;
    var lineCount = new LinesPerPartCalculator().Calculate(maxMemoryUsage, partCount);

    var (inputFile, outputFile, sorter) = CreateTestPrerequisites(
      lineCount,
      maxMemoryUsage,
      maxTempFiles,
      kWayMergeFactor);

    inputFile.Delete();
    inputFile.CreateText().Dispose();

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
    var (inputFile, _, sorter) = CreateTestPrerequisites(
      lineCount: 1000,
      MemorySize.FromKilobytes(kilobytes: 500),
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

    var sut = () => new ExternalMergeFileSorter2(
      null!,
      validPartsMergerFactory);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("partsBuilderFactory");

    sut = () => new ExternalMergeFileSorter2(
      validPartsBuilderFactory,
      null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("partsMergerFactory");
  }

  private (FileInfo inputFile, FileInfo outputFile, ExternalMergeFileSorter2 sorter) CreateTestPrerequisites(
    int lineCount,
    MemorySize maxMemoryUsage,
    int maxTempFiles,
    int kWayMergeFactor)
  {
    var options = new ExternalMergeFileSorterOptions2(
      maxMemoryUsage,
      _filesGeneratorFixture.Folder,
      maxTempFiles,
      kWayMergeFactor);

    var inputFile = _filesGeneratorFixture.GenerateFile(lineCount);
    var sorter = new ExternalMergeFileSorter2(
      new PartsBuilderFactory2(
        options.WorkingFolder,
        options.LinesPerPart,
        options.PartFileCount),
      new PartsMergerFactory2(
        options.WorkingFolder,
        options.KWayMergeFactor,
        inputFile));
    var outputFilePath = Path.Combine(
      _filesGeneratorFixture.Folder.FullName,
      Path.GetFileNameWithoutExtension(inputFile.Name) + "-sorted.txt");
    var outputFile = new FileInfo(outputFilePath);

    return (inputFile, outputFile, sorter);
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
