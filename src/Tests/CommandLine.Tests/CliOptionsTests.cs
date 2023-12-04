// -----------------------------------------------------------------------
// <copyright file="CliOptionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class CliOptionsTests
{
    [Fact]
    public void Verbosity()
    {
        var verbosity = CliOptions.VerbosityOption;
        verbosity.Recursive.Should().BeTrue();
        verbosity.HasDefaultValue.Should().BeTrue();
        verbosity.DefaultValueFactory.Should().NotBeNull();
    }
}