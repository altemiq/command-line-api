// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Extension methods.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Gets the command from the symbol result.
    /// </summary>
    /// <param name="symbolResult">The symbol result.</param>
    /// <returns>The command if found.</returns>
    public static CliCommand? GetCommand(this Parsing.SymbolResult symbolResult)
    {
        return GetCommandCore(symbolResult);

        static CliCommand? GetCommandCore(Parsing.SymbolResult? symbolResult)
        {
            return symbolResult switch
            {
                { Parent: Parsing.CommandResult commandResult } => commandResult.Command,
                { Parent: Parsing.SymbolResult parentSymbolResult } => GetCommandCore(parentSymbolResult),
                _ => default,
            };
        }
    }

    /// <summary>
    /// Gets the command from the symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The command if found.</returns>
    public static CliCommand? GetCommand(this CliSymbol symbol)
    {
        return GetCommandCore(symbol);

        static CliCommand? GetCommandCore(CliSymbol symbol)
        {
            foreach (var parent in symbol.Parents)
            {
                if (parent is CliCommand command)
                {
                    return command;
                }

                if (GetCommand(parent) is { } parentCommand)
                {
                    return parentCommand;
                }
            }

            return default;
        }
    }
}