#region Usings

using BigFile.Common;

#endregion

namespace BigFile.Sorter.Stage4.Application;

internal class ExternalMergeFileSorterOptions4
{
  public ExternalMergeFileSorterOptions4(
    DirectoryInfo workingFolder,
    int maxTempFiles,
    int kWayMergeFactor,
    MemorySize? partsBuilderBufferSize = null,
    MemorySize? partsMergerBufferSize = null)
  {
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

    if (partsBuilderBufferSize < MinimalPartsBuilderBufferSize)
    {
      throw new ArgumentOutOfRangeException(
        nameof(partsBuilderBufferSize),
        $"Parts builder buffers size should be at least {MinimalPartsBuilderBufferSize}.");
    }

    PartsBuilderBufferSize = partsBuilderBufferSize ?? MemorySize.FromKilobytes(kilobytes: 4);

    if (partsMergerBufferSize < MinimalPartsMergerBufferSize)
    {
      throw new ArgumentOutOfRangeException(
        nameof(partsMergerBufferSize),
        $"Parts merger buffers size should be at least {MinimalPartsMergerBufferSize}.");
    }

    PartsMergerBufferSize = partsMergerBufferSize ?? MemorySize.FromKilobytes(kilobytes: 4);
  }

  public DirectoryInfo WorkingFolder { get; }

  public int PartFileCount { get; }

  public MemorySize PartsBuilderBufferSize { get; }

  public MemorySize PartsMergerBufferSize { get; }

  public int KWayMergeFactor { get; }

  public const int MinimumRequiredTempFiles = MinimumRequiredMergeableParts + AlwaysPresentMergedPartsNumber;
  public const int MinimumRequiredMergeableParts = 2;
  public const int MaximumTempFilesInUse = 1000;
  public const int AlwaysPresentMergedPartsNumber = 2;

  private static readonly MemorySize MinimalPartsBuilderBufferSize = MemorySize.FromKilobytes(kilobytes: 4);
  private static readonly MemorySize MinimalPartsMergerBufferSize = MemorySize.FromKilobytes(kilobytes: 4);
}
