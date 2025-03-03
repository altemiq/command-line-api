// -----------------------------------------------------------------------
// <copyright file="CommandHelpers.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Internal;

/// <summary>
/// The <see cref="Command"/> helpers.
/// </summary>
internal static class CommandHelpers
{
    /// <summary>
    /// Gets the root command from the symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The root command, if found; otherwise <see langword="null"/>.</returns>
    public static RootCommand? GetRootCommand(Symbol? symbol) => symbol switch
    {
        null => default,
        RootCommand rootCommand => rootCommand,
        { Parents: var parents } => parents.Select(GetRootCommand).FirstOrDefault(static p => p is not null),
    };
}