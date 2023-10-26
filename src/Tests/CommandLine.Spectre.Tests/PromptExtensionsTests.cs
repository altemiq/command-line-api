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

    [Fact]
    public void TestFlag()
    {
        const FileShare fileShare = FileShare.Read | FileShare.Delete;
        var (parseResult, option, console) = GetResult<FileShare>();
        _ = console.Interactive();
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.Enter);
        _ = parseResult.GetValueOrPrompt(option, "Select enum value: ", console).Should().Be(fileShare);
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

    private static (ParseResult, CliOption<T>, TestConsole) GetResult<T>(string? value = default)
    {
        var console = new TestConsole();
        if (value is not null)
        {
            console.Input.PushText(value);
        }

        var option = new CliOption<T>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });
        return (configuration.Parse(string.Empty), option, console);
    }
}
