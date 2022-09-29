namespace BigFile.Sorter.Stage1.Application;

internal interface IPartsMerger
{
  FileInfo MergeParts(IReadOnlyList<FileInfo> partFiles, FileInfo? mergedFile);
}
