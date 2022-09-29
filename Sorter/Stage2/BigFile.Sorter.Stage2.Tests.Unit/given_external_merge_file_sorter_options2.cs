#region Usings

using System;
using System.IO;
using BigFile.Common;
using BigFile.Sorter.Stage2.Application;
using BigFile.Sorter.Stage2.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage2.Tests.Unit;

public class given_external_merge_file_sorter_options2
{
  [Fact]
  public void when_construct_it_should_produce_instance_with_expected_state()
  {
    var maxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 100);
    const int maxTempFiles = 4;
    const int kWayMergeFactor = 2;
    var options = new ExternalMergeFileSorterOptions2(
      maxMemoryUsage,
      new DirectoryInfo(Path.GetTempPath()),
      maxTempFiles,
      kWayMergeFactor);

    options.WorkingFolder.FullName.Should().Be(new DirectoryInfo(Path.GetTempPath()).FullName);
    options.PartFileCount.Should().Be(maxTempFiles - ExternalMergeFileSorterOptions2.AlwaysPresentMergedPartsNumber);
    options.LinesPerPart.Should().Be(new LinesPerPartCalculator().Calculate(maxMemoryUsage, maxTempFiles - kWayMergeFactor));
    options.KWayMergeFactor.Should().Be(kWayMergeFactor);
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var validMaxMemoryUsage = MemorySize.FromKilobytes(kilobytes: 100);
    var validWorkingFolder = new DirectoryInfo(Path.GetTempPath());
    var validMaxTempFiles = 4;
    var validKWayMergeFactor = 2;

    var sut = () => new ExternalMergeFileSorterOptions2(
      MemorySize.FromBytes(ExternalMergeFileSorterOptions2.MinimumRequiredMemorySize - 1),
      validWorkingFolder,
      validMaxTempFiles,
      validKWayMergeFactor);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxMemoryUsage")
      .And.Message.ToLower().Should().Contain("shouldn't be less than");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      null!,
      validMaxTempFiles,
      validKWayMergeFactor);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("workingFolder");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      new DirectoryInfo("not existing folder"),
      validMaxTempFiles,
      validKWayMergeFactor);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("workingFolder")
      .And.Message.ToLower().Should().Contain("should exist");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      validWorkingFolder,
      maxTempFiles: -1,
      validKWayMergeFactor);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      validWorkingFolder,
      ExternalMergeFileSorterOptions2.MinimumRequiredTempFiles - 1,
      validKWayMergeFactor);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      validWorkingFolder,
      ExternalMergeFileSorterOptions2.MaximumTempFilesInUse + 1,
      validKWayMergeFactor);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("maxTempFiles")
      .And.Message.ToLower().Should().Contain("can't exceed");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      validWorkingFolder,
      validMaxTempFiles,
      ExternalMergeFileSorterOptions2.MinimumRequiredMergeableParts - 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("kWayMergeFactor")
      .And.Message.ToLower().Should().Contain("at least");

    sut = () => new ExternalMergeFileSorterOptions2(
      validMaxMemoryUsage,
      validWorkingFolder,
      validMaxTempFiles,
      ExternalMergeFileSorterOptions2.MinimumRequiredTempFiles - ExternalMergeFileSorterOptions2.MinimumRequiredMergeableParts + 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("kWayMergeFactor")
      .And.Message.ToLower().Should().Contain("should be less than");
  }
}
