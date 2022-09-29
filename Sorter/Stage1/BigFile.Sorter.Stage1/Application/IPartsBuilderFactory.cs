namespace BigFile.Sorter.Stage1.Application;

internal interface IPartsBuilderFactory
{
  IPartBuilder Create(FileInfo inputFile);
}
