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
        CliRootCommand command = [];
        _ = command.ConfigureHelp(b => builder = b);

        _ = builder.Should().NotBeNull();
    }

    [Fact]
    public void NoHelpAction()
    {
        Help.HelpBuilder? builder = default;
        CliRootCommand command = [];
        command.Options.Clear();
        command.Directives.Clear();

        _ = command.ConfigureHelp(b => builder = b);

        _ = builder.Should().BeNull();
    }

    [Fact]
    public void NoRootCommand()
    {
        Help.HelpBuilder? builder = default;
        CliCommand command = new(nameof(NoRootCommand));
        _ = command.ConfigureHelp(b => builder = b);

        _ = builder.Should().BeNull();
    }

    [Fact]
    public void NestedCommand()
    {
        Help.HelpBuilder? builder = default;
        CliCommand command = new(nameof(NestedCommand));
        CliRootCommand rootCommand =
        [
            command,
        ];

        _ = command.ConfigureHelp(b => builder = b);

        _ = builder.Should().NotBeNull();
    }
}