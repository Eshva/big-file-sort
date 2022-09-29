namespace BigFile.Generator.Tests.Unit;

internal class SimilarLineGenerator : ILineGenerator
{
  public string Generate() => ExpectedString;

  public const string ExpectedString = "Expected string";
}
