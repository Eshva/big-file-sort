namespace BigFile.Sorter.Stage2.Domain;

public class NumberedLineComparer : IComparer<string>
{
  public int Compare(string? left, string? right)
  {
    var isLeftEmpty = left is null || string.IsNullOrWhiteSpace(left);
    var isRightEmpty = right is null || string.IsNullOrWhiteSpace(right);
    if (isLeftEmpty && isRightEmpty) return Equal;
    if (isLeftEmpty && !isRightEmpty) return LeftLessThanRight;
    if (!isLeftEmpty && isRightEmpty) return LeftGreaterThanRight;

    var leftDotIndex = left!.IndexOf(value: '.');
    if (leftDotIndex == -1) throw new ArgumentException("No dot in line.", nameof(left));
    if (leftDotIndex == 0) throw new ArgumentException("No number in line.", nameof(left));
    var leftSpaceIndex = left.IndexOf(value: ' ');
    if (leftSpaceIndex == -1 || leftSpaceIndex != leftDotIndex + 1)
      throw new ArgumentException("No space after dot in line.", nameof(left));
    var leftStringStart = leftDotIndex + 2;
    if (left.Length <= leftStringStart) throw new ArgumentException("No string part in line.", nameof(left));

    var rightDotIndex = right!.IndexOf(value: '.');
    if (rightDotIndex == -1) throw new ArgumentException("No dot in line.", nameof(right));
    if (rightDotIndex == 0) throw new ArgumentException("No number in line.", nameof(right));
    var rightSpaceIndex = right.IndexOf(value: ' ');
    if (rightSpaceIndex == -1 || rightSpaceIndex != rightDotIndex + 1)
      throw new ArgumentException("No space after dot in line.", nameof(right));
    var rightStringStart = rightDotIndex + 2;
    if (right.Length <= rightStringStart) throw new ArgumentException("No string part in line.", nameof(right));

    var leftStingPartSpan = left.AsSpan(leftStringStart);
    var rightStringPartSpan = right.AsSpan(rightStringStart);

    var stringPartComparisionResult = leftStingPartSpan.CompareTo(rightStringPartSpan, StringComparison.Ordinal);
    if (stringPartComparisionResult != 0) return stringPartComparisionResult;

    if (!int.TryParse(left.AsSpan(start: 0, leftDotIndex), out var leftNumber))
      throw new ArgumentException("Line not prefixed with number.", nameof(left));
    if (!int.TryParse(right.AsSpan(start: 0, rightDotIndex), out var rightNumber))
      throw new ArgumentException("Line not prefixed with number.", nameof(right));

    return leftNumber.CompareTo(rightNumber);
  }

  private const int Equal = 0;
  private const int LeftLessThanRight = -1;
  private const int LeftGreaterThanRight = 1;
}
