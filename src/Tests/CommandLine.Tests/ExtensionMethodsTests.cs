// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class ExtensionMethodsTests
{
    [Fact]
    public void GetRequiredValueFromOptionName()
    {
        CliOption<string> option = new("--option");
        CliConfiguration configuration = new(new CliRootCommand { option });

        _ = configuration.Parse("--option value").GetRequiredValue<string>("--option").Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOption()
    {
        CliOption<string> option = new("--option");
        CliConfiguration configuration = new(new CliRootCommand { option });

        _ = configuration.Parse("--option value").GetRequiredValue(option).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOptionWhenMissing()
    {
        CliOption<string> option = new("--option");
        CliConfiguration configuration = new(new CliRootCommand { option });

        _ = configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(option)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredValueFromArgument()
    {
        CliArgument<string> argument = new("ARG");
        CliConfiguration configuration = new(new CliRootCommand { argument });

        _ = configuration.Parse("value").GetRequiredValue(argument).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromArgumentWhenMissing()
    {
        CliArgument<string> argument = new("ARG") { DefaultValueFactory = _ => default! };
        CliConfiguration configuration = new(new CliRootCommand { argument });

        _ = configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(argument)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredResultFromOption()
    {
        CliOption<string> option = new("--option");
        CliConfiguration configuration = new(new CliRootCommand { option });

        _ = configuration.Parse("--option value").Invoking(parseResult => parseResult.GetRequiredResult(option)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromArgument()
    {
        CliArgument<string> argument = new("ARG");
        CliConfiguration configuration = new(new CliRootCommand { argument });

        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(argument)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromCommand()
    {
        CliRootCommand command = [];
        CliConfiguration configuration = new(command);

        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(command)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromDirective()
    {
        CliRootCommand command = [];
        CliConfiguration configuration = new(command);

        CliDirective directive = command.Directives.First();
        _ = configuration.Parse($"[{directive.Name}]").Invoking(parseResult => parseResult.GetRequiredResult(directive)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromSymbol()
    {
        CliRootCommand command = [];
        CliConfiguration configuration = new(command);
        CliSymbol symbol = command;
        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(symbol).Should()).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredCommandFromSymbolResult()
    {
        CliOption<string> option = new("--option");
        CliRootCommand command = [option];
        CliConfiguration configuration = new(command);

        _ = configuration.Parse("--option value")
            .GetResult(option).Should().BeOfType<Parsing.OptionResult>()
            .Which.Invoking(result => result.GetRequiredCommand()).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredCommandFromOption()
    {
        CliCommand? commandFromOptionDefault = default;
        CliOption<string> option = new("--api")
        {
            DefaultValueFactory = argumentResult =>
            {
                commandFromOptionDefault = argumentResult.GetCommand();
                return "This is the default value";
            },
        };
        CliRootCommand command = [option];
        CliConfiguration configuration = new(command)
        {
            Output = TextWriter.Null,
            Error = TextWriter.Null,
        };

        ParseResult parsedConfiguration = configuration.Parse("--help");
        int result = parsedConfiguration.Invoke();
        _ = commandFromOptionDefault.Should().NotBeNull();
    }



    [Fact]
    public void GetRequiredCommandFromArgument()
    {
        CliCommand? commandFromOptionDefault = default;
        CliArgument<string> argument = new("API")
        {
            DefaultValueFactory = argumentResult =>
            {
                commandFromOptionDefault = argumentResult.GetCommand();
                return "This is the default value";
            },
        };
        CliRootCommand command = [argument];
        CliConfiguration configuration = new(command)
        {
            Output = TextWriter.Null,
            Error = TextWriter.Null,
        };

        ParseResult parsedConfiguration = configuration.Parse("--help");
        int result = parsedConfiguration.Invoke();
        _ = commandFromOptionDefault.Should().NotBeNull();
    }

    [Fact]
    public void GetCommandFromNull()
    {
        _ = ExtensionMethods.GetCommand(default!).Should().BeNull();
    }

    [Fact]
    public void GetRequiredCommandFromNull()
    {
        Func<CliCommand> act = () => ExtensionMethods.GetRequiredCommand(null!);
        _ = act.Should().Throw<ArgumentNullException>();
    }
}