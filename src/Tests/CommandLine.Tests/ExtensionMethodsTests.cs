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
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = configuration.Parse("--option value").GetRequiredValue<string>("--option").Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOption()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = configuration.Parse("--option value").GetRequiredValue(option).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOptionWhenMissing()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(option)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredValueFromArgument()
    {
        Argument<string> argument = new("ARG");
        CommandLineConfiguration configuration = new(new RootCommand { argument });

        _ = configuration.Parse("value").GetRequiredValue(argument).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromArgumentWhenMissing()
    {
        Argument<string> argument = new("ARG") { DefaultValueFactory = _ => default! };
        CommandLineConfiguration configuration = new(new RootCommand { argument });

        _ = configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(argument)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredResultFromOption()
    {
        Option<string> option = new("--option");
        CommandLineConfiguration configuration = new(new RootCommand { option });

        _ = configuration.Parse("--option value").Invoking(parseResult => parseResult.GetRequiredResult(option)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromArgument()
    {
        Argument<string> argument = new("ARG");
        CommandLineConfiguration configuration = new(new RootCommand { argument });

        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(argument)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromCommand()
    {
        RootCommand command = [];
        CommandLineConfiguration configuration = new(command);

        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(command)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromDirective()
    {
        RootCommand command = [];
        CommandLineConfiguration configuration = new(command);

        Directive directive = command.Directives.First();
        _ = configuration.Parse($"[{directive.Name}]").Invoking(parseResult => parseResult.GetRequiredResult(directive)).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredResultFromSymbol()
    {
        RootCommand command = [];
        CommandLineConfiguration configuration = new(command);
        Symbol symbol = command;
        _ = configuration.Parse("value").Invoking(parseResult => parseResult.GetRequiredResult(symbol).Should()).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredCommandFromSymbolResult()
    {
        Option<string> option = new("--option");
        RootCommand command = [option];
        CommandLineConfiguration configuration = new(command);

        _ = configuration.Parse("--option value")
            .GetResult(option).Should().BeOfType<Parsing.OptionResult>()
            .Which.Invoking(result => result.GetRequiredCommand()).Should().NotThrow();
    }

    [Fact]
    public void GetRequiredCommandFromOption()
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
        _ = commandFromOptionDefault.Should().NotBeNull();
    }



    [Fact]
    public void GetRequiredCommandFromArgument()
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
        Func<Command> act = () => ExtensionMethods.GetRequiredCommand(null!);
        _ = act.Should().Throw<ArgumentNullException>();
    }
}