#region Usings

using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BigFile.Common;
using BigFile.Sorter.Stage1.Application;
using BigFile.Sorter.Stage2.Application;
using BigFile.Sorter.Stage3.Application;
using BigFile.Sorter.Stage4.Application;
using BigFile.Tests.Common;

#endregion

namespace BigFile.Sorter.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class SortBenchmark
{
#pragma warning disable CS8618
  public SortBenchmark()
  {
    _filesGeneratorFixture = new IndependentFilesGeneratorFixture();
  }

  [Params("100ki")]
  public string MaxMemoryUsage { get; set; }

  [Params(12)]
  public int MaxTempFiles { get; set; }

  [Params(5, 10)]
  public int KWayMergeFactor { get; set; }

  [Params("16ki")]
  public string PartsBuilderBufferSize { get; set; }

  [Params("16ki")]
  public string PartsMergerBufferSize { get; set; }

  [GlobalSetup]
  public void GlobalSetup()
  {
    _inputFile = _filesGeneratorFixture.GenerateFile(lineCount: 10_000);
  }

  [GlobalCleanup]
  public void GlobalCleanup()
  {
    _filesGeneratorFixture.Folder.Delete(recursive: true);
  }

  [Benchmark(Baseline = true, Description = "Со вспомогательным типом строки")]
  public void ExternalMergeFileSorterWithNumberedLineClass()
  {
    var outputFilePath = Path.Combine(_filesGeneratorFixture.Folder.FullName, $"{Guid.NewGuid():N}.txt");
    var outputFile = new FileInfo(outputFilePath);
    var options = new ExternalMergeFileSorterOptions1(
      MemorySize.From(MaxMemoryUsage),
      _filesGeneratorFixture.Folder,
      MaxTempFiles,
      KWayMergeFactor);
    var sorter = new ExternalMergeFileSorter1(
      new PartsBuilderFactory1(
        options.WorkingFolder,
        options.LinesPerPart,
        options.PartFileCount),
      new PartsMergerFactory1(
        options.WorkingFolder,
        options.KWayMergeFactor,
        _inputFile));

    try
    {
      sorter.Sort(_inputFile, outputFile);
    }
    finally
    {
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Benchmark(Description = "Сравнение строк на span")]
  public void ExternalMergeFileSorterWithSpan()
  {
    var outputFilePath = Path.Combine(_filesGeneratorFixture.Folder.FullName, $"{Guid.NewGuid():N}.txt");
    var outputFile = new FileInfo(outputFilePath);
    var options = new ExternalMergeFileSorterOptions2(
      MemorySize.From(MaxMemoryUsage),
      _filesGeneratorFixture.Folder,
      MaxTempFiles,
      KWayMergeFactor);
    var sorter = new ExternalMergeFileSorter2(
      new PartsBuilderFactory2(
        options.WorkingFolder,
        options.LinesPerPart,
        options.PartFileCount),
      new PartsMergerFactory2(
        options.WorkingFolder,
        options.KWayMergeFactor,
        _inputFile));

    try
    {
      sorter.Sort(_inputFile, outputFile);
    }
    finally
    {
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Benchmark(Description = "Сравнение в буферах")]
  public void ExternalMergeFileSorterWithBuffers()
  {
    var outputFilePath = Path.Combine(_filesGeneratorFixture.Folder.FullName, $"{Guid.NewGuid():N}.txt");
    var outputFile = new FileInfo(outputFilePath);
    var options = new ExternalMergeFileSorterOptions3(
      _filesGeneratorFixture.Folder,
      MaxTempFiles,
      KWayMergeFactor,
      MemorySize.From(PartsBuilderBufferSize),
      MemorySize.From(PartsMergerBufferSize));

    var sorter = new ExternalMergeFileSorter3(
      new PartsBuilderFactory3(
        options.WorkingFolder,
        options.PartsBuilderBufferSize,
        options.PartFileCount),
      new PartsMergerFactory3(
        options.WorkingFolder,
        _inputFile,
        options.KWayMergeFactor,
        options.PartsMergerBufferSize));

    try
    {
      sorter.Sort(_inputFile, outputFile);
    }
    finally
    {
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  [Benchmark(Description = "Сравнение в буферах HeapSort")]
  public void ExternalMergeFileSorterWithBuffersHeapSort()
  {
    var outputFilePath = Path.Combine(_filesGeneratorFixture.Folder.FullName, $"{Guid.NewGuid():N}.txt");
    var outputFile = new FileInfo(outputFilePath);
    var options = new ExternalMergeFileSorterOptions4(
      _filesGeneratorFixture.Folder,
      MaxTempFiles,
      KWayMergeFactor,
      MemorySize.From(PartsBuilderBufferSize),
      MemorySize.From(PartsMergerBufferSize));

    var sorter = new ExternalMergeFileSorter4(
      new PartsBuilderFactory4(
        options.WorkingFolder,
        options.PartsBuilderBufferSize,
        options.PartFileCount),
      new PartsMergerFactory4(
        options.WorkingFolder,
        _inputFile,
        options.KWayMergeFactor,
        options.PartsMergerBufferSize));

    try
    {
      sorter.Sort(_inputFile, outputFile);
    }
    finally
    {
      if (outputFile.Exists) outputFile.Delete();
    }
  }

  private readonly IndependentFilesGeneratorFixture _filesGeneratorFixture;
  private FileInfo _inputFile;
}
