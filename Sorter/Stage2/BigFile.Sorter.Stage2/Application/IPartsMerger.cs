namespace BigFile.Sorter.Stage2.Application;

internal interface IPartsMerger
{
  FileInfo MergeParts(IReadOnlyList<FileInfo> partFiles, FileInfo? mergedFile);
}
