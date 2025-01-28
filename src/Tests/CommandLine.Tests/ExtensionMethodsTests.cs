// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using TUnit.Assertions.AssertConditions.Throws;

public class ExtensionMethodsTests
{
    [Test]
    public async Task GetRequiredValueFromOptionName()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = await Assert.That(configuration.Parse("--option value").GetRequiredValue<string>("--option")).IsEqualTo("value");
    }

    [Test]
    public async Task GetRequiredValueFromOption()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = await Assert.That(configuration.Parse("--option value").GetRequiredValue(option)).IsEqualTo("value");
    }

    [Test]
    public async Task GetRequiredValueFromOptionWhenMissing()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });
        await Assert.That(() => configuration.Parse(string.Empty).GetRequiredValue(option)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetRequiredValueFromArgument()
    {
        Argument<string> argument = new("ARG");
        CommandLineConfiguration configuration = new(new RootCommand { argument });

        _ = await Assert.That(configuration.Parse("value").GetRequiredValue(argument)).IsEqualTo("value");
    }

    [Test]
    public async Task GetRequiredValueFromArgumentWhenMissing()
    {
        Argument<string> argument = new("ARG") { DefaultValueFactory = _ => default! };
        CommandLineConfiguration configuration = new(new RootCommand { argument });

        _ = await Assert.That(() => configuration.Parse(string.Empty).GetRequiredValue(argument)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetRequiredResultFromOption()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = await Assert.That(() => configuration.Parse("--option value").GetRequiredResult(option)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromArgument()
    {
        Argument<string> argument = new("ARG");
        CommandLineConfiguration configuration = new(new RootCommand { argument });

        _ = await Assert.That(() => configuration.Parse("value").GetRequiredResult(argument)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromCommand()
    {
        RootCommand command = [];
        CommandLineConfiguration configuration = new(command);

        _ = await Assert.That(() => configuration.Parse("value").GetRequiredResult(command)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromDirective()
    {
        RootCommand command = [];
        CommandLineConfiguration configuration = new(command);

        Directive directive = command.Directives.First();
        _ = await Assert.That(() => configuration.Parse($"[{directive.Name}]").GetRequiredResult(directive)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredResultFromSymbol()
    {
        RootCommand command = [];
        CommandLineConfiguration configuration = new(command);
        Symbol symbol = command;
        _ = await Assert.That(() => configuration.Parse("value").GetRequiredResult(symbol)).ThrowsNothing();
    }

    [Test]
    public async Task GetRequiredCommandFromSymbolResult()
    {
        Option<string> option = new("--option");
        RootCommand command = [option];
        CommandLineConfiguration configuration = new(command);

        var result = await Assert.That(configuration.Parse("--option value").GetResult(option)).IsTypeOf<Parsing.OptionResult>();
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
        CommandLineConfiguration configuration = new(command)
        {
            Output = TextWriter.Null,
            Error = TextWriter.Null,
        };

        ParseResult parsedConfiguration = configuration.Parse("--help");
        int result = parsedConfiguration.Invoke();
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
        CommandLineConfiguration configuration = new(command)
        {
            Output = TextWriter.Null,
            Error = TextWriter.Null,
        };

        ParseResult parsedConfiguration = configuration.Parse("--help");
        int result = parsedConfiguration.Invoke();
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