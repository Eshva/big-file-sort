#region Usings

using BenchmarkDotNet.Attributes;
using BigFile.Sorter.Stage3.Domain;

#endregion

namespace BigFile.Sorter.Benchmarks;

[MemoryDiagnoser]
public class EnumerateLinesInBuffer
{
  [Benchmark(Baseline = true)]
  public void UsingSpans()
  {
    // ReSharper disable once UnusedVariable
    foreach (var line in Buffer.AsSpan().GetLinesEnumerator()) { }
  }

  [Benchmark]
  public void UsingReadLine()
  {
    var reader = new StringReader(Buffer);
    while (reader.ReadLine() != null) { }
  }

  private const string Buffer =
    "aaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\naaa\r\n";
}
