#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BigFile.Sorter.Stage2.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage2.Tests.Unit;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class given_string_bubble_sorter_with_numbered_line_comparer
{
  public given_string_bubble_sorter_with_numbered_line_comparer()
  {
    _sorter = new StringBubbleSorter(new NumberedLineComparer());
  }

  [Fact]
  public void when_sort_single_item_list_it_should_return_it_unchanged()
  {
    var item = "88. string";
    var list = new List<string>(new[] { item });

    _sorter.Sort(list);

    list.Single().Should().Be(item);
  }

  [Fact]
  public void when_sort_it_should_sort_list_in_place()
  {
    var item1 = "1. aaa";
    var item2 = "1. aaaa";
    var item3 = "2. aaaa";
    var item4 = "1. z";

    var list = new List<string>(new[] { item3, item4, item2, item1 });

    _sorter.Sort(list);

    list[index: 0].Should().Be(item1);
    list[index: 1].Should().Be(item2);
    list[index: 2].Should().Be(item3);
    list[index: 3].Should().Be(item4);
  }

  [Fact]
  public void when_sort_with_invalid_arguments_it_should_complain()
  {
    var sut = () => _sorter.Sort(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("items");
  }

  [Fact]
  public void when_construct_with_invalid_arguments_it_should_complain()
  {
    var sut = () => new StringBubbleSorter(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("comparer");
  }

  private readonly StringBubbleSorter _sorter;
}
