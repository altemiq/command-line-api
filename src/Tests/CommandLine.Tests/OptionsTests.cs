// -----------------------------------------------------------------------
// <copyright file="OptionsTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

public class OptionsTests
{
    [Test]
    public async Task Verbosity()
    {
        VerbosityOption verbosity = new();
        await Assert.That(verbosity)
            .Satisfies(v => v.Recursive, recursive => recursive.IsTrue()).And
            .Satisfies(v => v.HasDefaultValue, hasDefaultValue => hasDefaultValue.IsTrue()).And
            .Satisfies(v => v.DefaultValueFactory, defaultValueFactory => defaultValueFactory.IsNotNull());
    }
}