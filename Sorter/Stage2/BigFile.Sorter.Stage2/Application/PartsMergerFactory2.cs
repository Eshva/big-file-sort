namespace BigFile.Sorter.Stage2.Application;

internal class PartsMergerFactory2 : IPartsMergerFactory
{
  public PartsMergerFactory2(
    DirectoryInfo workingFolder,
    int kWayMergeFactor,
    FileInfo inputFile)
  {
    _kWayMergeFactor = kWayMergeFactor;
    _workingFolder =
      new DirectoryInfo(Path.Combine(Path.Combine(workingFolder.FullName, Path.GetFileNameWithoutExtension(inputFile.Name))));
  }

  public IPartsMerger Create() => new PartsMerger2(_workingFolder, _kWayMergeFactor);

  private readonly int _kWayMergeFactor;
  private readonly DirectoryInfo _workingFolder;
}
