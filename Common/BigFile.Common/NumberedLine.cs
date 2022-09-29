namespace BigFile.Common;

public readonly ref struct NumberedLine
{
  public ReadOnlySpan<char> EntireLine { get; init; }

  public ReadOnlySpan<char> LineNumber { get; init; }

  public ReadOnlySpan<char> LineString { get; init; }
}
