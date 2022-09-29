#region Usings

using BigFile.Common;

#endregion

namespace BigFile.Sorter.Stage4.Application;

internal class PartsMergerFactory4 : IPartsMergerFactory
{
  public PartsMergerFactory4(
    DirectoryInfo workingFolder,
    FileInfo inputFile,
    int kWayMergeFactor,
    MemorySize enumeratorBufferSize)
  {
    if (workingFolder == null) throw new ArgumentNullException(nameof(workingFolder));
    if (inputFile == null) throw new ArgumentNullException(nameof(inputFile));
    if (kWayMergeFactor < 2) throw new ArgumentOutOfRangeException(nameof(kWayMergeFactor), "At least to files required for merging.");

    _workingFolder =
      new DirectoryInfo(Path.Combine(Path.Combine(workingFolder.FullName, Path.GetFileNameWithoutExtension(inputFile.Name))));
    _kWayMergeFactor = kWayMergeFactor;
    _enumeratorBufferSize = enumeratorBufferSize;
  }

  public IPartsMerger Create() => new PartsMerger4(
    _workingFolder,
    _kWayMergeFactor,
    _enumeratorBufferSize);

  private readonly MemorySize _enumeratorBufferSize;
  private readonly int _kWayMergeFactor;
  private readonly DirectoryInfo _workingFolder;
}
