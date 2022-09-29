#region Usings

using BigFile.Common;
using BigFile.Sorter.Stage1.Domain;

#endregion

namespace BigFile.Sorter.Stage1.Application;

internal class ExternalMergeFileSorterOptions1
{
  public ExternalMergeFileSorterOptions1(
    MemorySize maxMemoryUsage,
    DirectoryInfo workingFolder,
    int maxTempFiles,
    int kWayMergeFactor)
  {
    if (maxMemoryUsage < MinimumRequiredMemorySize)
      throw new ArgumentOutOfRangeException(nameof(maxMemoryUsage), "Max memory usage shouldn't be less than 10Ki.");

    WorkingFolder = workingFolder ?? throw new ArgumentNullException(nameof(workingFolder));
    if (!workingFolder.Exists) throw new ArgumentException("Working folder should exist.", nameof(workingFolder));

    switch (maxTempFiles)
    {
      case < MinimumRequiredTempFiles:
        throw new ArgumentOutOfRangeException(nameof(maxTempFiles), $"At least {MinimumRequiredTempFiles} temporary files are required.");
      case > MaximumTempFilesInUse:
        throw new ArgumentOutOfRangeException(nameof(maxTempFiles), $"Number of temporary files can't exceed {MaximumTempFilesInUse}.");
    }

    if (kWayMergeFactor < MinimumRequiredMergeableParts)
    {
      throw new ArgumentOutOfRangeException(
        nameof(kWayMergeFactor),
        $"At least {MinimumRequiredMergeableParts} K-Way merge factor is required.");
    }

    if (MinimumRequiredMergeableParts + kWayMergeFactor > maxTempFiles)
    {
      throw new ArgumentOutOfRangeException(
        nameof(kWayMergeFactor),
        $"K-Way merge factor should be less than {maxTempFiles - MinimumRequiredMergeableParts + 1}.");
    }

    KWayMergeFactor = kWayMergeFactor;
    PartFileCount = maxTempFiles - AlwaysPresentMergedPartsNumber;

    LinesPerPart = new LinesPerPartCalculator().Calculate(maxMemoryUsage, PartFileCount);
  }

  public DirectoryInfo WorkingFolder { get; }

  public int PartFileCount { get; }

  public int LinesPerPart { get; }

  public int KWayMergeFactor { get; }

  public const int MinimumRequiredTempFiles = MinimumRequiredMergeableParts + AlwaysPresentMergedPartsNumber;
  public const int MinimumRequiredMergeableParts = 2;
  public const int MaximumTempFilesInUse = 1000;
  public const int AlwaysPresentMergedPartsNumber = 2;

  internal static readonly MemorySize MinimumRequiredMemorySize = MemorySize.FromKilobytes(kilobytes: 10);
}
