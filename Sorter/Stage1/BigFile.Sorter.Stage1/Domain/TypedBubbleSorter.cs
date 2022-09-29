namespace BigFile.Sorter.Stage1.Domain;

internal class TypedBubbleSorter<TItem> : ISorter<TItem> where TItem : IComparable<TItem>
{
  public void Sort(List<TItem> items)
  {
    if (items == null) throw new ArgumentNullException(nameof(items));

    if (items.Count < 2) return;

    for (var leftIndex = 0; leftIndex < items.Count; leftIndex++)
    {
      for (var rightIndex = leftIndex + 1; rightIndex < items.Count; rightIndex++)
      {
        if (items[leftIndex].CompareTo(items[rightIndex]) > 0)
          (items[rightIndex], items[leftIndex]) = (items[leftIndex], items[rightIndex]);
      }
    }
  }
}
