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
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
        _ = await Assert.That(new VerbosityOption())
            .Member(verbosity => verbosity.Recursive, recursive => recursive.IsTrue()).And
            .Member(verbosity => verbosity.HasDefaultValue, hasDefaultValue => hasDefaultValue.IsTrue()).And
            .Member(verbosity => verbosity.DefaultValueFactory, defaultValueFactory => defaultValueFactory.IsNotNull());
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
    }
}