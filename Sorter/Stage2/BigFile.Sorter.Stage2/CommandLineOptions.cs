#region Usings

using System.Diagnostics.CodeAnalysis;
using CommandLine;

#endregion

namespace BigFile.Sorter.Stage2;

#pragma warning disable CS8618
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class CommandLineOptions
{
  [Option(
    longName: "file",
    shortName: 'f',
    Required = true,
    HelpText = "File to sort.",
    Default = "")]
  public string FileName { get; set; }

  [Option(
    longName: "working-folder",
    shortName: 'w',
    Required = false,
    HelpText = "Working folder for temporary files.")]
  public string WorkingFolder { get; set; } = Path.GetTempPath();

  [Option(
    longName: "max-memory",
    shortName: 'm',
    Required = false,
    HelpText = "Maximum memory amount used by the application. Value format: <number><unit>. Unit is one of: ki, mi, gi.",
    Default = "1mi")]
  public string MaxMemoryUsage { get; set; } = "1mi";

  [Option(
    longName: "temp-files",
    shortName: 't',
    Required = false,
    HelpText = "Maximum temporary files used by the application.",
    Default = "100")]
  public int MaxTempFiles { get; set; } = 100;

  [Option(
    longName: "k-way-factor",
    shortName: 'k',
    Required = false,
    HelpText = "Number of merging sorted parts.",
    Default = "10")]
  public int KWayMergeFactor { get; set; } = 10;
}
