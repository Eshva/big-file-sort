#region Usings

using BenchmarkDotNet.Attributes;

#endregion

namespace BigFile.Sorter.Benchmarks;

[MemoryDiagnoser]
public class AllocateLessFileReadWrite
{
  public AllocateLessFileReadWrite()
  {
    _folder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "AllocateLessFileReadWrite"));
    _folder.Create();
    var outputFile = new FileInfo(Path.Combine(_folder.FullName, Guid.NewGuid().ToString("N")));
    _outputFileWriter = new StreamWriter(outputFile.OpenWrite());
  }

  [Benchmark]
  public void WriteUsingSpanFromBuffer()
  {
    _outputFileWriter.Write(_buffer.AsSpan().Slice(start: 0, length: 5));
    _outputFileWriter.Write(_buffer.AsSpan().Slice(start: 5, length: 5));
    _outputFileWriter.Write(_buffer.AsSpan().Slice(start: 10, length: 5));
  }

  [GlobalCleanup]
  public void GlobalCleanup()
  {
    _outputFileWriter.Dispose();
    if (_folder.Exists) _folder.Delete(recursive: true);
  }

  private readonly char[] _buffer = "aaa\r\nbbb\r\nccc\r\n".ToCharArray();
  private readonly DirectoryInfo _folder;
  private readonly StreamWriter _outputFileWriter;
}
