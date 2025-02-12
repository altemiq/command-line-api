// -----------------------------------------------------------------------
// <copyright file="Parsers.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

public class RangeParserTests
{
    private const string RangeOptionName = "--range";
    private static readonly Command argumentCommand = new("base") { new Argument<string>("RANGE") };
    private static readonly Command optionCommand = new("base") { new Option<Range>(RangeOptionName) { CustomParser = RangeParser.Parse } };

    public static IEnumerable<Func<(string, Range)>> Ranges()
    {
        yield return () => ("..", new Range(start: Index.Start, end: Index.End));
        yield return () => ("0..", new Range(start: Index.Start, end: Index.End));
        yield return () => ("..^0", new Range(start: Index.Start, end: Index.End));
        yield return () => ("0..^0", new Range(start: Index.Start, end: Index.End));
        yield return () => ("1..", new Range(start: new Index(value: 1), end: Index.End));
        yield return () => ("1..^0", new Range(start: new Index(value: 1), end: Index.End));
        yield return () => ("3..^5", new Range(start: new Index(value: 3), end: new Index(value: 5, fromEnd: true)));
        yield return () => ("^5..^2", new Range(start: new Index(value: 5, fromEnd: true), end: new Index(value: 2, fromEnd: true)));
    }

    [Test]
    [MethodDataSource(nameof(Ranges))]
    public async Task ParseRange(string input, Range expected)
    {
        _ = await Assert.That(RangeParser.Parse(input)).IsEqualTo(expected, RangeEqualityComparer.Instance);
    }

    [Test]
    [MethodDataSource(nameof(Ranges))]
    public async Task ParseArgument(string input, Range expected)
    {
        _ = await Assert.That(RangeParser.Parse(argumentCommand.Parse(input).CommandResult.Children.OfType<ArgumentResult>().First())).IsEqualTo(expected, RangeEqualityComparer.Instance);
    }

    [Test]
    [MethodDataSource(nameof(Ranges))]
    public async Task ParseOption(string input, Range expected)
    {
        _ = await Assert.That(optionCommand.Parse(RangeOptionName + " " + input).GetValue<Range>(RangeOptionName)).IsEqualTo(expected, RangeEqualityComparer.Instance);
    }

    private class RangeEqualityComparer : IEqualityComparer<Range>
    {
        public static readonly RangeEqualityComparer Instance = new();

        public bool Equals(Range x, Range y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([Diagnostics.CodeAnalysis.DisallowNull] Range obj)
        {
            return obj.GetHashCode();
        }
    }

    private class IndexEqualityComparer : IEqualityComparer<Index>
    {
        public static readonly IndexEqualityComparer Instance = new();

        public bool Equals(Index x, Index y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([Diagnostics.CodeAnalysis.DisallowNull] Index obj)
        {
            return obj.GetHashCode();
        }
    }
}