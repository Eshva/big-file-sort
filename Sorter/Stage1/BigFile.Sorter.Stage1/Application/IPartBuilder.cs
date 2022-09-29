namespace BigFile.Sorter.Stage1.Application;

internal interface IPartBuilder
{
  List<FileInfo> BuildSortedParts(StreamReader inputFileReader);
}
