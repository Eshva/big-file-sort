#region Usings

using BigFile.Common;
using BigFile.Sorter.Stage3;
using BigFile.Sorter.Stage3.Application;
using CommandLine;

#endregion

var commandLineOptions = Parser.Default.ParseArguments<CommandLineOptions>(args);

if (commandLineOptions.Errors.Any()) return 1;

var file = new FileInfo(commandLineOptions.Value.FileName);

var options = new ExternalMergeFileSorterOptions3(
  new DirectoryInfo(commandLineOptions.Value.WorkingFolder),
  commandLineOptions.Value.MaxTempFiles,
  commandLineOptions.Value.KWayMergeFactor,
  MemorySize.From(commandLineOptions.Value.PartsBuilderBufferSize),
  MemorySize.From(commandLineOptions.Value.PartsMergerBufferSize));

var partsBuilderFactory = new PartsBuilderFactory3(
  options.WorkingFolder,
  options.PartsBuilderBufferSize,
  options.PartFileCount);
var partsMergerFactory = new PartsMergerFactory3(
  options.WorkingFolder,
  file,
  options.KWayMergeFactor,
  options.PartsMergerBufferSize);
var sorter = new ExternalMergeFileSorter3(
  partsBuilderFactory,
  partsMergerFactory);

try
{
  sorter.Sort(file, file);
}
catch (Exception exception)
{
  Console.Error.WriteLine($"Can't sort the file '{file}'.");
  Console.Error.WriteLine(exception.ToString());
  return 1;
}

return 0;
