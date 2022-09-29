namespace BigFile.Sorter.Stage4.Application;

internal interface IPartsMerger
{
  FileInfo MergeParts(IReadOnlyList<FileInfo> partFiles, FileInfo? mergedFile);
}
