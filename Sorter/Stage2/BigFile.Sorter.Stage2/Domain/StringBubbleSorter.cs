namespace BigFile.Sorter.Stage2.Domain;

internal class StringBubbleSorter : ISorter<string>
{
  public StringBubbleSorter(IComparer<string> comparer)
  {
    _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
  }

  public void Sort(List<string> items)
  {
    if (items == null) throw new ArgumentNullException(nameof(items));

    if (items.Count < 2) return;

    for (var leftIndex = 0; leftIndex < items.Count; leftIndex++)
    {
      for (var rightIndex = leftIndex + 1; rightIndex < items.Count; rightIndex++)
      {
        if (_comparer.Compare(items[leftIndex], items[rightIndex]) > 0)
          (items[rightIndex], items[leftIndex]) = (items[leftIndex], items[rightIndex]);
      }
    }
  }

  private readonly IComparer<string> _comparer;
}
