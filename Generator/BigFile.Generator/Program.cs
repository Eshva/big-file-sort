#region Usings

using BigFile.Generator;
using CommandLine;

#endregion

var options = Parser.Default.ParseArguments<CommandLineOptions>(args);

if (options.Errors.Any()) return 1;

var file = new FileInfo(options.Value.FileName);
var lineCount = options.Value.LineCount;
var lineGenerator = new NumberedStringLineGenerator(
  minNumber: 0,
  maxNumber: 10000,
  @"abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ",
  minStringLength: 20,
  maxStringLength: 100);
var fileGenerator = new TextFileGenerator(lineGenerator);
fileGenerator.GenerateFile(file, lineCount);

Console.WriteLine($"{lineCount} lines were generated in file '{file.FullName}'.");
return 0;
