// -----------------------------------------------------------------------
// <copyright file="HelpCommandExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class HelpCommandExtensionsTests
{
    [Fact]
    public void ConfigureHelp()
    {
        Help.HelpBuilder? builder = default;
        var command = new CliRootCommand();
        command.ConfigureHelp(b => builder = b);

        builder.Should().NotBeNull();
    }

    [Fact]
    public void NoHelpAction()
    {
        Help.HelpBuilder? builder = default;
        var command = new CliRootCommand();
        command.Options.Clear();
        command.Directives.Clear();

        command.ConfigureHelp(b => builder = b);

        builder.Should().BeNull();
    }

    [Fact]
    public void NoRootCommand()
    {
        Help.HelpBuilder? builder = default;
        var command = new CliCommand(nameof(NoRootCommand));
        command.ConfigureHelp(b => builder = b);

        builder.Should().BeNull();
    }

    [Fact]
    public void NestedCommand()
    {
        Help.HelpBuilder? builder = default;
        var command = new CliCommand(nameof(NestedCommand));
        var rootCommand = new CliRootCommand
        {
            command,
        };

        command.ConfigureHelp(b => builder = b);

        builder.Should().NotBeNull();
    }
}
