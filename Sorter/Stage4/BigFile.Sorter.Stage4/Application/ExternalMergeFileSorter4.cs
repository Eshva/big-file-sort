namespace BigFile.Sorter.Stage4.Application;

internal class ExternalMergeFileSorter4
{
  public ExternalMergeFileSorter4(
    IPartsBuilderFactory partsBuilderFactory,
    IPartsMergerFactory partsMergerFactory)
  {
    _partsBuilderFactory = partsBuilderFactory ?? throw new ArgumentNullException(nameof(partsBuilderFactory));
    _partsMergerFactory = partsMergerFactory ?? throw new ArgumentNullException(nameof(partsMergerFactory));
  }

  public void Sort(FileInfo inputFile, FileInfo outputFile)
  {
    if (inputFile == null) throw new ArgumentNullException(nameof(inputFile));
    if (outputFile == null) throw new ArgumentNullException(nameof(outputFile));

    // IMPORTANT: File.Exists used because FileInfo.Exists seams like caches the result.
    if (!File.Exists(inputFile.FullName))
      throw new ArgumentException($"Input file '{inputFile.FullName}' doesn't exist.", nameof(inputFile));

    FileInfo? mergedFile = null;
    using (var inputFileReader = new StreamReader(inputFile.OpenRead()))
    {
      var partsBuilder = _partsBuilderFactory.Create(inputFile);
      var partsMerger = _partsMergerFactory.Create();

      while (!inputFileReader.EndOfStream)
      {
        var partFiles = new List<FileInfo>(capacity: 0);
        if (mergedFile is not null) partFiles.Add(mergedFile);

        try
        {
          partFiles = partsBuilder.BuildSortedParts(inputFileReader);
          mergedFile = partsMerger.MergeParts(partFiles, mergedFile);
        }
        catch
        {
          if (mergedFile is not null && mergedFile.Exists) mergedFile.Delete();
          throw;
        }
        finally
        {
          foreach (var partFile in partFiles.Where(partFile => partFile.Exists))
          {
            partFile.Delete();
          }
        }
      }
    }

    if (mergedFile is not null)
    {
      mergedFile.MoveTo(outputFile.FullName, overwrite: true);
      return;
    }

    if (!outputFile.FullName.Equals(inputFile.FullName)) inputFile.MoveTo(outputFile.FullName, overwrite: true);
  }

  private readonly IPartsBuilderFactory _partsBuilderFactory;
  private readonly IPartsMergerFactory _partsMergerFactory;
}
