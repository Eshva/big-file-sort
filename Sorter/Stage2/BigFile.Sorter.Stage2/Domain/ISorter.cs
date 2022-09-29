namespace BigFile.Sorter.Stage2.Domain;

internal interface ISorter<TItem> where TItem : IComparable<TItem>
{
  void Sort(List<TItem> items);
}
