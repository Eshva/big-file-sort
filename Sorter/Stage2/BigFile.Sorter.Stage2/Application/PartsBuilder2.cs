#region Usings

using BigFile.Sorter.Stage2.Domain;

#endregion

namespace BigFile.Sorter.Stage2.Application;

internal class PartsBuilder2 : IPartBuilder
{
  public PartsBuilder2(
    DirectoryInfo workingFolder,
    FileInfo inputFile,
    int linesPerPart,
    int maxTempFiles)
  {
    if (workingFolder == null) throw new ArgumentNullException(nameof(workingFolder));
    if (!workingFolder.Exists)
      throw new ArgumentException($"Working folder {workingFolder.FullName} doesn't exist.", nameof(workingFolder));
    if (inputFile == null) throw new ArgumentNullException(nameof(inputFile));
    // IMPORTANT: File.Exists used because FileInfo.Exists seams like caches the result.
    if (!File.Exists(inputFile.FullName))
      throw new ArgumentException($"Input file {inputFile.FullName} doesn't exist.", nameof(inputFile));
    if (linesPerPart < 1) throw new ArgumentOutOfRangeException(nameof(linesPerPart), "Each part file should contains at least 1 line.");
    if (maxTempFiles < 2) throw new ArgumentOutOfRangeException(nameof(maxTempFiles), "At least 2 temporary files required.");

    _linesPerPart = linesPerPart;
    _maxTempFiles = maxTempFiles;
    _workingFolderPath = Path.Combine(workingFolder.FullName, Path.GetFileNameWithoutExtension(inputFile.Name));
  }

  public List<FileInfo> BuildSortedParts(StreamReader inputFileReader)
  {
    if (inputFileReader == null) throw new ArgumentNullException(nameof(inputFileReader));

    var partFiles = new List<FileInfo>(_maxTempFiles);
    try
    {
      while (!IsTemporaryFileLimitReached(partFiles) && !inputFileReader.EndOfStream)
      {
        var partLines = new List<string>(_linesPerPart);
        while (!inputFileReader.EndOfStream && partLines.Count < _linesPerPart)
        {
          var line = inputFileReader.ReadLine();
          if (line == null) break;

          partLines.Add(line);
        }

        var partFile = new FileInfo(Path.Combine(_workingFolderPath, $"part{_currentPartNumber++}"));
        BuildPart(partLines, partFile);
        partFiles.Add(partFile);
      }
    }
    catch
    {
      foreach (var partFile in partFiles)
      {
        partFile.Delete();
      }

      throw;
    }

    return partFiles;
  }

  private void BuildPart(List<string> partLines, FileInfo partFile)
  {
    _sorter.Sort(partLines);

    if (partFile.Directory is not null && !partFile.Directory.Exists) partFile.Directory.Create();

    using var writer = new StreamWriter(partFile.OpenWrite());
    foreach (var partLine in partLines)
    {
      writer.WriteLine(partLine);
    }
  }

  private static bool IsTemporaryFileLimitReached(List<FileInfo> partFiles) => partFiles.Count >= partFiles.Capacity;

  private readonly int _linesPerPart;
  private readonly int _maxTempFiles;
  private readonly ISorter<string> _sorter = new StringBubbleSorter(new NumberedLineComparer());
  private readonly string _workingFolderPath;
  private int _currentPartNumber = 1;
}
