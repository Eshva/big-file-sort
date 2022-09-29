#region Usings

using System.Buffers;

#endregion

namespace BigFile.Common;

public class NumberedLinesFileEnumerator : IDisposable
{
  public NumberedLinesFileEnumerator(FileInfo file, MemorySize bufferSize)
  {
    if (file == null) throw new ArgumentNullException(nameof(file));
    if (!file.Exists) throw new ArgumentException($"File {file.FullName} to enumerate doesn't exist.", nameof(file));
    if (bufferSize < MemorySize.FromBytes(bytes: 128))
      throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size should be at least 128 bytes.");
    if (!IsBufferSizeValid(bufferSize)) throw new ArgumentException("Buffer size should have value equal power of 2.", nameof(bufferSize));
    _memoryOwner = MemoryPool<char>.Shared.Rent((int)bufferSize);
    _buffer = _memoryOwner.Memory;
    _reader = new StreamReader(file.OpenRead(), bufferSize: (int)bufferSize);
  }

  public NumberedLine Current
  {
    get
    {
      if (!_isInitialized) throw new InvalidOperationException("MoveNext() must be called before reading the Current property.");
      return new NumberedLine
      {
        EntireLine = _buffer.Span.Slice(_lineStart, _lineLength),
        LineNumber = _buffer.Span.Slice(_numberStart, _numberLength),
        LineString = _buffer.Span.Slice(_stringStart, _stringLength)
      };
    }
  }

  public bool MoveNext()
  {
    if (!_isInitialized && !ReadBuffer()) return false;
    _isInitialized = true;

    if (!TryGetNextLine(out var line, out var lineEndingLength)) return false;
    FindLineParts(line, lineEndingLength);
    return true;
  }

  public void Dispose()
  {
    _reader.Dispose();
    _memoryOwner.Dispose();
  }

  private bool TryGetNextLine(out ReadOnlySpan<char> nextLine, out int lineEndingLength)
  {
    nextLine = ReadOnlySpan<char>.Empty;
    lineEndingLength = 0;
    do
    {
      var remaining = _buffer.Span.Slice(_remainingStart, _remainingLength);
      _lineStart = _remainingStart;

      for (var currentCharIndex = 0; currentCharIndex < _remainingLength; currentCharIndex++)
      {
        var currentChar = remaining[currentCharIndex];
        var nextCharIndex = currentCharIndex + 1;
        var isBufferEndReached = nextCharIndex == _remainingLength;
        if (isBufferEndReached && !_reader.EndOfStream)
        {
          if (!ReadBuffer(_remainingLength)) return false;

          break;
        }

        var nextChar = isBufferEndReached ? '\0' : remaining[nextCharIndex];

        switch (currentChar)
        {
          case '\r' when !isBufferEndReached:
            lineEndingLength = nextChar == '\n' ? 2 : 1;
            break;
          case '\n' when !isBufferEndReached:
            lineEndingLength = nextChar == '\r' ? 2 : 1;
            break;
          case '\r' or '\n':
            lineEndingLength = 1;
            break;
          default:
          {
            if (isBufferEndReached && _reader.EndOfStream) return false;

            continue;
          }
        }

        _lineLength = currentCharIndex + lineEndingLength;
        _remainingStart += _lineLength;
        _remainingLength -= _lineLength;

        nextLine = _buffer.Span.Slice(_lineStart, _lineLength);
        return true;
      }

      if (_remainingLength == 0 && !_reader.EndOfStream && !ReadBuffer(_remainingLength)) return false;
    } while (_remainingLength > 0);

    return false;
  }

  private void FindLineParts(ReadOnlySpan<char> line, int lineEndingLength)
  {
    const int notFound = -1;
    const string separator = ". ";
    var separatorIndex = line.IndexOf(separator);
    if (separatorIndex == notFound) throw new Exception("Number and string parts separator of a line isn't found.");

    _numberStart = _lineStart;
    _numberLength = separatorIndex;
    if (_numberLength == 0 || !int.TryParse(_buffer.Span.Slice(_numberStart, _numberLength), out _))
      throw new Exception("Number part of a line isn't an integer.");

    _stringStart = _numberStart + _numberLength + separator.Length;
    _stringLength = line.Length - _numberLength - separator.Length - lineEndingLength;
    if (_stringLength == 0) throw new Exception("String part of a line is not found.");
  }

  private bool ReadBuffer(int seekBackLength = 0)
  {
    if (_reader.EndOfStream) return false;

    if (seekBackLength != 0)
    {
      _reader.DiscardBufferedData();
      _reader.BaseStream.Position = _previousPosition - seekBackLength;
    }

    _remainingStart = 0;
    _remainingLength = _reader.Read(_buffer.Span);
    _previousPosition = _reader.BaseStream.Position;
    return true;
  }

  private static bool IsBufferSizeValid(MemorySize bufferSize)
  {
    long size = bufferSize;
    return (size & (size - 1)) == 0;
  }

  private readonly Memory<char> _buffer;
  private readonly IMemoryOwner<char> _memoryOwner;
  private readonly StreamReader _reader;
  private bool _isInitialized;
  private int _lineLength;
  private int _lineStart;
  private int _numberLength;
  private int _numberStart;
  private long _previousPosition;
  private int _remainingLength;
  private int _remainingStart;
  private int _stringLength;
  private int _stringStart;
}
