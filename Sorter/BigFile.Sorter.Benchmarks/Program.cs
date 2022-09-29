#region Usings

using BenchmarkDotNet.Running;
using BigFile.Sorter.Benchmarks;

#endregion

BenchmarkRunner.Run<SortBenchmark>();
// BenchmarkRunner.Run<StringComparision>();
// BenchmarkRunner.Run<NumberParsing>();
// BenchmarkRunner.Run<NumberedLineComparision>();
// BenchmarkRunner.Run<AllocateLessFileReadWrite>();
// BenchmarkRunner.Run<EnumerateLinesInBuffer>();
