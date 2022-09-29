namespace BigFile.Sorter.Stage4.Domain;

internal class LinesMinHeapSorter : IBufferSorter
{
  public LinesMinHeapSorter(IStringSpanComparer comparer)
  {
    _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
  }

  public IReadOnlyList<BufferPosition> Sort(ReadOnlySpan<char> buffer)
  {
    var lines = FindLinesInBuffer(buffer);

    BuildHeap(lines, buffer);

    for (var lastIndex = lines.Count - 1; lastIndex >= 0; lastIndex--)
    {
      (lines[index: 0], lines[lastIndex]) = (lines[lastIndex], lines[index: 0]);
      Heapify(
        lines,
        buffer,
        rootIndex: 0,
        lastIndex);
    }

    return lines;
  }

  private void BuildHeap(List<BufferPosition> lines, ReadOnlySpan<char> buffer)
  {
    for (var lineIndex = lines.Count / 2 - 1; lineIndex >= 0; lineIndex--)
    {
      Heapify(
        lines,
        buffer,
        lineIndex,
        lines.Count);
    }
  }

  private void Heapify(
    IList<BufferPosition> lines,
    ReadOnlySpan<char> buffer,
    int rootIndex,
    int endIndex)
  {
    while (rootIndex < endIndex)
    {
      var smallestIndex = rootIndex;
      var smallest = lines[smallestIndex];

      var leftIndex = rootIndex * 2 + 1;
      if (leftIndex < endIndex)
      {
        var left = lines[leftIndex];
        if (_comparer.Compare(
              buffer.Slice(left.Start, left.Length),
              buffer.Slice(smallest.Start, smallest.Length)) > 0)
        {
          smallestIndex = leftIndex;
          smallest = lines[smallestIndex];
        }
      }

      var rightIndex = leftIndex + 1;
      if (rightIndex < endIndex)
      {
        var right = lines[rightIndex];
        if (_comparer.Compare(
              buffer.Slice(right.Start, right.Length),
              buffer.Slice(smallest.Start, smallest.Length)) > 0)
          smallestIndex = rightIndex;
      }

      if (smallestIndex == rootIndex) return;

      (lines[rootIndex], lines[smallestIndex]) = (lines[smallestIndex], lines[rootIndex]);
      rootIndex = smallestIndex;
    }
  }

  private static List<BufferPosition> FindLinesInBuffer(ReadOnlySpan<char> buffer)
  {
    var bufferPositions = new List<BufferPosition>();
    foreach (var lineEntry in buffer.GetLinesEnumerator())
    {
      bufferPositions.Add(new BufferPosition(lineEntry.Start, lineEntry.Length));
    }

    return bufferPositions;
  }

  private readonly IStringSpanComparer _comparer;
}
