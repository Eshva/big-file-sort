#region Usings

using System.Diagnostics.CodeAnalysis;
using CommandLine;

#endregion

namespace BigFile.Generator;

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
    HelpText = "Output file name.",
    Default = "")]
  public string FileName { get; set; }

  [Option(
    longName: "lines",
    shortName: 'l',
    Required = true,
    HelpText = "Number of generating lines.")]
  public int LineCount { get; set; }
}
