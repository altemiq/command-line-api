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
        _ = verbosity.Recursive.Should().BeTrue();
        _ = verbosity.HasDefaultValue.Should().BeTrue();
        _ = verbosity.DefaultValueFactory.Should().NotBeNull();
    }
}