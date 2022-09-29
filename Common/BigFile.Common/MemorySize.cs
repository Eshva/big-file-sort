#region Usings

using System.Text.RegularExpressions;

#endregion

namespace BigFile.Common;

public readonly struct MemorySize : IComparable<MemorySize>, IEquatable<MemorySize>
{
  private MemorySize(long sizeInBytes)
  {
    _sizeInBytes = sizeInBytes;
  }

  public static MemorySize From(string memorySizeString)
  {
    if (string.IsNullOrWhiteSpace(memorySizeString))
      throw new ArgumentException("Value cannot be null or whitespace.", nameof(memorySizeString));

    memorySizeString = memorySizeString.Trim().ToLower();
    var match = ParsingRegex.Match(memorySizeString);
    if (!match.Success) throw new ArgumentException($"Memory size value '{memorySizeString}' can't be parsed.", nameof(memorySizeString));

    var sizeString = match.Groups["size"].Value;
    var unit = match.Groups["unit"].Value;

    var multiplier = unit switch
    {
      "ki" => Kilobyte,
      "mi" => Megabyte,
      "gi" => Gigabyte,
      "ti" => Terabyte,
      _ => throw new ArgumentException($"The unit '{unit}' not recognized.", nameof(memorySizeString))
    };

    return new MemorySize(int.Parse(sizeString) * multiplier);
  }

  public static implicit operator long(MemorySize size) => size._sizeInBytes;

  public static implicit operator MemorySize(long sizeInBytes) => FromBytes(sizeInBytes);

  public static MemorySize FromBytes(long bytes)
  {
    const long maximumAllowedBytes = long.MaxValue;
    if (bytes is < 0 or > maximumAllowedBytes) throw new ArgumentOutOfRangeException(nameof(bytes));

    return new MemorySize(bytes);
  }

  public static MemorySize FromKilobytes(long kilobytes)
  {
    const long maximumAllowedKilobytes = long.MaxValue / Kilobyte;
    if (kilobytes is < 0 or > maximumAllowedKilobytes) throw new ArgumentOutOfRangeException(nameof(kilobytes));

    return new MemorySize(kilobytes * Kilobyte);
  }

  public static MemorySize FromMegabytes(long megabytes)
  {
    const long maximumAllowedMegabytes = long.MaxValue / Megabyte;
    if (megabytes is < 0 or > maximumAllowedMegabytes) throw new ArgumentOutOfRangeException(nameof(megabytes));

    return new MemorySize(megabytes * Megabyte);
  }

  public static MemorySize FromGigabytes(long gigabytes)
  {
    const long maximumAllowedGigabytes = long.MaxValue / Gigabyte;
    if (gigabytes is < 0 or > maximumAllowedGigabytes) throw new ArgumentOutOfRangeException(nameof(gigabytes));

    return new MemorySize(gigabytes * Gigabyte);
  }

  public static MemorySize FromTerabytes(long terabytes)
  {
    const long maximumAllowedTerabytes = long.MaxValue / Terabyte;
    if (terabytes is < 0 or > maximumAllowedTerabytes) throw new ArgumentOutOfRangeException(nameof(terabytes));

    return new MemorySize(terabytes * Terabyte);
  }

  public int CompareTo(MemorySize other) => _sizeInBytes.CompareTo(other._sizeInBytes);

  public bool Equals(MemorySize other) => _sizeInBytes == other._sizeInBytes;

  public override bool Equals(object? obj) => obj is MemorySize other && Equals(other);

  public override int GetHashCode() => _sizeInBytes.GetHashCode();

  public override string ToString()
  {
    if (_sizeInBytes / Terabyte > 0) return $"{(decimal)_sizeInBytes / Terabyte:F3}TB";
    if (_sizeInBytes / Gigabyte > 0) return $"{(decimal)_sizeInBytes / Gigabyte:F3}GB";
    if (_sizeInBytes / Megabyte > 0) return $"{(decimal)_sizeInBytes / Megabyte:F3}MB";
    if (_sizeInBytes / Kilobyte > 0) return $"{(decimal)_sizeInBytes / Kilobyte:F3}KB";
    return $"{_sizeInBytes}B";
  }

  private readonly long _sizeInBytes;
  private static readonly Regex ParsingRegex = new(@"^(?<size>\d+)(?<unit>\w+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

  public const long Kilobyte = 1024;
  public const long Megabyte = 1024 * Kilobyte;
  public const long Gigabyte = 1024 * Megabyte;
  public const long Terabyte = 1024 * Gigabyte;
}
