namespace BigFile.Sorter.Stage4.Domain;

public static class StringExtensions
{
  public static LinesEnumerator GetLinesEnumerator(this ReadOnlySpan<char> bufferedLines) => new(bufferedLines);
}
