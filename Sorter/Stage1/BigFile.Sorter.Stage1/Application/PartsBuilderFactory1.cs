namespace BigFile.Sorter.Stage1.Application;

internal class PartsBuilderFactory1 : IPartsBuilderFactory
{
  public PartsBuilderFactory1(
    DirectoryInfo workingFolder,
    int linesPerPart,
    int maxPartFiles)
  {
    _workingFolder = workingFolder;
    _linesPerPart = linesPerPart;
    _maxPartFiles = maxPartFiles;
  }

  public IPartBuilder Create(FileInfo inputFile) =>
    new PartsBuilder1(
      _workingFolder,
      inputFile,
      _linesPerPart,
      _maxPartFiles);

  private readonly int _linesPerPart;
  private readonly int _maxPartFiles;
  private readonly DirectoryInfo _workingFolder;
}
