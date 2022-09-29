#region Usings

using BigFile.Common;
using BigFile.Sorter.Stage2;
using BigFile.Sorter.Stage2.Application;
using CommandLine;

#endregion

var commandLineOptions = Parser.Default.ParseArguments<CommandLineOptions>(args);

if (commandLineOptions.Errors.Any()) return 1;

var file = new FileInfo(commandLineOptions.Value.FileName);

var options = new ExternalMergeFileSorterOptions2(
  MemorySize.From(commandLineOptions.Value.MaxMemoryUsage),
  new DirectoryInfo(commandLineOptions.Value.WorkingFolder),
  commandLineOptions.Value.MaxTempFiles,
  commandLineOptions.Value.KWayMergeFactor);

var partsBuilderFactory = new PartsBuilderFactory2(
  options.WorkingFolder,
  options.LinesPerPart,
  options.PartFileCount);
var partsMergerFactory = new PartsMergerFactory2(
  options.WorkingFolder,
  options.KWayMergeFactor,
  file);
var sorter = new ExternalMergeFileSorter2(
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
