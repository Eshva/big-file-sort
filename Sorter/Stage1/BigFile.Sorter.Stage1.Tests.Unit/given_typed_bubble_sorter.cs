#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BigFile.Sorter.Stage1.Domain;
using FluentAssertions;
using Xunit;

#endregion

namespace BigFile.Sorter.Stage1.Tests.Unit;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class given_typed_bubble_sorter
{
  [Fact]
  public void when_sort_single_item_list_it_should_return_it_unchanged()
  {
    var sut = new TypedBubbleSorter<NumberedLine>();
    var item = new NumberedLine("88. string");
    var list = new List<NumberedLine>(new[] { item });

    sut.Sort(list);

    list.Single().Should().Be(item);
  }

  [Fact]
  public void when_sort_it_should_sort_list_in_place()
  {
    var sut = new TypedBubbleSorter<NumberedLine>();
    var item1 = new NumberedLine("1. aaa");
    var item2 = new NumberedLine("1. aaaa");
    var item3 = new NumberedLine("2. aaaa");
    var item4 = new NumberedLine("1. z");

    var list = new List<NumberedLine>(new[] { item3, item4, item2, item1 });

    sut.Sort(list);

    list[index: 0].Should().Be(item1);
    list[index: 1].Should().Be(item2);
    list[index: 2].Should().Be(item3);
    list[index: 3].Should().Be(item4);
  }

  [Fact]
  public void when_sort_with_invalid_arguments_it_should_complain()
  {
    var sorter = new TypedBubbleSorter<NumberedLine>();
    var sut = () => sorter.Sort(null!);
    sut.Should().ThrowExactly<ArgumentNullException>().WithParameterName("items");
  }
}
