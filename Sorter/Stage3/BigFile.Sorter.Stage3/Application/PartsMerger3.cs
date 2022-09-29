#region Usings

using BigFile.Common;

#endregion

namespace BigFile.Sorter.Stage3.Application;

internal class PartsMerger3 : IPartsMerger
{
  public PartsMerger3(
    DirectoryInfo workingFolder,
    int kWayMergeFactor,
    MemorySize enumeratorBufferSize)
  {
    _workingFolderPath = workingFolder.FullName;
    _kWayMergeFactor = kWayMergeFactor;
    _enumeratorBufferSize = enumeratorBufferSize;
  }

  public FileInfo MergeParts(IReadOnlyList<FileInfo> partFiles, FileInfo? previousMergedFile)
  {
    if (partFiles == null) throw new ArgumentNullException(nameof(partFiles));

    FileInfo? currentMergedFile = null;
    for (var alreadyMergedFileCount = 0; alreadyMergedFileCount < partFiles.Count; alreadyMergedFileCount += _kWayMergeFactor)
    {
      currentMergedFile = GetNextMergedFile();
      var filesToMerge = partFiles
        .Skip(alreadyMergedFileCount)
        .Take(_kWayMergeFactor)
        .Concat(previousMergedFile is not null ? new[] { previousMergedFile } : Enumerable.Empty<FileInfo>())
        .ToArray();

      try
      {
        using var mergedFileWriter = new StreamWriter(currentMergedFile.OpenWrite());
        Merge(filesToMerge, mergedFileWriter);
      }
      catch
      {
        if (currentMergedFile.Exists) currentMergedFile.Delete();
        throw;
      }

      previousMergedFile?.Delete();
      previousMergedFile = currentMergedFile;
    }

    return currentMergedFile!;
  }

  private void Merge(IReadOnlyList<FileInfo> filesToMerge, StreamWriter mergedFileWriter)
  {
    var enumerators = GetEnumerators(filesToMerge);
    var completedEnumerators = new List<NumberedLinesFileEnumerator>(enumerators.Count);

    try
    {
      while (enumerators.Any())
      {
        var lesserValueReaderIndex = 0;
        for (var readerIndex = 1; readerIndex < enumerators.Count; readerIndex++)
        {
          if (Comparer.Compare(enumerators[lesserValueReaderIndex].Current, enumerators[readerIndex].Current) > 0)
            lesserValueReaderIndex = readerIndex;
        }

        mergedFileWriter.Write(enumerators[lesserValueReaderIndex].Current.EntireLine);

        if (!enumerators[lesserValueReaderIndex].MoveNext())
        {
          completedEnumerators.Add(enumerators[lesserValueReaderIndex]);
          enumerators.RemoveAt(lesserValueReaderIndex);
        }
      }
    }
    finally
    {
      foreach (var enumerator in completedEnumerators)
      {
        enumerator.Dispose();
      }
    }
  }

  private FileInfo GetNextMergedFile() => new(Path.Combine(_workingFolderPath, $"merged{_currentMergedFileNumber++}"));

  private List<NumberedLinesFileEnumerator> GetEnumerators(IReadOnlyList<FileInfo> partFiles)
  {
    var enumerators = new List<NumberedLinesFileEnumerator>(partFiles.Count);
    var enumeratorCount = 0;
    try
    {
      foreach (var partFile in partFiles)
      {
        var enumerator = new NumberedLinesFileEnumerator(partFile, _enumeratorBufferSize);

        if (enumerator.MoveNext())
        {
          // File isn't empty.
          enumerators.Add(enumerator);
          enumeratorCount++;
        }
      }
    }
    catch
    {
      for (var readerIndex = 0; readerIndex < enumeratorCount; readerIndex++)
      {
        enumerators[readerIndex].Dispose();
      }

      throw;
    }

    return enumerators;
  }

  private readonly MemorySize _enumeratorBufferSize;
  private readonly int _kWayMergeFactor;
  private readonly string _workingFolderPath;
  private int _currentMergedFileNumber = 1;
  private static readonly NumberedLineComparer Comparer = new();
}
