// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleTests.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleTests
{
    [Test]
    [MethodDataSource(nameof(GetValueOrDefaultData))]
    public async Task GetValueOrDefault(IAnsiConsole? input, IAnsiConsole expected, IAnsiConsole unexpected)
    {
        _ = await Assert.That(input.GetValueOrDefault()).IsSameReferenceAs(expected).And.IsNotSameReferenceAs(unexpected);
    }

    public static IEnumerable<Func<(IAnsiConsole? Input, IAnsiConsole Expected, IAnsiConsole? Unexpected)>> GetValueOrDefaultData()
    {
        yield return () =>
        {
            var console = global::Spectre.Console.AnsiConsole.Create(new AnsiConsoleSettings());
            return (console, console, global::Spectre.Console.AnsiConsole.Console);
        };
        yield return () => (default, global::Spectre.Console.AnsiConsole.Console, default);
    }
}
