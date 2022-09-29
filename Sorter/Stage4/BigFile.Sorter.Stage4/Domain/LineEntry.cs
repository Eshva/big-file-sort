namespace BigFile.Sorter.Stage4.Domain;

public readonly ref struct LineEntry
{
  public LineEntry(int start, int length)
  {
    Start = start;
    Length = length;
  }

  public int Start { get; }

  public int Length { get; }
}
