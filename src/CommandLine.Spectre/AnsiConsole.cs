// -----------------------------------------------------------------------
// <copyright file="AnsiConsole.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Helper functions for <see cref="global::Spectre.Console.AnsiConsole"/>.
/// </summary>
public static class AnsiConsole
{
    /// <summary>
    /// Gets the <see cref="IAnsiConsole"/> or <see cref="global::Spectre.Console.AnsiConsole.Console"/>.
    /// </summary>
    /// <param name="console">The console to check.</param>
    /// <returns>Either <paramref name="console"/> if not <see langword="null"/>; otherwise <see cref="global::Spectre.Console.AnsiConsole.Console"/>.</returns>
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static IAnsiConsole GetValueOrDefault(this IAnsiConsole? console) => GetConsoleOrDefault(console);

    /// <summary>
    /// Gets the <see cref="IAnsiConsole"/> or <see cref="global::Spectre.Console.AnsiConsole.Console"/>.
    /// </summary>
    /// <param name="console">The console to check.</param>
    /// <returns>Either <paramref name="console"/> if not <see langword="null"/>; otherwise <see cref="global::Spectre.Console.AnsiConsole.Console"/>.</returns>
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static IAnsiConsole GetConsoleOrDefault(IAnsiConsole? console) => console ?? global::Spectre.Console.AnsiConsole.Console;
}