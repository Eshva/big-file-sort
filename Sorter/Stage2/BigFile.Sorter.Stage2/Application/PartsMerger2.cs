#region Usings

using BigFile.Sorter.Stage2.Domain;

#endregion

namespace BigFile.Sorter.Stage2.Application;

internal class PartsMerger2 : IPartsMerger
{
  public PartsMerger2(DirectoryInfo workingFolder, int kWayMergeFactor)
  {
    _workingFolderPath = workingFolder.FullName;
    _kWayMergeFactor = kWayMergeFactor;
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

  private static void Merge(IReadOnlyList<FileInfo> filesToMerge, StreamWriter mergedFileWriter)
  {
    var (readers, topLines) = GetReaders(filesToMerge);
    var emptyReaders = new List<StreamReader>(readers.Count);

    try
    {
      while (readers.Any())
      {
        var lesserValueReaderIndex = 0;
        for (var readerIndex = 1; readerIndex < readers.Count; readerIndex++)
        {
          if (Comparer.Compare(topLines[lesserValueReaderIndex], topLines[readerIndex]) > 0) lesserValueReaderIndex = readerIndex;
        }

        mergedFileWriter.WriteLine(topLines[lesserValueReaderIndex]);
        var nextLine = readers[lesserValueReaderIndex].ReadLine();
        if (nextLine is not null)
          topLines[lesserValueReaderIndex] = nextLine;
        else
        {
          emptyReaders.Add(readers[lesserValueReaderIndex]);
          readers.RemoveAt(lesserValueReaderIndex);
          topLines.RemoveAt(lesserValueReaderIndex);
        }
      }
    }
    finally
    {
      foreach (var reader in emptyReaders)
      {
        reader.Dispose();
      }
    }
  }

  private FileInfo GetNextMergedFile() => new(Path.Combine(_workingFolderPath, $"merged{_currentMergedFileNumber++}"));

  private static (List<StreamReader> readers, List<string> topLines) GetReaders(IReadOnlyList<FileInfo> partFiles)
  {
    var readers = new List<StreamReader>(partFiles.Count);
    var topLines = new List<string>(partFiles.Count);
    var readersCount = 0;
    try
    {
      foreach (var partFile in partFiles)
      {
        var reader = new StreamReader(partFile.OpenRead());
        var line = reader.ReadLine();
        if (line is null) continue;

        topLines.Add(line);
        readers.Add(reader);
        readersCount++;
      }
    }
    catch
    {
      for (var readerIndex = 0; readerIndex < readersCount; readerIndex++)
      {
        readers[readerIndex].Dispose();
      }

      throw;
    }

    return (readers, topLines);
  }

  private readonly int _kWayMergeFactor;
  private readonly string _workingFolderPath;
  private int _currentMergedFileNumber = 1;
  private static readonly NumberedLineComparer Comparer = new();
}
