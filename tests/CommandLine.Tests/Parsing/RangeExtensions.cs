namespace System.CommandLine.Parsing;

using TUnit.Assertions.Attributes;

public static class RangeExtensions
{
    [ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
    [GenerateAssertion]
    public static bool IsEqualTo(this Range actual, Range expected) => actual.Equals(expected);
}