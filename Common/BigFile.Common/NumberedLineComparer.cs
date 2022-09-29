namespace BigFile.Common;

public class NumberedLineComparer
{
  public int Compare(NumberedLine left, NumberedLine right)
  {
    var lineStringComparisionResult = left.LineString.CompareTo(right.LineString, StringComparison.Ordinal);
    return lineStringComparisionResult != 0
      ? lineStringComparisionResult
      : int.Parse(left.LineNumber).CompareTo(int.Parse(right.LineNumber));
  }
}
