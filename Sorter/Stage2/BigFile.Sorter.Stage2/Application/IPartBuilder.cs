namespace BigFile.Sorter.Stage2.Application;

internal interface IPartBuilder
{
  List<FileInfo> BuildSortedParts(StreamReader inputFileReader);
}
