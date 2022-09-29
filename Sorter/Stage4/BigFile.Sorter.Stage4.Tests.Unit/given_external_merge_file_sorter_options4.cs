#region Usings

using System;
using System.IO;
using BigFile.Common;
using BigFile.Sorter.Stage4.Application;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage4.Tests.Unit;

public class given_external_merge_file_sorter_options4
{
  [Fact]
  public void when_construct_it_should_produce_instance_with_expected_state()
  {
    const int maxTempFiles = 4;
    const int kWayMergeFactor = 2;
    var options = new ExternalMergeFileSorterOptions4(
      new DirectoryInfo(Path.GetTempPath()),
      maxTempFiles,
      kWayMergeFactor);

    options.WorkingFolder.FullName.Should().Be(new DirectoryInfo(Path.GetTempPath()).FullName);
    options.PartFileCount.Should().Be(maxTempFiles - ExternalMergeFileSorterOptions4.AlwaysPresentMergedPartsNumber);
    options.PartsBuilderBufferSize.Should().Be(MemorySize.FromKilobytes(kilobytes: 4));
    options.PartsMergerBufferSize.Should().Be(MemorySize.FromKilobytes(kilobytes: 4));
    options.KWayMergeFactor.Should().Be(kWayMergeFactor);

    options = new ExternalMergeFileSorterOptions4(
      new DirectoryInfo(Path.GetTempPath()),
      maxTempFiles,
      kWayMergeFactor,
      MemorySize.FromKilobytes(kilobytes: 12),
      MemorySize.FromKilobytes(kilobytes: 42));

    options.PartsBuilderBufferSize.Should().Be(MemorySize.FromKilobytes(kilobytes: 12));
    options.PartsMergerBufferSize.Should().Be(MemorySize.FromKilobytes(kilobytes: 42));
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var validWorkingFolder = new DirectoryInfo(Path.GetTempPath());
    var validMaxTempFiles = 4;
    var validKWayMergeFactor = 2;
    var validPartsBuilderBufferSize = MemorySize.FromKilobytes(kilobytes: 16);
    var validPartsMergerBufferSize = MemorySize.FromKilobytes(kilobytes: 4);

    var sut = () => new ExternalMergeFileSorterOptions4(
      null!,
      validMaxTempFiles,
      validKWayMergeFactor,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("workingFolder");

    sut = () => new ExternalMergeFileSorterOptions4(
      new DirectoryInfo("not existing folder"),
      validMaxTempFiles,
      validKWayMergeFactor,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("workingFolder")
      .And.Message.ToLower().Should().Contain("should exist");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      maxTempFiles: -1,
      validKWayMergeFactor,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      ExternalMergeFileSorterOptions4.MinimumRequiredTempFiles - 1,
      validKWayMergeFactor,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      ExternalMergeFileSorterOptions4.MaximumTempFilesInUse + 1,
      validKWayMergeFactor,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("can't exceed");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      validMaxTempFiles,
      ExternalMergeFileSorterOptions4.MinimumRequiredMergeableParts - 1,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("kWayMergeFactor")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      validMaxTempFiles,
      ExternalMergeFileSorterOptions4.MinimumRequiredTempFiles - ExternalMergeFileSorterOptions4.MinimumRequiredMergeableParts + 1,
      validPartsBuilderBufferSize,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("kWayMergeFactor")
      .And.Message.ToLower().Should().Contain("should be less than");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      validMaxTempFiles,
      validKWayMergeFactor,
      MemorySize.FromKilobytes(kilobytes: 4) - 1,
      validPartsMergerBufferSize);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("partsBuilderBufferSize")
      .And.Message.ToLower().Should().Contain("should be at least");

    sut = () => new ExternalMergeFileSorterOptions4(
      validWorkingFolder,
      validMaxTempFiles,
      validKWayMergeFactor,
      validPartsBuilderBufferSize,
      MemorySize.FromKilobytes(kilobytes: 4) - 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("partsMergerBufferSize")
      .And.Message.ToLower().Should().Contain("should be at least");
  }
}
