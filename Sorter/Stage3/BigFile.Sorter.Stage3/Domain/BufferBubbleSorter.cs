namespace BigFile.Sorter.Stage3.Domain;

internal class BufferBubbleSorter : IBufferSorter
{
  public BufferBubbleSorter(IStringSpanComparer comparer)
  {
    _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
  }

  public BufferPosition[] Sort(ReadOnlySpan<char> buffer)
  {
    var lines = FindLinesInBuffer(buffer);
    for (var leftIndex = 0; leftIndex < lines.Count; leftIndex++)
    {
      for (var rightIndex = leftIndex + 1; rightIndex < lines.Count; rightIndex++)
      {
        var left = lines[leftIndex];
        var right = lines[rightIndex];
        if (_comparer.Compare(buffer.Slice(left.Start, left.Length), buffer.Slice(right.Start, right.Length)) > 0)
          (lines[rightIndex], lines[leftIndex]) = (lines[leftIndex], lines[rightIndex]);
      }
    }

    return lines.ToArray();
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
