#region Usings

using BigFile.Generator;

#endregion

namespace BigFile.Tests.Common;

public class IndependentFilesGeneratorFixture : IDisposable
{
  public IndependentFilesGeneratorFixture()
  {
    Folder = new DirectoryInfo(
      Path.Combine(
        Path.GetTempPath(),
        "big-file",
        $"tff-{Guid.NewGuid():N}"));
    Folder.Create();
    _generator = new TextFileGenerator(
      new NumberedStringLineGenerator(
        MinNumber,
        MaxNumber,
        ValidStringCharacters,
        MinStringLength,
        MaxStringLength));
  }

  public DirectoryInfo Folder { get; }

  public FileInfo GenerateFile(int lineCount)
  {
    var file = new FileInfo(Path.Combine(Folder.FullName, $"file{Guid.NewGuid():N}.txt"));
    _generator.GenerateFile(file, lineCount);
    return file;
  }

  public void SetLinesGeneratingParameters(
    int minNumber = MinNumber,
    int maxNumber = MaxNumber,
    int minStringLength = MinStringLength,
    int maxStringLength = MaxStringLength,
    string validStringCharacters = ValidStringCharacters)
  {
    _generator = new TextFileGenerator(
      new NumberedStringLineGenerator(
        minNumber,
        maxNumber,
        validStringCharacters,
        minStringLength,
        maxStringLength));
  }

  public void Dispose()
  {
    if (Folder.Exists) Folder.Delete(recursive: true);
  }

  private TextFileGenerator _generator;

  private const int MinStringLength = 20;
  private const int MaxStringLength = 100;
  private const string ValidStringCharacters = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ";
  private const int MinNumber = 0;
  private const int MaxNumber = 10000;
}
