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
        _ = await Assert.That(new VerbosityOption())
            .Satisfies(verbosity => verbosity.Recursive, recursive => recursive.IsTrue()).And
            .Satisfies(verbosity => verbosity.HasDefaultValue, hasDefaultValue => hasDefaultValue.IsTrue()).And
            .Satisfies(verbosity => verbosity.DefaultValueFactory, defaultValueFactory => defaultValueFactory.IsNotNull());
    }
}