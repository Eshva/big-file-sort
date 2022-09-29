namespace BigFile.Sorter.Stage3.Domain;

public ref struct LinesEnumerator
{
  public LinesEnumerator(ReadOnlySpan<char> buffer)
  {
    _buffer = buffer;
    _bufferRemaining = _buffer;
    Current = default;
  }

  public LinesEnumerator GetEnumerator() => this;

  public bool MoveNext()
  {
    _bufferRemaining = _buffer[_currentStart..];
    if (_bufferRemaining.Length == 0) return false;

    var endOfLineIndex = _bufferRemaining.IndexOfAny(value0: '\r', value1: '\n');
    if (endOfLineIndex == -1) return false;

    if (endOfLineIndex < _bufferRemaining.Length - 1 && _bufferRemaining[endOfLineIndex] == '\r')
    {
      // Try to consume the '\n' associated to the '\r'
      var next = _bufferRemaining[endOfLineIndex + 1];
      if (next == '\n')
      {
        Current = new LineEntry(_currentStart, endOfLineIndex + 2);
        _currentStart += endOfLineIndex + 2;
        return true;
      }
    }

    Current = new LineEntry(_currentStart, endOfLineIndex + 1);
    _currentStart += endOfLineIndex + 1;
    return true;
  }

  public LineEntry Current { get; private set; }

  private readonly ReadOnlySpan<char> _buffer;
  private int _currentStart = 0;
  private ReadOnlySpan<char> _bufferRemaining;
}
