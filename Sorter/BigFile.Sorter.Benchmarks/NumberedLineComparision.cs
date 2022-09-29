#region Usings

using BenchmarkDotNet.Attributes;
using BigFile.Sorter.Stage1.Domain;
using BigFile.Sorter.Stage2.Domain;

#endregion

namespace BigFile.Sorter.Benchmarks;

[MemoryDiagnoser]
public class NumberedLineComparision
{
  [Benchmark]
  public void CompareUsingSpan()
  {
    _result = _comparer.Compare(StringOne, StringTwo);
  }

  [Benchmark]
  public void CompareUsingNumberedLineClass()
  {
    var one = new NumberedLine(StringOne);
    var two = new NumberedLine(StringTwo);
    _result = one.CompareTo(two);
  }

  public override string ToString() => _result.ToString();

  private readonly NumberedLineComparer _comparer = new();
  private int _result;

  private const string StringOne = "123. aaa";
  private const string StringTwo = "124. aaa";
}
