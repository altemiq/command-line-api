// -----------------------------------------------------------------------
// <copyright file="OptionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class OptionsTests
{
    [Fact]
    public void Verbosity()
    {
        VerbosityOption verbosity = new();
        _ = verbosity.Recursive.Should().BeTrue();
        _ = verbosity.HasDefaultValue.Should().BeTrue();
        _ = verbosity.DefaultValueFactory.Should().NotBeNull();
    }
}