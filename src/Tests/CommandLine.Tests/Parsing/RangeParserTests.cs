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

    public static TheoryData<string, SerializableRange> Ranges()
    {
        return new()
    {
        { "..", new(new Range(start: Index.Start, end: Index.End)) },
        { "0..", new(new Range(start: Index.Start, end: Index.End)) },
        { "..^0", new(new Range(start: Index.Start, end: Index.End)) },
        { "0..^0", new(new Range(start: Index.Start, end: Index.End)) },
        { "1..", new(new Range(start: new Index(value: 1), end: Index.End)) },
        { "1..^0", new(new Range(start: new Index(value: 1), end: Index.End)) },
        { "3..^5", new(new Range(start: new Index(value: 3), end: new Index(value: 5, fromEnd: true))) },
        { "^5..^2", new(new Range(start: new Index(value: 5, fromEnd: true), end: new Index(value: 2, fromEnd: true))) },
    };
    }

    [Fact]
    public void TestSerializableRange()
    {
        SerializableRange serializable = new(new Range(start: Index.Start, end: Index.End));
        string serialized = Xunit.Sdk.SerializationHelper.Serialize(serializable);
        SerializableRange deserialized = Xunit.Sdk.SerializationHelper.Deserialize<SerializableRange>(serialized);
        _ = deserialized.Should().Be(serializable);
    }


    [Fact]
    public void TestSerializableIndex()
    {
        SerializableIndex serializable = new(new Index(1, true));
        string serialized = Xunit.Sdk.SerializationHelper.Serialize(serializable);
        SerializableIndex deserialized = Xunit.Sdk.SerializationHelper.Deserialize<SerializableIndex>(serialized);
        _ = deserialized.Should().Be(serializable);
    }

    [Theory]
    [MemberData(nameof(Ranges))]
    public void ParseRange(string input, SerializableRange expected)
    {
        _ = RangeParser.Parse(input).Should().Be(expected, RangeEqualityComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(Ranges))]
    public void ParseArgument(string input, SerializableRange expected)
    {
        _ = RangeParser.Parse(argumentCommand.Parse(input).CommandResult.Children.OfType<ArgumentResult>().First()).Should().Be(expected, RangeEqualityComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(Ranges))]
    public void ParseOption(string input, SerializableRange expected)
    {
        _ = optionCommand.Parse(RangeOptionName + " " + input).GetValue<Range>(RangeOptionName).Should().Be(expected, RangeEqualityComparer.Instance);
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

    public class SerializableRange : Xunit.Abstractions.IXunitSerializable
    {
        private Range range;

        public SerializableRange()
        {
        }

        public SerializableRange(Range range)
        {
            this.range = range;
        }

        public void Deserialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            int startValue = info.GetValue<int>($"{nameof(range)}.{nameof(range.Start)}.{nameof(range.Start.Value)}");
            bool startIsFromEnd = info.GetValue<bool>($"{nameof(range)}.{nameof(range.Start)}.{nameof(range.Start.IsFromEnd)}");
            int endValue = info.GetValue<int>($"{nameof(range)}.{nameof(range.End)}.{nameof(range.End.Value)}");
            bool endIsFromEnd = info.GetValue<bool>($"{nameof(range)}.{nameof(range.End)}.{nameof(range.End.IsFromEnd)}");

            range = new Range(new Index(startValue, startIsFromEnd), new Index(endValue, endIsFromEnd));
        }

        public void Serialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            info.AddValue($"{nameof(range)}.{nameof(range.Start)}.{nameof(range.Start.Value)}", range.Start.Value, typeof(int));
            info.AddValue($"{nameof(range)}.{nameof(range.Start)}.{nameof(range.Start.IsFromEnd)}", range.Start.IsFromEnd, typeof(bool));
            info.AddValue($"{nameof(range)}.{nameof(range.End)}.{nameof(range.End.Value)}", range.End.Value, typeof(int));
            info.AddValue($"{nameof(range)}.{nameof(range.End)}.{nameof(range.End.IsFromEnd)}", range.End.IsFromEnd, typeof(bool));
        }

        public static implicit operator Range(SerializableRange range)
        {
            return range.range;
        }

        public static implicit operator SerializableRange(Range range)
        {
            return new(range);
        }

        public override string ToString()
        {
            return range.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                SerializableRange range => RangeEqualityComparer.Instance.Equals(this.range, range.range),
                Range range => RangeEqualityComparer.Instance.Equals(this.range, range),
                _ => base.Equals(obj),
            };
        }

        public override int GetHashCode()
        {
            return RangeEqualityComparer.Instance.GetHashCode(this);
        }
    }

    public class SerializableIndex : Xunit.Abstractions.IXunitSerializable
    {
        private Index index;

        public SerializableIndex()
        {
        }

        public SerializableIndex(Index index)
        {
            this.index = index;
        }

        public void Deserialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            int value = info.GetValue<int>($"{nameof(index)}.{nameof(index.Value)}");
            bool isFromEnd = info.GetValue<bool>($"{nameof(index)}.{nameof(index.IsFromEnd)}");
            index = new Index(value, isFromEnd);
        }

        public void Serialize(Xunit.Abstractions.IXunitSerializationInfo info)
        {
            info.AddValue($"{nameof(index)}.{nameof(index.Value)}", index.Value, typeof(int));
            info.AddValue($"{nameof(index)}.{nameof(index.IsFromEnd)}", index.IsFromEnd, typeof(bool));
        }

        public static implicit operator Index(SerializableIndex index)
        {
            return index.index;
        }

        public static implicit operator SerializableIndex(Index index)
        {
            return new(index);
        }

        public override string ToString()
        {
            return index.ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                SerializableIndex index => IndexEqualityComparer.Instance.Equals(this.index, index.index),
                Index index => IndexEqualityComparer.Instance.Equals(this.index, index),
                _ => base.Equals(obj),
            };
        }

        public override int GetHashCode()
        {
            return IndexEqualityComparer.Instance.GetHashCode(this);
        }
    }
}