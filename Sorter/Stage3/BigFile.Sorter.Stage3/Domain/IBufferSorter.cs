#region Usings

#endregion

namespace BigFile.Sorter.Stage3.Domain;

internal interface IBufferSorter
{
  BufferPosition[] Sort(ReadOnlySpan<char> buffer);
}
