namespace BigFile.Sorter.Stage3.Domain;

internal interface IStringSpanComparer
{
  int Compare(ReadOnlySpan<char> left, ReadOnlySpan<char> right);
}
