// -----------------------------------------------------------------------
// <copyright file="HelpConfigurationExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class HelpConfigurationExtensionsTests
{
    [Fact]
    public void CustomizeHelp()
    {
        var command = new CliRootCommand();
        command.CustomizeHelp("first", "second", "default").Should().NotBeNull();
    }

    [Fact]
    public void ConfigureHelp()
    {
        Help.HelpBuilder? builder = default;
        var configuration = new CliConfiguration(new CliRootCommand());
        configuration.ConfigureHelp(b => builder = b);

        builder.Should().NotBeNull();
    }
}
