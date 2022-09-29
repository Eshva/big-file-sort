namespace BigFile.Sorter.Stage1.Application;

internal class PartsMergerFactory1 : IPartsMergerFactory
{
  public PartsMergerFactory1(
    DirectoryInfo workingFolder,
    int kWayMergeFactor,
    FileInfo inputFile)
  {
    _kWayMergeFactor = kWayMergeFactor;
    _workingFolder =
      new DirectoryInfo(Path.Combine(Path.Combine(workingFolder.FullName, Path.GetFileNameWithoutExtension(inputFile.Name))));
  }

  public IPartsMerger Create() => new PartsMerger1(_workingFolder, _kWayMergeFactor);

  private readonly int _kWayMergeFactor;
  private readonly DirectoryInfo _workingFolder;
}
