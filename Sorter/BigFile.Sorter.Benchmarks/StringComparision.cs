#region Usings

using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

#endregion

namespace BigFile.Sorter.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
public class StringComparision
{
  [Benchmark(Baseline = true)]
  public void CompareReadOnlySpans()
  {
    var one = Str.AsSpan(start: 0, length: 10);
    var two = Str.AsSpan(start: 13, length: 10);
    one.CompareTo(two, StringComparison.Ordinal);
  }

  [Benchmark]
  public void CompareUsingStringConstructor()
  {
    var one = Str.AsSpan(start: 0, length: 10);
    var two = Str.AsSpan(start: 13, length: 10);
    string.Compare(
      new string(one),
      new string(two),
      StringComparison.Ordinal);
  }

  [Benchmark]
  public void CompareUsingToString()
  {
    var one = Str.AsSpan(start: 0, length: 10);
    var two = Str.AsSpan(start: 13, length: 10);
    string.Compare(
      one.ToString(),
      two.ToString(),
      StringComparison.Ordinal);
  }

  private const string Str = "Some string. Просто строка.";
}
