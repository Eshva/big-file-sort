namespace BigFile.Sorter.Stage4.Domain;

internal interface IStringSpanComparer
{
  int Compare(ReadOnlySpan<char> left, ReadOnlySpan<char> right);
}
