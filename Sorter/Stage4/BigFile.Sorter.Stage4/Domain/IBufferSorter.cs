#region Usings

#endregion

namespace BigFile.Sorter.Stage4.Domain;

internal interface IBufferSorter
{
  IReadOnlyList<BufferPosition> Sort(ReadOnlySpan<char> buffer);
}
