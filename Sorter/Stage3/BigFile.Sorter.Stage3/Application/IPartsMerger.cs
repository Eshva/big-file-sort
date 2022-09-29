namespace BigFile.Sorter.Stage3.Application;

internal interface IPartsMerger
{
  FileInfo MergeParts(IReadOnlyList<FileInfo> partFiles, FileInfo? mergedFile);
}
