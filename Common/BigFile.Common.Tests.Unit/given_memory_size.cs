#region Usings

using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Common.Tests.Unit;

public class given_memory_size
{
  [Fact]
  public void when_create_from_valid_size_string_it_should_produce_expected_object()
  {
    ((long)MemorySize.From("10ki")).Should().Be(TenKilobytes);
    ((long)MemorySize.From("10KI")).Should().Be(TenKilobytes);
    ((long)MemorySize.From("10kI")).Should().Be(TenKilobytes);
    ((long)MemorySize.From("10Ki")).Should().Be(TenKilobytes);

    ((long)MemorySize.From("10mi")).Should().Be(TenMegabytes);
    ((long)MemorySize.From("10MI")).Should().Be(TenMegabytes);
    ((long)MemorySize.From("10mI")).Should().Be(TenMegabytes);
    ((long)MemorySize.From("10Mi")).Should().Be(TenMegabytes);

    ((long)MemorySize.From("10gi")).Should().Be(TenGigabytes);
    ((long)MemorySize.From("10GI")).Should().Be(TenGigabytes);
    ((long)MemorySize.From("10gI")).Should().Be(TenGigabytes);
    ((long)MemorySize.From("10Gi")).Should().Be(TenGigabytes);

    ((long)MemorySize.From("10ti")).Should().Be(TenTerabytes);
    ((long)MemorySize.From("10TI")).Should().Be(TenTerabytes);
    ((long)MemorySize.From("10tI")).Should().Be(TenTerabytes);
    ((long)MemorySize.From("10Ti")).Should().Be(TenTerabytes);
  }

  [Fact]
  public void when_create_from_invalid_size_string_it_should_complain()
  {
    var sut = () => MemorySize.From("10k");
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("memorySizeString");
    sut = () => MemorySize.From("10");
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("memorySizeString");
    sut = () => MemorySize.From(null!);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("memorySizeString");
    sut = () => MemorySize.From(string.Empty);
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("memorySizeString");
    sut = () => MemorySize.From(" \t\n");
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("memorySizeString");
    sut = () => MemorySize.From("some string");
    sut.Should().ThrowExactly<ArgumentException>().WithParameterName("memorySizeString");
  }

  [Fact]
  public void when_create_from_bytes_it_should_produce_expected_object()
  {
    ((long)MemorySize.FromBytes(bytes: 10)).Should().Be(TenBytes);
    ((long)MemorySize.FromBytes(MaximumAllowedBytes)).Should().Be(MaximumAllowedBytes);
  }

  [Fact]
  public void when_create_from_bytes_with_invalid_arguments_it_should_complain()
  {
    var sut = () => MemorySize.FromBytes(bytes: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("bytes");
  }

  [Fact]
  public void when_create_from_kilobytes_it_should_produce_expected_object()
  {
    ((long)MemorySize.FromKilobytes(kilobytes: 10)).Should().Be(TenKilobytes);
    ((long)MemorySize.FromKilobytes(MaximumAllowedKilobytes)).Should().Be(MaximumAllowedKilobytes * MemorySize.Kilobyte);
  }

  [Fact]
  public void when_create_from_kilobytes_with_invalid_arguments_it_should_complain()
  {
    var sut = () => MemorySize.FromKilobytes(kilobytes: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("kilobytes");
    sut = () => MemorySize.FromKilobytes(MaximumAllowedKilobytes + 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("kilobytes");
  }

  [Fact]
  public void when_create_from_megabytes_it_should_produce_expected_object()
  {
    ((long)MemorySize.FromMegabytes(megabytes: 10)).Should().Be(TenMegabytes);
    ((long)MemorySize.FromMegabytes(MaximumAllowedMegabytes)).Should().Be(MaximumAllowedMegabytes * MemorySize.Megabyte);
  }

  [Fact]
  public void when_create_from_megabytes_with_invalid_arguments_it_should_complain()
  {
    var sut = () => MemorySize.FromMegabytes(megabytes: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("megabytes");
    sut = () => MemorySize.FromMegabytes(MaximumAllowedMegabytes + 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("megabytes");
  }

  [Fact]
  public void when_create_from_gigabytes_it_should_produce_expected_object()
  {
    ((long)MemorySize.FromGigabytes(gigabytes: 10)).Should().Be(TenGigabytes);
    ((long)MemorySize.FromGigabytes(MaximumAllowedGigabytes)).Should().Be(MaximumAllowedGigabytes * MemorySize.Gigabyte);
  }

  [Fact]
  public void when_create_from_gigabytes_with_invalid_arguments_it_should_complain()
  {
    var sut = () => MemorySize.FromGigabytes(gigabytes: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("gigabytes");
    sut = () => MemorySize.FromGigabytes(MaximumAllowedGigabytes + 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("gigabytes");
  }

  [Fact]
  public void when_create_from_terabytes_it_should_produce_expected_object()
  {
    ((long)MemorySize.FromTerabytes(terabytes: 10)).Should().Be(TenTerabytes);
    ((long)MemorySize.FromTerabytes(MaximumAllowedTerabytes)).Should().Be(MaximumAllowedTerabytes * MemorySize.Terabyte);
  }

  [Fact]
  public void when_create_from_terabytes_with_invalid_arguments_it_should_complain()
  {
    var sut = () => MemorySize.FromTerabytes(terabytes: -1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("terabytes");
    sut = () => MemorySize.FromTerabytes(MaximumAllowedTerabytes + 1);
    sut.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("terabytes");
  }

  [Fact]
  public void when_compare_memory_sizes_it_should_produce_expected_results()
  {
    var nineKilobytes = MemorySize.FromKilobytes(kilobytes: 9);
    var tenKilobytes = MemorySize.FromKilobytes(kilobytes: 10);
    var elevenKilobytes = MemorySize.FromKilobytes(kilobytes: 11);

    nineKilobytes.Should().BeLessThan(tenKilobytes);
    elevenKilobytes.Should().BeGreaterThan(tenKilobytes);
    nineKilobytes.Should().Be(MemorySize.FromKilobytes(kilobytes: 9));

    (tenKilobytes == MemorySize.FromKilobytes(kilobytes: 10)).Should().BeTrue();
    (nineKilobytes != MemorySize.FromKilobytes(kilobytes: 10)).Should().BeTrue();
    (nineKilobytes < tenKilobytes).Should().BeTrue();
  }

  [Fact]
  public void when_convert_to_string_it_should_produce_expected_strings()
  {
    MemorySize.FromGigabytes((long)Math.Round(1024 * 1.234M)).ToString().Should().Be("1.234TB");
    MemorySize.FromMegabytes((long)Math.Round(1024 * 1.234M)).ToString().Should().Be("1.234GB");
    MemorySize.FromKilobytes((long)Math.Round(1024 * 1.234M)).ToString().Should().Be("1.234MB");
    MemorySize.FromBytes((long)Math.Round(1024 * 1.234M)).ToString().Should().Be("1.234KB");
    MemorySize.FromBytes((long)Math.Round(d: 123)).ToString().Should().Be("123B");
  }

  private const long MaximumAllowedBytes = long.MaxValue;
  private const long MaximumAllowedKilobytes = long.MaxValue / MemorySize.Kilobyte;
  private const long MaximumAllowedMegabytes = long.MaxValue / MemorySize.Megabyte;
  private const long MaximumAllowedGigabytes = long.MaxValue / MemorySize.Gigabyte;
  private const long MaximumAllowedTerabytes = long.MaxValue / MemorySize.Terabyte;

  private const long TenBytes = 10;
  private const long TenKilobytes = 10 * MemorySize.Kilobyte;
  private const long TenMegabytes = 10 * MemorySize.Megabyte;
  private const long TenGigabytes = 10 * MemorySize.Gigabyte;
  private const long TenTerabytes = 10 * MemorySize.Terabyte;
}
