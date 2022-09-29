namespace BigFile.Sorter.Stage4.Application;

internal interface IPartBuilder
{
  List<FileInfo> BuildSortedParts(StreamReader inputFileReader);
}
