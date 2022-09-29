#region Usings

using System.Text;
using BigFile.Common;

#endregion

namespace BigFile.Sorter.Stage2.Domain;

internal class LinesPerPartCalculator
{
  public int Calculate(
    MemorySize maxMemoryUsage,
    int partFileCount,
    int maxLineLengthInChars = MaxLineLengthInChars)
  {
    var maxStringLengthInBytes = Encoding.Unicode.GetMaxByteCount(maxLineLengthInChars);
    return (int)(maxMemoryUsage / maxStringLengthInBytes / partFileCount);
  }

  // HACK: Taken from test requirements. It should be taken from command line for real useful application.
  private const int MaxStringLengthInChars = 100;
  private const int DelimiterLengthInChars = 2;
  private const int MaxNumberLengthInChars = 5;
  private const int MaxLineLengthInChars = MaxNumberLengthInChars + DelimiterLengthInChars + MaxStringLengthInChars;
}
