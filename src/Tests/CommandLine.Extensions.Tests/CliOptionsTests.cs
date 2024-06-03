// -----------------------------------------------------------------------
// <copyright file="CliOptionsTests.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Extensions;

public class CliOptionsTests
{
    private static readonly CliCommand command = new("base") { new CliArgument<string>("RANGE") };

    public static TheoryData<string, SerializableRange> Ranges() => new()
    {
        { "..", new(new Range(start: Index.Start, end: Index.End)) },
        { "0..", new(new Range(start: Index.Start, end: Index.End)) },
        { "..^0", new(new Range(start: Index.Start, end: Index.End)) },
        { "0..^0", new(new Range(start: Index.Start, end: Index.End)) },
        { "1..", new(new Range(start: new Index(value: 1), end: Index.End)) },
        { "3..^5", new(new Range(start: new Index(value: 3), end: new Index(value: 5, fromEnd: true))) },
        { "^5..^2", new(new Range(start: new Index(value: 5, fromEnd: true), end: new Index(value: 2, fromEnd: true))) },
    };

    [Fact]
    public void FileSystemGlobbing()
    {
        const string RootDir = "C:\\Files to Search";
        var first = Path.Join(RootDir, "first", "first.las");
        var second = Path.Join(RootDir, "second.las");
        var third = Path.Join(RootDir, "deep", "deep", "path", "third.las");
        var forth = Path.Join(RootDir, "forth.laz");

        var directoryInfo = new Microsoft.Extensions.FileSystemGlobbing.InMemoryDirectoryInfo("C:\\Files to Search", [first, second, third, forth]);

        var argument = new CliArgument<FileInfo[]>("FILES") { CustomParser = argumentResult => CliOptions.ParseGlobbing(argumentResult, directoryInfo) };
        var root = new CliRootCommand { argument };
        var configuration = new CliConfiguration(root);
        var parseResult = configuration.Parse("\"" + Path.Combine(RootDir, "**", "*.las") + "\"");

        parseResult.GetValue(argument).Should().NotBeNull()
            .And.Subject.Select(x => x.FullName).Should()
                .Contain(first).And
                .Contain(second).And
                .Contain(third).And
                .NotContain(forth);

    }

    [Fact]
    public void TestSerializableRange()
    {
        var serializable = new SerializableRange(new Range(start: Index.Start, end: Index.End));
        var serialized = Xunit.Sdk.SerializationHelper.Serialize(serializable);
        var deserialized = Xunit.Sdk.SerializationHelper.Deserialize<SerializableRange>(serialized);
        _ = deserialized.Should().Be(serializable);
    }


    [Fact]
    public void TestSerializableIndex()
    {
        var serializable = new SerializableIndex(new Index(1, true));
        var serialized = Xunit.Sdk.SerializationHelper.Serialize(serializable);
        var deserialized = Xunit.Sdk.SerializationHelper.Deserialize<SerializableIndex>(serialized);
        _ = deserialized.Should().Be(serializable);
    }

    [Theory]
    [MemberData(nameof(Ranges))]
    public void ParseRange(string input, SerializableRange expected) => CliOptions.ParseRange(input).Should().Be(expected, RangeEqualityComparer.Instance);

    [Theory]
    [MemberData(nameof(Ranges))]
    public void ParseOption(string input, SerializableRange expected) => CliOptions.ParseRange(command.Parse(input).CommandResult.Children.FirstOrDefault().As<Parsing.ArgumentResult>()).Should().Be(expected, RangeEqualityComparer.Instance);

    private class RangeEqualityComparer : IEqualityComparer<Range>
    {
        public static readonly IEqualityComparer<Range> Instance = new RangeEqualityComparer();

        public bool Equals(Range x, Range y) => x.Equals(y);

        public int GetHashCode([Diagnostics.CodeAnalysis.DisallowNull] Range obj) => obj.GetHashCode();
    }

    private class IndexEqualityComparer : IEqualityComparer<Index>
    {
        public static readonly IEqualityComparer<Index> Instance = new IndexEqualityComparer();

        public bool Equals(Index x, Index y) => x.Equals(y);

        public int GetHashCode([Diagnostics.CodeAnalysis.DisallowNull] Index obj) => obj.GetHashCode();
    }

    public class SerializableRange : Xunit.Abstractions.IXunitSerializable
    {
        private Range range;

        public SerializableRange()
        {
        }

        public SerializableRange(Range range) => this.range = range;

        public void Deserialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            var startValue = info.GetValue<int>($"{nameof(this.range)}.{nameof(this.range.Start)}.{nameof(this.range.Start.Value)}");
            var startIsFromEnd = info.GetValue<bool>($"{nameof(this.range)}.{nameof(this.range.Start)}.{nameof(this.range.Start.IsFromEnd)}");
            var endValue = info.GetValue<int>($"{nameof(this.range)}.{nameof(this.range.End)}.{nameof(this.range.End.Value)}");
            var endIsFromEnd = info.GetValue<bool>($"{nameof(this.range)}.{nameof(this.range.End)}.{nameof(this.range.End.IsFromEnd)}");

            this.range = new Range(new Index(startValue, startIsFromEnd), new Index(endValue, endIsFromEnd));
        }

        public void Serialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            info.AddValue($"{nameof(this.range)}.{nameof(this.range.Start)}.{nameof(this.range.Start.Value)}", this.range.Start.Value, typeof(int));
            info.AddValue($"{nameof(this.range)}.{nameof(this.range.Start)}.{nameof(this.range.Start.IsFromEnd)}", this.range.Start.IsFromEnd, typeof(bool));
            info.AddValue($"{nameof(this.range)}.{nameof(this.range.End)}.{nameof(this.range.End.Value)}", this.range.End.Value, typeof(int));
            info.AddValue($"{nameof(this.range)}.{nameof(this.range.End)}.{nameof(this.range.End.IsFromEnd)}", this.range.End.IsFromEnd, typeof(bool));
        }

        public static implicit operator Range(SerializableRange range) => range.range;

        public static implicit operator SerializableRange(Range range) => new(range);

        public override string ToString() => this.range.ToString();

        public override bool Equals(object? obj) => obj switch
        {
            SerializableRange range => RangeEqualityComparer.Instance.Equals(this.range, range.range),
            Range range => RangeEqualityComparer.Instance.Equals(this.range, range),
            _ => base.Equals(obj),
        };

        public override int GetHashCode() => RangeEqualityComparer.Instance.GetHashCode(this);
    }

    public class SerializableIndex : Xunit.Abstractions.IXunitSerializable
    {
        private Index index;

        public SerializableIndex()
        {
        }

        public SerializableIndex(Index index) => this.index = index;

        public void Deserialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            var value = info.GetValue<int>($"{nameof(this.index)}.{nameof(this.index.Value)}");
            var isFromEnd = info.GetValue<bool>($"{nameof(this.index)}.{nameof(this.index.IsFromEnd)}");
            this.index = new Index(value, isFromEnd);
        }

        public void Serialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            info.AddValue($"{nameof(this.index)}.{nameof(this.index.Value)}", this.index.Value, typeof(int));
            info.AddValue($"{nameof(this.index)}.{nameof(this.index.IsFromEnd)}", this.index.IsFromEnd, typeof(bool));
        }

        public static implicit operator Index(SerializableIndex index) => index.index;

        public static implicit operator SerializableIndex(Index index) => new(index);

        public override string ToString() => this.index.ToString();

        public override bool Equals(object? obj) => obj switch
        {
            SerializableIndex index => IndexEqualityComparer.Instance.Equals(this.index, index.index),
            Index index => IndexEqualityComparer.Instance.Equals(this.index, index),
            _ => base.Equals(obj),
        };

        public override int GetHashCode() => IndexEqualityComparer.Instance.GetHashCode(this);
    }
}
