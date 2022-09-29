namespace BigFile.Sorter.Stage2.Application;

internal class PartsBuilderFactory2 : IPartsBuilderFactory
{
  public PartsBuilderFactory2(
    DirectoryInfo workingFolder,
    int linesPerPart,
    int maxPartFiles)
  {
    _workingFolder = workingFolder;
    _linesPerPart = linesPerPart;
    _maxPartFiles = maxPartFiles;
  }

  public IPartBuilder Create(FileInfo inputFile) =>
    new PartsBuilder2(
      _workingFolder,
      inputFile,
      _linesPerPart,
      _maxPartFiles);

  private readonly int _linesPerPart;
  private readonly int _maxPartFiles;
  private readonly DirectoryInfo _workingFolder;
}
