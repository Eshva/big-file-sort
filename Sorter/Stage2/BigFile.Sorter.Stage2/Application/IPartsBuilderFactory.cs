namespace BigFile.Sorter.Stage2.Application;

internal interface IPartsBuilderFactory
{
  IPartBuilder Create(FileInfo inputFile);
}
