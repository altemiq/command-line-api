// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class ExtensionMethodsTests
{
    [Test]
    public async Task GetRequiredValueFromOptionName()
    {
        Option<string> option = new("--option");
        RootCommand root = [option];

        _ = await Assert.That(root.Parse("--option value").GetRequiredValueOrThrowWhenNull<string>("--option")).IsEqualTo("value");
    }

    [Test]
    public async Task GetRequiredValueFromOption()
    {
        Option<string> option = new("--option");
        RootCommand root = [option];

        _ = await Assert.That(root.Parse("--option value").GetRequiredValueOrThrowWhenNull(option)).IsEqualTo("value");
    }

    [Test]
    public async Task GetRequiredValueFromOptionWhenMissing()
    {
        Option<string> option = new("--option") { DefaultValueFactory = _ => default! };
        RootCommand root = [option];
        _ = await Assert.That(() => root.Parse(string.Empty).GetRequiredValueOrThrowWhenNull(option)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetRequiredValueFromArgument()
    {
        Argument<string> argument = new("ARG");
        RootCommand root = [argument];

        _ = await Assert.That(root.Parse("value").GetRequiredValueOrThrowWhenNull(argument)).IsEqualTo("value");
    }

    [Test]
    public async Task GetRequiredValueFromArgumentWhenMissing()
    {
        Argument<string> argument = new("ARG") { DefaultValueFactory = _ => default! };
        RootCommand root = [argument];

        _ = await Assert.That(() => root.Parse(string.Empty).GetRequiredValueOrThrowWhenNull(argument)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetRequiredResultFromOption()
    {
        Option<string> option = new("--option");
        RootCommand root = [option];

        _ = await Assert.That(() => root.Parse("--option value").GetRequiredResult(option)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromArgument()
    {
        Argument<string> argument = new("ARG");
        RootCommand root = [argument];

        _ = await Assert.That(() => root.Parse("value").GetRequiredResult(argument)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromCommand()
    {
        RootCommand root = [];

        _ = await Assert.That(() => root.Parse("value").GetRequiredResult(root)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromDirective()
    {
        RootCommand root = [];

        Directive directive = root.Directives.First();
        _ = await Assert.That(() => root.Parse($"[{directive.Name}]").GetRequiredResult(directive)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromSymbol()
    {
        RootCommand root = [];
        Symbol symbol = root;
        _ = await Assert.That(() => root.Parse("value").GetRequiredResult(symbol)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredCommandFromSymbolResult()
    {
        Option<string> option = new("--option");
        RootCommand root = [option];

        Parsing.OptionResult? result = await Assert.That(root.Parse("--option value").GetResult(option)).IsTypeOf<Parsing.OptionResult>();
        _ = await Assert.That(result!.GetRequiredCommand).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredCommandFromOption()
    {
        Command? commandFromOptionDefault = default;
        Option<string> option = new("--api")
        {
            DefaultValueFactory = argumentResult =>
            {
                commandFromOptionDefault = argumentResult.GetCommand();
                return "This is the default value";
            },
        };
        RootCommand command = [option];

        ParseResult parsedConfiguration = command.Parse("--help");
        parsedConfiguration.InvocationConfiguration.Output = TextWriter.Null;
        parsedConfiguration.InvocationConfiguration.Error = TextWriter.Null;
        _ = await parsedConfiguration.InvokeAsync();
        _ = await Assert.That(commandFromOptionDefault).IsNotNull();
    }



    [Test]
    public async Task GetRequiredCommandFromArgument()
    {
        Command? commandFromOptionDefault = default;
        Argument<string> argument = new("API")
        {
            DefaultValueFactory = argumentResult =>
            {
                commandFromOptionDefault = argumentResult.GetCommand();
                return "This is the default value";
            },
        };
        RootCommand command = [argument];

        ParseResult parsedConfiguration = command.Parse("--help");
        parsedConfiguration.InvocationConfiguration.Output = TextWriter.Null;
        parsedConfiguration.InvocationConfiguration.Error = TextWriter.Null;
        _ = await parsedConfiguration.InvokeAsync();
        _ = await Assert.That(commandFromOptionDefault).IsNotNull();
    }

    [Test]
    public async Task GetCommandFromNull()
    {
        _ = await Assert.That(ExtensionMethods.GetCommand(default!)).IsNull();
    }

    [Test]
    public async Task GetRequiredCommandFromNull()
    {
        _ = await Assert.That(() => ExtensionMethods.GetRequiredCommand(null!)).Throws<ArgumentNullException>();
    }
}