#region Usings

using System.Text;

#endregion

namespace BigFile.Common.Tests.Unit;

public abstract class NumberedLinesFileEnumeratorTests : IDisposable
{
  public void Dispose() => _file?.Delete();

  protected NumberedLinesFileEnumerator CreateEnumerator(ReadOnlySpan<char> fileContent, MemorySize? bufferSize = null)
  {
    bufferSize ??= MemorySize.FromKilobytes(kilobytes: 4);

    _file = new FileInfo(Path.GetTempFileName());
    var writer = new StreamWriter(_file.OpenWrite(), Encoding.ASCII);
    writer.Write(fileContent);
    writer.Dispose();
    return new NumberedLinesFileEnumerator(_file, bufferSize.Value);
  }

  private FileInfo? _file;
}
