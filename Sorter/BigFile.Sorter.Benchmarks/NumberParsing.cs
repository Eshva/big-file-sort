#region Usings

using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

#endregion

namespace BigFile.Sorter.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
public class NumberParsing
{
  [Benchmark(Baseline = true)]
  public void ParseUsingReadOnlySpan()
  {
    var span = NumberedString.AsSpan(start: 0, length: 4);
    int.Parse(span);
  }

  [Benchmark]
  public void ParseUsingSubstring()
  {
    var substring = NumberedString.Substring(startIndex: 0, length: 4);
    int.Parse(substring);
  }

  [Benchmark]
  public void ParseUsingRangeIndexer()
  {
    var substring = NumberedString[..4];
    int.Parse(substring);
  }

  private const string NumberedString = "1234. Some string";
}
