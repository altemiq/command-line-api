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
        var option = new CliOption<string>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });

        _ = configuration.Parse("--option value").GetRequiredValue<string>("--option").Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOption()
    {
        var option = new CliOption<string>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });

        _ = configuration.Parse("--option value").GetRequiredValue(option).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOptionWhenMissing()
    {
        var option = new CliOption<string>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });

        _ = configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(option)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredValueFromArgument()
    {
        var argument = new CliArgument<string>("ARG");
        var configuration = new CliConfiguration(new CliRootCommand { argument });

        _ = configuration.Parse("value").GetRequiredValue(argument).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromArgumentWhenMissing()
    {
        var argument = new CliArgument<string>("ARG") { DefaultValueFactory = _ => default! };
        var configuration = new CliConfiguration(new CliRootCommand { argument });

        _ = configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(argument)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredResultFromOption()
    {
        var option = new CliOption<string>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });

        _ = configuration.Parse("--option value").Invoking(parseResult => parseResult.GetRequiredResult(option)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromArgument()
    {
        var argument = new CliArgument<string>("ARG");
        var configuration = new CliConfiguration(new CliRootCommand { argument });

        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(argument)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromCommand()
    {
        var command = new CliRootCommand();
        var configuration = new CliConfiguration(command);

        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(command)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromDirective()
    {
        var command = new CliRootCommand();
        var configuration = new CliConfiguration(command);

        var directive = command.Directives.First();
        _ = configuration.Parse($"[{directive.Name}]").Invoking(parseResult => parseResult.GetRequiredResult(directive)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromSymbol()
    {
        var command = new CliRootCommand();
        var configuration = new CliConfiguration(command);
        CliSymbol symbol = command;
        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(symbol).Should()).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredCommandFromSymbolResult()
    {
        var option = new CliOption<string>("--option");
        var command = new CliRootCommand { option };
        var configuration = new CliConfiguration(command);

        _ = configuration.Parse("--option value")
            .GetResult(option).Should().BeOfType<Parsing.OptionResult>()
            .Which.Invoking(result => result.GetRequiredCommand()).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredCommandFromOption()
    {
        CliCommand? commandFromOptionDefault = default;
        var option = new CliOption<string>("--api")
        {
            DefaultValueFactory = argumentResult =>
            {
                commandFromOptionDefault = argumentResult.GetCommand();
                return "This is the default value";
            },
        };
        var command = new CliRootCommand { option };
        var configuration = new CliConfiguration(command)
        {
            Output = TextWriter.Null,
            Error = TextWriter.Null,
        };

        var parsedConfiguration = configuration.Parse("--help");
        var result = parsedConfiguration.Invoke();
        _ = commandFromOptionDefault.Should().NotBeNull();
    }



    [Fact]
    public void GetRequiredCommandFromArgument()
    {
        CliCommand? commandFromOptionDefault = default;
        var argument = new CliArgument<string>("API")
        {
            DefaultValueFactory = argumentResult =>
            {
                commandFromOptionDefault = argumentResult.GetCommand();
                return "This is the default value";
            },
        };
        var command = new CliRootCommand { argument };
        var configuration = new CliConfiguration(command)
        {
            Output = TextWriter.Null,
            Error = TextWriter.Null,
        };

        var parsedConfiguration = configuration.Parse("--help");
        var result = parsedConfiguration.Invoke();
        _ = commandFromOptionDefault.Should().NotBeNull();
    }

    [Fact]
    public void GetCommandFromNull() => ExtensionMethods.GetCommand(default!).Should().BeNull();

    [Fact]
    public void GetRequiredCommandFromNull()
    {
        var act = () => ExtensionMethods.GetRequiredCommand(null!);
        _ = act.Should().Throw<ArgumentNullException>();
    }
}