namespace BigFile.Sorter.Stage4.Application;

internal interface IPartsBuilderFactory
{
  IPartBuilder Create(FileInfo inputFile);
}
