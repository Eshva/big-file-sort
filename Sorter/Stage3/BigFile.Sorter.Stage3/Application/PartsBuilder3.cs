#region Usings

using System.Buffers;
using BigFile.Common;
using BigFile.Sorter.Stage3.Domain;

#endregion

namespace BigFile.Sorter.Stage3.Application;

internal class PartsBuilder3 : IPartBuilder
{
  public PartsBuilder3(
    DirectoryInfo workingFolder,
    FileInfo inputFile,
    MemorySize bufferSize,
    int maxTempFiles)
  {
    if (workingFolder == null) throw new ArgumentNullException(nameof(workingFolder));
    if (!workingFolder.Exists)
      throw new ArgumentException($"Working folder {workingFolder.FullName} doesn't exist.", nameof(workingFolder));
    if (inputFile == null) throw new ArgumentNullException(nameof(inputFile));
    // IMPORTANT: File.Exists used because FileInfo.Exists seams like caches the result.
    if (!File.Exists(inputFile.FullName))
      throw new ArgumentException($"Input file {inputFile.FullName} doesn't exist.", nameof(inputFile));
    if (bufferSize < DefaultFileStreamBufferSize)
    {
      throw new ArgumentOutOfRangeException(
        nameof(bufferSize),
        $"Buffer size should be at least {DefaultFileStreamBufferSize} bytes long.");
    }

    if (maxTempFiles < 2) throw new ArgumentOutOfRangeException(nameof(maxTempFiles), "At least 2 temporary files required.");

    bufferSize = bufferSize % DefaultFileStreamBufferSize == 0
      ? bufferSize
      : DefaultFileStreamBufferSize * (bufferSize / DefaultFileStreamBufferSize + 1);
    _bufferSize = bufferSize <= MemoryPool<char>.Shared.MaxBufferSize
      ? bufferSize
      : MemoryPool<char>.Shared.MaxBufferSize;
    _maxTempFiles = maxTempFiles;
    _workingFolderPath = Path.Combine(workingFolder.FullName, Path.GetFileNameWithoutExtension(inputFile.Name));
  }

  public List<FileInfo> BuildSortedParts(StreamReader inputFileReader)
  {
    if (inputFileReader == null) throw new ArgumentNullException(nameof(inputFileReader));

    var partFiles = new List<FileInfo>(_maxTempFiles);
    using var buffer = MemoryPool<char>.Shared.Rent((int)_bufferSize);
    try
    {
      while (!IsTemporaryFileLimitReached(partFiles) && !inputFileReader.EndOfStream)
      {
        var readChars = FillBuffer(
          inputFileReader,
          buffer);

        var partFile = new FileInfo(Path.Combine(_workingFolderPath, $"part{_currentPartNumber++}"));
        var bufferSpan = buffer.Memory.Span.Slice(start: 0, readChars);

        var endOfLineLastIndex = bufferSpan.LastIndexOfAny(value0: '\r', value1: '\n');
        if (endOfLineLastIndex == readChars - 1 && bufferSpan[readChars - 2] != '\r' && bufferSpan[readChars - 2] != '\n')
        {
          // Buffer ends with CR or LF. To be sure next buffer will not start from CR or LF make the current shorter.
          bufferSpan = buffer.Memory.Span.Slice(start: 0, readChars - 1);
          endOfLineLastIndex = bufferSpan.LastIndexOfAny(value0: '\r', value1: '\n');
        }

        BuildPart(partFile, bufferSpan.Slice(start: 0, endOfLineLastIndex + 1));
        var seekBackLength = readChars - endOfLineLastIndex - 1;
        inputFileReader.BaseStream.Seek(-seekBackLength, SeekOrigin.Current);
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

  private int FillBuffer(StreamReader inputFileReader, IMemoryOwner<char> buffer)
  {
    var readChars = 0;
    for (var segmentIndex = 0; segmentIndex < _bufferSize / DefaultFileStreamBufferSize && !inputFileReader.EndOfStream; segmentIndex++)
    {
      var segmentSpan = buffer.Memory.Span.Slice(segmentIndex * DefaultFileStreamBufferSize, DefaultFileStreamBufferSize);
      readChars += inputFileReader.Read(segmentSpan);
    }

    return readChars;
  }

  private void BuildPart(FileInfo partFile, ReadOnlySpan<char> bufferToSort)
  {
    var sortResult = _sorter.Sort(bufferToSort);

    if (partFile.Directory is not null && !partFile.Directory.Exists) partFile.Directory.Create();

    using var writer = new StreamWriter(partFile.OpenWrite());
    foreach (var linePosition in sortResult)
    {
      var lineSpan = bufferToSort.Slice(linePosition.Start, linePosition.Length);
      writer.Write(lineSpan);
    }
  }

  private static bool IsTemporaryFileLimitReached(List<FileInfo> partFiles) => partFiles.Count >= partFiles.Capacity;

  private readonly MemorySize _bufferSize;
  private readonly int _maxTempFiles;
  // NOTE: There is no need to change this sorter from outside. This is why it's not provide as constructor argument.
  private readonly IBufferSorter _sorter = new BufferBubbleSorter(new BufferLineComparer());
  private readonly string _workingFolderPath;
  private int _currentPartNumber = 1;
  private const int DefaultFileStreamBufferSize = 4096;
}
