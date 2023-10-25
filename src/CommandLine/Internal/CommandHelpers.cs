// -----------------------------------------------------------------------
// <copyright file="CommandHelpers.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Internal;

/// <summary>
/// The <see cref="CliCommand"/> helpers.
/// </summary>
internal static class CommandHelpers
{
    /// <summary>
    /// Gets the root command from the symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The root command, if found; otherwise <see langword="null"/>.</returns>
    public static CliRootCommand? GetRootCommand(CliSymbol? symbol)
    {
        if (symbol is null)
        {
            return default;
        }

        if (symbol is CliRootCommand rootCommand)
        {
            return rootCommand;
        }

        foreach (var parent in symbol.Parents)
        {
            if (GetRootCommand(parent) is { } parentRootCommand)
            {
                return parentRootCommand;
            }
        }

        return default;
    }
}