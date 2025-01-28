// -----------------------------------------------------------------------
// <copyright file="HelpConfigurationExtensionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class HelpConfigurationExtensionsTests
{
    [Test]
    public async Task CustomizeHelp()
    {
        RootCommand command = [];
        _ = await Assert.That(command.CustomizeHelp("first", "second", "default")).IsNotNull();
    }

    [Test]
    public async Task ConfigureHelp()
    {
        Help.HelpBuilder? builder = default;
        CommandLineConfiguration configuration = new(new RootCommand());
        _ = configuration.ConfigureHelp(b => builder = b);

        _ = await Assert.That(builder).IsNotNull();
    }
}