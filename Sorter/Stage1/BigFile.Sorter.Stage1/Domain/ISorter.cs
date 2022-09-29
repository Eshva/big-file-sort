namespace BigFile.Sorter.Stage1.Domain;

internal interface ISorter<TItem> where TItem : IComparable<TItem>
{
  void Sort(List<TItem> items);
}
