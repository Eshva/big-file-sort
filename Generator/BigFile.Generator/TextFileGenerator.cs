#region Usings

using System.Text;

#endregion

namespace BigFile.Generator;

internal class TextFileGenerator
{
  public TextFileGenerator(ILineGenerator lineGenerator)
  {
    _lineGenerator = lineGenerator ?? throw new ArgumentNullException(nameof(lineGenerator));
  }

  public void GenerateFile(FileInfo file, int lineCount)
  {
    if (file is null) throw new ArgumentNullException(nameof(file));
    if (lineCount <= 0) throw new ArgumentOutOfRangeException(nameof(lineCount));

    if (file.Directory is not null && !file.Directory.Exists) file.Directory.Create();
    if (file.Exists) file.Delete();

    using var writer = new StreamWriter(file.OpenWrite(), Encoding.Latin1);
    for (var line = 0; line < lineCount; line++)
    {
      writer.WriteLine(_lineGenerator.Generate());
    }
  }

  private readonly ILineGenerator _lineGenerator;
}
