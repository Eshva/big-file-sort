namespace BigFile.Sorter.Stage3.Application;

internal interface IPartBuilder
{
  List<FileInfo> BuildSortedParts(StreamReader inputFileReader);
}
