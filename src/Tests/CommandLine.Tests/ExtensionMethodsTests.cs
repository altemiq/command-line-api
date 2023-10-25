// -----------------------------------------------------------------------
// <copyright file="ServiceExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Services;

public class ExtensionMethodsTests
{
    [Fact]
    public void GetRequiredValueFromOption()
    {
        var option = new CliOption<string>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });

        var parseResult = configuration.Parse("--option value");
        parseResult.GetRequiredValue(option).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromOptionWhenMissing()
    {
        var option = new CliOption<string>("--option");
        var configuration = new CliConfiguration(new CliRootCommand { option });

        configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(option)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetRequiredValueFromArgument()
    {
        var argument = new CliArgument<string>("ARG");
        var configuration = new CliConfiguration(new CliRootCommand { argument });

        var parseResult = configuration.Parse("value");
        parseResult.GetRequiredValue(argument).Should().Be("value");
    }

    [Fact]
    public void GetRequiredValueFromArgumentWhenMissing()
    {
        var argument = new CliArgument<string>("ARG") { DefaultValueFactory = _ => default! };
        var configuration = new CliConfiguration(new CliRootCommand { argument });

        configuration.Parse(string.Empty).Invoking(parseResult => parseResult.GetRequiredValue(argument)).Should().Throw<ArgumentNullException>();
    }
}
