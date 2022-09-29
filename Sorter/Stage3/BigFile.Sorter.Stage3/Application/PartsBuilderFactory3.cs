#region Usings

using BigFile.Common;

#endregion

namespace BigFile.Sorter.Stage3.Application;

internal class PartsBuilderFactory3 : IPartsBuilderFactory
{
  public PartsBuilderFactory3(
    DirectoryInfo workingFolder,
    MemorySize bufferSize,
    int maxPartFiles)
  {
    if (maxPartFiles < 2) throw new ArgumentOutOfRangeException(nameof(maxPartFiles), "At least 2 part files required.");

    _workingFolder = workingFolder ?? throw new ArgumentNullException(nameof(workingFolder));
    _bufferSize = bufferSize;
    _maxPartFiles = maxPartFiles;
  }

  public IPartBuilder Create(FileInfo inputFile) =>
    new PartsBuilder3(
      _workingFolder,
      inputFile,
      _bufferSize,
      _maxPartFiles);

  private readonly MemorySize _bufferSize;
  private readonly int _maxPartFiles;
  private readonly DirectoryInfo _workingFolder;
}
