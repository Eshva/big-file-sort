namespace BigFile.Sorter.Stage3.Application;

internal interface IPartsBuilderFactory
{
  IPartBuilder Create(FileInfo inputFile);
}
