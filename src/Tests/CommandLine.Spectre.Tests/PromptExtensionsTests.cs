// -----------------------------------------------------------------------
// <copyright file="PromptExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

using global::Spectre.Console.Testing;

public class PromptExtensionsTests
{
    [Fact]
    public void TestSpecifiedBoolean()
    {
        var (parseResult, option, console) = GetResult<bool>("y", "--option true");
        _ = parseResult.GetValueOrPrompt(option, "Specify boolean value: ", console).Should().BeTrue();
    }

    [Fact]
    public void TestBoolean()
    {
        var (parseResult, option, console) = GetResult<bool>("y");
        console.Input.PushKey(ConsoleKey.Enter);
        _ = parseResult.GetValueOrPrompt(option, "Specify boolean value: ", console).Should().BeTrue();
    }

    [Fact]
    public void TestDefaultBoolean()
    {
        var (parseResult, option, console) = GetResult<bool>();
        option.DefaultValueFactory = _ => true;
        console.Input.PushKey(ConsoleKey.Enter);
        _ = parseResult.GetValueOrPrompt(option, "Get default value: ", console).Should().BeTrue();
    }
    [Fact]
    public void TestSpecifiedString()
    {
        var (parseResult, option, console) = GetResult<string>(args: "--option value");
        _ = parseResult.GetValueOrPrompt(option, "Enter string value: ", console).Should().Be("value");
    }

    [Fact]
    public void TestString()
    {
        const string value = nameof(value);
        _ = GetValue<string>("Enter string value:", value).Should().Be(value);
    }

    [Fact]
    public void TestDefaultString()
    {
        const string value = nameof(value);
        _ = GetValue("Get default value: ", value).Should().Be(value);
    }

    [Fact]
    public void TestInt32()
    {
        const int value = 123;
        _ = GetValue<int>("Enter integer value: ", value.ToString()).Should().Be(value);
    }

    [Fact]
    public void TestDefaultInt32()
    {
        const int value = 123;
        _ = GetValue("Get default value: ", value).Should().Be(value);
    }

    [Fact]
    public void TestEnum()
    {
        const FileMode fileMode = FileMode.Open;
        var (parseResult, option, console) = GetResult<FileMode>();
        _ = console.Interactive();
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Enter);
        _ = parseResult.GetValueOrPrompt(option, "Select enum value: ", console).Should().Be(fileMode);
    }

    [Theory]
    [InlineData(typeof(Enums.SByte.FileShare))]
    [InlineData(typeof(Enums.Byte.FileShare))]
    [InlineData(typeof(Enums.Int16.FileShare))]
    [InlineData(typeof(Enums.UInt16.FileShare))]
    [InlineData(typeof(FileShare))]
    [InlineData(typeof(Enums.UInt32.FileShare))]
    [InlineData(typeof(Enums.Int64.FileShare))]
    [InlineData(typeof(Enums.UInt64.FileShare))]
    public void TestFlag(Type type)
    {
        var expected = Enum.ToObject(type, Convert.ChangeType(1 | 4, type.GetEnumUnderlyingType()));
        _ = typeof(PromptExtensionsTests).GetMethod(nameof(TestFlagCore), Reflection.BindingFlags.Static | Reflection.BindingFlags.NonPublic)
            .Should().BeAssignableTo<Reflection.MethodInfo>()
            .Which.MakeGenericMethod(type).Invoke(null, [expected]);
    }

    [Fact]
    public void TestWithCompletions()
    {
        var (parseResult, option, console) = GetResult<string>("second");
        option.CompletionSources.Add("first", "second", "third");
        console.Input.PushKey(ConsoleKey.Enter);
        _ = parseResult.GetValueOrPrompt(option, "GetValue", console).Should().Be("second");
    }

    private static T GetValue<T>(string prompt, string value)
    {
        var (parseResult, option, console) = GetResult<T>(value);
        console.Input.PushKey(ConsoleKey.Enter);
        return parseResult.GetValueOrPrompt(option, prompt, console);
    }
    private static T GetValue<T>(string prompt, T defaultValue)
    {
        var (parseResult, option, console) = GetResult<T>();
        console.Input.PushKey(ConsoleKey.Enter);
        option.DefaultValueFactory = _ => defaultValue;
        return parseResult.GetValueOrPrompt(option, prompt, console);
    }

    private static (ParseResult, CliOption<T>, TestConsole) GetResult<T>(string? value = default, string? args = default)
    {
        var console = new TestConsole();
        if (value is not null)
        {
            console.Input.PushText(value);
        }

        var option = new CliOption<T>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });
        return (configuration.Parse(args ?? string.Empty), option, console);
    }

    private static void TestFlagCore<T>(T expected)
    {
        var (parseResult, option, console) = GetResult<T>();
        _ = console.Interactive();
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.Enter);
        _ = parseResult.GetValueOrPrompt(option, "Select enum value: ", console).Should().Be(expected);
    }

    private static class Enums
    {
        public static class SByte
        {
            [Flags]
            public enum FileShare : sbyte
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }

        public static class Byte
        {
            [Flags]
            public enum FileShare : byte
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }

        public static class Int16
        {
            [Flags]
            public enum FileShare : short
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }

        public static class UInt16
        {
            [Flags]
            public enum FileShare : ushort
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }

        public static class UInt32
        {
            [Flags]
            public enum FileShare : uint
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }

        public static class Int64
        {
            [Flags]
            public enum FileShare : long
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }

        public static class UInt64
        {
            [Flags]
            public enum FileShare : ulong
            {
                None = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Delete = 4,
                Inheritable = 16
            }
        }
    }
}